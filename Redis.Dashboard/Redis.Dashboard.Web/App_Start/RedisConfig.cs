using Newtonsoft.Json;
using Redis.Dashboard.Web.Models;
using Redis.Dashboard.Web.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;

namespace Redis.Dashboard.Web
{
    public static class RedisConfig
    {
        public static List<Group> GroupsToShow { get; private set; }

        public static void ReadConfiguration()
        {
            using (StreamReader r = new StreamReader(HostingEnvironment.MapPath("~/config.json")))
            {
                string json = r.ReadToEnd();
                GroupsToShow = JsonConvert.DeserializeObject<List<Group>>(json);
            }
            if(GroupsToShow != null && GroupsToShow.Any())
            {
                var redisManager = new RedisManager();
                foreach (var redisConn in GroupsToShow
                    .SelectMany(g => g.ServerGroups
                        .Select(sg => new { Key = sg.FriendlyUrl, ConnString = sg.Endpoints })))
                {
                    redisManager.ReadServerInfo(redisConn.Key, redisConn.ConnString);
                }
            }
        }
        public static GroupServerPageModel GetDefaultServerGroup()
        {
            var group = GroupsToShow.FirstOrDefault(g => g.ServerGroups != null && g.ServerGroups.Any());
            if (group != null)
            {
                return new GroupServerPageModel
                {
                    GroupId = group.GroupId,
                    GroupTitle = group.Title,
                    ServerGroup = group.ServerGroups.FirstOrDefault()
                };
            }
            return null;
        }
        public static GroupServerPageModel GetServerGroupByFriendlyUrl(string friendlyUrl)
        {
            if (!string.IsNullOrWhiteSpace(friendlyUrl))
            {
                foreach (var group in GroupsToShow)
                {
                    var serverGroup = group.ServerGroups.FirstOrDefault(s => s.FriendlyUrl != null && s.FriendlyUrl.Equals(friendlyUrl, StringComparison.InvariantCultureIgnoreCase));
                    if (serverGroup != null)
                    {
                        return new GroupServerPageModel
                        {
                            GroupId = group.GroupId,
                            GroupTitle = group.Title,
                            ServerGroup = serverGroup
                        };
                    }
                }
            }
            return null;
        }


    }
}
