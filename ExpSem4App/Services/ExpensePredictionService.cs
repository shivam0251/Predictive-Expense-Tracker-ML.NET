using ExpSem4App.Data;
using ExpSem4App.Models;
using ExpSem4App.Models.PredictionModels;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.TimeSeries;
using System;
using System.Collections.Generic;
using System.Linq;

public class ExpensePredictionService
{
    private readonly ApplicationDbContext _context;
    private readonly MLContext _mlContext;

    public ExpensePredictionService(ApplicationDbContext context)
    {
        _context = context;
        _mlContext = new MLContext();
    }

    public PredictionResult PredictExpenses(int userId)
    {
        var historicalData = GetHistoricalData(userId);

        int windowSize = 2;  // 6 months window
        int seriesLength = historicalData.Count;

        if (seriesLength < 2 * windowSize + 1)
        {
            return new PredictionResult
            {
                Error = $"Not enough data to predict. Need at least {2 * windowSize + 1} months of data, but only found {seriesLength}."
            };
        }

        // Prepare data for ML.NET
        var data = historicalData.Select(h => new ModelInput { TotalExpense = h.TotalExpense }).ToList();

        var dataView = _mlContext.Data.LoadFromEnumerable(data);

        // Configure SSA forecasting options
        var forecastingPipeline = _mlContext.Forecasting.ForecastBySsa(
            outputColumnName: nameof(ForecastResult.ForecastedExpenses),
            inputColumnName: nameof(ModelInput.TotalExpense),
            windowSize: windowSize,
            seriesLength: seriesLength,
            trainSize: seriesLength,
            horizon: 1,            // predict next 1 month
            confidenceLevel: 0.95f,
            confidenceLowerBoundColumn: nameof(ForecastResult.ConfidenceLowerBound),
            confidenceUpperBoundColumn: nameof(ForecastResult.ConfidenceUpperBound)
        );

        // Train the model
        var model = forecastingPipeline.Fit(dataView);

        // Create forecasting engine
        var forecastingEngine = model.CreateTimeSeriesEngine<ModelInput, ForecastResult>(_mlContext);

        // Predict next month
        var forecast = forecastingEngine.Predict();

        // Prepare result
        var predictedAmount = forecast.ForecastedExpenses[0];
        var confLow = forecast.ConfidenceLowerBound[0];
        var confHigh = forecast.ConfidenceUpperBound[0];

        // Compose PredictionResult
        var result = new PredictionResult
        {
            PredictedAmount = predictedAmount,
            ConfidenceLower = confLow,
            ConfidenceUpper = confHigh,
            PredictionDate = DateTime.Now.AddMonths(1).ToString("yyyy-MM"),
            HistoricalData = historicalData,
            CategoryBreakdown = new Dictionary<string, float>(),  // you can fill this from your categories
            Error = null
        };

        return result;
    }

    public List<HistoricalData> GetHistoricalData(int userId)
    {
        // Monthly total expenses grouped by year and month
        var expenses = (from t in _context.Transactions
                        join c in _context.Categories on t.CategoryId equals c.CategoryId
                        where t.UserId == userId && c.Type == "Expense"
                        group t by new { t.Date.Year, t.Date.Month } into g
                        select new HistoricalData
                        {
                            Year = g.Key.Year,
                            Month = g.Key.Month,
                            TotalExpense = (float)g.Sum(x => x.Amount),
                            AverageExpense = (float)g.Average(x => x.Amount),
                            NextMonthExpense = 0
                        })
                 .OrderBy(h => h.Year)
                 .ThenBy(h => h.Month)
                 .ToList();



        return expenses;
    }

    // Helper class for ML.NET input schema
    private class ModelInput
    {
        public float TotalExpense { get; set; }
    }

    // Forecast result class for ML.NET output schema
    private class ForecastResult
    {
        [VectorType(1)]
        public float[] ForecastedExpenses { get; set; }

        [VectorType(1)]
        public float[] ConfidenceLowerBound { get; set; }

        [VectorType(1)]
        public float[] ConfidenceUpperBound { get; set; }
    }
}
