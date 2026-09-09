using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventarioApi.Models;
using Inventory.Data;

namespace InventarioApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InmueblesController : ControllerBase
    {
        private readonly InventarioContext _context;
        private readonly IWebHostEnvironment _environment;

        public InmueblesController(InventarioContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Inmueble>>> GetInmuebles([FromQuery] string? estado)
        {
            var query = _context.Inmuebles
                .Include(i => i.Archivos)
                .Include(i => i.Polizas)
                .AsQueryable();

            if (!string.IsNullOrEmpty(estado))
            {
                query = query.Where(i => i.Estado.ToLower() == estado.ToLower());
            }

            return await query.ToListAsync();
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<Inmueble>> GetInmueble(int id)
        {
            var inmueble = await _context.Inmuebles
                .Include(i => i.Archivos)
                .Include(i => i.Polizas)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inmueble == null)
            {
                return NotFound(new { mensaje = "Inmueble no encontrado." });
            }

            return inmueble;
        }

        
        [HttpPost]
        public async Task<ActionResult<Inmueble>> PostInmueble(Inmueble inmueble)
        {
            _context.Inmuebles.Add(inmueble);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInmueble), new { id = inmueble.Id }, inmueble);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInmueble(int id, Inmueble inmueble)
        {
            if (id != inmueble.Id)
            {
                return BadRequest(new { mensaje = "El ID enviado no coincide con el registro." });
            }

            _context.Entry(inmueble).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InmuebleExists(id))
                {
                    return NotFound(new { mensaje = "Inmueble no encontrado." });
                }
                throw;
            }

            return NoContent();
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInmueble(int id)
        {
            var inmueble = await _context.Inmuebles.FindAsync(id);
            if (inmueble == null)
            {
                return NotFound(new { mensaje = "Inmueble no encontrado." });
            }

            _context.Inmuebles.Remove(inmueble);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        
        [HttpPost("{id}/polizas")]
        public async Task<ActionResult<PolizaSeguro>> PostPoliza(int id, PolizaSeguro poliza)
        {
            var inmueble = await _context.Inmuebles.FindAsync(id);
            if (inmueble == null)
            {
                return NotFound(new { mensaje = "El inmueble no existe." });
            }

            poliza.InmuebleId = id;
            _context.PolizasSeguro.Add(poliza);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInmueble), new { id = id }, poliza);
        }

        
        [HttpPost("{id}/archivos")]
        public async Task<ActionResult<InmuebleArchivo>> UploadArchivo(int id, IFormFile archivo, [FromForm] string tipoDocumento)
        {
            var inmueble = await _context.Inmuebles.FindAsync(id);
            if (inmueble == null)
            {
                return NotFound(new { mensaje = "El inmueble no existe." });
            }

            if (archivo == null || archivo.Length == 0)
            {
                return BadRequest(new { mensaje = "No se ha proporcionado un archivo válido." });
            }

            
            var uploadsFolder = Path.Combine(_environment.ContentRootPath, "Uploads", "Inmuebles");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            
            var fileName = $"{Guid.NewGuid()}_{archivo.FileName}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            var inmuebleArchivo = new InmuebleArchivo
            {
                InmuebleId = id,
                NombreOriginal = archivo.FileName,
                RutaArchivo = Path.Combine("Uploads", "Inmuebles", fileName),
                TipoDocumento = string.IsNullOrEmpty(tipoDocumento) ? "documento" : tipoDocumento,
                FechaSubida = DateTime.Now
            };

            _context.InmuebleArchivos.Add(inmuebleArchivo);
            await _context.SaveChangesAsync();

            return Ok(inmuebleArchivo);
        }

        
        [HttpGet("archivos/{archivoId}/descargar")]
        public async Task<IActionResult> DownloadArchivo(int archivoId)
        {
            var archivoRecord = await _context.InmuebleArchivos.FindAsync(archivoId);
            if (archivoRecord == null)
            {
                return NotFound(new { mensaje = "El registro de archivo no existe." });
            }

            var filePath = Path.Combine(_environment.ContentRootPath, archivoRecord.RutaArchivo);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new { mensaje = "El archivo físico no se encuentra en el servidor." });
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(bytes, "application/octet-stream", archivoRecord.NombreOriginal);
        }

        private bool InmuebleExists(int id)
        {
            return _context.Inmuebles.Any(e => e.Id == id);
        }
    }
}