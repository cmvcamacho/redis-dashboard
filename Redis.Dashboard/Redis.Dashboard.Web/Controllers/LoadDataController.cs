using Redis.Dashboard.Web.Models;
using Redis.Dashboard.Web.Services;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Redis.Dashboard.Web.Controllers
{
    public class LoadDataController : Controller
    {        
        public async Task<ActionResult> Index(string friendlyUrl)
        {
            var model = RedisConfig.GetServerGroupByFriendlyUrl(friendlyUrl);
            model.Servers = await RedisManager.GetRedisServersInfo(friendlyUrl, model.ServerGroup.Endpoints);

            var masterServers = model.Servers
                                    .Where(s => s.Replication != null && s.Replication.Role.Equals("master", StringComparison.InvariantCultureIgnoreCase));

            model.Totals = new ServersTotals();
            model.Totals.TotalMemory = masterServers
                                    .Sum(s => s.Memory.UsedMemory)
                                    .GetBytesReadable();
            model.Totals.TotalKeys = masterServers
                                    .Where(s => s.Keyspace != null)
                                    .Sum(s => s.Keyspace.Keys)
                                    .GetBytesReadable().Replace("B", string.Empty);
            model.Totals.TotalClients = masterServers
                                    .Sum(s => s.Clients.ConnectedClients)
                                    .GetBytesReadable().Replace("B", string.Empty);
            model.Totals.TotalInputKbps = masterServers
                                    .Sum(s => s.Stats.InstantaneousInputKbps);
            model.Totals.TotalOutputKbps = masterServers
                                    .Sum(s => s.Stats.InstantaneousOutputKbps);
            model.Totals.TotalOps = masterServers
                                    .Sum(s => s.Stats.InstantaneousOpsPerSec);
            model.Totals.TotalMisses = masterServers
                                    .Sum(s => s.Stats.KeyspaceMisses)
                                    .GetBytesReadable().Replace("B", string.Empty);
            model.Totals.TotalHits = masterServers
                                    .Sum(s => s.Stats.KeyspaceHits)
                                    .GetBytesReadable().Replace("B", string.Empty);
            model.Totals.InfoCluster = string.Format("{0} masters / {1} slaves",
                                    masterServers.Count(),
                                    model.Servers.Count() - masterServers.Count());

            return PartialView(model);
        }
    }
}