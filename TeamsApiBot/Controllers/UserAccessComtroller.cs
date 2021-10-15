using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TeamsApiBot.Configs;
using TeamsApiBot.HttpClients;
using TeamsApiBot.Models;
using TeamsApiBot.Services;
using Telegram.Bot;

namespace TeamsApiBot.Controllers
{
    public class UserAccessComtroller : ControllerBase
    {
        private readonly IAccessService _accessService;
        private readonly CacheManages.CacheManage _CacheManage;
        private readonly IGraphService _graphService;
        private readonly IMemoryCache _memoryCache;

        public UserAccessComtroller(IAccessService accessService, CacheManages.CacheManage CacheManage, IGraphService graphService, IMemoryCache memoryCache)
        {
            _accessService = accessService;
            _CacheManage = CacheManage;
            _graphService = graphService;
            _memoryCache = memoryCache;
        }
        [Route("/Authorize")]
        public async Task Authorize()
        {
            var querytimes = await _CacheManage.GetChatTimeConfig();
            _memoryCache.Set("QueryTimes", querytimes);
            _memoryCache.Set("ChatTimes", new Dictionary<string, string>());
            string queryString = await _accessService.GetAuthorizeUri();
            Response.Redirect($"{RequestUris.BaseLoginUri}/{RequestUris.AuthorizeUri}?{queryString}");
        }
        [Route("/redirect")]
        [HttpGet]
        public async Task<IActionResult> redirect(AuthorizationResponseModel authorizationResponseModel)
        {
            //获取用户信息
            var userInfo = await _accessService.Redirect(authorizationResponseModel);
            return Ok(userInfo);
        }
        [Route("/ShowCacheTime")]
        [HttpGet]
        public IActionResult ShowCacheTime()
        {
            var ChatTimes = _memoryCache.Get<Dictionary<string, DateTime>>("QueryTimes");
            return Ok(JsonConvert.SerializeObject(ChatTimes));
        }
        [Route("/GetTimesTest")]
        [HttpGet]
        public IActionResult GetTimesTest()
        {
            var times = _CacheManage.GetChatTimeConfig();
            return Ok(JsonConvert.SerializeObject(times));
        }
        [Route("/Run")]
        public async Task<IActionResult> Run()
        {
            //查询所有聊天信息，抓取查岗人员
            var Chats = await _graphService.MeChats();
            var UserChats = Chats.Where(s => s.chatType == ChatType.oneOnOne.ToString());
            var user = _CacheManage.GetInfo();
            Chat HR = new Chat();
            foreach (var chat in UserChats)
            {
                var messages = await _graphService.GetMessagesByUser(chat.id);
                //if (chat.id == "19:8b4dbf79-7f92-44f6-8440-9c36a09ba94b_ec52b952-f968-43a4-a87c-7cf89fbcb510@unq.gbl.spaces") 
                //{

                //}
                messages.ForEach(message =>
                {
                    //ss -ntl
                    //例行查岗
                    if (message.body.content.Contains("ss -ntl") && message.from.user.id != user.id)
                    {
                        HR.id = chat.id;
                    }
                });
            }
            _CacheManage.SetChat(HR);
            var HRmessages = await _graphService.GetMessagesByUser(HR.id);

            return Ok(HRmessages);
        }
        [Route("/SendMessage")]
        public async Task<IActionResult> SendMessageTest([FromServices] TelegramBotClient telegramBotClient)
        {
            await telegramBotClient.SendTextMessageAsync("@TeamsChatNotify", "测试消息");
            return Ok();
        }
    }
}
