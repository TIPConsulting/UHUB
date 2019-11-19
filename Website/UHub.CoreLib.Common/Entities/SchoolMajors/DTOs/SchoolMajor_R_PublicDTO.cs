using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.DataInterop.Attributes;
using UHub.CoreLib.Entities.SchoolMajors.Interfaces;

namespace UHub.CoreLib.Entities.SchoolMajors.DTOs
{
    [DtoClass(typeof(SchoolMajor))]
    public sealed partial class SchoolMajor_R_PublicDTO : DtoEntityBase, ISchoolMajor_R_Public
    {
        public string Name { get; set; }
        public bool IsEnabled { get; set; }
        public string Description { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }
    }
}
