using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace _net_integrador.Models
{
    [Table("inmueble")]
    public class Inmueble
    {
        public int id { get; set; }
        public Propietario? propietario { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un propietario")]

        public int? id_propietario { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]

        public string direccion { get; set; } = string.Empty;


        [Required(ErrorMessage = "Este campo es obligatorio")]

        public UsoInmueble? uso { get; set; }
        public TipoInmueble? tipoInmueble { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un tipo de inmueble")]
        public int? id_tipo { get; set; }


        [Required(ErrorMessage = "La cantidad de ambientes es obligatoria")]
        [Range(1, 20, ErrorMessage = "La cantidad de ambientes debe ser entre 1 y 20")]

        public int? ambientes { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio")]

        public string eje_x { get; set; } = string.Empty;
        [Required(ErrorMessage = "Este campo es obligatorio")]


        public string eje_y { get; set; } = string.Empty;
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public decimal? precio { get; set; }
        public Estado estado { get; set; }


    }
    public enum UsoInmueble
    {
        Comercial = 1,
        Residencial = 2
    }
    public enum Estado
    {
        Disponible= 1,
        Suspendido= 2,
        Alquilado= 3
    }
}