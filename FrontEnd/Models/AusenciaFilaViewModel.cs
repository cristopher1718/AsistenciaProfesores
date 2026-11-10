using System.ComponentModel.DataAnnotations;

namespace FrontEnd.Models
{
    public class AusenciaFilaViewModel
    {
        public int IdProfesor { get; set; }
        public string Cedula { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;

        [Range(0, 100, ErrorMessage = "Valor inválido")]
        public int Justificadas { get; set; } = 0;

        [Range(0, 100, ErrorMessage = "Valor inválido")]
        public int Injustificadas { get; set; } = 0;
    }

    // Modelo principal que agrupa la fecha y todas las filas
    public class AusenciaLoteViewModel
    {
        [Required(ErrorMessage = "La fecha del reporte es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Mes a Reportar (Seleccione el primer día del mes)")]
        public DateOnly FechaMes { get; set; }

        public List<AusenciaFilaViewModel> Filas { get; set; } = [];
    }
}
