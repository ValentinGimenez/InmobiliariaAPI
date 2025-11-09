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
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT id, nombre, apellido, dni, email, telefono, estado, clave 
                            FROM propietario 
                            WHERE id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@id", id);
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
                        propietario.clave = reader.IsDBNull(reader.GetOrdinal("clave")) ? null : reader.GetString("clave"); // NUEVO
                    }
                }
            }
            return propietario;
        }
        public Propietario ActualizarPropietario(Propietario propietario)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"UPDATE propietario 
                            SET nombre = @nombre, apellido = @apellido, dni = @dni, 
                                email = @email, telefono = @telefono 
                            WHERE id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@id", propietario.id);
                    command.Parameters.AddWithValue("@nombre", propietario.nombre);
                    command.Parameters.AddWithValue("@apellido", propietario.apellido);
                    command.Parameters.AddWithValue("@dni", propietario.dni);
                    command.Parameters.AddWithValue("@email", propietario.email);
                    command.Parameters.AddWithValue("@telefono", propietario.telefono);
                    command.ExecuteNonQuery();
                }
            }
            return propietario;
        }
        public bool CambiarPassword(int id, string hashClave)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"UPDATE propietario SET clave = @clave WHERE id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@clave", hashClave);
                    int rows = command.ExecuteNonQuery();
                    connection.Close();
                    return rows > 0;
                }
            }
        }
        public void AgregarPropietario(Propietario propietario)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"INSERT INTO propietario 
                            (nombre, apellido, dni, email, telefono, estado, clave) 
                            VALUES (@nombre, @apellido, @dni, @email, @telefono, @estado, @clave)";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@nombre", propietario.nombre);
                    command.Parameters.AddWithValue("@apellido", propietario.apellido);
                    command.Parameters.AddWithValue("@dni", propietario.dni);
                    command.Parameters.AddWithValue("@email", propietario.email);
                    command.Parameters.AddWithValue("@telefono", propietario.telefono);
                    command.Parameters.AddWithValue("@estado", propietario.estado);
                    command.Parameters.AddWithValue("@clave", propietario.clave);
                    command.ExecuteNonQuery();
                }
            }
        }
        public Propietario? ObtenerPorEmail(string email)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT id, nombre, apellido, dni, email, telefono, estado, clave
                            FROM propietario
                            WHERE email = @email
                            LIMIT 1";
                using (var cmd = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    cmd.Parameters.AddWithValue("@email", email);
                    using (var r = cmd.ExecuteReader())
                    {
                        if (!r.Read()) return null;
                        return new Propietario
                        {
                            id = r.GetInt32("id"),
                            nombre = r.GetString("nombre"),
                            apellido = r.GetString("apellido"),
                            dni = r.GetString("dni"),
                            email = r.GetString("email"),
                            telefono = r.GetString("telefono"),
                            estado = r.GetInt32("estado"),
                            clave = r.IsDBNull(r.GetOrdinal("clave")) ? null : r.GetString("clave"),
                        };
                    }
                }
            }
        }
    }
}
