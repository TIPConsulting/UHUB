using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using UHub.CoreLib.Management;
using UHub.CoreLib.Tools.Extensions;
using System.Web;
using UHub.CoreLib.Security.Authentication;
using System.Threading;

namespace UHub.CoreLib.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ApiAuthControlAttribute : AuthorizeAttribute
    {
        public bool RequireAdmin { get; set; } = false;


        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            try
            {
                bool isLoggedIn = false;
                var isValid = actionContext.Request.Headers.TryGetValues(Constants.AUTH_HEADER_TOKEN, out var valueSet);


                string authToken = "";
                if (isValid && valueSet != null)
                {
                    authToken = valueSet.FirstOrDefault();
                }

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
                    return false;
                }


                var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;


                return 
                    cmsUser.ID != null
                    && cmsUser.IsFullyEnabled
                    && (!RequireAdmin || cmsUser.IsAdmin);
            }
            catch (Exception ex)
            {
                var errCode = "EA1A7A06-36FD-4276-9D7D-095A83C2E513";
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, new Guid(errCode));

                return false;
            }
        }

    }
}
