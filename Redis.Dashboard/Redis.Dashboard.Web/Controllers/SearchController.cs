using Redis.Dashboard.Web.Models;
using Redis.Dashboard.Web.Services;
using Serilog;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Redis.Dashboard.Web.Controllers
{
    public class SearchController : Controller
    {
        [HttpGet]
        public async Task<ActionResult> Index(string friendlyUrl, string pattern)
        {
            try
            {
                if (string.IsNullOrEmpty(pattern))
                    pattern = "*";
                else
                    pattern = "*" + pattern + "*";

                var model = RedisConfig.GetServerGroupByFriendlyUrl(friendlyUrl);
                var results = await RedisManager.GetRedisKeysByPattern(friendlyUrl, model.ServerGroup.Endpoints, pattern);

                return PartialView(results);
            }
            catch (Exception e)
            {
                Log.Error(e, "Search for {pattern} in {friendlyUrl}", pattern, friendlyUrl);
                return PartialView(new List<SearchResult>());
            }
        }
    }
}