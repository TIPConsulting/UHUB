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
        [Route("Update")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> Update([FromBody] Post_U_PublicDTO PostDTO)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }

            if (PostDTO == null)
            {
                return BadRequest();
            }
            if (PostDTO.ID == null)
            {
                return BadRequest();
            }

            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;



            var PostInternal = await PostReader.TryGetPostAsync(PostDTO.ID.Value);

            if (PostInternal == null)
            {
                return BadRequest();
            }

            if (PostInternal.CreatedBy != cmsUser.ID.Value)
            {
                return Content(HttpStatusCode.Forbidden, "Access Denied");
            }


            var taskIsValidParent = PostReader.TryValidatePostParentAsync((long)cmsUser.ID, PostInternal.ParentID);
            var taskIsUserBanned = SchoolClubReader.TryIsUserBannedAsync(PostInternal.ParentID, cmsUser.ID.Value);


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

                PostInternal.Name = PostDTO.Name;
                PostInternal.Content = PostDTO.Content;
                PostInternal.CanComment = PostDTO.CanComment;
                PostInternal.IsPublic = PostDTO.IsPublic;

                PostInternal.ModifiedBy = cmsUser.ID.Value;



                var postResult = await PostManager.TryUpdatePostAsync(PostInternal);
                var ResultCode = postResult;


                if (ResultCode == 0)
                {
                    status = "Post Updated";
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
                var exID = new Guid("2EAA1B03-A569-4A16-A77F-403F7DB7CA36");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);

                statCode = HttpStatusCode.InternalServerError;
            }


            return Content(statCode, status);

        }



        [HttpPost()]
        [Route("CreateLike")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> CreateLike(long PostID)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }


            var postInternal = await PostReader.TryGetPostAsync(PostID);
            if (postInternal == null)
            {
                return NotFound();
            }

            var taskPostCreateUser = UserReader.GetUserAsync(postInternal.CreatedBy);
            var parentID = postInternal.ParentID;
            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;



            var taskPostClub = SchoolClubReader.TryGetClubAsync(parentID);
            var taskIsUserBanned = SchoolClubReader.TryIsUserBannedAsync(parentID, cmsUser.ID.Value);
            var taskIsUserMember = SchoolClubReader.TryValidateMembershipAsync(parentID, cmsUser.ID.Value);



            var postClub = await taskPostClub;
            if (postClub != null)
            {
                //verify same school
                if (postClub.SchoolID != cmsUser.SchoolID)
                {
                    return NotFound();
                }

                var IsUserBanned = await taskIsUserBanned;
                //ensure not banned
                if (IsUserBanned)
                {
                    return Content(HttpStatusCode.Forbidden, "Access Denied");
                }


                var IsUserMember = await taskIsUserMember;

                //check for member status
                if (IsUserMember || postInternal.IsPublic)
                {
                    var stat = await PostManager.TryCreateUserLikeAsync(PostID, cmsUser.ID.Value);
                    if (stat == true)
                    {
                        return Ok();
                    }
                    else if (stat == false)
                    {
                        return BadRequest();
                    }
                    else
                    {
                        return InternalServerError();
                    }
                }
                else
                {
                    return Content(HttpStatusCode.Forbidden, "Access Denied");
                }
            }
            else
            {


                // This is what happens if the parent is a school.
                //verify same school
                if (postInternal.ParentID != cmsUser.SchoolID)
                {
                    return NotFound();
                }


                var stat = await PostManager.TryCreateUserLikeAsync(PostID, cmsUser.ID.Value);
                if (stat == true)
                {
                    return Ok();
                }
                else if (stat == false)
                {
                    return BadRequest();
                }
                else
                {
                    return InternalServerError();
                }



            }

        }



    }
}
