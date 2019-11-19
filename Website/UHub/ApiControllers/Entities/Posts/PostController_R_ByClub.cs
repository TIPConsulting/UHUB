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

namespace UHub.ApiControllers.Entities.Posts
{
    public sealed partial class PostController
    {
        [HttpPost()]
        [Route("GetPostCountByClub")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> GetPostCountByClub(long ClubID)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }


            var taskGetTargetClub = SchoolClubReader.TryGetClubAsync(ClubID);
            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;


            var taskIsUserBanned = SchoolClubReader.TryIsUserBannedAsync(ClubID, cmsUser.ID.Value);
            var taskIsUserMember = SchoolClubReader.TryValidateMembershipAsync(ClubID, cmsUser.ID.Value);



            var targetClub = await taskGetTargetClub;

            if (targetClub == null)
            {
                return NotFound();
            }

            if (targetClub.SchoolID != cmsUser.SchoolID)
            {
                return NotFound();
            }

            var isUserMember = await taskIsUserMember;
            var taskGetCount = PostReader.TryGetPostCountByClubAsync(ClubID, isUserMember);


            if (await taskIsUserBanned)
            {
                return Ok(0);
            }


            var count = await taskGetCount;
            if (count == null)
            {
                return InternalServerError();
            }
            var countVal = count.Value;

            return Ok(countVal);
        }


        [HttpPost()]
        [Route("GetPageCountByClub")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> GetPageCountByClub(long ClubID, short PageSize = DEFAULT_PAGE_SIZE)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }


            var taskGetTargetClub = SchoolClubReader.TryGetClubAsync(ClubID);
            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;
            var taskIsUserMember = SchoolClubReader.TryValidateMembershipAsync(ClubID, cmsUser.ID.Value);


            var targetClub = await taskGetTargetClub;
            if (targetClub == null)
            {
                return NotFound();
            }

            if (targetClub.SchoolID != cmsUser.SchoolID)
            {
                return NotFound();
            }

            var isUserMember = await taskIsUserMember;
            var taskGetCount = PostReader.TryGetPostCountByClubAsync(ClubID, isUserMember);


            var count = await taskGetCount;
            if (count == null)
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
        [Route("GetAllByClub")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> GetAllByClub(long ClubID)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }
            
            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;


            var taskTargetClub = SchoolClubReader.TryGetClubAsync(ClubID);
            var taskIsUserBanned = SchoolClubReader.TryIsUserBannedAsync(ClubID, cmsUser.ID.Value);
            var taskIsUserMember = SchoolClubReader.TryValidateMembershipAsync(ClubID, cmsUser.ID.Value);


            await Task.WhenAll(taskTargetClub, taskIsUserBanned);
            var targetClub = taskTargetClub.Result;
            var IsUserBanned = taskIsUserBanned.Result;

            if(targetClub == null)
            {
                return NotFound();
            }

            //verify same school
            if (targetClub.SchoolID != cmsUser.SchoolID.Value)
            {
                return NotFound();
            }

            //ensure not banned
            if (IsUserBanned)
            {
                return Content(HttpStatusCode.Forbidden, "Access Denied");
            }


            var taskPosts = PostReader.TryGetPostsByClubAsync(ClubID);
            var taskUsers = UserReader.GetAllBySchoolAsync(cmsUser.SchoolID.Value);
            var sanitizerMode = CoreFactory.Singleton.Properties.HtmlSanitizerMode;
            var shouldSanitize = (sanitizerMode & HtmlSanitizerMode.OnRead) != 0;

            var posts = await taskPosts;
            if (posts == null)
            {
                return InternalServerError();
            }


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

            //check for member status
            var isUserMember = await taskIsUserMember;
            if (!isUserMember)
            {
                posts = posts.Where(x => x.IsPublic);
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
        [Route("GetPageByClub")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> GetPageByClub(long ClubID, long? StartID = null, int? PageNum = null, short PageSize = DEFAULT_PAGE_SIZE)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }


            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;


            var taskTargetClub = SchoolClubReader.TryGetClubAsync(ClubID);
            var taskIsUserBanned = SchoolClubReader.TryIsUserBannedAsync(ClubID, cmsUser.ID.Value);
            var taskIsUserMember = SchoolClubReader.TryValidateMembershipAsync(ClubID, cmsUser.ID.Value);

            await Task.WhenAll(taskTargetClub, taskIsUserBanned);
            var targetClub = await taskTargetClub;
            var isUserBanned = await taskIsUserBanned;


            //verify same school
            if (targetClub.SchoolID != cmsUser.SchoolID)
            {
                return NotFound();
            }

            //ensure not banned
            if (isUserBanned)
            {
                return Content(HttpStatusCode.Forbidden, "Access Denied");
            }


            var taskPosts = PostReader.TryGetPostsByClubPageAsync(ClubID, StartID, PageNum, PageSize);

            var sanitizerMode = CoreFactory.Singleton.Properties.HtmlSanitizerMode;
            var shouldSanitize = (sanitizerMode & HtmlSanitizerMode.OnRead) != 0;

            var posts = await taskPosts;
            if(posts == null)
            {
                return InternalServerError();
            }


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


            //check for member status
            var isUserMember = await taskIsUserMember;
            if (!isUserMember)
            {
                outSet = outSet.Where(x => x.IsPublic);
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
