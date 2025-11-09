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
            var i = new Inmueble
            {
                id = r.GetInt32("id"),
                id_propietario = r.GetInt32("id_propietario"),
                direccion = r.GetString("direccion"),
                ambientes = r.IsDBNull(r.GetOrdinal("ambientes")) ? (int?)null : r.GetInt32("ambientes"),
                eje_x = r.IsDBNull(r.GetOrdinal("eje_x")) ? (double?)null : r.GetDouble("eje_x"),
                eje_y = r.IsDBNull(r.GetOrdinal("eje_y")) ? (double?)null : r.GetDouble("eje_y"),
                precio = r.IsDBNull(r.GetOrdinal("precio")) ? (decimal?)null : r.GetDecimal("precio"),
                imagen = r.IsDBNull(r.GetOrdinal("imagen")) ? null : r.GetString("imagen"),
                superficie = r.IsDBNull(r.GetOrdinal("superficie")) ? (double?)null : r.GetDouble("superficie"),
            };

            var tipoStr = r.GetString("tipo");
            var usoStr = r.GetString("uso");
            var estadoStr = r.GetString("estado");

            i.tipo = Enum.TryParse<TipoInmueble>(tipoStr, true, out var t) ? t : TipoInmueble.Casa;
            i.uso = Enum.TryParse<UsoInmueble>(usoStr, true, out var u) ? u : UsoInmueble.Residencial;
            i.estado = Enum.TryParse<Estado>(estadoStr, true, out var e) ? e : Estado.Disponible;
            return i;
        }
        public List<Inmueble> ObtenerPorPropietario(int propietarioId)
        {
            var lista = new List<Inmueble>();
            using (var cn = new MySqlConnection(connectionString))
            {
                const string sql = @"SELECT id, id_propietario, tipo, direccion, uso, ambientes, eje_x, eje_y, precio, estado, imagen, superficie
                                     FROM inmueble
                                     WHERE id_propietario = @pid";
                using (var cmd = new MySqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@pid", propietarioId);
                    cn.Open();
                    using var r = cmd.ExecuteReader();
                    while (r.Read()) lista.Add(Map(r));
                }
            }
            return lista;
        }
        public Inmueble ObtenerInmuebleId(int id)
        {
            Inmueble i = new();
            using (var cn = new MySqlConnection(connectionString))
            {
                const string sql = @"SELECT id, id_propietario, tipo, direccion, uso, ambientes, eje_x, eje_y, precio, estado, imagen, superficie
                                     FROM inmueble
                                     WHERE id = @id";
                using (var cmd = new MySqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cn.Open();
                    using var r = cmd.ExecuteReader();
                    if (r.Read()) i = Map(r);
                }
            }
            return i;
        }
        public Inmueble AgregarInmueble(Inmueble i)
        {
            var estadoCreacion = Estado.Suspendido;

            using (var cn = new MySqlConnection(connectionString))
            {
                const string sql = @"INSERT INTO inmueble
                                    (id_propietario, tipo, direccion, uso, ambientes, eje_x, eje_y, precio, estado, imagen, superficie)
                                    VALUES (@prop, @tipo, @dir, @uso, @amb, @x, @y, @precio, @estado, @img, @sup);
                                    SELECT LAST_INSERT_ID();";
                using (var cmd = new MySqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@prop", i.id_propietario);
                    cmd.Parameters.AddWithValue("@tipo", i.tipo.ToString());
                    cmd.Parameters.AddWithValue("@dir", i.direccion);
                    cmd.Parameters.AddWithValue("@uso", i.uso.ToString());
                    cmd.Parameters.AddWithValue("@amb", (object?)i.ambientes ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@x", (object?)i.eje_x ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@y", (object?)i.eje_y ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@precio", (object?)i.precio ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@estado", estadoCreacion.ToString());
                    cmd.Parameters.AddWithValue("@img", (object?)i.imagen ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@sup", (object?)i.superficie ?? DBNull.Value);

                    cn.Open();
                    i.id = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            i.estado = estadoCreacion;
            return i;
        }
        public Inmueble ActualizarInmueble(Inmueble i)
        {
            using (var cn = new MySqlConnection(connectionString))
            {
                const string sql = @"UPDATE inmueble SET
                                     tipo=@tipo, direccion=@dir, uso=@uso, ambientes=@amb,
                                     eje_x=@x, eje_y=@y, precio=@precio, estado=@estado, superficie=@sup
                                     WHERE id=@id";
                using (var cmd = new MySqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@id", i.id);
                    cmd.Parameters.AddWithValue("@tipo", i.tipo.ToString());
                    cmd.Parameters.AddWithValue("@dir", i.direccion);
                    cmd.Parameters.AddWithValue("@uso", i.uso.ToString());
                    cmd.Parameters.AddWithValue("@amb", (object?)i.ambientes ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@x", (object?)i.eje_x ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@y", (object?)i.eje_y ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@precio", (object?)i.precio ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@estado", i.estado.ToString());
                    cmd.Parameters.AddWithValue("@sup", (object?)i.superficie ?? DBNull.Value);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return ObtenerInmuebleId(i.id);
        }
        public bool ActualizarEstado(int id, Estado estado)
        {
            using (var cn = new MySqlConnection(connectionString))
            {
                const string sql = "UPDATE inmueble SET estado = @est WHERE id = @id";
                using (var cmd = new MySqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@est", estado.ToString());
                    cn.Open();
                    var rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
            }
        }
        public void MarcarComoAlquilado(int id)
        {
            string sql = "UPDATE inmueble SET estado=@estado WHERE id=@id";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@estado", (int)Estado.Alquilado);
                cmd.Parameters.AddWithValue("@id", id);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }

        }
    }
}
