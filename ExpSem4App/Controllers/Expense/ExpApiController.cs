using ExpSem4App.Models;
using ExpSem4App.Repo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace ExpSem4App.Controllers.Expense
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpApiController : ControllerBase
    {
        private readonly IExpenses _repo;
        public ExpApiController(IExpenses repo)
        {
            _repo = repo;
        }
        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            if (user == null) return BadRequest("Invalid user data");

            var addedUser = _repo.AddUser(user);
            return Ok(addedUser);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User login)
        {
            if (login == null || string.IsNullOrEmpty(login.Username) || string.IsNullOrEmpty(login.PasswordHash))
            {
                return BadRequest("Username and password are required.");
            }

            var user = _repo.LoginUser(login.Username, login.PasswordHash);

            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            return Ok(user);
        }

        [HttpGet("GetAllCat")]
        public IActionResult GetCategory()
        {
            var category = _repo.GetAllCategories();
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        [HttpPost("AddTran")]
        public IActionResult AddTransaction([FromBody] Transaction transaction)
        {
            if (transaction == null)
            {
                return BadRequest("Invalid transaction data");
            }
            var addedTransaction = _repo.AddTransaction(transaction);
            return Ok(addedTransaction);
        }

        [HttpGet("GetDashboard")]
        public IActionResult GetDashboard(int userId)
        {
            var dashboard = _repo.GetDashboard(userId);
            if (dashboard == null)
            {
                return NotFound();
            }
            return Ok(dashboard);
        }

        [HttpGet("GetReport")]
        public IActionResult GetReport(int userId, int month, int year)
        {
            var report = _repo.GetMonthlyReport(userId,month,year);
            if (report == null)
            {
                return NotFound();
            }
            return Ok(report);
        }

    }
}
