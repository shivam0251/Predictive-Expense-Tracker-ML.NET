namespace ExpSem4App.Models
{
    public class MonthlyTransactionReport
    {
        public string Username { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; } // Income / Expense
        public string Category { get; set; }
        public string Icon { get; set; } // Emoji
        public decimal Amount { get; set; }
    }

}
