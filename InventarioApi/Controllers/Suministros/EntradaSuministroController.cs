using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inventory.Data;
using InventarioApi.Models.Suministros;

namespace InventarioApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntradaSuministroController : ControllerBase
    {
        private readonly InventarioContext _context;

        public EntradaSuministroController(InventarioContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EntradaSuministro>>> GetEntradas()
        {
            return await _context.EntradaSuministros
                                 .Include(e => e.Suministro)
                                 .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EntradaSuministro>> GetEntrada(int id)
        {
            var entrada = await _context.EntradaSuministros
                                        .Include(e => e.Suministro)
                                        .FirstOrDefaultAsync(e => e.Id == id);

            if (entrada == null)
                return NotFound();

            return entrada;
        }

        [HttpPost]
        public async Task<ActionResult<EntradaSuministro>> PostEntrada(EntradaSuministro entrada)
        {
            var suministro = await _context.Suministros
                                           .FirstOrDefaultAsync(s => s.Id == entrada.SuministroId);

            if (suministro == null)
                return BadRequest("El suministro no existe.");

            entrada.Fecha = DateTime.Now;
            _context.EntradaSuministros.Add(entrada);

            suministro.CantidadActual += entrada.CantidadProducto;

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEntrada), new { id = entrada.Id }, entrada);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutEntrada(int id, EntradaSuministro entrada)
        {
            if (id != entrada.Id)
                return BadRequest();

            _context.Entry(entrada).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EntradaExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEntrada(int id)
        {
            var entrada = await _context.EntradaSuministros.FindAsync(id);
            if (entrada == null)
                return NotFound();

            var suministro = await _context.Suministros
                                           .FirstOrDefaultAsync(s => s.Id == entrada.SuministroId);

            if (suministro != null)
                suministro.CantidadActual -= entrada.CantidadProducto;

            _context.EntradaSuministros.Remove(entrada);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EntradaExists(int id)
        {
            return _context.EntradaSuministros.Any(e => e.Id == id);
        }
    }
}
