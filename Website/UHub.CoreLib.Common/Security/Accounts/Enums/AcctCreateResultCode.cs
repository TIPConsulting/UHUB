namespace UHub.CoreLib.Security.Accounts
{
    public enum AcctCreateResultCode : short
    {
        Success = 0,
        UnknownError = 1000,
        NullArgument = 1001,
        InvalidArgument = 1002,
        InvalidArgumentType = 1003,
        InvalidOperation = 1100,
        AccessDenied = 1200,

        UserInvalid = 2000,

        UsernameInvalid = 3000,
        UsernameDuplicate = 3001,
        UserNameEmpty = 3002,

        EmailInvalid = 4000,
        EmailEmpty = 4001,
        EmailDuplicate = 4002,
        EmailDomainInvalid = 4003,

        PswdInvalid = 5000,
        PswdEmpty = 5001,
        PswdNotChanged = 5002,

        NameEmpty = 6010,
        NameInvalid = 6011,
        MajorInvalid = 6020,
        MajorEmpty = 6021,
        YearInvalid = 6030,
        YearEmpty = 6031,
        CompanyInvalid = 6040,
        CompanyEmpty = 6041,
        JobTitleInvalid = 6050,
        PhoneInvalid = 6060,
        GradDateInvalid = 6070,

    }
}
