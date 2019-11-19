namespace UHub.CoreLib.Security.Accounts
{
    public enum AcctPswdResultCode : short
    {
        Success = 0,
        UnknownError = 1000,
        NullArgument = 1001,
        InvalidArgument = 1002,
        InvalidArgumentType = 1003,
        InvalidOperation = 1100,
        AccessDenied = 1200,

        UserInvalid = 2000,
        LoginFailed = 2001,

        EmailInvalid = 4000,
        EmailEmpty = 4001,

        PswdInvalid = 5000,
        PswdEmpty = 5001,
        PswdNotChanged = 5002,
    }
}
