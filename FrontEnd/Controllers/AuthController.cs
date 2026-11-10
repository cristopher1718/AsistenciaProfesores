using BusinessLogicLevel.Servicios;
using Entities.DTOS;
using FrontEnd.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FrontEnd.Controllers
{
    public class AuthController(UsuarioServicio usuarioServicio) : Controller
    {
        private readonly UsuarioServicio _usuarioServicio = usuarioServicio;

        // Login inicio de sesión
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if(User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var respuesta = await _usuarioServicio.ValidarLoginAsync(model.NombreUsuario, model.Contrasenna);

            if (respuesta.Ok && respuesta.Data != null)
            {
                var usuario = respuesta.Data;

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                    new Claim(ClaimTypes.Role, usuario.Rol), 
                    new Claim("IdUsuario", usuario.IdUsuario.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, respuesta.Message);
                ViewBag.Error = respuesta.Message;
                return View(model);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth");
        }

        // Registro de usuarios en el sistema
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(UsuarioRegistroViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var dto = new UsuarioCreateDTO
            {
                NombreUsuario = modelo.NombreUsuario,
                Contrasenna = modelo.Contrasenna,
                Rol = modelo.Rol
            };

            var respuesta = await _usuarioServicio.RegistrarUsuarioAsync(dto);

            if (respuesta.Ok)
            {
                TempData["MensajeExito"] = "Usuario creado con éxito. Ya puede iniciar sesión.";
                return RedirectToAction(nameof(Registrar));
            }
            else
            {
                ViewBag.Error = respuesta.Message;
                return View(modelo);
            }
        }
    }
}
