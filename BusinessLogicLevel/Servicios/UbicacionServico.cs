using DataAccessLevel.Context;
using Entities.DTOS;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogicLevel.Servicios
{
    public class UbicacionServico(ControlAsistenciaContext context)
    {
        private readonly ControlAsistenciaContext _context = context;

        /// <summary>
        /// Obtiene las ubicaciones de los profesores filtradas por un día de la semana.
        /// </summary>
        /// <param name="diaSemana">1=Lunes, 2=Martes, 3=Miércoles, 4=Jueves, 5=Viernes</param>
        public async Task<Response<List<UbicacionProfesorDTO>>> ObtenerUbicacionesPorDiaAsync(int diaSemana)
        {
            var respuesta = new Response<List<UbicacionProfesorDTO>>();

            try
            {
                // Hacemos JOIN con Profesor y Aula para traer los nombres
                var ubicacionesRaw = await _context.UbicacionProfesores
                    .Include(u => u.IdProfesorNavigation)
                    .Include(u => u.IdAulaNavigation)
                    .Where(u => u.DiaSemana == diaSemana && u.IdProfesorNavigation.Estado == true)
                    .ToListAsync();

                var listaDTOs = new List<UbicacionProfesorDTO>();

                foreach (var u in ubicacionesRaw)
                {
                    var dto = new UbicacionProfesorDTO
                    {
                        IdUbicacion = u.IdUbicacion,
                        IdProfesor = u.IdProfesor,
                        // Concatenamos el nombre completo del profesor
                        NombreProfesor = u.IdProfesorNavigation != null
                            ? $"{u.IdProfesorNavigation.Nombre} {u.IdProfesorNavigation.Apellido1} {u.IdProfesorNavigation.Apellido2}"
                            : "Desconocido",
                        IdAula = u.IdAula,
                        NumeroAula = u.IdAulaNavigation?.NumeroAula ?? "Sin Aula",
                        DiaSemana = u.DiaSemana
                    };

                    // Transformamos los campos bit de SQL (Leccion_1 a Leccion_12) en una lista
                    // NOTA: Revisa tu clase UbicacionProfesores generada por EF Core para confirmar 
                    // que las propiedades se llaman exactamente Leccion1, Leccion2, etc. (a veces EF quita el guion bajo).
                    if (u.Leccion_1 == true) dto.LeccionesActivas.Add(1);
                    if (u.Leccion_2 == true) dto.LeccionesActivas.Add(2);
                    if (u.Leccion_3 == true) dto.LeccionesActivas.Add(3);
                    if (u.Leccion_4 == true) dto.LeccionesActivas.Add(4);
                    if (u.Leccion_5 == true) dto.LeccionesActivas.Add(5);
                    if (u.Leccion_6 == true) dto.LeccionesActivas.Add(6);
                    if (u.Leccion_7 == true) dto.LeccionesActivas.Add(7);
                    if (u.Leccion_8 == true) dto.LeccionesActivas.Add(8);
                    if (u.Leccion_9 == true) dto.LeccionesActivas.Add(9);
                    if (u.Leccion_10 == true) dto.LeccionesActivas.Add(10);
                    if (u.Leccion_11 == true) dto.LeccionesActivas.Add(11);
                    if (u.Leccion_12 == true) dto.LeccionesActivas.Add(12);

                    listaDTOs.Add(dto);
                }

                respuesta.Ok = true;
                respuesta.Data = listaDTOs;
                return respuesta;
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Message = $"Error al obtener las ubicaciones en BLL: {ex.Message}";
                return respuesta;
            }
        }
    }
}