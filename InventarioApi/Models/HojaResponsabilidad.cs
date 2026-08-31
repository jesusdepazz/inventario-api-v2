using System.Text.Json.Serialization;
using InventarioApi.Models;

public class HojaResponsabilidad
{   
    public int Id { get; set; }
    public int Version { get; set; }
    public string? TipoHoja { get; set; }
    public string HojaNo { get; set; }
    public string Motivo { get; set; }
    public string? Comentarios { get; set; }
    public string Estado { get; set; }
    public string? SolvenciaNo { get; set; }
    public DateTime? FechaSolvencia { get; set; } = DateTime.Now;
    public string Observaciones { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public string? Accesorios { get; set; }
    public string JefeInmediato { get; set; }
    public string? Proyecto { get; set; }
    public List<HojaSolvencia> Solvencias { get; set; } = new();
    public List<HojaEmpleado> Empleados { get; set; } = new();
    public List<HojaEquipo> Equipos { get; set; } = new();
}

public class HojaEmpleado
{
    public int Id { get; set; }
    public int HojaResponsabilidadId { get; set; }
    public string EmpleadoId { get; set; } 
    public string Nombre { get; set; }
    public string Puesto { get; set; }
    public string Departamento { get; set; }

    [JsonIgnore]
    public HojaResponsabilidad HojaResponsabilidad { get; set; }
}

public class HojaEquipo
{
    public int Id { get; set; }
    public int HojaResponsabilidadId { get; set; }
    public string Codificacion { get; set; }
    public string Marca { get; set; }
    public string Modelo { get; set; }
    public string Serie { get; set; }
    public string TipoEquipo { get; set; }
    public string Ubicacion { get; set; }
    public string FechaIngreso { get; set; }
    public string Estado { get; set; }
    public string? EquipoTipo { get; set; }
    public string? Extension { get; set; }
    public string? NumeroAsignado { get; set; }
    public string? Observaciones { get; set; }
    public string? Imei { get; set; }

    [JsonIgnore]
    public HojaResponsabilidad HojaResponsabilidad { get; set; }
}
