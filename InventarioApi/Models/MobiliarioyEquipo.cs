using System.ComponentModel.DataAnnotations;

namespace InventoryApi.Models
{
    public class MobiliarioEquipo
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
        public string? NumeroAsignado { get; set; }
        public string? EquipoTipo { get; set; } 
        public string? Ubicacion { get; set; }
        public string? ResponsableAnterior { get; set; }
        public string? Comentarios { get; set; }
        public string? Observaciones { get; set; }
        public string? Estado { get; set; }

        
        public string? NumeroChapaActivo { get; set; }
        public string? ControlLlaves { get; set; }
        public string? EstadoFisicoActual { get; set; }
        public string? Color { get; set; }
        public string? Dimensiones { get; set; }
        public string? CatalogoActivos { get; set; }

       
        public ICollection<ReporteDanio>? ReportesDanios { get; set; }
    }

    public class ReporteDanio
    {
        public int Id { get; set; }
        public int MobiliarioEquipoId { get; set; }
        public MobiliarioEquipo? MobiliarioEquipo { get; set; }

        public DateTime FechaReporte { get; set; }
        public string? Descripcion { get; set; }
        public string? TipoIncidencia { get; set; } // Ej: "Daño", "Pérdida", "Mantenimiento"
        public string? ReportadoPor { get; set; }
        public string? EstadoReporte { get; set; } // "Pendiente", "En proceso", "Resuelto"
    }
}