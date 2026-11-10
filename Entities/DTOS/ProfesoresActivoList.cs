
namespace Entities.DTOS
{
    public class ProfesoresActivoList
    {
        public int IdProfesor { get; set; }
        public string Cedula { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string NombreEspecialidad { get; set; } = string.Empty;
        public string CodigoInstitucional { get; set; } = string.Empty;
    }
}
