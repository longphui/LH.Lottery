using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Legacy;
using Microsoft.ML.Legacy.Data;
using Microsoft.ML.Legacy.Models;
using Microsoft.ML.Legacy.Trainers;
using Microsoft.ML.Legacy.Transforms;
using Microsoft.ML.Runtime.Api;

namespace Lottery
{
    public class IncomePredict
    {
        public class IncomeData
        {
            [Column("0")] public float Age;
            [Column("1")] public float Sex;
            [Column("2")] public float Income;
            [Column("3")] public string Politic;
        }
        public class IncomePrediction
        {
            [ColumnName("Score")]
            public float Income;
        }
        public static void GetMyPrediction()
        {
            Console.WriteLine("Begin ML.NET demo run");
            Console.WriteLine("Income from age, sex, politics");
            var pipeline = new LearningPipeline();
            string dataPath = AppDomain.CurrentDomain.BaseDirectory + "/datamodel/PeopleData.txt";
            pipeline.Add(new TextLoader(dataPath).
              CreateFrom<IncomeData>(separator: ','));
            pipeline.Add(new ColumnCopier(("Income", "Label")));
            pipeline.Add(new CategoricalOneHotVectorizer("Politic"));
            pipeline.Add(new ColumnConcatenator("Features", "Age",
              "Sex", "Politic"));
            var sdcar = new StochasticDualCoordinateAscentRegressor();
            sdcar.MaxIterations = 1000;
            sdcar.NormalizeFeatures = NormalizeOption.Auto;
            pipeline.Add(sdcar);
            // pipeline.N
            Console.WriteLine("\nStarting training \n");
            var model = pipeline.Train<IncomeData, IncomePrediction>();
            Console.WriteLine("\nTraining complete \n");
            string modelPath = AppDomain.CurrentDomain.BaseDirectory + "/IncomeModel.zip";
            Task.Run(async () =>
            {
                await model.WriteAsync(modelPath);
            }).GetAwaiter().GetResult();
            var testData = new TextLoader(dataPath).
              CreateFrom<IncomeData>(separator: ',');
            var evaluator = new RegressionEvaluator();
            var metrics = evaluator.Evaluate(model, testData);
            double rms = metrics.Rms;
            Console.WriteLine("Root mean squared error = " +
              rms.ToString("F4"));
            Console.WriteLine("Income age 40 conservative male: ");
            IncomeData newPatient = new IncomeData()
            {
                Age = 40.0f,
                Sex = -1f,
                Politic = "conservative"
            };
            IncomePrediction prediction = model.Predict(newPatient);
            float predIncome = prediction.Income * 10000;
            Console.WriteLine("Predicted income = $" +
              predIncome.ToString("F2"));
            Console.WriteLine("\nEnd ML.NET demo");
            Console.ReadLine();
        } // Main
    }
}
