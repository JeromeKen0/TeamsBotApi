using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamsApiBot.Models
{
    public class AuthorizationResponseModel
    {
        public string code { get; set; }
        public string state { get; set; }
    }
}
