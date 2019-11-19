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
    public sealed partial class EntChildMap : DBEntityBase
    {
        [DataProperty]
        public short ParentEntType { get; set; }

        [DataProperty]
        public short ChildEntType { get; set; }

        [DataProperty]
        public string Description { get; set; }


    }
}
