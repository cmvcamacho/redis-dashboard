using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Redis.Dashboard.Web.Models
{
    public class GroupServerPageModel
    {
        public ServerGroup ServerGroup { get; set; }
        public int GroupId { get; set; }
        public string GroupTitle { get; set; }
        public List<ServerInfo> Servers { get; set; }
        public ServersTotals Totals { get; set; }

    }
}