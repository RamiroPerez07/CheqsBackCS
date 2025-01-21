namespace CheqsApp.Models
{
    public class Bank
    {
        public int Id { get; set; }

        public string BankName { get; set; } = string.Empty;

        // Relacion con bank business
        public ICollection<BankBusiness> BankBusinesses { get; set; } = new List<BankBusiness>();
    }
}
