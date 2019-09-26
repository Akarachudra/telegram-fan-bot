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
            this.filePath = Path.Combine(Environment.CurrentDirectory, fileName);
            this.mlContext = new MLContext();
            this.LoadDataAndTrainModel();
        }

        public void AddToData(string text, bool isMoshna)
        {
            File.AppendAllText(this.filePath, $"{text}\t{Convert.ToInt32(isMoshna)}\n");
            this.LoadDataAndTrainModel();
        }

        public bool IsMoshna(string text)
        {
            return this.predictionFunction.Predict(new SentimentData { SentimentText = text }).Prediction;
        }

        private void LoadDataAndTrainModel()
        {
            var dataView = this.mlContext.Data.LoadFromTextFile<SentimentData>(this.filePath);
            var estimator = this.mlContext.Transforms.Text.FeaturizeText("Features", nameof(SentimentData.SentimentText))
                                .Append(this.mlContext.BinaryClassification.Trainers.SdcaLogisticRegression());
            var model = estimator.Fit(dataView);
            this.predictionFunction = this.mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
        }
    }
}