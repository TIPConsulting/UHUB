using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Logging;
using UHub.CoreLib.Management;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Entities.Users;
using UHub.CoreLib.Entities.Users.Interfaces;
using UHub.CoreLib.Entities.Users.DataInterop;
using System.Text.RegularExpressions;
using System.Web;
using UHub.CoreLib.Security.Authentication.Providers.DataInterop;

namespace UHub.CoreLib.Security.Authentication.Providers.Forms
{
    internal sealed partial class FormsAuthProvider : AuthenticationProvider
    {
        /// <summary>
        /// Try to authenticate a user account using the supplied account credentials.  Includes internal logging
        /// </summary>
        /// <param name="userEmail">Email address associated with the user account</param>
        /// <param name="userPassword">Password associated with the user account</param>
        /// <param name="ResultCode">Result code to indicate process status</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <param name="UserTokenHandler">Success handler to handle user token distribution</param>
        /// <returns>Status Flag</returns>
        override internal async Task<AuthResultCode> TryAuthenticateUserAsync(
            string UserEmail,
            string UserPassword,
            Func<User, bool> UserTokenHandler = null)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            UserEmail = UserEmail?.Trim();
            var attrIsValid = Shared.TryAuthenticate_ValidateFields(UserEmail, UserPassword);
            if (attrIsValid != 0)
            {
                return attrIsValid;
            }


            var taskGetUserAuthInfo = UserAuthReader.TryGetUserAuthDataAsync(UserEmail);


            //get userAuth info (pswf info)
            AccountAuthData userAuthInfo = await taskGetUserAuthInfo;

            //BEGIN ASYNC GET USER
            var taskGetUser = UserReader.GetUserAsync(userAuthInfo.UserID);


            //ensure account exists
            if (userAuthInfo == null)
            {
                return AuthResultCode.UserInvalid;
            }



            if (IsUserLockedOut(userAuthInfo))
            {
                var lastLock = userAuthInfo.LastLockoutDate.Value;
                var resetDt = lastLock.Add(CoreFactory.Singleton.Properties.PswdLockResetPeriod);


                var now = FailoverDateTimeOffset.UtcNow;
                if (resetDt < now)
                {
                    try
                    {
                        await ResetUserLockoutAsync(userAuthInfo.UserID);
                    }
                    catch (Exception ex)
                    {
                        var exID = new Guid("A53922C5-0923-4382-A572-50ED87A0B2BE");
                        await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                        return AuthResultCode.UnknownError;
                    }
                }
                else
                {
                    return AuthResultCode.UserLocked;
                }
            }

            //check name/password combo
            PasswordValidationStatus validationStatus = PasswordValidationStatus.UnknownError;
            try
            {
                validationStatus = ValidatePassword(UserEmail, UserPassword);
            }
            catch (Exception ex)
            {
                var exID = new Guid("55AE48DF-B91A-4B24-8EED-2077493BEFE8");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return AuthResultCode.UnknownError;
            }
            if (validationStatus != PasswordValidationStatus.Success)
            {

                if (validationStatus == PasswordValidationStatus.PswdExpired)
                {
                    return AuthResultCode.PswdExpired;
                }
                else if (validationStatus == PasswordValidationStatus.InvalidUser)
                {
                    return AuthResultCode.UserInvalid;
                }
                else if (validationStatus == PasswordValidationStatus.HashMismatch)
                {
                    return AuthResultCode.CredentialsInvalid;
                }
                else
                {
                    var exID = new Guid("07F533B2-FBAD-4751-A467-494BA2D6D19D");
                    await CoreFactory.Singleton.Logging.CreateErrorLogAsync(exID);
                    return AuthResultCode.UnknownError;
                }

            }




            //try to get the real user from CMS DB
            User cmsUser = null;
            try
            {
                cmsUser = await taskGetUser;
            }
            catch (Exception ex)
            {
                var exID = new Guid("D7101F53-4E23-4786-A1C2-69CC6E5EFA25");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return AuthResultCode.UnknownError;
            }

            var userAccessIsValid = Shared.TryAuthenticate_ValidateUserAccess(cmsUser);
            if (userAccessIsValid != AuthResultCode.Success)
            {
                return userAccessIsValid;
            }


            var status = true;
            if (UserTokenHandler != null)
            {
                status = UserTokenHandler.Invoke(cmsUser);
            }


            if (status)
            {
                //CoreFactory.Singleton.Logging.CreateDBActivityLog(ActivityLogType.UserLogin);
                return AuthResultCode.Success;
            }
            else
            {
                return AuthResultCode.UnknownError;
            }

        }


    }
}
