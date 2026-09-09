using System.ComponentModel.DataAnnotations;

namespace InventoryApi.Models
{
    public class Vehiculo
    {
        public int Id { get; set; }

        public string? OrdenCompra { get; set; }
        public string? Factura { get; set; }
        public string? Proveedor { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public string? HojaNo { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public string? Codificacion { get; set; }
        public string? TipoEquipo { get; set; } // ej. "Vehículo"
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public string? Serie { get; set; }
        public string? NumeroAsignado { get; set; } // placa / número de unidad
        public string? Ubicacion { get; set; }
        public string? ResponsableAnterior { get; set; }
        public string? Comentarios { get; set; }
        public string? Observaciones { get; set; }
        public string? Estado { get; set; }

        
        [MaxLength(17)]
        public string? Vin { get; set; } 
        public int? ModeloAnio { get; set; } 
        public string? Color { get; set; }
        public string? TipoCombustible { get; set; } 
        public string? Placa { get; set; }
        public string? ResponsableActual { get; set; }

        
        public int? KilometrajeAsignacion { get; set; }
        public DateTime? FechaAsignacion { get; set; }
        public int? KilometrajeActual { get; set; }
        public DateTime? FechaUltimaActualizacionKm { get; set; }

        
        public bool EnCatalogo { get; set; }
        public string? DescripcionCatalogo { get; set; }

        
        public ICollection<HistorialReparacionVehiculo>? HistorialReparaciones { get; set; }
        public ICollection<MantenimientoVehiculo>? Mantenimientos { get; set; }
        public ICollection<AlertaServicioVehiculo>? AlertasServicio { get; set; }
        public ICollection<BitacoraFallaVehiculo>? BitacoraFallas { get; set; }
        public ICollection<PolizaSeguroVehiculo>? PolizasSeguro { get; set; }
        public ICollection<ReporteEstadoFisicoVehiculo>? ReportesEstadoFisico { get; set; }
    }

    
    public class HistorialReparacionVehiculo
    {
        public int Id { get; set; }
        public int VehiculoId { get; set; }
        public Vehiculo? Vehiculo { get; set; }

        public DateTime Fecha { get; set; }
        public string? Taller { get; set; }
        public string? Descripcion { get; set; }
        public decimal? Costo { get; set; }
        public int? KilometrajeEnReparacion { get; set; }
        public string? Factura { get; set; }
    }

    public class MantenimientoVehiculo
    {
        public int Id { get; set; }
        public int VehiculoId { get; set; }
        public Vehiculo? Vehiculo { get; set; }

        public string? TipoMantenimiento { get; set; } 
        public DateTime? FechaProgramada { get; set; }
        public DateTime? FechaRealizada { get; set; }
        public int? KilometrajeProgramado { get; set; }
        public string? Descripcion { get; set; }
        public string? Estado { get; set; } // Pendiente, Realizado, Vencido
    }

    // --- Alertas de servicios ---
    public class AlertaServicioVehiculo
    {
        public int Id { get; set; }
        public int VehiculoId { get; set; }
        public Vehiculo? Vehiculo { get; set; }

        public string? TipoAlerta { get; set; } 
        public DateTime? FechaAlerta { get; set; }
        public int? KilometrajeAlerta { get; set; }
        public bool Atendida { get; set; }
        public string? Notas { get; set; }
    }

    
    public class BitacoraFallaVehiculo
    {
        public int Id { get; set; }
        public int VehiculoId { get; set; }
        public Vehiculo? Vehiculo { get; set; }

        public DateTime Fecha { get; set; }
        public string? DescripcionFalla { get; set; }
        public string? ReportadoPor { get; set; }
        public string? Estado { get; set; } // Reportada, En revisión, Resuelta
        public string? Solucion { get; set; }
    }

    
    public class PolizaSeguroVehiculo
    {
        public int Id { get; set; }
        public int VehiculoId { get; set; }
        public Vehiculo? Vehiculo { get; set; }

        public string? Aseguradora { get; set; }
        public string? NumeroPoliza { get; set; }
        public string? TipoCobertura { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaRenovacion { get; set; }
        public decimal? Prima { get; set; }
        public string? Estado { get; set; } 
    }

    public class ReporteEstadoFisicoVehiculo
    {
        public int Id { get; set; }
        public int VehiculoId { get; set; }
        public Vehiculo? Vehiculo { get; set; }

        public DateTime Fecha { get; set; }
        public string? EstadoGeneral { get; set; } 
        public string? DetalleCarroceria { get; set; }
        public string? DetalleInterior { get; set; }
        public string? DetalleMecanico { get; set; }
        public string? EvaluadoPor { get; set; }
        public string? ImagenesUrl { get; set; } 
    }
}
