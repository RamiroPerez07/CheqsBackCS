using System.ComponentModel.DataAnnotations.Schema;

namespace CheqsApp.Models
{
    public class Business
    {
        public int Id { get; set; }

        public string BusinessName { get; set; } = string.Empty;

        // Usuario creador
        public int UserId { get; set; }

        public User? User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? LastUpdatedAt { get; set;} = DateTime.Now;

        // Relación con bank Business. Bancos con los que trabaja la empresa

        public ICollection<BankBusiness> BankBusinesses { get; set; } = new List<BankBusiness>();
    }
}
