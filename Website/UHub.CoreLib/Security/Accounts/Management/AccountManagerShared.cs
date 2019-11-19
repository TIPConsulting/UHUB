using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;
using SysSec = System.Web.Security;
using UHub.CoreLib.Entities.Users;
using UHub.CoreLib.Management;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.RegExp;

namespace UHub.CoreLib.Security.Accounts.Management
{
    public sealed partial class AccountManager
    {

        private static class Shared
        {
            static Shared()
            {
            }




            internal static void TryCreate_HandleAttrTrim(ref User NewUser)
            {
                NewUser.Name = NewUser.Name?.Trim();
                NewUser.PhoneNumber = NewUser.PhoneNumber?.Trim();
                NewUser.Year = NewUser.Year?.Trim();
                NewUser.ExpectedGradDate = NewUser.ExpectedGradDate?.Trim();
                NewUser.Company = NewUser.Company?.Trim();
                NewUser.JobTitle = NewUser.JobTitle?.Trim();
                NewUser.Email = NewUser.Email?.Trim();
            }


            internal static AcctPswdResultCode ValidateUserPswd(in User NewUser)
            {
                //ensure pswd is populated
                if (NewUser.Password.IsEmpty())
                {
                    return AcctPswdResultCode.PswdEmpty;
                }
                //check for valid password
                if (!NewUser.Password.RgxIsMatch(CoreFactory.Singleton.Properties.PswdStrengthRegex))
                {
                    return AcctPswdResultCode.PswdInvalid;
                }

                return AcctPswdResultCode.Success;
            }

            internal static AcctPswdResultCode ValidateUserPswd(in string UserPswd)
            {
                //ensure pswd is populated
                if (UserPswd.IsEmpty())
                {
                    return AcctPswdResultCode.PswdEmpty;
                }
                //check for valid password
                if (!UserPswd.RgxIsMatch(CoreFactory.Singleton.Properties.PswdStrengthRegex))
                {
                    return AcctPswdResultCode.PswdInvalid;
                }

                return AcctPswdResultCode.Success;
            }


            internal static AcctCreateResultCode TryCreate_ValidateUserAttrs(in User NewUser)
            {
                //ensure email is populated
                if (NewUser.Email.IsEmpty())
                {
                    return AcctCreateResultCode.EmailEmpty;
                }
                //check for valid email length
                if (NewUser.Email.Length < EMAIL_MIN_LEN || NewUser.Email.Length > EMAIL_MAX_LEN)
                {
                    return AcctCreateResultCode.EmailInvalid;
                }
                //check for valid email format
                if (!Validators.IsValidEmail(NewUser.Email))
                {
                    return AcctCreateResultCode.EmailInvalid;
                }


                //check for valid username
                if (NewUser.Username.IsEmpty())
                {
                    return AcctCreateResultCode.UserNameEmpty;
                }
                if (!NewUser.Username.RgxIsMatch(RgxPtrns.EntUser.USERNAME_B, RegexOptions.Singleline))
                {
                    return AcctCreateResultCode.UsernameInvalid;
                }


                //check for invalid user name
                if (NewUser.Name.IsEmpty())
                {
                    return AcctCreateResultCode.NameEmpty;
                }
                if (!NewUser.Name.RgxIsMatch(RgxPtrns.EntUser.NAME_B, RegexOptions.Singleline))
                {
                    return AcctCreateResultCode.NameInvalid;
                }


                //check for invalid phone #
                if (NewUser.PhoneNumber.IsNotEmpty())
                {
                    if (!NewUser.PhoneNumber.RgxIsMatch(RgxPtrns.EntUser.PHONE_B, RegexOptions.Singleline))
                    {
                        return AcctCreateResultCode.PhoneInvalid;
                    }
                }

                //Check for empty major
                if (NewUser.Major.IsEmpty())
                {
                    return AcctCreateResultCode.MajorEmpty;
                }


                //check for invalid year
                if (NewUser.Year.IsNotEmpty())
                {
                    if (!NewUser.Year.RgxIsMatch(RgxPtrns.EntUser.YEAR_B, RegexOptions.Singleline))
                    {
                        return AcctCreateResultCode.YearInvalid;
                    }
                }


                //Check for invalid Grad Date
                if (NewUser.ExpectedGradDate.IsNotEmpty())
                {
                    CultureInfo provider = CultureInfo.InvariantCulture;

                    if (!NewUser.ExpectedGradDate.RgxIsMatch(RgxPtrns.EntUser.GRAD_DATE_B, RegexOptions.Singleline))
                    {
                        return AcctCreateResultCode.GradDateInvalid;
                    }

                    if (!DateTime.TryParseExact(NewUser.ExpectedGradDate,"yyyy-mm-dd", provider, DateTimeStyles.None, out _))
                    {
                        return AcctCreateResultCode.GradDateInvalid;
                    }
                    
                }


                //check for invalid company
                if (NewUser.Company.IsNotEmpty())
                {
                    if (!NewUser.Company.RgxIsMatch(RgxPtrns.EntUser.COMPANY_B, RegexOptions.Singleline))
                    {
                        return AcctCreateResultCode.CompanyInvalid;
                    }
                }


                //check for invalid job title
                if (NewUser.JobTitle.IsNotEmpty())
                {
                    if (!NewUser.JobTitle.RgxIsMatch(RgxPtrns.EntUser.JOB_TITLE_B, RegexOptions.Singleline))
                    {
                        return AcctCreateResultCode.JobTitleInvalid;
                    }
                }

                return AcctCreateResultCode.Success;
            }


            internal static void TryCreate_HandleUserDefaults(ref User NewUser)
            {

                //set property constants
                bool isConfirmed = CoreFactory.Singleton.Properties.AutoConfirmNewAccounts;
                bool isApproved = CoreFactory.Singleton.Properties.AutoApproveNewAccounts;
                string userVersion = SysSec.Membership.GeneratePassword(USER_VERSION_LEN, 0);
                //sterilize for token processing
                userVersion = userVersion.Replace('|', '0');


                NewUser.IsConfirmed = isConfirmed;
                NewUser.IsApproved = isApproved;
                NewUser.Version = userVersion;

            }

        }
    }
}
