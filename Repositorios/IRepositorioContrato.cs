using _net_integrador.Models;
using System.Collections.Generic;

namespace _net_integrador.Repositorios
{
    public interface IRepositorioContrato
    {
        List<Contrato> ObtenerContratos();
        Contrato? ObtenerContratoId(int id);
        public int AgregarContrato(Contrato contrato);
        public int ActualizarContrato(Contrato contrato);
        List<Contrato> ObtenerContratoPorInmueble(int idInmueble, int idContrato);
        List<Contrato> ObtenerContratosVigentesPorRango(DateTime fechaInicio, DateTime fechaFin_);
        List<Contrato> ObtenerContratosPorVencimiento(int diasVencimiento);
    }
}
