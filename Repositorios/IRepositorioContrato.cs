using _net_integrador.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _net_integrador.Repositorios
{
    public interface IRepositorioContrato
    {
        Task<List<Contrato>> ObtenerContratos(); 
        Task<Contrato?> ObtenerContratoId(int id);  
        Task<List<Contrato>> ObtenerVigentesPorPropietario(int idPropietario);
        Task<Contrato?> ObtenerVigentePorInmuebleYPropietario(int idInmueble, int idPropietario);
        Task<int> AgregarContrato(Contrato contrato); 
        Task<int> ActualizarContrato(Contrato contrato); 
        Task<List<Contrato>> ObtenerContratoPorInmueble(int idInmueble, int idContrato);
        Task<List<Contrato>> ObtenerContratosVigentesPorRango(DateTime fechaInicio, DateTime fechaFin);
        Task<Contrato?> ObtenerContratoConInmueble(int id);
        Task<List<Contrato>> ObtenerContratoPorInmueble(int idInmueble);
    }
}
