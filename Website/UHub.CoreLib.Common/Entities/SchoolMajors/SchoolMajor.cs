using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.DataInterop.Attributes;
using UHub.CoreLib.Entities.SchoolMajors.Interfaces;

namespace UHub.CoreLib.Entities.SchoolMajors
{
    [DataClass]
    public sealed partial class SchoolMajor : DBEntityBase, ISchoolMajor_R_Public
    {
        [DataProperty]
        public long? ID { get; set; }

        [DataProperty]
        public string Name { get; set; }

        [DataProperty]
        public bool IsEnabled { get; set; }

        [DataProperty]
        public bool IsReadonly { get; set; }

        [DataProperty]
        public string Description { get; set; }

        [DataProperty]
        public bool IsDeleted { get; set; }

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
