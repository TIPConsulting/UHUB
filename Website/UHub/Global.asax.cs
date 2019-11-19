using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Configuration;
using UHub.CoreLib.Management.Config;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Logging;
using UHub.CoreLib.Management;
using UHub.CoreLib.Security;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Util;
using System.Management.Automation;
using UHub.CoreLib.ErrorHandling.Exceptions;

namespace UHub
{
    public class MvcApplication : System.Web.HttpApplication
    {


        protected void Application_Start()
        {

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configure(WebApiConfig.Register);



            var cmsConfig = Common.GetCmsConfig();
            if (cmsConfig != null)
            {
                CoreFactory.Initialize(cmsConfig);
            }



            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

        }

        protected void Session_Start(object sender, EventArgs e)
        {
            Session["INIT"] = "INIT";
        }
    }
}
