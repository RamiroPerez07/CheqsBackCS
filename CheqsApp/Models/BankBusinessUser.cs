using System.ComponentModel.DataAnnotations;

namespace CheqsApp.Models
{
    public class BankBusinessUser
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public required User User { get; set; }

        public int BankBusinessId {  get; set; }
        
        public required BankBusiness BankBusiness { get; set; }
    }
}
