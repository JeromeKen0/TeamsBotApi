using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Serialization;
using TeamsApiBot.CacheManages;
using TeamsApiBot.Configs;
using TeamsApiBot.HttpClients;
using TeamsApiBot.Jobs;
using TeamsApiBot.Services;
using Telegram.Bot;

namespace TeamsApiBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var KeyVaultOptions = Configuration.GetSection("ClientSettings");
            var TelegramBotToken = Configuration.GetValue<string>("TelegramBotToken");
            services.Configure<KeyVaultOptions>(KeyVaultOptions);
            services.AddHttpClient<LoginClient>();
            services.AddHttpClient<GraphClient>();
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });


            IEnumerable<string> graphScopes = new string[] { "offline_access", "user.read", "email", "openid" };
            IPublicClientApplication publicClientApplication = PublicClientApplicationBuilder
                                                                .Create(KeyVaultOptions["ClientId"])
                                                                .Build();
            // Create an authentication provider by passing in a client application and graph scopes.
            DeviceCodeProvider authProvider = new DeviceCodeProvider(publicClientApplication, graphScopes);
            // Create a new instance of GraphServiceClient with the authentication provider.
            GraphServiceClient graphClient = new GraphServiceClient(authProvider);

            IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
                                                                            .Create(KeyVaultOptions["ClientId"])
                                                                            .WithRedirectUri(KeyVaultOptions["CertificateUrl"])
                                                                            .WithClientSecret(KeyVaultOptions["ClientSecret"])
                                                                            .Build();

            services.AddMemoryCache();
            services.AddQuartz();
            services.AddSingleton(new GraphServiceClient(authProvider));
            services.AddSingleton(new OnBehalfOfProvider(confidentialClientApplication, graphScopes));
            services.AddSingleton<CacheManage>();
            services.AddScoped<IAccessService, AccessService>();
            services.AddScoped<IGraphService, GraphService>();
            services.AddSingleton(new TelegramBotClient(TelegramBotToken));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseQuartz();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
