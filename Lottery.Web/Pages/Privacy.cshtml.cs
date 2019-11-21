using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lottery.ML.Domain.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Lottery.Web.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ITrainingGroundService _tgService;
        private readonly ILogger<PrivacyModel> _logger;

        ILotteryResultService lotteryResultService;
        public PrivacyModel(IHostingEnvironment hostingEnvironment,
            ITrainingGroundService tgService,
            ILogger<PrivacyModel> logger,
        ILotteryResultService lotteryResultService)
        {
            this._hostingEnvironment = hostingEnvironment;
            this._tgService = tgService;
            this._logger = logger;
            this.lotteryResultService = lotteryResultService;
        }

        public void OnGet()
        {
            //string webRootPath = _hostingEnvironment.WebRootPath;
            //_tgService.BuildTrainingGround(webRootPath);
            lotteryResultService.SaveNewLotteryResult();
        }
    }
}