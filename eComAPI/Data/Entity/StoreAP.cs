using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary; //for binary serializer
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace eComAPI.Data.Entity
{
    public enum EntityState
    {
        Detached = 1,
        Unchanged = 2,
        Added = 4,
        Deleted = 8,
        Modified = 16
    }

    public class QEntities<TEntity> : ICollection, IEnumerator, IEnumerable
    {
        private Dictionary<object, TEntity> _entities = new Dictionary<object, TEntity>();
        //private TEntity _current_entity;
        private long _current_position = 0;

        //TODO
        public QEntities(IEnumerable<TEntity> collection)
        {
            AddRange(collection);
        }

        public int AddRange(IEnumerable<TEntity> collection)
        {
            int rt = 0;
            foreach (TEntity _e in collection)
            {
                rt += Add(_e) ? 1 : 0;
            }
            return rt;
        }

        public bool Add(TEntity entity)
        {
            var _key = getKeyValue(entity);

            if (_key != null)
            {
                _entities.Add(_key, entity); return true;
            }
            return false;
        }

        public object getKeyValue(TEntity entity)
        {
            Type _thisType = typeof(TEntity);

            foreach(PropertyInfo property in _thisType.GetProperties() )
            {
                if (property.CustomAttributes.Where(a => a.AttributeType.Name == "KeyAttribute").Select(a => a.AttributeType.Name ).Count() > 0 )
                {
                    return property.GetValue(entity);
                }
            }
            return null;
        }

        public int Count
        {
            get
            {
                return _entities.Count();
            }
        }

        public object Current
        {
            get
            {
                return _entities.Values.ToArray()[_current_position];
            }
        }

        //TODO
        public object SyncRoot => throw new NotImplementedException();
        //TODO
        public bool IsSynchronized => throw new NotImplementedException();
        //TODO
        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            return _entities.Values.GetEnumerator();
        }

        public bool MoveNext()
        {
            if(_current_position+1<_entities.Values.Count) // if we can move next do it
            {
                _current_position++;
                return true; //return success
            }
            return false; //return false
        }

        public void Reset()
        {
            _current_position = 0;
        }

        public TEntity this[object index]
        {
            get
            {
                return _entities[index];
            }
        }

        //TODO: Where
        public IEnumerable<TEntity> Where(Func<TEntity, bool> predicate)
        {
            foreach (var item in _entities.Values)
            {
                if (predicate(item))
                {
                    yield return item;
                }
            }
        }

        //TODO: Select
        public IEnumerable<TEntity> Select(Func<TEntity, bool> predicate)
        {
            foreach (var item in _entities.Values)
            {
                if ( predicate(item) )
                {
                    yield return item;
                }
            }
        }

    }
    public class QPropertyContext
    {
        public Dictionary<object, object> _keys = new Dictionary<object, object>();
        public string Name = "";
        public bool isKey = false;
        public bool isIndexed = false;
        public bool isRequired = false;
        public bool isAutoIncrement = false;
        public Type PropertyType = null;
    }

    public class QPropertyGroup
    {
        public List<string> _IndexFields = new List<string>();
        public Dictionary<string, QPropertyContext> _fields = new Dictionary<string, QPropertyContext>();
        public List<string> _requiredFields = new List<string>();
        public Dictionary<string, int> _autoFields = new Dictionary<string, int>();
        public long autoIndex = 0;
        public string KeyField = "";

        public Type ThisType { get; private set; }

        public QPropertyGroup(Type EntityType)
        {
            ThisType = EntityType;
            generatePropertyGroup();
        }
        private void generatePropertyGroup()
        {
            QPropertyContext newProperty = null;
            var enumerator = ((IEnumerable<PropertyInfo>)ThisType.GetProperties()).GetEnumerator();
            while (enumerator.MoveNext())
            {
                newProperty = new QPropertyContext();
                var enum2 = enumerator.Current.CustomAttributes.GetEnumerator();
                while (enum2.MoveNext())
                {
                    if (enum2.Current.AttributeType.ToString().Contains("KeyAttribute"))
                    {
                        newProperty.isKey = true;
                        KeyField = enumerator.Current.Name;
                    }

                    if (enum2.Current.AttributeType.ToString().Contains("IndexAttribute"))
                    {
                        newProperty.isIndexed = true;
                        _IndexFields.Add(enumerator.Current.Name);
                    }

                    if (enum2.Current.AttributeType.ToString().Contains("RequiredAttribute"))
                    {
                        newProperty.isRequired = true;
                        _requiredFields.Add(enumerator.Current.Name);
                    }

                    if (enum2.Current.AttributeType.ToString().Contains("AutoIncrementAttribute"))
                    {
                        newProperty.isAutoIncrement = true;
                        _autoFields.Add(enumerator.Current.Name, 0);
                    }
                }
                newProperty.Name = enumerator.Current.Name;
                newProperty.PropertyType = enumerator.Current.PropertyType;

                _fields.Add(newProperty.Name, newProperty);
            }
        }
    }

    public class StoreAP<TEntity> : IEnumerable<TEntity>, IEnumerator<TEntity>, ICollection<TEntity>
    {
        //private string _idxHash = "";
        //internal APIPropertyGroup _PropertyGroup;
        internal Dictionary<object, TEntity> _LocalStore = new Dictionary<object, TEntity>();
        internal List<APIPropertyGroup> _CurrentValues = new List<APIPropertyGroup>();
        internal QPropertyGroup _QuantumStruct;
        internal uint position = 0;

        //Expose the dictionary values with get only.
        public ICollection<TEntity> Values
        {
            get
            {
                return _LocalStore.Values.ToArray();
            }
        }

        public bool Contains(TEntity entity)
        {
            return _LocalStore.Values.Contains(entity);
        }

        public int Count
        {
            get
            {
                return _LocalStore.Values.Count();
            }
        }

        public bool MoveNext()
        {
            if( position < _LocalStore.Values.Count() )
            {
                position++;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            position = 0;
        }

        public void Clear()
        {
            //TODO: Delete all records.
            throw new NotImplementedException();
        }

        public Type thisType { get; } // public read only

        //TODO
        public bool IsReadOnly => false;

        object IEnumerator.Current => this.Current; //Simple forward

        public TEntity Current {
            get
            {
                if( _LocalStore.Values.Count() > 0 )
                    return _LocalStore.Values.ToArray()[position];
                else
                {
                    return Create();
                }
            }
        }

        #region Constructors
        public StoreAP()
        {
            //Current = Create(); 
            thisType = Current.GetType();
            _QuantumStruct = new QPropertyGroup(thisType);
        }
        #endregion

        #region DB Creation
        void ICollection<TEntity>.Add(TEntity entity)
        {
            // Add without the return
            // auto-populated fields still process

            this.Add(entity);
        }
        public TEntity Add(TEntity entity)
        {
            //TODO Add DataBase functionality
            return Attach(entity);
        }
        public IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities)
        {
            foreach (TEntity item in entities)
            {
                Attach(item);
            }

            return null;
        }
        public TEntity Attach(TEntity entity, bool autofields = true)
        {
            //use a simple for loop for better performance
            //string newIdxHash = generateSignature(entity);
            int increment = 0;

            //increment autofields
            (new List<string>(_QuantumStruct._autoFields.Keys)).ForEach(FieldName =>
            {
                increment = 1; //TODO: create customizable value

                //create a new counter reference if missing
                if (!_QuantumStruct._autoFields.ContainsKey(FieldName))
                    _QuantumStruct._autoFields.Add(FieldName, 0);

                //increment counter
                _QuantumStruct._autoFields[FieldName] += increment; //increment the counter

                //update object with new value
                thisType
                    .GetProperty(FieldName)
                    .SetValue(entity, _QuantumStruct._autoFields[FieldName]);

            });

            //validate required fields
            _QuantumStruct._requiredFields.ForEach(FieldName =>
            {
                //verify field content
                if (thisType.GetProperty(FieldName).GetValue(entity) == null)
                    throw new ArgumentNullException(FieldName + " cannot be null.");

                if ((thisType.GetProperty(FieldName).GetValue(entity).ToString()).Length <= 0)
                    throw new InvalidOperationException(FieldName + " cannot be empty.");

            });

            //add indexed fields
            _QuantumStruct._IndexFields.ForEach(FieldName =>
            {
                //add key fields to their index array (dictionary)
                _QuantumStruct._fields[FieldName]._keys.Add(
                    eComAPI.Data.Entity.Comparables.getEquatable(thisType.GetProperty(FieldName).GetValue(entity)),
                    thisType.GetProperty(_QuantumStruct.KeyField).GetValue(entity));
            });

            _LocalStore.Add(eComAPI.Data.Entity.Comparables.getEquatable(thisType.GetProperty(_QuantumStruct.KeyField).GetValue(entity)), entity);
            //if (_LocalStore.Count == 1)
            //    this.Current = entity;

            return entity;
        }
        public TEntity Create()
        {
            return (TEntity)Activator.CreateInstance(typeof(TEntity));
        }
        //public virtual TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, TEntity;
        #endregion

        #region Updaters
        //this could possibly be Add-or-Update, TODO: make sure it would be ok to Add if not found.
        public void Update(TEntity entity, object idxField = null)
        {
            object idxHash = idxField;
            object idxOrig = thisType.GetProperty(_QuantumStruct.KeyField).GetValue(entity);
            if (idxHash == null) { idxHash = idxOrig; }//get the object hash for indexing
            TEntity OrigEntity;
            idxHash = eComAPI.Data.Entity.Comparables.getEquatable(idxHash); //ensure dictionary can handle the key object.

            //remove the object if found
            if (_LocalStore.ContainsKey(idxHash))
            {
                if( idxOrig.Equals(idxHash) )
                {
                    OrigEntity = _LocalStore[idxHash];
                    reviseObj(ref OrigEntity, ref entity);
                } else
                {
                    //Remove and add it again if the KeyField has changed
                    Remove(idxHash);
                    //Add it to the data structure, but do not increment autoincrement fields
                    Attach(entity, false);
                }

            } else
            {
                throw new KeyNotFoundException("Object that doesn't exist cannot be updated.");
            }

        }

        public void reviseObj(ref TEntity cEntity, ref TEntity nEntity)
        {
            foreach(QPropertyContext property in _QuantumStruct._fields.Values)
            {
                if (thisType.GetProperty(property.Name).GetValue(cEntity) != thisType.GetProperty(property.Name).GetValue(nEntity))
                {
                    thisType
                        .GetProperty(property.Name)
                        .SetValue(cEntity, thisType.GetProperty(property.Name).GetValue(nEntity));
                }
            }
        }

        public static void SetFields(string propertyName, ref TEntity orgEntity, ref TEntity newEntity)
        {
            Type _thisType = typeof(TEntity);

            var cValue = _thisType.GetProperty(propertyName).GetValue(orgEntity);
            var nValue = _thisType.GetProperty(propertyName).GetValue(newEntity);

            if (_thisType.GetProperty(propertyName).GetValue(orgEntity) != _thisType.GetProperty(propertyName).GetValue(newEntity))
            {
                _thisType
                    .GetProperty(propertyName)
                    .SetValue(orgEntity, _thisType.GetProperty(propertyName).GetValue(newEntity));
            }
        }
        #endregion

        #region Removers
        public void RemoveRange(IEnumerable entities)
        {
            //remove each.
            foreach (TEntity _entity in entities)
            {
                Remove(thisType.GetProperty(_QuantumStruct.KeyField).GetValue(_entity)); //get a hash for the remover.
            }
        }
        public bool Remove(TEntity entity)
        {
            return Remove(thisType.GetProperty(_QuantumStruct.KeyField).GetValue(entity)); //get a hash for the remover.
        }

        public bool Remove(object indexHash)
        {
            if (_LocalStore.Remove(indexHash)) //remove object
            {
                //remove the indices for the object
                _QuantumStruct._IndexFields.ForEach(FieldName =>
                {
                    if (_QuantumStruct._fields[FieldName]._keys.ContainsKey(indexHash))
                    {
                        _QuantumStruct._fields[FieldName]._keys.Remove(indexHash);
                    }
                    
                });
                return true; //remove completed.
            }
            return false; //remove did not succeed.
        }
        #endregion

        #region DB Retriever
        /// <summary>
        /// Classic 'DBSet' style find...
        /// </summary>
        /// <param name="keyValues">Index field value(s) to look up. Use multiple values for composite keys (not yet implemented)</param>
        /// <returns>An object if found otherwise an empty object</returns>
        public TEntity Find(params object[] keyValues)
        {
            object _thisKey = null;

            if (keyValues.Length > 0)
            {
                _thisKey = keyValues[0];

                if (keyValues.Length > 1)
                {
                    //TODO : Composite Keys
                    throw new NotImplementedException("Composite Keys not yet implemented.");
                }

                try
                {
                    //try to locate indexed object and return it
                    //on error return empty object (default)
                    return _LocalStore[eComAPI.Data.Entity.Comparables.getEquatable(_QuantumStruct._fields[_QuantumStruct.KeyField]._keys[_thisKey])];

                }
                catch (Exception _e) { } //Not found? Do nothing and return empty object, TODO add more handling
            }

            return default(TEntity);
        }

        /*
        /// <summary>
        /// Advanced Find for indexed fields and non-indexed fields
        /// </summary>
        /// <param name="FindObject">Create an object and ONLY populate the fields with search values leaving field you don't need to search empty.</param>
        /// <returns>The matching object</returns>
        public TEntity Find(TEntity FindObject)
        {

            return default(TEntity);
        }

        public TEntity Match(TEntity MatchObject)
        {

        }*/

        /// <summary>
        /// Classic Async Find
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to allow cancellation</param>
        /// <param name="keyValues">Index field value(s) to look up. Use multiple values for composite keys (not yet implemented)</param>
        /// <returns>An object if found otherwise an empty object</returns>
        public Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            object _thisKey = null;

            if (keyValues.Length > 0)
            {
                _thisKey = keyValues[0];

                if (keyValues.Length > 1)
                {
                    //TODO : Composite Keys
                    throw new NotImplementedException("Composite Keys not yet implemented.");
                }

                try
                {
                    //try to locate indexed object and return it
                    //on error return empty object (default)
                    cancellationToken.ThrowIfCancellationRequested(); //allow it to be cancelled.

                    //var _kfv = _QuantumStruct._fields[_QuantumStruct.KeyField]._keys[_thisKey];
                    //var _obj = eComAPI.Data.Entity.Comparables.getEquatable(_kfv);
                    if(_LocalStore.ContainsKey(_thisKey))
                        return Task.FromResult(_LocalStore[_thisKey]);


                    //return Task.FromResult(_LocalStore[_thisKey]);

                }
                catch (Exception _e) { } //Not found? Do nothing and return empty object, TODO more handling
            }

            return Task.FromResult(default(TEntity));
        }

        /// <summary>
        /// Classic Async Find
        /// </summary>
        /// <param name="keyValues">Index field value(s) to look up. Use multiple values for composite keys (not yet implemented)</param>
        /// <returns>An object if found otherwise an empty object</returns>
        public Task<TEntity> FindAsync(params object[] keyValues)
        {
            return FindAsync(new CancellationToken(false), keyValues); //no need to duplicate code here.
        }
        #endregion

        #region Serializers and Encoders
        public string generateSignature(object SerialObj)
        {
            MemoryStream mStream = new MemoryStream();

            //TODO create simple serializer.
            new BinaryFormatter().Serialize(mStream, SerialObj);

            return generateSignature(mStream);

        }
        public string generateSignature(string ObjString)
        {
            //create a memory stream out of the string
            MemoryStream mStream = new MemoryStream(Encoding.GetEncoding("utf-8").GetBytes(ObjString));

            return generateSignature(mStream);
        }
        public string generateSignature(MemoryStream ObjStream)
        {
            ObjStream.Position = 0;

            int csum = 0;

            //read the memory stream and add up the checksum
            while (ObjStream.CanRead)
            {
                csum += ObjStream.ReadByte();
            }

            //finalize and return hexadecimal
            csum &= 0xff;
            return csum.ToString("X2");
        }
        #endregion

        #region Enumerators
        public IEnumerator<TEntity> GetEnumerator()
        {
            return _LocalStore.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _LocalStore.Values.GetEnumerator();
        }
        #endregion

        #region Delegates
        //Delegates
        internal delegate void UpdateAccessor(TEntity entity, object idxField = null);
        internal delegate bool UpdateFieldAccessor(string FieldName, object FieldValue);
        internal delegate void UpdateStateAccessor(EntityState newState);
        #endregion

        public void CopyTo(TEntity[] array, int arrayIndex)
        {
            //TODO Replicate to an Array
            throw new NotImplementedException();
        }

        #region API Entry
        //internal APIEntityEntry<TEntity> _currentEntry = new APIEntityEntry<TEntity>();
        public eComAPI.Data.Entity.StoreAP<TEntity>.APIEntityEntry Entry(TEntity SelectedEntity)
        {
            //return new eComAPI.Data.Entity.StoreAP<TEntity>.APIEntityEntry<TEntity>();
            APIEntityEntry newEntry = new APIEntityEntry(SelectedEntity);

            //newEntry.updater = this.Update;
            //_currentEntry.SetEntity(SelectedEntity);
            return newEntry;
        }

        public eComAPI.Data.Entity.StoreAP<TEntity>.APIEntityEntry Entry()
        {
            //return new eComAPI.Data.Entity.StoreAP<TEntity>.APIEntityEntry<TEntity>();
            APIEntityEntry newEntry = new APIEntityEntry(Current);

            //newEntry.updater = this.Update;
            //_currentEntry.SetEntity(SelectedEntity);
            return newEntry;
        }

        public class APIEntityEntry
        {
            //TODO: State change for DB activity.
            public Data.Entity.EntityState State { get; set; }
            //internal UpdateAccessor updater;

            private APIPropertyGroup _originalvalues;
            private APIPropertyGroup _currentvalues;
            //private APIPropertyGroup _laststorevalues;

            private TEntity _current_entity;

            internal bool UpdateField(string FieldName, object FieldValue)
            {
                if( _originalvalues[FieldName]!=FieldValue)
                {
                    _currentvalues[FieldName] = FieldValue;
                    State = EntityState.Modified;
                    return true;
                }
                return false;
            }

            internal void UpdateState(EntityState newState)
            {
                State = newState;
            }

            public APIPropertyGroup CurrentValues
            {
                get { return _currentvalues; }
                set { _currentvalues = value; }
            }

            public APIPropertyGroup OriginalValues
            {
                get { return _originalvalues; }
                set { _originalvalues = value; }
            }

            public APIEntityEntry(TEntity entryEntity)
            {
                SetEntity(ref entryEntity);
                //_current_entity = entryEntity;
                //_currentvalues = new APIPropertyGroup(ref _current_entity);

            }

            public void SetEntity(ref TEntity newEntity)
            {
                _current_entity = newEntity;
                _currentvalues = new APIPropertyGroup(ref _current_entity);
                _currentvalues.stateupdater = UpdateState; //primarily for the state change update

                TEntity _newEntity = (TEntity)Activator.CreateInstance(typeof(TEntity));
                Type _thisType = typeof(TEntity);

                var enumerator = ((IEnumerable<PropertyInfo>)_thisType.GetProperties()).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    _thisType
                        .GetProperty(enumerator.Current.Name)
                        .SetValue(_newEntity, _thisType.GetProperty(enumerator.Current.Name).GetValue(_current_entity));
                }
                _originalvalues = new APIPropertyGroup(ref _newEntity);
                _originalvalues.stateupdater = UpdateState; //primarily for the state change update
                State = EntityState.Unchanged;

            }

            public APIPropertyContext Property(string propertyName)
            {
                APIPropertyContext _newPropContext = new APIPropertyContext(ref _current_entity);
                _newPropContext._thisFieldName = propertyName;
                _newPropContext.SetField(_current_entity.GetType().GetProperty(propertyName));
                _newPropContext.fieldupdater = UpdateField;
                return _newPropContext;
            }
        }
        #endregion

        #region API Properties
        /// <summary>
        /// Properties collection
        /// </summary>
        public class APIPropertyGroup //: StoreAP<TEntity>
        {
            private TEntity _CurrentEntity;

            private Type thisType = null;

            public APIPropertyGroup(ref TEntity CurrentEntity)
            {
                _CurrentEntity = CurrentEntity;
                thisType = _CurrentEntity.GetType();
                
            }

            internal UpdateStateAccessor stateupdater;
            
            /*
            public APIPropertyValues()
            {
                _CurrentEntity = Current;
            }*/

            /*
            public void SetValues(object newValues)
            {

            }*/

            public object this[string propertyName]
            {
                get
                {
                    return thisType.GetProperty(propertyName).GetValue(_CurrentEntity);
                }
                set
                {
                    thisType
                        .GetProperty(propertyName)
                        .SetValue(_CurrentEntity, value);
                    stateupdater(EntityState.Modified); // primarily for state change update.
                }
            }
        }

        public class APIPropertyContext
        {
            private TEntity _CurrentEntity;
            public Type thisType { get; private set; }

            private PropertyInfo _thisField;

            internal void SetField(PropertyInfo currentFieldInfo)
            {
                thisType = currentFieldInfo.PropertyType;
                _thisField = currentFieldInfo;
                _thisFieldName = currentFieldInfo.Name;
                //fieldupdater(_thisFieldName, _thisField.GetValue(_CurrentEntity));
            }

            internal string _thisFieldName = "";

            internal UpdateFieldAccessor fieldupdater;

            public object CurrentValue
            {
                get
                {
                    return _thisField.GetValue(_CurrentEntity);
                }
                set
                {
                    if (!_thisField.GetValue(_CurrentEntity).Equals(value))
                    {
                        _thisField.SetValue(_CurrentEntity, value);
                        isModified = fieldupdater(_thisFieldName, value); //ensure state change status has applied.
                        
                    }
                }
            }

            public bool isModified { get; set; }
            public string PropertyName {
                get
                    {
                    return _thisFieldName;
                    }
             }

            public APIPropertyContext(ref TEntity CurrentEntity)
            {
                _CurrentEntity = CurrentEntity;
                thisType = CurrentEntity.GetType();
                isModified = false;
            }


        }
        #endregion

        #region Dispose
        private bool disposed = false; // to detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //nothing here for the moment...
                }

                disposed = true;
            }
        }

        ~StoreAP()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}