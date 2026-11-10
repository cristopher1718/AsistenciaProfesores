using DataAccessLevel.Context;
using Entities.DTOS;
using Microsoft.EntityFrameworkCore;
using Entities;

namespace BusinessLogicLevel.Servicios
{
    public class ReporteServicio(ControlAsistenciaContext context)
    {
        private readonly ControlAsistenciaContext _context = context;

        public async Task<Response<List<ReporteAsistenciaDTO>>> GenerarReporteAsync(DateOnly fechaInicio, DateOnly fechaFin, string? codigoFiltro = null)
        {
            var respuesta = new Response<List<ReporteAsistenciaDTO>>();

            try
            {
                // 1. Obtenemos todos los profesores activos para armar la base del reporte
                var queryProfesores = _context.Profesores
                    .Include(p => p.HistorialCodigos)
                    .Where(p => p.Estado == true)
                    .AsQueryable();

                // Si viene un código específico, filtramos por aquellos que tengan ese código activo[cite: 1, 2]
                if (!string.IsNullOrWhiteSpace(codigoFiltro))
                {
                    queryProfesores = queryProfesores.Where(p => p.HistorialCodigos
                        .Any(h => h.CodigoInstitucional == codigoFiltro && h.FechaRetiro == null));
                }

                var profesores = await queryProfesores.ToListAsync();
                var listaReporte = new List<ReporteAsistenciaDTO>();

                foreach (var profe in profesores)
                {
                    // Buscamos el código actual del profesor
                    var codigoActual = profe.HistorialCodigos
                        .FirstOrDefault(h => h.FechaRetiro == null)?.CodigoInstitucional ?? "Sin Código";

                    // 2. Calculamos las métricas diarias en el rango de fechas
                    var marcasDiarias = await _context.AsistenciaDiaria
                        .Where(a => a.IdProfesor == profe.IdProfesor && a.Fecha >= fechaInicio && a.Fecha <= fechaFin)
                        .ToListAsync();

                    int tardiasMenores = marcasDiarias.Count(a => a.TipoTardia == "-5m");
                    int tardiasMayores = marcasDiarias.Count(a => a.TipoTardia == "+5m");

                    // 3. Calculamos las métricas mensuales en el rango de fechas[cite: 2]
                    var ausenciasMensuales = await _context.AusenciasMensuales
                        .Where(a => a.IdProfesor == profe.IdProfesor && a.FechaAusencia >= fechaInicio && a.FechaAusencia <= fechaFin)
                        .ToListAsync();

                    int justificadas = ausenciasMensuales.Sum(a => a.LeccionesJustificadas);
                    int injustificadas = ausenciasMensuales.Sum(a => a.LeccionesInjustificadas);

                    // 4. Armamos el DTO solo si tiene datos (o puedes omitir este if si quieres ver a todos en 0)
                    if (tardiasMenores > 0 || tardiasMayores > 0 || justificadas > 0 || injustificadas > 0)
                    {
                        listaReporte.Add(new ReporteAsistenciaDTO
                        {
                            CodigoInstitucional = codigoActual,
                            NombreCompleto = $"{profe.Nombre} {profe.Apellido1} {profe.Apellido2}",
                            TardiasMenores5 = tardiasMenores,
                            TardiasMayores5 = tardiasMayores,
                            AusenciasJustificadas = justificadas,
                            AusenciasInjustificadas = injustificadas
                        });
                    }
                }

                respuesta.Ok = true;
                respuesta.Data = listaReporte;
                respuesta.Message = "Reporte generado con éxito.";
                return respuesta;
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Message = $"Error al generar el reporte en BLL: {ex.Message}";
                return respuesta;
            }
        }
    }
}