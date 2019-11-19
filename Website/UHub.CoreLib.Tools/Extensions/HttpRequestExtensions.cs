using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace UHub.CoreLib.Tools.Extensions
{
    /// <summary>
    /// HttpRequest extension methods
    /// </summary>
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Get IP address from request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetIPAddress(this HttpRequest request)
        {
            if (request == null)
                return null;

            string ipAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (ipAddress.IsNotEmpty())
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return request.ServerVariables["REMOTE_ADDR"] ?? null;
        }

    }
}
