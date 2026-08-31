using Inventory.Data;
using Inventory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Inventory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UbicacionesController : ControllerBase
    {
        private readonly InventarioContext _context;

        public UbicacionesController(InventarioContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ubicacion>>> GetUbicaciones()
        {
            return await _context.Ubicaciones.OrderBy(u => u.Nombre).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Ubicacion>> CrearUbicacion([FromBody] Ubicacion nuevaUbicacion)
        {
            if (string.IsNullOrWhiteSpace(nuevaUbicacion.Nombre))
            {
                return BadRequest("El nombre de la ubicación es obligatorio.");
            }

            bool existe = await _context.Ubicaciones.AnyAsync(u => u.Nombre == nuevaUbicacion.Nombre);
            if (existe)
            {
                return Conflict("Ya existe una ubicación con ese nombre.");
            }

            _context.Ubicaciones.Add(nuevaUbicacion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUbicaciones), new { id = nuevaUbicacion.Id }, nuevaUbicacion);
        }


        [HttpPost("importar-excel")]
        public async Task<IActionResult> ImportarUbicacionesDesdeExcel(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
                return BadRequest("El archivo está vacío.");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var nuevasUbicaciones = new List<Ubicacion>();

            using (var stream = new MemoryStream())
            {
                await archivo.CopyToAsync(stream);
                using var paquete = new ExcelPackage(stream);
                var hoja = paquete.Workbook.Worksheets[0];

                int filas = hoja.Dimension.Rows;

                for (int fila = 2; fila <= filas; fila++)
                {
                    string? nombreUbicacion = hoja.Cells[fila, 2].Text?.Trim();

                    if (string.IsNullOrWhiteSpace(nombreUbicacion))
                        continue;

                    bool yaExiste = await _context.Ubicaciones.AnyAsync(u => u.Nombre == nombreUbicacion);
                    if (!yaExiste)
                    {
                        nuevasUbicaciones.Add(new Ubicacion
                        {
                            Nombre = nombreUbicacion
                        });
                    }
                }

                if (!nuevasUbicaciones.Any())
                    return BadRequest("No se encontraron ubicaciones nuevas para importar.");

                await _context.Ubicaciones.AddRangeAsync(nuevasUbicaciones);
                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                mensaje = "✅ Ubicaciones importadas correctamente",
                total = nuevasUbicaciones.Count
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> EliminarUbicacion(int id)
        {
            var ubicacion = await _context.Ubicaciones.FindAsync(id);
            if (ubicacion == null) return NotFound();

            _context.Ubicaciones.Remove(ubicacion);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("eliminar-todas")]
        public async Task<IActionResult> EliminarTodasLasUbicaciones([FromQuery] bool confirm = false)
        {
            if (!confirm)
            {
                return BadRequest("Debes confirmar la eliminación agregando ?confirm=true");
            }

            var ubicaciones = await _context.Ubicaciones.ToListAsync();

            if (!ubicaciones.Any())
            {
                return NotFound("No hay ubicaciones para eliminar.");
            }

            _context.Ubicaciones.RemoveRange(ubicaciones);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Se eliminaron todas las ubicaciones correctamente.",
                totalEliminadas = ubicaciones.Count
            });
        }

    }
}
