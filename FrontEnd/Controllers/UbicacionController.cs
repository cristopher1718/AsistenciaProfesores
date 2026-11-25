using BusinessLogicLevel.Servicios;
using Entities.DTOS;
using FrontEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FrontEnd.Controllers
{
    [Authorize]
    public class UbicacionController(UbicacionServico ubicacionServico, ProfesorServicio profesorServicio, AulaServicio aulaServicio) : Controller
    {
        private readonly UbicacionServico _ubicacionServico = ubicacionServico;
        private readonly ProfesorServicio _profesorServicio = profesorServicio;
        private readonly AulaServicio _aulaServicio = aulaServicio;

        [HttpGet]
        public async Task<IActionResult> Index(int diaSemana = 1)
        {
            // Guardamos el día seleccionado en el ViewBag para marcar el botón HTML de un color diferente
            ViewBag.DiaSeleccionado = diaSemana;

            var respuesta = await _ubicacionServico.ObtenerUbicacionesPorDiaAsync(diaSemana);

            if (respuesta.Ok)
            {
                return View(respuesta.Data);
            }
            else
            {
                ViewBag.Error = respuesta.Message;
                // Devolvemos una lista vacía para no quebrar la vista
                return View(new List<UbicacionProfesorDTO>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditarHorario (string codigo)
        {

            // 1. Obtener datos del profesor y las aulas (Asumiendo que tienes estos métodos en tus servicios)
            var respuestaProfesor = await _profesorServicio.ObtenerProfesorInfo(codigo);
            var respuestaAulas = await _aulaServicio.ObtenerTodasAsync();

            if (!respuestaProfesor.Ok || respuestaProfesor.Data == null)
            {
                TempData["Error"] = "No se encontró el profesor.";
                return RedirectToAction("Index"); // Redirigir a tu lista de docentes
            }

            var modelo = new EditarHorarioViewModel
            {
                IdProfesor = respuestaProfesor.Data.IdProfesor,
                NombreProfesor = $"{respuestaProfesor.Data.NombreCompleto}",
                ListaAulas = respuestaAulas.Data?.Select(a => new SelectListItem
                {
                    Value = a.IdAula.ToString(),
                    Text = a.NumeroAula
                })
            };

            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> EditarHorario(EditarHorarioViewModel modelo)
        {

            // Filtramos filas vacías que el usuario haya agregado por error sin marcar lecciones
            var ubicacionesValidas = modelo.NuevasUbicaciones
                .Where(u => u.LeccionesActivas != null && u.LeccionesActivas.Count != 0)
                .ToList();

            var dtoToSave = new ActualizarHorarioDTO
            {
                IdProfesor = modelo.IdProfesor,
                NuevasUbicaciones = ubicacionesValidas
            };

            // Enviamos el DTO al BLL para ejecutar el Wipe & Replace transaccional
            var respuesta = await _ubicacionServico.ActualizarUbicacionesProfesores(dtoToSave);

            if (respuesta.Ok)
            {
                TempData["MensajeExito"] = respuesta.Message;
                return RedirectToAction("Index", "Profesor"); // Volver al catálogo de personal
            }

            // Si hay error, recargamos los catálogos y mostramos la vista
            ViewBag.Error = respuesta.Message;
            var respuestaAulas = await _aulaServicio.ObtenerTodasAsync();
            modelo.ListaAulas = respuestaAulas.Data?.Select(a => new SelectListItem { Value = a.IdAula.ToString(), Text = a.NumeroAula });

            return View(modelo);
        }

    }
}