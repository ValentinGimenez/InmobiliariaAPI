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

        public Inquilino? ObtenerInquilinoId(int id)
        {
            Inquilino? inquilino = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var sql = "SELECT id, nombre, apellido, dni, email, telefono, estado, imagen FROM inquilino WHERE id = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        inquilino = new Inquilino
                        {
                            id = reader.GetInt32("id"),
                            nombre = reader.GetString("nombre"),
                            apellido = reader.GetString("apellido"),
                            dni = reader.GetString("dni"),
                            email = reader.GetString("email"),
                            telefono = reader.GetString("telefono"),
                            estado = reader.GetInt32("estado"),
                            imagen = reader.IsDBNull(reader.GetOrdinal("imagen")) ? null : reader.GetString("imagen")

                        };
                    }
                    connection.Close();
                }
            }
            return inquilino;
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
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT COUNT(*) FROM inquilino WHERE dni = @dni ";
                if (idExcluido.HasValue)
                    sql += " AND id != @id";
                using (var cmd = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    cmd.Parameters.AddWithValue("@dni", dni);
                    if (idExcluido.HasValue)
                        cmd.Parameters.AddWithValue("@id", idExcluido.Value);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        public bool ExisteEmail(string email, int? idExcluido = null)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT COUNT(*) FROM inquilino WHERE email = @email ";
                if (idExcluido.HasValue)
                    sql += " AND id != @id";
                using (var cmd = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    cmd.Parameters.AddWithValue("@email", email);
                    if (idExcluido.HasValue)
                        cmd.Parameters.AddWithValue("@id", idExcluido.Value);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }
    }
}
