using System;
using System.Collections.Generic;
using System.Text;

namespace Lottery.ML.Domain.Model
{
    /// <summary>
    /// 预测日志
    /// </summary>
    public class LotteryPredictionLog
    {
        public LotteryPredictionLog() {
            this.IsSuccess = false;
        }
        /// <summary>
        /// 彩票期号
        /// </summary>
        public string LotteryCode { get; set; }

        /// <summary>
        /// 0:表示开始，1:表示结束，没有数据表示没有预测
        /// </summary>
        public int PredictionStatus { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrMsg { get; set; }

        public void End(bool isSuccess,string errMsg)
        {
            this.EndTime = DateTime.Now;
            this.IsSuccess = isSuccess;
            this.ErrMsg = errMsg;
        }
    }
}
