using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryApi.Models;
using Inventory.Data;

namespace InventoryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MobiliarioEquipoController : ControllerBase
    {
        private readonly InventarioContext _context;

        public MobiliarioEquipoController(InventarioContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MobiliarioEquipo>>> GetMobiliarioEquipos([FromQuery] string? estado, [FromQuery] string? ubicacion)
        {
            var query = _context.MobiliarioEquipos
                .Include(m => m.ReportesDanios)
                .AsQueryable();

          
            if (!string.IsNullOrEmpty(estado))
            {
                query = query.Where(m => m.Estado != null && m.Estado.ToLower() == estado.ToLower());
            }

            if (!string.IsNullOrEmpty(ubicacion))
            {
                query = query.Where(m => m.Ubicacion != null && m.Ubicacion.Contains(ubicacion));
            }

            return await query.ToListAsync();
        }

       
        [HttpGet("{id}")]
        public async Task<ActionResult<MobiliarioEquipo>> GetMobiliarioEquipo(int id)
        {
            var mobiliarioEquipo = await _context.MobiliarioEquipos
                .Include(m => m.ReportesDanios)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mobiliarioEquipo == null)
            {
                return NotFound(new { mensaje = "Mobiliario o equipo no encontrado." });
            }

            return mobiliarioEquipo;
        }

        
        [HttpPost]
        public async Task<ActionResult<MobiliarioEquipo>> PostMobiliarioEquipo(MobiliarioEquipo mobiliarioEquipo)
        {
            if (mobiliarioEquipo.FechaIngreso == null)
            {
                mobiliarioEquipo.FechaIngreso = DateTime.Now;
            }

            _context.MobiliarioEquipos.Add(mobiliarioEquipo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMobiliarioEquipo), new { id = mobiliarioEquipo.Id }, mobiliarioEquipo);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMobiliarioEquipo(int id, MobiliarioEquipo mobiliarioEquipo)
        {
            if (id != mobiliarioEquipo.Id)
            {
                return BadRequest(new { mensaje = "El ID enviado no coincide con el registro a actualizar." });
            }

            mobiliarioEquipo.FechaActualizacion = DateTime.Now;
            _context.Entry(mobiliarioEquipo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MobiliarioEquipoExists(id))
                {
                    return NotFound(new { mensaje = "Mobiliario o equipo no encontrado." });
                }
                throw;
            }

            return NoContent();
        }

  
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMobiliarioEquipo(int id)
        {
            var mobiliarioEquipo = await _context.MobiliarioEquipos.FindAsync(id);
            if (mobiliarioEquipo == null)
            {
                return NotFound(new { mensaje = "Mobiliario o equipo no encontrado." });
            }

            _context.MobiliarioEquipos.Remove(mobiliarioEquipo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        
        
        [HttpPost("{id}/reportes-danios")]
        public async Task<ActionResult<ReporteDanio>> PostReporteDanio(int id, ReporteDanio reporte)
        {
            var mobiliarioEquipo = await _context.MobiliarioEquipos.FindAsync(id);
            if (mobiliarioEquipo == null)
            {
                return NotFound(new { mensaje = "No se puede registrar el reporte porque el equipo no existe." });
            }

            reporte.MobiliarioEquipoId = id;

            if (reporte.FechaReporte == default)
            {
                reporte.FechaReporte = DateTime.Now;
            }

            _context.ReportesDanios.Add(reporte);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMobiliarioEquipo), new { id = id }, reporte);
        }

        private bool MobiliarioEquipoExists(int id)
        {
            return _context.MobiliarioEquipos.Any(e => e.Id == id);
        }
    }
}