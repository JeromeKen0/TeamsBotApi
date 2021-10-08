using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamsApiBot.Models;

namespace TeamsApiBot.CacheManages
{
    public class CacheManage
    {
        private static UserInfo _userInfo;
        private static string access_token;
        private static string refresh_token;
        private static Chat _chat;
        public void SetUserInfo(UserInfo userInfo)
        {
            _userInfo = userInfo;
        }
        public void SetChat(Chat chat)
        {
            _chat = chat;
        }
        public UserInfo GetInfo() => _userInfo;
        public Chat GetChat() => _chat;
        public void SetToken(TokenResponseModel tokenResponseModel)
        {
            access_token = tokenResponseModel.access_token;
            refresh_token = tokenResponseModel.refresh_token;
        }
        public void SetChatTimeConfigJson(Dictionary<string, DateTime> keyValuePairs)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "ChatTimesConfig.json";
            var jsonConents = JsonConvert.SerializeObject(keyValuePairs);
            using StreamWriter outputFile = new StreamWriter(path, false);
            outputFile.WriteLine(jsonConents);
        }
        public async Task<Dictionary<string, DateTime>> GetChatTimeConfig()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "ChatTimesConfig.json";
            if (File.Exists(path))
            {
                string text = await System.IO.File.ReadAllTextAsync(path);
                return JsonConvert.DeserializeObject<Dictionary<string, DateTime>>(text);
            }
            else
            {
                return new Dictionary<string, DateTime>();
            }
        }
        public string GetAccToken() => access_token;
        public string GetRefToken() => refresh_token;
    }
}
