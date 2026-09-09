using Inventory.Data;
using InventoryApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehiculosController : ControllerBase
    {
        private readonly InventarioContext _context;

        public VehiculosController(InventarioContext context)
        {
            _context = context;
        }

        

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vehiculo>>> GetVehiculos()
        {
            return await _context.Vehiculos.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Vehiculo>> GetVehiculo(int id)
        {
            var vehiculo = await _context.Vehiculos
                .Include(v => v.HistorialReparaciones)
                .Include(v => v.Mantenimientos)
                .Include(v => v.AlertasServicio)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vehiculo == null) return NotFound();
            return vehiculo;
        }

        [HttpPost]
        public async Task<ActionResult<Vehiculo>> PostVehiculo(Vehiculo vehiculo)
        {
            _context.Vehiculos.Add(vehiculo);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetVehiculo), new { id = vehiculo.Id }, vehiculo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutVehiculo(int id, Vehiculo vehiculo)
        {
            if (id != vehiculo.Id) return BadRequest();
            _context.Entry(vehiculo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehiculoExists(id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehiculo(int id)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo == null) return NotFound();

            _context.Vehiculos.Remove(vehiculo);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}/kilometraje")]
        public async Task<IActionResult> ActualizarKilometraje(int id, [FromBody] int nuevoKilometraje)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo == null) return NotFound();

            vehiculo.KilometrajeActual = nuevoKilometraje;
            vehiculo.FechaUltimaActualizacionKm = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("alertas/pendientes")]
        public async Task<ActionResult<IEnumerable<AlertaServicioVehiculo>>> GetAlertasPendientes()
        {
            return await _context.AlertasServicioVehiculo
                .Where(a => !a.Atendida)
                .Include(a => a.Vehiculo)
                .ToListAsync();
        }

        [HttpPatch("alertas/{id}/atender")]
        public async Task<IActionResult> AtenderAlerta(int id)
        {
            var alerta = await _context.AlertasServicioVehiculo.FindAsync(id);
            if (alerta == null) return NotFound();

            alerta.Atendida = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool VehiculoExists(int id)
        {
            return _context.Vehiculos.Any(e => e.Id == id);
        }
    }
}
