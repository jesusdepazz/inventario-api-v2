namespace InventarioApi.Models;

public class HojaSolvencia
{
    public int Id { get; set; }
    public string SolvenciaNo { get; set; }
    public DateTime FechaSolvencia { get; set; }
    public string Observaciones { get; set; }
    public int HojaResponsabilidadId { get; set; }
    public HojaResponsabilidad HojaResponsabilidad { get; set; }
    public string JefeInmediato { get; set; }
    public string HojaNo { get; set; }
    public DateTime FechaHoja { get; set; }
    public string Empleados { get; set; }   
    public string Equipos { get; set; }    
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
}
