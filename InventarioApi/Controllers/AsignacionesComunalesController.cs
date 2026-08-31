using InventarioApi.Models;
using InventarioApi.Models.DTOs;
using Inventory.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AsignacionesComunalesController : ControllerBase
{
    private static readonly System.Text.Json.JsonSerializerOptions SnapshotJsonOptions = new()
    {
        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
    };

    private readonly InventarioContext _context;

    public AsignacionesComunalesController(InventarioContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CrearAsignacionComunal([FromBody] CrearAsignacionComunalDTO dto)
    {
        if (dto.Codificaciones == null || !dto.Codificaciones.Any())
            return BadRequest("Debe agregar al menos un equipo.");

        if (string.IsNullOrWhiteSpace(dto.Ubicacion))
            return BadRequest("Debe seleccionar una ubicación.");

        if (string.IsNullOrWhiteSpace(dto.Correlativo))
            return BadRequest("Debe ingresar el correlativo.");

        bool correlativoExiste = await _context.AsignacionesComunales
            .AnyAsync(a => a.Correlativo == dto.Correlativo);

        if (correlativoExiste)
            return BadRequest("Ya existe una asignación comunal con este correlativo.");

        var codigos = dto.Codificaciones.Distinct().ToList();

        var equipos = await _context.Equipos
            .Where(e => codigos.Contains(e.Codificacion))
            .ToListAsync();

        var noExisten = codigos.Except(equipos.Select(e => e.Codificacion)).ToList();
        if (noExisten.Any())
            return BadRequest($"Equipos no existen: {string.Join(", ", noExisten)}");

        var loteId = Guid.NewGuid();

        var nuevas = codigos.Select(cod => new AsignacionComunal
        {
            LoteId = loteId,
            Correlativo = dto.Correlativo,
            CodificacionEquipo = cod,
            Ubicacion = dto.Ubicacion,
            Observaciones = dto.Observaciones,
            FechaAsignacion = DateTime.UtcNow
        }).ToList();

        _context.AsignacionesComunales.AddRange(nuevas);

        foreach (var equipo in equipos)
        {
            equipo.Ubicacion = dto.Ubicacion;
        }

        await _context.SaveChangesAsync();

        return Ok(nuevas);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AsignacionComunal>>> GetAsignacionesComunales()
    {
        return await _context.AsignacionesComunales
            .OrderByDescending(a => a.FechaAsignacion)
            .ToListAsync();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> ActualizarAsignacionComunal(int id, [FromBody] ActualizarAsignacionComunalDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Ubicacion))
            return BadRequest("Debe seleccionar una ubicación.");

        var asignacion = await _context.AsignacionesComunales.FindAsync(id);
        if (asignacion == null)
            return NotFound();

        // Antes de sobrescribir, guardamos el estado actual como versión histórica.
        var snapshot = new
        {
            asignacion.CodificacionEquipo,
            asignacion.Ubicacion,
            asignacion.Observaciones,
            asignacion.FechaAsignacion,
            asignacion.FechaActualizacion,
            asignacion.Version
        };

        var version = new AsignacionComunalVersion
        {
            AsignacionComunalId = asignacion.Id,
            NumeroVersion = asignacion.Version,
            FechaGuardado = DateTime.UtcNow,
            DatosJson = System.Text.Json.JsonSerializer.Serialize(snapshot, SnapshotJsonOptions)
        };
        _context.AsignacionComunalVersiones.Add(version);

        asignacion.Ubicacion = dto.Ubicacion;
        asignacion.Observaciones = dto.Observaciones;
        asignacion.FechaActualizacion = DateTime.UtcNow;
        asignacion.Version += 1;

        var equipo = await _context.Equipos
            .FirstOrDefaultAsync(e => e.Codificacion == asignacion.CodificacionEquipo);

        if (equipo != null)
        {
            equipo.Ubicacion = dto.Ubicacion;
        }

        await _context.SaveChangesAsync();

        return Ok(asignacion);
    }

    [HttpGet("grupo/{id}")]
    public async Task<IActionResult> GetGrupo(int id)
    {
        var referencia = await _context.AsignacionesComunales.FindAsync(id);
        if (referencia == null)
            return NotFound();

        var grupo = referencia.LoteId.HasValue
            ? await _context.AsignacionesComunales
                .Where(a => a.LoteId == referencia.LoteId)
                .OrderBy(a => a.CodificacionEquipo)
                .ToListAsync()
            : new List<AsignacionComunal> { referencia };

        return Ok(grupo);
    }

    [HttpPut("grupo/{id}")]
    public async Task<IActionResult> ActualizarGrupo(int id, [FromBody] ActualizarGrupoAsignacionComunalDTO dto)
    {
        var referencia = await _context.AsignacionesComunales.FindAsync(id);
        if (referencia == null)
            return NotFound();

        var loteId = referencia.LoteId;

        var grupo = loteId.HasValue
            ? await _context.AsignacionesComunales.Where(a => a.LoteId == loteId).ToListAsync()
            : new List<AsignacionComunal> { referencia };

        if (dto.QuitarIds != null && dto.QuitarIds.Any())
        {
            var aQuitar = grupo.Where(a => dto.QuitarIds.Contains(a.Id)).ToList();

            var quedanNuevos = dto.AgregarCodificaciones?.Any() == true;
            if (aQuitar.Count == grupo.Count && !quedanNuevos)
                return BadRequest("Debe quedar al menos un equipo en la asignación.");

            _context.AsignacionesComunales.RemoveRange(aQuitar);
            grupo = grupo.Except(aQuitar).ToList();
        }

        var ubicacionFinal = !string.IsNullOrWhiteSpace(dto.Ubicacion) ? dto.Ubicacion : null;

        foreach (var asignacion in grupo)
        {
            var cambioUbicacion = ubicacionFinal != null && ubicacionFinal != asignacion.Ubicacion;
            var cambioObservaciones = dto.Observaciones != null && dto.Observaciones != asignacion.Observaciones;

            if (!cambioUbicacion && !cambioObservaciones)
                continue;

            var snapshot = new
            {
                asignacion.CodificacionEquipo,
                asignacion.Ubicacion,
                asignacion.Observaciones,
                asignacion.FechaAsignacion,
                asignacion.FechaActualizacion,
                asignacion.Version
            };

            _context.AsignacionComunalVersiones.Add(new AsignacionComunalVersion
            {
                AsignacionComunalId = asignacion.Id,
                NumeroVersion = asignacion.Version,
                FechaGuardado = DateTime.UtcNow,
                DatosJson = System.Text.Json.JsonSerializer.Serialize(snapshot, SnapshotJsonOptions)
            });

            if (cambioUbicacion) asignacion.Ubicacion = ubicacionFinal;
            if (cambioObservaciones) asignacion.Observaciones = dto.Observaciones;
            asignacion.FechaActualizacion = DateTime.UtcNow;
            asignacion.Version += 1;

            if (cambioUbicacion)
            {
                var equipoExistente = await _context.Equipos
                    .FirstOrDefaultAsync(e => e.Codificacion == asignacion.CodificacionEquipo);

                if (equipoExistente != null)
                    equipoExistente.Ubicacion = ubicacionFinal;
            }
        }

        if (dto.AgregarCodificaciones != null && dto.AgregarCodificaciones.Any())
        {
            var nuevosCodigos = dto.AgregarCodificaciones
                .Distinct()
                .Where(c => !grupo.Any(a => a.CodificacionEquipo == c))
                .ToList();

            if (nuevosCodigos.Any())
            {
                var equiposNuevos = await _context.Equipos
                    .Where(e => nuevosCodigos.Contains(e.Codificacion))
                    .ToListAsync();

                var noExisten = nuevosCodigos.Except(equiposNuevos.Select(e => e.Codificacion)).ToList();
                if (noExisten.Any())
                    return BadRequest($"Equipos no existen: {string.Join(", ", noExisten)}");

                var ubicacionParaNuevos = ubicacionFinal ?? grupo.FirstOrDefault()?.Ubicacion ?? referencia.Ubicacion;
                var observacionesParaNuevos = dto.Observaciones ?? grupo.FirstOrDefault()?.Observaciones;
                var correlativoParaNuevos = grupo.FirstOrDefault()?.Correlativo ?? referencia.Correlativo;

                if (!loteId.HasValue)
                {
                    loteId = Guid.NewGuid();
                    if (grupo.Any(a => a.Id == referencia.Id))
                        referencia.LoteId = loteId;
                }

                var nuevasFilas = equiposNuevos.Select(eq => new AsignacionComunal
                {
                    LoteId = loteId,
                    Correlativo = correlativoParaNuevos,
                    CodificacionEquipo = eq.Codificacion,
                    Ubicacion = ubicacionParaNuevos,
                    Observaciones = observacionesParaNuevos,
                    FechaAsignacion = DateTime.UtcNow
                }).ToList();

                _context.AsignacionesComunales.AddRange(nuevasFilas);

                foreach (var eq in equiposNuevos)
                    eq.Ubicacion = ubicacionParaNuevos;
            }
        }

        await _context.SaveChangesAsync();

        var resultado = loteId.HasValue
            ? await _context.AsignacionesComunales.Where(a => a.LoteId == loteId).ToListAsync()
            : await _context.AsignacionesComunales.Where(a => a.Id == referencia.Id).ToListAsync();

        return Ok(resultado);
    }

    [HttpGet("{id}/versiones")]
    public async Task<IActionResult> GetVersiones(int id)
    {
        var versiones = await _context.AsignacionComunalVersiones
            .Where(v => v.AsignacionComunalId == id)
            .OrderByDescending(v => v.NumeroVersion)
            .Select(v => new
            {
                v.Id,
                v.NumeroVersion,
                v.FechaGuardado,
                v.DatosJson
            })
            .ToListAsync();

        return Ok(versiones);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarAsignacionComunal(int id)
    {
        var asignacion = await _context.AsignacionesComunales.FindAsync(id);
        if (asignacion == null)
        {
            return NotFound();
        }

        _context.AsignacionesComunales.Remove(asignacion);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
