using _net_integrador.Models;

namespace _net_integrador.Repositorios
{
    public interface IRepositorioInmueble
    {
        List<Inmueble> ObtenerPorPropietario(int propietarioId);
        Inmueble ObtenerInmuebleId(int id);
        Inmueble AgregarInmueble(Inmueble i);
        Inmueble ActualizarInmueble(Inmueble i);
        bool ActualizarImagen(int id, string rutaRelativa);
    }
}
