using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Security.Cryptography;
using System.Runtime.CompilerServices;
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
        internal abstract AuthResultCode TryAuthenticateUser(
            string userEmail,
            string userPassword,
            Func<User, bool> UserTokenHandler = null);


        /// <summary>
        /// Get user auth data from cookie and load as the CurrentUser identity
        /// </summary>
        internal (TokenValidationStatus TokenStatus, User CmsUser) ValidateAuthCookie()
        {
            var authTknCookieName = CoreFactory.Singleton.Properties.AuthTknCookieName;

            //get cookie
            HttpCookie authCookie =
                    HttpContext.Current?.Request?.Cookies.Get(authTknCookieName) ??
                    HttpContext.Current?.Response?.Cookies.Get(authTknCookieName);

            if (authCookie == null)
            {
                var cmsUser = UserReader.GetAnonymousUser();
                return (TokenValidationStatus.TokenNotFound, cmsUser);
            }

            if (authCookie.Value.IsEmpty())
            {
                var cmsUser = UserReader.GetAnonymousUser();
                return (TokenValidationStatus.TokenNotFound, cmsUser);
            }


            var enableAuthSlide = CoreFactory.Singleton.Properties.EnableAuthTokenSlidingExpiration;
            Action<AuthenticationToken> succHandler =
                (token) =>
                {
                    //RE-DATE AUTH TICKET
                    //custom sliding expiration
                    if (enableAuthSlide)
                    {
                        var isSlide = SlideAuthTokenExpiration(token);

                        if (isSlide)
                        {
                            SetCurrentUser_ClientToken(token, HttpContext.Current);
                        }
                    }
                };


            return ValidateAuthToken(authCookie.Value, succHandler);

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
        internal (TokenValidationStatus TokenStatus, User CmsUser) ValidateAuthToken(
            string tokenStr,
            Action<AuthenticationToken> SuccessHandler = null)
        {

            if (tokenStr.IsEmpty())
            {
                var cmsUser = UserReader.GetAnonymousUser();
                return (TokenValidationStatus.TokenNotFound, cmsUser);
            }
            //get token
            AuthenticationToken authToken = null;
            try
            {
                authToken = AuthenticationToken.Decrypt(tokenStr);
            }
            catch(CryptographicException)
            {
                var cmsUser = UserReader.GetAnonymousUser();
                return (TokenValidationStatus.TokenAESFailure, cmsUser);
            }
            catch (Exception ex)
            {
                var exID = new Guid("BCB41528-CF1B-48B9-B7EA-4F622AE7F047");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);

                var cmsUser = UserReader.GetAnonymousUser();
                return (TokenValidationStatus.TokenAESFailure, cmsUser);
            }


            return ValidateAuthToken(authToken, SuccessHandler);
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
        private protected (TokenValidationStatus TokenStatus, User CmsUser) ValidateAuthToken(
            AuthenticationToken token,
            Action<AuthenticationToken> SuccessHandler = null)
        {

            //authenticate token
            if (token == null)
            {
                var cmsUser = UserReader.GetAnonymousUser();
                return (TokenValidationStatus.TokenNotFound, cmsUser);
            }

            var context = HttpContext.Current;

            string sessionID = GetAdjustedSessionID(token.IsPersistent, context);

            var tempTokenStatus = TokenManager.IsTokenValid(token, sessionID);
            if (tempTokenStatus != TokenValidationStatus.Success)
            {
                var cmsUser = UserReader.GetAnonymousUser();
                return (tempTokenStatus, cmsUser);
            }

            if
            (
                CoreFactory.Singleton.Properties.EnableTokenVersioning &&
                token.SystemVersion != CoreFactory.Singleton.Properties.CurrentAuthTknVersion
            )
            {
                var cmsUser = UserReader.GetAnonymousUser();
                return (TokenValidationStatus.TokenVersionMismatch, cmsUser);
            }


            long userID = token.UserID;
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            //Validate CMS User
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            //try to get the real user from CMS DB
            User cmsUser_local;
            try
            {
                cmsUser_local = UserReader.GetUser(userID);
            }
            catch
            {

                var cmsUser = UserReader.GetAnonymousUser();
                return (TokenValidationStatus.TokenUserError, cmsUser);
            }

            //validate CMS specific user
            if (cmsUser_local == null || cmsUser_local.ID == null)
            {
                var cmsUser = UserReader.GetAnonymousUser();
                return (TokenValidationStatus.TokenUserError, cmsUser);
            }

            //check user version
            if (CoreFactory.Singleton.Properties.EnableTokenVersioning)
            {
                string userVersion = token.UserVersion;
                if (cmsUser_local.Version != userVersion)
                {
                    var cmsUser = UserReader.GetAnonymousUser();
                    return (TokenValidationStatus.TokenVersionMismatch, cmsUser);
                }
            }

            //user not confirmed
            if (!cmsUser_local.IsConfirmed)
            {
                var cmsUser = UserReader.GetAnonymousUser();
                return (TokenValidationStatus.TokenUserNotConfirmed, cmsUser);
            }
            if (!cmsUser_local.IsApproved)
            {
                var cmsUser = UserReader.GetAnonymousUser();
                return (TokenValidationStatus.TokenUserNotApproved, cmsUser);
            }

            //user disabled for some other reason
            //could be disabled by admin
            if (!cmsUser_local.IsFullyEnabled)
            {
                var cmsUser = UserReader.GetAnonymousUser();
                return (TokenValidationStatus.TokenUserDisabled, cmsUser);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            //Validation passed
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            SuccessHandler?.Invoke(token);
            return (TokenValidationStatus.Success, cmsUser_local);

        }

        /// <summary>
        /// Get real session ID from web client or use the get the ID presented by the MACHINE_KEY attribute
        /// </summary>
        /// <param name="isPersistent"></param>
        /// <returns></returns>
        private protected string GetAdjustedSessionID(bool isPersistent, HttpContext Context)
        {
            string sessionID = "";
            string clientKey = "";
            clientKey = Context?.Request?.Headers[Constants.AUTH_HEADER_MACHINE_KEY] ?? "";

            //session ID must be ignored for persistent connections
            //otherwise token will not work if user comes back to site at later date
            if (isPersistent)
            {
                if (clientKey.IsNotEmpty())
                {
                    return clientKey;
                }
                else
                {
                    return "";
                }
            }

            //NOT PERSISTENT
            if (clientKey.IsNotEmpty())
            {
                sessionID = clientKey;
            }
            else
            {
                var sessionCookie = CoreFactory.Singleton.Properties.SessionIDCookieName;

                //get session directly from cookie because Context.Session is not initialized yet
                sessionID =
                    Context?.Request?.Cookies?.Get(sessionCookie)?.Value
                    ?? "";
            }

            //sterilize for token processing
            sessionID = sessionID.Replace('|', '0');
            return sessionID;
        }

        /// <summary>
        /// Slide the expiration date for an auth token
        /// </summary>
        /// <param name="token"></param>
        internal bool SlideAuthTokenExpiration(AuthenticationToken token)
        {
            if (token.IsPersistent)
            {
                //persistent tokens are already set to max possible age
                //no slide is necessary
                return false;
            }

            //ensure that the new expiration date is inside the max lifespan window
            var timeout = CoreFactory.Singleton.Properties.AuthTokenTimeout;
            var slideDate = FailoverDateTimeOffset.UtcNow.Add(timeout);
            DateTimeOffset maxExpire;


            var maxLifespan = CoreFactory.Singleton.Properties.MaxAuthTokenLifespan;
            if (maxLifespan.Ticks == 0)
            {
                maxExpire = DateTimeOffset.MaxValue;
            }
            else
            {
                maxExpire = token.IssueDate.Add(maxLifespan);
            }


            if (slideDate <= maxExpire)
            {
                token.SetExpiration(slideDate);
                return true;
            }
            if (slideDate > maxExpire)
            {
                if (token.ExpirationDate == maxExpire)
                {
                    return false;
                }
                token.SetExpiration(maxExpire);
                return true;
            }

            return false;
        }


        


        #region IsUserLocked

        internal bool IsUserLockedOut(long UserID)
        {
            var info = UserAuthReader.TryGetUserAuthData(UserID);
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
        internal DateTimeOffset GetUserLockoutDate(long UserID)
        {
            var info = UserAuthReader.TryGetUserAuthData(UserID);
            if (info == null)
            {
                throw new InvalidOperationException("User does not exist");
            }


            return info.LastLockoutDate ?? DateTimeOffset.MinValue;
        }
        #endregion GetUserLockoutDate


        #region LockoutUser
        private protected void LockoutUser(long UserID)
        {

            SqlWorker.ExecNonQuery(
               CoreFactory.Singleton.Properties.CmsDBConfig,
               "[dbo].[User_SetLastLockoutDateByID]",
               (cmd) =>
               {
                   cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
               });
        }
        #endregion LockoutUser


        #region ResetLockout
        internal void ResetUserLockout(long UserID)
        {

            SqlWorker.ExecNonQuery(
               CoreFactory.Singleton.Properties.CmsDBConfig,
               "[dbo].[User_ResetFailedPswdAttemptsByID]",
               (cmd) =>
               {
                   cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
               });
        }
        #endregion ResetLockout


        #region HandleBadPswd

        private protected void HandleBadPswdAttempt(AccountAuthData UserAuthInfo)
        {
            //TODO:
            //Fix side-channel exploit vector
            //It is possible for multiple simultaneous bad passwords to only increment the counter once


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
            if ((dt + psdAttmptPrd) < FailoverDateTimeOffset.UtcNow)
            {
                failCount = 0;
                ResetUserLockout(UserAuthInfo.UserID);
                failCount++;
                UserAuthWriter.LogFailedPsdAttempt(UserAuthInfo.UserID);
            }
            else
            {
                failCount++;
                UserAuthWriter.LogFailedPsdAttempt(UserAuthInfo.UserID);
            }


            if (failCount >= maxPsdAttmpt)
            {
                LockoutUser(UserAuthInfo.UserID);
            }
        }
        #endregion HandleBadPsd

    }
}
