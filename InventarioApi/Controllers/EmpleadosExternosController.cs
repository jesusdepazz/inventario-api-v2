using Inventory.Data;
using InventarioApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmpleadosExternosController : ControllerBase
{
    private readonly InventarioContext _context;

    public EmpleadosExternosController(InventarioContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var externos = await _context.EmpleadosExternos
            .Where(e => e.Activo)
            .OrderBy(e => e.Nombre)
            .ToListAsync();

        return Ok(externos);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obtener(int id)
    {
        var externo = await _context.EmpleadosExternos.FindAsync(id);

        if (externo == null)
            return NotFound(new { mensaje = "Empleado externo no encontrado." });

        return Ok(externo);
    }

    [HttpGet("codigo/{codigo}")]
    public async Task<IActionResult> ObtenerPorCodigo(string codigo)
    {
        var externo = await _context.EmpleadosExternos
            .FirstOrDefaultAsync(e => e.CodigoEmpleado == codigo && e.Activo);

        if (externo == null)
            return NotFound(new { mensaje = "Empleado externo no encontrado." });

        return Ok(externo);
    }

    [HttpGet("buscar")]
    public async Task<IActionResult> Buscar([FromQuery] string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return Ok(new List<object>());

        var externos = await _context.EmpleadosExternos
            .Where(e => e.Activo && e.Nombre.Contains(nombre))
            .OrderBy(e => e.Nombre)
            .ToListAsync();

        return Ok(externos);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] EmpleadoExterno dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        bool codigoExiste = await _context.EmpleadosExternos
            .AnyAsync(e => e.CodigoEmpleado == dto.CodigoEmpleado);

        if (codigoExiste)
            return BadRequest(new { mensaje = "Ya existe un empleado externo con ese código." });

        dto.FechaRegistro = DateTime.Now;
        dto.Activo = true;

        _context.EmpleadosExternos.Add(dto);
        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Empleado externo creado correctamente.", dto.Id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] EmpleadoExterno dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var externo = await _context.EmpleadosExternos.FindAsync(id);

        if (externo == null)
            return NotFound(new { mensaje = "Empleado externo no encontrado." });

        bool codigoDuplicado = await _context.EmpleadosExternos
            .AnyAsync(e => e.CodigoEmpleado == dto.CodigoEmpleado && e.Id != id);

        if (codigoDuplicado)
            return BadRequest(new { mensaje = "Ya existe otro empleado externo con ese código." });

        externo.CodigoEmpleado = dto.CodigoEmpleado;
        externo.Nombre = dto.Nombre;
        externo.Puesto = dto.Puesto;
        externo.Documento = dto.Documento;
        externo.Telefono = dto.Telefono;

        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Empleado externo actualizado correctamente." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Desactivar(int id)
    {
        var externo = await _context.EmpleadosExternos.FindAsync(id);

        if (externo == null)
            return NotFound(new { mensaje = "Empleado externo no encontrado." });

        externo.Activo = false;
        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Empleado externo desactivado correctamente." });
    }
}
