using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TeamsApiBot.CacheManages;
using TeamsApiBot.Configs;
using TeamsApiBot.Models;
using TeamsApiBot.Services;

namespace TeamsApiBot.HttpClients
{
    public class GraphClient
    {
        private readonly HttpClient _client;
        private readonly CacheManage _CacheManage;
        //private readonly IAccessService _accessService;

        public GraphClient(HttpClient client, CacheManage CacheManage)
        {
            //_accessService = accessService;
            _client = client;
            _CacheManage = CacheManage;
            _client.BaseAddress = new Uri(RequestUris.BaseGraphUri);
            _client.Timeout = new TimeSpan(0, 0, 30);
            _client.DefaultRequestHeaders.Clear();
            if (!string.IsNullOrEmpty(_CacheManage.GetAccToken()))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _CacheManage.GetAccToken());
            }
        }
        public async Task<UserInfo> CallMicrosoftGraph(TokenResponseModel tokenResponseModel)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenResponseModel.token_type, tokenResponseModel.access_token);
            var request = new HttpRequestMessage(HttpMethod.Get, RequestUris.CallMicrosoftGraph);

            // Call API and get the response
            using (var response = await _client.SendAsync(request))
            {
  
                // Ensure we have a Success Status Code
                response.EnsureSuccessStatusCode();
                // Read Response Content (this will usually be JSON content)
                var content = await response.Content.ReadAsStringAsync();
                // Deserialize the JSON into the C# T object and return
                var userInfo = JsonConvert.DeserializeObject<UserInfo>(content);
                userInfo.context = JObject.Parse(content).Value<string>("@odata.context");
                return userInfo;
            }
        }
        public async Task<T> GraphApi<T>(string url, HttpMethod httpMethod)
        {
            var request = new HttpRequestMessage(httpMethod, url);

            // Call API and get the response
            using (var response = await _client.SendAsync(request))
            {
                //if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                //{
                //    await _accessService.RefreshToken();
                //}
                // Ensure we have a Success Status Code
                response.EnsureSuccessStatusCode();
                // Read Response Content (this will usually be JSON content)
                var content = await response.Content.ReadAsStringAsync();
                // Deserialize the JSON into the C# T object and return
                return JsonConvert.DeserializeObject<T>(content);
            }
        }
        public async Task<T> GraphApi<T>(string url, string Content)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var requestContent = new StringContent(Content, Encoding.UTF8, "application/json");
            // Call API and get the response
            using (var response = await _client.PostAsync(url, requestContent))
            {
                // Ensure we have a Success Status Code
                //if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                //{
                //    await _accessService.RefreshToken();
                //}
                response.EnsureSuccessStatusCode();
                // Read Response Content (this will usually be JSON content)
                var content = await response.Content.ReadAsStringAsync();
                // Deserialize the JSON into the C# T object and return
                return JsonConvert.DeserializeObject<T>(content);
            }
        }
    }
}
