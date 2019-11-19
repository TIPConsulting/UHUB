using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Security.Authentication
{
    public enum PasswordValidationStatus
    {
        Success = 0,
        NotFound = 1,
        InvalidUser = 2,
        HashMismatch = 3,
        PswdExpired = 4,
        UnknownError = 5
    }
}
