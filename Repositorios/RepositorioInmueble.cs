using _net_integrador.Models;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace _net_integrador.Repositorios
{
    public class RepositorioInmueble : RepositorioBase, IRepositorioInmueble
    {

        public RepositorioInmueble(IConfiguration configuration) : base(configuration) { }

        public List<Inmueble> ObtenerInmuebles()
        {
            List<Inmueble> inmuebles = new List<Inmueble>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var query = @"SELECT 
                                i.id, i.direccion, i.uso, i.id_tipo, i.precio, i.estado, 
                                i.ambientes, i.eje_x, i.eje_y,
                                p.nombre AS nombrePropietario, p.apellido AS apellidoPropietario, 
                                t.tipo AS tipoInmueble
                            FROM inmueble i 
                            JOIN propietario p ON i.id_propietario = p.id AND p.estado = 1 
                            JOIN tipo_inmueble t ON i.id_tipo = t.id";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        inmuebles.Add(new Inmueble
                        {
                            id = reader.GetInt32("id"),
                            uso = Enum.Parse<UsoInmueble>(reader.GetString("uso")),
                            id_tipo = reader.GetInt32("id_tipo"),
                            precio = reader.GetDecimal("precio"),
                            estado = Enum.Parse<Estado>(reader.GetString("estado")),
                            direccion = reader.GetString("direccion"),
                            ambientes = reader.GetInt32("ambientes"),
                            eje_x = reader.GetString("eje_x"),
                            eje_y = reader.GetString("eje_y"),
                            propietario = new Propietario
                            {
                                nombre = reader.GetString("nombrePropietario"),
                                apellido = reader.GetString("apellidoPropietario")
                            },
                            tipoInmueble = new TipoInmueble
                            {
                                id = reader.GetInt32("id_tipo"),
                                tipo = reader.GetString("tipoInmueble")
                            }
                        });
                    }
                    connection.Close();
                }
            }
            return inmuebles;
        }


        public Inmueble? ObtenerInmuebleId(int id)
        {
            Inmueble? inmueble = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var sql = "SELECT id, id_propietario, direccion, uso, id_tipo, ambientes, eje_x, eje_y, precio, estado FROM inmueble WHERE id = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@id", id);
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        inmueble = new Inmueble
                        {
                            id = reader.GetInt32("id"),
                            id_propietario = reader.GetInt32("id_propietario"),
                            direccion = reader.GetString("direccion"),
                            uso = Enum.Parse<UsoInmueble>(reader.GetString("uso")),
                            id_tipo = reader.GetInt32("id_tipo"),
                            ambientes = reader.GetInt32("ambientes"),
                            eje_x = reader.GetString("eje_x"),
                            eje_y = reader.GetString("eje_y"),
                            precio = reader.GetDecimal("precio"),
                            estado = Enum.Parse<Estado>(reader.GetString("estado"))

                        };
                    }
                    connection.Close();
                }
            }
            return inmueble;
        }

        public void AgregarInmueble(Inmueble inmuebleNuevo)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    var sql = "INSERT INTO inmueble (id_propietario, direccion, uso, id_tipo, ambientes, eje_x, eje_y, precio, estado) VALUES (@id_propietario, @direccion, @uso, @id_tipo, @ambientes, @eje_x, @eje_y, @precio, @estado)";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        connection.Open();
                        command.Parameters.AddWithValue("@id_propietario", inmuebleNuevo.id_propietario);
                        command.Parameters.AddWithValue("@direccion", inmuebleNuevo.direccion);
                        command.Parameters.AddWithValue("@uso", inmuebleNuevo.uso.ToString());
                        command.Parameters.AddWithValue("@id_tipo", inmuebleNuevo.id_tipo);
                        command.Parameters.AddWithValue("@ambientes", inmuebleNuevo.ambientes);
                        command.Parameters.AddWithValue("@eje_x", inmuebleNuevo.eje_x);
                        command.Parameters.AddWithValue("@eje_y", inmuebleNuevo.eje_y);
                        command.Parameters.AddWithValue("@precio", inmuebleNuevo.precio);
                        command.Parameters.AddWithValue("@estado", (int)Estado.Disponible);
                        command.ExecuteNonQuery();
                        connection.Close();
                        Console.WriteLine("Inmueble agregado correctamente.");
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error SQL: {ex.Message}");
            }
        }

        public void ActualizarInmueble(Inmueble inmuebleEditado)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var sql = "UPDATE inmueble SET id_propietario = @id_propietario, direccion = @direccion, uso = @uso, id_tipo = @id_tipo, ambientes = @ambientes, eje_x = @eje_x, eje_y = @eje_y, precio = @precio, estado = @estado WHERE id = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@id", inmuebleEditado.id);
                    command.Parameters.AddWithValue("@id_propietario", inmuebleEditado.id_propietario);
                    command.Parameters.AddWithValue("@direccion", inmuebleEditado.direccion);
                    command.Parameters.AddWithValue("@uso", inmuebleEditado.uso.ToString());
                    command.Parameters.AddWithValue("@id_tipo", inmuebleEditado.id_tipo);
                    command.Parameters.AddWithValue("@ambientes", inmuebleEditado.ambientes);
                    command.Parameters.AddWithValue("@eje_x", inmuebleEditado.eje_x);
                    command.Parameters.AddWithValue("@eje_y", inmuebleEditado.eje_y);
                    command.Parameters.AddWithValue("@precio", inmuebleEditado.precio);
                    command.Parameters.AddWithValue("@estado", inmuebleEditado.estado);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public void SuspenderOferta(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var query = "UPDATE inmueble SET estado = 'Suspendido' WHERE id = @id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }
        public bool ActivarOferta(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var querySelect = "SELECT p.estado FROM inmueble i INNER JOIN propietario p ON i.id_propietario=p.id WHERE i.id=@idInmueble";
                using (var checkCommand = new MySqlCommand(querySelect, connection))
                {
                    checkCommand.Parameters.AddWithValue("@idInmueble", id);

                    object result = checkCommand.ExecuteScalar();

                    if (result == null || Convert.ToInt32(result) == 0)
                    {
                        connection.Close();
                        return false;
                    }
                }
                var query = "UPDATE inmueble SET estado = 'Disponible' WHERE id = @id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
                connection.Close();
                return true;
            }
        }

        public List<Inmueble> ObtenerInmueblesDisponibles()
        {
            List<Inmueble> inmuebles = new List<Inmueble>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var query = @"
                    SELECT 
                        i.id, i.direccion, i.uso, i.id_tipo, i.precio, i.estado, 
                        p.nombre AS nombrePropietario, p.apellido AS apellidoPropietario, 
                        t.tipo AS tipoInmueble
                    FROM inmueble i
                    JOIN propietario p ON i.id_propietario = p.id AND p.estado = 1
                    JOIN tipo_inmueble t ON i.id_tipo = t.id
                    WHERE i.estado = 'Disponible' AND i.id NOT IN (SELECT id_inmueble FROM contrato WHERE estado = 1)";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        inmuebles.Add(new Inmueble
                        {
                            id = reader.GetInt32("id"),
                            uso = Enum.Parse<UsoInmueble>(reader.GetString("uso")),
                            id_tipo = reader.GetInt32("id_tipo"),
                            precio = reader.GetDecimal("precio"),
                            estado = Enum.Parse<Estado>(reader.GetString("estado")),
                            direccion = reader.GetString("direccion"),
                            propietario = new Propietario
                            {
                                nombre = reader.GetString("nombrePropietario"),
                                apellido = reader.GetString("apellidoPropietario")
                            },
                            tipoInmueble = new TipoInmueble
                            {
                                id = reader.GetInt32("id_tipo"),
                                tipo = reader.GetString("tipoInmueble")
                            }
                        });
                    }
                    connection.Close();
                }
            }
            return inmuebles;
        }
        public List<Inmueble> ObtenerInmueblesPorPropietario(int propietarioId)
        {
            List<Inmueble> inmuebles = new List<Inmueble>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var query = "SELECT id, id_propietario, direccion, uso, id_tipo, ambientes, eje_x, eje_y, precio, estado FROM inmueble WHERE id_propietario = @propietarioId";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@propietarioId", propietarioId);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        inmuebles.Add(new Inmueble
                        {
                            id = reader.GetInt32("id"),
                            id_propietario = reader.GetInt32("id_propietario"),
                            direccion = reader.GetString("direccion"),
                            uso = Enum.Parse<UsoInmueble>(reader.GetString("uso")),
                            id_tipo = reader.GetInt32("id_tipo"),
                            ambientes = reader.GetInt32("ambientes"),
                            eje_x = reader.GetString("eje_x"),
                            eje_y = reader.GetString("eje_y"),
                            precio = reader.GetDecimal("precio"),
                            estado = Enum.Parse<Estado>(reader.GetString("estado"))
                        });
                    }
                    connection.Close();
                }
            }
            return inmuebles;
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
        public List<Inmueble> BuscarDisponiblePorFecha(DateTime inicio, DateTime fin)
        {
            var inmuebles = new List<Inmueble>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT id, direccion, id_propietario, uso, id_tipo, ambientes, eje_x, eje_y, precio, estado FROM inmueble WHERE estado!=2 AND id NOT IN (SELECT id_inmueble FROM contrato WHERE NOT (@fin <= fecha_inicio OR @inicio >= fecha_fin) AND (fecha_terminacion_anticipada IS NULL OR fecha_terminacion_anticipada > @inicio) AND estado = 1)";

                using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@inicio", inicio);
                    cmd.Parameters.AddWithValue("@fin", fin);

                    connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inmuebles.Add(new Inmueble
                            {
                                id = reader.GetInt32("id"),
                                direccion = reader.GetString("direccion"),
                                id_propietario = reader.GetInt32("id_propietario"),
                                uso = Enum.Parse<UsoInmueble>(reader.GetString("uso")),
                                id_tipo = reader.GetInt32("id_tipo"),
                                ambientes = reader.GetInt32("ambientes"),
                                eje_x = reader.GetString("eje_x"),
                                eje_y = reader.GetString("eje_y"),
                                precio = reader.GetDecimal("precio"),
                                estado = Enum.Parse<Estado>(reader.GetString("estado"))
                            });
                        }
                    }
                    connection.Close();
                }
            }
            return inmuebles;
        }


    }

}

