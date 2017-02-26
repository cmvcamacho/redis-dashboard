using System.Web;
using System.Web.Optimization;

namespace Redis.Dashboard.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/vendor")
                .Include("~/Scripts/jquery-{version}.js")
                .Include("~/Scripts/jquery.validate*")
                .Include("~/Scripts/bootstrap.js")
                .Include("~/Scripts/respond.js")
                .Include("~/Scripts/metisMenu/metisMenu.min.js")
                .Include("~/Scripts/raphael/raphael.min.js")
                .Include("~/Scripts/morrisjs/morris.min.js")
                .Include("~/Scripts/RivetJs/rivets.bundled.min.js")
                );
            bundles.Add(new ScriptBundle("~/bundles/custom")
                .Include("~/Scripts/sb-admin-2.js")
                .Include("~/Scripts/site.js")
                );


            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));


            bundles.Add(new StyleBundle("~/Content/vendor").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/metisMenu/metisMenu.min.css",
                      "~/Content/morrisjs/morris.css",
                      "~/Content/font-awesome/css/font-awesome.min.css"
                      ));

            bundles.Add(new StyleBundle("~/Content/custom").Include(
                      "~/Content/sb-admin-2.css",
                      "~/Content/site.css"));
        }
    }
}
