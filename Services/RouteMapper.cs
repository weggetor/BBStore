using System;
using System.Web.Routing;
using DotNetNuke.Web.Api;

namespace Bitboxx.DNNModules.BBStore.Services
{
    public class RouteMapper : IServiceRouteMapper
    {
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            mapRouteManager.MapHttpRoute("BBStore",
                                         "default",
                                         "{controller}/{action}",
                                         new[] { "Bitboxx.DNNModules.BBStore.Services" });
        }
    }
}