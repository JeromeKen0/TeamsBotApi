using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeamsApiBot.Services;

namespace TeamsApiBot.Controllers
{
    [Route("[controller]")]
    public class ChannelController : ControllerBase
    {
        private readonly CacheManages.CacheManage _CacheManage;
        private readonly IGraphService _graphService;
        public ChannelController(CacheManages.CacheManage CacheManage, IGraphService graphService)
        {
            _CacheManage = CacheManage;
            _graphService = graphService;
        }
        [HttpGet,Route("GetChannels")]
        public async Task<IActionResult> GeChannels()
        {
            var channels = await _graphService.GetChannels();
            return Ok(channels);
        }   
        [HttpGet,Route("GetTeams")]
        public async Task<IActionResult> GetTeams()
        {
            var channels = await _graphService.GetTeams();
            return Ok(channels);
        } 
        [HttpGet,Route("GetChats")]
        public async Task<IActionResult> GetChats()
        {
            var Chats = await _graphService.MeChats();
            return Ok(Chats);
        }    
        [HttpGet,Route("GetMessages")]
        public async Task<IActionResult> GetMessages()
        {
            var Chats = await _graphService.GetMessagesByUser("");
            return Ok(Chats);
        }
    }
}
