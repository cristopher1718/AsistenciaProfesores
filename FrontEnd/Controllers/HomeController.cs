using BusinessLogicLevel.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static QuestPDF.Helpers.Colors;

namespace FrontEnd.Controllers
{
    [Authorize]
    public class HomeController(ProfesorServicio profesorServicio, HistorialCodigoServicio historialCodigoServicio) : Controller
    {
        private readonly ProfesorServicio _profesorServicio = profesorServicio;
        private readonly HistorialCodigoServicio _historialCodigoServicio = historialCodigoServicio;

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerInfoProfesor(string codigo)
        {
            var respuesta = await _profesorServicio.ObtenerProfesorInfo(codigo);

            return Json(respuesta);
            
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerHistorialCodigos()
        {
            var respuesta = await _historialCodigoServicio.ObtenerHistorialCodigos();
            return Json(respuesta);
        }
    }
}
