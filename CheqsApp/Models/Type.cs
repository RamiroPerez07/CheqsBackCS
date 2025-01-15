namespace CheqsApp.Models
{
    public class Type
    {
        public int Id { get; set; }

        public string TypeName { get; set; } = string.Empty;

        public ICollection<Cheq> Cheqs { get; set; } = new List<Cheq>();
    }
}
