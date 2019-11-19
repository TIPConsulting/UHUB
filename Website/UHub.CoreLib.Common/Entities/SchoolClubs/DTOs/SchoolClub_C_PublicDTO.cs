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
    public sealed partial class SchoolClub_C_PublicDTO : DtoEntityBase, ISchoolClub_C_Public
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
