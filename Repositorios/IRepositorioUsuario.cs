using _net_integrador.Models;
using System.Collections.Generic;

namespace _net_integrador.Repositorios
{
    public interface IRepositorioUsuario
    {
        List<Usuario> ObtenerUsuarios();
        Usuario? ObtenerUsuarioId(int id);
        void AgregarUsuario(Usuario usuario);
        void ActualizarUsuario(Usuario usuario);
        void EliminarUsuario(int id);
        void ActivarUsuario(int id);
        
        Usuario? ObtenerUsuarioEmail(string email);
    }
}