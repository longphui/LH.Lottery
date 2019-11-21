using Lottery.ML.Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lottery.ML.Domain.Service
{
    public interface IPredictionService
    {
        Dictionary<string, TrainingData> GetPredictionData(string lotteryCode);

        void PredictionOne(string webRootPath, string noSite, string noType, TrainingData data, string lotteryCode);

        LotteryPredictionResult PredictionOneToFile(string webRootPath, string noSite, string noType, TrainingData data, string lotteryCode);

        void Prediction(string webRootPath, string lotteryCode);

        void PredictionToFile(string webRootPath, string lotteryCode);

        void PredictionAll(string webRootPath);

        void PredictionAllToFile(string webRootPath);

        void PredictionNext1(string webRootPath);

        string GetLotteryCode();

        void SetLotteryPredictionTotal();

        void InsertLotteryPredictionTotal_T(string lotteryCode);
    }
}
