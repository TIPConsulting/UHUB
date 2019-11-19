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
    public sealed partial class EntType : DBEntityBase
    {
        [DataProperty]
        public short ID { get; set; }

        [DataProperty]
        public Guid UID { get; set; }

        [DataProperty]
        public string Name { get; set; }

        [DataProperty]
        public string Description { get; set; }

        [DataProperty]
        public string AutoViewSchema { get; set; }

        [DataProperty]
        public string AutoViewName { get; set; }

    }
}
