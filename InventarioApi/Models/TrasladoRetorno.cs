using System.Text.Json.Serialization;

namespace InventarioApi.Models;

public class TrasladoRetorno
{
    public int Id { get; set; }
    public string No { get; set; }
    public DateTime FechaPase { get; set; }
    public string MotivoSalida { get; set; }
    public string UbicacionRetorno { get; set; }
    public string FechaRetorno { get; set; }
    public string? TipoRetiro { get; set; }
    public string Estado { get; set; } = "Vigente";
    public string? CodigoProveedor { get; set; }
    public string? TelefonoProveedor { get; set; }
    public string? PersonaRetira { get; set; }
    public string? NombreProveedor { get; set; }
    public string? NombreContacto { get; set; }
    public string? Identificacion { get; set; }

    public List<TrasladoRetornoEquipo> Equipos { get; set; } = new();
    public List<TrasladoRetornoEmpleado> Empleados { get; set; } = new();
}

public class TrasladoRetornoEquipo
{
    public int Id { get; set; }
    public int TrasladoRetornoId { get; set; }

    public string Equipo { get; set; }
    public string DescripcionEquipo { get; set; }
    public string Marca { get; set; }
    public string Modelo { get; set; }
    public string Serie { get; set; }

    [JsonIgnore]
    public TrasladoRetorno TrasladoRetorno { get; set; }
}

public class TrasladoRetornoEmpleado
{
    public int Id { get; set; }
    public int TrasladoRetornoId { get; set; }
    public string EmpleadoId { get; set; }
    public string Nombre { get; set; }
    public string Puesto { get; set; }
    public string Departamento { get; set; }

    [JsonIgnore]
    public TrasladoRetorno TrasladoRetorno { get; set; }
}