namespace Entities.DBEntities;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string NombreUsuario { get; set; } = null!;

    public string Contrasenna { get; set; } = null!;

    public string Rol { get; set; } = null!;

    public bool Estado { get; set; }
}
