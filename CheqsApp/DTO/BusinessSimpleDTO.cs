namespace CheqsApp.DTO
{
    public class BusinessSimpleDTO
    {
        public int BusinessId { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
    }
}
