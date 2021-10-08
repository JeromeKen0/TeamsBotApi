using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TeamsApiBot.Models
{
    public class Channel
    {
        public string description { get; set; }
        public string displayName { get; set; }
        public string id { get; set; }
        public bool isFavoriteByDefault { get; set; }
        public string webUrl { get; set; }
        public string email { get; set; }
    }
}
