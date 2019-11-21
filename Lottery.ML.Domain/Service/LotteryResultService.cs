using Lottery.ML.Domain.Model;
using Lottery.Open.Model;
using Lottery.Open.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.ML.Domain.Service
{
    public class LotteryResultService: ILotteryResultService
    {
        private readonly ILogger _logger;
        ILotteryResultRepository resultRepo;
        IOpenService openService;
        public LotteryResultService(IEnumerable<ILotteryResultRepository> resultRepos, 
            IOpenService openService, ILogger<LotteryResultService> logger, 
            IConfiguration config)
        {
            this._logger = logger;
            this.resultRepo = resultRepos.FirstOrDefault(x => x.DBType == config["DBType"]); 
            this.openService = openService;
        }

        public void SaveNewLotteryResult()
        {
            _logger.LogInformation("获取最新开奖结果");
            OpenResult openResult = openService.GetOpenResult();
            if (openResult != null)
            {
                LotteryResult lotteryResult = new LotteryResult(openResult.periodicalnum,
                    openResult.result,openResult.resulttime);
                var oldResult = resultRepo.GetById(lotteryResult.Id);
                if (oldResult == null)
                {
                    resultRepo.Insert(lotteryResult);


                    resultRepo.InsertLotteryPredictionCode_4_Temp(lotteryResult.Id);
                    resultRepo.InsertLotteryPredictionTotal_T(lotteryResult.Id);
                }
            }
        }
    }
}
