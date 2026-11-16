using BusinessLogicLevel.Servicios;
using Entities.DTOS;
using FrontEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FrontEnd.Controllers
{
    [Authorize]
    public class ProfesorController(ProfesorServicio profesorServicio, EspecialidadServicio especialidadServicio) : Controller
    {
        private readonly ProfesorServicio _profesorServicio = profesorServicio;
        private readonly EspecialidadServicio _especialidadServico = especialidadServicio;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var response = await _profesorServicio.GetAllProfesoresActivosAsync();

            if (response.Ok)
            {
                return View(response.Data);
            }
            else
            {
                ViewBag.Error= response.Message;

                return View(new List<ProfesoresActivoList>());
            }
        }

        // Abre la pantalla vacía
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var modelo = new ProfesorCreateViewModel
            {
                ListaEspecialidades = await ObtenerListaEspecialidadesAsync()
            };
            return View(modelo);
        }

        // Recibe los datos cuando dan clic en Guardar
        [HttpPost]
        public async Task<IActionResult> Create(ProfesorCreateViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                modelo.ListaEspecialidades = await ObtenerListaEspecialidadesAsync();
                return View(modelo);
            }

            var dto = new ProfesorCrearDTO
            {
                Cedula = modelo.Cedula,
                Nombre = modelo.Nombre,
                Apellido1 = modelo.Apellido1,
                Apellido2 = modelo.Apellido2,
                Telefono = modelo.Telefono,
                IdEspecialidad = modelo.IdEspecialidad,
                CodigoInstitucional = modelo.CodigoInstitucional,
                EntradaLunes = modelo.EntradaLunes,
                EntradaMartes = modelo.EntradaMartes,
                EntradaMiercoles = modelo.EntradaMiercoles,
                EntradaJueves = modelo.EntradaJueves,
                EntradaViernes = modelo.EntradaViernes
            };

            var respuesta = await _profesorServicio.CrearProfesorAsync(dto);

            if (respuesta.Ok)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewBag.Error = respuesta.Message;
                modelo.ListaEspecialidades = await ObtenerListaEspecialidadesAsync();
                return View(modelo);
            }
        }

        // Método auxiliar para no repetir código al cargar el DropDownList
        private async Task<IEnumerable<SelectListItem>> ObtenerListaEspecialidadesAsync()
        {
            var respuestaEspecialidades = await _especialidadServico.GetAllEspecialidadesAsync();
            if (respuestaEspecialidades.Ok && respuestaEspecialidades.Data != null)
            {
                return respuestaEspecialidades.Data.Select(e => new SelectListItem
                {
                    Value = e.IdEspecialidad.ToString(),
                    Text = e.NombreEspecialidad
                });
            }
            return [];
        }

        // Carga los datos actuales del profesor
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var respuesta = await _profesorServicio.ObtenerProfesorParaEdicionAsync(id);

            if (!respuesta.Ok || respuesta.Data == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var dto = respuesta.Data;

            // Mapeamos el DTO al ViewModel visual
            var modelo = new ProfesorEditViewModel
            {
                IdProfesor = dto.IdProfesor,
                Cedula = dto.Cedula,
                Nombre = dto.Nombre,
                Apellido1 = dto.Apellido1,
                Apellido2 = dto.Apellido2,
                Telefono = dto.Telefono,
                IdEspecialidad = dto.IdEspecialidad,
                CodigoInstitucional = dto.CodigoInstitucional,
                Estado = dto.Estado,
                EntradaLunes = dto.EntradaLunes,
                EntradaMartes = dto.EntradaMartes,
                EntradaMiercoles = dto.EntradaMiercoles,
                EntradaJueves = dto.EntradaJueves,
                EntradaViernes = dto.EntradaViernes,
                // Cargamos la lista para el menú desplegable
                ListaEspecialidades = await ObtenerListaEspecialidadesAsync()
            };
            return View(modelo);
        }

        // Recibe y guarda los cambios
        [HttpPost]
        public async Task<IActionResult> Edit(ProfesorEditViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                modelo.ListaEspecialidades = await ObtenerListaEspecialidadesAsync();
                return View(modelo);
            }

            var dto = new ProfesorEditDTO
            {
                IdProfesor = modelo.IdProfesor,
                Cedula = modelo.Cedula,
                Nombre = modelo.Nombre,
                Apellido1 = modelo.Apellido1,
                Apellido2 = modelo.Apellido2,
                Telefono = modelo.Telefono,
                IdEspecialidad = modelo.IdEspecialidad,
                CodigoInstitucional = modelo.CodigoInstitucional,
                Estado = modelo.Estado,
                EntradaLunes = modelo.EntradaLunes,
                EntradaMartes = modelo.EntradaMartes,
                EntradaMiercoles = modelo.EntradaMiercoles,
                EntradaJueves = modelo.EntradaJueves,
                EntradaViernes = modelo.EntradaViernes
            };

            var respuesta = await _profesorServicio.ActualizarProfesorAsync(dto);

            if (respuesta.Ok)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewBag.Error = respuesta.Message;
                modelo.ListaEspecialidades = await ObtenerListaEspecialidadesAsync();
                return View(modelo);
            }
        }

        
    }
}