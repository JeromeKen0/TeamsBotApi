using Microsoft.Graph;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TeamsApiBot.Configs;
using TeamsApiBot.Models;

namespace TeamsApiBot.HttpClients
{
    public class LoginClient
    {
        private readonly HttpClient _client;

        public LoginClient(HttpClient client)
        {
            _client = client;
            _client.BaseAddress = new Uri(RequestUris.BaseLoginUri);
            _client.Timeout = new TimeSpan(0, 0, 30);
            _client.DefaultRequestHeaders.Clear();
        }
        public async Task Authorize(string ClientId)
        {
            AuthorizationRequestModel authorizationRequest = new AuthorizationRequestModel()
            {
                client_id = ClientId,
                response_type = "code",
                response_mode = "query",
                redirect_uri = Configs.RequestUris.redirect_uri,
                scope = "offline_access+user.read+mail.read",
                state = "12345"
            };
            string queryString = Helper.QueryString(authorizationRequest);
            // Set the request message
            var request = new HttpRequestMessage(HttpMethod.Get, $"{RequestUris.AuthorizeUri}?{queryString}");
            // Call API and get the response
            using (var response = await _client.SendAsync(request))
            {
                // Ensure we have a Success Status Code
                response.EnsureSuccessStatusCode();
                // Read Response Content (this will usually be JSON content)
                var content = await response.Content.ReadAsStringAsync();

            }
        }

        public async Task<TokenResponseModel> GetToken(TokenRequestModel tokenRequestModel) 
        {
            string queryString = Helper.QueryString(tokenRequestModel);

            var httpcontent = new StringContent(queryString, Encoding.UTF8, "application/x-www-form-urlencoded");

            // Call API and get the response
            using (var response = await _client.PostAsync($"{RequestUris.BaseLoginUri}/{RequestUris.TokenRequestUri}", httpcontent))
            {
                // Ensure we have a Success Status Code
                response.EnsureSuccessStatusCode();
                // Read Response Content (this will usually be JSON content)
                var content = await response.Content.ReadAsStringAsync();
                //
                return JsonConvert.DeserializeObject<TokenResponseModel>(content);
            }
        }
        public async Task<TokenResponseModel> RefToken(RefreshTokenModel  refreshTokenModel)
        {
            string queryString = Helper.QueryString(refreshTokenModel);

            var httpcontent = new StringContent(queryString, Encoding.UTF8, "application/x-www-form-urlencoded");

            // Call API and get the response
            using (var response = await _client.PostAsync($"{RequestUris.BaseLoginUri}/{RequestUris.TokenRequestUri}", httpcontent))
            {
                // Ensure we have a Success Status Code
                response.EnsureSuccessStatusCode();
                // Read Response Content (this will usually be JSON content)
                var content = await response.Content.ReadAsStringAsync();
                //
                return JsonConvert.DeserializeObject<TokenResponseModel>(content);
            }
        }
    }
}
