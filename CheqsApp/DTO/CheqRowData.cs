using System.ComponentModel.DataAnnotations.Schema;

namespace CheqsApp.DTO
{
    public class CheqDetail
    {
        public int CheqId { get; set; }

        public string CheqNumber {  get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime DueDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public int BusinessId { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public decimal Balance { get; set; }

        public int TypeId { get; set; }
        public string TypeName { get; set; } = string.Empty;

        public int StateId { get; set; }
        public string StateName { get; set; } = string.Empty;

        public int EntityId { get; set; }
        public string EntityName { get; set; } = string.Empty;
    }
}
