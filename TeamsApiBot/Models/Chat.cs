using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamsApiBot.Models
{
    public class Chat
    {
        public string id { get; set; }
        public string topic { get; set; }
        public string createdDateTime { get; set; }
        public string lastUpdatedDateTime { get; set; }
        public string meeting { get; set; }
        public string chatType { get; set; }
        //public object viewpoint { get; set; }
    }
}
