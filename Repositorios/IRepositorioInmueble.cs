using _net_integrador.Models;

namespace _net_integrador.Repositorios
{
    public interface IRepositorioInmueble
    {
        List<Inmueble> ObtenerPorPropietario(int propietarioId);
        Inmueble ObtenerInmuebleId(int id);
        Inmueble AgregarInmueble(Inmueble i);
        Inmueble ActualizarInmueble(Inmueble i);
        bool ActualizarEstado(int id, Estado estado);
        List<Inmueble> ObtenerTodos();
        List<Inmueble> ObtenerInmueblesDisponibles();
        bool SuspenderOferta(int id);
        bool ActivarOferta(int id);
        bool MarcarComoAlquilado(int id);
        List<Inmueble> BuscarDisponiblePorFecha(DateTime inicio, DateTime fin);

    }
}
