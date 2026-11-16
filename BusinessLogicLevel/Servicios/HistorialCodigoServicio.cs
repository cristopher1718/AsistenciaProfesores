using DataAccessLevel.Context;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogicLevel.Servicios
{
    public class HistorialCodigoServicio(ControlAsistenciaContext context)
    {
        private readonly ControlAsistenciaContext _context = context;

        public async Task<Response<List<HistorialCodigoInfoDTO>>> ObtenerHistorialCodigos()
        {
            var response = new Response<List<HistorialCodigoInfoDTO>>();

            try
            {
                var historialCodigos = await _context.HistorialCodigos
                    .Include(h => h.IDProfesorNavigation)
                    .Include(h => h.IDProfesorNavigation.IdEspecialidadNavigation)
                    .Where(h => h.IDProfesor == h.IDProfesorNavigation.IdProfesor)
                    .Select(c => new HistorialCodigoInfoDTO
                    {
                        Codigo = c.CodigoInstitucional,
                        NombreCompleto = c.IDProfesorNavigation.Nombre + " " + c.IDProfesorNavigation.Apellido1 + " " + c.IDProfesorNavigation.Apellido2,
                        Especialidad = c.IDProfesorNavigation.IdEspecialidadNavigation.NombreEspecialidad
                    })
                    .ToListAsync();

                response.Ok = true;
                response.Message = "Historial de códigos obtenido correctamente.";
                response.Data = historialCodigos;

                return response;

            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.Message = $"Error al obtener el historial de códigos: {ex.Message}";
                response.Data = null;

                return response;

            }
        }
    }

    public class HistorialCodigoInfoDTO
    {
        public string? Codigo { get; set; }
        public string? NombreCompleto { get; set; }
        public string? Especialidad { get; set; }
    }
}
