namespace UHub.CoreLib.Security.Accounts
{
    public enum AcctRecoveryResultCode : short
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

        RecoveryNotEnabled = 7000,
        RecoveryContextInvalid = 7001,
        RecoveryContextDisabled = 7002,
        RecoveryContextExpired = 7003,
        RecoveryContextDestroyed = 7004,
        RecoveryKeyInvalid = 7005,
        
    }
}
