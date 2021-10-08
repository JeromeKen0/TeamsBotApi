using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TeamsApiBot.CacheManages;
using TeamsApiBot.Configs;
using TeamsApiBot.HttpClients;
using TeamsApiBot.Models;

namespace TeamsApiBot.Services
{
    public interface IGraphService
    {
        Task<List<Channel>> GetChannels();
        Task<List<Team>> GetTeams();
        Task<List<Chat>> MeChats();
        Task<List<Message>> GetMessagesByUser(string chatid, int number = 50);
        Task<bool> SendMessage(string content, string chatid);
    }
    public class GraphService : IGraphService
    {
        private readonly GraphClient _graphClient;
        private readonly CacheManage _cache;
        public GraphService(GraphClient graphClient, CacheManage cacheManage)
        {
            _graphClient = graphClient;
            _cache = cacheManage;
        }

        public async Task<List<Channel>> GetChannels()
        {
            string url = string.Format(RequestUris.ChannelsList, _cache.GetInfo()?.id);
            return await _graphClient.GraphApi<List<Channel>>(url, HttpMethod.Get);
        }
        public async Task<List<Team>> GetTeams()
        {
            var infos = await _graphClient.GraphApi<Dictionary<string, object>>(RequestUris.MeTeams, HttpMethod.Get);
            infos.TryGetValue("value", out object team);
            return JsonConvert.DeserializeObject<List<Team>>(JsonConvert.SerializeObject(team));
        }
        public async Task<List<Chat>> MeChats()
        {
            var infos = await _graphClient.GraphApi<Dictionary<string, object>>(RequestUris.MeChats, HttpMethod.Get);
            infos.TryGetValue("value", out object ConversationMembers);
            return JsonConvert.DeserializeObject<List<Chat>>(JsonConvert.SerializeObject(ConversationMembers));
        }
        public async Task<List<Message>> GetMessagesByUser(string chatid, int number = 50)
        {
            var infos = await _graphClient.GraphApi<Dictionary<string, object>>(string.Format(RequestUris.GetChats, chatid, number), HttpMethod.Get);
            infos.TryGetValue("value", out object Message);
            return JsonConvert.DeserializeObject<List<Message>>(JsonConvert.SerializeObject(Message));

        }

        public async Task<bool> SendMessage(string content, string chatid)
        {
            string json = $"{{\"body\":{{\"content\":\"{content}\"}}}}";
            var infos = await _graphClient.GraphApi<Message>(string.Format(RequestUris.SendMessage, chatid), json);
            return true;
        }
    }
}
