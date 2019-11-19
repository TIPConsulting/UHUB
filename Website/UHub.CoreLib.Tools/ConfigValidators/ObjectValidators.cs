using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Tools
{
    public static partial class ConfigValidators
    {

        /// <summary>
        /// Ensure object is not null
        /// </summary>
        /// <param name="obj">Object to test</param>
        /// <param name="argName">Name of object variable</param>
        public static void ValidateObject(object obj, string argName)
        {
            if (obj == null)
            {
                throw new ArgumentException(argName + " cannot be null.");
            }
        }

    }
}
