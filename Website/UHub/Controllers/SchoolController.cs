using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UHub.CoreLib.Attributes;

namespace UHub.Controllers
{
    public class SchoolController : Controller
    {
        [System.Web.Mvc.HttpGet]
        [MvcAuthControl]
        public ActionResult Index()
        {
            var idObj = Url.RequestContext.RouteData.Values["id"];


            return View();
        }


        [System.Web.Mvc.HttpGet]
        [MvcAuthControl(RequireAdmin = true)]
        public ActionResult CreatePost()
        {
            var idObj = Url.RequestContext.RouteData.Values["id"];


            return View();
        }


        [System.Web.Mvc.HttpGet]
        [MvcAuthControl]
        public ActionResult Clubs()
        {
            var idObj = Url.RequestContext.RouteData.Values["id"];


            return View();
        }


        [System.Web.Mvc.HttpGet]
        [MvcAuthControl]
        public ActionResult CreateClub()
        {
            var idObj = Url.RequestContext.RouteData.Values["id"];


            return View();
        }
    }
}