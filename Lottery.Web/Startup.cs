using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.SQLite;
using Lottery.ML.Domain;
using Lottery.ML.Domain.Infrastructure;
using Lottery.ML.Domain.Service;
using Lottery.Open.Service;
using Lottery.Web.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lottery.Web
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            //自带的log
            services.AddLogging();

            //注册后台即时服务
            //services.AddHostedService<TimedHostedService>();

            //配置HangFire
            services.AddHangfire(x => x.UseSQLiteStorage("Data Source="+ AppDomain.CurrentDomain.BaseDirectory + Configuration.GetConnectionString("hangfireConn") + ";"));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSingleton<IOpenService, OpenService>();
            services.AddSingleton<ILotteryResultService, LotteryResultService>();

            //services.AddSingleton<ILotteryResultRepository, LotteryResultRepository>();
            services.AddTransient<ILotteryResultRepository, MsSqlLotteryResultRepository>();
            services.AddTransient<ILotteryResultRepository, SqliteLotteryResultRepository>();

            services.AddSingleton<ITrainingGroundService, TrainingGroundService>();
            services.AddSingleton<IPredictionService, PredictionService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc();

            var jobOptions = new BackgroundJobServerOptions
            {
                Queues = new[] { "test", "default" },//队列名称，只能为小写
                WorkerCount = 1,// Environment.ProcessorCount * 5, //并发任务数
                ServerName = "hangfire1",//服务器名称
            };
            app.UseHangfireServer(jobOptions);//启动Hangfire服务
            app.UseHangfireDashboard();//启动hangfire面板
            //GlobalConfiguration.Configuration.UseSQLiteStorage("Data Source=./data.db;");
        }
    }
}
