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
using Nigel.SignalR.Server.Hubs;

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
                      .WithOrigins(Configuration["App:CorsOrigins"].ToStringArray(",", "��"))
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
            .AddMessagePackProtocol()
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

            //֧�ֿ���
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
