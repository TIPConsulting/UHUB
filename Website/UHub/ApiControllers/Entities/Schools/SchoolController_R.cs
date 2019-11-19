using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using UHub.CoreLib.APIControllers;
using UHub.CoreLib.Entities.Schools.Interfaces;
using UHub.CoreLib.Entities.Schools.DataInterop;
using UHub.CoreLib.Entities.Schools.DTOs;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Entities.Users;
using UHub.CoreLib.Entities.Users.DTOs;
using UHub.CoreLib.Entities.Users.Interfaces;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.Management;
using UHub.CoreLib;

namespace UHub.ApiControllers.Entities.Schools
{



    [RoutePrefix(Constants.API_ROUTE_PREFIX + "/schools")]
    public class SchoolController : APIController
    {

        [HttpGet()]
        [Route("GetAll")]
        [ApiCacheControl(12 * 3600)]
        public async Task<IHttpActionResult> GetAll()
        {

            var schoolSet = await SchoolReader.TryGetAllSchoolsAsync();
            if (schoolSet == null)
            {
                return InternalServerError();
            }

            return Ok(schoolSet.Select(x => x.ToDto<School_R_PublicDTO>()));

        }


        [HttpGet()]
        [Route("GetByID")]
        [ApiCacheControl(12 * 3600)]
        public async Task<IHttpActionResult> GetByID(long SchoolID)
        {

            var school = await SchoolReader.TryGetSchoolAsync(SchoolID);

            if (school == null)
            {
                return NotFound();
            }


            return Ok(school.ToDto<School_R_PublicDTO>());

        }


        [HttpGet()]
        [Route("IsEmailValid")]
        [ApiCacheControl(1 * 3600)]
        public async Task<IHttpActionResult> IsEmailValid(string Email)
        {
            if (await SchoolReader.TryIsEmailValidAsync(Email))
            {
                return BadRequest();
            }
            else
            {
                return Ok();
            }

        }


        [HttpGet()]
        [Route("IsDomainValid")]
        [ApiCacheControl(1 * 3600)]
        public async Task<IHttpActionResult> IsDomainValid(string domain)
        {
            if (await SchoolReader.TryIsDomainValidAsync(domain))
            {
                return BadRequest();
            }
            else
            {
                return Ok();
            }

        }


        [HttpGet()]
        [Route("GetMine")]
        [ApiCacheControl(12 * 3600)]
        public async Task<IHttpActionResult> GetMine()
        {
            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;


            var school = await SchoolReader.TryGetSchoolAsync(cmsUser.SchoolID.Value);
            if (school == null)
            {
                return InternalServerError();
            }


            return Ok(school.ToDto<School_R_PublicDTO>());

        }

    }
}
