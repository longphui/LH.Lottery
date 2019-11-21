using System;
using System.Collections.Generic;
using System.Text;

namespace Lottery.ML.Domain.Model
{
    public class LotteryResult
    {
        public LotteryResult() { }
        public LotteryResult(string num, string result, string resulttime)
        {
            if (string.IsNullOrEmpty(num))
            {
                throw new Exception("期号不能为空");
            }
            if (string.IsNullOrEmpty(result))
            {
                throw new Exception("开奖结果不能为空");
            }
            if (string.IsNullOrEmpty(resulttime))
            {
                throw new Exception("开奖日期不能为空");
            }
            DateTime dt = DateTime.Now;
            if (!DateTime.TryParse(resulttime, out dt))
            {
                throw new Exception("开奖日期格式错误");
            }
            IList<string> noList = result.Split(',');
            if (noList.Count != 7)
            {
                throw new Exception("开奖结果错误");
            }
            this.Id = num;
            this.LotteryDate = dt;
            this.No1 = noList[0];
            this.No2 = noList[1];
            this.No3 = noList[2];
            this.No4 = noList[3];
            this.No5 = noList[4];
            this.No6 = noList[5];
            this.No7 = noList[6];
            this.LotteryNo= string.Join("", noList);
            this.NoSum = int.Parse(this.No1) + int.Parse(this.No2) + int.Parse(this.No3)
                + int.Parse(this.No4) + int.Parse(this.No5) + int.Parse(this.No6) + int.Parse(this.No7);
        }
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string LotteryNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string No1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string No2 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string No3 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string No4 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string No5 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string No6 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string No7 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int NoSum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime LotteryDate { get; set; }
    }
}
