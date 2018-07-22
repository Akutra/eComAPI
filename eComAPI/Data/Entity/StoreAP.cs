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

    public class StoreAP<TEntity> : IEnumerable<TEntity>
    {
        //private string _idxHash = "";
        protected Dictionary<string, TEntity> _LocalStore = new Dictionary<string, TEntity>();
        private PropertyGroup _PropertyGroup = new PropertyGroup();

        //Expose the dictionary values with get only.
        public ICollection<TEntity> Values
        {
            get
            {
                return _LocalStore.Values.ToArray();
            }
        }

        public class PropertyContext
        {
            public Dictionary<object, string> _keys = new Dictionary<object, string>();
            public string Name = "";
            public bool isKey = false;
            public bool isRequired = false;
            public bool isAutoIncrement = false;
            public Type PropertyType = null;
        }

        public class PropertyGroup
        {
            public List<string> _keyFields = new List<string>();
            public Dictionary<string, PropertyContext> _fields = new Dictionary<string, PropertyContext>();
            public Dictionary<string, int> _autoFields = new Dictionary<string, int>();
            public List<string> _requiredFields = new List<string>();
        }

        public Type thisType = null;

        //public object Current { get; set; }

        #region Constructors
        public StoreAP()
        {
            thisType = typeof(TEntity);
            generatePropertyGroup();
        }
        #endregion

        private void generatePropertyGroup()
        {
            PropertyContext newProperty = null;
            var enumerator = ((IEnumerable<PropertyInfo>)thisType.GetProperties()).GetEnumerator();
            while (enumerator.MoveNext())
            {
                newProperty = new PropertyContext();
                var enum2 = enumerator.Current.CustomAttributes.GetEnumerator();
                while (enum2.MoveNext())
                {
                    if (enum2.Current.AttributeType.ToString().Contains("KeyAttribute"))
                    {
                        newProperty.isKey = true;
                        _PropertyGroup._keyFields.Add(enumerator.Current.Name);
                    }

                    if (enum2.Current.AttributeType.ToString().Contains("RequiredAttribute"))
                    {
                        newProperty.isRequired = true;
                        _PropertyGroup._requiredFields.Add(enumerator.Current.Name);
                    }

                    if (enum2.Current.AttributeType.ToString().Contains("AutoIncrementAttribute"))
                    {
                        newProperty.isAutoIncrement = true;
                        _PropertyGroup._autoFields.Add(enumerator.Current.Name, 0);
                    }
                }
                newProperty.Name = enumerator.Current.Name;
                newProperty.PropertyType = enumerator.Current.PropertyType;

                _PropertyGroup._fields.Add(newProperty.Name, newProperty);
            }
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
        public TEntity Attach(TEntity entity)
        {
            //use a simple for loop for better performance
            string newIdxHash = generateSignature(entity);
            int increment = 0;

            //validate required fields
            _PropertyGroup._requiredFields.ForEach(FieldName =>
            {
                //verify field content
                if (thisType.GetField(FieldName, BindingFlags.Public | BindingFlags.Instance) == null)
                    throw new ArgumentNullException(FieldName);

                if ((thisType.GetField(FieldName, BindingFlags.Public | BindingFlags.Instance).ToString()).Length <= 0)
                    throw new InvalidOperationException(FieldName + " cannot be empty.");

            });

            //add key fields
            _PropertyGroup._keyFields.ForEach(FieldName =>
            {
                //add key fields to their index array (dictionary)
                _PropertyGroup._fields[FieldName]._keys.Add(
                    thisType.GetField(FieldName, BindingFlags.Public | BindingFlags.Instance).GetValue(entity),
                    newIdxHash);
            });

            //increment autofields
            (new List<string>(_PropertyGroup._autoFields.Keys)).ForEach(FieldName =>
            {
                increment = 1; //TODO: create customizable value
                _PropertyGroup._autoFields[FieldName] += increment; //increment the counter

                //update object with new value
                thisType
                    .GetField(FieldName, BindingFlags.Public | BindingFlags.Instance)
                    .SetValue(entity, _PropertyGroup._autoFields[FieldName]);

            });


            _LocalStore.Add(newIdxHash, entity);
            //if (_LocalStore.Count == 1)
            //    this.Current = entity;

            return entity;
        }
        public TEntity Create()
        {
            return (TEntity)Activator.CreateInstance(typeof(TEntity)); ;
        }
        //public virtual TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, TEntity;
        public void RemoveRange(IEnumerable entities)
        {
            foreach (TEntity _entity in entities)
            {
                _LocalStore.Remove(generateSignature(_entity));
            }
        }
        public void Remove(TEntity entity)
        {
            RemoveRange(_LocalStore.Values.Where(_entity => _entity.Equals(entity)));
        }

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
                    return _LocalStore[(_PropertyGroup._fields[_PropertyGroup._keyFields[0]]._keys[_thisKey])];

                }
                catch (Exception _e) { } //Not found? Do nothing and return empty object, TODO add exception handling
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

                    return Task.FromResult(_LocalStore[(_PropertyGroup._fields[_PropertyGroup._keyFields[0]]._keys[_thisKey])]);

                }
                catch (Exception _e) { } //Not found? Do nothing and return empty object, TODO add exception handling
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
            return FindAsync(null, keyValues); //no need to duplicate code here.
        }

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

    }
}