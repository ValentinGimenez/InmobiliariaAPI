using _net_integrador.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace _net_integrador.Repositorios;

public class RepositorioUsuario : RepositorioBase, IRepositorioUsuario
{
    public RepositorioUsuario(IConfiguration configuration) : base(configuration) { }

    public List<Usuario> ObtenerUsuarios()
    {
        List<Usuario> usuarios = new List<Usuario>();
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = "SELECT id, nombre, apellido, dni, email, password, rol, estado, avatar FROM usuario";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    usuarios.Add(new Usuario
                    {
                        id = reader.GetInt32("id"),
                        nombre = reader.GetString("nombre"),
                        apellido = reader.GetString("apellido"),
                        dni = reader.GetString("dni"),
                        email = reader.GetString("email"),
                        password = reader.GetString("password"),
                        rol = reader.GetString("rol"),
                        estado = reader.GetInt32("estado"),
                        avatar = reader.IsDBNull(reader.GetOrdinal("avatar")) ? null : reader.GetString("avatar")
                    });
                }
                connection.Close();
            }
        }
        return usuarios;
    }

    public Usuario? ObtenerUsuarioId(int id)
    {
        Usuario? usuario = null;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var sql = "SELECT id, nombre, apellido, dni, email, password, rol, estado, avatar FROM usuario WHERE id = @id";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@id", id);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    usuario = new Usuario
                    {
                        id = reader.GetInt32("id"),
                        nombre = reader.GetString("nombre"),
                        apellido = reader.GetString("apellido"),
                        dni = reader.GetString("dni"),
                        email = reader.GetString("email"),
                        password = reader.GetString("password"),
                        rol = reader.GetString("rol"),
                        estado = reader.GetInt32("estado"),
                        avatar = reader.IsDBNull(reader.GetOrdinal("avatar")) ? null : reader.GetString("avatar")
                    };
                }
                connection.Close();
            }
        }
        return usuario;
    }
    
    public Usuario? ObtenerUsuarioEmail(string email)
    {
        Usuario? usuario = null;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var sql = "SELECT id, nombre, apellido, dni, email, password, rol, estado, avatar FROM usuario WHERE email = @email";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@email", email);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    usuario = new Usuario
                    {
                        id = reader.GetInt32("id"),
                        nombre = reader.GetString("nombre"),
                        apellido = reader.GetString("apellido"),
                        dni = reader.GetString("dni"),
                        email = reader.GetString("email"),
                        password = reader.GetString("password"),
                        rol = reader.GetString("rol"),
                        estado = reader.GetInt32("estado"),
                        avatar = reader.IsDBNull(reader.GetOrdinal("avatar")) ? null : reader.GetString("avatar")
                    };
                }
                connection.Close();
            }
        }
        return usuario;
    }

    public void AgregarUsuario(Usuario usuario)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var sql = "INSERT INTO usuario (nombre, apellido, dni, email, password, rol, estado, avatar) VALUES (@nombre, @apellido, @dni, @email, @password, @rol, @estado, @avatar)";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@nombre", usuario.nombre);
                command.Parameters.AddWithValue("@apellido", usuario.apellido);
                command.Parameters.AddWithValue("@dni", usuario.dni);
                command.Parameters.AddWithValue("@email", usuario.email);
                command.Parameters.AddWithValue("@password", usuario.password);
                command.Parameters.AddWithValue("@rol", usuario.rol);
                command.Parameters.AddWithValue("@estado", 1);
                command.Parameters.AddWithValue("@avatar", usuario.avatar);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    public void ActualizarUsuario(Usuario usuario)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var sql = "UPDATE usuario SET nombre = @nombre, apellido = @apellido, dni = @dni, email = @email, password = @password, rol = @rol, estado = @estado, avatar = @avatar WHERE id = @id";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@id", usuario.id);
                command.Parameters.AddWithValue("@nombre", usuario.nombre);
                command.Parameters.AddWithValue("@apellido", usuario.apellido);
                command.Parameters.AddWithValue("@dni", usuario.dni);
                command.Parameters.AddWithValue("@email", usuario.email);
                command.Parameters.AddWithValue("@password", usuario.password);
                command.Parameters.AddWithValue("@rol", usuario.rol);
                command.Parameters.AddWithValue("@estado", usuario.estado);
                command.Parameters.AddWithValue("@avatar", usuario.avatar);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    public void EliminarUsuario(int id)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = "UPDATE usuario SET estado = 0 WHERE id = @id";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    public void ActivarUsuario(int id)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = "UPDATE usuario SET estado = 1 WHERE id = @id";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}