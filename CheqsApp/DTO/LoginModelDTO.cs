using System.ComponentModel.DataAnnotations;

namespace CheqsApp.DTO
{
    public class LoginModelDTO
    {
        // Nombre de usuario del usuario que está intentando iniciar sesión
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        public string Username { get; set; } = string.Empty;

        // Contraseña del usuario
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Password { get; set; } = string.Empty;
    }
}
