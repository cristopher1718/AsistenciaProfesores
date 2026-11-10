using Microsoft.EntityFrameworkCore;
using DataAccessLevel.Context;
using Entities;
using Entities.DTOS;

namespace BusinessLogicLevel.Servicios
{
    public class AulaServicio(ControlAsistenciaContext context)
    {
        private readonly ControlAsistenciaContext _context = context;

        /// <summary>
        /// Obtiene el catálogo completo de aulas registradas.
        /// </summary>
        public async Task<Response<List<AulaDTO>>> ObtenerTodasAsync()
        {
            var respuesta = new Response<List<AulaDTO>>();

            try
            {
                var lista = await _context.Aulas
                    .Select(a => new AulaDTO
                    {
                        IdAula = a.IdAula,
                        NumeroAula = a.NumeroAula
                    })
                    .ToListAsync();

                respuesta.Ok = true;
                respuesta.Data = lista;
                respuesta.Message = "Catálogo de aulas obtenido correctamente.";
                return respuesta;
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Message = $"Error al obtener las aulas en BLL: {ex.Message}";
                return respuesta;
            }
        }
    }
}
