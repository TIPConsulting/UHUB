using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace UHub.CoreLib.Tools.Extensions
{
    /// <summary>
    /// HttpCookie extension methods
    /// </summary>
    public static class HttpCookieExtensions
    {
        /// <summary>
        /// Encrypt cookie to prevent client access
        /// </summary>
        /// <param name="cookie"></param>
        public static void Encrypt(this HttpCookie cookie)
        {
            string purpose = "cookie";
            if (string.IsNullOrEmpty(cookie.Value))
                return;

            byte[] stream = Encoding.UTF8.GetBytes(cookie.Value);
            byte[] encodedValue = MachineKey.Protect(stream, purpose);
            cookie.Value = HttpServerUtility.UrlTokenEncode(encodedValue);
        }

        /// <summary>
        /// Decrypt cookie to get value
        /// </summary>
        /// <param name="cookie"></param>
        public static void Decrypt(this HttpCookie cookie)
        {
            string purpose = "cookie";
            if (string.IsNullOrEmpty(cookie.Value))
                return;

            byte[] stream = HttpServerUtility.UrlTokenDecode(cookie.Value);
            byte[] decodedValue = MachineKey.Unprotect(stream, purpose);
            cookie.Value = Encoding.UTF8.GetString(decodedValue);
        }

        /// <summary>
        /// Expire/invalidate a cookie
        /// </summary>
        /// <param name="cookie"></param>
        public static void Expire(this HttpCookie cookie)
        {
            cookie.Expires = DateTime.Now.AddDays(-5);
        }

        /// <summary>
        /// Delete the contents of a cookie
        /// </summary>
        /// <param name="cookie"></param>
        public static void ClearContent(this HttpCookie cookie)
        {
            cookie.Value = string.Empty;
        }
    }
}
