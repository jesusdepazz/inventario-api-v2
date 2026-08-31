using InventarioApi.Models;
using Inventory.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HojaSolvenciasController : ControllerBase
    {
        private readonly InventarioContext _context;

        public HojaSolvenciasController(InventarioContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<HojaSolvencia>> CrearSolvencia(int hojaResponsabilidadId, string observaciones, string solvenciaNo)
        {
            if (string.IsNullOrWhiteSpace(solvenciaNo))
                return BadRequest("Debe enviar el número de solvencia.");

            var hojaResp = await _context.HojasResponsabilidad
                .Include(h => h.Empleados)
                .Include(h => h.Equipos)
                .FirstOrDefaultAsync(h => h.Id == hojaResponsabilidadId);

            if (hojaResp == null)
                return BadRequest("No se encontró la Hoja de Responsabilidad.");

            var empleados = string.Join(", ", hojaResp.Empleados
                .Select(e => $"{e.EmpleadoId} - {e.Nombre} - {e.Puesto} - {e.Departamento}"));

            var equipos = string.Join(", ", hojaResp.Equipos
                .Select(eq => $"{eq.Codificacion} {eq.Marca} {eq.Modelo} ({eq.Ubicacion})"));

            var solvencia = new HojaSolvencia
            {
                SolvenciaNo = solvenciaNo,
                FechaSolvencia = DateTime.Now,
                Observaciones = observaciones,
                HojaResponsabilidadId = hojaResp.Id,
                HojaNo = hojaResp.HojaNo,
                FechaHoja = hojaResp.FechaCreacion,
                Empleados = empleados,
                Equipos = equipos,
                JefeInmediato = hojaResp.JefeInmediato,
                FechaRegistro = DateTime.Now
            };

            _context.Solvencias.Add(solvencia);

            hojaResp.Estado = "Inactiva";

            var codificaciones = hojaResp.Equipos.Select(e => e.Codificacion).ToList();

            foreach (var cod in codificaciones)
            {
                var equipoInv = await _context.Equipos
                    .FirstOrDefaultAsync(e => e.Codificacion == cod);

                if (equipoInv != null)
                {
                    var ultimaAsignacion = await _context.Asignaciones
                        .Where(a => a.CodificacionEquipo == equipoInv.Codificacion)
                        .OrderByDescending(a => a.Id)
                        .FirstOrDefaultAsync();

                    equipoInv.ResponsableAnterior =
                        ultimaAsignacion?.NombreEmpleado ?? "Ninguno";

                    var asignacionesEquipo = _context.Asignaciones
                        .Where(a => a.CodificacionEquipo == equipoInv.Codificacion);

                    _context.Asignaciones.RemoveRange(asignacionesEquipo);

                    equipoInv.Ubicacion = "Stock";
                    equipoInv.FechaActualizacion = DateTime.Now;
                }
            }

            await _context.SaveChangesAsync();

            return Ok(solvencia);
        }

        [HttpGet("historico")]
        public async Task<ActionResult<IEnumerable<object>>> GetHistorico()
        {
            var historico = await _context.Solvencias
                .Include(s => s.HojaResponsabilidad)
                .Select(s => new
                {
                    s.Id,
                    s.SolvenciaNo,
                    s.FechaSolvencia,
                    s.Empleados,
                    s.Equipos,
                    s.HojaNo,
                    s.JefeInmediato,
                    s.Observaciones
                })
                .ToListAsync();

            return Ok(historico);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarSolvencia(int id)
        {
            var solvencia = await _context.Solvencias.FindAsync(id);
            if (solvencia == null)
            {
                return NotFound("No se encontró la solvencia.");
            }

            _context.Solvencias.Remove(solvencia);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Solvencia eliminada correctamente." });
        }


    }
}
