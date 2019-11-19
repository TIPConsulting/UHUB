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
    public sealed partial class EntPropertyRevisionMap : DBEntityBase
    {
        [DataProperty]
        public short EntTypeID { get; set; }

        [DataProperty]
        public int PropID { get; set; }

        [DataProperty]
        public string Description { get; set; }

    }
}
