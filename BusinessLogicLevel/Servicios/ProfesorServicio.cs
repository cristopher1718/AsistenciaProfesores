using DataAccessLevel.Context;
using Entities;
using Entities.DBEntities;
using Entities.DTOS;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogicLevel.Servicios
{
    public class ProfesorServicio(ControlAsistenciaContext context)
    {
        private readonly ControlAsistenciaContext _context = context;

        /// <summary>
        /// Devuelve una lista con todos los profesores activos.
        /// </summary>
        public async Task<Response<List<ProfesoresActivoList>>> GetAllProfesoresActivosAsync()
        {
            var respuesta = new Response<List<ProfesoresActivoList>>();

            try
            {
                var listaProfesores = await _context.Profesores
                    .Include(p => p.IdEspecialidadNavigation)
                    .Include(p => p.HistorialCodigos) // Hacemos JOIN con el historial de códigos
                    .Where(p => p.Estado == true)
                    .Select(p => new ProfesoresActivoList
                    {
                        IdProfesor = p.IdProfesor,
                        Cedula = p.Cedula,
                        NombreCompleto = $"{p.Nombre} {p.Apellido1} {p.Apellido2}", // Concatenamos el nombre aquí mismo
                        Telefono = p.Telefono,
                        // Si la especialidad no es null, tomamos el nombre
                        NombreEspecialidad = p.IdEspecialidadNavigation != null ? p.IdEspecialidadNavigation.NombreEspecialidad : "Sin Asignar",

                        // REGLA DE NEGOCIO: Buscamos en la lista de historiales el código activo (FechaRetiro == null).
                        // Si lo encontramos, tomamos el string del código. Si no tiene, devolvemos "Sin Código".
                        CodigoInstitucional = p.HistorialCodigos
                            .Where(h => h.FechaRetiro == null)
                            .Select(h => h.CodigoInstitucional)
                            .FirstOrDefault() ?? "Sin Código"
                    })
                    .ToListAsync();

                respuesta.Ok = true;
                respuesta.Message = "Profesores activos obtenidos correctamente";
                respuesta.Data = listaProfesores;

                return respuesta;
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Message = $"Error al obtener los profesores activos en la capa BLL: {ex.Message}";
                respuesta.Data = null;
                return respuesta;
            }
        }

        /// <summary>
        /// Valida las reglas de negocio y registra un nuevo profesor junto con su código institucional.
        /// </summary>
        public async Task<Response<bool>> CrearProfesorAsync(ProfesorCrearDTO dto)
        {
            var respuesta = new Response<bool>();

            try
            {
                // Validar que la cédula no exista
                var existeCedula = await _context.Profesores.AnyAsync(p => p.Cedula == dto.Cedula);
                if (existeCedula)
                {
                    respuesta.Ok = false;
                    respuesta.Message = "Ya existe un profesor registrado con esta cédula.";
                    return respuesta;
                }

                // Validar que el código institucional no esté en uso
                var codigoEnUso = await _context.HistorialCodigos
                    .AnyAsync(h => h.CodigoInstitucional == dto.CodigoInstitucional && h.FechaRetiro == null);

                if (codigoEnUso)
                {
                    respuesta.Ok = false;
                    respuesta.Message = "El código institucional ingresado ya está asignado a un profesor activo.";
                    return respuesta;
                }

                var nuevoProfesor = new Profesore
                {
                    Cedula = dto.Cedula,
                    Nombre = dto.Nombre,
                    Apellido1 = dto.Apellido1,
                    Apellido2 = dto.Apellido2,
                    Telefono = dto.Telefono,
                    IdEspecialidad = dto.IdEspecialidad,
                    Estado = true,

                    HistorialCodigos =
                    [
                        new() {
                            CodigoInstitucional = dto.CodigoInstitucional,
                            FechaAsignacion = DateOnly.FromDateTime(DateTime.Now),
                            FechaRetiro = null
                        }
                    ],

                    HorariosEntradum = new HorariosEntradum
                    {
                        EntradaLunes = dto.EntradaLunes,
                        EntradaMartes = dto.EntradaMartes,
                        EntradaMiercoles = dto.EntradaMiercoles,
                        EntradaJueves = dto.EntradaJueves,
                        EntradaViernes = dto.EntradaViernes
                    }
                };

                _context.Profesores.Add(nuevoProfesor);

                await _context.SaveChangesAsync();

                respuesta.Ok = true;
                respuesta.Message = "Profesor registrado exitosamente.";
                respuesta.Data = true;

                return respuesta;
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Message = $"Ocurrió un error al registrar el profesor en la capa BLL: {ex.Message}";
                respuesta.Data = false;

                return respuesta;
            }
        }

        /// <summary>
        /// Obtiene los datos de un profesor específico para cargarlos en la pantalla de edición.
        /// </summary>
        public async Task<Response<ProfesorEditDTO>> ObtenerProfesorParaEdicionAsync(int idProfesor)
        {
            var respuesta = new Response<ProfesorEditDTO>();

            try
            {
                // Buscamos al profesor con su horario y su historial de códigos
                var profesor = await _context.Profesores
                    .Include(p => p.HorariosEntradum)
                    .Include(p => p.HistorialCodigos)
                    .FirstOrDefaultAsync(p => p.IdProfesor == idProfesor);

                if (profesor == null)
                {
                    respuesta.Ok = false;
                    respuesta.Message = "El profesor no fue encontrado en el sistema.";
                    return respuesta;
                }

                // Mapeamos los datos de la Entidad hacia nuestro DTO de edición
                var dto = new ProfesorEditDTO
                {
                    IdProfesor = profesor.IdProfesor,
                    Cedula = profesor.Cedula,
                    Nombre = profesor.Nombre,
                    Apellido1 = profesor.Apellido1,
                    Apellido2 = profesor.Apellido2,
                    Telefono = profesor.Telefono,
                    IdEspecialidad = profesor.IdEspecialidad,
                    Estado = profesor.Estado,

                    // Extraemos el código institucional activo (FechaRetiro nula)
                    CodigoInstitucional = profesor.HistorialCodigos
                        .Where(h => h.FechaRetiro == null)
                        .Select(h => h.CodigoInstitucional)
                        .FirstOrDefault() ?? "",

                    // Cargamos el horario si existe utilizando el operador de propagación nula (?.)
                    EntradaLunes = profesor.HorariosEntradum?.EntradaLunes,
                    EntradaMartes = profesor.HorariosEntradum?.EntradaMartes,
                    EntradaMiercoles = profesor.HorariosEntradum?.EntradaMiercoles,
                    EntradaJueves = profesor.HorariosEntradum?.EntradaJueves,
                    EntradaViernes = profesor.HorariosEntradum?.EntradaViernes
                };

                respuesta.Ok = true;
                respuesta.Data = dto;
                return respuesta;
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Message = $"Error al obtener los datos del profesor en la capa BLL: {ex.Message}";
                return respuesta;
            }
        }

        /// <summary>
        /// Valida y actualiza la información de un profesor, manejando el historial de códigos y sus horarios.
        /// </summary>
        public async Task<Response<bool>> ActualizarProfesorAsync(ProfesorEditDTO dto)
        {
            var respuesta = new Response<bool>();

            try
            {
                // 1. Buscamos al profesor con todas sus relaciones
                var profesor = await _context.Profesores
                    .Include(p => p.HorariosEntradum)
                    .Include(p => p.HistorialCodigos)
                    .FirstOrDefaultAsync(p => p.IdProfesor == dto.IdProfesor);

                if (profesor == null)
                {
                    respuesta.Ok = false;
                    respuesta.Message = "El profesor que intenta editar no existe.";
                    return respuesta;
                }

                // 2. REGLA: Validar que la nueva cédula no pertenezca a OTRO profesor
                var existeCedula = await _context.Profesores
                    .AnyAsync(p => p.Cedula == dto.Cedula && p.IdProfesor != dto.IdProfesor);

                if (existeCedula)
                {
                    respuesta.Ok = false;
                    respuesta.Message = "La cédula ingresada ya pertenece a otro profesor en el sistema.";
                    return respuesta;
                }

                // 3. REGLA: Lógica del Historial de Códigos
                // Obtenemos el código que tiene activo actualmente (FechaRetiro == null)
                var codigoActivoActual = profesor.HistorialCodigos.FirstOrDefault(h => h.FechaRetiro == null);

                if (dto.Estado == false && codigoActivoActual != null)
                {
                    // Si está libre, "cerramos" el código viejo poniéndole la fecha de retiro
                    codigoActivoActual?.FechaRetiro = DateOnly.FromDateTime(DateTime.Now);
                }

                // 4. Actualizamos los datos personales y el estado
                profesor.Cedula = dto.Cedula;
                profesor.Nombre = dto.Nombre;
                profesor.Apellido1 = dto.Apellido1;
                profesor.Apellido2 = dto.Apellido2;
                profesor.Telefono = dto.Telefono;
                profesor.IdEspecialidad = dto.IdEspecialidad;
                profesor.Estado = dto.Estado;

                // 5. Actualizamos el Horario de Entrada
                if (profesor.HorariosEntradum != null)
                {
                    // Si ya tenía horario, lo sobrescribimos
                    profesor.HorariosEntradum.EntradaLunes = dto.EntradaLunes;
                    profesor.HorariosEntradum.EntradaMartes = dto.EntradaMartes;
                    profesor.HorariosEntradum.EntradaMiercoles = dto.EntradaMiercoles;
                    profesor.HorariosEntradum.EntradaJueves = dto.EntradaJueves;
                    profesor.HorariosEntradum.EntradaViernes = dto.EntradaViernes;
                }
                else
                {
                    profesor.HorariosEntradum = new HorariosEntradum
                    {
                        EntradaLunes = dto.EntradaLunes,
                        EntradaMartes = dto.EntradaMartes,
                        EntradaMiercoles = dto.EntradaMiercoles,
                        EntradaJueves = dto.EntradaJueves,
                        EntradaViernes = dto.EntradaViernes
                    };
                }

                // 6. Guardamos todos los cambios en cascada
                await _context.SaveChangesAsync();

                respuesta.Ok = true;
                respuesta.Data = true;
                respuesta.Message = "Datos del profesor actualizados exitosamente.";
                return respuesta;
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Message = $"Ocurrió un error al actualizar en BLL: {ex.Message}";
                return respuesta;
            }
        }

        /// <sumary>
        /// Retorna un profesor con todos sus datos y ubicacion de aula del dia actual
        /// </sumary>
        public async Task<Response<ProfesorObtenerInfoDTO>> ObtenerProfesorInfo(string codigo)
        {
            Response<ProfesorObtenerInfoDTO> respuesta = new();

            try
            {
                int diaSemanaActual = (int)DateTime.Now.DayOfWeek;

                if(diaSemanaActual == 0 || diaSemanaActual == 6)
                {
                    respuesta.Ok = false;
                    respuesta.Message = "No se laboran sabados o domingos";
                    respuesta.Data = null;

                    return respuesta;
                }

                var profesorInfo = await _context.HistorialCodigos
                    .Where(h => h.CodigoInstitucional == codigo && h.FechaRetiro == null)
                    .Select(h => new ProfesorObtenerInfoDTO
                    {
                        IdProfesor = h.IDProfesorNavigation.IdProfesor,
                        Estado = h.IDProfesorNavigation.Estado,
                        CodigoInstitucional = h.CodigoInstitucional,
                        NombreCompleto = $"{h.IDProfesorNavigation.Nombre} {h.IDProfesorNavigation.Apellido1} {h.IDProfesorNavigation.Apellido2}" ?? "No disponible",
                        Especialidad = h.IDProfesorNavigation.IdEspecialidadNavigation != null ? h.IDProfesorNavigation.IdEspecialidadNavigation.NombreEspecialidad : "Sin Asignar",
                        Ubicaciones = h.IDProfesorNavigation.UbicacionProfesores.Where(u => u.DiaSemana == diaSemanaActual)
                            .Select(u => new UbicacionesHoy
                            {
                                Aula = u.IdAulaNavigation != null ? u.IdAulaNavigation.NumeroAula : "No disponible",
                                LeccionesActivas = new List<int>
                                {
                                    u.Leccion_1 ? 1 : 0,
                                    u.Leccion_2 ? 2 : 0,
                                    u.Leccion_3 ? 3 : 0,
                                    u.Leccion_4 ? 4 : 0,
                                    u.Leccion_5 ? 5 : 0,
                                    u.Leccion_6 ? 6 : 0,
                                    u.Leccion_7 ? 7 : 0,
                                    u.Leccion_8 ? 8 : 0
                                }
                            })
                            .ToList(),
                    })
                    .FirstOrDefaultAsync();

                if (profesorInfo == null || profesorInfo.Estado != true)
                {
                    respuesta.Ok = false;
                    respuesta.Message = "No se encontró un profesor activo con el código institucional proporcionado.";
                    respuesta.Data = null;
                    return respuesta;
                }
                else
                {
                    foreach (var ubicacion in profesorInfo.Ubicaciones)
                    {
                        // Filtramos las lecciones activas para eliminar los ceros
                        ubicacion.LeccionesActivas = [.. ubicacion.LeccionesActivas.Where(l => l != 0)];
                    }
                    // Cambiamos numeros de dias par dias de la semana en Español
                    switch (diaSemanaActual)
                    {
                        case 0:
                            profesorInfo.DiaSemana = "Domingo";
                            break;
                        case 1:
                            profesorInfo.DiaSemana = "Lunes";
                            break;
                        case 2:
                            profesorInfo.DiaSemana = "Martes";
                            break;
                        case 3:
                            profesorInfo.DiaSemana = "Miércoles";
                            break;
                        case 4:
                            profesorInfo.DiaSemana = "Jueves";
                            break;
                        case 5:
                            profesorInfo.DiaSemana = "Viernes";
                            break;
                        case 6:
                            profesorInfo.DiaSemana = "Sábado";
                            break;
                    }
                    respuesta.Ok = true;
                    respuesta.Message = "Información del profesor obtenida correctamente.";
                    respuesta.Data = profesorInfo;
                    return respuesta;
                }

            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Message = $"Ocurrió un error al obtener la información del profesor en BLL: {ex.Message}";
                respuesta.Data = null;
                return respuesta;
            }
        }

        public async Task<Response<bool>> RehabilitarProfesor(string codigo)
        {
            Response<bool> respuesta = new();
            try
            {
                var profesor = await _context.HistorialCodigos
                    .Where(h => h.CodigoInstitucional == codigo && h.FechaRetiro == null)
                    .Select(h => h.IDProfesorNavigation)
                    .FirstOrDefaultAsync();

                if (profesor == null)
                {
                    respuesta.Ok = false;
                    respuesta.Message = "No se encontró un profesor con el código institucional proporcionado.";
                    respuesta.Data = false;
                    return respuesta;
                }
                profesor.Estado = true;

                await _context.SaveChangesAsync();

                respuesta.Ok = true;
                respuesta.Message = "Profesor rehabilitado exitosamente.";
                respuesta.Data = true;
                return respuesta;
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Message = $"Ocurrió un error al rehabilitar al profesor en BLL: {ex.Message}";
                respuesta.Data = false;
                return respuesta;
            }
        }
    }
}
