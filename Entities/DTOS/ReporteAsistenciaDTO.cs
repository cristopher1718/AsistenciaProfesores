using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DTOS
{
    public class ReporteAsistenciaDTO
    {
        public string CodigoInstitucional { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;

        // Métricas de Asistencia Diaria
        public int TardiasMenores5 { get; set; }
        public int TardiasMayores5 { get; set; }

        // Métricas de Ausencias Mensuales
        public int AusenciasJustificadas { get; set; }
        public int AusenciasInjustificadas { get; set; }
    }
}
