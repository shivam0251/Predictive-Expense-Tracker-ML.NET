// Models/PredictionModels/PredictionResult.cs
using System.Collections.Generic;
using Microsoft.ML.Data;

namespace ExpSem4App.Models.PredictionModels
{
    public class PredictionResult
    {
        public float PredictedAmount { get; set; }
        public string? PredictionDate { get; set; }
        public List<HistoricalData> HistoricalData { get; set; } = new List<HistoricalData>();
        public Dictionary<string, float> CategoryBreakdown { get; set; } = new Dictionary<string, float>();

        [ColumnName("ConfidenceLowerBound")]
        public float ConfidenceLower { get; set; }

        [ColumnName("ConfidenceUpperBound")]
        public float ConfidenceUpper { get; set; }

        public string? ConfidenceInterval { get; set; }
        public int AnomalyCount { get; set; }
        public string? Error { get; set; }
    }

    public class ExpensePredictionResult
    {
        public float PredictedAmount { get; set; }
    }


}