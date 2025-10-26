using _net_integrador.Models;
using System.Collections.Generic;

namespace _net_integrador.Repositorios
{
    public interface IRepositorioTipoInmueble
    {
        List<TipoInmueble> ObtenerTiposInmueble();
        TipoInmueble? ObtenerTipoInmuebleId(int id);
        void AgregarTipoInmueble(TipoInmueble tipo);
        void ActualizarTipoInmueble(TipoInmueble tipo);
        void DesactivarTipoInmueble(int id);
        public void ActivarTipoInmueble(int id);
        public bool EstaEnUso(int id);
    }
}
