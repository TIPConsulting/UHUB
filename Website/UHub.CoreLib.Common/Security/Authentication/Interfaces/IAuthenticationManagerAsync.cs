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
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        Task<AuthResultCode> TrySetClientAuthTokenAsync(
            string UserEmail,
            string UserPassword,
            bool IsPersistent,
            HttpContext Context);



        /// <summary>
        /// Validate user credentials then return encrypted authentication token
        /// </summary>
        /// <param name="UserEmail">Email address associated with the user account</param>
        /// <param name="UserPassword">Password associated with the user account</param>
        /// <param name="IsPersistent">Flag to set token persistence status</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <returns></returns>
        Task<(AuthResultCode ResultCode, string AuthToken)> TryGetClientAuthTokenAsync(
            string UserEmail,
            string UserPassword,
            bool IsPersistent,
            HttpContext Context);



        /// <summary>
        /// Slide the expiration date of a token and return a new encrypted client token
        /// <para/> If token cannot be extend, then the original token is returned
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<(TokenValidationStatus TokenStatus, string AuthToken)> TrySlideAuthTokenExpirationAsync(string token, HttpContext Context);



        /// <summary>
        /// Try to authenticate a user account using the supplied account credentials
        /// </summary>
        /// <param name="UserEmail">Email address associated with the user account</param>
        /// <param name="UserPassword">Password associated with the user account</param>
        /// <param name="GeneralFailHandler">Error handler in case DB cannot be reached or there is other unknown error</param>
        /// <returns></returns>
        Task<AuthResultCode> TryAuthenticateUserAsync(
            string UserEmail,
            string UserPassword);



        /// <summary>
        /// Set the user for the current request using an Auth Token.  If the token is invalid, then the user will be set to ANON privs.
        /// This method is provided so that restful API requests without an auth cookie can be authenticated throughout the CMS pipeline
        /// </summary>
        /// <param name="tokenStr">AuthToken in string form</param>
        /// <returns>Status flag</returns>
        Task<TokenValidationStatus> TrySetRequestUserAsync(string tokenStr, HttpContext Context);




        /// <summary>
        /// Ensure that auth token is valid and user is logged in
        /// </summary>
        /// <param name="tokenStr">Auth token in string form</param>
        /// <param name="CmsUser">User encapsulated by auth token (if valid)</param>
        /// <param name="tokenStatus">Return auth token validation status</param>
        /// <returns></returns>
        Task<(TokenValidationStatus TokenStatus, User CmsUser)> ValidateAuthTokenAsync(string tokenStr, HttpContext Context);



        /// <summary>
        /// Determine if there is a user logged in
        /// Uses the auth cookie and various expiration timers
        /// Returns the authenticated user or a reference to Anon instance
        /// </summary>
        /// <returns></returns>
        Task<bool> IsUserLoggedInAsync(HttpContext Context);




        /// <summary>
        /// Get the currently authenticated CMS user. If the user is not authenticated, then an anonymous user is returned (UID=null, class=Anon)
        /// </summary>
        /// <returns></returns>
        Task<(TokenValidationStatus TokenStatus, User CmsUser)> GetCurrentUserAsync(HttpContext Context);

    }
}
