using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DTOS
{
    public class ProfesorObtenerInfoDTO
    {
        public int IdProfesor { get; set; }
        public bool Estado { get; set; }
        public string? CodigoInstitucional { get; set; } = string.Empty;
        public string? NombreCompleto { get; set; } = string.Empty;
        public string? Especialidad { get; set; } = string.Empty;
        public string? DiaSemana { get; set; } = string.Empty;
        public List<UbicacionesHoy> Ubicaciones { get; set; } = [];
    }

    public class UbicacionesHoy
    {
        public string? Aula { get; set; } = string.Empty;
        public List<int> LeccionesActivas { get; set; } = [];
    }
}