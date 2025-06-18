
using Microsoft.ML.Data;

public class ForecastResult
{
    [VectorType(1)]
    public float[] ForecastedExpenses { get; set; }

    [VectorType(1)]
    public float[] ConfidenceLowerBound { get; set; }

    [VectorType(1)]
    public float[] ConfidenceUpperBound { get; set; }
}
