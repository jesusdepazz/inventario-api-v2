using InventarioApi.Models;
using Inventory.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MantenimientosController : ControllerBase
    {
        private readonly InventarioContext _context;

        public MantenimientosController(InventarioContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Mantenimiento>>> GetMantenimientos([FromQuery] string? codificacion)
        {
            if (string.IsNullOrEmpty(codificacion))
            {
                return await _context.Mantenimientos.ToListAsync();
            }

            return await _context.Mantenimientos
                .Where(m => m.Codificacion == codificacion)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Mantenimiento>> GetMantenimientoById(int id)
        {
            var mantenimiento = await _context.Mantenimientos.FindAsync(id);

            if (mantenimiento == null)
            {
                return NotFound();
            }

            return mantenimiento;
        }

        [HttpPost]
        public async Task<ActionResult<Mantenimiento>> PostMantenimiento([FromBody] Mantenimiento mantenimiento)
        {
            var equipoExiste = await _context.Equipos
                .AnyAsync(e => e.Codificacion == mantenimiento.Codificacion && e.Modelo == mantenimiento.Modelo);

            if (!equipoExiste)
            {
                return NotFound($"No se encontró un equipo con el código {mantenimiento.Codificacion} y modelo {mantenimiento.Modelo}.");
            }

            if (mantenimiento.Fecha == default)
            {
                mantenimiento.Fecha = DateTime.Now;
            }

            _context.Mantenimientos.Add(mantenimiento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMantenimientoById), new { id = mantenimiento.Id }, mantenimiento);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMantenimiento(int id)
        {
            var mantenimiento = await _context.Mantenimientos.FindAsync(id);
            if (mantenimiento == null)
            {
                return NotFound();
            }

            _context.Mantenimientos.Remove(mantenimiento);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
