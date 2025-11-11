using _net_integrador.Models;
using Microsoft.EntityFrameworkCore;

namespace _net_integrador.Repositorios
{
    public class RepositorioContrato : IRepositorioContrato
    {
        private readonly DataContext _context;

        public RepositorioContrato(DataContext context)
        {
            _context = context;
        }
        public async Task<List<Contrato>> ObtenerContratos()
        {
            return await _context.contrato
                .Include(c => c.Inquilino)
                .Include(c => c.Inmueble)
                .ToListAsync();
        }
        public async Task<Contrato?> ObtenerContratoId(int id)
        {
            return await _context.contrato
                .Include(c => c.Inquilino)
                .Include(c => c.Inmueble)
                .FirstOrDefaultAsync(c => c.id == id);
        }
        public async Task<int> AgregarContrato(Contrato contrato)
        {
            _context.contrato.Add(contrato);
            await _context.SaveChangesAsync();
            return contrato.id;
        }
        public async Task<int> ActualizarContrato(Contrato contrato)
        {
            _context.contrato.Update(contrato);
            await _context.SaveChangesAsync();
            return contrato.id;
        }
        public async Task<List<Contrato>> ObtenerContratoPorInmueble(int idInmueble, int idContrato)
        {
            return await _context.contrato
                .Where(c => c.id_inmueble == idInmueble && c.id != idContrato && c.estado == 1)
                .Include(c => c.Inquilino)
                .Include(c => c.Inmueble)
                .ToListAsync();
        }
        public async Task<List<Contrato>> ObtenerContratosVigentesPorRango(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.contrato
                .Include(c => c.Inquilino)
                .Include(c => c.Inmueble)
                .Where(c => c.estado == 1 && c.fecha_inicio <= fechaFin && c.fecha_fin >= fechaInicio)
                .ToListAsync();
        }
        public async Task<List<Contrato>> ObtenerVigentesPorPropietario(int idPropietario)
        {
            return await _context.contrato
                .Include(c => c.Inquilino)
                .Include(c => c.Inmueble)
                .Where(c => c.Inmueble.id_propietario == idPropietario && c.estado == 1)
                .ToListAsync();
        }
        public async Task<Contrato?> ObtenerVigentePorInmuebleYPropietario(int idInmueble, int idPropietario)
        {
            return await _context.contrato
                .Include(c => c.Inquilino)
                .Include(c => c.Inmueble)
                .FirstOrDefaultAsync(c => c.Inmueble.id == idInmueble && c.Inmueble.id_propietario == idPropietario && c.estado == 1);
        }
        public async Task<Contrato?> ObtenerContratoConInmueble(int id)
        {
            return await _context.contrato
                .Include(c => c.Inmueble)
                .FirstOrDefaultAsync(c => c.id == id);
        }
        public async Task<List<Contrato>> ObtenerContratoPorInmueble(int idInmueble)
        {
            return await _context.contrato
                .Where(c => c.id_inmueble == idInmueble && c.estado == 1)
                .ToListAsync();
        }

    }
}
