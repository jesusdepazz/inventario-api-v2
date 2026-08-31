using InventarioApi.Models;
using Inventory.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MiProyectoBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly InventarioContext _context;
        private readonly IConfiguration _config;

        public AuthController(InventarioContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpGet("token")]
        [Authorize]
        public async Task<IActionResult> ObtenerTokenPersonalizado()
        {
            var correo = User.FindFirst(ClaimTypes.Email)?.Value
                      ?? User.FindFirst("preferred_username")?.Value;

            if (string.IsNullOrEmpty(correo))
                return Unauthorized("Correo no encontrado.");

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);

            if (usuario == null)
            {
                return Unauthorized("No tienes permiso para ingresar.");
            }

            if (string.IsNullOrEmpty(usuario.Rol))
                return Unauthorized("No tienes un rol asignado.");

            var claims = new[]
            {
        new Claim(ClaimTypes.Email, usuario.Correo),
        new Claim(ClaimTypes.Role, usuario.Rol)
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                rol = usuario.Rol
            });
        }
    }
}
