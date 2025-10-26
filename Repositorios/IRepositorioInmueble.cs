using _net_integrador.Models;
using System.Collections.Generic;

namespace _net_integrador.Repositorios
{
    public interface IRepositorioInmueble
    {
        List<Inmueble> ObtenerInmuebles();
        Inmueble? ObtenerInmuebleId(int id);
        void AgregarInmueble(Inmueble inmuebleNuevo);
        void ActualizarInmueble(Inmueble inmuebleEditado);
        void SuspenderOferta(int id);
        bool ActivarOferta(int id);
        List<Inmueble> ObtenerInmueblesDisponibles();
        List<Inmueble> ObtenerInmueblesPorPropietario(int propietarioId);
        void MarcarComoAlquilado(int id);
        List <Inmueble> BuscarDisponiblePorFecha(DateTime fechaDesde, DateTime fechaHasta);
    }
}
