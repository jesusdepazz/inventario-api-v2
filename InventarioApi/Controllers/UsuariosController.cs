using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inventory.Data;
using InventarioApi.Models;

namespace InventarioApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly InventarioContext _context;

        public UsuariosController(InventarioContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CrearUsuario([FromBody] Usuario nuevoUsuario)
        {
            if (string.IsNullOrEmpty(nuevoUsuario.Correo) || string.IsNullOrEmpty(nuevoUsuario.Rol))
            {
                return BadRequest("Correo y rol son obligatorios.");
            }

            var existe = await _context.Usuarios.AnyAsync(u => u.Correo == nuevoUsuario.Correo);
            if (existe)
            {
                return Conflict("Este usuario ya está registrado.");
            }

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Usuario creado con éxito.", usuario = nuevoUsuario });
        }

        [HttpPut("{correo}/rol")]
        public async Task<IActionResult> CambiarRol(string correo, [FromBody] string nuevoRol)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);
            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            usuario.Rol = nuevoRol;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Rol actualizado con éxito.", usuario });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Usuario eliminado con éxito." });
        }

    }
}                                                                                                                                                                                                                                                                                                          
 