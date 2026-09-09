namespace InventarioApi.Models
{
    public class Inmueble
    {
        public int Id { get; set; }

        
        public string? NumeroOrdenCompra { get; set; }
        public DateTime? FechaOrdenCompra { get; set; }

        
        public string? NumeroFacturaElectronica { get; set; }
        public string? NombreProveedor { get; set; }
        public DateTime? FechaFactura { get; set; }

        
        public string Descripcion { get; set; } = string.Empty;
        public string Estado { get; set; } = "disponible"; 
        public string Direccion { get; set; } = string.Empty;
        public string? FichaTecnica { get; set; }

       
        public int? CatalogoActivoId { get; set; }
        public string? NombreCatalogoActivo { get; set; }

       
        public ICollection<InmuebleArchivo>? Archivos { get; set; }
        public ICollection<PolizaSeguro>? Polizas { get; set; }
    }

    
    public class InmuebleArchivo
    {
        public int Id { get; set; }
        public int InmuebleId { get; set; }
        public Inmueble? Inmueble { get; set; }

        public string RutaArchivo { get; set; } = string.Empty;
        public string NombreOriginal { get; set; } = string.Empty;
        public string TipoDocumento { get; set; } = string.Empty; 
        public DateTime FechaSubida { get; set; } = DateTime.Now;
    }

    
    public class PolizaSeguro
    {
        public int Id { get; set; }
        public int InmuebleId { get; set; }
        public Inmueble? Inmueble { get; set; }

        public string NumeroPoliza { get; set; } = string.Empty;
        public string Aseguradora { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaRenovacion { get; set; }
        public string? CoberturasEspecificas { get; set; }
        public string? GestionReclamosSiniestros { get; set; }
        public string Estado { get; set; } = "activa"; // "activa", "vencida", "cancelada"
    }
}