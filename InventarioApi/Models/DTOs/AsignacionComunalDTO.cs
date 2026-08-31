namespace InventarioApi.Models.DTOs;

public class CrearAsignacionComunalDTO
{
    public List<string> Codificaciones { get; set; } = new();
    public string Correlativo { get; set; }
    public string Ubicacion { get; set; }
    public string? Observaciones { get; set; }
}

public class ActualizarAsignacionComunalDTO
{
    public string Ubicacion { get; set; }
    public string? Observaciones { get; set; }
}

public class ActualizarGrupoAsignacionComunalDTO
{
    public string? Ubicacion { get; set; }
    public string? Observaciones { get; set; }
    public List<string>? AgregarCodificaciones { get; set; }
    public List<int>? QuitarIds { get; set; }
}
