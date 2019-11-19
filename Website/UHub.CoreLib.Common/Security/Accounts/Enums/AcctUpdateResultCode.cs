namespace UHub.CoreLib.Security.Accounts
{
    public enum AcctUpdateResultCode : short
    {
        Success = 0,
        UnknownError = 1000,
        NullArgument = 1001,
        InvalidArgument = 1002,
        InvalidArgumentType = 1003,
        InvalidOperation = 1100,
        AccessDenied = 1200,

        UserInvalid = 2000,

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
