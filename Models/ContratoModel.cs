using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _net_integrador.Models
{
    [Table("contrato")]
    public class Contrato
    {
        [Key]
        public int id { get; set; }

        [ForeignKey("Inquilino")]
        [Column("id_inquilino")]
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? id_inquilino { get; set; }
        public Inquilino? Inquilino { get; set; }

        [ForeignKey("Inmueble")]
        [Column("id_inmueble")]
        [Required(ErrorMessage = "Este campo es obligatorio")]
        public int? id_inmueble { get; set; }
        public Inmueble? Inmueble { get; set; }

        [Range(1.01, double.MaxValue, ErrorMessage = "Ingrese solo números positivos")]
        public decimal? monto_mensual { get; set; }

        public decimal? multa { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Ingrese una fecha válida")]
        public DateTime? fecha_terminacion_anticipada { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio")]
        [DataType(DataType.Date, ErrorMessage = "Ingrese una fecha válida")]
        public DateTime? fecha_inicio { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio")]
        [DataType(DataType.Date, ErrorMessage = "Ingrese una fecha válida")]

        public DateTime? fecha_fin { get; set; }
        public int estado { get; set; }

        [NotMapped]
        [Display(Name = "Duración del Contrato (Meses)")]
        public int DuracionEnMeses 
        {
            get
            {
                if (fecha_inicio.HasValue && fecha_fin.HasValue)
                {
                    int duration = ((fecha_fin.Value.Year - fecha_inicio.Value.Year) * 12) + fecha_fin.Value.Month - fecha_inicio.Value.Month;
                    if (fecha_fin.Value.Day >= fecha_inicio.Value.Day)
                        duration++;
                    return duration;
                }
                return 0;
            }
        }
    }
}
