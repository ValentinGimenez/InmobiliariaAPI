using _net_integrador.Models;
using System.Collections.Generic;

namespace _net_integrador.Repositorios
{
    public interface IRepositorioContrato
    {
        List<Contrato> ObtenerContratos();
        Contrato? ObtenerContratoId(int id);
        List<Contrato> ObtenerVigentesPorPropietario(int idPropietario);
        Contrato? ObtenerVigentePorInmuebleYPropietario(int idInmueble, int idPropietario);
        int AgregarContrato(Contrato contrato);
        int ActualizarContrato(Contrato contrato);
        List<Contrato> ObtenerContratoPorInmueble(int idInmueble, int idContrato);
        List<Contrato> ObtenerContratosVigentesPorRango(DateTime fechaInicio, DateTime fechaFin);
        List<Contrato> ObtenerContratosPorVencimiento(int diasHastaVencimiento);
    }
}