using BusinessLogicLevel.Servicios;
using FrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Entities.DTOS;
using Microsoft.AspNetCore.Authorization;

namespace FrontEnd.Controllers
{
    [Authorize]
    public class AsistenciaController(AsistenciaServicio asistenciaServicio, ProfesorServicio profesorServicio, AusenciaService ausenciaService) : Controller
    {
        readonly AsistenciaServicio _asistenciaServicio = asistenciaServicio;
        readonly ProfesorServicio _profesorServicio = profesorServicio;
        readonly AusenciaService _ausenciaService = ausenciaService;

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> RegistrarEntrada()
        {
            var modelo = new AsistenciaCreateViewModel
            {
                ListaProfesores = await ObtenerListaProfesoresAsync()
            };
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarEntrada(AsistenciaCreateViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            // Enviamos los datos al cerebro (BLL) para que calcule la tardía según el horario de ese día
            var respuesta = await _asistenciaServicio.RegistrarEntradaAsync(modelo.CodigoProfesor, modelo.Fecha, modelo.HoraEntrada);

            if (respuesta.Ok)
            {
                // Si el cálculo y guardado fue exitoso, mostramos un mensaje temporal de éxito
                TempData["MensajeExito"] = respuesta.Message;
                return RedirectToAction(nameof(RegistrarEntrada)); // Recargamos el formulario limpio
            }
            else
            {
                // Si es fin de semana o el profesor tiene el día libre, mostramos el error
                ViewBag.Error = respuesta.Message;
                modelo.ListaProfesores = await ObtenerListaProfesoresAsync();
                return View(modelo);
            }
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerListaProfesoresAsync()
        {
            var respuesta = await _profesorServicio.GetAllProfesoresActivosAsync();
            if (respuesta.Ok && respuesta.Data != null)
            {
                // Mostramos el nombre completo y la cédula para facilitar la búsqueda
                return respuesta.Data.Select(p => new SelectListItem
                {
                    Value = p.IdProfesor.ToString(),
                    Text = $"{p.Cedula} - {p.NombreCompleto}"
                });
            }
            return new List<SelectListItem>();
        }

        // Registro de Ausencias en Lote
        // Prepara la tabla con todos los docentes activos
        [HttpGet]
        public async Task<IActionResult> RegistrarAusencias()
        {
            var modelo = new AusenciaLoteViewModel
            {
                // Sugerimos el día 1 del mes actual por defecto
                FechaMes = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1)
            };

            // Traemos todos los profesores activos
            var respuestaProfesores = await _profesorServicio.GetAllProfesoresActivosAsync();

            if (respuestaProfesores.Ok && respuestaProfesores.Data != null)
            {
                // Armamos una fila en 0 para cada profesor
                foreach (var profe in respuestaProfesores.Data)
                {
                    modelo.Filas.Add(new AusenciaFilaViewModel
                    {
                        IdProfesor = profe.IdProfesor,
                        Cedula = profe.Cedula,
                        NombreCompleto = profe.NombreCompleto,
                        Justificadas = 0,
                        Injustificadas = 0
                    });
                }
            }

            return View(modelo);
        }

        // 2. MÉTODO POST: Recibe toda la tabla llena y la envía a procesar
        [HttpPost]
        public async Task<IActionResult> RegistrarAusencias(AusenciaLoteViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            // Convertimos la lista visual a nuestra lista DTO para el servicio
            var listaDTOs = new List<AusenciaMensualDTO>();

            foreach (var fila in modelo.Filas)
            {
                // Solo enviamos los que el usuario modificó (que tengan más de 0)
                if (fila.Justificadas > 0 || fila.Injustificadas > 0)
                {
                    listaDTOs.Add(new AusenciaMensualDTO
                    {
                        IdProfesor = fila.IdProfesor,
                        FechaAusencia = modelo.FechaMes,
                        LeccionesJustificadas = fila.Justificadas,
                        LeccionesInjustificadas = fila.Injustificadas
                    });
                }
            }

            // Si nadie tuvo ausencias y le dio guardar...
            if (listaDTOs.Count == 0)
            {
                ViewBag.Error = "No se ingresó ninguna ausencia mayor a 0 en la tabla.";
                return View(modelo);
            }

            var respuesta = await _ausenciaService.RegistrarAusenciasEnLoteAsync(listaDTOs); 

            if (respuesta.Ok)
            {
                TempData["MensajeExito"] = respuesta.Message;
                return RedirectToAction(nameof(RegistrarAusencias));
            }
            else
            {
                ViewBag.Error = respuesta.Message;
                return View(modelo);
            }
        }
    }
}
