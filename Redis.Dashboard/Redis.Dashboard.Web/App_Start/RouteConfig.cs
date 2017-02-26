using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Redis.Dashboard.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "ServerGroup",
                url: "{friendlyUrl}",
                defaults: new { controller = "Home", action = "Index", friendlyUrl = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "LoadData",
                url: "LoadData/{friendlyUrl}",
                defaults: new { controller = "LoadData", action = "Index", friendlyUrl = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Search",
                url: "Search/{friendlyUrl}/{pattern}",
                defaults: new { controller = "Search", action = "Index", pattern = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
