using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TeamsApiBot.Configs
{
    public class Scopes
    {

        public static List<string> scopes = new List<string>()
        {
            "Channel.ReadBasic.All",
            "ChannelMessage.Delete",
            "ChannelMessage.Edit",
            "ChannelMessage.Send",
            "Chat.Read",
            "Chat.ReadWrite",
            "email",
            "offline_access",
            "openid",
            "profile",
            "User.Read",
            "Team.Create",
            "Team.ReadBasic.All",
            "TeamsActivity.Read",
            "TeamsActivity.Send",
            "TeamsApp.Read",
            "TeamsApp.ReadWrite",
            "User.ReadBasic.All",
        };
    }
}
