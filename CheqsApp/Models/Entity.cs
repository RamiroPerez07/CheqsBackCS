namespace CheqsApp.Models
{
    public class Entity
    {
        public int Id { get; set; }

        public string EntityName { get; set; } = string.Empty;

        public ICollection<Cheq> Cheqs { get; set; } = new List<Cheq>();

    }
}
