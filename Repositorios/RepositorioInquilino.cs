using _net_integrador.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _net_integrador.Repositorios
{
    public class RepositorioInquilino : IRepositorioInquilino
    {
        private readonly DataContext _context;

        public RepositorioInquilino(DataContext context)
        {
            _context = context;
        }
        public async Task<Inquilino?> ObtenerInquilinoId(int id)
        {
            return await _context.inquilino
                .FirstOrDefaultAsync(i => i.id == id);
        }
        public async Task AgregarInquilino(Inquilino inquilino)
        {
            await _context.inquilino.AddAsync(inquilino); 
            await _context.SaveChangesAsync();
        }
        public async Task<bool> ExisteDni(string dni, int? idExcluido = null)
        {
            var query = _context.inquilino.Where(i => i.dni == dni);
            if (idExcluido.HasValue)
                query = query.Where(i => i.id != idExcluido.Value);

            var count = await query.CountAsync();
            return count > 0;
        }
        public async Task<bool> ExisteEmail(string email, int? idExcluido = null)
        {
            var query = _context.inquilino.Where(i => i.email == email);
            if (idExcluido.HasValue)
                query = query.Where(i => i.id != idExcluido.Value);

            var count = await query.CountAsync();
            return count > 0;
        }
    }
}
