using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Tools.Extensions
{
    public static class DynamicExtensions
    {
        public static bool HasProperty(dynamic Obj, string PropName)
        {

            if (Obj is ExpandoObject)
            {
                return ((IDictionary<string, object>)Obj).ContainsKey(PropName);
            }


            return Obj.GetProperty(PropName) != null;
        }
    }
}
