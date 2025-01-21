using System.ComponentModel.DataAnnotations;

namespace CheqsApp.DTO
{
    public class BanksDTO
    {
        [Required]
        public int BankId {  get; set; }

        [Required]
        public string BankName { get; set; } = string.Empty;
    }
}
