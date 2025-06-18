    using ExpSem4App.Models;

    namespace ExpSem4App.Repo
    {
        public interface IExpenses
        {
            public User AddUser(User user);
            public User LoginUser(string username, string password);    
            public Transaction AddTransaction(Transaction transaction);
            public List<Category> GetAllCategories();

            public Dashboard GetDashboard(int userId);

            public List<MonthlyTransactionReport> GetMonthlyReport(int userId, int month, int year);


        }
    }
