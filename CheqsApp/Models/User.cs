using System.ComponentModel.DataAnnotations;

namespace CheqsApp.Models
{
    public enum UserRole
    {
        Admin,
        User,
        Moderator
    }
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [EmailAddress]
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; } = UserRole.User;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        // Relación con BankBusinessUser
        public ICollection<BankBusinessUser> BankBusinessUsers { get; set; } = new List<BankBusinessUser>();


        // Relación con los negocios creados por el usuario
        public ICollection<Business> CreatedBusinesses { get; set; } = new List<Business>();
    }
}
