namespace CheqsApp.DTO
{
    public class UserSimpleDetailDTO
    {
        public required int UserId { get; set; }
        public required string Username { get; set; } = string.Empty;
        public required string Email { get; set; } = string.Empty;
    }
}
