using System.ComponentModel.DataAnnotations;

namespace CheqsApp.DTO
{
    public class BusinessesByUserDTO
    {
        [Required]
        public int BusinessId { set; get; }

        [Required]
        public string BusinessName { set; get; } = string.Empty;
    }
}
