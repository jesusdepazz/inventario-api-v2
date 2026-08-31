using System.ComponentModel.DataAnnotations;

namespace InventoryApi.Models
{
    public class Equipo
    {
        public int Id { get; set; }
        public string? OrdenCompra { get; set; }
        public string? Factura { get; set; }
        public string? Proveedor { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public string? HojaNo { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public string? Codificacion { get; set; }
        public string? TipoEquipo { get; set; } 
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public string? Serie { get; set; }
        public string? Extension { get; set; }
        public string? NumeroAsignado { get; set; }
        public string? Imei { get; set; }
        public string? EquipoTipo { get; set; }
        public string? Ubicacion { get; set; }
        public string? ResponsableAnterior { get; set; }
        public string? Comentarios { get; set; }
        public string? Observaciones { get; set; }
        public string? Estado { get; set; }
    }

}
