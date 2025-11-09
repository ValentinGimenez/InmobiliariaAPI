using _net_integrador.Models;
using System.Collections.Generic;

namespace _net_integrador.Repositorios
{
    public interface IRepositorioInquilino
    {
        Inquilino? ObtenerInquilinoId(int id);
        void AgregarInquilino(Inquilino inquilino);
        bool ExisteDni(string dni, int? idExcluido = null);
        bool ExisteEmail(string email, int? idExcluido = null);

    }
}