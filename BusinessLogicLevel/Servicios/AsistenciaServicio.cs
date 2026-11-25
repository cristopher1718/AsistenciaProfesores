using DataAccessLevel.Context;
using Entities;
using Entities.DBEntities;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogicLevel.Servicios
{
    public class AsistenciaServicio(ControlAsistenciaContext context)
    {
        private readonly ControlAsistenciaContext _context = context;

        /// <summary>
        /// Registra la hora de entrada, calcula matemáticamente la tardía y la guarda en la base de datos.
        /// </summary>
        public async Task<Response<bool>> RegistrarEntradaAsync(string codProfesor, DateTime fecha, TimeOnly horaEntrada)
        {
            var respuesta = new Response<bool>();

            try
            {

                var profesor = await _context.HistorialCodigos.FirstOrDefaultAsync(c => c.CodigoInstitucional == codProfesor && c.FechaRetiro == null);

                if (profesor == null)
                {
                    respuesta.Ok = false;
                    respuesta.Message = "El profesor no existe en el sistema.";
                    return respuesta;
                }

                // Buscamos el horario oficial del profesor
                var horario = await _context.HorariosEntrada
                    .FirstOrDefaultAsync(h => h.IdProfesor == profesor.IDProfesor);

                if (horario == null)
                {
                    respuesta.Ok = false;
                    respuesta.Message = "El profesor no tiene un horario configurado en el sistema.";
                    return respuesta;
                }

                // 2. Determinamos qué día de la semana es la fecha ingresada para buscar la hora oficial
                TimeOnly? horaOficialEsperada = null;

                switch (fecha.DayOfWeek)
                {
                    case DayOfWeek.Monday: horaOficialEsperada = horario.EntradaLunes; break;
                    case DayOfWeek.Tuesday: horaOficialEsperada = horario.EntradaMartes; break;
                    case DayOfWeek.Wednesday: horaOficialEsperada = horario.EntradaMiercoles; break;
                    case DayOfWeek.Thursday: horaOficialEsperada = horario.EntradaJueves; break;
                    case DayOfWeek.Friday: horaOficialEsperada = horario.EntradaViernes; break;
                    case DayOfWeek.Saturday:
                    case DayOfWeek.Sunday:
                        respuesta.Ok = false;
                        respuesta.Message = "El sistema no registra asistencias en fines de semana.";
                        return respuesta;
                }

                // 3. Regla de Negocio: Si el valor es NULL, el profesor tiene el día libre
                if (horaOficialEsperada == null)
                {
                    respuesta.Ok = false;
                    respuesta.Message = "El profesor tiene el día libre hoy, no se requiere marca de entrada.";
                    return respuesta;
                }

                // 4. EL CÁLCULO MATEMÁTICO DE LA TARDÍA
                string tipoTardia = "A Tiempo"; // Estado por defecto

                // Restamos la hora real menos la oficial (Ej: 07:06 - 07:00 = 6 minutos)
                double diferencia = horaEntrada.ToTimeSpan().TotalMinutes - horaOficialEsperada.Value.ToTimeSpan().TotalMinutes;

                if (diferencia > 0)
                {
                    if (diferencia <= 5)
                    {
                        tipoTardia = "-5m";
                    }
                    else
                    {
                        tipoTardia = "+5m";
                    }
                }

                // 5. Instanciamos el registro para guardarlo en la bitácora
                var nuevaAsistencia = new AsistenciaDiarium
                {
                    IdProfesor = profesor.IDProfesor,
                    Fecha = DateOnly.FromDateTime(fecha.Date),
                    HoraEntrada = horaEntrada,
                    TipoTardia = tipoTardia
                    // HoraSalida queda NULL por ahora
                };

                _context.AsistenciaDiaria.Add(nuevaAsistencia);
                await _context.SaveChangesAsync();

                respuesta.Ok = true;
                respuesta.Data = true;
                respuesta.Message = $"Entrada registrada para {profesor.CodigoInstitucional}. Estado calculado: {tipoTardia}.";
                return respuesta;
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Message = $"Ocurrió un error al registrar la entrada en BLL: {ex.Message}";
                return respuesta;
            }
        }

    }
}
