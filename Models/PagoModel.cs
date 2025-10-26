using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _net_integrador.Models
{
    [Table("pago")]
    public class Pago
    {
        public int id { get; set; }
        
        [Display(Name = "Contrato")]
        public int id_contrato { get; set; }
        public Contrato Contrato { get; set; } = new Contrato();
        
        [Display(Name = "Nro. Pago")]
        public int nro_pago { get; set; }
        
        [Display(Name = "Fecha de Pago")]
        public DateTime? fecha_pago { get; set; }
        
        [Display(Name = "Estado")]
        public EstadoPago estado { get; set; }
        
        [Display(Name = "Concepto")]
        public string concepto { get; set; } = string.Empty;
    }
    
    public enum EstadoPago
    {
        pendiente,
        recibido,
        anulado
    }
}