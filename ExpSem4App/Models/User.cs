using System.ComponentModel.DataAnnotations.Schema;

namespace ExpSem4App.Models
{
    [Table("tbl_User")]
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string? Email { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
