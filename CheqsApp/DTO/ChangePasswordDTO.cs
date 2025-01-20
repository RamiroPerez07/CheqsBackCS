using System.ComponentModel.DataAnnotations;

namespace CheqsApp.DTO
{
    public class ChangePasswordDTO
    {
        [Required(ErrorMessage = "La contraseña actual es obligatoria.")]
        public string CurrentPassword { get; set; } = string.Empty;  // Contraseña actual del usuario

        [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
        public string NewPassword { get; set; } = string.Empty;      // Nueva contraseña
    }
}
