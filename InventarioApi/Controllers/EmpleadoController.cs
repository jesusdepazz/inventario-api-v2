using Microsoft.AspNetCore.Mvc;
using Inventory.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpleadosController : ControllerBase
    {
        private readonly InventarioContext _context;

        public EmpleadosController(InventarioContext context)
        {
            _context = context;
        }

        private async Task<string?> ObtenerIdCCosto(string empleadoCodigo)
        {
            try
            {
                var conn = _context.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open)
                    await conn.OpenAsync();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT TOP 1 idccosto FROM [SoftlandCA].[GUANDY].[empleados] WHERE idnumero = @emp";
                var param = cmd.CreateParameter();
                param.ParameterName = "@emp";
                param.Value = empleadoCodigo;
                cmd.Parameters.Add(param);

                var result = await cmd.ExecuteScalarAsync();
                if (result != null && result != DBNull.Value && !string.IsNullOrWhiteSpace(result.ToString()))
                    return result.ToString();
            }
            catch { }

            return null;
        }
        private async Task<string> ObtenerPuesto(string? idccosto, string? puestoFallback)
        {
            if (string.IsNullOrWhiteSpace(idccosto))
                return puestoFallback ?? "";

            try
            {
                var conn = _context.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open)
                    await conn.OpenAsync();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT TOP 1 descripcion FROM [SoftlandCA].[GUANDY].[ph_ccostos] WHERE idccosto = @id";
                var param = cmd.CreateParameter();
                param.ParameterName = "@id";
                param.Value = idccosto;
                cmd.Parameters.Add(param);

                var result = await cmd.ExecuteScalarAsync();
                if (result != null && result != DBNull.Value && !string.IsNullOrWhiteSpace(result.ToString()))
                    return result.ToString()!;
            }
            catch
            {
               
            }

            return puestoFallback ?? "";
        }

        private async Task<Dictionary<string, string>> ObtenerIdCCostosBatch(IEnumerable<string> codigos)
        {
            var lista = codigos.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
            var dic = new Dictionary<string, string>();

            if (!lista.Any()) return dic;

            try
            {
                var conn = _context.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open)
                    await conn.OpenAsync();

                using var cmd = conn.CreateCommand();
                var placeholders = string.Join(",", lista.Select((_, i) => $"@e{i}"));
                cmd.CommandText = $"SELECT idnumero, idccosto FROM [SoftlandCA].[GUANDY].[empleados] WHERE idnumero IN ({placeholders})";

                for (int i = 0; i < lista.Count; i++)
                {
                    var p = cmd.CreateParameter();
                    p.ParameterName = $"@e{i}";
                    p.Value = lista[i];
                    cmd.Parameters.Add(p);
                }

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var emp = reader.IsDBNull(0) ? "" : reader.GetString(0);
                    var cc  = reader.IsDBNull(1) ? "" : reader.GetString(1);
                    if (!string.IsNullOrWhiteSpace(emp))
                        dic[emp] = cc;
                }
            }
            catch { }

            return dic;
        }

        private async Task<Dictionary<string, string>> ObtenerPuestosBatch(IEnumerable<string> ids)
        {
            var lista = ids.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
            var dic = new Dictionary<string, string>();

            if (!lista.Any()) return dic;

            try
            {
                var conn = _context.Database.GetDbConnection();
                if (conn.State != System.Data.ConnectionState.Open)
                    await conn.OpenAsync();

                using var cmd = conn.CreateCommand();
                var placeholders = string.Join(",", lista.Select((_, i) => $"@p{i}"));
                cmd.CommandText = $"SELECT idccosto, descripcion FROM [SoftlandCA].[GUANDY].[ph_ccostos] WHERE idccosto IN ({placeholders})";

                for (int i = 0; i < lista.Count; i++)
                {
                    var p = cmd.CreateParameter();
                    p.ParameterName = $"@p{i}";
                    p.Value = lista[i];
                    cmd.Parameters.Add(p);
                }

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var key = reader.IsDBNull(0) ? "" : reader.GetString(0);
                    var val = reader.IsDBNull(1) ? "" : reader.GetString(1);
                    if (!string.IsNullOrWhiteSpace(key))
                        dic[key] = val;
                }
            }
            catch
            {
                
            }

            return dic;
        }


        [HttpGet("{codigo}")]
        public async Task<ActionResult<object>> GetEmpleadoPorCodigo(string codigo)
        {
            var codigoNorm = codigo.Trim().ToUpper();

            var empRow = await _context.EmpleadosInfo
                .Where(e => e.Empleado.Trim().ToUpper() == codigoNorm)
                .Select(e => new { e.Empleado, e.Nombre, e.Puesto, e.Departamento })
                .FirstOrDefaultAsync();

            if (empRow != null)
            {
                var idccosto = await ObtenerIdCCosto(empRow.Empleado);
                var puesto   = await ObtenerPuesto(idccosto, empRow.Puesto);

                var departamento = await _context.Departamentos
                    .Where(d => d.Codigo == empRow.Departamento)
                    .Select(d => d.Descripcion ?? "")
                    .FirstOrDefaultAsync() ?? "";

                return Ok(new
                {
                    codigoEmpleado = empRow.Empleado ?? "",
                    nombre         = empRow.Nombre   ?? "",
                    puesto,
                    departamento
                });
            }

            var asignacion = await _context.Asignaciones
                .Where(a => a.CodigoEmpleado.Trim().ToUpper() == codigoNorm)
                .Select(a => new
                {
                    codigoEmpleado = a.CodigoEmpleado,
                    nombre         = a.NombreEmpleado,
                    puesto         = a.Puesto,
                    departamento   = a.Departamento ?? ""
                })
                .FirstOrDefaultAsync();

            if (asignacion != null)
                return Ok(asignacion);

            var hojaEmp = await _context.HojaEmpleados
                .Where(h => h.EmpleadoId.Trim().ToUpper() == codigoNorm)
                .Select(h => new
                {
                    codigoEmpleado = h.EmpleadoId,
                    nombre         = h.Nombre,
                    puesto         = h.Puesto,
                    departamento   = h.Departamento ?? ""
                })
                .FirstOrDefaultAsync();

            if (hojaEmp != null)
                return Ok(hojaEmp);

            return NotFound(new { mensaje = $"No se encontró el empleado con código '{codigo}'." });
        }

[HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<object>>> BuscarEmpleados([FromQuery] string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return Ok(new List<object>());

            var rows = await _context.EmpleadosInfo
                .Where(e => e.Nombre != null && e.Nombre.Contains(nombre))
                .Select(e => new { e.Empleado, e.Nombre, e.Puesto, e.Departamento })
                .ToListAsync();

            var dicIdCCosto = await ObtenerIdCCostosBatch(rows.Select(e => e.Empleado!));

            var dicPuestos = await ObtenerPuestosBatch(dicIdCCosto.Values);

            var empleados = rows.Select(e =>
            {
                dicIdCCosto.TryGetValue(e.Empleado!, out var idcc);
                string puesto = (!string.IsNullOrWhiteSpace(idcc) &&
                                 dicPuestos.TryGetValue(idcc, out var desc) &&
                                 !string.IsNullOrWhiteSpace(desc))
                                ? desc
                                : (e.Puesto ?? "");

                return new
                {
                    codigoEmpleado = e.Empleado    ?? "",
                    nombre         = e.Nombre      ?? "",
                    puesto,
                    departamento   = _context.Departamentos
                        .Where(d => d.Codigo == e.Departamento)
                        .Select(d => d.Descripcion ?? "")
                        .FirstOrDefault() ?? ""
                };
            }).ToList();

            return Ok(empleados);
        }
    }
}
