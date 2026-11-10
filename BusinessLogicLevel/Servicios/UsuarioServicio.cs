using DataAccessLevel.Context;
using Entities;
using Entities.DBEntities;
using Entities.DTOS;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace BusinessLogicLevel.Servicios
{
    public class UsuarioServicio(ControlAsistenciaContext context)
    {
        private readonly ControlAsistenciaContext _context = context;

        public async Task<Response<Usuario>> ValidarLoginAsync(string nombreUsuario, string contrasenna)
        {
            try
            {
                string contrasennaHasheada = HashServicio.ComputeHash(contrasenna);
                var usuarioEncontrado = await _context.Usuarios.FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario && u.Contrasenna == contrasennaHasheada) ?? null;

                // Validar si el usuario fue encontrado
                if (usuarioEncontrado != null)
                {
                    if (usuarioEncontrado.Estado != true)
                    {
                        return new Response<Usuario> { Ok = false, Message = "Usuario Inactivo, consulte al administrador.", Data = null };
                    }
                    else
                    {
                        return new Response<Usuario> { Ok = true, Message = "Login exitoso.", Data = usuarioEncontrado };
                    }
                }
                if(usuarioEncontrado == null)
                {
                    return new Response<Usuario> { Ok = false, Message = "Credenciales inválidas.", Data = null };
                }

                return new Response<Usuario> { Ok = false, Message = "Credenciales inválidas.", Data = null };
            }
            catch (Exception ex)
            {
                return new Response<Usuario> { Ok = false, Message = "Error al validar el login del usuario: " + ex.Message, Data = null };
            }
        }

        /// <summary>
        /// Registra un nuevo usuario administrativo aplicando hashing SHA256 a la contraseña.
        /// </summary>
        public async Task<Response<bool>> RegistrarUsuarioAsync(UsuarioCreateDTO dto)
        {
            var respuesta = new Response<bool>();

            try
            {
                bool existeUsuario = await _context.Usuarios
                    .AnyAsync(u => u.NombreUsuario.ToLower() == dto.NombreUsuario.ToLower());

                if (existeUsuario)
                {
                    respuesta.Ok = false;
                    respuesta.Message = "El nombre de usuario ya está en uso. Por favor, elija otro.";
                    return respuesta;
                }

                // 2. Encriptar la contraseña usando tu clase SHA256
                // (Reemplaza 'SeguridadHelper.HashearClave' por el nombre real de tu clase/método)
                string contrasennaHasheada = HashServicio.ComputeHash(dto.Contrasenna);

                // 3. Instanciar la entidad para guardar en la base de datos[cite: 3]
                var nuevoUsuario = new Usuario
                {
                    NombreUsuario = dto.NombreUsuario,
                    Contrasenna = contrasennaHasheada,
                    Rol = dto.Rol,
                    Estado = true 
                };

                _context.Usuarios.Add(nuevoUsuario);
                await _context.SaveChangesAsync();

                respuesta.Ok = true;
                respuesta.Data = true;
                respuesta.Message = "Usuario administrativo registrado exitosamente.";
                return respuesta;
            }
            catch (Exception ex)
            {
                respuesta.Ok = false;
                respuesta.Message = $"Ocurrió un error al registrar el usuario en BLL: {ex.Message}";
                return respuesta;
            }
        }
    }
}
