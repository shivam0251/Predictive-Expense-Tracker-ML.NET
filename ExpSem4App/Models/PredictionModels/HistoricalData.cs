// Models/PredictionModels/HistoricalData.cs
using Microsoft.ML.Data;

namespace ExpSem4App.Models.PredictionModels
{
    public class HistoricalData
    {
        [LoadColumn(0)]
        public float Month { get; set; }

        [LoadColumn(1)]
        public float Year { get; set; }

        [LoadColumn(2)]
        public float TotalExpense { get; set; }

        [LoadColumn(3)]
        public float AverageExpense { get; set; }

        [LoadColumn(4)]
        public float NextMonthExpense { get; set; }

        // Anomaly detection
        [ColumnName("IsAnomaly")]
        public bool IsAnomaly { get; set; }

        [ColumnName("AnomalyScore")]
        public float AnomalyScore { get; set; }

        [ColumnName("AnomalyExplanation")]
        public string AnomalyExplanation { get; set; }
    }
}