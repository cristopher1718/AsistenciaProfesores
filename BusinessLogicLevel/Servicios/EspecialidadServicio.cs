using DataAccessLevel.Context;
using Entities.DBEntities;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogicLevel.Servicios
{
    public class EspecialidadServicio (ControlAsistenciaContext context)
    {
        private readonly ControlAsistenciaContext _context = context;

        public async Task<Response<List<Especialidade>>> GetAllEspecialidadesAsync()
        {
            var respuesta = new Response<List<Especialidade>>();

            try
            {
                var especialidades = await _context.Especialidades.ToListAsync();

                if(especialidades != null && especialidades.Count > 0)
                {
                    respuesta.Ok = true;
                    respuesta.Message = "Especialidades obtenidas correctamente.";
                    respuesta.Data = especialidades;
                }
                else
                {
                    respuesta.Ok = false;
                    respuesta.Message = "No se encontraron especialidades.";
                    respuesta.Data = null;
                }

            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Message = $"Error al obtener las especialidades: {ex.Message}";
                respuesta.Data = null;
            }

            return respuesta;
        }
    }
}
