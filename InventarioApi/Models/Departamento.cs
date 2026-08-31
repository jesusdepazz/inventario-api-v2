using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioApi.Models;

[Table("departamento")]
public class Departamento
{
    [Key]
    [Column("Departamento")]
    public string Codigo { get; set; }
    [Column("Descripcion")]
    public string? Descripcion { get; set; }
    public ICollection<EmpleadoInfo>? Empleados { get; set; }
}
