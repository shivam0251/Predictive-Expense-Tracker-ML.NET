using System.ComponentModel.DataAnnotations.Schema;

namespace ExpSem4App.Models
{
    [Table("tbl_Transactions")]
    public class Transaction
    {
        public int TransactionId { get; set; }
        public int CategoryId { get; set; }
        public decimal Amount { get; set; }
        public string Note { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }

        public Category? Category { get; set; } = null!;
    }
}
