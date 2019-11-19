using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.DataInterop.Attributes;
using UHub.CoreLib.Entities.SchoolClubs.Interfaces;

namespace UHub.CoreLib.Entities.SchoolClubs.DTOs
{
    [DtoClass(typeof(SchoolClub))]
    public sealed partial class SchoolClub_R_PublicDTO : DtoEntityBase, ISchoolClub_R_Public
    {
        public long? ID { get; set; }
        public long SchoolID { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsReadOnly { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }
    }
}
