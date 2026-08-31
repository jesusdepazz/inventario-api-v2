using System.ComponentModel.DataAnnotations;

namespace InventarioApi.Models;

public class Mantenimiento
{
    public int Id { get; set; }

    [Required]
    public string Codificacion { get; set; }
    public string Modelo { get; set; }

    [Required]
    public string TipoMantenimiento { get; set; }
    public string RealizadoPor { get; set; }
    public string Motivo { get; set; }
    public DateTime Fecha { get; set; }

}
