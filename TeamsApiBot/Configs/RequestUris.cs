using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamsApiBot.Configs
{
    public class RequestUris
    {
        public static string BaseLoginUri = "https://login.microsoftonline.com";
        public static string BaseGraphUri = "https://graph.microsoft.com";
        public static string redirect_uri = "http://localhost:5000/redirect";

        public static string tenant = "common";

        public static string AuthorizeUri = $"{tenant}/oauth2/v2.0/authorize";
        public static string TokenRequestUri = $"{tenant}/oauth2/v2.0/token";

        public static string CallMicrosoftGraph = "v1.0/me";

        public static string ChannelsList = "v1.0/teams/{0}/channels";
        public static string MeTeams = "v1.0/me/joinedTeams";
        public static string MeChats = "beta/me/chats";
        public static string ListMessagesChat = "v1.0/me/chats/{chat-id}/messages";
        public static string GetChats = "v1.0/me/chats/{0}/messages?$top={1}";
        public static string SendMessage = "v1.0/chats/{0}/messages";
    }
}
