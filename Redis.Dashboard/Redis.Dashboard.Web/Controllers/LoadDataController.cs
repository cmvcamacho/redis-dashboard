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
    public class LoadDataController : Controller
    {
        [HttpGet]
        public async Task<ActionResult> Index(string friendlyUrl)
        {
            try
            {
                var model = RedisConfig.GetServerGroupByFriendlyUrl(friendlyUrl);
                model.Servers = await RedisManager.GetRedisServersInfo(friendlyUrl, model.ServerGroup.Endpoints);

                if(model.Servers == null)
                    return Json(model, JsonRequestBehavior.AllowGet);

                var masterServers = model.Servers.Where(s => s.IsMaster);

                model.Totals = new ServersTotals();
                model.Totals.TotalMemory = masterServers
                                        .Sum(s => s.Memory.UsedMemory)
                                        .GetBytesReadable();
                model.Totals.TotalKeys = masterServers
                                        .Sum(s => s.Keyspace.Sum(k => k.Keys))
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

                var totalHits = masterServers.Sum(s => s.Stats.KeyspaceHits);
                var totalMisses = masterServers.Sum(s => s.Stats.KeyspaceMisses);
                model.Totals.TotalHits = totalHits
                                        .GetBytesReadable().Replace("B", string.Empty);
                model.Totals.TotalMisses = totalMisses
                                        .GetBytesReadable().Replace("B", string.Empty);
                model.Totals.TotalMisses = totalMisses
                                        .GetBytesReadable().Replace("B", string.Empty);

                model.Totals.PercentageHits = string.Format("{0}%", totalHits > 0 
                    ? Math.Round((float)totalHits / (totalHits + totalMisses), 3) * 100
                    : 0);
                model.Totals.PercentageMisses = string.Format("{0}%", totalMisses > 0
                    ? Math.Round((float)totalMisses / (totalHits + totalMisses), 3) * 100
                    : 0);


                model.Totals.InfoCluster = string.Format("{0} masters / {1} slaves",
                                        masterServers.Count(),
                                        model.Servers.Count() - masterServers.Count());

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(e, "LoadData for {friendlyUrl}", friendlyUrl);
                return Json(new GroupServerPageModel(), JsonRequestBehavior.AllowGet);
            }
        }
    }
}