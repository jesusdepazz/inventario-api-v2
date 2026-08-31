using Inventory.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class HojasResponsabilidadController : ControllerBase
{
    private static readonly System.Text.Json.JsonSerializerOptions SnapshotJsonOptions = new()
    {
        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
    };

    private readonly InventarioContext _context;

    public HojasResponsabilidadController(InventarioContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CrearHoja([FromBody] HojaResponsabilidadDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (dto.Empleados == null || !dto.Empleados.Any())
            return BadRequest(new { mensaje = "Debe agregar al menos un empleado." });

        if (dto.Equipos == null || !dto.Equipos.Any())
            return BadRequest(new { mensaje = "Debe agregar al menos un equipo." });

        int cantidadConMismoCorrelativo = await _context.HojasResponsabilidad
            .CountAsync(h => h.HojaNo == dto.HojaNo);

        if (cantidadConMismoCorrelativo >= 2)
            return BadRequest(new
            {
                mensaje = "Ya existen dos hojas con este Correlativo. No se puede crear una tercera."
            });

        var codigosEquipo = dto.Equipos
            .Where(eq => !string.IsNullOrWhiteSpace(eq.Codificacion))
            .Select(eq => eq.Codificacion)
            .ToList();

        var equiposEnOtraHoja = await _context.HojaEquipos
            .Where(eq => codigosEquipo.Contains(eq.Codificacion)
                      && eq.HojaResponsabilidad.Estado != "Inactiva")
            .Select(eq => eq.Codificacion)
            .ToListAsync();

        if (equiposEnOtraHoja.Any())
            return BadRequest(new
            {
                mensaje = "Los siguientes equipos ya están asignados a otra hoja: " +
                          string.Join(", ", equiposEnOtraHoja)
            });

        var hoja = new HojaResponsabilidad
        {
            TipoHoja = dto.TipoHoja,
            HojaNo = dto.HojaNo,
            Motivo = dto.Motivo,
            Comentarios = dto.Comentarios,
            FechaCreacion = DateTime.Now,
            Estado = dto.Estado,
            SolvenciaNo = dto.SolvenciaNo,
            FechaSolvencia = dto.FechaSolvencia,
            Observaciones = dto.Observaciones,
            Accesorios = dto.Accesorios,
            JefeInmediato = dto.JefeInmediato,
            Proyecto = dto.Proyecto,
            Version = 0,

            Empleados = dto.Empleados.Select(e => new HojaEmpleado
            {
                EmpleadoId = e.EmpleadoId,
                Nombre = e.Nombre,
                Puesto = e.Puesto,
                Departamento = e.Departamento
            }).ToList(),

            Equipos = dto.Equipos.Select(eq => new HojaEquipo
            {
                Codificacion = eq.Codificacion,
                Marca = eq.Marca,
                Modelo = eq.Modelo,
                Serie = eq.Serie,
                TipoEquipo = eq.TipoEquipo,
                Ubicacion = eq.Ubicacion,
                FechaIngreso = eq.FechaIngreso,
                Estado = eq.Estado,
                NumeroAsignado = eq.NumeroAsignado,
                Observaciones = eq.Observaciones,
                Extension = eq.Extension,
                Imei = eq.Imei,
                EquipoTipo = eq.EquipoTipo,
            }).ToList()
        };

        _context.HojasResponsabilidad.Add(hoja);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            mensaje = "Hoja creada correctamente",
            hoja.Id,
            hoja.HojaNo,
            hoja.Version
        });
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetHoja(int id)
    {
        var hoja = await _context.HojasResponsabilidad
            .Include(h => h.Empleados)
            .Include(h => h.Equipos)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (hoja == null)
            return NotFound();

        var dicFechas = (await _context.Asignaciones
            .Select(a => new { a.CodificacionEquipo, a.FechaAsignacion })
            .ToListAsync())
            .GroupBy(a => a.CodificacionEquipo)
            .ToDictionary(g => g.Key, g => g.Max(a => a.FechaAsignacion));

        return Ok(new
        {
            hoja.Id, hoja.Version, hoja.TipoHoja, hoja.HojaNo, hoja.Motivo,
            hoja.Comentarios, hoja.Estado, hoja.SolvenciaNo, hoja.FechaSolvencia,
            hoja.Observaciones, hoja.FechaCreacion, hoja.Accesorios,
            hoja.JefeInmediato, hoja.Proyecto,
            Empleados = hoja.Empleados,
            Equipos = hoja.Equipos.Select(eq => new
            {
                eq.Id, eq.Codificacion, eq.Marca, eq.Modelo, eq.Serie,
                eq.TipoEquipo, eq.Ubicacion, eq.FechaIngreso, eq.Estado,
                eq.EquipoTipo, eq.Extension, eq.NumeroAsignado, eq.Observaciones, eq.Imei,
                FechaAsignacion = dicFechas.TryGetValue(eq.Codificacion ?? "", out var fa) ? fa : hoja.FechaCreacion
            }).ToList()
        });
    }

    [HttpGet]
    public async Task<IActionResult> ListarHojas()
    {
        var hojas = (await _context.HojasResponsabilidad
            .Include(h => h.Empleados)
            .Include(h => h.Equipos)
            .ToListAsync())
            .OrderBy(h => int.TryParse(h.HojaNo, out var n) ? n : int.MaxValue)
            .ThenBy(h => h.HojaNo)
            .ToList();

        var dicFechas = (await _context.Asignaciones
            .Select(a => new { a.CodificacionEquipo, a.FechaAsignacion })
            .ToListAsync())
            .GroupBy(a => a.CodificacionEquipo)
            .ToDictionary(g => g.Key, g => g.Max(a => a.FechaAsignacion));

        var resultado = hojas.Select(h => new
        {
            h.Id, h.Version, h.TipoHoja, h.HojaNo, h.Motivo,
            h.Comentarios, h.Estado, h.SolvenciaNo, h.FechaSolvencia,
            h.Observaciones, h.FechaCreacion, h.Accesorios,
            h.JefeInmediato, h.Proyecto,
            Empleados = h.Empleados,
            Equipos = h.Equipos.Select(eq => new
            {
                eq.Id, eq.Codificacion, eq.Marca, eq.Modelo, eq.Serie,
                eq.TipoEquipo, eq.Ubicacion, eq.FechaIngreso, eq.Estado,
                eq.EquipoTipo, eq.Extension, eq.NumeroAsignado, eq.Observaciones, eq.Imei,
                FechaAsignacion = dicFechas.TryGetValue(eq.Codificacion ?? "", out var fa) ? fa : h.FechaCreacion
            }).ToList()
        }).ToList();

        return Ok(resultado);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarHoja(int id)
    {
        var hoja = await _context.HojasResponsabilidad
            .Include(h => h.Empleados)
            .Include(h => h.Equipos)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (hoja == null)
            return NotFound(new { mensaje = "No se encontró la hoja con el ID especificado." });

        _context.HojasResponsabilidad.Remove(hoja);
        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Hoja eliminada correctamente." });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> ActualizarHoja(int id, [FromBody] HojaResponsabilidadDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (dto.Empleados == null || !dto.Empleados.Any())
            return BadRequest(new { mensaje = "Debe agregar al menos un empleado." });

        if (dto.Equipos == null || !dto.Equipos.Any())
            return BadRequest(new { mensaje = "Debe agregar al menos un equipo." });

        var hoja = await _context.HojasResponsabilidad
            .Include(h => h.Empleados)
            .Include(h => h.Equipos)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (hoja == null)
            return NotFound(new { mensaje = "Hoja no encontrada." });

        if (hoja.HojaNo != dto.HojaNo)
        {
            bool existe = await _context.HojasResponsabilidad
                .AnyAsync(h => h.HojaNo == dto.HojaNo && h.Id != id);

            if (existe)
                return BadRequest(new { mensaje = "Ya existe una hoja con este Correlativo." });
        }

        var codigosEquipo = dto.Equipos
            .Where(e => !string.IsNullOrWhiteSpace(e.Codificacion))
            .Select(e => e.Codificacion)
            .ToList();

        var equiposEnOtraHoja = await _context.HojaEquipos
            .Where(eq =>
                codigosEquipo.Contains(eq.Codificacion) &&
                eq.HojaResponsabilidadId != id &&
                eq.HojaResponsabilidad.Estado != "Inactiva"
            )
            .Select(eq => eq.Codificacion)
            .ToListAsync();

        if (equiposEnOtraHoja.Any())
            return BadRequest(new
            {
                mensaje = "Los siguientes equipos ya están asignados a otra hoja: " +
                          string.Join(", ", equiposEnOtraHoja)
            });

        bool soloHojaNoChanged =
            hoja.HojaNo        != dto.HojaNo &&
            hoja.Motivo        == dto.Motivo &&
            hoja.Comentarios   == dto.Comentarios &&
            hoja.Estado        == dto.Estado &&
            hoja.SolvenciaNo   == dto.SolvenciaNo &&
            hoja.FechaSolvencia == dto.FechaSolvencia &&
            hoja.Observaciones == dto.Observaciones &&
            hoja.Accesorios    == dto.Accesorios &&
            hoja.JefeInmediato == dto.JefeInmediato &&
            hoja.Proyecto      == dto.Proyecto;

        // Before updating hoja fields, save current state as a version
        var snapshot = new
        {
            HojaNo = hoja.HojaNo,
            TipoHoja = hoja.TipoHoja,
            Motivo = hoja.Motivo,
            Observaciones = hoja.Observaciones,
            Comentarios = hoja.Comentarios,
            JefeInmediato = hoja.JefeInmediato,
            Estado = hoja.Estado,
            Accesorios = hoja.Accesorios,
            Proyecto = hoja.Proyecto,
            FechaCreacion = hoja.FechaCreacion,
            Version = hoja.Version,
            Empleados = hoja.Empleados.Select(e => new { e.EmpleadoId, e.Nombre, e.Puesto, e.Departamento }),
            Equipos = hoja.Equipos.Select(eq => new { eq.Codificacion, eq.Marca, eq.Modelo, eq.Serie, eq.TipoEquipo, eq.Ubicacion, eq.Estado, eq.Observaciones })
        };

        var version = new HojaResponsabilidadVersion
        {
            HojaResponsabilidadId = hoja.Id,
            NumeroVersion = hoja.Version,
            FechaGuardado = DateTime.Now,
            DatosJson = System.Text.Json.JsonSerializer.Serialize(snapshot, SnapshotJsonOptions)
        };
        _context.HojaResponsabilidadVersiones.Add(version);

        hoja.HojaNo = dto.HojaNo;
        hoja.TipoHoja = dto.TipoHoja;
        hoja.Motivo = dto.Motivo;
        hoja.Comentarios = dto.Comentarios;
        hoja.Estado = dto.Estado;
        hoja.SolvenciaNo = dto.SolvenciaNo;
        hoja.FechaSolvencia = dto.FechaSolvencia;
        hoja.Observaciones = dto.Observaciones;
        hoja.Accesorios = dto.Accesorios;
        hoja.JefeInmediato = dto.JefeInmediato;
        hoja.Proyecto = dto.Proyecto;

        hoja.Empleados.Clear();
        foreach (var e in dto.Empleados)
        {
            hoja.Empleados.Add(new HojaEmpleado
            {
                EmpleadoId = e.EmpleadoId,
                Nombre = e.Nombre,
                Puesto = e.Puesto,
                Departamento = e.Departamento
            });
        }

        hoja.Equipos.Clear();
        foreach (var eq in dto.Equipos)
        {
            hoja.Equipos.Add(new HojaEquipo
            {
                Codificacion   = eq.Codificacion,
                Marca          = eq.Marca,
                Modelo         = eq.Modelo,
                Serie          = eq.Serie,
                TipoEquipo     = eq.TipoEquipo,
                Ubicacion      = eq.Ubicacion,
                FechaIngreso   = eq.FechaIngreso,
                Estado         = eq.Estado,
                NumeroAsignado = eq.NumeroAsignado,
                Observaciones  = eq.Observaciones,
                Extension      = eq.Extension,
                Imei           = eq.Imei,
                EquipoTipo     = eq.EquipoTipo,
            });
        }

        if (!soloHojaNoChanged)
            hoja.Version += 1;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            mensaje = "Hoja actualizada correctamente",
            hoja.Id,
            hoja.HojaNo,
            hoja.Version
        });
    }

    [HttpGet("{id}/versiones")]
    public async Task<IActionResult> GetVersiones(int id)
    {
        var versiones = await _context.HojaResponsabilidadVersiones
            .Where(v => v.HojaResponsabilidadId == id)
            .OrderByDescending(v => v.NumeroVersion)
            .Select(v => new {
                v.Id,
                v.NumeroVersion,
                v.FechaGuardado,
                v.DatosJson
            })
            .ToListAsync();

        return Ok(versiones);
    }

    [HttpDelete("{id}/versiones/{versionId}")]
    public async Task<IActionResult> EliminarVersion(int id, int versionId)
    {
        var hoja = await _context.HojasResponsabilidad.FindAsync(id);
        if (hoja == null)
            return NotFound(new { mensaje = "No se encontró la hoja especificada." });

        var version = await _context.HojaResponsabilidadVersiones
            .FirstOrDefaultAsync(v => v.Id == versionId && v.HojaResponsabilidadId == id);

        if (version == null)
            return NotFound(new { mensaje = "No se encontró la versión especificada." });

        var numeroEliminado = version.NumeroVersion;

        _context.HojaResponsabilidadVersiones.Remove(version);

        // Renumerar las versiones posteriores para que queden consecutivas.
        var posteriores = await _context.HojaResponsabilidadVersiones
            .Where(v => v.HojaResponsabilidadId == id && v.NumeroVersion > numeroEliminado)
            .ToListAsync();

        foreach (var v in posteriores)
        {
            v.NumeroVersion -= 1;
        }

        if (hoja.Version > 0)
            hoja.Version -= 1;

        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Versión eliminada correctamente.", version = hoja.Version });
    }

}
