using Entities.DTOS;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FrontEnd.Models
{
    public class EditarHorarioViewModel
    {
        public int IdProfesor { get; set; }
        public string NombreProfesor { get; set; } = string.Empty;

        // Catálogo para los <select> de la vista
        public IEnumerable<SelectListItem>? ListaAulas { get; set; }

        // Aquí MVC mapeará automáticamente las filas dinámicas de la vista
        public List<UbicacionDetalleDTO> NuevasUbicaciones { get; set; } = new List<UbicacionDetalleDTO>();
    }
}
