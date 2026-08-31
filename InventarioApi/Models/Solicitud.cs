namespace InventarioApi.Models;

public class Solicitud
{
    public int Id { get; set; }
    public string CodigoEmpleado { get; set; }
    public string NombreEmpleado { get; set; }
    public string Puesto { get; set; }
    public string Departamento { get; set; }
    public string Ubicacion { get; set; }
    public string JefeInmediato { get; set; }
    public string CodificacionEquipo { get; set; }
    public string Marca { get; set; }
    public string Modelo { get; set; }
    public string Serie { get; set; }
    public string Estado { get; set; } = "Pendiente";
    public string TipoSolicitud { get; set; }
    public string Correlativo { get; set; }

}
