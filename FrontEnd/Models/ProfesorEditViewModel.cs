using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FrontEnd.Models
{
    public class ProfesorEditViewModel
    {
        [Required]
        public int IdProfesor { get; set; }

        [Required(ErrorMessage = "La cédula es obligatoria")]
        [Display(Name = "Cédula")]
        public string Cedula { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El primer apellido es obligatorio")]
        [Display(Name = "Primer Apellido")]
        public string Apellido1 { get; set; } = string.Empty;

        [Display(Name = "Segundo Apellido")]
        public string Apellido2 { get; set; } = string.Empty;

        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe seleccionar una especialidad")]
        [Display(Name = "Especialidad")]
        public int IdEspecialidad { get; set; }

        [Required(ErrorMessage = "El código institucional es obligatorio")]
        [Display(Name = "Código Institucional")]
        public string CodigoInstitucional { get; set; } = string.Empty;

        [Display(Name = "Estado en el Sistema")]
        public bool Estado { get; set; } // True = Activo, False = Inactivo

        // Horarios
        [DataType(DataType.Time)]
        [Display(Name = "Entrada Lunes")]
        public TimeOnly? EntradaLunes { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Entrada Martes")]
        public TimeOnly? EntradaMartes { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Entrada Miércoles")]
        public TimeOnly? EntradaMiercoles { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Entrada Jueves")]
        public TimeOnly? EntradaJueves { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Entrada Viernes")]
        public TimeOnly? EntradaViernes { get; set; }

        public IEnumerable<SelectListItem>? ListaEspecialidades { get; set; }
    }
}
