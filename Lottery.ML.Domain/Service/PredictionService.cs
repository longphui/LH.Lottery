using Lottery.Common;
using Lottery.ML.Domain.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.ML.Legacy;
using Microsoft.ML.Legacy.Data;
using Microsoft.ML.Legacy.Trainers;
using Microsoft.ML.Legacy.Transforms;
using Microsoft.ML.Legacy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;

namespace Lottery.ML.Domain.Service
{
    public class PredictionService : IPredictionService
    {
        private readonly ILogger _logger;
        ILotteryResultRepository resultRepo;
        readonly List<string> noSite = new List<string> { "first", "second", "third", "fourth", "fifth", "sixth", "seventh" };
        readonly List<string> noType = new List<string> { "Size", "Parity", "Prime" };
        //readonly List<string> noType = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "Size", "Parity" };
        public PredictionService(IEnumerable<ILotteryResultRepository> resultRepos,
                ILogger<PredictionService> logger,
                IConfiguration config)
        {
            this._logger = logger;
            this.resultRepo = resultRepos.FirstOrDefault(x => x.DBType == config["DBType"]);
        }

        public Dictionary<string, TrainingData> GetPredictionData(string lotteryCode)
        {
            _logger.LogInformation("获取前99条结果值用于预测");
            //var noSite = new List<string> { "first", "second", "third", "fourth", "fifth", "sixth", "seventh" };
            Dictionary<string, TrainingData> dicTrainingData = new Dictionary<string, TrainingData>();
            IList<LotteryResult> resultList = resultRepo.GetLast100Rows(lotteryCode);
            if (resultList.Count < 99)
            {
                throw new Exception(lotteryCode+"期之前的开奖数不够99");
            }
            DateTime lastDate = GetNextLotteryDate(resultList[resultList.Count - 1].LotteryDate);
            long timeStamp = DateTimeUtility.ConvertToTimeStamp(lastDate) / 100000;
            for (var i = 0; i < noSite.Count; i++)
            {
                dicTrainingData.Add(noSite[i], new TrainingData());
                var t = dicTrainingData[noSite[i]].GetType();
                for (int j = 0; j < resultList.Count; j++)
                {
                    t.GetField("t" + j).SetValue(dicTrainingData[noSite[i]], float.Parse(resultList[j].No1));
                }
                dicTrainingData[noSite[i]].t99 = timeStamp;
            }
            return dicTrainingData;
        }

        public void PredictionOne(string webRootPath, string noSite, string noType, TrainingData data, string lotteryCode)
        {
            var pipeline = new LearningPipeline();
            string dataPath = webRootPath + $"/TrainingGround/{noSite}{noType}.txt";
            pipeline.Add(new TextLoader(dataPath).CreateFrom<TrainingData>(separator: ','));
            pipeline.Add(new Dictionarizer("Label"));
            pipeline.Add(new ColumnConcatenator("Features", TrainingData.GetColumns()));
            pipeline.Add(new LogisticRegressionBinaryClassifier());
            pipeline.Add(new PredictedLabelColumnOriginalValueConverter()
            {
                PredictedLabelColumn = "PredictedLabel"
            });
            _logger.LogInformation("Start PredictionOne :" + lotteryCode + "—" + noSite + noType);
            var model = pipeline.Train<TrainingData, LotteryPrediction>();
            var testData = new TextLoader(dataPath).CreateFrom<TrainingData>(separator: ',');
            var evaluator = new BinaryClassificationEvaluator();
            var metrics = evaluator.Evaluate(model, testData);
            TrainingData newPoint = data;
            LotteryPrediction prediction = model.Predict(newPoint);
            string result = prediction.PredictedLabels;
            _logger.LogInformation("End PredictionOne :" + lotteryCode + "—" + noSite + noType);
            resultRepo.InsertPredictionResult(new LotteryPredictionResult()
            {
                PredictionType = noType,
                PredictionSite = noSite,
                PredictionResult = result,
                LotteryCode = lotteryCode
            });
        }

        public LotteryPredictionResult PredictionOneToFile(string webRootPath, string noSite, string noType, TrainingData data, string lotteryCode)
        {
            var pipeline = new LearningPipeline();
            string dataPath = webRootPath + $"/TrainingGround/{noSite}{noType}.txt";
            pipeline.Add(new TextLoader(dataPath).CreateFrom<TrainingData>(separator: ','));
            pipeline.Add(new Dictionarizer("Label"));
            pipeline.Add(new ColumnConcatenator("Features", TrainingData.GetColumns()));
            pipeline.Add(new LogisticRegressionBinaryClassifier());
            pipeline.Add(new PredictedLabelColumnOriginalValueConverter()
            {
                PredictedLabelColumn = "PredictedLabel"
            });
            _logger.LogInformation("Start PredictionOne :" + lotteryCode + "—" + noSite + noType);
            var model = pipeline.Train<TrainingData, LotteryPrediction>();
            var testData = new TextLoader(dataPath).CreateFrom<TrainingData>(separator: ',');
            var evaluator = new BinaryClassificationEvaluator();
            var metrics = evaluator.Evaluate(model, testData);
            TrainingData newPoint = data;
            LotteryPrediction prediction = model.Predict(newPoint);
            string result = prediction.PredictedLabels;
            _logger.LogInformation("End PredictionOne :" + lotteryCode + "—" + noSite + noType);
            return new LotteryPredictionResult()
            {
                PredictionType = noType,
                PredictionSite = noSite,
                PredictionResult = result,
                LotteryCode = lotteryCode
            };
        }

        public void Prediction(string webRootPath,string lotteryCode)
        {
            if (string.IsNullOrEmpty(lotteryCode))
            {
                lotteryCode = GetLotteryCode();
            }
            LotteryPredictionLog oplog = resultRepo.GetLotteryPredictionLog(lotteryCode);
            if (oplog != null)
            {
                if (oplog.IsSuccess)
                {
                    _logger.LogInformation(oplog.LotteryCode + " 已经预测完成");
                    return;
                }
            }
            LotteryPredictionLog plog = new LotteryPredictionLog()
            {
                LotteryCode = lotteryCode,
                StartTime = DateTime.Now,
                PredictionStatus = 0
            };
            try
            {
                _logger.LogInformation("Save PredictionLog");
                resultRepo.InsertPredictionLog(plog);
                _logger.LogInformation("Get PredictionData");
                Dictionary<string, TrainingData> dicPredictionData = GetPredictionData(lotteryCode);
                _logger.LogInformation("Start Prediction :" + lotteryCode);
                int i = 0, j = 0;
                for (i = 0; i < noSite.Count; i++)
                {
                    for (j = 0; j < noType.Count; j++)
                    {
                        PredictionOne(webRootPath, noSite[i], noType[j], dicPredictionData[noSite[i]], lotteryCode);
                    }
                }
                plog.End(true, "");
                resultRepo.UpdatePredictionLog(plog);
            }
            catch (Exception ex)
            {
                plog.End(false, ex.ToString());
                resultRepo.UpdatePredictionLog(plog);
                _logger.LogError("预测出错 :" + ex.ToString());
            }
        }

        public void PredictionToFile(string webRootPath, string lotteryCode)
        {
            if (string.IsNullOrEmpty(lotteryCode))
            {
                lotteryCode = GetLotteryCode();
            }
            try
            {
                Dictionary<string, TrainingData> dicPredictionData = GetPredictionData(lotteryCode);
                _logger.LogInformation("Start Prediction :" + lotteryCode);
                int i = 0, j = 0;
                IList<LotteryPredictionResult> resultList = new List<LotteryPredictionResult>();
                for (i = 0; i < noSite.Count; i++)
                {
                    for (j = 0; j < noType.Count; j++)
                    {
                        resultList.Add(PredictionOneToFile(webRootPath, noSite[i], noType[j], dicPredictionData[noSite[i]], lotteryCode));
                    }
                }
                var engine = new FileHelperEngine<LotteryPredictionResult>();
                engine.AppendToFile(webRootPath + "/PredictionResult/" + DateTime.Now.ToString("yyyyMMddHH") + ".Txt", resultList);
            }
            catch (Exception ex)
            {
                _logger.LogError("预测出错 :" + ex.ToString());
            }
        }

        public void PredictionAll(string webRootPath)
        {
            var reusltList = resultRepo.GetAll();
            foreach (LotteryResult lr in reusltList)
            {
                Prediction(webRootPath, lr.Id);
            }
        }

        public void PredictionNext1(string webRootPath)
        {
            var LastSuccessLog = resultRepo.GetLastSuccessLog();
            if (LastSuccessLog != null)
            {
                resultRepo.DeleteErrorPrediction(LastSuccessLog.LotteryCode);
                var nextResult = resultRepo.GetNextLotteryCode(LastSuccessLog.LotteryCode);
                if (nextResult != null)
                {
                    Prediction(webRootPath, nextResult.Id);
                }
                else
                {
                    Prediction(webRootPath, "");
                }
            }
            else {
                Prediction(webRootPath, "");
            }
        }

        public void PredictionAllToFile(string webRootPath)
        {
            var LastSuccessLog = resultRepo.GetLastSuccessLog();
            if (LastSuccessLog != null)
            {
                var nextResult = resultRepo.GetNextLotteryCode(LastSuccessLog.LotteryCode);
                var reusltList = resultRepo.GetAll();
                if (nextResult != null)
                {
                    int nextId = int.Parse(nextResult.Id);
                    foreach (LotteryResult lr in reusltList)
                    {
                        if (int.Parse(lr.Id) < nextId)
                        {
                            continue;
                        }
                        else
                        {

                            PredictionToFile(webRootPath, lr.Id);
                        }
                    }
                }
                else
                {
                    PredictionToFile(webRootPath, "");
                }
            }
            else
            {
                PredictionToFile(webRootPath, "");
            }
            //var engine = new FileHelperEngine<LotteryPredictionResult>();
            //engine.WriteFile(webRootPath + "/PredictionResult/" + DateTime.Now.ToString("yyyyMMddHH") + ".Txt", resultList);
        }

        public string GetLotteryCode()
        {
            _logger.LogInformation("Get LotteryCode");
            LotteryResult lr = resultRepo.GetLastRow();
            if (lr != null)
            {
                var nextDate = GetNextLotteryDate(lr.LotteryDate);
                if (nextDate.Year == lr.LotteryDate.Year)
                {
                    return (int.Parse(lr.Id) + 1).ToString();
                }
                else
                {
                    return nextDate.ToString("yy")+"001";
                }
            }
            else
            {
                return "errorNo";
            }
        }

        DateTime GetNextLotteryDate(DateTime lotteryDate)
        {
            DateTime nextDate = lotteryDate;
            while (true)
            {
                nextDate = nextDate.AddDays(1);
                if (nextDate.DayOfWeek == DayOfWeek.Sunday || nextDate.DayOfWeek == DayOfWeek.Tuesday || nextDate.DayOfWeek == DayOfWeek.Friday)
                {
                    break;
                }
            }
            return nextDate;
        }

        public void SetLotteryPredictionTotal()
        {
            var reusltList = resultRepo.GetAll();
            foreach (LotteryResult lr in reusltList)
            {
                resultRepo.SetLotteryPredictionTotal(lr.Id);
            }
        }

        public void InsertLotteryPredictionTotal_T(string lotteryCode)
        {
            resultRepo.InsertLotteryPredictionCode_4_Temp(lotteryCode);
            resultRepo.InsertLotteryPredictionTotal_T(lotteryCode);
        }
    }
}
