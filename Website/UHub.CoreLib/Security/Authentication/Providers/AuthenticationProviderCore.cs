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
using System.Runtime.CompilerServices;
using UHub.CoreLib.Security.Authentication.Management;

namespace UHub.CoreLib.Security.Authentication.Providers
{
    internal abstract partial class AuthenticationProvider
    {
        /// <summary>
        /// Request variable to store the current user.  Used as temp caching to prevent extra lookups
        /// </summary>
        internal const string REQUEST_CURRENT_USER = "Global_CurrentUser";


        internal AuthenticationProvider()
        {

        }


        internal bool IsUserLockedOut(AccountAuthData AuthData)
        {
            var maxPsdAttmpt = CoreFactory.Singleton.Properties.MaxPswdAttempts;

            var lock1 = AuthData.LastLockoutDate != null;
            var lock2 = lock1 && AuthData.FailedPswdAttemptCount >= maxPsdAttmpt;
            return lock2;
        }





        /// <summary>
        /// Set current request user for caching
        /// </summary>
        /// <param name="CmsUser"></param>
        internal void SetCurrentUser_Local(User CmsUser, HttpContext Context)
        {
            //set identity
            Context.Items[REQUEST_CURRENT_USER] = CmsUser;
        }


        /// <summary>
        /// Set current user via cookie - allows login to persist between requests
        /// </summary>
        /// <param name="IsPersistentent">Is token persisten</param>
        /// <param name="CmsUser">User to set</param>
        /// <param name="SystemVersion">Specify an Auth Tkn version number. Defaults to <see cref="CmsProperties.CurrentAuthTknVersion"/> </param>
        /// <exception cref="SqlException"></exception>
        /// /// <exception cref="Exception"></exception>
        internal void SetCurrentUser_ClientToken(bool IsPersistentent, User CmsUser, HttpContext Context)
        {
            var context = Context;
            var token = GenerateAuthToken(IsPersistentent, CmsUser, context);
            SetCurrentUser_ClientToken(token, Context);
        }

        /// <summary>
        /// Set current user via cookie - allows login to persist between requests
        /// </summary>
        /// <param name="token"></param>
        private protected void SetCurrentUser_ClientToken(AuthenticationToken token, HttpContext Context)
        {
            var authTknCookieName = CoreFactory.Singleton.Properties.AuthTknCookieName;
            var forceSecure = CoreFactory.Singleton.Properties.ForceSecureCookies;


            //remove old token
            Context.Request.Cookies.Remove(authTknCookieName);
            Context.Response.Cookies.Remove(authTknCookieName);


            string encryptedToken = token.Encrypt();
            HttpCookie authCookie = new HttpCookie(authTknCookieName, encryptedToken);
            authCookie.Shareable = false;
            authCookie.Secure = forceSecure;
            authCookie.HttpOnly = forceSecure;
            //set expiration for persistent cookies
            //otherwise the cookie will expire with browser session
            if (token.IsPersistent)
            {
                authCookie.Expires = token.ExpirationDate.UtcDateTime;
            }

            Context.Response.SetCookie(authCookie);
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
        internal AuthenticationToken GenerateAuthToken(bool IsPersistent, User cmsUser, HttpContext context)
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
            string sessionID = GetAdjustedSessionID(isPersistent, context);


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
                TokenManager.SaveTokenValidatorToDB(authToken);
            }
            catch
            {
                //DB error
                //insert could potentially violate unique key constraints
                throw;
            }

            return authToken;
        }
    }
}
