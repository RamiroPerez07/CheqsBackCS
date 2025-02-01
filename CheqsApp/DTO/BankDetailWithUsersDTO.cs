using System.ComponentModel.DataAnnotations.Schema;

namespace CheqsApp.DTO
{
    public class BankDetailWithUsersDTO
    {
        public required BanksDTO Bank { get; set; }
        public List<UserSimpleDetailDTO> Users { get; set; } = new List<UserSimpleDetailDTO>();

        [Column(TypeName = "decimal(18,2)")]
        public required decimal Balance { get; set; }
    }
}
