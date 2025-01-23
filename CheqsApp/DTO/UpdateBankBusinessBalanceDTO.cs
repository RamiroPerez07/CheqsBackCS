namespace CheqsApp.DTO
{
    public class UpdateBankBusinessBalanceDTO
    {
        public int BankId { get; set; }
        public int BusinessId { get; set; }
        public decimal Balance { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
