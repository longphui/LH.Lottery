using FileHelpers;
using Lottery.Common;
using Lottery.ML.Domain.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.ML.Domain.Service
{
    public class TrainingGroundService: ITrainingGroundService
    {
        private readonly ILogger _logger;
        ILotteryResultRepository resultRepo;
        public TrainingGroundService(IEnumerable<ILotteryResultRepository> resultRepos,
            ILogger<TrainingGroundService> logger,
            IConfiguration config)
        {
            this._logger = logger;
            this.resultRepo = resultRepos.FirstOrDefault(x => x.DBType == config["DBType"]);
        }

        public void BuildTrainingGround(string rootPath)
        {
            var noSite = new List<string> { "first", "second", "third", "fourth", "fifth", "sixth", "seventh" };
            //var noType = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "Size", "Parity" };
            var noType = new List<string> {"Size", "Parity", "Prime" };
            Dictionary<string, StringBuilder> dicTrainingData = new Dictionary<string, StringBuilder>();
            int i = 0;
            for (i = 0; i < noSite.Count; i++)
            {
                dicTrainingData.Add(noSite[i], new StringBuilder());
            }
            Dictionary<string, IList<FileLineData>> dicFileLineData = new Dictionary<string, IList<FileLineData>>();
            var lotteryResultList = resultRepo.GetAll();
            if (lotteryResultList.Count >= 100)
            {
                for (i = 99; i < lotteryResultList.Count; i++)
                {
                    int[] intNos = { int.Parse(lotteryResultList[i].No1),
                    int.Parse(lotteryResultList[i].No2),
                    int.Parse(lotteryResultList[i].No3),
                    int.Parse(lotteryResultList[i].No4),
                    int.Parse(lotteryResultList[i].No5),
                    int.Parse(lotteryResultList[i].No6),
                    int.Parse(lotteryResultList[i].No7)};
                    long dateStamp = DateTimeUtility.ConvertToTimeStamp(lotteryResultList[i].LotteryDate) / 100000;
                    if (i == 99)
                    {
                        for (int j = i; j > 0; j--)
                        {
                            dicTrainingData["first"].Append(lotteryResultList[i - j].No1);
                            dicTrainingData["first"].Append(",");
                            dicTrainingData["second"].Append(lotteryResultList[i - j].No2);
                            dicTrainingData["second"].Append(",");
                            dicTrainingData["third"].Append(lotteryResultList[i - j].No3);
                            dicTrainingData["third"].Append(",");
                            dicTrainingData["fourth"].Append(lotteryResultList[i - j].No4);
                            dicTrainingData["fourth"].Append(",");
                            dicTrainingData["fifth"].Append(lotteryResultList[i - j].No5);
                            dicTrainingData["fifth"].Append(",");
                            dicTrainingData["sixth"].Append(lotteryResultList[i - j].No6);
                            dicTrainingData["sixth"].Append(",");
                            dicTrainingData["seventh"].Append(lotteryResultList[i - j].No7);
                            dicTrainingData["seventh"].Append(",");
                        }
                    }
                    else
                    {
                        for (int l = 0; l < noSite.Count; l++)
                        {
                            string strData = dicTrainingData[noSite[l]].ToString();
                            int index = strData.IndexOf(',');
                            strData = strData.Substring(index + 1);
                            index = strData.LastIndexOf(',');
                            strData = strData.Substring(0,index + 1);
                            dicTrainingData[noSite[l]] = new StringBuilder(strData);
                            //dataList.RemoveAt(0);
                            //dataList.RemoveAt(dataList.Count - 1);
                            //dataList.RemoveAt(dataList.Count - 1);
                            //dicTrainingData[noSite[l]] = new StringBuilder(string.Join(",", dataList));
                        }
                        dicTrainingData["first"].Append(lotteryResultList[i - 1].No1);
                        dicTrainingData["first"].Append(",");
                        dicTrainingData["second"].Append(lotteryResultList[i - 1].No2);
                        dicTrainingData["second"].Append(",");
                        dicTrainingData["third"].Append(lotteryResultList[i - 1].No3);
                        dicTrainingData["third"].Append(",");
                        dicTrainingData["fourth"].Append(lotteryResultList[i - 1].No4);
                        dicTrainingData["fourth"].Append(",");
                        dicTrainingData["fifth"].Append(lotteryResultList[i - 1].No5);
                        dicTrainingData["fifth"].Append(",");
                        dicTrainingData["sixth"].Append(lotteryResultList[i - 1].No6);
                        dicTrainingData["sixth"].Append(",");
                        dicTrainingData["seventh"].Append(lotteryResultList[i - 1].No7);
                        dicTrainingData["seventh"].Append(",");
                    } 
                    dicTrainingData["first"].Append(dateStamp.ToString());
                    dicTrainingData["second"].Append(dateStamp.ToString());
                    dicTrainingData["third"].Append(dateStamp.ToString());
                    dicTrainingData["fourth"].Append(dateStamp.ToString());
                    dicTrainingData["fifth"].Append(dateStamp.ToString());
                    dicTrainingData["sixth"].Append(dateStamp.ToString());
                    dicTrainingData["seventh"].Append(dateStamp.ToString());
                    for (int n = 0; n < noSite.Count; n++)
                    {
                        for (int m = 0; m < noType.Count; m++)
                        {
                            string keyStr = noSite[n] + noType[m];
                            if (!dicFileLineData.ContainsKey(keyStr))
                            {
                                dicFileLineData.Add(keyStr, new List<FileLineData>());
                            }
                            string label = "";
                            if (noType[m] == "Size")
                            {
                                if (intNos[n] < 5)
                                {
                                    label = ",D";
                                }
                                else
                                {
                                    label = ",X";
                                }
                            }
                            else if (noType[m] == "Parity")
                            {
                                if (intNos[n] % 2 == 0)
                                {
                                    label = ",O";
                                }
                                else
                                {
                                    label = ",J";
                                }
                            }
                            else if (noType[m] == "Prime")
                            {
                                if (intNos[n] == 1 || intNos[n] == 2 || intNos[n] == 3 || intNos[n] == 5 || intNos[n] == 7)
                                {
                                    label = ",Z";
                                }
                                else
                                {
                                    label = ",H";
                                }
                            }
                            else
                            {
                                if (intNos[n].ToString() == noType[m])
                                {
                                    label = ",Y";
                                }
                                else
                                {
                                    label = ",N";
                                }
                            }
                            dicFileLineData[keyStr].Add(new FileLineData()
                            {
                                data = dicTrainingData[noSite[n]].ToString() + label
                            });
                        }
                    }
                }
                var engine = new FileHelperEngine<FileLineData>();
                foreach (KeyValuePair<string, IList<FileLineData>> kv in dicFileLineData)
                {
                    engine.WriteFile(rootPath+"/TrainingGround/" + kv.Key+".Txt", kv.Value);
                }
            }
        }
    }
}
