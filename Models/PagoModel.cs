using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _net_integrador.Models
{
    [Table("pago")]
    public class Pago
    {
        [Key]
        public int id { get; set; }

        [ForeignKey("Contrato")] 
        [Display(Name = "Contrato")]
        public int id_contrato { get; set; }

        public virtual Contrato Contrato { get; set; }

        [Display(Name = "Nro. Pago")]
        public int nro_pago { get; set; }

        [Display(Name = "Fecha de Pago")]
        public DateTime? fecha_pago { get; set; }

        [Display(Name = "Estado")]
        public EstadoPago estado { get; set; }

        [Display(Name = "Concepto")]
        [StringLength(255)] 
        public string concepto { get; set; } = string.Empty;
    }
    public enum EstadoPago
    {
        pendiente = 0,
        recibido = 1,
        anulado = 2
    }
}
