using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.DataInterop.Attributes
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class DtoClassAttribute : Attribute
    {

        public Type ConversionType { get; }


        public DtoClassAttribute(Type ConversionType)
        {
            this.ConversionType = ConversionType;
        }
    }
}
