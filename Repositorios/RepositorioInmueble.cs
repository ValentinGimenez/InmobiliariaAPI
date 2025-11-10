using _net_integrador.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _net_integrador.Repositorios
{
    public class RepositorioInmueble : IRepositorioInmueble
    {
        private readonly DataContext _context;

        public RepositorioInmueble(DataContext context)
        {
            _context = context;
        }

        private Inmueble Map(Inmueble i)
        {
            return i;
        }
        public async Task<List<Inmueble>> ObtenerPorPropietario(int propietarioId)
        {
            var inmuebles = await _context.inmueble
                .Where(i => i.id_propietario == propietarioId)
                .ToListAsync();
            return inmuebles;
        }
        public async Task<Inmueble> ObtenerInmuebleId(int id)
        {
            return await _context.inmueble
                                 .FirstOrDefaultAsync(i => i.id == id);
        }
        public async Task<Inmueble> AgregarInmueble(Inmueble i)
        {
            await _context.inmueble.AddAsync(i);
            await _context.SaveChangesAsync();
            return i;
        }
        public async Task<Inmueble> ActualizarInmueble(Inmueble i)
        {
            _context.inmueble.Update(i);
            await _context.SaveChangesAsync();
            return i;
        }
        public async Task<bool> ActualizarEstado(int id, int estado)
        {
            var inmueble = await _context.inmueble.FindAsync(id);
            if (inmueble == null) return false;

            inmueble.estado = estado;
            _context.inmueble.Update(inmueble);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task MarcarComoAlquilado(int id)
        {
            var inmueble = await _context.inmueble.FindAsync(id);
            if (inmueble == null) return;

            inmueble.estado = 3;
            _context.inmueble.Update(inmueble);
            await _context.SaveChangesAsync();
        }
    }
}
