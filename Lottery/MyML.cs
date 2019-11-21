using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ML.Legacy;
using Microsoft.ML.Legacy.Data;
using Microsoft.ML.Legacy.Models;
using Microsoft.ML.Legacy.Trainers;
using Microsoft.ML.Legacy.Transforms;
using Microsoft.ML.Runtime.Api;

namespace Lottery
{
    public class MyML
    {
        public class myData
        {
            [Column(ordinal: "0", name: "XCoord")]
            public float x;
            [Column(ordinal: "1", name: "YCoord")]
            public float y;
            [Column(ordinal: "2", name: "ZCoord")]
            public float z;
            [Column(ordinal: "3", name: "Label")]
            public string Label;
        }
        public class myPrediction
        {
            [ColumnName("PredictedLabel")]
            public string PredictedLabels;
        }

        public static void GetMyPrediction()
        {
            var pipeline = new LearningPipeline();
            string dataPath = AppDomain.CurrentDomain.BaseDirectory + "/datamodel/myMLData.txt";
            pipeline.Add(new TextLoader(dataPath).CreateFrom<myData>(separator: ' '));
            pipeline.Add(new Dictionarizer("Label"));
            pipeline.Add(new ColumnConcatenator("Features", "XCoord", "YCoord", "ZCoord"));
            pipeline.Add(new LogisticRegressionBinaryClassifier());
            pipeline.Add(new PredictedLabelColumnOriginalValueConverter()
            {
                PredictedLabelColumn = "PredictedLabel"
            });
            Console.WriteLine("\nStarting training\n");
            var model = pipeline.Train<myData, myPrediction>();
            var testData = new TextLoader(dataPath).CreateFrom<myData>(separator: ' ');
            var evaluator = new BinaryClassificationEvaluator();
            var metrics = evaluator.Evaluate(model, testData);
            double acc = metrics.Accuracy * 100;
            Console.WriteLine("Model accuracy = " + acc.ToString("F2") + "%");
            myData newPoint = new myData()
            {
                x = 9,
                y = 8,
                z = 10
            };
            myPrediction prediction = model.Predict(newPoint);
            string result = prediction.PredictedLabels;
            Console.WriteLine("Prediction = " + result);
            Console.WriteLine("\nEnd ML.NET demo");
            Console.ReadLine();
        }
    }
}
