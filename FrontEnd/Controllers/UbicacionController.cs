using Microsoft.AspNetCore.Mvc;
using BusinessLogicLevel.Servicios;
using Entities.DTOS;
using Microsoft.AspNetCore.Authorization;

namespace FrontEnd.Controllers
{
    [Authorize]
    public class UbicacionController(UbicacionServico ubicacionServico) : Controller
    {
        private readonly UbicacionServico _ubicacionServico = ubicacionServico;

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
    }
}