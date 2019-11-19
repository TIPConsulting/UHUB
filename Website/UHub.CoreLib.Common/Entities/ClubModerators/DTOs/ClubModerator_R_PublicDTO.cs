using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.DataInterop.Attributes;
using UHub.CoreLib.Entities.ClubModerators.Interfaces;

namespace UHub.CoreLib.Entities.ClubModerators.DTOs
{
    [DtoClass(typeof(ClubModerator))]
    public sealed partial class ClubModerator_R_PublicDTO : DtoEntityBase, IClubModerator_R_Public
    {
        public long UserID { get; set; }
        public bool IsOwner { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}
