using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using UHub.CoreLib;
using UHub.CoreLib.APIControllers;
using UHub.CoreLib.Entities.Users;
using UHub.CoreLib.Entities.Users.DTOs;
using UHub.CoreLib.Management;
using UHub.CoreLib.Security.Accounts;

namespace UHub.ApiControllers.Security.Accounts
{
    [RoutePrefix(Constants.API_ROUTE_PREFIX + "/account")]
    public sealed class AccountController : APIController
    {
        protected override bool ValidateSystemState(out string status, out HttpStatusCode statCode)
        {
            if (!base.ValidateSystemState(out status, out statCode))
            {
                return false;
            }

            return true;
        }


        [HttpPost()]
        [Route("CreateUser")]
        public async Task<IHttpActionResult> CreateUser([FromBody] User_C_PublicDTO User)
        {


            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                var resultObj = new
                {
                    status,
                    canLogin = false
                };
                return Content(statCode, resultObj);
            }

            var context = System.Web.HttpContext.Current;
            var recaptchaResult = await HandleRecaptchaAsync(context);
            if (!recaptchaResult.IsValid)
            {
                var recapObj = new
                {
                    status = recaptchaResult.Result,
                    canLogin = false
                };
                return Content(HttpStatusCode.BadRequest, recapObj);
            }

            if (User == null)
            {
                return BadRequest();
            }

            var tmpUser = User.ToInternal<User>();



            var enableDetail = CoreFactory.Singleton.Properties.EnableDetailedAPIErrors;
            var enableFailCode = CoreFactory.Singleton.Properties.EnableInternalAPIErrors;
            status = "Account Creation Failed";
            statCode = HttpStatusCode.BadRequest;
            bool userCanLogin = false;


            AcctCreateResultCode resultCode = AcctCreateResultCode.UnknownError;
            try
            {
                resultCode = await CoreFactory.Singleton.Accounts.TryCreateUserAsync(
                    tmpUser,
                    true,
                    SuccessHandler: (newUser, canLogin) =>
                    {
                        status = "User Created";
                        statCode = HttpStatusCode.OK;
                        userCanLogin = canLogin;
                    });
            }
            catch (Exception ex)
            {
                var exID = new Guid("68934E91-EC89-41A4-A25A-C985496B99AA");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
           
            }



            var isCreated = (resultCode == AcctCreateResultCode.Success);

            if (resultCode == AcctCreateResultCode.UnknownError)
            {
                return Content(HttpStatusCode.InternalServerError, resultCode.ToString());
            }



            if (!isCreated && enableDetail)
            {
                status = resultCode.ToString();
            }



            var responseObj = new
            {
                status,
                canLogin = userCanLogin
            };

            return Content(statCode, responseObj);

        }


    }
}
