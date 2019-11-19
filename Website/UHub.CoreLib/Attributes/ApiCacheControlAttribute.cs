using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace UHub.CoreLib.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ApiCacheControlAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// MaxAge in seconds
        /// </summary>
        public TimeSpan MaxAge { get; set; }

        public ApiCacheControlAttribute()
        {
            MaxAge = new TimeSpan(1, 0, 0);
        }

        public ApiCacheControlAttribute(int MaxAge)
        {
            this.MaxAge = TimeSpan.FromSeconds(MaxAge);
        }

        public override void OnActionExecuted(HttpActionExecutedContext context)
        {
            if (context.Response != null)
                context.Response.Headers.CacheControl = new CacheControlHeaderValue()
                {
                    Public = true,
                    MaxAge = MaxAge
                };

            base.OnActionExecuted(context);
        }
    }
}
