using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Management;

namespace UHub.Controllers
{
    public class ErrorController : Controller
    {
        private static Dictionary<int, string> CapturedCodes = new Dictionary<int, string>()
        {
            [400] = "The server cannot process the specified request",
            [401] = "User not authenticated",
            [403] = "User not authorized",
            [404] = "Requested resource not found",
            [410] = "Requested resource has been removed",
            [418] = "This is a teapot",
            [500] = "An unexpected server error has occured",
            [501] = "Requested resource is not implemented",
            [502] = "Bad Gateway",
            [503] = "Requested service is not currently available",
        };


        // GET: Error
        [System.Web.Mvc.HttpGet]
        public ActionResult Index()
        {
            Response.TrySkipIisCustomErrors = true;


            if (Request.QueryString.HasKeys() && Request.QueryString["aspxerrorpath"] != null)
            {
                Response.Redirect(Request.Url.GetLeftPart(UriPartial.Path), true);
            }



            //IMPORTANT
            //POST-CONDITIONS
            //
            //"ViewBag.Title" must be set
            //"ViewBag.ErrorMessage" must be set
            //"ViewBag.CatAddr" must be set



            var idObj = Url.RequestContext.RouteData.Values["id"];
            var idStr = idObj?.ToString() ?? "0";


            var valid = int.TryParse(idStr, out var id);

            //set default error code
            id = valid ? id : 400;



            //verify valid code
            if (!CapturedCodes.TryGetValue(id, out var message))
            {
                if (id >= 400 && id < 500)
                {
                    id = 400;
                }
                else if (id >= 500 && id < 600)
                {
                    id = 500;
                }
                else
                {
                    id = 400;
                }
                message = CapturedCodes[id];
            }



            //401 - 499
            if (id > 400 && id < 500)
            {
                ViewBag.Title = "Request Error " + id;
                ViewBag.ErrorMessage = message;
            }
            //500 - 599
            else if (id >= 500 && id < 600)
            {
                ViewBag.Title = "Server Error " + id;
                ViewBag.ErrorMessage = message;
            }
            else
            {
                ViewBag.Title = "Request Error " + id;
                ViewBag.ErrorMessage = CapturedCodes[400];
            }

            ViewBag.CatAddr = $"https://http.cat/{id}.jpg";
            Response.StatusCode = id;
            return View();
        }



        [System.Web.Mvc.HttpGet]
        public ActionResult Startup()
        {
            try
            {
                var x = CoreFactory.Singleton;
                return Redirect("~/");
            }
            catch
            {
                return View();
            }
        }
    }
}