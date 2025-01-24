using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CheqsApp.DTO
{
    public class BankBusinessBalanceDTO
    {
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int UserId { get; set; }

        public string Username { get; set; } = string.Empty;
    }
}
