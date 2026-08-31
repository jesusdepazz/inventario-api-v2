using System.Text.Json.Serialization;

namespace InventarioApi.Models.Suministros
{
    public class SalidaSuministro
    {
        public int Id { get; set; }
        public int SuministroId { get; set; }

        [JsonIgnore]
        public Suministro? Suministro { get; set; }

        public int CantidadProducto { get; set; }
        public string PersonaResponsable { get; set; }
        public string DepartamentoResponsable { get; set; }
        public DateTime Fecha { get; set; }
    }
}
