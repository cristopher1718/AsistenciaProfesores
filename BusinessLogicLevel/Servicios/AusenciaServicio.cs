using DataAccessLevel.Context;
using Entities;
using Entities.DTOS;
using Entities.DBEntities;

namespace BusinessLogicLevel.Servicios
{
    public class AusenciaService (ControlAsistenciaContext contexto)
    {
        private readonly ControlAsistenciaContext _context = contexto;

        /// <summary>
        /// Recibe una lista de ausencias para procesarlas en lote, aplicando el sello de auditoría.
        /// </summary>
        public async Task<Response<bool>> RegistrarAusenciasEnLoteAsync(List<AusenciaMensualDTO> listaAusencias)
        {
            var respuesta = new Response<bool>();

            try
            {
                if (listaAusencias == null || listaAusencias.Count == 0)
                {
                    respuesta.Ok = false;
                    respuesta.Message = "No se recibieron datos para procesar.";
                    return respuesta;
                }

                // Sello de auditoría invariable: La fecha y hora exacta en la que el servidor procesa el registro[cite: 1]
                DateOnly fechaRegistroAuditoria = DateOnly.FromDateTime(DateTime.Now);

                // Lista de entidades que Entity Framework va a insertar en la base de datos
                var nuevasAusencias = new List<AusenciasMensuale>();

                foreach (var dto in listaAusencias)
                {
                    // Validación de seguridad: No guardar registros en 0
                    if (dto.LeccionesJustificadas > 0 || dto.LeccionesInjustificadas > 0)
                    {
                        nuevasAusencias.Add(new AusenciasMensuale
                        {
                            IdProfesor = dto.IdProfesor,
                            FechaAusencia = dto.FechaAusencia,
                            LeccionesJustificadas = dto.LeccionesJustificadas,
                            LeccionesInjustificadas = dto.LeccionesInjustificadas,

                            FechaRegistroAplicacion = fechaRegistroAuditoria
                        });
                    }
                }

                if (nuevasAusencias.Count != 0)
                {
                    // Guardado masivo (Bulk Insert optimizado por EF Core)
                    _context.AusenciasMensuales.AddRange(nuevasAusencias);
                    await _context.SaveChangesAsync();
                }

                respuesta.Ok = true;
                respuesta.Data = true;
                respuesta.Message = $"Se registraron {nuevasAusencias.Count} reportes de ausencias exitosamente.";
                return respuesta;
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Message = $"Ocurrió un error al registrar las ausencias: {ex.Message}";
                return respuesta;
            }
        }
    }
}