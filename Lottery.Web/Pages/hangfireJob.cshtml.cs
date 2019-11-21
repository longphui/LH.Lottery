using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Lottery.ML.Domain.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Lottery.Web.Pages
{
    public class hangfireJobModel : PageModel
    {

        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IPredictionService _pService;
        private readonly ILogger<ContactModel> _logger;
        ILotteryResultService lotteryResultService;
        public hangfireJobModel(IHostingEnvironment hostingEnvironment,
            IPredictionService pService,
            ILogger<ContactModel> logger, ILotteryResultService lotteryResultService)
        {
            this._hostingEnvironment = hostingEnvironment;
            this._pService = pService;
            this._logger = logger;
            this.lotteryResultService = lotteryResultService;
        }
        public void OnGet()
        {
            RecurringJob.AddOrUpdate<ILotteryResultService>(e => e.SaveNewLotteryResult(), Cron.Daily(21), TimeZoneInfo.Local);
            string webRootPath = _hostingEnvironment.WebRootPath;
            RecurringJob.AddOrUpdate<IPredictionService>(e => e.Prediction(webRootPath,""), Cron.Daily(22), TimeZoneInfo.Local);
        }
    }
}