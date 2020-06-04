using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessagePack;
using MessagePack.Resolvers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nigel.Core;
using Nigel.Extensions;
using Nigel.SignalR.Server.Hubs;
using StackExchange.Redis;

namespace Nigel.SignalR.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            this._webEnv = env;
        }

        const string policyName = "CorsPolicy";
        private readonly IWebHostEnvironment _webEnv;
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            #region [ Add SignalR ]

            services.AddScoped<IBroadcastHub, BroadcastHub>();

            services.AddCors(options => options.AddPolicy(policyName, builder =>
            {
                builder
                      .SetIsOriginAllowedToAllowWildcardSubdomains()
                      .WithOrigins(Configuration["App:CorsOrigins"].Split(",", true))
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials()
                      .Build();
            }));

            ////�ͻ��˷������������󵽷����������Ĭ��30�룬�ĳ�4���ӣ���ҳ���������connection.keepAliveIntervalInMilliseconds = 12e4;��2����
            //options.ClientTimeoutInterval = TimeSpan.FromMinutes(4);
            ////����˷������������󵽿ͻ��˼����Ĭ��15�룬�ĳ�2���ӣ���ҳ���������connection.serverTimeoutInMilliseconds = 24e4;��4����
            //options.KeepAliveInterval = TimeSpan.FromMinutes(2);

            // SignalR
            services.AddSignalR(options =>
            {
                // _webEnvΪͨ������ע����Startup�Ĺ��캯����ע��� IWebHostEnvironment
                if (_webEnv.IsDevelopment())
                {
                    options.EnableDetailedErrors = true;
                }
            })
            //.AddJsonProtocol()
            .AddMessagePackProtocol(options =>
            {
                //options.SerializerOptions = StandardResolver.Options;
                options.FormatterResolvers = new List<IFormatterResolver>(){
                    StandardResolver.Instance,
                    DynamicEnumAsStringResolver.Instance
                };
            })
            // ʹ��redis���װ� ֧�ֺ�����չ Scale-out
            //.AddStackExchangeRedis(o =>
            //{
            //    o.ConnectionFactory = async writer =>
            //    {
            //        var config = new ConfigurationOptions
            //        {
            //            AbortOnConnectFail = false,
            //            ChannelPrefix = "__signalr_",
            //        };
            //        config.DefaultDatabase = 10;
            //        var connection = await ConnectionMultiplexer.ConnectAsync(appSetting.SignalrRedisCache.ConnectionString, writer);
            //        connection.ConnectionFailed += (_, e) =>
            //        {
            //            Console.WriteLine("Connection to Redis failed.");
            //        };

            //        if (connection.IsConnected)
            //        {
            //            Console.WriteLine("connected to Redis.");
            //        }
            //        else
            //        {
            //            Console.WriteLine("Did not connect to Redis");
            //        }

            //        return connection;
            //    };
            //})
            ;
            #endregion

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            #region [ Use SignalR ]

            //֧�ֿ���
            app.UseCors(policyName);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<BroadcastHub>("/hubs/broadcast", options =>
                {
                    options.Transports = HttpTransportType.WebSockets | HttpTransportType.ServerSentEvents | HttpTransportType.LongPolling;
                    options.LongPolling.PollTimeout = TimeSpan.FromSeconds(10);
                });
            });

            app.UseWebSockets();

            #endregion
        }
    }
}
