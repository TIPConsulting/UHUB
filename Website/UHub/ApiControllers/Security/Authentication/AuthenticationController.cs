using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.Threading.Tasks;
using System.Web.Http;
using res = System.Web.Http.Results;
using Newtonsoft;
using UHub.CoreLib.APIControllers;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.Entities.Users.DTOs;
using UHub.CoreLib;
using UHub.CoreLib.Security.Authentication;

namespace UHub.ApiControllers.Security.Authentication
{
    [RoutePrefix(Constants.API_ROUTE_PREFIX + "/auth")]
    public sealed class AuthenticationController : APIController
    {

        protected override bool ValidateSystemState(out string status, out HttpStatusCode statCode)
        {
            if (!base.ValidateSystemState(out status, out statCode))
            {
                return false;
            }

            return true;
        }


        [Route("GetToken")]
        [HttpPost()]
        public async Task<IHttpActionResult> GetToken([FromBody] User_CredentialDTO User, bool Persistent = false)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }

            var context = System.Web.HttpContext.Current;
            var recaptchaResult = await HandleRecaptchaAsync(context);
            if (!recaptchaResult.IsValid)
            {
                return Content(HttpStatusCode.BadRequest, recaptchaResult.Result);
            }

            if (User == null)
            {
                return BadRequest();
            }
            string email = User.Email;
            string password = User.Password;


            var enableDetail = CoreFactory.Singleton.Properties.EnableDetailedAPIErrors;
            var enableFailCode = CoreFactory.Singleton.Properties.EnableInternalAPIErrors;
            status = "Login Failed";
            statCode = HttpStatusCode.BadRequest;

            string token = null;
            try
            {

                //if token generation fails, then create an error message to send to client
                //if detailed errors are enabled, then give brief description
                //if not, then present the same error for all failure types
                var authResultSet = await CoreFactory.Singleton.Auth.TryGetClientAuthTokenAsync(
                    email,
                    password,
                    Persistent,
                    context);

                if (authResultSet.ResultCode == AuthResultCode.UnknownError)
                {
                    statCode = HttpStatusCode.InternalServerError;
                    return Content(HttpStatusCode.InternalServerError, status);
                }


                var resultCode = authResultSet.ResultCode;
                token = authResultSet.AuthToken;


                if (enableDetail)
                {
                    status = resultCode.ToString();
                }
                //this looks strange, but is relevant for a very specific edge case
                //if the auth worker emits a "Success" code without a populated token
                //then this will properly alert the user that some unknown internal error has occured
                if (resultCode == AuthResultCode.Success)
                {
                    statCode = HttpStatusCode.InternalServerError;
                }


            }
            catch (Exception ex)
            {
                var exID = new Guid("4717C1CF-7C2E-4596-9917-119FF7248B10");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);

                return Content(HttpStatusCode.InternalServerError, status);
            }



            if (token.IsNotEmpty())
            {
                return Ok(token);
            }
            else
            {
                return Content(statCode, status);
            }
        }


        [Route("ExtendToken")]
        [HttpPost()]
        [ApiAuthControl]
        public async Task<IHttpActionResult> ExtendToken()
        {
            string result = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out result, out statCode))
            {
                return Content(statCode, result);
            }

            var token = GetRawAuthToken();


            //in case of any error, fail silent and return the original token
            try
            {
                var context = HttpContext.Current;

                var resultSet = await CoreFactory.Singleton.Auth.TrySlideAuthTokenExpirationAsync(token, context);

                string newToken = resultSet.AuthToken;
                var status = resultSet.TokenStatus;
                if (status == TokenValidationStatus.Success)
                {
                    return Ok(newToken);
                }
                else
                {
                    return Content(HttpStatusCode.Forbidden, newToken);
                }
            }
            catch (Exception ex)
            {
                var exID = new Guid("7D136E21-6F6C-436B-89E3-9F57E6C0861D");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);


                //return original token
                return BadRequest(token);
            }



        }



    }
}
