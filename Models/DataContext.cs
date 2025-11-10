using Microsoft.EntityFrameworkCore;

namespace _net_integrador.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }

        public DbSet<Propietario> propietario { get; set; }
        public DbSet<Inmueble> inmueble { get; set; }
        public DbSet<Inquilino> inquilino { get; set; }
        public DbSet<Contrato> contrato { get; set; }
        public DbSet<Pago> pago { get; set; }
    }
}
