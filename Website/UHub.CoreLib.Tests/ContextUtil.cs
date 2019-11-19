using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.Routing;

namespace UHub.CoreLib.Tests
{
    /// <summary>
    /// This represents the context utility entity.
    /// </summary>
    /// <remarks>
    /// This is copied/modified from http://aspnetwebstack.codeplex.com/SourceControl/latest#test/System.Web.Http.Test/Util/ContextUtil.cs
    /// </remarks>
    public class ContextUtil
    {
        public static HttpControllerContext CreateControllerContext(HttpConfiguration configuration = null, IHttpController instance = null, IHttpRouteData routeData = null, HttpRequestMessage request = null)
        {
            var config = configuration ?? new HttpConfiguration();
            var route = routeData ?? new HttpRouteData(new HttpRoute());
            var req = request ?? new HttpRequestMessage();
            req.SetConfiguration(config);
            req.SetRouteData(route);

            var context = new HttpControllerContext(config, route, req);
            if (instance != null)
            {
                context.Controller = instance;
            }
            context.ControllerDescriptor = CreateControllerDescriptor(config);

            return context;
        }

        public static HttpActionContext CreateActionContext(HttpControllerContext controllerContext = null, HttpActionDescriptor actionDescriptor = null)
        {
            var context = controllerContext ?? ContextUtil.CreateControllerContext();
            var descriptor = actionDescriptor ?? CreateActionDescriptor();
            descriptor.ControllerDescriptor = context.ControllerDescriptor;
            return new HttpActionContext(context, descriptor);
        }

        public static HttpActionContext GetActionContext(HttpRequestMessage request)
        {
            var actionContext = CreateActionContext();
            actionContext.ControllerContext.Request = request;
            return actionContext;
        }

        public static HttpActionExecutedContext GetActionExecutedContext(HttpRequestMessage request, HttpResponseMessage response)
        {
            var actionContext = CreateActionContext();
            actionContext.ControllerContext.Request = request;
            var actionExecutedContext = new HttpActionExecutedContext(actionContext, null) { Response = response };
            return actionExecutedContext;
        }

        public static HttpControllerDescriptor CreateControllerDescriptor(HttpConfiguration config = null)
        {
            if (config == null)
            {
                config = new HttpConfiguration();
            }
            return new HttpControllerDescriptor() { Configuration = config, ControllerName = "FooController" };
        }

        public static HttpActionDescriptor CreateActionDescriptor()
        {
            var desc = Substitute.For<HttpActionDescriptor>();
            desc.ActionName.Returns("Bar");
            return desc;
        }
    }
}
