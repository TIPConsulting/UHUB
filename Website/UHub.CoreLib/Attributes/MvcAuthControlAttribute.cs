using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Web.Routing;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Security.Authentication;

namespace UHub.CoreLib.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class MvcAuthControlAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        public bool RequireAdmin { get; set; } = false;


        public override void OnAuthorization(AuthorizationContext filterContext)
        {

            try
            {
                bool isLoggedIn = false;
                var authToken = filterContext.HttpContext.Request.Headers.Get(Constants.AUTH_HEADER_TOKEN);



                if (authToken.IsEmpty())
                {
                    //test for cookie auth
                    isLoggedIn = CoreFactory.Singleton.Auth.IsUserLoggedIn();
                }
                else
                {
                    //test for token auth
                    var tokenStatus = CoreFactory.Singleton.Auth.TrySetRequestUser(authToken);
                    isLoggedIn = (tokenStatus == TokenValidationStatus.Success);
                }

                if (!isLoggedIn)
                {
                    HandleLoginRedirect(ref filterContext);
                    return;
                }


                var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;

                var isValid = 
                    cmsUser.ID != null 
                    && cmsUser.IsFullyEnabled
                    && (!RequireAdmin || cmsUser.IsAdmin);


                if (!isValid)
                {
                    HandleLoginRedirect(ref filterContext);
                    return;
                }


            }
            catch (Exception ex)
            {
                var exID = new Guid("C39F81BF-E61D-4C55-AA0D-E8950549E74B");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);


                HandleLoginRedirect(ref filterContext);
            }
        }


        private void HandleLoginRedirect(ref AuthorizationContext filterContext)
        {
            var loginAddr = CoreFactory.Singleton.Properties.LoginURL;
            var fwrdCookieName = CoreFactory.Singleton.Properties.PostAuthCookieName;
            var forceSecure = CoreFactory.Singleton.Properties.ForceSecureCookies;



            var targetAddr = filterContext.HttpContext.Request.Url.AbsoluteUri;
            var cookie = new HttpCookie(fwrdCookieName);
            cookie.Value = targetAddr;
            cookie.Domain = CoreFactory.Singleton.Properties.CookieDomain;
            cookie.Secure = forceSecure;
            cookie.HttpOnly = forceSecure;
            cookie.Encrypt();
            filterContext.HttpContext.Response.SetCookie(cookie);



            filterContext.Result = new RedirectResult(loginAddr);


        }

    }
}
