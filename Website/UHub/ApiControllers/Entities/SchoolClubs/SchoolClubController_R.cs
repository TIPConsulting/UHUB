using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using UHub.CoreLib.APIControllers;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.Entities.Posts.DTOs;
using UHub.CoreLib.Entities.Posts.DataInterop;
using UHub.CoreLib.Entities.SchoolClubs;
using UHub.CoreLib.Entities.SchoolClubs.DTOs;
using UHub.CoreLib.Entities.SchoolClubs.DataInterop;
using UHub.CoreLib.Entities.Users.DataInterop;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Management;
using UHub.CoreLib.Tools.Extensions;

namespace UHub.ApiControllers.Entities.SchoolClubs
{
    public sealed partial class SchoolClubController
    {

        [HttpPost]
        [Route("GetAllBySchool")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> GetAllBySchool()
        {

            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;

            var schoolID = cmsUser.SchoolID.Value;

            var clubSet = await SchoolClubReader.TryGetClubsBySchoolAsync(schoolID);
            if (clubSet == null)
            {
                return InternalServerError();
            }


            return Ok(clubSet.Select(x => x.ToDto<SchoolClub_R_PublicDTO>()));
        }


        [HttpPost]
        [Route("GetByID")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> GetByID(long ClubID)
        {

            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;

            var schoolID = cmsUser.SchoolID.Value;


            var clubInternal = await SchoolClubReader.TryGetClubAsync(ClubID);
            if (clubInternal == null)
            {
                return NotFound();
            }


            if(clubInternal.SchoolID != schoolID)
            {
                return NotFound();
            }


            return Ok(clubInternal.ToDto<SchoolClub_R_PublicDTO>());
        }

    }
}
