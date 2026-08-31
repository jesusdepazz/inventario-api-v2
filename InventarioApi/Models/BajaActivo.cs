namespace InventarioApi.Models;

public class BajaActivo
{
    public int Id { get; set; }
    public string? NumeroBaja { get; set; }
    public DateTime FechaBaja { get; set; }
    public string CodificacionEquipo { get; set; }
    public string MotivoBaja { get; set; }
    public string DetallesBaja { get; set; }
    public string UbicacionActual {  get; set; }
    public string UbicacionDestino { get; set; }

}
