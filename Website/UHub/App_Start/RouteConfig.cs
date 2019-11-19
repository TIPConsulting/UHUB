using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace UHub
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute(CoreLib.Constants.API_ROUTE_PREFIX + "/*");

            //Action for a specified controller
            //Will default to Index when only controller is specified
            routes.MapRoute(
                name: "Controller_DefaultAction",
                url: "{controller}",
                defaults: new { action = "Index"}
            );


            //Action for a controller and ID
            //Will default to Index controller and ID are specified
            //Only works if ID is a number
            routes.MapRoute(
                name: "ControllerWithID",
                url: "{controller}/{id}",
                defaults: new { action = "Index" },
                constraints: new { id = @"\d+" }
            );


            //Fully specified path
            //Establishes default home page (Account/Login)
            routes.MapRoute(
                name: "StandardBehavior",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional },
                constraints: new { action = @"\D.*" }
            );

        }
    }
}
