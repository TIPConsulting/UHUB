using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Security.Authentication.Interfaces;
using UHub.CoreLib.Entities.Users;
using UHub.CoreLib.Entities.Users.Interfaces;
using UHub.CoreLib.Entities.Users.DataInterop;

namespace UHub.CoreLib.Security.Authentication.Management
{

    internal sealed partial class AuthenticationManager
    {


        /// <summary>
        /// Validate user credentials then set authentication token via cookie
        /// </summary>
        /// <param name="UserEmail">Email address associated with the user account</param>
        /// <param name="UserPassword">Password associated with the user account</param>
        /// <param name="IsPersistent">Flag to set token persistence status</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        public async Task<AuthResultCode> TrySetClientAuthTokenAsync(
            string UserEmail,
            string UserPassword,
            bool IsPersistent,
            HttpContext Context)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            bool userTokenHandler(User cmsUser)
            {
                var authCookieName = CoreFactory.Singleton.Properties.AuthTknCookieName;

                //remove old token
                Context.Request.Cookies.Remove(authCookieName);
                Context.Response.Cookies.Remove(authCookieName);

                //everything good
                //write user auth cookie
                try
                {
                    authWorker.SetCurrentUser_Local(cmsUser, Context);
                    authWorker.SetCurrentUser_ClientToken(IsPersistent, cmsUser, Context);
                    return true;
                }
                catch (Exception ex)
                {
                    var exID = new Guid("AC0FB05B-0910-4407-B567-2CDCB7A31733");
                    CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                    return false;
                }
            }


            return await authWorker.TryAuthenticateUserAsync(
                UserEmail,
                UserPassword,
                userTokenHandler);
        }

        /// <summary>
        /// Validate user credentials then return encrypted authentication token
        /// </summary>
        /// <param name="UserEmail">Email address associated with the user account</param>
        /// <param name="UserPassword">Password associated with the user account</param>
        /// <param name="IsPersistent">Flag to set token persistence status</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <returns></returns>
        public async Task<(AuthResultCode ResultCode, string AuthToken)> TryGetClientAuthTokenAsync(
            string UserEmail,
            string UserPassword,
            bool IsPersistent,
            HttpContext Context)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            string token = "";

            bool userTokenHandler(User cmsUser)
            {
                //everything good
                //write user auth cookie
                try
                {
                    var tkn = authWorker.GenerateAuthToken(IsPersistent, cmsUser, Context);
                    token = tkn.Encrypt();

                    return true;
                }
                catch (CryptographicException ex)
                {
                    token = "ERROR";
                    var exID = new Guid("5350AF25-954A-4C4F-B162-B93A1BF57B38");
                    CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);

                    return false;
                }
                catch (Exception ex)
                {
                    token = "ERROR";
                    var exID = new Guid("810D0724-BCBB-46AC-B661-556A89EBA34B");
                    CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);

                    return false;
                }
            }


            var resultCode = await authWorker.TryAuthenticateUserAsync(
                UserEmail,
                UserPassword,
                userTokenHandler);


            return (resultCode, token);
        }


        /// <summary>
        /// Slide the expiration date of a token and return a new encrypted client token
        /// <para/> If token cannot be extend, then the original token is returned
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<(TokenValidationStatus TokenStatus, string AuthToken)> TrySlideAuthTokenExpirationAsync(string token, HttpContext Context)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            string newToken = token;


            void succHandler(AuthenticationToken authToken)
            {
                authWorker.SlideAuthTokenExpiration(authToken);
                newToken = authToken.Encrypt();
            }

            var result = await authWorker.ValidateAuthTokenAsync(token, Context, succHandler);
            if (result.TokenStatus != 0)
            {
                TryLogOut(Context, 5);
            }


            return (result.TokenStatus, token);
        }

        /// <summary>
        /// Try to authenticate a user account using the supplied account credentials
        /// </summary>
        /// <param name="UserEmail">Email address associated with the user account</param>
        /// <param name="UserPassword">Password associated with the user account</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <returns></returns>
        public async Task<AuthResultCode> TryAuthenticateUserAsync(
            string UserEmail,
            string UserPassword)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            return await authWorker.TryAuthenticateUserAsync(
                UserEmail,
                UserPassword,
                null);
        }



        /// <summary>
        /// Set the user for the current request using an Auth Token.  If the token is invalid, then the user will be set to ANON privs.
        /// This method is provided so that restful API requests without an auth cookie can be authenticated throughout the CMS pipeline
        /// </summary>
        /// <param name="tokenStr">AuthToken in string form</param>
        /// <returns>Status flag</returns>
        public async Task<TokenValidationStatus> TrySetRequestUserAsync(string tokenStr, HttpContext Context)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            var validationResult = await ValidateAuthTokenAsync(tokenStr, Context);


            var cmsUser = validationResult.CmsUser;
            var tokenStatus = validationResult.TokenStatus;
            var isValid = (tokenStatus == TokenValidationStatus.Success);


            if (!isValid)
            {
                cmsUser = UserReader.GetAnonymousUser();
                Context.Items[REQUEST_CURRENT_USER] = cmsUser;

                return tokenStatus;
            }


            if (cmsUser == null || cmsUser.ID == null)
            {
                cmsUser = UserReader.GetAnonymousUser();
                Context.Items[REQUEST_CURRENT_USER] = cmsUser;

                return TokenValidationStatus.AnonUser;
            }


            Context.Items[REQUEST_CURRENT_USER] = cmsUser;


            return tokenStatus;
        }

        /// <summary>
        /// Ensure that auth token is valid and user is logged in
        /// </summary>
        /// <param name="tokenStr">Auth token in string form</param>
        /// <param name="CmsUser">User encapsulated by auth token (if valid)</param>
        /// <param name="tokenStatus">Return auth token validation status</param>
        /// <returns></returns>
        public async Task<(TokenValidationStatus TokenStatus, User CmsUser)> ValidateAuthTokenAsync(string tokenStr, HttpContext Context)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            return await authWorker.ValidateAuthTokenAsync(tokenStr, Context, null);
        }


        /// <summary>
        /// Determine if there is a user logged in
        /// Uses the auth cookie and various expiration timers
        /// Returns the authenticated user or a reference to Anon instance
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsUserLoggedInAsync(HttpContext Context)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            var requestUser = Context.Items[REQUEST_CURRENT_USER];

            //check for cached user
            if (false && requestUser != null && requestUser is User currentUser)
            {
                if (currentUser.ID == null)
                {
                    return false;
                }

                return true;
            }
            //if no cached user
            else
            {
                var authData = await authWorker.ValidateAuthCookieAsync(Context);
                var cmsUser = authData.CmsUser;
                var tokenStatus = authData.TokenStatus;


                //check for real user vs Anon user
                if (cmsUser == null || cmsUser.ID == null)
                {
                    if (tokenStatus == TokenValidationStatus.Success)
                    {
                        tokenStatus = TokenValidationStatus.AnonUser;
                    }
                }
                if (tokenStatus != TokenValidationStatus.Success)
                {
                    cmsUser = UserReader.GetAnonymousUser();
                }

                //write user to cache
                Context.Items[REQUEST_CURRENT_USER] = cmsUser;


                return (tokenStatus == 0);
            }

        }


        /// <summary>
        /// Get the currently authenticated CMS user. If the user is not authenticated, then an anonymous user is returned (UID=null, class=Anon)
        /// </summary>
        /// <returns></returns>
        public async Task<(TokenValidationStatus TokenStatus, User CmsUser)> GetCurrentUserAsync(HttpContext Context)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {

                var requestUser = Context.Items[REQUEST_CURRENT_USER];


                //check for cached user
                if (requestUser != null && requestUser is User currentUser)
                {
                    if (currentUser.ID == null)
                    {
                        return (TokenValidationStatus.AnonUser, currentUser);
                    }


                    return (TokenValidationStatus.Success, currentUser);
                }
                //if no cached user
                else
                {

                    var authData = await authWorker.ValidateAuthCookieAsync(Context);
                    var cmsUser = authData.CmsUser;
                    var tokenStatus = authData.TokenStatus;


                    //BAD USER
                    if (cmsUser == null || cmsUser.ID == null)
                    {
                        if (tokenStatus == TokenValidationStatus.Success)
                        {
                            tokenStatus = TokenValidationStatus.AnonUser;
                        }
                    }
                    if (tokenStatus != TokenValidationStatus.Success)
                    {
                        cmsUser = UserReader.GetAnonymousUser();
                    }


                    Context.Items[REQUEST_CURRENT_USER] = cmsUser;
                    return (tokenStatus, cmsUser);
                }

            }
            catch (Exception ex)
            {
                var exID = new Guid("56BB6A44-406A-487C-988B-9BBF0CF9F660");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);

                var CmsUser = UserReader.GetAnonymousUser();
                return (TokenValidationStatus.AnonUser, CmsUser);
            }
        }




    }
}
