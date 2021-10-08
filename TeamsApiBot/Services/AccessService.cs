using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamsApiBot.CacheManages;
using TeamsApiBot.Configs;
using TeamsApiBot.HttpClients;
using TeamsApiBot.Models;

namespace TeamsApiBot.Services
{
    public interface IAccessService
    {
        Task<string> GetAuthorizeUri();
        Task<UserInfo> Redirect(AuthorizationResponseModel authorizationResponseModel);
        Task RefreshToken();
    }
    public class AccessService : IAccessService
    {
        private readonly IOptions<KeyVaultOptions> _options;
        private readonly LoginClient _loginClient;
        private readonly GraphClient _graphClient;
        private readonly string scope;
        private readonly CacheManage _CacheManage;
        public AccessService(IOptions<KeyVaultOptions> options, LoginClient loginClient, GraphClient graphClient,CacheManage CacheManage)
        {
            _options = options;
            _loginClient = loginClient;
            scope = string.Join(' ', Scopes.scopes);
            _graphClient = graphClient;
            _CacheManage = CacheManage;
        }
        public async Task<string> GetAuthorizeUri()
        {
            AuthorizationRequestModel authorizationRequest = new AuthorizationRequestModel()
            {
                client_id = _options.Value.ClientId,
                response_type = "code",
                response_mode = "query",
                redirect_uri = Configs.RequestUris.redirect_uri,
                scope = scope,
                state = "12345"
            };
            return await Task.FromResult(Helper.QueryString(authorizationRequest));
        }

        public async Task<UserInfo> Redirect(AuthorizationResponseModel authorizationResponseModel)
        {
            var userInfo = _CacheManage.GetInfo();
            if (_CacheManage.GetInfo() == null)
            {
                TokenRequestModel tokenRequest = new TokenRequestModel()
                {
                    client_id = _options.Value.ClientId,
                    grant_type = "authorization_code",
                    scope = scope,
                    code = authorizationResponseModel.code,
                    redirect_uri = RequestUris.redirect_uri,
                    client_secret = _options.Value.ClientSecret,
                };
                var tokenResponse = await _loginClient.GetToken(tokenRequest);
                _CacheManage.SetToken(tokenResponse);
                userInfo= await _graphClient.CallMicrosoftGraph(tokenResponse);
                _CacheManage.SetUserInfo(userInfo);
            }
            return userInfo;
        }    
        public async Task RefreshToken()
        {
            RefreshTokenModel refreshTokenModel = new RefreshTokenModel()
            {
                client_id = _options.Value.ClientId,
                client_secret = _options.Value.ClientSecret,
                scope = scope,
                grant_type = "refresh_token",
                redirect_uri = RequestUris.redirect_uri,
                refresh_token = _CacheManage.GetRefToken()
            };
            var tokenResponse = await _loginClient.RefToken(refreshTokenModel);
            _CacheManage.SetToken(tokenResponse);
        }
    }
}
