using _net_integrador.Models;

namespace _net_integrador.Repositorios
{
    public interface IRepositorioInmueble
    {
        List<Inmueble> ObtenerPorPropietario(int propietarioId);
        Inmueble ObtenerInmuebleId(int id);
        Inmueble AgregarInmueble(Inmueble i);
        bool ActualizarEstado(int id, Estado estado);
        Inmueble ActualizarInmueble(Inmueble i);

    }
}
