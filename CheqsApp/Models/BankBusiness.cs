using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CheqsApp.Models
{
    public class BankBusiness
    {
        [Key]
        public int Id { get; set; }

        public int BankId { get; set; }

        public required Bank Bank { get; set; }

        public int BusinessId {get; set; }
        public required Business Business { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

        //Actualizado por
        public int UserId { get; set; }

        public required User User { get; set; }

        public ICollection<BankBusinessUser> BankBusinessUsers { get; set; } = new List<BankBusinessUser>();
    }
}
