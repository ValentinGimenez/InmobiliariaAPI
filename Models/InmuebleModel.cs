using System.ComponentModel.DataAnnotations;

namespace _net_integrador.Models
{
    public class Inmueble
    {
        [Key]
        public int id { get; set; }

        [Required]
        public int id_propietario { get; set; }

        public int tipo { get; set; } 

        public int uso { get; set; } 

        [Required]
        public int estado { get; set; }

        public string direccion { get; set; } = string.Empty;
        public int? ambientes { get; set; }
        public double? eje_x { get; set; }
        public double? eje_y { get; set; }
        public decimal? precio { get; set; }
        public string? imagen { get; set; }
        public double? superficie { get; set; }
    }
}
