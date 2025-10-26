using System.ComponentModel.DataAnnotations.Schema;

namespace _net_integrador.Models
{
    [Table("tipo_inmueble")]
    public class TipoInmueble
    {
        public int id { get; set; }
        public string tipo { get; set; } = string.Empty;
        public int estado { get; set; }
    }
}