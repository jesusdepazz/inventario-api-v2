using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryApi.Models;
using Inventory.Data;
using ExcelDataReader;

namespace InventoryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OtrosActivosController : ControllerBase
    {
        private readonly InventarioContext _context;
        private readonly IWebHostEnvironment _env;

        public OtrosActivosController(InventarioContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetEquipos()
        {
            var equiposConAsignaciones = await _context.Equipos
                .Select(e => new
                {
                    e.Id,
                    e.OrdenCompra,
                    e.Factura,
                    e.Proveedor,
                    e.FechaIngreso,
                    e.HojaNo,
                    e.FechaActualizacion,
                    e.ResponsableAnterior,
                    e.Codificacion,
                    e.Estado,
                    e.TipoEquipo,
                    e.Marca,
                    e.Modelo,
                    e.Serie,
                    e.Extension,
                    e.Ubicacion,
                    e.Comentarios,
                    e.Observaciones,

                    Asignaciones = _context.Asignaciones
                        .Where(a => a.CodificacionEquipo == e.Codificacion)
                        .Select(a => new {
                            a.CodigoEmpleado,
                            a.NombreEmpleado,
                            a.Puesto
                        })
                        .ToList()
                })
                .ToListAsync();

            return Ok(equiposConAsignaciones);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Equipo>> GetEquipo(int id)
        {
            var equipo = await _context.Equipos.FindAsync(id);

            if (equipo == null)
            {
                return NotFound();
            }

            return equipo;
        }

        [HttpGet("por-codificacion")]
        public async Task<IActionResult> ObtenerPorCodificacion(
            [FromQuery] string codificacion
        )
        {
            var equipo = await _context.Equipos
                .Where(e => e.Codificacion == codificacion)
                .Select(e => new EquipoDTO
                {
                    Id = e.Id,
                    Codificacion = e.Codificacion,
                    Marca = e.Marca,
                    Modelo = e.Modelo,
                    Serie = e.Serie,
                    Ubicacion = e.Ubicacion,
                    Estado = e.Estado,
                    TipoEquipo = e.TipoEquipo,
                    FechaIngreso = e.FechaIngreso
                })
                .FirstOrDefaultAsync();

            if (equipo == null)
                return NotFound("Equipo no encontrado");

            return Ok(equipo);
        }


        [HttpPost]
        public async Task<ActionResult<Equipo>> PostEquipo([FromForm] EquipoDTO dto)
        {
            var existeDuplicado = await _context.Equipos.AnyAsync(e =>
                e.Codificacion == dto.Codificacion ||
                (!string.IsNullOrEmpty(dto.Serie) && e.Serie == dto.Serie)
            );

            if (await _context.Equipos.AnyAsync(e => e.Codificacion == dto.Codificacion))
                return BadRequest("Ya existe un equipo con la misma Codificación.");

            if (!string.IsNullOrEmpty(dto.Serie) &&
                await _context.Equipos.AnyAsync(e => e.Serie == dto.Serie))
                return BadRequest("Ya existe un equipo con la misma Serie.");

            var equipo = new Equipo
            {
                OrdenCompra = dto.OrdenCompra,
                Factura = dto.Factura,
                Proveedor = dto.Proveedor,
                FechaIngreso = dto.FechaIngreso,
                HojaNo = dto.HojaNo,
                FechaActualizacion = dto.FechaActualizacion,
                ResponsableAnterior = dto.ResponsableAnterior,
                Codificacion = dto.Codificacion,
                Estado = dto.Estado,
                TipoEquipo = dto.TipoEquipo,
                Marca = dto.Marca,
                Modelo = dto.Modelo,
                Serie = dto.Serie,
                Extension = dto.Extension,
                Ubicacion = dto.Ubicacion,
                Comentarios = dto.Comentarios,
                Observaciones = dto.Observaciones,
            };

            _context.Equipos.Add(equipo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEquipo), new { id = equipo.Id }, equipo);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipo(int id)
        {
            var equipo = await _context.Equipos.FindAsync(id);
            if (equipo == null)
            {
                return NotFound();
            }

            _context.Equipos.Remove(equipo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditarEquipo(int id, [FromBody] EquipoDTO dto)
        {
            if (id != dto.Id)
                return BadRequest("ID inválido");

            var equipo = await _context.Equipos.FindAsync(id);
            if (equipo == null)
                return NotFound("Equipo no encontrado");

            Console.WriteLine($"DTO OrdenCompra: {dto.OrdenCompra}");
            Console.WriteLine($"ANTES DB OrdenCompra: {equipo.OrdenCompra}");

            equipo.OrdenCompra = dto.OrdenCompra;
            equipo.Marca = dto.Marca;
            equipo.Modelo = dto.Modelo;
            equipo.Serie = dto.Serie;
            equipo.Ubicacion = dto.Ubicacion;
            equipo.Estado = dto.Estado;
            equipo.FechaActualizacion = DateTime.UtcNow;

            _context.Entry(equipo).Property(e => e.OrdenCompra).IsModified = true;

            Console.WriteLine($"DESPUÉS Entity OrdenCompra: {equipo.OrdenCompra}");

            var cambios = await _context.SaveChangesAsync();

            var actualizado = await _context.Equipos
                .Where(e => e.Id == id)
                .Select(e => new
                {
                    e.Id,
                    e.OrdenCompra,
                    e.Marca,
                    e.Modelo,
                    e.Serie,
                    e.Ubicacion,
                    e.Estado
                })
                .FirstOrDefaultAsync();

            return Ok(new
            {
                cambios,
                actualizado
            });
        }

        [HttpPost("importar-excel")]
        public async Task<IActionResult> ImportarExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Debes subir un archivo Excel válido.");

            var equipos = new List<Equipo>();

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = file.OpenReadStream())
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var result = reader.AsDataSet();
                var table = result.Tables[0];

                for (int i = 1; i < table.Rows.Count; i++)
                {
                    var row = table.Rows[i];

                    var equipo = new Equipo
                    {
                        OrdenCompra = row[0]?.ToString(),
                        Factura = row[1]?.ToString(),
                        Proveedor = row[2]?.ToString(),
                        FechaIngreso = DateTime.TryParse(row[3]?.ToString(), out var fi) ? fi : DateTime.Now,
                        HojaNo = row[4]?.ToString(),
                        FechaActualizacion = DateTime.TryParse(row[5]?.ToString(), out var fa) ? fa : DateTime.Now,
                        Codificacion = row[6]?.ToString(),
                        Estado = row[7]?.ToString(),
                        TipoEquipo = row[8]?.ToString(),
                        Marca = row[9]?.ToString(),
                        Modelo = row[10]?.ToString(),
                        Serie = row[11]?.ToString(),
                        Extension = row[12]?.ToString(),
                        Ubicacion = row[13]?.ToString(),
                        ResponsableAnterior = row[14]?.ToString(),
                        Comentarios = row[15]?.ToString(),
                        Observaciones = row[16]?.ToString()
                    };

                    equipos.Add(equipo);
                }
            }

            await _context.Equipos.AddRangeAsync(equipos);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = $"{equipos.Count} equipos importados correctamente." });
        }

        [HttpDelete("EliminarTodos")]
        public async Task<IActionResult> EliminarTodosEquipos()
        {
            var equipos = _context.Equipos;
            _context.Equipos.RemoveRange(equipos);
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Todos los equipos fueron eliminados." });
        }

    }

}