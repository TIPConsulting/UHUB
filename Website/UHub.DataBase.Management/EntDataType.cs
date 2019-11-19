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
    public sealed partial class EntDataType : DBEntityBase
    {
        [DataProperty]
        public short ID { get; set; }

        [DataProperty]
        public string Name { get; set; }

        [DataProperty]
        public int? DefaultLength { get; set; }

        [DataProperty]
        public string DefaultPrecision { get; set; }

    }
}
