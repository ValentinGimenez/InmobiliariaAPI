using _net_integrador.Models;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace _net_integrador.Repositorios
{
    public class RepositorioInquilino : RepositorioBase, IRepositorioInquilino
    {
        public RepositorioInquilino(IConfiguration configuration) : base(configuration) { }

        public List<Inquilino> ObtenerInquilinos()
        {
            List<Inquilino> inquilinos = new List<Inquilino>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var query = "SELECT id, nombre, apellido, dni, email, telefono, estado FROM inquilino";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Inquilino inquilino = new Inquilino();
                        inquilino.id = reader.GetInt32("id");
                        inquilino.nombre = reader.GetString("nombre");
                        inquilino.apellido = reader.GetString("apellido");
                        inquilino.dni = reader.GetString("dni");
                        inquilino.email = reader.GetString("email");
                        inquilino.telefono = reader.GetString("telefono");
                        inquilino.estado = reader.GetInt32("estado");
                        inquilinos.Add(inquilino);
                    }
                    connection.Close();
                }
            }
            return inquilinos;
        }

        public Inquilino? ObtenerInquilinoId(int id)
        {
            Inquilino? inquilino = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var sql = "SELECT id, nombre, apellido, dni, email, telefono, estado FROM inquilino WHERE id = @id";
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
                            estado = reader.GetInt32("estado")
                        };
                    }
                    connection.Close();
                }
            }
            return inquilino;
        }

        public Inquilino ActualizarInquilino(Inquilino inquilino)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var sql = @"UPDATE inquilino 
                            SET nombre = @nombre, apellido = @apellido, dni = @dni, 
                                email = @email, telefono = @telefono, estado = @estado 
                            WHERE id = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", inquilino.id);
                    command.Parameters.AddWithValue("@nombre", inquilino.nombre);
                    command.Parameters.AddWithValue("@apellido", inquilino.apellido);
                    command.Parameters.AddWithValue("@dni", inquilino.dni);
                    command.Parameters.AddWithValue("@email", inquilino.email);
                    command.Parameters.AddWithValue("@telefono", inquilino.telefono);
                    command.Parameters.AddWithValue("@estado", inquilino.estado);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return inquilino;
        }

       public bool EliminarInquilino(int id)
{
    using (MySqlConnection connection = new MySqlConnection(connectionString))
    {
        connection.Open();
        var checkQuery = "SELECT COUNT(*) FROM contrato WHERE id_inquilino = @id AND estado = 1";
        using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
        {
            checkCommand.Parameters.AddWithValue("@id", id);
            int contratosActivos = Convert.ToInt32(checkCommand.ExecuteScalar());

            if (contratosActivos > 0)
            {
                return false; 
            }
        }
        var updateQuery = "UPDATE inquilino SET estado = 0 WHERE id = @id";
        using (MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection))
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
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var query = "UPDATE inquilino SET estado = 1 WHERE id = @id";
                using (MySqlCommand command = new MySqlCommand(query, connection))
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
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var sql = @"INSERT INTO inquilino 
                            (nombre, apellido, dni, email, telefono, estado) 
                            VALUES (@nombre, @apellido, @dni, @email, @telefono, @estado)";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", inquilino.nombre);
                    command.Parameters.AddWithValue("@apellido", inquilino.apellido);
                    command.Parameters.AddWithValue("@dni", inquilino.dni);
                    command.Parameters.AddWithValue("@email", inquilino.email);
                    command.Parameters.AddWithValue("@telefono", inquilino.telefono);
                    command.Parameters.AddWithValue("@estado", inquilino.estado);
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
        public List<Inquilino> ObtenerInquilinosActivos()
        {
            List<Inquilino> inquilinos = new List<Inquilino>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var query = "SELECT id, nombre, apellido, dni, email, telefono, estado FROM inquilino WHERE estado != 0";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Inquilino inquilino = new Inquilino
                        {
                            id = reader.GetInt32("id"),
                            nombre = reader.GetString("nombre"),
                            apellido = reader.GetString("apellido"),
                            dni = reader.GetString("dni"),
                            email = reader.GetString("email"),
                            telefono = reader.GetString("telefono"),
                            estado = reader.GetInt32("estado")
                        };
                        inquilinos.Add(inquilino);
                    }
                    connection.Close();
                }
            }
            return inquilinos;
        }

    }
}
