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
using UHub.CoreLib.Entities.SchoolClubs.DataInterop;
using UHub.CoreLib.Entities.Users.DataInterop;
using UHub.CoreLib.Management;
using UHub.CoreLib.Security;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Entities.Posts.Management;
using UHub.CoreLib.Entities.Posts;

namespace UHub.ApiControllers.Entities.Posts
{
    public sealed partial class PostController
    {
        [HttpPost()]
        [Route("Create")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> Create([FromBody] Post_C_PublicDTO post)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }

            if (post == null)
            {
                return BadRequest();
            }


            var tmpPost = post.ToInternal<Post>();
            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;



            var taskIsValidParent = PostReader.TryValidatePostParentAsync((long)cmsUser.ID, tmpPost.ParentID);
            var taskIsUserBanned = SchoolClubReader.TryIsUserBannedAsync(post.ParentID, cmsUser.ID.Value);



            var isValidParent = await taskIsValidParent;

            if (!isValidParent)
            {
                status = "Access Denied";
                statCode = HttpStatusCode.Forbidden;
                return Content(statCode, status);
            }

            if (await taskIsUserBanned)
            {
                status = "Access Denied";
                statCode = HttpStatusCode.Forbidden;
                return Content(statCode, status);
            }


            status = "Failed to Create Post.";
            statCode = HttpStatusCode.BadRequest;

            try
            {
                tmpPost.CreatedBy = cmsUser.ID.Value;


                var postResult = await PostManager.TryCreatePostAsync(tmpPost);
                var ResultCode = postResult.ResultCode;
                var PostID = postResult.PostID;



                if (ResultCode == 0)
                {
                    status = PostID.ToString();
                    statCode = HttpStatusCode.OK;
                }
                else if (ResultCode == PostResultCode.UnknownError)
                {
                    status = "Unknown Server Error";
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
                var exID = new Guid("D8D1B694-F7AA-4D98-A832-0F0A220D4871");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);

                statCode = HttpStatusCode.InternalServerError;
            }


            return Content(statCode, status);

        }


    }
}
