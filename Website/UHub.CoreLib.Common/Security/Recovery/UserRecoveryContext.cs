using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.DataInterop.Attributes;
using UHub.CoreLib.Security;
using UHub.CoreLib.Entities.Users.Interfaces;
using UHub.CoreLib.Security.Accounts;

namespace UHub.CoreLib.Entities.Users
{
    [DataClass]
    public sealed partial class UserRecoveryContext : DBEntityBase, IUserRecoveryContext
    {


        [DataProperty]
        public long UserID { get; set; }
        [DataProperty]
        public string RecoveryID { get; set; }
        [DataProperty]
        public string RecoveryKey { get; set; }
        [DataProperty]
        public DateTimeOffset EffFromDate { get; set; }
        [DataProperty]
        public DateTimeOffset EffToDate { get; set; }
        [DataProperty]
        public bool IsEnabled { get; set; }
        [DataProperty]
        public byte AttemptCount { get; set; }
        [DataProperty]
        public bool IsOptional { get; set; }



    }
}
