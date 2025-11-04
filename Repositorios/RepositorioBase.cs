using Microsoft.Extensions.Configuration;

namespace _net_integrador.Repositorios
{
    public abstract class RepositorioBase
    {
        protected readonly string connectionString;

        protected RepositorioBase(IConfiguration configuration)
        {
            var cs = configuration.GetConnectionString("Mysql");
            if (string.IsNullOrWhiteSpace(cs))
                throw new InvalidOperationException("Falta ConnectionStrings:Mysql en appsettings.json");

            connectionString = cs; // ✅ ya no es null
        }
    }
}
