using System.ComponentModel.DataAnnotations;

namespace FrontEnd.Models
{
    public class UsuarioRegistroViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [Display(Name = "Nombre de Usuario")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        public string Contrasenna { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe confirmar la contraseña")]
        [DataType(DataType.Password)]
        [Compare("Contrasenna", ErrorMessage = "Las contraseñas no coinciden")]
        [Display(Name = "Confirmar Contraseña")]
        public string ConfirmarContrasenna { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe asignar un rol al usuario")]
        public string Rol { get; set; } = string.Empty;
    }
}