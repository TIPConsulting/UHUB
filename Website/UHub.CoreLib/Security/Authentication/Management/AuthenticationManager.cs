using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Management;
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
        /// <paramref name="ResultCode">Result code to indicate process status</paramref>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        public AuthResultCode TrySetClientAuthToken(
            string UserEmail,
            string UserPassword,
            bool IsPersistent)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            bool userTokenHandler(User cmsUser)
            {
                var authCookieName = CoreFactory.Singleton.Properties.AuthTknCookieName;

                //remove old token
                HttpContext.Current.Request.Cookies.Remove(authCookieName);
                HttpContext.Current.Response.Cookies.Remove(authCookieName);

                //everything good
                //write user auth cookie
                try
                {
                    authWorker.SetCurrentUser_Local(cmsUser, HttpContext.Current);
                    authWorker.SetCurrentUser_ClientToken(IsPersistent, cmsUser, HttpContext.Current);
                    return true;
                }
                catch (Exception ex)
                {
                    var exID = new Guid("F1459F20-3C34-4DAD-9925-A3A09D917E5F");
                    CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                    return false;
                }
            }


            return authWorker.TryAuthenticateUser(
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
        /// <paramref name="ResultCode">Result code to indicate process status</paramref>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <returns></returns>
        public (AuthResultCode ResultCode, string AuthToken) TryGetClientAuthToken(
            string UserEmail,
            string UserPassword,
            bool IsPersistent)
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
                    var context = HttpContext.Current;
                    var tkn = authWorker.GenerateAuthToken(IsPersistent, cmsUser, context);
                    token = tkn.Encrypt();
                    return true;
                }
                catch (CryptographicException ex)
                {
                    token = "ERROR";
                    var exID = new Guid("28A013BC-BB19-47FF-977A-A4AAA6798884");
                    CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);

                    return false;
                }
                catch (Exception ex)
                {
                    token = "ERROR";
                    var exID = new Guid("3BB3F302-6D3F-4271-824C-589EDFD49C13");
                    CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);

                    return false;
                }
            }


            var resultCode = authWorker.TryAuthenticateUser(
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
        public (TokenValidationStatus TokenStatus, string AuthToken) TrySlideAuthTokenExpiration(string token)
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

            var validationResult = authWorker.ValidateAuthToken(token, succHandler);
            if (validationResult.TokenStatus != 0)
            {
                var context = HttpContext.Current;
                TryLogOut(context, 5);
            }

            return (validationResult.TokenStatus, newToken);
        }


        /// <summary>
        /// Try to authenticate a user account using the supplied account credentials
        /// </summary>
        /// <param name="UserEmail">Email address associated with the user account</param>
        /// <param name="UserPassword">Password associated with the user account</param>
        /// <paramref name="ResultCode">Result code to indicate process status</paramref>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <returns></returns>
        public AuthResultCode TryAuthenticateUser(
            string UserEmail,
            string UserPassword)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            return authWorker.TryAuthenticateUser(
                UserEmail,
                UserPassword,
                null);
        }


        /// <summary>
        /// Get the url that the user should be redirected to after login
        /// </summary>
        public string GetAuthForwardUrl()
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            var postAuthCookieName = CoreFactory.Singleton.Properties.PostAuthCookieName;
            var cookie = HttpContext.Current.Request.Cookies[postAuthCookieName];


            var loginUrl = CoreFactory.Singleton.Properties.LoginURL;
            var defaultFwdUrl = CoreFactory.Singleton.Properties.DefaultAuthFwdURL;


            if (cookie != null)
            {
                cookie.Decrypt();
                if (cookie.Value.IsNotEmpty() && cookie.Value != loginUrl)
                {
                    //go to requested page

                    if (Uri.TryCreate(cookie.Value, UriKind.Absolute, out var url))
                    {
                        return url.AbsolutePath;
                    }

                    return defaultFwdUrl;
                }
                else
                {
                    //go to default page
                    return defaultFwdUrl;
                }
            }
            else
            {
                return defaultFwdUrl;
            }
        }

        /// <summary>
        /// Forward user to originally requested page (if set) or default auth page
        /// </summary>
        public void AuthUserPageForward()
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            //forward to originally requested page
            var url = GetAuthForwardUrl();

            HttpContext.Current.Response.Redirect(url);
        }

        /// <summary>
        /// Set the user for the current request using an Auth Token.  If the token is invalid, then the user will be set to ANON privs.
        /// This method is provided so that restful API requests without an auth cookie can be authenticated throughout the CMS pipeline
        /// </summary>
        /// <param name="tokenStr">AuthToken in string form</param>
        /// <param name="tokenStatus">Returns token validation status</param>
        /// <returns>Status flag</returns>
        public TokenValidationStatus TrySetRequestUser(string tokenStr)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            var validationResult = ValidateAuthToken(tokenStr);
            var cmsUser = validationResult.CmsUser;


            if (validationResult.TokenStatus != TokenValidationStatus.Success)
            {
                cmsUser = UserReader.GetAnonymousUser();
                HttpContext.Current.Items[REQUEST_CURRENT_USER] = cmsUser;
                return validationResult.TokenStatus;
            }


            if (cmsUser == null || cmsUser.ID == null)
            {
                cmsUser = UserReader.GetAnonymousUser();
                HttpContext.Current.Items[REQUEST_CURRENT_USER] = cmsUser;
                return TokenValidationStatus.AnonUser;
            }


            HttpContext.Current.Items[REQUEST_CURRENT_USER] = cmsUser;

            return TokenValidationStatus.Success;
        }

        /// <summary>
        /// Ensure that auth token is valid and user is logged in
        /// </summary>
        /// <param name="tokenStr">Auth token in string form</param>
        /// <param name="CmsUser">User encapsulated by auth token (if valid)</param>
        /// <param name="tokenStatus">Return auth token validation status</param>
        /// <returns></returns>
        public (TokenValidationStatus TokenStatus, User CmsUser) ValidateAuthToken(string tokenStr)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            var tokenStatus = authWorker.ValidateAuthToken(tokenStr, null);
            return tokenStatus;
        }


        /// <summary>
        /// Determine if there is a user logged in
        /// Uses the auth cookie and various expiration timers
        /// Returns the authenticated user or a reference to Anon instance
        /// </summary>
        /// <returns></returns>
        public bool IsUserLoggedIn()
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }



            var requestUser = HttpContext.Current.Items[REQUEST_CURRENT_USER];

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
                //get user info from tkn (if it exists)
                var cookieResult = authWorker.ValidateAuthCookie();
                var tokenStatus = cookieResult.TokenStatus;
                var cmsUser = cookieResult.CmsUser;


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
                HttpContext.Current.Items[REQUEST_CURRENT_USER] = cmsUser;


                return (tokenStatus == 0);
            }

        }

        /// <summary>
        /// Get the currently authenticated CMS user. If the user is not authenticated, then an anonymous user is returned (UID=null, class=Anon)
        /// </summary>
        /// <returns></returns>
        public (TokenValidationStatus TokenStatus, User CmsUser) GetCurrentUser()
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {

                var requestUser = HttpContext.Current?.Items[REQUEST_CURRENT_USER];

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
                    //get user info from tkt (if it exists)
                    var validateResult = authWorker.ValidateAuthCookie();
                    var tokenStatus = validateResult.TokenStatus;
                    var cmsUser = validateResult.CmsUser;

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
                    HttpContext.Current.Items[REQUEST_CURRENT_USER] = cmsUser;


                    return (tokenStatus, cmsUser);
                }

            }
            catch (Exception ex)
            {
                var exID = new Guid("FF7B1AA6-FAD8-43CB-A633-868E2BD7784B");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                var cmsUser = UserReader.GetAnonymousUser();
                return (TokenValidationStatus.AnonUser, cmsUser);
            }
        }


        




    }
}
