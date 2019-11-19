using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace UHub.CoreLib.Tools.Extensions
{
    /// <summary>
    /// HttpResponse extension methods
    /// </summary>
    public static class HttpResponseExtensions
    {
        /// <summary>
        /// Redirect response using 301 status code
        /// </summary>
        /// <param name="response"></param>
        /// <param name="URL"></param>
        public static void RedirectCanonical(this HttpResponse response, string URL)
        {
            if (!HttpContext.Current.Response.HeadersWritten)
            {
                response.Clear();
                response.StatusCode = 301;
                response.Status = "301 Moved Permanently";
                response.AddHeader("Location", URL);
                response.CacheControl = "max-age=3600";
                response.Flush();
                response.End();

            }

        }

        /// <summary>
        /// Redirect response using 302 status code
        /// </summary>
        /// <param name="response"></param>
        /// <param name="URL"></param>
        public static void RedirectFound(this HttpResponse response, string URL)
        {
            if (!HttpContext.Current.Response.HeadersWritten)
            {
                response.Clear();
                response.StatusCode = 302;
                response.Status = "302 Found";
                response.AddHeader("Location", URL);
                response.Flush();
                response.End();
            }
        }

        /// <summary>
        /// Generate and throw and HTTP server serror
        /// </summary>
        /// <param name="response"></param>
        /// <param name="code"></param>
        /// <param name="description"></param>
        private static void ThrowHTTPError(HttpResponse response, int code, string description)
        {
            //*
            if (!HttpContext.Current.Response.HeadersWritten)
            {
                response.Clear();
                response.StatusCode = code;
                response.StatusDescription = description;
                response.Flush();
                response.End();
            }
            //*/
        }

        #region HTTP Codes
        /// <summary>
        /// Found, No Content
        /// </summary>
        /// <param name="response"></param>
        /// <param name="msg"></param>
        public static void Throw204(this HttpResponse response, string msg = null)
        {
            ThrowHTTPError(response, 204, msg ?? "No Content");
        }

        /// <summary>
        /// Invalid Request
        /// </summary>
        public static void Throw400(this HttpResponse response, string msg = null)
        {
            ThrowHTTPError(response, 400, msg ?? "Invalid Request");
        }

        /// <summary>
        /// Unauthorized
        /// </summary>
        public static void Throw401(this HttpResponse response, string msg = null)
        {
            ThrowHTTPError(response, 401, msg ?? "Unauthorized");
        }

        /// <summary>
        /// Forbidden
        /// </summary>
        public static void Throw403(this HttpResponse response, string msg = null)
        {
            ThrowHTTPError(response, 403, msg ?? "Forbidden");
        }

        /// <summary>
        /// Not Found
        /// </summary>
        public static void Throw404(this HttpResponse response, string msg = null)
        {
            ThrowHTTPError(response, 404, msg ?? "Not Found");
        }

        /// <summary>
        /// Gone
        /// </summary>
        public static void Throw410(this HttpResponse response, string msg = null)
        {
            ThrowHTTPError(response, 410, msg ?? "Gone");
        }

        /// <summary>
        /// Internal Server Error
        /// </summary>
        public static void Throw500(this HttpResponse response, string msg = null)
        {
            ThrowHTTPError(response, 500, msg ?? "Internal Server Error");
        }
        #endregion HTTP Errors



    }
}
