using _net_integrador.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _net_integrador.Repositorios
{
    public interface IRepositorioInmueble
    {
        Task<List<Inmueble>> ObtenerPorPropietario(int propietarioId);
        Task<Inmueble> ObtenerInmuebleId(int id);
        Task<Inmueble> AgregarInmueble(Inmueble i);
        Task<bool> ActualizarEstado(int id, int estado);
        Task<Inmueble> ActualizarInmueble(Inmueble i);
        Task MarcarComoAlquilado(int i);
    }
}
