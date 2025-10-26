using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _net_integrador.Models
{
    [Table("contrato")]
    public class Contrato
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio ")]

        public int? id_inquilino { get; set; }
        public Inquilino? Inquilino { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio ")]
        public int? id_inmueble { get; set; }
        public Inmueble? Inmueble { get; set; }
        public decimal? monto_mensual { get; set; }
        [Range(1.01, double.MaxValue, ErrorMessage = "Ingrese solo números positivos")]

        public decimal? multa { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Ingrese una fecha valida")]

        public DateTime? fecha_terminacion_anticipada { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio ")]
        [DataType(DataType.Date, ErrorMessage = "Ingrese una fecha valida")]
        public DateTime? fecha_inicio { get; set; }
        [Required(ErrorMessage = "Este campo es obligatorio ")]
        [DataType(DataType.Date, ErrorMessage = "Ingrese una fecha valida")]

        public DateTime? fecha_fin { get; set; }
        public int estado { get; set; }
        [NotMapped]
        [Display(Name = "Duración del Contrato (Meses)")]
        public int DuracionEnMeses { get; set; }
    }
}