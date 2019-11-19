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
using UHub.CoreLib.Entities.Posts;

namespace UHub.ApiControllers.Entities.Posts
{
    public sealed partial class PostController
    {
        [HttpPost()]
        [Route("GetPostCountByParent")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> GetPostCountByParent(long ParentID)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }

            var taskGetTargetClub = SchoolClubReader.TryGetClubAsync(ParentID);
            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;

            long? count = null;


            //parent is school
            if (ParentID == cmsUser.SchoolID)
            {
                count = await PostReader.TryGetPostCountByClubAsync(ParentID, true);
            }
            //parent is club
            else
            {
                var targetClub = await taskGetTargetClub;

                if (targetClub == null)
                {
                    return NotFound();
                }

                var taskIsUserBanned = SchoolClubReader.TryIsUserBannedAsync(ParentID, cmsUser.ID.Value);
                var taskIsUserMember = SchoolClubReader.TryValidateMembershipAsync(ParentID, cmsUser.ID.Value);

                if (targetClub.SchoolID != cmsUser.SchoolID)
                {
                    return NotFound();
                }

                var isUserMember = await taskIsUserMember;
                var taskGetCount = PostReader.TryGetPostCountByClubAsync(ParentID, isUserMember);

                if (await taskIsUserBanned)
                {
                    return Ok(0);
                }


                count = await taskGetCount;
            }

            if (count == null)
            {
                return InternalServerError();
            }

            return Ok(count);
        }


        [HttpPost()]
        [Route("GetPageCountByParent")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> GetPageCountByParent(long ParentID, short PageSize = DEFAULT_PAGE_SIZE)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }


            var taskGetTargetClub = SchoolClubReader.TryGetClubAsync(ParentID);
            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;

            long? count = null;


            //parent is school
            if (ParentID == cmsUser.SchoolID)
            {
                count = await PostReader.TryGetPostCountByClubAsync(ParentID, true);
            }
            //parent is club
            else
            {
                var targetClub = await taskGetTargetClub;

                if (targetClub != null)
                {

                }
                var taskIsUserBanned = SchoolClubReader.TryIsUserBannedAsync(ParentID, cmsUser.ID.Value);
                var taskIsUserMember = SchoolClubReader.TryValidateMembershipAsync(ParentID, cmsUser.ID.Value);

                if (targetClub.SchoolID != cmsUser.SchoolID)
                {
                    return NotFound();
                }

                var isUserMember = await taskIsUserMember;
                var taskGetCount = PostReader.TryGetPostCountByClubAsync(ParentID, isUserMember);

                if (await taskIsUserBanned)
                {
                    return Ok(0);
                }


                count = await taskGetCount;
            }

            if(count == null)
            {
                return InternalServerError();
            }
            var countVal = count.Value;


            if (countVal == 0)
            {
                return Ok(0);
            }
            if (PageSize == -1)
            {
                return Ok(1);
            }
            if (PageSize < 1)
            {
                double divResult = (countVal * 1.0) / DEFAULT_PAGE_SIZE;
                long ceil = (long)Math.Ceiling(divResult);
                return Ok(ceil);
            }


            double divResult2 = (countVal * 1.0) / PageSize;
            long ceil2 = (long)Math.Ceiling(divResult2);
            return Ok(ceil2);
        }


        [HttpPost()]
        [Route("GetAllByParent")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> GetAllByParent(long ParentID)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }


            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;


            IEnumerable<Post> posts = null;

            var taskUsers = UserReader.GetAllBySchoolAsync(cmsUser.SchoolID.Value);

            if (ParentID == cmsUser.SchoolID)
            {
                posts = await PostReader.TryGetPostsByParentAsync(ParentID);

                if (posts == null)
                {
                    return InternalServerError();
                }
            }
            else
            {
                var taskTargetClub = SchoolClubReader.TryGetClubAsync(ParentID);
                var taskIsUserBanned = SchoolClubReader.TryIsUserBannedAsync(ParentID, cmsUser.ID.Value);
                var taskIsUserMember = SchoolClubReader.TryValidateMembershipAsync(ParentID, cmsUser.ID.Value);

                await Task.WhenAll(taskTargetClub, taskIsUserBanned);

                var targetClub = taskTargetClub.Result;
                bool IsUserBanned = taskIsUserBanned.Result;
                //verify same school
                if (targetClub.SchoolID != cmsUser.SchoolID)
                {
                    return NotFound();
                }

                //ensure not banned
                if (IsUserBanned)
                {
                    return Content(HttpStatusCode.Forbidden, "Access Denied");
                }

                posts = await PostReader.TryGetPostsByParentAsync(ParentID);
                if (posts == null)
                {
                    return InternalServerError();
                }



                bool IsUserMember = await taskIsUserMember;
                //check for member status
                if (!IsUserMember)
                {
                    posts= posts.Where(x => x.IsPublic);
                }
            }


            var sanitizerMode = CoreFactory.Singleton.Properties.HtmlSanitizerMode;
            var shouldSanitize = (sanitizerMode & HtmlSanitizerMode.OnRead) != 0;

            posts = posts.AsParallel();
            if (shouldSanitize)
            {
                posts = posts
                    .Select(x =>
                    {
                        x.Content = x.Content.SanitizeHtml().HtmlDecode();
                        return x;
                    });
            }

            await taskUsers;
            var userNameDict = taskUsers.Result.ToDictionary(key => key.ID, val => val.Username);


            var outSetWithUser = posts
                .Select(post =>
                {
                    var Username = userNameDict[post.CreatedBy];
                    return new
                    {
                        post.ID,
                        post.IsReadOnly,
                        post.Name,
                        post.Content,
                        post.IsModified,
                        post.ViewCount,
                        post.IsLocked,
                        post.CanComment,
                        post.IsPublic,
                        post.ParentID,
                        post.CreatedBy,
                        post.CreatedDate,
                        post.ModifiedBy,
                        post.ModifiedDate,
                        Username
                    };
                });

            return Ok(outSetWithUser);
        }


        [HttpPost()]
        [Route("GetPageByParent")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> GetPageByParent(long ParentID, long? StartID = null, int? PageNum = null, short PageSize = DEFAULT_PAGE_SIZE)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }

            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;


            IEnumerable<Post> posts = null;

            if (ParentID == cmsUser.SchoolID)
            {
                posts = await PostReader.TryGetPostsByParentPageAsync(ParentID,StartID, PageNum, PageSize);
                if (posts == null)
                {
                    return InternalServerError();
                }
            }
            else
            {
                var taskTargetClub = SchoolClubReader.TryGetClubAsync(ParentID);
                var taskIsUserBanned = SchoolClubReader.TryIsUserBannedAsync(ParentID, cmsUser.ID.Value);
                var taskIsUserMember = SchoolClubReader.TryValidateMembershipAsync(ParentID, cmsUser.ID.Value);

                await Task.WhenAll(taskTargetClub, taskIsUserBanned);

                var targetClub = taskTargetClub.Result;
                bool IsUserBanned = taskIsUserBanned.Result;
                //verify same school
                if (targetClub.SchoolID != cmsUser.SchoolID)
                {
                    return NotFound();
                }

                //ensure not banned
                if (IsUserBanned)
                {
                    return Content(HttpStatusCode.Forbidden, "Access Denied");
                }

                posts = await PostReader.TryGetPostsByParentPageAsync(ParentID, StartID, PageNum, PageSize);
                if (posts == null)
                {
                    return InternalServerError();
                }


                bool IsUserMember = await taskIsUserMember;
                //check for member status
                if (!IsUserMember)
                {
                    posts = posts.Where(x => x.IsPublic);
                }
            }


            var sanitizerMode = CoreFactory.Singleton.Properties.HtmlSanitizerMode;
            var shouldSanitize = (sanitizerMode & HtmlSanitizerMode.OnRead) != 0;

            IEnumerable<Post_R_PublicDTO> outSet = null;
            if (shouldSanitize)
            {
                outSet = posts
                    .AsParallel()
                    .Select(x =>
                    {
                        x.Content = x.Content.SanitizeHtml().HtmlDecode();
                        return x.ToDto<Post_R_PublicDTO>();
                    });
            }
            else
            {
                outSet = posts
                    .AsParallel()
                    .Select(x =>
                    {
                        return x.ToDto<Post_R_PublicDTO>();
                    });
            }


            //get the highest ent ID from data set
            //this is the pagination StartID
            //used to lock paging to the entities available at initial invocation
            var maxId = outSet.Max(x => x.ID);
            var procSet = new
            {
                StartID = maxId,
                Data = outSet
            };

            return Ok(procSet);
        }


    }
}
