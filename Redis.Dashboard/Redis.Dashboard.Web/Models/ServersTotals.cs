using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Redis.Dashboard.Web.Models
{
    public class ServersTotals
    {
        public string TotalMemory { get; set; }
        public string TotalKeys { get; set; }
        public string TotalClients { get; set; }
        public long TotalOps { get; set; }
        public decimal TotalInputKbps { get; set; }
        public decimal TotalOutputKbps { get; set; }
        public string InfoCluster { get; set; }
        public string TotalHits { get; set; }
        public string TotalMisses { get; set; }

    }
}