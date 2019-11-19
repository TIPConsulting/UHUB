using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.DataInterop.Attributes;

namespace UHub.CoreLib.Entities.Posts
{
    [DataClass]
    public sealed partial class PostClusteredCount : DBEntityBase
    {
        [DataProperty]
        public long SchoolID { get; set; }

        [DataProperty]
        public long? SchoolClubID { get; set; }

        [DataProperty]
        public long PublicPostCount { get; set; }

        [DataProperty]
        public long PrivatePostCount { get; set; }

    }
}
