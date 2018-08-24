using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eComAPI.Data.Entity
{
    public class Comparables
    {
        public class CInt : IEquatable<CInt>
        {
            private int _integer = 0;

            public CInt(int Value)
            {
                _integer = Value;
            }

            public int Value
            {
                get
                {
                    return _integer;
                }
            }

            public override int GetHashCode()
            {
                return this.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as CInt);
            }

            public bool Equals(CInt obj)
            {
                return obj != null && obj._integer == this._integer;
            }
        }

        public class CStr : IEquatable<CStr>
        {
            private string _string = "";

            public CStr(string Value)
            {
                _string = Value;
            }

            public string Value
            {
                get
                {
                    return _string;
                }
            }

            public override int GetHashCode()
            {
                return this.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as CStr);
            }

            public bool Equals(CStr obj)
            {
                return obj != null && obj._string == this._string;
            }
        }

        public class CLong : IEquatable<CLong>
        {
            private long _long = 0;

            public CLong(long Value)
            {
                _long = Value;
            }

            public long Value
            {
                get
                {
                    return _long;
                }
            }

            public override int GetHashCode()
            {
                return this.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as CLong);
            }

            public bool Equals(CLong obj)
            {
                return obj != null && obj._long == this._long;
            }
        }

        public class CDate : IEquatable<CDate>
        {
            private DateTime _date;

            public CDate(DateTime Value)
            {
                _date = Value;
            }

            public DateTime Value
            {
                get
                {
                    return _date;
                }
            }

            public override int GetHashCode()
            {
                return this.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as CDate);
            }

            public bool Equals(CDate obj)
            {
                return obj != null && obj._date == this._date;
            }
        }

        public static object getEquatable(object inputObject)
        {
            Type objType = inputObject.GetType();

            if(objType.Name.Contains("int"))
            {
                return new CInt((int)inputObject);
            }
            if (objType.Name.Contains("long"))
            {
                return new CLong((long)inputObject);
            }

            if (objType.Name.Contains("tring"))
            {
                return new CStr((string)inputObject);
            }

            if (objType.Name.Contains("DateTime"))
            {
                return new CDate((DateTime)inputObject);
            }

            return inputObject; //unspecified type, user responsible for equals.
        }
    }
}