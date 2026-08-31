using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InventarioApi.Models;

public class Traslado
{
    public int Id { get; set; }
    public string No { get; set; }
    public DateTime FechaEmision { get; set; }
    public string Status { get; set; }

    public string Motivo { get; set; }
    public string Observaciones { get; set; }
    public string UbicacionDesde { get; set; }
    public string UbicacionHasta { get; set; }

    public List<TrasladoEquipo> Equipos { get; set; } = new();

    public TrasladoEmpleadoEntrega EmpleadoEntrega { get; set; }
    public TrasladoEmpleadoRecibe EmpleadoRecibe { get; set; }
}

public class TrasladoEquipo
{
    public int Id { get; set; }
    public int TrasladoId { get; set; }

    public string Equipo { get; set; }
    public string DescripcionEquipo { get; set; }
    public string Marca { get; set; }
    public string Modelo { get; set; }
    public string Serie { get; set; }

}

public class TrasladoEmpleadoEntrega
{
    public int Id { get; set; }
    public int TrasladoId { get; set; }

    public string Codigo { get; set; }
    public string Nombre { get; set; }
    public string Puesto { get; set; }
    public string Departamento { get; set; }
}

public class TrasladoEmpleadoRecibe
{
    public int Id { get; set; }
    public int TrasladoId { get; set; }

    public string Codigo { get; set; }
    public string Nombre { get; set; }
    public string Puesto { get; set; }
    public string Departamento { get; set; }

}
