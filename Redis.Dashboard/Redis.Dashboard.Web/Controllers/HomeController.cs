using Redis.Dashboard.Web.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Redis.Dashboard.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string friendlyUrl)
        {
            var model = new HomeModel
            {
                GroupsToShow = RedisConfig.GroupsToShow
            };

            var groupServerPageModel = RedisConfig.GetServerGroupByFriendlyUrl(friendlyUrl);
            if (groupServerPageModel != null)
            {
                model.ActiveGroupId = groupServerPageModel.GroupId;
                model.ActiveServerGroup = groupServerPageModel;
            }

            if (model.ActiveGroupId == 0 && model.GroupsToShow != null && model.GroupsToShow.Any())
            {
                var defaultGroupServer = RedisConfig.GetDefaultServerGroup();
                if (defaultGroupServer != null)
                {
                    model.ActiveGroupId = defaultGroupServer.GroupId;
                    model.ActiveServerGroup = defaultGroupServer;
                }
            }

            return View(model);
        }



    }
}