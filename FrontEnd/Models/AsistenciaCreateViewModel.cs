using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FrontEnd.Models
{
    public class AsistenciaCreateViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar un codigo de profesor")]
        [Display(Name = "Codigo Profesor")]
        public string? CodigoProfesor { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; } = DateTime.Now.Date; // Por defecto la fecha de hoy

        [Required(ErrorMessage = "La hora de entrada es obligatoria")]
        [DataType(DataType.Time)]
        [Display(Name = "Hora de Entrada Real")]
        public TimeOnly HoraEntrada { get; set; }

        // Propiedad auxiliar para cargar el menú desplegable en la vista HTML
        public IEnumerable<SelectListItem>? ListaProfesores { get; set; }
    }
}
