using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace _net_integrador.Models
{
    [Table("auditoria")]
    public class Auditoria
    {
        public int id_auditoria { get; set; }

        public TipoAuditoria tipo { get; set; }

        public int id_registro_afectado { get; set; }
        public AccionAuditoria accion { get; set; }
        public string usuario { get; set; } = string.Empty;
        public DateTime fecha_hora { get; set; }
    }


    public enum TipoAuditoria
    {
        Contrato,
        Pago,
        Inmueble,
        Usuario
    }

    public enum AccionAuditoria
    {
        Recibir,
        Anular,
        Crear,
        Actualizar,
        Finalizar
    }
}
