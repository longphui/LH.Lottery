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
    public class SetLotteryPredictionTotalModel : PageModel
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IPredictionService _pService;
        private readonly ILogger<ContactModel> _logger;
        public SetLotteryPredictionTotalModel(IHostingEnvironment hostingEnvironment,
            IPredictionService pService,
            ILogger<ContactModel> logger)
        {
            this._hostingEnvironment = hostingEnvironment;
            this._pService = pService;
            this._logger = logger;
        }

        public string Message { get; set; }
        public void OnGet(string lotteryCode)
        {
            try
            {
                _pService.InsertLotteryPredictionTotal_T(lotteryCode);

                //_pService.SetLotteryPredictionTotal();
                //_pService.InsertLotteryPredictionTotal_T("19095");

                //_pService.InsertLotteryPredictionTotal_T("19096");
                //_pService.InsertLotteryPredictionTotal_T("19097");
                Message = "success";
            }
            catch (Exception ex)
            {
                Message = ex.ToString();
            }
        }
    }
}