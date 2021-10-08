using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamsApiBot.Models
{
    public class ConversationMembers
    {
        public string id { get; set; }
        public string[] roles { get; set; }
        public string displayName { get; set; }
        public string userId { get; set; }
        public string email { get; set; }
    }
}
