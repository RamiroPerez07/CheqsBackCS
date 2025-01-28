using CheqsApp.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace CheqsApp.DTO
{
    public class CreateCheqDTO
    {
        public int Id { get; set; }
        public string CheqNumber { get; set; } = string.Empty;
        public int EntityId { get; set; }
        public int TypeId { get; set; }
        public int StateId { get; set; }
        public DateTime IssueDate { get; set; } = DateTime.Now;
        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int BankBusinessUserId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
    }
}
