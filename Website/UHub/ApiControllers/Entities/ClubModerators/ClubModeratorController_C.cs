using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.Entities.ClubModerators.DTOs;
using UHub.CoreLib.Entities.ClubModerators.DataInterop;
using UHub.CoreLib.Entities.SchoolClubs.DataInterop;
using UHub.CoreLib.Entities.Users.DataInterop;
using UHub.CoreLib.Management;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Entities.ClubModerators.Management;
using UHub.CoreLib.Entities.ClubModerators;

namespace UHub.ApiControllers.Entities.ClubModerators
{

    public sealed partial class ClubModeratorController
    {
        [HttpPost()]
        [Route("Create")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> Create([FromBody] ClubModerator_C_PublicDTO ClubModerator, long ClubID)
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
                return Content(statCode, recaptchaResult.Result);
            }

            if (ClubModerator == null)
            {
                return BadRequest();
            }



            var tmpClubModerator = ClubModerator.ToInternal<ClubModerator>();
            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;


            var taskIsCurrentUserOwner = ClubModeratorReader.TryValidateClubModeratorAsync(ClubID, (long)cmsUser.ID);
            var taskIsCurrentUserBanned = SchoolClubReader.TryIsUserBannedAsync(ClubID, (long)cmsUser.ID);
            var taskIsNewUserBanned = SchoolClubReader.TryIsUserBannedAsync(ClubID, tmpClubModerator.UserID);

            await Task.WhenAll(taskIsCurrentUserOwner, taskIsCurrentUserBanned, taskIsNewUserBanned);
            var isCurrentUserBanned = taskIsCurrentUserBanned.Result;
            var isCurrentUserOwner = taskIsCurrentUserOwner.Result;
            var isNewUserBanned = taskIsNewUserBanned.Result;



            if (isCurrentUserBanned || !isCurrentUserOwner)
            {
                status = "Access Denied";
                statCode = HttpStatusCode.Forbidden;
                return Content(statCode, status);
            }

            if (isNewUserBanned)
            {
                status = "User is banned";
                statCode = HttpStatusCode.Forbidden;
                return Content(statCode, status);
            }


            status = "Failed to create post.";
            statCode = HttpStatusCode.BadRequest;

            try
            {

                var clubModResult = await ClubModeratorManager.TryCreateClubModeratorAsync(tmpClubModerator, ClubID);
                var clubModID = clubModResult.ClubModID;
                var ResultCode = clubModResult.ResultCode;


                if (ResultCode == 0)
                {
                    status = "Club moderator created";
                    statCode = HttpStatusCode.OK;
                }
                else if(ResultCode == ClubModeratorResultCode.UnknownError)
                {
                    status = "Unknown server error";
                    statCode = HttpStatusCode.InternalServerError;
                }
                else
                {
                    status = "Invalid Field - " + ResultCode.ToString();
                    statCode = HttpStatusCode.BadRequest;
                }

            }
            catch (Exception ex)
            {
                var exID = new Guid("69605919-C129-4409-BE24-3FFFBD702B39");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);

                statCode = HttpStatusCode.InternalServerError;
            }


            return Content(statCode, status);

        }
    }
}
