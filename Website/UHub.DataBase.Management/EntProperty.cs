using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.DataInterop.Attributes;

namespace UHub.Database.Management
{
    [DataClass]
    public sealed partial class EntProperty : DBEntityBase
    {
        [DataProperty]
        public int ID { get; set; }

        [DataProperty]
        public string PropName { get; set; }

        [DataProperty]
        public string PropFriendlyName { get; set; }

        [DataProperty]
        public string Description { get; set; }

        [DataProperty]
        public short DataTypeID { get; set; }

        [DataProperty]
        public string DataType { get; set; }

        [DataProperty]
        public int? DefaultLength { get; set; }

        [DataProperty]
        public string DefaultPrecision { get; set; }

    }
}
