using ExpSem4App.Models;
using ExpSem4App.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class PredictionApiController : ControllerBase
{
    private readonly ExpensePredictionService _predictionService;

    public PredictionApiController(ExpensePredictionService predictionService)
    {
        _predictionService = predictionService;
    }

    [HttpGet("NxtMonth")]
    public async Task<IActionResult> NxtMonth([FromQuery] int userId)
    {
        if (userId <= 0)
        {
            return BadRequest(new { error = "Invalid userId" });
        }

        var result = await Task.Run(() => _predictionService.PredictExpenses(userId));
        if (!string.IsNullOrEmpty(result.Error))
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new
        {
            predictedAmount = result.PredictedAmount,
            predictionDate = result.PredictionDate,
            confidenceLower = result.ConfidenceLower,
            confidenceUpper = result.ConfidenceUpper,
            historicalData = result.HistoricalData,
            categoryBreakdown = result.CategoryBreakdown,
            error = result.Error
        });
    }


}