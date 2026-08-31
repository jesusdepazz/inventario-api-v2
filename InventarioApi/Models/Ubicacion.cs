using System.ComponentModel.DataAnnotations;
using InventarioApi.Models;

namespace Inventory.Models;

public class Ubicacion
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Nombre { get; set; }
}
