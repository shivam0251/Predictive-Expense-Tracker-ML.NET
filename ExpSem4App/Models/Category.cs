using System.ComponentModel.DataAnnotations.Schema;

namespace ExpSem4App.Models
{
    [Table("tbl_Categories")]
    public class Category
    {
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Type { get; set; } // "Income" or "Expense"
        public int UserId { get; set; }
    }
}
