using System;
using System.Collections.Generic;
using _net_integrador.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace _net_integrador.Repositorios
{
    public class RepositorioInquilino : RepositorioBase, IRepositorioInquilino
    {
        public RepositorioInquilino(IConfiguration configuration) : base(configuration) { }

        private Inquilino Map(MySqlDataReader r)
        {
            return new Inquilino
            {
                id       = r.GetInt32("id"),
                nombre   = r.GetString("nombre"),
                apellido = r.GetString("apellido"),
                dni      = r.GetString("dni"),
                email    = r.GetString("email"),
                telefono = r.GetString("telefono"),
                estado   = r.GetInt32("estado"),
                imagen   = r.IsDBNull(r.GetOrdinal("imagen")) ? null : r.GetString("imagen")
            };
        }

        public List<Inquilino> ObtenerInquilinos()
        {
            var inquilinos = new List<Inquilino>();
            using (var connection = new MySqlConnection(connectionString))
            {
                const string query = @"SELECT id, nombre, apellido, dni, email, telefono, estado, imagen
                                       FROM inquilino";
                using (var command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                        inquilinos.Add(Map(reader));
                    connection.Close();
                }
            }
            return inquilinos;
        }

        public Inquilino? ObtenerInquilinoId(int id)
        {
            Inquilino? inquilino = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                const string sql = @"SELECT id, nombre, apellido, dni, email, telefono, estado, imagen
                                     FROM inquilino WHERE id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using var reader = command.ExecuteReader();
                    if (reader.Read())
                        inquilino = Map(reader);
                    connection.Close();
                }
            }
            return inquilino;
        }

        public Inquilino ActualizarInquilino(Inquilino inquilino)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                const string sql = @"
                    UPDATE inquilino
                       SET nombre   = @nombre,
                           apellido = @apellido,
                           dni      = @dni,
                           email    = @email,
                           telefono = @telefono,
                           estado   = @estado,
                           imagen   = @imagen
                     WHERE id       = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", inquilino.id);
                    command.Parameters.AddWithValue("@nombre", inquilino.nombre);
                    command.Parameters.AddWithValue("@apellido", inquilino.apellido);
                    command.Parameters.AddWithValue("@dni", inquilino.dni);
                    command.Parameters.AddWithValue("@email", inquilino.email);
                    command.Parameters.AddWithValue("@telefono", inquilino.telefono);
                    command.Parameters.AddWithValue("@estado", inquilino.estado);
                    command.Parameters.AddWithValue("@imagen", (object?)inquilino.imagen ?? DBNull.Value);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return inquilino;
        }

        public bool EliminarInquilino(int id)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                const string checkQuery = "SELECT COUNT(*) FROM contrato WHERE id_inquilino = @id AND estado = 1";
                using (var checkCommand = new MySqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@id", id);
                    int contratosActivos = Convert.ToInt32(checkCommand.ExecuteScalar());
                    if (contratosActivos > 0)
                        return false;
                }

                const string updateQuery = "UPDATE inquilino SET estado = 0 WHERE id = @id";
                using (var updateCommand = new MySqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@id", id);
                    updateCommand.ExecuteNonQuery();
                }

                connection.Close();
            }
            return true;
        }

        public void ActivarInquilino(int id)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                const string query = "UPDATE inquilino SET estado = 1 WHERE id = @id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public void AgregarInquilino(Inquilino inquilino)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                const string sql = @"
                    INSERT INTO inquilino
                        (nombre, apellido, dni, email, telefono, estado, imagen)
                    VALUES
                        (@nombre, @apellido, @dni, @email, @telefono, @estado, @imagen)";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", inquilino.nombre);
                    command.Parameters.AddWithValue("@apellido", inquilino.apellido);
                    command.Parameters.AddWithValue("@dni", inquilino.dni);
                    command.Parameters.AddWithValue("@email", inquilino.email);
                    command.Parameters.AddWithValue("@telefono", inquilino.telefono);
                    command.Parameters.AddWithValue("@estado", inquilino.estado);
                    command.Parameters.AddWithValue("@imagen", (object?)inquilino.imagen ?? DBNull.Value);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public bool ExisteDni(string dni, int? idExcluido = null)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "SELECT COUNT(*) FROM inquilino WHERE dni = @dni";
                if (idExcluido.HasValue) sql += " AND id != @id";
                using (var cmd = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    cmd.Parameters.AddWithValue("@dni", dni);
                    if (idExcluido.HasValue) cmd.Parameters.AddWithValue("@id", idExcluido.Value);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        public bool ExisteEmail(string email, int? idExcluido = null)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "SELECT COUNT(*) FROM inquilino WHERE email = @email";
                if (idExcluido.HasValue) sql += " AND id != @id";
                using (var cmd = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    cmd.Parameters.AddWithValue("@email", email);
                    if (idExcluido.HasValue) cmd.Parameters.AddWithValue("@id", idExcluido.Value);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        public List<Inquilino> ObtenerInquilinosActivos()
        {
            var inquilinos = new List<Inquilino>();
            using (var connection = new MySqlConnection(connectionString))
            {
                const string query = @"SELECT id, nombre, apellido, dni, email, telefono, estado, imagen
                                       FROM inquilino
                                       WHERE estado != 0";
                using (var command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                        inquilinos.Add(Map(reader));
                    connection.Close();
                }
            }
            return inquilinos;
        }
    }
}
