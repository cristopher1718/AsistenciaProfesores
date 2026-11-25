using DataAccessLevel.Context;
using Entities.DTOS;
using Entities;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using Entities.DBEntities;

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

        public async Task<Response<bool>> ActualizarUbicacionesProfesores(ActualizarHorarioDTO dto)
        {
            var response = new Response<bool>();

            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                if(dto == null)
                {
                    response.Ok = false;
                    response.Message = "Lista de lecciones vacía, imposible actualizar";

                    return response;
                }
                /*
                var profesorId = await _context.HistorialCodigos
                    .Where(h => h.CodigoInstitucional == dto.Codigo && h.FechaRetiro == null)
                    .Select(h => h.IDProfesor)
                    .FirstOrDefaultAsync();
                */
                var listaUbicaciones = await _context.UbicacionProfesores
                    .Where(u => u.IdProfesor == dto.IdProfesor)
                    .ToListAsync();

                if (listaUbicaciones.Count != 0)
                {
                    _context.UbicacionProfesores.RemoveRange(listaUbicaciones);
                    await _context.SaveChangesAsync();
                }

                var nuevoHorario = new List<UbicacionProfesore>();

                foreach (var horario in  dto.NuevasUbicaciones)
                {
                    if (horario.LeccionesActivas == null || horario.LeccionesActivas.Count == 0)
                        continue;

                    var entidad = new UbicacionProfesore // Ajusta el nombre según tu EF
                    {
                        IdProfesor = dto.IdProfesor,
                        IdAula = horario.IdAula,
                        DiaSemana = horario.DiaSemana,
                        Leccion_1 = false,
                        Leccion_2 = false,
                        Leccion_3 = false,
                        Leccion_4 = false,
                        Leccion_5 = false,
                        Leccion_6 = false,
                        Leccion_7 = false,
                        Leccion_8 = false,
                        Leccion_9 = false,
                        Leccion_10 = false,
                        Leccion_11 = false,
                        Leccion_12 = false
                    };

                    if (horario.LeccionesActivas.Contains(1)) entidad.Leccion_1 = true;
                    if (horario.LeccionesActivas.Contains(2)) entidad.Leccion_2 = true;
                    if (horario.LeccionesActivas.Contains(3)) entidad.Leccion_3 = true;
                    if (horario.LeccionesActivas.Contains(4)) entidad.Leccion_4 = true;
                    if (horario.LeccionesActivas.Contains(5)) entidad.Leccion_5 = true;
                    if (horario.LeccionesActivas.Contains(6)) entidad.Leccion_6 = true;
                    if (horario.LeccionesActivas.Contains(7)) entidad.Leccion_7 = true;
                    if (horario.LeccionesActivas.Contains(8)) entidad.Leccion_8 = true;
                    if (horario.LeccionesActivas.Contains(9)) entidad.Leccion_9 = true;
                    if (horario.LeccionesActivas.Contains(10)) entidad.Leccion_10 = true;
                    if (horario.LeccionesActivas.Contains(11)) entidad.Leccion_11 = true;
                    if (horario.LeccionesActivas.Contains(12)) entidad.Leccion_12 = true;

                    nuevoHorario.Add(entidad);
                }

                if(nuevoHorario.Count != 0)
                {
                    _context.UbicacionProfesores.AddRange(nuevoHorario);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                response.Ok = true;
                response.Message = "Horario Actualizado Exitosamente";

                return response;

            }catch(Exception ex)
            {
                await transaction.RollbackAsync();
                response.Ok = false;
                response.Message = "Error al actualizar ubicaciones en BLL: " + ex.Message;

                return response;
            }
        }
    }
}