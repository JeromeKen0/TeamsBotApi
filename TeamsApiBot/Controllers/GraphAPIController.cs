using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;

namespace TeamsApiBot.Controllers
{
    public class GraphAPIController : ControllerBase
    {
        private readonly GraphServiceClient _graphClient;
        public GraphAPIController(GraphServiceClient graphServiceClient)
        {
            _graphClient = graphServiceClient;
        }
        [Route("/User")]
        [HttpGet]
        public async Task<User> User()
        {
            return await _graphClient.Me.Request()
                                        .Select(u => new
                                        {
                                            u.DisplayName,
                                            u.JobTitle
                                        }).GetAsync();
        }
    }
}
