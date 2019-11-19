using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.DataInterop.Attributes;
using UHub.CoreLib.Entities.ClubModerators.Interfaces;

namespace UHub.CoreLib.Entities.ClubModerators
{
    [DataClass]
    public sealed partial class ClubModerator : DBEntityBase, IClubModerator
    {
        [DataProperty]
        public long? ID { get; set; }

        [DataProperty]
        public bool IsEnabled { get; set; }

        [DataProperty]
        public bool IsReadOnly { get; set; }

        [DataProperty]
        public long UserID { get; set; }

        [DataProperty]
        public bool IsOwner { get; set; }

        [DataProperty]
        public bool IsValid { get; set; }

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
