using System.Text.Json.Serialization;

namespace InventarioApi.Models;

public class AsignacionComunal
{
    public int Id { get; set; }
    public Guid? LoteId { get; set; }
    public string? Correlativo { get; set; }
    public string CodificacionEquipo { get; set; }
    public string Ubicacion { get; set; }
    public DateTime FechaAsignacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaActualizacion { get; set; }
    public string? Observaciones { get; set; }
    public int Version { get; set; } = 0;
}

public class AsignacionComunalVersion
{
    public int Id { get; set; }
    public int AsignacionComunalId { get; set; }
    public int NumeroVersion { get; set; }
    public DateTime FechaGuardado { get; set; }
    public string DatosJson { get; set; }

    [JsonIgnore]
    public AsignacionComunal AsignacionComunal { get; set; }
}
