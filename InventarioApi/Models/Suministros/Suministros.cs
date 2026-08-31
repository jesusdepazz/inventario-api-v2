using Inventory.Models;

namespace InventarioApi.Models.Suministros
{
    public class Suministro
    {
        public int Id { get; set; }
        public string NombreProducto { get; set; }
        public string UbicacionProducto { get; set; }
        public int CantidadActual { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;
        public List<EntradaSuministro> Entradas { get; set; } = new();
        public List<SalidaSuministro> Salidas { get; set; } = new();

    }


}
