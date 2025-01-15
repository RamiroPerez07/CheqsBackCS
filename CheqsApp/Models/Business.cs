using System.ComponentModel.DataAnnotations.Schema;

namespace CheqsApp.Models
{
    public class Business
    {
        public int Id { get; set; }

        public string BusinessName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }
    }
}
