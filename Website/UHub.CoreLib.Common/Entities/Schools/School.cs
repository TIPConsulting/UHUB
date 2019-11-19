using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.DataInterop.Attributes;
using UHub.CoreLib.Entities.Schools.Interfaces;


namespace UHub.CoreLib.Entities.Schools
{
    [DataClass]
    public sealed partial class School : DBEntityBase, ISchool_R_Public
    {
        [DataProperty]
        public long? ID { get; set; }

        [DataProperty]
        public string Name { get; set; }

        [DataProperty]
        public string State { get; set; }

        [DataProperty]
        public string City { get; set; }

        [DataProperty]
        public bool IsEnabled { get; set; }

        [DataProperty]
        public bool IsReadOnly { get; set; }

        [DataProperty]
        public string Description { get; set; }

        [DataProperty]
        public string DomainValidator { get; private set; }

        [DataProperty]
        public long CreatedBy { get; set; }

        [DataProperty]
        public DateTimeOffset CreatedDate { get; set; }

        [DataProperty]
        public long? ModifiedBy { get; set; }

        [DataProperty]
        public DateTimeOffset? ModifiedDate { get; set; }

    }
}
