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
using UHub.CoreLib;
using UHub.CoreLib.APIControllers;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.Entities.SchoolMajors.DataInterop;
using UHub.CoreLib.Entities.SchoolMajors.DTOs;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Tools.Extensions;

namespace UHub.ApiControllers.Entities.SchoolMajors
{
    [RoutePrefix(Constants.API_ROUTE_PREFIX + "/schoolmajors")]
    public class SchoolMajorController : APIController
    {

        [HttpGet]
        [Route("GetAllBySchool")]
        [ApiCacheControl(12 * 3600)]
        public async Task<IHttpActionResult> GetAllBySchool(long SchoolID)
        {

            var majorSet = await SchoolMajorReader.TryGetMajorsBySchoolAsync(SchoolID);
            if (majorSet == null)
            {
                return InternalServerError();
            }


            return Ok(majorSet.Select(x => x.ToDto<SchoolMajor_R_PublicDTO>()));

        }


        [HttpGet]
        [Route("GetAllByEmail")]
        [ApiCacheControl(12 * 3600)]
        public async Task<IHttpActionResult> GetAllByEmail(string Email)
        {
            if (!Validators.IsValidEmail(Email))
            {
                return BadRequest();
            }

            var majorSet = await SchoolMajorReader.TryGetMajorsByEmailAsync(Email);
            if (majorSet == null)
            {
                return InternalServerError();
            }


            return Ok(majorSet.Select(x => x.ToDto<SchoolMajor_R_PublicDTO>()));

        }


        [HttpGet]
        [Route("GetAllByDomain")]
        [ApiCacheControl(12 * 3600)]
        public async Task<IHttpActionResult> GetAllByDomain(string Domain)
        {
            if (!Validators.IsValidEmailDomain(Domain))
            {
                return BadRequest();
            }


            var majorSet = await SchoolMajorReader.TryGetMajorsByDomainAsync(Domain);
            if (majorSet == null)
            {
                return InternalServerError();
            }


            return Ok(majorSet.Select(x => x.ToDto<SchoolMajor_R_PublicDTO>()));

        }

    }
}
