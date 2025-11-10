using _net_integrador.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _net_integrador.Repositorios
{
    public interface IRepositorioInquilino
    {
        Task<Inquilino?> ObtenerInquilinoId(int id);  
        Task AgregarInquilino(Inquilino inquilino); 
        Task<bool> ExisteDni(string dni, int? idExcluido = null);  
        Task<bool> ExisteEmail(string email, int? idExcluido = null);
    }
}
