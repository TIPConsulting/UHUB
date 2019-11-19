using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.DataInterop.Attributes
{
    /// <summary>
    /// Data attribute to designate DB autoload property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class DataPropertyAttribute : Attribute
    {
        /// <summary>
        /// DB native name to be set if the property name does not match the DB column name
        /// </summary>
        public string DBNativeName { get; set; }

        /// <summary>
        /// Enable checks for column validity when reading from DB
        /// </summary>
        public bool EnableDBColumnValidation { get; set; } = false;

        /// <summary>
        /// DataPropertyAttribute constructor
        /// </summary>
        public DataPropertyAttribute()
        {
            DBNativeName = null;
        }
    }
}
