using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Security.Accounts
{
    public enum AcctConfirmResultCode
    {
        Success = 0,
        UnknownError = 1000,
        NullArgument = 1001,
        InvalidArgument = 1002,
        InvalidArgumentType = 1003,
        InvalidOperation = 1100,
        AccessDenied = 1200,

        RefUIDEmpty = 8000,
        RefUIDInvalid = 8001,
        ConfirmTokenInvalid = 8002
    }
}
