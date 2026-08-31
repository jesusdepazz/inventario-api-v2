using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventarioApi.Models;
using InventoryApi.Models;
using Inventory.Data;

namespace InventarioApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BajaActivoController : ControllerBase
    {
        private readonly InventarioContext _context;

        public BajaActivoController(InventarioContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CrearBaja([FromBody] BajaActivo baja)
        {
            if (baja == null)
                return BadRequest("Datos inválidos.");

            var equipo = await _context.Equipos
                .FirstOrDefaultAsync(e => e.Codificacion == baja.CodificacionEquipo);

            if (equipo == null)
                return NotFound($"No se encontró el equipo con codificación {baja.CodificacionEquipo}");

            _context.BajaActivos.Add(baja);

            if (!string.IsNullOrWhiteSpace(baja.UbicacionDestino))
            {
                equipo.Ubicacion = baja.UbicacionDestino;
                equipo.FechaActualizacion = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Baja registrada correctamente",
                baja,
                equipoActualizado = new
                {
                    equipo.Id,
                    equipo.Codificacion,
                    equipo.Ubicacion,
                    equipo.FechaActualizacion
                }
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetBajas()
        {
            var bajas = await _context.BajaActivos
                .OrderByDescending(b => b.FechaBaja)
                .ToListAsync();

            return Ok(bajas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var baja = await _context.BajaActivos
                .FirstOrDefaultAsync(b => b.Id == id);

            if (baja == null)
                return NotFound($"No se encontró la baja con ID {id}");

            return Ok(baja);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarBaja(int id)
        {
            var baja = await _context.BajaActivos.FindAsync(id);
            if (baja == null)
                return NotFound($"No se encontró la baja con ID {id}");

            _context.BajaActivos.Remove(baja);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Baja eliminada correctamente" });
        }
    }
}
