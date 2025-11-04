using _net_integrador.Models;

namespace _net_integrador.Repositorios
{
    public interface IRepositorioPropietario
    {
        Propietario? ObtenerPorEmail(string email);

        Propietario? ObtenerPropietarioId(int id);

        Propietario ActualizarPropietario(Propietario p);

        bool CambiarPassword(int id, string nueva);

        int AgregarPropietario(Propietario p);
    }
}
