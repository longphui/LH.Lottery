using Microsoft.ML.Runtime.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lottery.ML.Domain.Model
{
    public class LotteryPrediction
    {
        [ColumnName("PredictedLabel")]
        public string PredictedLabels;
    }
}
