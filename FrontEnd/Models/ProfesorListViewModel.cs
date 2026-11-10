namespace FrontEnd.Models
{
    public class ProfesorListViewModel
    {
        public int IdProfesor { get; set; }
        public string CodigoInstitucional { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string NombreEspecialidad { get; set; } = string.Empty;
    }
}