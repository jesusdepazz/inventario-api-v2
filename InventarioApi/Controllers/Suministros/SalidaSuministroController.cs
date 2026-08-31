using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inventory.Data;
using InventarioApi.Models.Suministros;

namespace InventarioApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalidasSuministrosController : ControllerBase
    {
        private readonly InventarioContext _context;

        public SalidasSuministrosController(InventarioContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalidaSuministro>>> GetSalidas()
        {
            return await _context.SalidaSuministros
                .Include(s => s.Suministro)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SalidaSuministro>> GetSalida(int id)
        {
            var salida = await _context.SalidaSuministros
                .Include(s => s.Suministro)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (salida == null)
                return NotFound();

            return salida;
        }

        [HttpPost]
        public async Task<ActionResult<SalidaSuministro>> CrearSalida(SalidaSuministro salida)
        {
            var suministro = await _context.Suministros.FindAsync(salida.SuministroId);

            if (suministro == null)
                return BadRequest("El suministro no existe.");

            if (suministro.CantidadActual < salida.CantidadProducto)
                return BadRequest("No hay suficiente inventario para esta salida.");

            salida.Fecha = DateTime.Now;
            _context.SalidaSuministros.Add(salida);

            suministro.CantidadActual -= salida.CantidadProducto;

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSalida), new { id = salida.Id }, salida);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSalida(int id)
        {
            var salida = await _context.SalidaSuministros.FindAsync(id);

            if (salida == null)
                return NotFound();

            var suministro = await _context.Suministros.FindAsync(salida.SuministroId);
            if (suministro != null)
                suministro.CantidadActual += salida.CantidadProducto;

            _context.SalidaSuministros.Remove(salida);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
