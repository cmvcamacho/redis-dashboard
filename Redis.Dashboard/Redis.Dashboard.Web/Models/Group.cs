using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Redis.Dashboard.Web.Models
{
    public class Group
    {
        public int GroupId { get; set; }
        public string Title { get; set; }
        public List<ServerGroup> ServerGroups { get; set; }
    }
}