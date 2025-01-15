namespace CheqsApp.Models
{
    public class State
    {
        public int Id { get; set; }

        public string StateName { get; set; } = string.Empty;

        public ICollection<Cheq> Cheqs { get; set; } = new List<Cheq>();
    }
}
