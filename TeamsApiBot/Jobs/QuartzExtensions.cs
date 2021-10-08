using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamsApiBot.Jobs
{
    public static class QuartzExtensions
    {
        private static IJobDetail QueryOrderJobSettle;
        private static IJobDetail DrinkingAssistantJobSettle;

        private static ITrigger QueryOrderJobtrigger;
        private static ITrigger DrinkingAssistantJobtrigger;
        public static void AddQuartz(this IServiceCollection services)
        {
            //, Type jobType, TimeSpan timeSpan
            services.AddSingleton<QueryMessageJob>();
            services.AddSingleton<IJobFactory, JobScheduler>();

            QueryOrderJobSettle = JobBuilder.Create<QueryMessageJob>().WithIdentity($"QueryOrderJobSettle", $"QueryOrderJobJob").Build();

            QueryOrderJobtrigger = TriggerBuilder.Create().WithIdentity($"QueryOrderJob.trigger", $"QueryOrderJobtrigger").StartNow().WithSimpleSchedule(s => s.WithInterval(TimeSpan.FromSeconds(30)).RepeatForever()).Build();
            //DrinkingAssistantJobtrigger = TriggerBuilder.Create().WithIdentity($"DrinkingAssistantJob.trigger", $"DrinkingAssistantJobtrigger").StartNow().WithSimpleSchedule(s => s.WithInterval(TimeSpan.FromHours(1)).RepeatForever()).Build();

            services.AddSingleton(provider =>
            {
                var schedulerFactory = new StdSchedulerFactory();
                var scheduler = schedulerFactory.GetScheduler().Result;
                scheduler.JobFactory = provider.GetService<IJobFactory>();
                scheduler.ScheduleJob(DrinkingAssistantJobSettle, DrinkingAssistantJobtrigger);
                scheduler.ScheduleJob(QueryOrderJobSettle, QueryOrderJobtrigger);
                scheduler.Start();
                return scheduler;
            });
        }
        public static void UseQuartz(this IApplicationBuilder app)
        {
            app.ApplicationServices.GetService<IScheduler>();
        }
    }
}
