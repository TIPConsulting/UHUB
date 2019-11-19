using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop.Attributes;
using UHub.CoreLib.DataInterop;



namespace UHub.CoreLib.Security.Authentication
{
    [DataClass]
    public sealed partial class AccountAuthData : DBEntityBase
    {


        [DataProperty]
        public long UserID { get; private set; }
        [DataProperty]
        public string PswdHash { get; private set; }
        [DataProperty]
        public string Salt { get; private set; }
        [DataProperty]
        public DateTimeOffset PswdCreatedDate { get; private set; }
        [DataProperty]
        public DateTimeOffset PswdModifiedDate { get; private set; }
        [DataProperty]
        public DateTimeOffset? LastLockoutDate { get; private set; }
        [DataProperty]
        public DateTimeOffset? StartFailedPswdWindow { get; private set; }
        [DataProperty]
        public byte FailedPswdAttemptCount { get; private set; }


    }
}
