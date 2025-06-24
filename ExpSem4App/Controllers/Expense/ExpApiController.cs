using ExpSem4App.Models;
using ExpSem4App.Repo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ExpSem4App.Controllers.Expense
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpApiController : ControllerBase
    {

        private readonly string _connectionString;

        
        private readonly IExpenses _repo;
        public ExpApiController(IExpenses repo, IConfiguration configuration)
        {
            _repo = repo;
            _connectionString = configuration.GetConnectionString("dfcs");
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

        [HttpGet("ShowTransaction")]
        public IActionResult ShowTransaction(int userId)
        {
            List<Transaction> transactions = new List<Transaction>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
            SELECT t.TransactionId,
                   t.CategoryId,
                   c.Title,
                   t.Amount,
                   t.Note,
                   t.Date,
                   t.UserId
            FROM tbl_Transactions t
            INNER JOIN tbl_Categories c ON c.CategoryId = t.CategoryId
            WHERE t.UserId = @UserId
            ORDER BY t.Date DESC;";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            transactions.Add(new Transaction
                            {
                                TransactionId = reader.GetInt32(0),
                                CategoryId = reader.GetInt32(1),
                                Category = new Category
                                {
                                    Title = reader.GetString(2) // Populates the Category's Title
                                },
                                Amount = reader.GetDecimal(3),
                                Note = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                Date = reader.GetDateTime(5),
                                UserId = reader.GetInt32(6),
                            });
                        }
                    }
                }
            }

            return Ok(transactions);
        }

        [HttpPost("UpdateTransaction")]
        public IActionResult UpdateTransaction([FromBody] Transaction transaction)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"
            UPDATE tbl_Transactions
            SET CategoryId = @CategoryId,
                Amount = @Amount,
                Note = @Note,
                Date = @Date
            WHERE TransactionId = @TransactionId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CategoryId", transaction.CategoryId);
                    cmd.Parameters.AddWithValue("@Amount", transaction.Amount);
                    cmd.Parameters.AddWithValue("@Note", (object)transaction.Note ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Date", transaction.Date);
                    cmd.Parameters.AddWithValue("@TransactionId", transaction.TransactionId);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                        return Ok(new { message = "Transaction updated successfully" });
                    else
                        return NotFound(new { message = "Transaction not found" });
                }
            }
        }

        [HttpDelete("DeleteTransaction/{transactionId}")]
        public IActionResult DeleteTransaction(int transactionId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "DELETE FROM tbl_Transactions WHERE TransactionId = @TransactionId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TransactionId", transactionId);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                        return Ok(new { message = "Transaction deleted successfully" });
                    else
                        return NotFound(new { message = "Transaction not found" });
                }
            }
        }


    }
}
