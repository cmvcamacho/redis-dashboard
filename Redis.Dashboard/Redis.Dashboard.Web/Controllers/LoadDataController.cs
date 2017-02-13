using Redis.Dashboard.Web.Models;
using Redis.Dashboard.Web.Services;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Redis.Dashboard.Web.Controllers
{
    public class LoadDataController : Controller
    {
        
        public ActionResult Index(string friendlyUrl)
        {
            var model = RedisConfig.GetServerGroupByFriendlyUrl(friendlyUrl);
            model.Servers = new RedisManager().GetServerInfo(friendlyUrl);

            model.Totals = new ServersTotals
            {
                TotalMemory = model.Servers
                                    .Where(s => s.Replication != null && s.Replication.Role.Equals("master", StringComparison.InvariantCultureIgnoreCase))
                                    .Sum(s => s.Memory.UsedMemory).GetBytesReadable(),
                TotalKeys = model.Servers
                                    .Where(s => s.Replication != null && s.Replication.Role.Equals("master", StringComparison.InvariantCultureIgnoreCase) && s.Keyspace != null)
                                    .Sum(s => s.Keyspace.Keys)
                                    .GetBytesReadable().Replace("B", string.Empty),
                TotalClients = model.Servers
                                    .Where(s => s.Replication != null && s.Replication.Role.Equals("master", StringComparison.InvariantCultureIgnoreCase) && s.Keyspace != null)
                                    .Sum(s => s.Clients.ConnectedClients)
                                    .GetBytesReadable().Replace("B", string.Empty),
                TotalInputKbps = model.Servers
                                    .Where(s => s.Replication != null && s.Replication.Role.Equals("master", StringComparison.InvariantCultureIgnoreCase))
                                    .Sum(s => s.Stats.InstantaneousInputKbps),
                TotalOutputKbps = model.Servers
                                    .Where(s => s.Replication != null && s.Replication.Role.Equals("master", StringComparison.InvariantCultureIgnoreCase))
                                    .Sum(s => s.Stats.InstantaneousOutputKbps),
                TotalOps = model.Servers
                                    .Where(s => s.Replication != null && s.Replication.Role.Equals("master", StringComparison.InvariantCultureIgnoreCase))
                                    .Sum(s => s.Stats.InstantaneousOpsPerSec),
                TotalMisses = model.Servers
                                    .Where(s => s.Replication != null && s.Replication.Role.Equals("master", StringComparison.InvariantCultureIgnoreCase))
                                    .Sum(s => s.Stats.KeyspaceMisses)
                                    .GetBytesReadable().Replace("B", string.Empty),
                TotalHits = model.Servers
                                    .Where(s => s.Replication != null && s.Replication.Role.Equals("master", StringComparison.InvariantCultureIgnoreCase))
                                    .Sum(s => s.Stats.KeyspaceHits)
                                    .GetBytesReadable().Replace("B", string.Empty),
                InfoCluster = string.Format("{0} masters / {1} slaves",
                                    model.Servers.Count(s => s.Replication != null && s.Replication.Role.Equals("master", StringComparison.InvariantCultureIgnoreCase)),
                                    model.Servers.Count(s => s.Replication != null && !s.Replication.Role.Equals("master", StringComparison.InvariantCultureIgnoreCase)))
            };

            return PartialView(model);
        }
    }
}