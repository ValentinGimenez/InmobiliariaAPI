using _net_integrador.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace _net_integrador.Repositorios
{
    public class RepositorioInmueble : RepositorioBase, IRepositorioInmueble
    {
        public RepositorioInmueble(IConfiguration configuration) : base(configuration) { }

        private Inmueble Map(MySqlDataReader r)
        {
            Inmueble i = new Inmueble();
            i.id             = r.GetInt32("id");
            i.id_propietario = r.GetInt32("id_propietario");

            var tipoStr   = r.GetString("tipo");
            var usoStr    = r.GetString("uso");
            var estadoStr = r.GetString("estado");

            i.tipo   = Enum.TryParse<TipoInmueble>(tipoStr, true, out var t) ? t : TipoInmueble.Casa;
            i.uso    = Enum.TryParse<UsoInmueble>(usoStr, true, out var u) ? u : UsoInmueble.Residencial;
            i.estado = Enum.TryParse<Estado>(estadoStr, true, out var e) ? e : Estado.Disponible;

            i.direccion  = r.GetString("direccion");
            i.ambientes  = r.IsDBNull(r.GetOrdinal("ambientes")) ? (int?)null    : r.GetInt32("ambientes");
            i.eje_x      = r.IsDBNull(r.GetOrdinal("eje_x"))     ? (double?)null : r.GetDouble("eje_x");
            i.eje_y      = r.IsDBNull(r.GetOrdinal("eje_y"))     ? (double?)null : r.GetDouble("eje_y");
            i.precio     = r.IsDBNull(r.GetOrdinal("precio"))    ? (decimal?)null: r.GetDecimal("precio");
            i.imagen     = r.IsDBNull(r.GetOrdinal("imagen"))    ? null          : r.GetString("imagen");
            i.superficie = r.IsDBNull(r.GetOrdinal("superficie"))? (double?)null : r.GetDouble("superficie");
            return i;
        }

        public List<Inmueble> ObtenerPorPropietario(int propietarioId)
        {
            List<Inmueble> lista = new List<Inmueble>();
            using (var cn = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT id, id_propietario, tipo, direccion, uso, ambientes, eje_x, eje_y, precio, estado, imagen, superficie
                            FROM inmueble WHERE id_propietario = @pid";
                using (var cmd = new MySqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@pid", propietarioId);
                    cn.Open();
                    var r = cmd.ExecuteReader();
                    while (r.Read()) lista.Add(Map(r));
                    cn.Close();
                }
            }
            return lista;
        }

        public Inmueble ObtenerInmuebleId(int id)
        {
            Inmueble i = new Inmueble();
            using (var cn = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT id, id_propietario, tipo, direccion, uso, ambientes, eje_x, eje_y, precio, estado, imagen, superficie
                            FROM inmueble WHERE id = @id";
                using (var cmd = new MySqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cn.Open();
                    var r = cmd.ExecuteReader();
                    if (r.Read()) i = Map(r);
                    cn.Close();
                }
            }
            return i;
        }
        public Inmueble AgregarInmueble(Inmueble i)
        {
            using (var cn = new MySqlConnection(connectionString))
            {
                var sql = @"INSERT INTO inmueble
                           (id_propietario, tipo, direccion, uso, ambientes, eje_x, eje_y, precio, estado, imagen, superficie)
                           VALUES (@prop, @tipo, @dir, @uso, @amb, @x, @y, @precio, @estado, @img, @sup);
                           SELECT LAST_INSERT_ID();";
                using (var cmd = new MySqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@prop", i.id_propietario);
                    cmd.Parameters.AddWithValue("@tipo",   i.tipo.ToString()); 
                    cmd.Parameters.AddWithValue("@dir",    i.direccion);
                    cmd.Parameters.AddWithValue("@uso",    i.uso.ToString());  
                    cmd.Parameters.AddWithValue("@amb",   (object?)i.ambientes  ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@x",     (object?)i.eje_x      ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@y",     (object?)i.eje_y      ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@precio",(object?)i.precio     ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@estado", i.estado.ToString()); 
                    cmd.Parameters.AddWithValue("@img",   (object?)i.imagen     ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@sup",   (object?)i.superficie ?? DBNull.Value);

                    cn.Open();
                    i.id = Convert.ToInt32(cmd.ExecuteScalar());
                    cn.Close();
                }
            }
            return i;
        }

        public Inmueble ActualizarInmueble(Inmueble i)
        {
            using (var cn = new MySqlConnection(connectionString))
            {
                var sql = @"UPDATE inmueble SET
                            tipo=@tipo, direccion=@dir, uso=@uso, ambientes=@amb,
                            eje_x=@x, eje_y=@y, precio=@precio, estado=@estado, superficie=@sup
                            WHERE id=@id";
                using (var cmd = new MySqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@id",    i.id);
                    cmd.Parameters.AddWithValue("@tipo",  i.tipo.ToString());
                    cmd.Parameters.AddWithValue("@dir",   i.direccion);
                    cmd.Parameters.AddWithValue("@uso",   i.uso.ToString());
                    cmd.Parameters.AddWithValue("@amb",  (object?)i.ambientes  ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@x",    (object?)i.eje_x      ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@y",    (object?)i.eje_y      ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@precio",(object?)i.precio    ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@estado", i.estado.ToString());
                    cmd.Parameters.AddWithValue("@sup",  (object?)i.superficie ?? DBNull.Value);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
            }
            return ObtenerInmuebleId(i.id);
        }

        public bool ActualizarImagen(int id, string rutaRelativa)
        {
            using (var cn = new MySqlConnection(connectionString))
            {
                var sql = "UPDATE inmueble SET imagen=@img WHERE id=@id";
                using (var cmd = new MySqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@img", rutaRelativa);
                    cn.Open();
                    var rows = cmd.ExecuteNonQuery();
                    cn.Close();
                    return rows > 0;
                }
            }
        }
    }
}
