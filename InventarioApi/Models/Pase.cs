namespace InventarioApi.Models;

public class Pase
{
    public string No {  get; set; }
    public DateTime? FechaPase { get; set; }
    public string Solicitante { get; set; }
    public string DescripcionEquipo { get; set; }
    public string MotivoSalida { get; set; }
    public string Ubicacion {  get; set; }
    public DateTime? FechaRetorno { get; set; }
    public string Estado { get; set; }
    public string Razon { get; set; }

}
