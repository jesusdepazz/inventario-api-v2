using InventarioApi.Models;
using Inventory.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AsignacionesController : ControllerBase
{
    private readonly InventarioContext _context;

    public AsignacionesController(InventarioContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CrearAsignacion([FromBody] Asignacion asignacion)
    {
        _context.Asignaciones.Add(asignacion);

        if (!string.IsNullOrWhiteSpace(asignacion.Ubicacion))
        {
            var equipo = await _context.Equipos
                .FirstOrDefaultAsync(e => e.Codificacion == asignacion.CodificacionEquipo);

            if (equipo != null)
            {
                equipo.Ubicacion = asignacion.Ubicacion;
            }
        }

        await _context.SaveChangesAsync();

        return Ok(asignacion);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Asignacion>>> GetAsignaciones()
    {
        return await _context.Asignaciones
            .OrderByDescending(a => a.FechaAsignacion)
            .ToListAsync();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarAsignacion(int id)
    {
        var asignacion = await _context.Asignaciones.FindAsync(id);
        if (asignacion == null)
        {
            return NotFound();
        }

        _context.Asignaciones.Remove(asignacion);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("empleado/{codigoEmpleado}/equipos")]
    public async Task<IActionResult> ObtenerEquiposPorEmpleado(string codigoEmpleado)
    {
        var equipos = await (
            from a in _context.Asignaciones
            join e in _context.Equipos
                on a.CodificacionEquipo equals e.Codificacion
            where a.CodigoEmpleado == codigoEmpleado
            select new
            {
                e.Codificacion,
                e.Marca,
                e.Modelo,
                e.Serie,
                e.TipoEquipo,
                e.Ubicacion,
                e.FechaIngreso,
                e.Estado
            }
        ).ToListAsync();

        return Ok(equipos);
    }

}
