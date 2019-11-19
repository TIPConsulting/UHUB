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
    public class SchoolClubController : Controller
    {
        [System.Web.Mvc.HttpGet]
        [MvcAuthControl]
        public async Task<ActionResult> Index()
        {
            var idObj = Url.RequestContext.RouteData.Values["id"];
            var idStr = idObj?.ToString() ?? "";
            var valid = long.TryParse(idStr, out var clubId);

            if (!valid)
            {
                return Redirect("~/Error/400");
            }


            var club = await CoreLib.Entities.SchoolClubs.DataInterop.SchoolClubReader.TryGetClubAsync(clubId);
            if (club == null)
            {
                return Redirect("~/Error/404");
            }


            return View();
        }


        [System.Web.Mvc.HttpGet]
        [MvcAuthControl]
        public async Task<ActionResult> About()
        {
            var idObj = Url.RequestContext.RouteData.Values["id"];
            var idStr = idObj?.ToString() ?? "";
            var valid = long.TryParse(idStr, out var clubId);

            if (!valid)
            {
                return Redirect("~/Error/400");
            }


            var club = await CoreLib.Entities.SchoolClubs.DataInterop.SchoolClubReader.TryGetClubAsync(clubId);
            if (club == null)
            {
                return Redirect("~/Error/404");
            }


            return View();
        }


        [System.Web.Mvc.HttpGet]
        [MvcAuthControl(RequireAdmin = true)]
        public async Task<ActionResult> ModEdit()
        {
            var idObj = Url.RequestContext.RouteData.Values["id"];
            var idStr = idObj?.ToString() ?? "";
            var valid = long.TryParse(idStr, out var clubId);

            if (!valid)
            {
                return Redirect("~/Error/400");
            }


            var club = await CoreLib.Entities.SchoolClubs.DataInterop.SchoolClubReader.TryGetClubAsync(clubId);
            if (club == null)
            {
                return Redirect("~/Error/404");
            }


            return View();
        }


        [System.Web.Mvc.HttpGet]
        [MvcAuthControl]
        public async Task<ActionResult> CreatePost()
        {
            var idObj = Url.RequestContext.RouteData.Values["id"];
            var idStr = idObj?.ToString() ?? "";
            var valid = long.TryParse(idStr, out var clubId);

            if (!valid)
            {
                return Redirect("~/Error/400");
            }


            var club = await CoreLib.Entities.SchoolClubs.DataInterop.SchoolClubReader.TryGetClubAsync(clubId);
            if (club == null)
            {
                return Redirect("~/Error/404");
            }


            return View();
        }
    }
}