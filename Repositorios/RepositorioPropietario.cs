using _net_integrador.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace _net_integrador.Repositorios
{
    public class RepositorioPropietario : IRepositorioPropietario
    {
        private readonly DataContext _context;

        public RepositorioPropietario(DataContext context)
        {
            _context = context;
        }

        public async Task<Propietario> ObtenerPropietarioId(int id)
        {
            return await _context.propietario
                .FirstOrDefaultAsync(p => p.id == id);
        }

        public async Task<Propietario> ActualizarPropietario(Propietario propietario)
        {
            var existingPropietario = await _context.propietario
                .FirstOrDefaultAsync(p => p.id == propietario.id);

            if (existingPropietario != null)
            {
                existingPropietario.nombre = propietario.nombre;
                existingPropietario.apellido = propietario.apellido;
                existingPropietario.dni = propietario.dni;
                existingPropietario.email = propietario.email;
                existingPropietario.telefono = propietario.telefono;

                await _context.SaveChangesAsync();
            }

            return existingPropietario;
        }
        public async Task<bool> CambiarPassword(int id, string hashClave)
        {
            var propietario = await _context.propietario
                .FirstOrDefaultAsync(p => p.id == id);

            if (propietario != null)
            {
                propietario.clave = hashClave;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        public async Task AgregarPropietario(Propietario propietario)
        {
            await _context.propietario.AddAsync(propietario);
            await _context.SaveChangesAsync();
        }

        public async Task<Propietario?> ObtenerPorEmail(string email)
        {
            return await _context.propietario
                .FirstOrDefaultAsync(p => p.email == email);
        }
    }
}
