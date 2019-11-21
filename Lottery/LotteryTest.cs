using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Legacy;
using Microsoft.ML.Legacy.Data;
using Microsoft.ML.Legacy.Models;
using Microsoft.ML.Legacy.Trainers;
using Microsoft.ML.Legacy.Transforms;
using Microsoft.ML.Runtime;
using Microsoft.ML.Runtime.Api;

namespace Lottery
{
    public class LotteryTest
    {
        public class myLottery
        {
            [Column(ordinal: "0", name: "pre10")]
            public float pre10;
            [Column(ordinal: "1", name: "pre9")]
            public float pre9;
            [Column(ordinal: "2", name: "pre8")]
            public float pre8;
            [Column(ordinal: "3", name: "pre7")]
            public float pre7;
            [Column(ordinal: "4", name: "pre6")]
            public float pre6;
            [Column(ordinal: "5", name: "pre5")]
            public float pre5;
            [Column(ordinal: "6", name: "pre4")]
            public float pre4;
            [Column(ordinal: "7", name: "pre3")]
            public float pre3;
            [Column(ordinal: "8", name: "pre2")]
            public float pre2;
            [Column(ordinal: "9", name: "pre1")]
            public float pre1;
            //[Column(ordinal: "10", name: "No")]
            //public float no;
            [Column(ordinal: "10")]
            public string Income;
        }
        public class myPrediction
        {
            [ColumnName("Score")]
            public float Income;
        }

        public static void GetMyPrediction()
        {
            Console.WriteLine("Begin ML.NET demo run");
            Console.WriteLine("Income from age, sex, politics");
            var pipeline = new LearningPipeline();
            string dataPath = AppDomain.CurrentDomain.BaseDirectory + "/PeopleData.txt";
            pipeline.Add(new TextLoader(dataPath).
              CreateFrom<myLottery>(separator: ' '));
            pipeline.Add(new ColumnCopier(("Income", "Label")));
            //pipeline.Add(new CategoricalOneHotVectorizer("Politic"));
            pipeline.Add(new ColumnConcatenator("Features", "pre10",
              "pre9", "pre8", "pre7", "pre6", "pre5", "pre4", "pre3"
              , "pre2", "pre1"));
            var sdcar = new StochasticDualCoordinateAscentRegressor();
            sdcar.MaxIterations = 1000;
            sdcar.NormalizeFeatures = NormalizeOption.Auto;
            pipeline.Add(sdcar);
            // pipeline.N
            Console.WriteLine("\nStarting training \n");
            var model = pipeline.Train<myLottery, myPrediction>();
            Console.WriteLine("\nTraining complete \n");
            string modelPath = AppDomain.CurrentDomain.BaseDirectory + "/IncomeModel.zip";
            Task.Run(async () =>
            {
                await model.WriteAsync(modelPath);
            }).GetAwaiter().GetResult();
            var testData = new TextLoader(dataPath).
              CreateFrom<myLottery>(separator: ' ');
            var evaluator = new RegressionEvaluator();
            var metrics = evaluator.Evaluate(model, testData);
            double rms = metrics.Rms;
            Console.WriteLine("Root mean squared error = " +
              rms.ToString("F4"));
            Console.WriteLine("Income age 40 conservative male: ");
            myLottery newPatient = new myLottery()
            {
                pre10 = 6824298f,
                pre9 = 2589916f,
                pre8 = 2602089f,
                pre7 = 2915497f,
                pre6 = 8507838f,
                pre5 = 7679324f,
                pre4 = 607461f,
                pre3 = 5806877,
                pre2 = 6776442f,
                pre1 = 9975203
            };
            myPrediction prediction = model.Predict(newPatient);
            float predIncome = prediction.Income;
            Console.WriteLine("Predicted income = $" +
              predIncome.ToString("F2"));
            Console.WriteLine("\nEnd ML.NET demo");
            Console.ReadLine();
        }
    }
}
