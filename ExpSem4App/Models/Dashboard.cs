using System.ComponentModel.DataAnnotations.Schema;

namespace ExpSem4App.Models
{
    public class Dashboard
    {
        public decimal ThisMonthIncome { get; set; }
        public decimal LastMonthIncome { get; set; }
        public decimal ThisMonthExpense { get; set; }
        public decimal LastMonthExpense { get; set; }
        public decimal ThisMonthSavings { get; set; }
        public decimal IncomeChangePercent { get; set; }
        public decimal ExpenseChangePercent { get; set; }
        public decimal SavingPercentOfIncome { get; set; }

        public List<RecentTransaction> RecentTransactions { get; set; }
        public List<ExpenseTrend> ExpenseTrends { get; set; }
    }

    public class RecentTransaction
    {
        public string Category { get; set; }
        public string Note { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
    }

    public class ExpenseTrend
    {
        public string Month { get; set; }
        public decimal TotalExpense { get; set; }
    }
}
