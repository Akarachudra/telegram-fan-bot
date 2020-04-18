using System;
using System.IO;
using Microsoft.ML;

namespace Moshna.Bot
{
    public class SentimentService : ISentimentService
    {
        private readonly string filePath;
        private readonly MLContext mlContext;
        private PredictionEngine<SentimentData, SentimentPrediction> predictionFunction;

        public SentimentService(string fileName)
        {
            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            mlContext = new MLContext();
            LoadDataAndTrainModel();
        }

        public void AddToData(string text, bool isMoshna)
        {
            File.AppendAllText(filePath, $"{text}\t{Convert.ToInt32(isMoshna)}\n");
            LoadDataAndTrainModel();
        }

        public bool IsMoshna(string text)
        {
            return predictionFunction.Predict(new SentimentData { SentimentText = text }).Prediction;
        }

        private void LoadDataAndTrainModel()
        {
            var dataView = mlContext.Data.LoadFromTextFile<SentimentData>(filePath);
            var estimator = mlContext.Transforms.Text.FeaturizeText("Features", nameof(SentimentData.SentimentText))
                                .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression());
            var model = estimator.Fit(dataView);
            predictionFunction = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
        }
    }
}