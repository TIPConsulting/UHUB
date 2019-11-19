using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace UHub.CoreLib.HttpModules
{
    /// <summary>
    /// Configure page response headers
    /// </summary>
    public sealed class ResponseHeaderModule : IHttpModule
    {

        public void Dispose()
        {
            //clean-up code here.
        }

        string csp = "";


        public void Init(HttpApplication context)
        {

            var cspBuilder = new StringBuilder();
            cspBuilder.Append("default-src 'self' *.u-hub.life *.google.com *.google-analytics.com;");
            cspBuilder.Append("script-src 'self' 'unsafe-eval' *.u-hub.life *.google.com *.gstatic.com *.googletagmanager.com *.google-analytics.com *.bootstrapcdn.com code.jquery.com cdn.jsdelivr.net cdnjs.cloudflare.com;");
            cspBuilder.Append("connect-src 'self' *.u-hub.life *.google.com *.gstatic.com *.googletagmanager.com *.google-analytics.com *.bootstrapcdn.com cdn.jsdelivr.net;");
            cspBuilder.Append("style-src 'self' 'unsafe-inline' *.u-hub.life *.google.com *.gstatic.com *.bootstrapcdn.com code.jquery.com cdn.jsdelivr.net;");
            cspBuilder.Append("img-src * data:;");
            cspBuilder.Append("font-src 'self' *.u-hub.life fonts.gstatic.com *.bootstrapcdn.com data:;");
            cspBuilder.Append("child-src *.google.com;");
            csp = cspBuilder.ToString();

            //lock (lockObject)
            //{
            //context.EndRequest += Context_EndRequest;
            context.PostReleaseRequestState += Context_PostReleaseRequestState;
            //}
        }



        private void Context_PostReleaseRequestState(object sender, EventArgs e)
        {
            var Response = ((HttpApplication)sender).Context.Response;

            //ensure headers have not already been written
            //this prevents errors with file downloads
            if (Response.HeadersWritten)
            {
                return;
            }

            //Response.ClearHeaders();


            Response.Headers.Remove("Server");
            Response.Headers.Remove("X-AspNetMvc-Version");
            Response.Headers.Remove("X-AspNet-Version");
            Response.Headers.Remove("X-Powered-By");

            void doHeaderWork(string name, string val)
            {
                Response.Headers.Remove(name);
                Response.Headers.Add(name, val);
            };


            //set response headers to limit site XSS vectors and frame access
            if (Response.ContentType.ToLower() == "text/html")
            {
                doHeaderWork("Content-Security-Policy", csp);
            }

            doHeaderWork("Referrer-Policy", "strict-origin");
            doHeaderWork("Vary", "Accept-Encoding");
            doHeaderWork("X-UA-Compatible", "IE=edge");
            doHeaderWork("Access-Control-Allow-Methods", "GET");
            doHeaderWork("X-XSS-Protection", "1; mode=block");
            doHeaderWork("X-Content-Type-Options", "nosniff");
            doHeaderWork("X-Frame-Options", "sameorigin");

        }

    }
}
