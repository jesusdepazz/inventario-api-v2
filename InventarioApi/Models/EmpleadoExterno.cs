using System.ComponentModel.DataAnnotations;

namespace InventarioApi.Models;

public class EmpleadoExterno
{
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string CodigoEmpleado { get; set; }

    [Required]
    [MaxLength(150)]
    public string Nombre { get; set; }

    [Required]
    [MaxLength(100)]
    public string Puesto { get; set; }

    [Required]
    [MaxLength(30)]
    public string Documento { get; set; }

    [MaxLength(20)]
    public string? Telefono { get; set; }

    [MaxLength(150)]
    public string? Proyecto { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    public bool Activo { get; set; } = true;
}
