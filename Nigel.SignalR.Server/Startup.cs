using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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

            ////客户端发保持连接请求到服务端最长间隔，默认30秒，改成4分钟，网页需跟着设置connection.keepAliveIntervalInMilliseconds = 12e4;即2分钟
            //options.ClientTimeoutInterval = TimeSpan.FromMinutes(4);
            ////服务端发保持连接请求到客户端间隔，默认15秒，改成2分钟，网页需跟着设置connection.serverTimeoutInMilliseconds = 24e4;即4分钟
            //options.KeepAliveInterval = TimeSpan.FromMinutes(2);

            // SignalR
            services.AddSignalR(options =>
            {
                // _webEnv为通过依赖注入在Startup的构造函数中注入的 IWebHostEnvironment
                if (_webEnv.IsDevelopment())
                {
                    options.EnableDetailedErrors = true;
                }
            })
            .AddMessagePackProtocol()
            // 使用redis做底板 支持横向扩展 Scale-out
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
            .AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.PropertyNamingPolicy = null;
            });

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

            //支持跨域
            app.UseCors(policyName);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<BroadcastHub>("/hubs/broadcast");
            });

            app.UseWebSockets();

            #endregion
        }
    }
}
