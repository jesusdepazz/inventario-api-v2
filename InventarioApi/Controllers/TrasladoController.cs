using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventarioApi.Models;
using Inventory.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventarioApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrasladosController : ControllerBase
    {
        private readonly InventarioContext _context;

        public TrasladosController(InventarioContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Traslado>>> GetAll()
        {
            return await _context.Traslados
                .Include(t => t.Equipos)
                .Include(t => t.EmpleadoEntrega)
                .Include(t => t.EmpleadoRecibe)
                .OrderByDescending(t => t.FechaEmision)
                .ToListAsync();
        }

        [HttpGet("{no}")]
        public async Task<ActionResult<Traslado>> GetByNo(string no)
        {
            var traslado = await _context.Traslados
                .Include(t => t.Equipos)
                .Include(t => t.EmpleadoEntrega)
                .Include(t => t.EmpleadoRecibe)
                .FirstOrDefaultAsync(t => t.No == no);

            if (traslado == null)
                return NotFound();

            return traslado;
        }

        [HttpPost]
        public async Task<ActionResult<Traslado>> Create([FromBody] Traslado nuevoTraslado)
        {
            if (nuevoTraslado == null || string.IsNullOrEmpty(nuevoTraslado.No))
                return BadRequest("Datos de traslado inválidos.");

            if (!nuevoTraslado.Equipos.Any())
                return BadRequest("Debe agregar al menos un equipo.");

            if (await _context.Traslados.AnyAsync(t => t.No == nuevoTraslado.No))
                return Conflict("Ya existe un traslado con ese número.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Traslados.Add(nuevoTraslado);
                await _context.SaveChangesAsync();

                foreach (var det in nuevoTraslado.Equipos)
                {
                    var equipo = await _context.Equipos
                        .FirstOrDefaultAsync(e => e.Codificacion == det.Equipo);

                    if (equipo == null)
                        return NotFound($"No se encontró el equipo {det.Equipo}");

                    equipo.Ubicacion = nuevoTraslado.UbicacionHasta;
                    equipo.FechaActualizacion = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetByNo),
                    new { no = nuevoTraslado.No },
                    nuevoTraslado);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        [HttpPut("{no}")]
        public async Task<IActionResult> Update(string no, [FromBody] Traslado updated)
        {
            if (no != updated.No)
                return BadRequest("El número de traslado no coincide.");

            var traslado = await _context.Traslados
                .Include(t => t.Equipos)
                .Include(t => t.EmpleadoEntrega)
                .Include(t => t.EmpleadoRecibe)
                .FirstOrDefaultAsync(t => t.No == no);

            if (traslado == null)
                return NotFound();

            traslado.FechaEmision = updated.FechaEmision;
            traslado.Status = updated.Status;
            traslado.Motivo = updated.Motivo;
            traslado.Observaciones = updated.Observaciones;
            traslado.UbicacionDesde = updated.UbicacionDesde;
            traslado.UbicacionHasta = updated.UbicacionHasta;
            traslado.EmpleadoEntrega = updated.EmpleadoEntrega;
            traslado.EmpleadoRecibe = updated.EmpleadoRecibe;
            _context.TrasladoEquipos.RemoveRange(traslado.Equipos);
            traslado.Equipos = updated.Equipos;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{no}")]
        public async Task<IActionResult> Delete(string no)
        {
            var traslado = await _context.Traslados
                .Include(t => t.Equipos)
                .Include(t => t.EmpleadoEntrega)
                .Include(t => t.EmpleadoRecibe)
                .FirstOrDefaultAsync(t => t.No == no);

            if (traslado == null)
                return NotFound();

            _context.Traslados.Remove(traslado);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}