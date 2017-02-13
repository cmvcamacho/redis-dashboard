using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Redis.Dashboard.Web.Models
{
    public class ServerGroup
    {
        public string FriendlyUrl { get; set; }
        public string Title { get; set; }
        public string Endpoints { get; set; }
    }
}