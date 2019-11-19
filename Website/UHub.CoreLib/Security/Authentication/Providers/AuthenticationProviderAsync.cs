using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Logging;
using UHub.CoreLib.Management;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Entities.Users;
using UHub.CoreLib.Entities.Users.Interfaces;
using UHub.CoreLib.Entities.Users.DataInterop;
using UHub.CoreLib.Security.Authentication.Management;
using static UHub.CoreLib.DataInterop.SqlConverters;
using UHub.CoreLib.Security.Authentication.Providers.DataInterop;

namespace UHub.CoreLib.Security.Authentication.Providers
{
    internal abstract partial class AuthenticationProvider
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
        internal abstract Task<AuthResultCode> TryAuthenticateUserAsync(
            string userEmail,
            string userPassword,
            Func<User, bool> UserTokenHandler = null);



        /// <summary>
        /// Get user auth data from cookie and load as the CurrentUser identity
        /// </summary>
        internal async Task<(TokenValidationStatus TokenStatus, User CmsUser)> ValidateAuthCookieAsync(HttpContext Context)
        {
            var authTknCookieName = CoreFactory.Singleton.Properties.AuthTknCookieName;

            //get cookie
            HttpCookie authCookie =
                    Context.Request?.Cookies.Get(authTknCookieName) ??
                    Context.Response?.Cookies.Get(authTknCookieName);

            if (authCookie == null)
            {
                var CmsUser = UserReader.GetAnonymousUser();
                var TokenStatus = TokenValidationStatus.TokenNotFound;

                return (TokenStatus, CmsUser);
            }

            if (authCookie.Value.IsEmpty())
            {
                var CmsUser = UserReader.GetAnonymousUser();
                var TokenStatus = TokenValidationStatus.TokenNotFound;

                return (TokenStatus, CmsUser);
            }


            var enableAuthSlide = CoreFactory.Singleton.Properties.EnableAuthTokenSlidingExpiration;
            void succHandler(AuthenticationToken token)
            {
                //RE-DATE AUTH TICKET
                //custom sliding expiration
                if (enableAuthSlide)
                {
                    var isSlide = SlideAuthTokenExpiration(token);

                    if (isSlide)
                    {
                        SetCurrentUser_ClientToken(token, Context);
                    }
                }
            }


            return await ValidateAuthTokenAsync(authCookie.Value, Context, succHandler);

        }

        /// <summary>
        /// Check if authentication token is valid
        /// </summary>
        /// <param name="tokenStr"></param>
        /// <param name="CmsUser"></param>
        /// <param name="tokenStatus"></param>
        /// <param name="errorHandler"></param>
        /// <param name="SuccessHandler"></param>
        /// <returns></returns>
        internal async Task<(TokenValidationStatus TokenStatus, User CmsUser)> ValidateAuthTokenAsync(
            string tokenStr,
            HttpContext Context,
            Action<AuthenticationToken> SuccessHandler = null)
        {

            if (tokenStr.IsEmpty())
            {
                var CmsUser = UserReader.GetAnonymousUser();
                return (TokenValidationStatus.TokenNotFound, CmsUser);
            }
            //get token
            AuthenticationToken authToken = null;
            try
            {
                authToken = AuthenticationToken.Decrypt(tokenStr);
            }
            catch (Exception ex)
            {
                var exID = new Guid("8959AB05-5BEF-4351-A980-A3A8AD140EE7");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);

                var CmsUser = UserReader.GetAnonymousUser();
                return (TokenValidationStatus.TokenAESFailure, CmsUser);
            }


            return await ValidateAuthTokenAsync(authToken, Context, SuccessHandler);
        }

        /// <summary>
        /// Check if authentication token is valid
        /// </summary>
        /// <param name="token"></param>
        /// <param name="CmsUser"></param>
        /// <param name="tokenStatus"></param>
        /// <param name="errorHandler"></param>
        /// <param name="SuccessHandler"></param>
        /// <returns></returns>
        private protected async Task<(TokenValidationStatus TokenStatus, User CmsUser)> ValidateAuthTokenAsync(
            AuthenticationToken token,
            HttpContext Context,
            Action<AuthenticationToken> SuccessHandler = null)
        {

            //authenticate token
            if (token == null)
            {
                var CmsUser = UserReader.GetAnonymousUser();

                return (TokenValidationStatus.TokenNotFound, CmsUser);
            }


            string sessionID = GetAdjustedSessionID(token.IsPersistent, Context);


            long userID = token.UserID;
            var taskGetUser = UserReader.GetUserAsync(userID);

            var tokenStatus = await TokenManager.IsTokenValidAsync(token, sessionID);
            var isTokenValid = (tokenStatus == 0);


            if (isTokenValid)
            {
                var CmsUser = UserReader.GetAnonymousUser();

                return (tokenStatus, CmsUser);
            }

            if
            (
                CoreFactory.Singleton.Properties.EnableTokenVersioning &&
                token.SystemVersion != CoreFactory.Singleton.Properties.CurrentAuthTknVersion
            )
            {
                var CmsUser = UserReader.GetAnonymousUser();

                return (TokenValidationStatus.TokenVersionMismatch, CmsUser);
            }



            ////////////////////////////////////////////////////////////////////////////////////////////////////
            //Validate CMS User
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            //try to get the real user from CMS DB
            User cmsUser_local = await taskGetUser;


            //validate CMS specific user
            if (cmsUser_local == null || cmsUser_local.ID == null)
            {
                var CmsUser = UserReader.GetAnonymousUser();

                return (TokenValidationStatus.TokenUserError, CmsUser);
            }


            //check user version
            if (CoreFactory.Singleton.Properties.EnableTokenVersioning)
            {
                string userVersion = token.UserVersion;
                if (cmsUser_local.Version != userVersion)
                {
                    var CmsUser = UserReader.GetAnonymousUser();

                    return (TokenValidationStatus.TokenVersionMismatch, CmsUser);
                }
            }

            //user not confirmed
            if (!cmsUser_local.IsConfirmed)
            {
                var CmsUser = UserReader.GetAnonymousUser();

                return (TokenValidationStatus.TokenUserNotConfirmed, CmsUser);
            }
            if (!cmsUser_local.IsApproved)
            {
                var CmsUser = UserReader.GetAnonymousUser();

                return (TokenValidationStatus.TokenUserNotApproved, CmsUser);
            }

            //user disabled for some other reason
            //could be disabled by admin
            if (!cmsUser_local.IsFullyEnabled)
            {
                var CmsUser = UserReader.GetAnonymousUser();

                return (TokenValidationStatus.TokenUserDisabled, CmsUser);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            //Validation passed
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            SuccessHandler?.Invoke(token);
            return (TokenValidationStatus.Success, cmsUser_local);

        }


        /// <summary>
        /// Generate new auth token for user and write validator to DB
        /// </summary>
        /// <param name="IsPersistent"></param>
        /// <param name="cmsUser"></param>
        /// <param name="SystemVersion"></param>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        internal async Task<AuthenticationToken> GenerateAuthTokenAsync(bool IsPersistent, User cmsUser, HttpContext Context)
        {
            if (cmsUser.ID == null)
            {
                throw new InvalidOperationException("User not valid");
            }

            //Only set persistent tokens if it is allowed by the CMS config
            var isPersistent = CoreFactory.Singleton.Properties.EnablePersistentAuthTokens && IsPersistent;


            var issue = FailoverDateTimeOffset.UtcNow;
            var ID = cmsUser.ID.Value;
            var sysVersion = CoreFactory.Singleton.Properties.CurrentAuthTknVersion;
            string userVersion = cmsUser.Version;
            string sessionID = GetAdjustedSessionID(isPersistent, Context);


            var maxTknLifespan = CoreFactory.Singleton.Properties.MaxAuthTokenLifespan;
            var authTknTimeout = CoreFactory.Singleton.Properties.AuthTokenTimeout;

            DateTimeOffset expiration;
            if (maxTknLifespan.Ticks == 0)
            {
                expiration = DateTimeOffset.MaxValue;
            }
            else
            {
                var tSpan = isPersistent ? maxTknLifespan : authTknTimeout;
                expiration = FailoverDateTimeOffset.UtcNow.Add(tSpan);
            }


            AuthenticationToken authToken = new AuthenticationToken(isPersistent, issue, expiration, ID, sysVersion, userVersion, sessionID);


            try
            {
                await TokenManager.SaveTokenValidatorToDBAsync(authToken);
            }
            catch
            {
                //DB error
                //insert could potentially violate unique key constraints
                throw;
            }

            return authToken;
        }



        #region IsUserLocked

        internal async Task<bool> IsUserLockedOutAsync(long UserID)
        {
            var info = await UserAuthReader.TryGetUserAuthDataAsync(UserID);
            if (info == null)
            {
                throw new InvalidOperationException("User does not exist");
            }


            var maxPsdAttmpt = CoreFactory.Singleton.Properties.MaxPswdAttempts;

            var lock1 = info.LastLockoutDate != null;
            var lock2 = lock1 && info.FailedPswdAttemptCount >= maxPsdAttmpt;
            return lock2;
        }
        #endregion IsUserLocked


        #region GetUserLockoutDate
        internal async Task<DateTimeOffset> GetUserLockoutDateAsync(long UserID)
        {
            var info = await UserAuthReader.TryGetUserAuthDataAsync(UserID);
            if (info == null)
            {
                throw new InvalidOperationException("User does not exist");
            }


            return info.LastLockoutDate ?? DateTimeOffset.MinValue;
        }
        #endregion GetUserLockoutDate


        #region LockoutUser
        private protected async Task LockoutUserAsync(long UserID)
        {

            await SqlWorker.ExecNonQueryAsync(
               CoreFactory.Singleton.Properties.CmsDBConfig,
               "[dbo].[User_SetLastLockoutDateByID]",
               (cmd) =>
               {
                   cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
               });
        }
        #endregion LockoutUser


        #region ResetLockout
        internal async Task ResetUserLockoutAsync(long UserID)
        {

            await SqlWorker.ExecNonQueryAsync(
               CoreFactory.Singleton.Properties.CmsDBConfig,
               "[dbo].[User_ResetFailedPswdAttemptsByID]",
               (cmd) =>
               {
                   cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
               });
        }
        #endregion ResetLockout


        #region HandleBadPswd

        private protected async Task HandleBadPswdAttemptAsync(AccountAuthData UserAuthInfo)
        {

            //log failed pswd attempt to DB
            //CoreFactory.Singleton.Logging.CreateDBActivityLog(ActivityLogType.UserLoginFailure);

            var failCount = UserAuthInfo.FailedPswdAttemptCount;
            var maxPsdAttmpt = CoreFactory.Singleton.Properties.MaxPswdAttempts;


            //do nothing if the account is already locked
            if (failCount >= maxPsdAttmpt)
            {
                return;
            }

            //check if the current failure is outside the failure attempt window
            //if not, then reset lockout window (but only if the account is not already locked)

            var psdAttmptPrd = CoreFactory.Singleton.Properties.PswdAttemptPeriod;

            var dt = UserAuthInfo.StartFailedPswdWindow ?? DateTimeOffset.MinValue;


            //initialize error state
            List<Task> taskSet = new List<Task>();
            if ((dt + psdAttmptPrd) < FailoverDateTimeOffset.UtcNow)
            {
                failCount = 0;
                taskSet.Add(ResetUserLockoutAsync(UserAuthInfo.UserID));
            }
            //advance error state
            failCount++;
            taskSet.Add(UserAuthWriter.LogFailedPsdAttemptAsync(UserAuthInfo.UserID));


            if (failCount >= maxPsdAttmpt)
            {
                taskSet.Add(LockoutUserAsync(UserAuthInfo.UserID));
            }


            await Task.WhenAll(taskSet);
        }
        #endregion HandleBadPsd

    }
}
