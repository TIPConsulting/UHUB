using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.WebPages;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ApiValidateRecaptchaAttribute : AuthorizeAttribute
    {

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var hasRecaptcha = CoreFactory.Singleton.Properties.EnableRecaptcha;
            if (!hasRecaptcha)
            {
                return true;
            }

            try
            {
                var isValid = CoreFactory.Singleton.Recaptcha.IsCaptchaValid();

                return isValid;

            }
            catch
            {
                return false;
            }

        }


    }
}
