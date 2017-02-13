using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Redis.Dashboard.Web.Models
{
    public class HomeModel
    {
        public List<Group> GroupsToShow { get; set; }
        public int ActiveGroupId { get; set; }
        public GroupServerPageModel ActiveServerGroup { get; set; }
    }
}