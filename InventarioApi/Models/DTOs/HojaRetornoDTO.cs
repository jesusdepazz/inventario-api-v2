namespace InventarioApi.Models.DTOs;

public class TrasladoRetornoDTO
{
    public string No { get; set; }
    public DateTime FechaPase { get; set; }
    public string MotivoSalida { get; set; }
    public string UbicacionRetorno { get; set; }
    public string FechaRetorno { get; set; }
    public string? TipoRetiro { get; set; }   // "proveedor" | "empleado"
    public string? CodigoProveedor { get; set; }
    public string? TelefonoProveedor { get; set; }
    public string? PersonaRetira { get; set; }
    public string? NombreProveedor { get; set; }
    public string? NombreContacto { get; set; }
    public string? Identificacion { get; set; }

    public List<TrasladoRetornoEquipoDTO> Equipos { get; set; } = new();
    public List<TrasladoRetornoEmpleadoDTO> Empleados { get; set; } = new();
}

public class TrasladoRetornoEquipoDTO
{
    public string Equipo { get; set; }
    public string DescripcionEquipo { get; set; }
    public string Marca { get; set; }
    public string Modelo { get; set; }
    public string Serie { get; set; }
}

public class TrasladoRetornoEmpleadoDTO
{
    public string EmpleadoId { get; set; }
    public string Nombre { get; set; }
    public string Puesto { get; set; }
    public string Departamento { get; set; }
}