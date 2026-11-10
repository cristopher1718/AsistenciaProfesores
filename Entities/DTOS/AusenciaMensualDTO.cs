using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DTOS
{
    public class AusenciaMensualDTO
    {
        public int IdProfesor { get; set; }

        // Fecha correspondiente al mes que se está reportando
        public DateOnly FechaAusencia { get; set; }

        public int LeccionesJustificadas { get; set; }
        public int LeccionesInjustificadas { get; set; }

        // No pedimos FechaRegistroAplicacion aquí porque la generará 
        // el servidor automáticamente por seguridad.
    }
}
