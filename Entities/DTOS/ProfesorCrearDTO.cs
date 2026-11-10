
namespace Entities.DTOS
{
    public class ProfesorCrearDTO
    {
        public string Cedula { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido1 { get; set; } = string.Empty;
        public string Apellido2 { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public int IdEspecialidad { get; set; }
        public string CodigoInstitucional { get; set; } = string.Empty;

        // Entradas de horario para cada día de la semana
        public TimeOnly? EntradaLunes { get; set; }
        public TimeOnly? EntradaMartes { get; set; }
        public TimeOnly? EntradaMiercoles { get; set; }
        public TimeOnly? EntradaJueves { get; set; }
        public TimeOnly? EntradaViernes { get; set; }
    }
}
