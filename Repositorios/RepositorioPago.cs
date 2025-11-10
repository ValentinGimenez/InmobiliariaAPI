using _net_integrador.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _net_integrador.Repositorios
{
    public class RepositorioPago : IRepositorioPago
    {
        private readonly DataContext _context;

        public RepositorioPago(DataContext context)
        {
            _context = context;
        }
        public async Task<List<Pago>> ObtenerPagosPorContrato(int contratoId)
        {
            return await _context.pago
                .Where(p => p.id_contrato == contratoId)
                .ToListAsync();
        }
        public async Task<Pago?> ObtenerPagoId(int id)
        {
            return await _context.pago
                .FirstOrDefaultAsync(p => p.id == id);
        }
        public async Task AgregarPago(Pago pago)
        {
            await _context.pago.AddAsync(pago); 
            await _context.SaveChangesAsync();
        }
        public async Task AnularPago(int id)
        {
            var pago = await _context.pago.FirstOrDefaultAsync(p => p.id == id);
            if (pago != null)
            {
                pago.estado = EstadoPago.anulado;
                await _context.SaveChangesAsync();
            }
        }
        public async Task ActualizarPago(Pago pago)
        {
            _context.pago.Update(pago);
            await _context.SaveChangesAsync();
        }
        public async Task<DateTime?> ObtenerFechaUltimoPagoRealizado(int contratoId)
        {
            var fechaUltimoPago = await _context.pago
                .Where(p => p.id_contrato == contratoId && p.estado == EstadoPago.recibido && p.fecha_pago.HasValue)
                .OrderByDescending(p => p.fecha_pago)
                .Select(p => p.fecha_pago)
                .FirstOrDefaultAsync();
            return fechaUltimoPago;
        }
        public async Task<int> ContarPagosRealizados(int idContrato)
        {
            return await _context.pago
                .CountAsync(p => p.id_contrato == idContrato && p.estado == EstadoPago.recibido);
        }
    }
}
