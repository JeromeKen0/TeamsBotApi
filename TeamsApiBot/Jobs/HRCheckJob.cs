using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamsApiBot.Jobs
{
    [DisallowConcurrentExecution]
    public class HRCheckJob : IJob
    {
        private readonly IServiceProvider _serviceProvider;
        public HRCheckJob(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
