using _net_integrador.Models;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace _net_integrador.Repositorios
{
    public class RepositorioAuditoria : RepositorioBase, IRepositorioAuditoria
    {
        public RepositorioAuditoria(IConfiguration configuration) : base(configuration) { }

        public void InsertarRegistroAuditoria(TipoAuditoria tipo, int idRegistro, AccionAuditoria accion, string usuario)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var sql = @"INSERT INTO auditoria 
                            (tipo, id_registro_afectado, accion, usuario, fecha_hora) 
                            VALUES (@tipo, @id_registro, @accion, @usuario, @fecha_hora)";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@tipo", tipo.ToString());
                    command.Parameters.AddWithValue("@id_registro", idRegistro);
                    command.Parameters.AddWithValue("@accion", accion.ToString());
                    command.Parameters.AddWithValue("@usuario", usuario);
                    command.Parameters.AddWithValue("@fecha_hora", DateTime.Now);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public List<Auditoria> ObtenerAuditorias()
        {
            return ObtenerAuditoriasPorFiltro(null);
        }

        public List<Auditoria> ObtenerAuditoriasPorTipo(TipoAuditoria tipo)
        {
            return ObtenerAuditoriasPorFiltro(tipo);
        }

        private List<Auditoria> ObtenerAuditoriasPorFiltro(TipoAuditoria? tipo)
        {
            var lista = new List<Auditoria>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT id_auditoria, tipo, id_registro_afectado, accion, usuario, fecha_hora 
                            FROM auditoria ";

                if (tipo.HasValue)
                    sql += "WHERE tipo = @tipo ";

                sql += "ORDER BY fecha_hora DESC";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    if (tipo.HasValue)
                        command.Parameters.AddWithValue("@tipo", tipo.Value.ToString());

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var auditoria = new Auditoria
                            {
                                id_auditoria = reader.GetInt32("id_auditoria"),
                                tipo = Enum.Parse<TipoAuditoria>(reader.GetString("tipo")),
                                id_registro_afectado = reader.GetInt32("id_registro_afectado"),
                                accion = Enum.Parse<AccionAuditoria>(reader.GetString("accion")),
                                usuario = reader.GetString("usuario"),
                                fecha_hora = reader.GetDateTime("fecha_hora")
                            };
                            lista.Add(auditoria);
                        }
                    }
                    connection.Close();
                }
            }

            return lista;
        }

        public Auditoria? ObtenerAuditoriaPorId(int id)
        {
            Auditoria? auditoria = null;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT id_auditoria, tipo, id_registro_afectado, accion, usuario, fecha_hora 
                            FROM auditoria
                            WHERE id_auditoria = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            auditoria = new Auditoria
                            {
                                id_auditoria = reader.GetInt32("id_auditoria"),
                                tipo = Enum.Parse<TipoAuditoria>(reader.GetString("tipo")),
                                id_registro_afectado = reader.GetInt32("id_registro_afectado"),
                                accion = Enum.Parse<AccionAuditoria>(reader.GetString("accion")),
                                usuario = reader.GetString("usuario"),
                                fecha_hora = reader.GetDateTime("fecha_hora")
                            };
                        }
                    }
                    connection.Close();
                }
            }

            return auditoria;
        }

        public List<Auditoria> ObtenerAuditoriasContrato()
        {
            return ObtenerAuditoriasPorTipo(TipoAuditoria.Contrato);
        }

        public List<Auditoria> ObtenerAuditoriasPago()
        {
            return ObtenerAuditoriasPorTipo(TipoAuditoria.Pago);
        }
    }
}
