using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.DataInterop.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class DataClassAttribute : Attribute
    {
        /// <summary>
        /// Enable checks for column validity when reading from DB
        /// </summary>
        public bool EnableDBColumnValidation { get; private set; } = false;


        public DataClassAttribute()
        {

        }


        public DataClassAttribute(bool EnableDBColumnValidation)
        {
            this.EnableDBColumnValidation = EnableDBColumnValidation;
        }

    }
}
