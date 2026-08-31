using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inventory.Data;
using InventarioApi.Models.Suministros;
using ExcelDataReader;
using System.Text;
using OfficeOpenXml;

namespace InventarioApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuministrosController : ControllerBase
    {
        private readonly InventarioContext _context;

        public SuministrosController(InventarioContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Suministro>>> GetSuministros()
        {
            var suministros = await _context.Suministros
                .Include(s => s.Entradas)
                .Include(s => s.Salidas)
                .ToListAsync();

            return Ok(suministros);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Suministro>> GetSuministro(int id)
        {
            var suministro = await _context.Suministros
                .Include(s => s.Entradas)
                .Include(s => s.Salidas)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (suministro == null)
            {
                return NotFound();
            }

            return Ok(suministro);
        }

        [HttpPost]
        public async Task<ActionResult<Suministro>> CreateSuministro(Suministro suministro)
        {
            if (suministro == null)
                return BadRequest("Datos inválidos.");

            if (suministro.CantidadActual < 0)
                return BadRequest("La cantidad no puede ser negativa.");

            suministro.DateTime = DateTime.Now;

            _context.Suministros.Add(suministro);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSuministro), new { id = suministro.Id }, suministro);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSuministro(int id, Suministro suministro)
        {
            if (id != suministro.Id)
            {
                return BadRequest("Los ID no coinciden.");
            }

            var existing = await _context.Suministros.FindAsync(id);

            if (existing == null)
            {
                return NotFound();
            }

            existing.NombreProducto = suministro.NombreProducto;
            existing.UbicacionProducto = suministro.UbicacionProducto;
            existing.DateTime = DateTime.Now;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSuministro(int id)
        {
            var suministro = await _context.Suministros.FindAsync(id);

            if (suministro == null)
            {
                return NotFound();
            }

            _context.Suministros.Remove(suministro);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/recalcular")]
        public async Task<IActionResult> RecalcularInventario(int id)
        {
            var suministro = await _context.Suministros
                .Include(s => s.Entradas)
                .Include(s => s.Salidas)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (suministro == null)
            {
                return NotFound();
            }

            int entradas = suministro.Entradas.Sum(e => e.CantidadProducto);
            int salidas = suministro.Salidas.Sum(s => s.CantidadProducto);

            suministro.CantidadActual = entradas - salidas;

            await _context.SaveChangesAsync();
            return Ok(new { cantidadActual = suministro.CantidadActual });
        }

        [HttpPost("importar-excel")]
        public async Task<IActionResult> ImportarExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Debes subir un archivo Excel válido.");

            var suministros = new List<Suministro>();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using (var stream = file.OpenReadStream())
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var result = reader.AsDataSet();
                var table = result.Tables[0];

                for (int i = 1; i < table.Rows.Count; i++)
                {
                    var row = table.Rows[i];

                    var suministro = new Suministro
                    {
                        NombreProducto = row[0]?.ToString(),
                        UbicacionProducto = row[1]?.ToString(),
                        CantidadActual = int.TryParse(row[2]?.ToString(), out var cantidad) ? cantidad : 0
                    };

                    suministros.Add(suministro);
                }
            }

            await _context.Suministros.AddRangeAsync(suministros);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = $"{suministros.Count} suministros importados correctamente." });
        }

        [HttpDelete("todos")]
        public async Task<IActionResult> DeleteAllSuministros()
        {
            var salidas = _context.SalidaSuministros;
            _context.SalidaSuministros.RemoveRange(salidas);

            var entradas = _context.EntradaSuministros;
            _context.EntradaSuministros.RemoveRange(entradas);

            var suministros = _context.Suministros;
            _context.Suministros.RemoveRange(suministros);

            await _context.SaveChangesAsync();

            await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Suministros', RESEED, 0)");

            return Ok(new { mensaje = "Todos los suministros y sus movimientos fueron eliminados. ID reiniciado a 0." });
        }

        [HttpGet("con-totales")]
        public async Task<ActionResult<IEnumerable<object>>> GetSuministrosConTotales()
        {
            var data = await _context.Suministros
                .Select(s => new {
                    s.Id,
                    s.NombreProducto,
                    TotalEntradas = _context.EntradaSuministros
                                           .Where(e => e.SuministroId == s.Id)
                                           .Sum(e => (int?)e.CantidadProducto) ?? 0,
                    TotalSalidas = _context.SalidaSuministros
                                           .Where(sa => sa.SuministroId == s.Id)
                                           .Sum(sa => (int?)sa.CantidadProducto) ?? 0,
                    ExistenciaActual = s.CantidadActual
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("exportar-excel")]
        public async Task<IActionResult> ExportarExcel()
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                var suministros = await _context.Suministros.ToListAsync();
                var entradas = await _context.EntradaSuministros
                    .Include(e => e.Suministro)
                    .ToListAsync();
                var salidas = await _context.SalidaSuministros
                    .Include(s => s.Suministro)
                    .ToListAsync();

                using var package = new OfficeOpenXml.ExcelPackage();

                var ws1 = package.Workbook.Worksheets.Add("Suministros");
                ws1.Cells[1, 1].Value = "ID";
                ws1.Cells[1, 2].Value = "Producto";
                ws1.Cells[1, 3].Value = "Ubicación";
                ws1.Cells[1, 4].Value = "Cantidad Actual";

                int row = 2;
                foreach (var s in suministros)
                {
                    ws1.Cells[row, 1].Value = s.Id;
                    ws1.Cells[row, 2].Value = s.NombreProducto;
                    ws1.Cells[row, 3].Value = s.UbicacionProducto;
                    ws1.Cells[row, 4].Value = s.CantidadActual;
                    row++;
                }

                var ws2 = package.Workbook.Worksheets.Add("Entradas");
                ws2.Cells[1, 1].Value = "ID";
                ws2.Cells[1, 2].Value = "Producto";
                ws2.Cells[1, 3].Value = "Cantidad";
                ws2.Cells[1, 4].Value = "Fecha";

                row = 2;
                foreach (var e in entradas)
                {
                    ws2.Cells[row, 1].Value = e.Id;
                    ws2.Cells[row, 2].Value = e.Suministro?.NombreProducto;
                    ws2.Cells[row, 3].Value = e.CantidadProducto;
                    ws2.Cells[row, 4].Value = e.Fecha.ToString("dd/MM/yyyy");
                    row++;
                }

                var ws3 = package.Workbook.Worksheets.Add("Salidas");
                ws3.Cells[1, 1].Value = "ID";
                ws3.Cells[1, 2].Value = "Producto";
                ws3.Cells[1, 3].Value = "Cantidad";
                ws3.Cells[1, 4].Value = "Responsable";
                ws3.Cells[1, 5].Value = "Departamento";
                ws3.Cells[1, 6].Value = "Fecha";

                row = 2;
                foreach (var s in salidas)
                {
                    ws3.Cells[row, 1].Value = s.Id;
                    ws3.Cells[row, 2].Value = s.Suministro?.NombreProducto;
                    ws3.Cells[row, 3].Value = s.CantidadProducto;
                    ws3.Cells[row, 4].Value = s.PersonaResponsable;
                    ws3.Cells[row, 5].Value = s.DepartamentoResponsable;
                    ws3.Cells[row, 6].Value = s.Fecha.ToString("dd/MM/yyyy");
                    row++;
                }

                //ws1.Cells.AutoFitColumns();
                //ws2.Cells.AutoFitColumns();
                //ws3.Cells.AutoFitColumns();

                var excelBytes = package.GetAsByteArray();

                return File(
                    excelBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "InventarioSuministros.xlsx"
                );
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message,
                    stack = ex.StackTrace,
                    inner = ex.InnerException?.Message
                });
            }
        }
    }
}
