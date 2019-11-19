using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.DataInterop.Attributes;

namespace UHub.Database.Management
{
    [DataClass]
    public sealed partial class EntPropertyMap : DBEntityBase
    {
        [DataProperty]
        public short EntTypeID { get; set; }

        [DataProperty]
        public int PropID { get; set; }

        [DataProperty]
        public short DataTypeID { get; set; }

        [DataProperty]
        public int? OverrideLength { get; set; }

        [DataProperty]
        public string OverridePrecision { get; set; }

        [DataProperty]
        public bool IsNullable { get; set; }

        [DataProperty]
        public string DefaultValue { get; set; }

        [DataProperty]
        public string Description { get; set; }
    }
}
