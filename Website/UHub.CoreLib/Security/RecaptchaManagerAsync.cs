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

        /// <summary>
        /// Validate ReCaptcha token from user
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public async Task<bool> IsCaptchaValidAsync(HttpContext Context)
        {
            var gResponse = Context.Request.Headers[RECAPTCHA_HEADER];
            if (gResponse.IsEmpty())
            {
                gResponse = Context.Request.Form[RECAPTCHA_HEADER];
            }
            return await IsCaptchaValidAsync(gResponse, Context);
        }


        /// <summary>
        /// Validate ReCaptcha token from user
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public async Task<bool> IsCaptchaValidAsync(string recaptchaResponse, HttpContext Context)
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
                    using (HttpResponseMessage response = await client.GetAsync(new Uri(url)))
                    {

                        response.EnsureSuccessStatusCode();
                        using (HttpContent cont = response.Content)
                        {
                            string result = await cont.ReadAsStringAsync();
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
