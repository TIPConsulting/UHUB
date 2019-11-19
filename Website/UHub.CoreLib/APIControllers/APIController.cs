using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Security;
using UHub.CoreLib.Entities.Users.Interfaces;
using UHub.CoreLib.ClientFriendly;
using UHub.CoreLib.Security.Authentication;
using UHub.CoreLib.Entities.Users;
using System.Web;

namespace UHub.CoreLib.APIControllers
{
    /// <summary>
    /// Base controller for CMS API operations. Wraps validation and authentication
    /// </summary>
    public abstract class APIController : ApiController
    {
        /// <summary>
        /// Custom constructor to inject a Cms state Manager into API systems
        /// </summary>
        /// <param name="Manager"></param>
        public APIController()
        {

        }

        /// <summary>
        /// Validate that the CMS system is enabled and the requested API is available
        /// </summary>
        protected virtual bool ValidateSystemState(out string result, out HttpStatusCode resultCode)
        {
            //Check if system is enabled
            if (!CoreFactory.Singleton.IsEnabled)
            {
                result = "System Disabled";
                resultCode = HttpStatusCode.ServiceUnavailable;
                return false;
            }

            //CHECK HTTPS STATUS
            if (Request.RequestUri.Scheme != Uri.UriSchemeHttps)
            {
                result = "Must Use HTTPS";
                resultCode = HttpStatusCode.UpgradeRequired;
                return false;
            }

            result = "System Validated";
            resultCode = HttpStatusCode.OK;
            return true;
        }

        /// <summary>
        /// Check if user is logged in.  Checks both cookie and token authentication
        /// </summary>
        /// <param name="currentUser">Capture current user (or Anon)</param>
        /// <param name="tokenStatus">Capture token validation status</param>
        /// <returns></returns>
        protected bool IsUserLoggedIn(out User currentUser, out TokenValidationStatus tokenStatus)
        {

            bool isLoggedIn = false;
            string authToken = Request.Headers.Where(x => x.Key == Constants.AUTH_HEADER_TOKEN).SingleOrDefault().Value?.First();

            if (authToken.IsEmpty())
            {
                //test for cookie auth
                var userStatus = CoreFactory.Singleton.Auth.GetCurrentUser();
                currentUser = userStatus.CmsUser;

                tokenStatus = userStatus.TokenStatus;
                isLoggedIn = (currentUser.ID != null);
            }
            else
            {
                //test for token auth
                tokenStatus = CoreFactory.Singleton.Auth.TrySetRequestUser(authToken);

                currentUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;
                isLoggedIn = (currentUser.ID != null);
            }


            return isLoggedIn;
        }


        protected string GetRawAuthToken()
        {
            //get header
            var isValid = Request.Headers.TryGetValues(Constants.AUTH_HEADER_TOKEN, out var valueSet);
            string authToken = "";

            if (isValid && valueSet != null)
            {
                authToken = valueSet.FirstOrDefault();
                return authToken;
            }


            //get cookie
            var authTknCookieName = CoreFactory.Singleton.Properties.AuthTknCookieName;
            authToken = Request.Headers.GetCookies(authTknCookieName).FirstOrDefault()?.Cookies?.FirstOrDefault()?.Value;
            if (authToken.IsEmpty())
            {
                return null;
            }

            return authToken;

        }


        /// <summary>
        /// Throw error if Recaptcha status is not valid
        /// </summary>
        protected async Task<(bool IsValid, string Result)> HandleRecaptchaAsync(HttpContext Context)
        {
            if (!CoreFactory.Singleton.Properties.EnableRecaptcha)
            {
                return (true, "Recaptcha Not Required");
            }


            try
            {
                string captchaResponse = Request.Headers.Where(x => x.Key == RecaptchaManager.RECAPTCHA_HEADER).SingleOrDefault().Value?.First();

                if (captchaResponse == null)
                {
                    return (false, "Recaptcha Failed");
                }

                if (captchaResponse.IsEmpty())
                {
                    return (false, ResponseStrings.RecaptchaError.RECAPTCHA_MISSING);
                }

                var isValid = await CoreFactory.Singleton.Recaptcha.IsCaptchaValidAsync(captchaResponse, Context);

                if (!isValid)
                {
                    return (false, ResponseStrings.RecaptchaError.RECAPTCHA_INVALID);
                }


                return (true, "Recaptcha Validated");

            }
            catch(Exception ex)
            {
                var exID = new Guid("2773EC77-FA18-4445-9EBB-41780638D993");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);

                return (false, "Recaptcha Failed");
            }

        }


        /// <summary>
        /// Throw proper errors for invalid user tokens (applies to both token forms)
        /// </summary>
        /// <param name="tokenStatus">Token validation status that needs to be processed</param>
        protected void HandleFatalTokenStatus(TokenValidationStatus tokenStatus)
        {

            if (tokenStatus == TokenValidationStatus.Success)
            {
                return;
            }

            switch (tokenStatus)
            {
                case TokenValidationStatus.TokenNotFound:
                case TokenValidationStatus.TokenAESFailure:
                    {
                        throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Forbidden, "User Not Authenticated"));
                    }
                case TokenValidationStatus.TokenInvalid:
                case TokenValidationStatus.TokenSessionMismatch:
                case TokenValidationStatus.TokenValidatorInvalid:
                case TokenValidationStatus.TokenValidatorMismatch:
                case TokenValidationStatus.TokenValidatorNotFound:
                    {
                        throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Forbidden, "User Token Invalid"));
                    }
                case TokenValidationStatus.TokenExpired:
                case TokenValidationStatus.TokenVersionMismatch:
                    {
                        throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Forbidden, "User Token Expired"));
                    }
                case TokenValidationStatus.TokenUserError:
                    {
                        throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Forbidden, "User Account Invalid"));
                    }
                case TokenValidationStatus.TokenUserDisabled:
                    {
                        throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Forbidden, "User Account Disabled"));
                    }
                case TokenValidationStatus.TokenUserForbidden:
                    {
                        throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Forbidden, "User Account Forbidden"));
                    }
                case TokenValidationStatus.TokenUserNotConfirmed:
                    {
                        throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Forbidden, "User Account Not Confirmed"));
                    }
                case TokenValidationStatus.TokenUserNotApproved:
                    {
                        throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Forbidden, "User Account Not Approved"));
                    }
            }

        }
    }
}
