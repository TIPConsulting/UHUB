using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.Management;

namespace UHub.Controllers
{
    public class PostController : Controller
    {
        const string POST_PREVIEW_POSTFIX = "preview";


        [System.Web.Mvc.HttpGet]
        [MvcAuthControl]
        public async Task<ActionResult> Index()
        {
            var idObj = Url.RequestContext.RouteData.Values["id"];
            var idStr = idObj?.ToString() ?? "";
            var valid = long.TryParse(idStr, out var postId);

            if (!valid)
            {
                return Redirect("~/Error/400");
            }


            var taskPost = CoreLib.Entities.Posts.DataInterop.PostReader.TryGetPostAsync(postId);
            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;
            var post = await taskPost;


            if (post == null)
            {
                return Redirect("~/Error/404");
            }


            //get querystring value without key
            bool enableUpdatable = true;
            var querySet = Request.QueryString[null];
            if (querySet != null)
            {
                enableUpdatable = !querySet.ToLower().Split(',').Contains(POST_PREVIEW_POSTFIX);
            }


            if (post.CreatedBy == cmsUser.ID.Value && enableUpdatable)
            {
                return View("Index_Updatable");
            }
            else
            {
                return View();
            }


        }


        [System.Web.Mvc.HttpGet]
        [MvcAuthControl]
        public async Task<ActionResult> History()
        {
            var idObj = Url.RequestContext.RouteData.Values["id"];
            var idStr = idObj?.ToString() ?? "";
            var valid = long.TryParse(idStr, out var postId);

            if (!valid)
            {
                return Redirect("~/Error/400");
            }


            var post = await CoreLib.Entities.Posts.DataInterop.PostReader.TryGetPostAsync(postId);


            if (post == null)
            {
                return Redirect("~/Error/404");
            }


            return View();
        }
    }
}