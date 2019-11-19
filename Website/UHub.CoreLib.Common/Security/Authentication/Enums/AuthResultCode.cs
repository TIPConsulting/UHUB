using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Security.Authentication
{
    public enum AuthResultCode
    {
        Success = 0,
        UnknownError = 1,

        UserInvalid = 10,
        UserLocked = 11,
        UserDisabled = 12,
        CredentialsInvalid = 13,

        EmailInvalid = 20,
        EmailEmpty = 21,
        EmailDuplicate = 22,

        PswdInvalid = 30,
        PswdEmpty = 31,
        PswdNotChanged = 32,
        PswdExpired = 33,

        PendingConfirmation = 40,
        PendingApproval = 41,

        PendingRecoveryContextResolution = 50,
    }
}
