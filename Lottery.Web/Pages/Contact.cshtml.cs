using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lottery.ML.Domain.Model;
using Lottery.ML.Domain.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.ML.Legacy;
using Microsoft.ML.Legacy.Data;
using Microsoft.ML.Legacy.Models;
using Microsoft.ML.Legacy.Trainers;
using Microsoft.ML.Legacy.Transforms;

namespace Lottery.Web.Pages
{
    public class ContactModel : PageModel
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IPredictionService _pService;
        private readonly ILogger<ContactModel> _logger;
        public ContactModel(IHostingEnvironment hostingEnvironment,
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
            string webRootPath = _hostingEnvironment.WebRootPath;
            //_pService.PredictionAllToFile(webRootPath);
            _pService.Prediction(webRootPath, lotteryCode);
        }
    }
}
