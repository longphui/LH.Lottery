using Lottery.ML.Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lottery.ML.Domain
{
    public interface ILotteryResultRepository:IRepository<LotteryResult>
    {
        bool Insert(LotteryResult lotteryResult);
        LotteryResult GetById(string Id);
        IList<LotteryResult> Get100Rows(int beginRow = 0, int rowCount = 100);
        IList<LotteryResult> GetAll();
        IList<LotteryResult> GetLast100Rows();

        IList<LotteryResult> GetLast100Rows(string id);

        LotteryResult GetLastRow();

        bool InsertPredictionLog(LotteryPredictionLog predictionLog);
        bool InsertPredictionResult(LotteryPredictionResult predictionResult);
        bool UpdatePredictionLog(LotteryPredictionLog predictionLog);
        LotteryPredictionLog GetLotteryPredictionLog(string lotteryCode);

        LotteryPredictionLog GetLastSuccessLog();
        bool DeleteErrorPrediction(string lotteryCode);
        LotteryResult GetNextLotteryCode(string lotteryCode);

        bool SetLotteryPredictionTotal(string lotteryCode);

        bool InsertLotteryPredictionCode_4_Temp(string lotteryCode);

        bool InsertLotteryPredictionTotal_T(string lotteryCode);
    }
}
