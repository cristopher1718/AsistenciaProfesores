using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DTOS
{
    public class ActualizarHorarioDTO
    {
        public int IdProfesor { get; set; }
        public List<UbicacionDetalleDTO> NuevasUbicaciones { get; set; } = [];
    }

    public class UbicacionDetalleDTO
    {
        public int IdAula { get; set; }
        public int DiaSemana { get; set; } // 1=Lunes, 2=Martes, etc.
        public List<int> LeccionesActivas { get; set; } = []; // Ej: [1, 2, 3]
    }
}