using Microsoft.AspNetCore.Mvc;

namespace ExpSem4App.Controllers.Expense
{
    public class ExpController : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Transaction()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Category()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Report()
        {
            return View();
        }
    }

}

