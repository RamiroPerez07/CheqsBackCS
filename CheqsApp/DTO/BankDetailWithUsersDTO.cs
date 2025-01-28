namespace CheqsApp.DTO
{
    public class BankDetailWithUsersDTO
    {
        public required BanksDTO Bank { get; set; }
        public List<UserSimpleDetailDTO> Users { get; set; } = new List<UserSimpleDetailDTO>();
    }
}
