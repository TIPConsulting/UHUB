using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Tools.Extensions
{
    public static class PropertyInfoExtensions
    {
        /// <summary>
        /// Get property value from PropertyInfo (returns value or default)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <param name="obj">Null object will provide value of static property</param>
        /// <returns></returns>
        public static T GetValue<T>(this PropertyInfo property, object obj = null)
        {
            var temp = property.GetValue(obj);

            if (temp.GetType() == typeof(T))
            {
                return (T)temp;
            }
            else
            {
                return default(T);
            }
        }
    }
}
