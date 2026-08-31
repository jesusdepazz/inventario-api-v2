using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioApi.Models;

[Table("empleado")]
public class EmpleadoInfo
{
    [Key]
    [Column("Empleado")]
    public string Empleado { get; set; }

    [Column("Nombre")]
    public string? Nombre { get; set; }

    [Column("U_NOMBRE_CC")]
    public string? Puesto { get; set; }

    public string? Departamento { get; set; }

    [ForeignKey("Departamento")]
    public Departamento? DepartamentoInfo { get; set; }
}
