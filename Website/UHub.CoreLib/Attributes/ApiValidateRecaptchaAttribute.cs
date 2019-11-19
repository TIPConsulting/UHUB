using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.WebPages;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class MvcValidateRecaptchaAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        public string FailView { get; set; }


        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var hasRecaptcha = CoreFactory.Singleton.Properties.EnableRecaptcha;

            if(!hasRecaptcha)
            {
                return;
            }


            try
            {
                var isValid = CoreFactory.Singleton.Recaptcha.IsCaptchaValid();

                if(!isValid)
                {
                    HandleFailure(ref filterContext);
                }

            }
            catch
            {
                HandleFailure(ref filterContext);
            }

        }


        private void HandleFailure(ref AuthorizationContext filterContext)
        {
            if (FailView.IsEmpty())
            {
                filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                filterContext.Result = new RedirectResult(FailView);
            }

        }

    }
}
