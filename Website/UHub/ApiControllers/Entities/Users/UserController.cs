using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using UHub.CoreLib.APIControllers;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.Entities.Users.DTOs;
using UHub.CoreLib.Entities.Users.DataInterop;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Management;
using UHub.CoreLib;
using UHub.CoreLib.Entities.Users;

namespace UHub.ApiControllers.Entities.Users
{
    [RoutePrefix(Constants.API_ROUTE_PREFIX + "/users")]
    public sealed class UserController : APIController
    {
        protected override bool ValidateSystemState(out string status, out HttpStatusCode statCode)
        {
            if (!base.ValidateSystemState(out status, out statCode))
            {
                return false;
            }
            return true;
        }


        [Route("GetMe")]
        [HttpPost]
        [ApiAuthControl]
        public IHttpActionResult GetMe()
        {
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out string status, out statCode))
            {
                return Content(statCode, status);
            }


            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;

            var dtoUser = cmsUser.ToDto<User_R_PrivateDTO>();

            return Ok(dtoUser);

        }


        [Route("GetByUname")]
        [HttpPost]
        [ApiAuthControl]
        public async Task<IHttpActionResult> GetByUname(string Username)
        {
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out string status, out statCode))
            {
                return Content(statCode, status);
            }

            var (TokenStatus, CmsUser) = CoreFactory.Singleton.Auth.GetCurrentUser();
            var cmsUser = CmsUser;


            var domain = cmsUser.Email.GetEmailDomain();


            //search for user
            User targetUser = null;
            try
            {
                targetUser = await UserReader.GetUserAsync(Username, domain);
            }
            catch (Exception ex)
            {
                var exID = new Guid("D8EB78E4-3C48-4976-A234-6B5EACDC053A");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return InternalServerError();
            }


            //ensure user is found
            if (targetUser == null)
            {
                return NotFound();
            }
            //ensure user is tied to a school
            if (targetUser.SchoolID == null)
            {
                return NotFound();
            }

            //Return full detail if user requests self
            if (targetUser.ID == cmsUser.ID)
            {
                return Ok(targetUser.ToDto<User_R_PrivateDTO>());
            }

            //only allow users to see users from same school
            if (targetUser.SchoolID != cmsUser.SchoolID)
            {
                return NotFound();
            }


            //otherwise
            //return partial detail
            return Ok(targetUser.ToDto<User_R_PublicDTO>());

        }


        [Route("GetByID")]
        [HttpPost]
        [ApiAuthControl]
        public async Task<IHttpActionResult> GetByID(long UserID)
        {
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out string status, out statCode))
            {
                return Content(statCode, status);
            }

            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;


            //search for user
            User targetUser = null;
            try
            {
                targetUser = await UserReader.GetUserAsync(UserID);
            }
            catch (Exception ex)
            {
                var exID = new Guid("D2D5D240-FEBB-43A0-8B27-2B44AB28AEF7");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return InternalServerError();
            }


            //ensure user is found
            if (targetUser == null)
            {
                return NotFound();
            }
            //ensure user is tied to a school
            if (targetUser.SchoolID == null)
            {
                return NotFound();
            }

            //Return full detail if user requests self
            if (targetUser.ID == cmsUser.ID)
            {
                return Ok(targetUser.ToDto<User_R_PrivateDTO>());
            }

            //only allow users to see users from same school
            if (targetUser.SchoolID != cmsUser.SchoolID)
            {
                return NotFound();
            }

            //otherwise
            //return partial detail
            return Ok(targetUser.ToDto<User_R_PublicDTO>());

        }

    }
}
