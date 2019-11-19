using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop.Attributes;

namespace UHub.CoreLib.DataInterop
{
    [DataClass]
    public sealed partial class TableRelationshipInfo : DBEntityBase
    {
        [DataProperty]
        public string FK_Schema { get; set; }

        [DataProperty]
        public string FK_Table { get; set; }

        [DataProperty]
        public string FK_Column { get; set; }

        [DataProperty]
        public string PK_Schema { get; set; }

        [DataProperty]
        public string PK_Table { get; set; }

        [DataProperty]
        public string PK_Column { get; set; }

        [DataProperty]
        public string Constraint_Name { get; set; }


    }
}
