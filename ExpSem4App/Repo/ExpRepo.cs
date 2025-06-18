using ExpSem4App.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace ExpSem4App.Repo
{
    public class ExpRepo : IExpenses
    {
        private readonly string _connectionString;

        public ExpRepo(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("dfcs");
        }

        public List<Category> GetAllCategories()
        {
            List<Category> categories = new List<Category>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM tbl_Categories";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    categories.Add(new Category
                    {
                        CategoryId = Convert.ToInt32(reader["CategoryId"]),
                        Title = reader["Title"].ToString(),
                        Icon = reader["Icon"].ToString(),
                        Type = reader["Type"].ToString(),
                        //UserId = Convert.ToInt32(reader["UserId"])
                    });
                }
            }

            return categories;
        }


        public Transaction AddTransaction(Transaction transaction)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
            INSERT INTO tbl_Transactions (CategoryId, Amount, Note, Date, UserId)
            VALUES (@CategoryId, @Amount, @Note, @Date, @UserId);
            SELECT CAST(scope_identity() AS int);
        ";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CategoryId", transaction.CategoryId);
                    cmd.Parameters.AddWithValue("@Amount", transaction.Amount);
                    cmd.Parameters.AddWithValue("@Note", (object)transaction.Note ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Date", transaction.Date);
                    cmd.Parameters.AddWithValue("@UserId", transaction.UserId);

                    conn.Open();
                    int insertedId = (int)cmd.ExecuteScalar();
                    transaction.TransactionId = insertedId;
                }
            }

            return transaction;
        }


        public User AddUser(User user)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO tbl_Users (Username, Email, PasswordHash) 
                                 OUTPUT INSERTED.UserId 
                                 VALUES (@Username, @Email, @PasswordHash)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", user.Username);
                cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash); // Hash in real apps
                cmd.Parameters.AddWithValue("@Email", user.Email);

                conn.Open();
                user.UserId = (int)cmd.ExecuteScalar();
            }

            return user;
        }

        public User LoginUser(string username, string password)
        {
            User user = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM tbl_Users WHERE Username = @Username AND PasswordHash = @Password";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password); // Consider hashing for real apps

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    user = new User
                    {
                        UserId = Convert.ToInt32(reader["UserId"]),
                        Username = reader["Username"].ToString(),
                        Email = reader["Email"].ToString(),
                        PasswordHash = reader["PasswordHash"].ToString(),
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"]).Date
                    };
                }
            }

            return user;
        }

        public Dashboard GetDashboard(int userId)
        {
            Dashboard dashboard = new Dashboard
            {
                RecentTransactions = new List<RecentTransaction>(),
                ExpenseTrends = new List<ExpenseTrend>()
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("usp_GetDashboardSummary", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Summary
                        if (reader.Read())
                        {
                            dashboard.ThisMonthIncome = reader.GetDecimal(0);
                            dashboard.LastMonthIncome = reader.GetDecimal(1);
                            dashboard.ThisMonthExpense = reader.GetDecimal(2);
                            dashboard.LastMonthExpense = reader.GetDecimal(3);
                            dashboard.ThisMonthSavings = reader.GetDecimal(4);
                            dashboard.IncomeChangePercent = reader.GetDecimal(5);
                            dashboard.ExpenseChangePercent = reader.GetDecimal(6);
                            dashboard.SavingPercentOfIncome = reader.GetDecimal(7);
                        }

                        // Move to 2nd result set (Recent Transactions)
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                dashboard.RecentTransactions.Add(new RecentTransaction
                                {
                                    Category = reader["Category"].ToString(),
                                    Note = reader["Note"].ToString(),
                                    Amount = Convert.ToDecimal(reader["Amount"]),
                                    Date = Convert.ToDateTime(reader["Date"]),
                                    Type = reader["Type"].ToString()
                                });
                            }
                        }

                        // Move to 3rd result set (Expense Trends)
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                dashboard.ExpenseTrends.Add(new ExpenseTrend
                                {
                                    Month = reader["Month"].ToString(),
                                    TotalExpense = Convert.ToDecimal(reader["TotalExpense"])
                                });
                            }
                        }
                    }
                }
            }

            return dashboard;
        }


        public List<MonthlyTransactionReport> GetMonthlyReport(int userId, int month, int year)
        {
            List<MonthlyTransactionReport> reports = new List<MonthlyTransactionReport>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("usp_GetMonthlyReport", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@Month", month);
                    cmd.Parameters.AddWithValue("@Year", year);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reports.Add(new MonthlyTransactionReport
                            {
                                Username = reader["Username"].ToString(),
                                Date = Convert.ToDateTime(reader["Date"]),
                                Type = reader["Type"].ToString(),
                                Category = reader["Category"].ToString(),
                                Icon = reader["Icon"].ToString(),
                                Amount = Convert.ToDecimal(reader["Amount"])
                            });
                        }
                    }
                }
            }

            return reports;
        }


    }
}
