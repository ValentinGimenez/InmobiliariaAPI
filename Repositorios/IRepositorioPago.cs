using _net_integrador.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _net_integrador.Repositorios
{
    public interface IRepositorioPago
    {
        Task<List<Pago>> ObtenerPagosPorContrato(int contratoId); 
        Task<Pago?> ObtenerPagoId(int id);  
        Task AgregarPago(Pago pago);  
        Task AnularPago(int id);  
        Task ActualizarPago(Pago pago);  
        Task<DateTime?> ObtenerFechaUltimoPagoRealizado(int contratoId);
        Task<int> ContarPagosRealizados(int contratoId);
    }
}
