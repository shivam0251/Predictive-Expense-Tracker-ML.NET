using ExpSem4App.Models.PredictionModels;
using Microsoft.AspNetCore.Mvc;

namespace ExpSem4App.Controllers
{
    public class PredictionController : Controller
    {
        public IActionResult NxtMonth()
        {
            var predictionResult = new PredictionResult(); // Ensure this is initialized properly
            return View(predictionResult);
        }

    }
}
