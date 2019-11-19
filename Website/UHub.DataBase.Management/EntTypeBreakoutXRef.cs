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
    public sealed partial class EntTypeBreakoutXRef : DBEntityBase
    {
        [DataProperty]
        public short EntTypeID { get; set; }

        [DataProperty]
        public string TableSchema { get; set; }

        [DataProperty]
        public string TableName { get; set; }

        [DataProperty]
        public string  JoinType { get; set; }

        [DataProperty]
        public string OverrideComparer { get; set; }

        [DataProperty]
        public string ColumnWhiteList { get; set; }

        [DataProperty]
        public string ColumnBlackList { get; set; }

    }
}
