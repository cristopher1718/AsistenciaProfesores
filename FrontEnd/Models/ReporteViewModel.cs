using Entities.DTOS;
using System.ComponentModel.DataAnnotations;

namespace FrontEnd.Models
{
    public class ReporteViewModel
    {
        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Desde")]
        public DateOnly FechaInicio { get; set; } = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1); // Por defecto el día 1 del mes

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Hasta")]
        public DateOnly FechaFin { get; set; } = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day); // Por defecto el día de hoy

        [Display(Name = "Código Institucional (Opcional)")]
        public string? CodigoFiltro { get; set; }

        // Aquí almacenaremos la respuesta del BLL para dibujarla en la tabla
        public List<ReporteAsistenciaDTO> Resultados { get; set; } = [];
    }
}
