using _net_integrador.Models;
using System.Collections.Generic;

namespace _net_integrador.Repositorios
{
    public interface IRepositorioAuditoria
    {
        void InsertarRegistroAuditoria(TipoAuditoria tipo, int idRegistro, AccionAuditoria accion, string usuario);

        List<Auditoria> ObtenerAuditorias();

        Auditoria? ObtenerAuditoriaPorId(int id);

        List<Auditoria> ObtenerAuditoriasPorTipo(TipoAuditoria tipo);

        List<Auditoria> ObtenerAuditoriasContrato();
        List<Auditoria> ObtenerAuditoriasPago();
    }
}
