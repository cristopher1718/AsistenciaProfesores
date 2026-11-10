using System.ComponentModel.DataAnnotations;

namespace Entities.DTOS
{
    public class UsuarioCreateDTO
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Contrasenna { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe seleccionar un rol")]
        public string Rol { get; set; } = string.Empty;
    }
}
