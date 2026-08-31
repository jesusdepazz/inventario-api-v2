namespace InventarioApi.Models;

public class Asignacion
{
    public int Id { get; set; }
    public string CodigoEmpleado { get; set; }
    public string NombreEmpleado { get; set; }
    public string Puesto { get; set; }
    public string Departamento { get; set; }
    public string CodificacionEquipo { get; set; }
    public string? Ubicacion { get; set; }
    public DateTime FechaAsignacion { get; set; } = DateTime.UtcNow;
}

