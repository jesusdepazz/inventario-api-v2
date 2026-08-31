using System.Text.Json.Serialization;

public class HojaResponsabilidadVersion
{
    public int Id { get; set; }
    public int HojaResponsabilidadId { get; set; }
    public int NumeroVersion { get; set; }
    public DateTime FechaGuardado { get; set; } = DateTime.Now;
    public string DatosJson { get; set; } = "";

    [JsonIgnore]
    public HojaResponsabilidad? HojaResponsabilidad { get; set; }
}
