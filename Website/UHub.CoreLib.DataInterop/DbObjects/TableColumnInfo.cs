using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop.Attributes;

namespace UHub.CoreLib.DataInterop
{
    [DataClass]
    public sealed partial class TableColumnInfo : DBEntityBase
    {
        [DataProperty(DBNativeName = "TABLE_SCHEMA")]
        public string TableSchema { get; set; }


        [DataProperty(DBNativeName = "TABLE_NAME")]
        public string TableName { get; set; }


        [DataProperty(DBNativeName = "COLUMN_NAME")]
        public string ColumnName { get; set; }


        [DataProperty(DBNativeName = "ORDINAL_POSITION")]
        public int OrdinalPosition { get; set; }


        [DataProperty(DBNativeName = "DATA_TYPE")]
        public string DataType { get; set; }
    }
}
