using BusinessLogicLevel.Servicios;
using FrontEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FrontEnd.Controllers
{
    [Authorize]
    public class ReportesController(ReporteServicio reporteServicio) : Controller
    {
        private readonly ReporteServicio _reporteServicio = reporteServicio;

        [HttpGet]
        public IActionResult Index()
        {
            var modelo = new ReporteViewModel();
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Index(ReporteViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            // Pequeña validación lógica para evitar que viajen al pasado
            if (modelo.FechaInicio > modelo.FechaFin)
            {
                ViewBag.Error = "La fecha 'Desde' no puede ser mayor a la fecha 'Hasta'.";
                return View(modelo);
            }

            // Llamamos a nuestro motor de consultas en el BLL
            var respuesta = await _reporteServicio.GenerarReporteAsync(modelo.FechaInicio, modelo.FechaFin, modelo.CodigoFiltro);

            if (respuesta.Ok && respuesta.Data != null)
            {
                modelo.Resultados = respuesta.Data;

                if (modelo.Resultados.Count == 0)
                {
                    ViewBag.Info = "No se encontraron registros de tardías o ausencias en este rango de fechas.";
                }
            }
            else
            {
                ViewBag.Error = respuesta.Message;
            }

            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> DescargarPdf(ReporteViewModel modelo)
        {
            // Reutilizamos el motor de consultas que ya teníamos
            var respuesta = await _reporteServicio.GenerarReporteAsync(modelo.FechaInicio, modelo.FechaFin, modelo.CodigoFiltro);

            if (respuesta.Ok && respuesta.Data != null && respuesta.Data.Any())
            {
                // Generamos el archivo
                var pdfBytes = PdfReporteService.GenerarReportePdf(respuesta.Data, modelo.FechaInicio, modelo.FechaFin);

                // Retornamos el archivo al navegador del usuario
                return File(pdfBytes, "application/pdf", $"Reporte_Asistencia_{DateTime.Now:yyyyMMdd}.pdf");
            }

            // Si no hay datos, lo devolvemos a la vista normal con un error
            TempData["Error"] = "No hay datos para generar el PDF en este rango de fechas.";
            return RedirectToAction(nameof(Index));
        }
    }
}
