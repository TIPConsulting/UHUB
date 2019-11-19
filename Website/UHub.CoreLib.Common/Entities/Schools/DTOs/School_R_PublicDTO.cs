using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.DataInterop.Attributes;
using UHub.CoreLib.Entities.Schools.Interfaces;

namespace UHub.CoreLib.Entities.Schools.DTOs
{
    [DtoClass(typeof(School))]
    public sealed partial class School_R_PublicDTO : DtoEntityBase, ISchool_R_Public
    {
        public long? ID { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsReadOnly { get; set; }
        public string Description { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }
    }
}
