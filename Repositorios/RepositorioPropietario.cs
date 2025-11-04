using _net_integrador.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace _net_integrador.Repositorios
{
    public class RepositorioPropietario : RepositorioBase, IRepositorioPropietario
    {
        public RepositorioPropietario(IConfiguration configuration) : base(configuration) { }

        public Propietario ObtenerPropietarioId(int id)
        {
            Propietario propietario = new Propietario();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var sql = "SELECT id, nombre, apellido, dni, email, telefono, estado, clave " +
                          "FROM propietario WHERE id = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        propietario.id = reader.GetInt32("id");
                        propietario.nombre = reader.GetString("nombre");
                        propietario.apellido = reader.GetString("apellido");
                        propietario.dni = reader.GetString("dni");
                        propietario.email = reader.GetString("email");
                        propietario.telefono = reader.GetString("telefono");
                        propietario.estado = reader.GetInt32("estado");

                        int idxClave = reader.GetOrdinal("clave");
                        propietario.clave = reader.IsDBNull(idxClave) ? null : reader.GetString(idxClave);
                    }
                    connection.Close();
                }
            }
            return propietario;
        }

        public Propietario ActualizarPropietario(Propietario propietario)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var sql = "UPDATE propietario SET nombre=@nombre, apellido=@apellido, dni=@dni, email=@email, telefono=@tel " +
                          "WHERE id=@id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", propietario.id);
                    command.Parameters.AddWithValue("@nombre", propietario.nombre);
                    command.Parameters.AddWithValue("@apellido", propietario.apellido);
                    command.Parameters.AddWithValue("@dni", propietario.dni);
                    command.Parameters.AddWithValue("@email", propietario.email);
                    command.Parameters.AddWithValue("@tel", propietario.telefono);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return propietario;
        }

        public Propietario ObtenerPorEmail(string email)
        {
            Propietario propietario = new Propietario();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var sql = "SELECT id, nombre, apellido, dni, email, telefono, estado, clave " +
                          "FROM propietario WHERE email = @mail AND estado = 1 LIMIT 1";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@mail", email);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        propietario.id = reader.GetInt32("id");
                        propietario.nombre = reader.GetString("nombre");
                        propietario.apellido = reader.GetString("apellido");
                        propietario.dni = reader.GetString("dni");
                        propietario.email = reader.GetString("email");
                        propietario.telefono = reader.GetString("telefono");
                        propietario.estado = reader.GetInt32("estado");

                        int idxClave = reader.GetOrdinal("clave");
                        propietario.clave = reader.IsDBNull(idxClave) ? null : reader.GetString(idxClave);
                    }
                    connection.Close();
                }
            }
            return propietario;
        }

        public bool CambiarPassword(int id, string nueva)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string hash = BCrypt.Net.BCrypt.HashPassword(nueva);

                var sql = "UPDATE propietario SET clave=@nueva WHERE id=@id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@nueva", hash);
                    connection.Open();
                    int rows = command.ExecuteNonQuery();
                    connection.Close();
                    return rows > 0;
                }
            }
        }

        public int AgregarPropietario(Propietario propietario)
        {
            int idGenerado = 0;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string hash = BCrypt.Net.BCrypt.HashPassword(propietario.clave ?? "");

                var sql = "INSERT INTO propietario (nombre, apellido, dni, email, telefono, estado, clave) " +
                          "VALUES (@nombre, @apellido, @dni, @email, @telefono, @estado, @clave); SELECT LAST_INSERT_ID();";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", propietario.nombre);
                    command.Parameters.AddWithValue("@apellido", propietario.apellido);
                    command.Parameters.AddWithValue("@dni", propietario.dni);
                    command.Parameters.AddWithValue("@email", propietario.email);
                    command.Parameters.AddWithValue("@telefono", propietario.telefono);
                    command.Parameters.AddWithValue("@estado", propietario.estado);
                    command.Parameters.AddWithValue("@clave", hash);

                    connection.Open();
                    idGenerado = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
            }
            return idGenerado;
        }
    }
}
