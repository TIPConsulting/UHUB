using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using UHub.CoreLib.Entities.Users;
using UHub.CoreLib.Entities.Users.Interfaces;

namespace UHub.CoreLib.Security.Authentication.Interfaces
{
    /// <summary>
    /// Authentication manager interface to control method exposure
    /// </summary>
    public partial interface IAuthenticationManager
    {
        /// <summary>
        /// Validate user credentials then set authentication token via cookie
        /// </summary>
        /// <param name="UserEmail">Email address associated with the user account</param>
        /// <param name="UserPassword">Password associated with the user account</param>
        /// <param name="IsPersistent">Flag to set token persistence status</param>
        /// <param name="ResultCode">Result code to indicate process status</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        AuthResultCode TrySetClientAuthToken(
            string UserEmail,
            string UserPassword,
            bool IsPersistent);
        /// <summary>
        /// Validate user credentials then return encrypted authentication token
        /// </summary>
        /// <param name="UserEmail">Email address associated with the user account</param>
        /// <param name="UserPassword">Password associated with the user account</param>
        /// <param name="IsPersistent">Flag to set token persistence status</param>
        /// <param name="ResultCode">Result code to indicate process status</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <returns></returns>
        (AuthResultCode ResultCode, string AuthToken) TryGetClientAuthToken(
            string UserEmail,
            string UserPassword,
            bool IsPersistent);



        /// <summary>
        /// Slide the expiration date of a token and return a new encrypted client token
        /// <para/> If token cannot be extend, then the original token is returned
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        (TokenValidationStatus TokenStatus, string AuthToken) TrySlideAuthTokenExpiration(string token);



        /// <summary>
        /// Try to authenticate a user account using the supplied account credentials
        /// </summary>
        /// <param name="UserEmail">Email address associated with the user account</param>
        /// <param name="UserPassword">Password associated with the user account</param>
        /// <param name="ResultCode">Result code to indicate process status</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <returns></returns>
        AuthResultCode TryAuthenticateUser(
            string UserEmail,
            string UserPassword);



        /// <summary>
        /// Get the url that the user should be redirected to after login
        /// </summary>
        string GetAuthForwardUrl();
        /// <summary>
        /// Forward user to originally requested page (if set) or default auth page
        /// </summary>
        void AuthUserPageForward();



        /// <summary>
        /// Set the user for the current request using an Auth Token.  If the token is invalid, then the user will be set to ANON privs.
        /// This method is provided so that restful API requests without an auth cookie can be authenticated throughout the CMS pipeline
        /// </summary>
        /// <param name="tokenStr">Auth token in string form</param>
        /// <param name="tokenStatus">Returns token validation status</param>
        /// <returns>Status flag</returns>
        TokenValidationStatus TrySetRequestUser(string tokenStr);

        /// <summary>
        /// Ensure that auth token is valid and user is logged in
        /// </summary>
        /// <param name="tokenStr">Auth token in string form</param>
        /// <param name="CmsUser">User encapsulated by auth token (if valid)</param>
        /// <param name="tokenStatus">Returns token validation status</param>
        /// <returns></returns>
        (TokenValidationStatus TokenStatus, User CmsUser) ValidateAuthToken(string tokenStr);



        /// <summary>
        /// Check if user is logged in
        /// </summary>
        /// <returns></returns>
        bool IsUserLoggedIn();


        /// <summary>
        /// Get the currently authenticated CMS user. If the user is not authenticated, then an anonymous user is returned (UID=null, class=Anon)
        /// </summary>
        /// <returns></returns>
        (TokenValidationStatus TokenStatus, User CmsUser) GetCurrentUser();





    }
}
