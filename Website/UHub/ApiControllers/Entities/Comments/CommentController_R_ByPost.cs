using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.Entities.Comments.DTOs;
using UHub.CoreLib.Entities.Comments.DataInterop;
using UHub.CoreLib.Management;
using UHub.CoreLib.Entities.Posts.DataInterop;
using UHub.CoreLib.Entities.SchoolClubs.DataInterop;
using UHub.CoreLib.Entities.Users.DataInterop;
using UHub.CoreLib.Entities.Users.DTOs;

namespace UHub.ApiControllers.Entities.Comments
{
    public sealed partial class CommentController
    {
        [HttpPost()]
        [Route("GetByPost")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> GetByPost(long PostID)
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

            var parentID = postInternal.ParentID;
            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;



            var taskPostClub = SchoolClubReader.TryGetClubAsync(parentID);
            var taskIsUserBanned = SchoolClubReader.TryIsUserBannedAsync(parentID, cmsUser.ID.Value);
            var taskIsUserMember = SchoolClubReader.TryValidateMembershipAsync(parentID, cmsUser.ID.Value);
            var taskComments = CommentReader.TryGetCommentsByPostAsync(PostID);
            var taskUsers = PostReader.TryGetPostCommentersAsync(PostID);



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

                    await Task.WhenAll(taskComments, taskUsers);
                    var userNameDict = taskUsers.Result.ToDictionary(key => key.ID, val => val.Username);

                    var commentUserSet = taskComments.Result
                        .AsParallel()
                        .Select(comment =>
                        {
                            var Username = userNameDict[comment.CreatedBy];
                            return new
                            {
                                comment.ID,
                                comment.IsEnabled,
                                comment.IsReadOnly,
                                comment.Content,
                                comment.IsModified,
                                comment.ViewCount,
                                comment.ParentID,
                                comment.CreatedBy,
                                comment.CreatedDate,
                                comment.ModifiedBy,
                                comment.ModifiedDate,
                                Username
                            };
                        });

                    return Ok(commentUserSet);

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

                await Task.WhenAll(taskComments, taskUsers);
                var userNameDict = taskUsers.Result.ToDictionary(key => key.ID, val => val.Username);

                var commentUserSet = taskComments.Result
                    .AsParallel()
                    .Select(comment =>
                    {
                        var Username = userNameDict[comment.CreatedBy];
                        return new
                        {
                            comment.ID,
                            comment.IsEnabled,
                            comment.IsReadOnly,
                            comment.Content,
                            comment.IsModified,
                            comment.ViewCount,
                            comment.ParentID,
                            comment.CreatedBy,
                            comment.CreatedDate,
                            comment.ModifiedBy,
                            comment.ModifiedDate,
                            Username
                        };
                    });

                return Ok(commentUserSet);

            }

        }
    }
}
