using Newtonsoft.Json;
using Redis.Dashboard.Web.Models;
using Redis.Dashboard.Web.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Redis.Dashboard.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Debug()
                            .WriteTo.RollingFile(Server.MapPath("~/log/log-{Date}.txt"))
                            .CreateLogger();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            RedisConfig.ReadConfiguration();
        }
    }
}
