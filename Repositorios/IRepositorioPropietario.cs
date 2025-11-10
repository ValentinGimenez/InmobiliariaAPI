using _net_integrador.Models;

namespace _net_integrador.Repositorios
{
    public interface IRepositorioPropietario
    {
        Task<Propietario> ObtenerPropietarioId(int id); 
        Task<Propietario> ActualizarPropietario(Propietario propietario);
        Task<Propietario?> ObtenerPorEmail(string email); 
        Task<bool> CambiarPassword(int id, string hashClave); 
        Task AgregarPropietario(Propietario propietario);
    }
}
