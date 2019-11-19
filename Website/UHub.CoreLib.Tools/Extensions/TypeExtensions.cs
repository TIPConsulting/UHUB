using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Tools.Extensions
{
    public static class TypeExtensions
    {
        static bool IsSameOrSubclass(this Type class1, Type class2)
        {

            return class1.IsSubclassOf(class2) || class1 == class2;

        }

    }
}
