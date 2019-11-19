using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Security
{
    public sealed partial class RecaptchaManager
    {
        public const string RECAPTCHA_HEADER = "g-recaptcha-response";

        internal RecaptchaManager()
        {

        }


        /// <summary>
        /// Validate ReCaptcha token from user
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public bool IsCaptchaValid()
        {
            var gResponse = HttpContext.Current.Request.Headers[RECAPTCHA_HEADER];
            if (gResponse.IsEmpty())
            {
                gResponse = HttpContext.Current.Request.Form[RECAPTCHA_HEADER];
            }
            return IsCaptchaValid(gResponse);
        }


        /// <summary>
        /// Validate ReCaptcha token from user
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public bool IsCaptchaValid(string recaptchaResponse)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (!CoreFactory.Singleton.Properties.EnableRecaptcha)
            {
                throw new InvalidOperationException("Recaptcha is not enabled");
            }

            try
            {
                var privateKey = CoreFactory.Singleton.Properties.RecaptchaPrivateKey;

                string url = "https://www.google.com/recaptcha/api/siteverify" + $"?secret={privateKey}&response={recaptchaResponse}";

                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage response = client.GetAsync(new Uri(url)).Result)
                    {

                        response.EnsureSuccessStatusCode();
                        using (HttpContent cont = response.Content)
                        {
                            string result = cont.ReadAsStringAsync().Result;
                            bool status = Convert.ToBoolean(JObject.Parse(result).GetValue("success"));

                            return status;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
