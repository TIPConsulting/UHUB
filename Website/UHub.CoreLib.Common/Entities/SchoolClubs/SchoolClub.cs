using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.DataInterop.Attributes;
using UHub.CoreLib.Entities.SchoolClubs.Interfaces;

namespace UHub.CoreLib.Entities.SchoolClubs
{
    [DataClass]
    public sealed partial class SchoolClub : DBEntityBase, ISchoolClub_R_Public, ISchoolClub_C_Public
    {
        [DataProperty]
        public long? ID { get; set; }

        [DataProperty]
        public bool IsEnabled { get; set; }

        [DataProperty]
        public bool IsReadOnly { get; set; }

        [DataProperty]
        public string Name { get; set; }

        [DataProperty]
        public string Description { get; set; }

        [DataProperty(DBNativeName = "ParentID")]
        public long SchoolID { get; set; }

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
