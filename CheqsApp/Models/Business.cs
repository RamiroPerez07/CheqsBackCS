using System.ComponentModel.DataAnnotations.Schema;

namespace CheqsApp.Models
{
    public class Business
    {
        public int Id { get; set; }

        public string BusinessName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }

        public int UserId { get; set; }

        public User? User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? LastUpdatedAt { get; set;} = DateTime.Now;

        // Relación con BusinessUser (usuarios que ven la empresa)
        public ICollection<BusinessUser> BusinessUsers { get; set; } = new List<BusinessUser>();
    }
}
