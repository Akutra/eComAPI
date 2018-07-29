using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace eComAPI.Models
{
    public class Annotations
    {
        public class AutoIncrementAttribute : Attribute
        {
            public AutoIncrementAttribute()
            {

            }
        }

        public class IndexAttribute : Attribute
        {
            public IndexAttribute()
            {

            }
        }

        [DebuggerStepThrough]
        public static void ArgumentNotNull(object argument, string argumentName)
        {
            //With Resharper add parameter information 
            //public static void ArgumentNotNull(object argument, [InvokerParameterName] string argumentName)

            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }
    }
}