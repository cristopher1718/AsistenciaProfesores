using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DTOS
{
    public class UbicacionProfesorDTO
    {
        public int IdUbicacion { get; set; }

        // Datos del Profesor
        public int IdProfesor { get; set; }
        public string NombreProfesor { get; set; } = string.Empty;

        // Datos del Aula
        public int IdAula { get; set; }
        public string NumeroAula { get; set; } = string.Empty;

        // Datos de la Ubicación
        public int DiaSemana { get; set; } 
        public List<int> LeccionesActivas { get; set; } = [];
    }
}
