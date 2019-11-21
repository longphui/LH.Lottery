using Lottery.ML.Domain.Service;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Lottery.Web.Service
{
    internal class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;
        ILotteryResultService lotteryResultService;
        IPredictionService predictionService;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;

        public TimedHostedService(ILogger<TimedHostedService> logger,
            ILotteryResultService lotteryResultService, 
            IPredictionService predictionService,
            Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
        {
            _logger = logger;
            this.lotteryResultService = lotteryResultService;
            this.predictionService = predictionService;
            this._hostingEnvironment = hostingEnvironment;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            //_timer = new Timer(DoWork, null, TimeSpan.Zero,
            //    TimeSpan.FromMinutes(30));

            //_timer = new Timer(DoWork, null, TimeSpan.Zero,
            //  TimeSpan.FromHours(4));

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
              TimeSpan.FromMinutes(5));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            try
            {
                //_logger.LogInformation("Timed Background Service is working.");

                //_logger.LogInformation("预测下一个开奖");
                //string webRootPath = _hostingEnvironment.WebRootPath;
                //predictionService.PredictionNext(webRootPath);


                //_logger.LogInformation("获取新开奖结果");
                //lotteryResultService.SaveNewLotteryResult();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("定时任务出错:"+ex.ToString());
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
