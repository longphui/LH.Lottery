using FileHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lottery.ML.Domain.Model
{
    /// <summary>
    /// 预测初始结果
    /// </summary>
    [DelimitedRecord(",")]
    public class LotteryPredictionResult
    {
        /// <summary>
        /// 预测类型
        /// </summary>
        public string PredictionType { get; set; }

        /// <summary>
        /// 彩票期号
        /// </summary>
        public string LotteryCode { get; set; }

        /// <summary>
        /// 预测的位置
        /// </summary>
        public string PredictionSite { get; set; }


        /// <summary>
        /// 预测结果
        /// </summary>
        public string PredictionResult { get; set; }
    }
}
