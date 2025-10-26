using _net_integrador.Models;
using System.Collections.Generic;

namespace _net_integrador.Repositorios
{
    public interface IRepositorioInquilino
    {
        List<Inquilino> ObtenerInquilinos();
        Inquilino? ObtenerInquilinoId(int id);
        Inquilino ActualizarInquilino(Inquilino inquilino);
        bool EliminarInquilino(int id);
        void ActivarInquilino(int id);

        void AgregarInquilino(Inquilino inquilino);
        bool ExisteDni(string dni, int? idExcluido = null);
        bool ExisteEmail(string email, int? idExcluido = null);
        List<Inquilino> ObtenerInquilinosActivos();

    }
}
