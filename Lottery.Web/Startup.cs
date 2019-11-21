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


            //�Դ���log
            services.AddLogging();

            //ע���̨��ʱ����
            //services.AddHostedService<TimedHostedService>();

            //����HangFire
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
                Queues = new[] { "test", "default" },//�������ƣ�ֻ��ΪСд
                WorkerCount = 1,// Environment.ProcessorCount * 5, //����������
                ServerName = "hangfire1",//����������
            };
            app.UseHangfireServer(jobOptions);//����Hangfire����
            app.UseHangfireDashboard();//����hangfire���
            //GlobalConfiguration.Configuration.UseSQLiteStorage("Data Source=./data.db;");
        }
    }
}
