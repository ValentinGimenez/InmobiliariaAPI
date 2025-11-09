using _net_integrador.Models;

namespace _net_integrador.Repositorios
{
    public interface IRepositorioPropietario
    {
        Propietario ObtenerPropietarioId(int id);
        Propietario ActualizarPropietario(Propietario propietario);
        public Propietario? ObtenerPorEmail(string email);
        public bool CambiarPassword(int id, string hashClave);
    }
}