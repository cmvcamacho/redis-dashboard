using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Redis.Dashboard.Web.Models
{
    public class SearchResult
    {
        public string Key { get; set; }
        public string Type { get; set; }
        public long Ttl { get; set; }
        public long Size { get; set; }
        public string Server { get; set; }
    }
}