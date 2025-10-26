using _net_integrador.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace _net_integrador.Repositorios;

public class RepositorioPago : RepositorioBase, IRepositorioPago
{
    public RepositorioPago(IConfiguration configuration) : base(configuration) { }

    public List<Pago> ObtenerPagosPorContrato(int contratoId)
    {
        List<Pago> pagos = new List<Pago>();
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var sql = "SELECT id, id_contrato, nro_pago, fecha_pago, estado, concepto FROM pago WHERE id_contrato = @id_contrato";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@id_contrato", contratoId);
                var reader = command.ExecuteReader();

                int idxId = reader.GetOrdinal("id");
                int idxIdContrato = reader.GetOrdinal("id_contrato");
                int idxNroPago = reader.GetOrdinal("nro_pago");
                int idxFechaPago = reader.GetOrdinal("fecha_pago");
                int idxEstado = reader.GetOrdinal("estado");
                int idxConcepto = reader.GetOrdinal("concepto");

                while (reader.Read())
                {
                    DateTime? fechaPago = null;
                    var fechaString = reader.IsDBNull(idxFechaPago) ? null : reader.GetValue(idxFechaPago).ToString();
                    if (!string.IsNullOrEmpty(fechaString) && fechaString != "0000-00-00")
                    {
                        fechaPago = DateTime.Parse(fechaString);
                    }

                    pagos.Add(new Pago
                    {
                        id = reader.GetInt32(idxId),
                        id_contrato = reader.GetInt32(idxIdContrato),
                        nro_pago = reader.GetInt32(idxNroPago),
                        fecha_pago = fechaPago,
                        estado = reader.GetString(idxEstado) switch
                        {
                            "pendiente" => EstadoPago.pendiente,
                            "recibido" => EstadoPago.recibido,
                            "anulado" => EstadoPago.anulado,
                            _ => throw new ArgumentException("Estado de pago no reconocido")
                        },
                        concepto = reader.GetString(idxConcepto)
                    });
                }
                connection.Close();
            }
        }
        return pagos;
    }


    public Pago? ObtenerPagoId(int id)
    {
        Pago? pago = null;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var sql = "SELECT id, id_contrato, nro_pago, fecha_pago, estado, concepto FROM pago WHERE id = @id";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@id", id);
                var reader = command.ExecuteReader();

                int idxId = reader.GetOrdinal("id");
                int idxIdContrato = reader.GetOrdinal("id_contrato");
                int idxNroPago = reader.GetOrdinal("nro_pago");
                int idxFechaPago = reader.GetOrdinal("fecha_pago");
                int idxEstado = reader.GetOrdinal("estado");
                int idxConcepto = reader.GetOrdinal("concepto");

                if (reader.Read())
                {
                    DateTime? fechaPago = null;
                    var fechaString = reader.IsDBNull(idxFechaPago) ? null : reader.GetValue(idxFechaPago).ToString();
                    if (!string.IsNullOrEmpty(fechaString) && fechaString != "0000-00-00")
                    {
                        fechaPago = DateTime.Parse(fechaString);
                    }

                    pago = new Pago
                    {
                        id = reader.GetInt32(idxId),
                        id_contrato = reader.GetInt32(idxIdContrato),
                        nro_pago = reader.GetInt32(idxNroPago),
                        fecha_pago = fechaPago,
                        estado = reader.GetString(idxEstado) switch
                        {
                            "pendiente" => EstadoPago.pendiente,
                            "recibido" => EstadoPago.recibido,
                            "anulado" => EstadoPago.anulado,
                            _ => throw new ArgumentException("Estado de pago no reconocido")
                        },
                        concepto = reader.GetString(idxConcepto)
                    };
                }
                connection.Close();
            }
        }
        return pago;
    }



    public void AgregarPago(Pago pago)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var sql = "INSERT INTO pago (id_contrato, nro_pago, fecha_pago, estado, concepto) VALUES (@id_contrato, @nro_pago, @fecha_pago, @estado, @concepto)";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@id_contrato", pago.id_contrato);
                command.Parameters.AddWithValue("@nro_pago", pago.nro_pago);
                command.Parameters.AddWithValue("@fecha_pago", pago.fecha_pago);
                command.Parameters.AddWithValue("@estado", pago.estado.ToString());
                command.Parameters.AddWithValue("@concepto", pago.concepto);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    public void AnularPago(int id)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var query = "UPDATE pago SET estado = 'anulado' WHERE id = @id";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    public void ActualizarPago(Pago pago)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var sql = "UPDATE pago SET nro_pago = @nro_pago, fecha_pago = @fecha_pago, estado = @estado, concepto = @concepto WHERE id = @id";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@id", pago.id);
                command.Parameters.AddWithValue("@nro_pago", pago.nro_pago);
                command.Parameters.AddWithValue("@fecha_pago", pago.fecha_pago);
                command.Parameters.AddWithValue("@estado", pago.estado.ToString());
                command.Parameters.AddWithValue("@concepto", pago.concepto);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
  public DateTime? ObtenerFechaUltimoPagoRealizado(int contratoId)
    {
        DateTime? fechaUltimoPago = null;
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            var sql = @"
            SELECT MAX(fecha_pago) AS UltimaFecha 
            FROM pago 
            WHERE id_contrato = @idContrato AND estado = 'recibido' AND fecha_pago IS NOT NULL";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@idContrato", contratoId);

                var result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    fechaUltimoPago = Convert.ToDateTime(result);
                }
                connection.Close();
            }
        }
        return fechaUltimoPago;
    }
public int ContarPagosRealizados(int idContrato)
    {
        int pagosRealizados = 0;
        
        string query = "SELECT COUNT(*) FROM pago WHERE id_contrato = @idContrato AND estado = 'recibido'";

        using (var connection = new MySqlConnection(connectionString))
        {
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@idContrato", idContrato);

                try
                {
                    connection.Open();
                    object result = command.ExecuteScalar();
                    
                    if (result != null && result != DBNull.Value)
                    {
                        pagosRealizados = Convert.ToInt32(result);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al contar pagos realizados en MySQL: {ex.Message}");
                }
            }
        }

        return pagosRealizados;
    }
}