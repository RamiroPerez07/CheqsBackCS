using System.ComponentModel.DataAnnotations;

namespace CheqsApp.Models
{
    public class BankBusinessUser
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public User? User { get; set; }

        public int BankBusinessId {  get; set; }
        
        public BankBusiness? BankBusiness { get; set; }
    }
}
