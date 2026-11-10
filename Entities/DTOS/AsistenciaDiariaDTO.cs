using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DTOS
{
    public class AsistenciaDiariaDTO
    {
        public int IdAsistencia { get; set; }
        public int IdProfesor { get; set; }
        public string NombreProfesor { get; set; } = string.Empty;

        // Datos transaccionales de la asistencia
        public DateTime Fecha { get; set; }
        public TimeOnly HoraEntrada { get; set; }

        // Puede ser nulo porque al entrar en la mañana, aún no hay hora de salida
        public TimeOnly? HoraSalida { get; set; }

        public string TipoTardia { get; set; } = string.Empty;
    }
}
