using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Model
{
    public class ConversacionRepository : IRepository<conversacion>
    {
        private string DbConnectionString { get; set; }

        public ConversacionRepository()
        {
            var configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("config.json", false)
                   .Build();
            var connectionString = configuration.GetSection("connectionString").Value;
            this.DbConnectionString = connectionString;
        }


        public string Delete(conversacion employee)
        {
            throw new NotImplementedException();
        }

        public conversacion GetById(int? id)
        {
            throw new NotImplementedException();
        }

        public IList<conversacion> Get()
        {
            List<conversacion> conversaciones = new List<conversacion>();
            try
            {
                var resultTable = new DataTable();
                using (var conn = new SqlConnection(this.DbConnectionString))
                {
                    var sql = "SELECT * FROM conversacion";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(resultTable);
                        }
                    }
                }
                foreach (DataRow item in resultTable.Rows)
                {
                    var result = new conversacion()
                    {
                        id = item.Field<long>("id"),
                        intension = item.Field<string>("intension"),
                        respuesta = item.Field<string>("respuesta"),
                        identificacion = item.Field<string>("identificacion"),
                        solicitud = item.Field<string>("solicitud"),
                        fecha = item.Field<DateTime>("fecha")
                    };
                    conversaciones.Add(result);
                }
            }
            catch (Exception)
            {
                conversaciones = new List<conversacion>();
            }
            return conversaciones;
        }

        public string InsertNew(conversacion entity)
        {
            string errorMessage = string.Empty;
            try
            {
                using (SqlConnection connection = new SqlConnection(this.DbConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO conversacion(identificacion,intension,fue_solucionado, solicitud,respuesta,fecha) VALUES(@param1,@param2,@param3,@param4,@param5,@param6)";
                        command.Parameters.AddWithValue("@param1", entity.identificacion);
                        command.Parameters.AddWithValue("@param2", entity.intension);
                        command.Parameters.AddWithValue("@param3", entity.fue_solucionado);
                        command.Parameters.AddWithValue("@param4", entity.solicitud);
                        command.Parameters.AddWithValue("@param5", entity.respuesta);
                        command.Parameters.AddWithValue("@param6", entity.fecha);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException exc)
            {
                errorMessage = exc.Message;
            }
            return errorMessage;
        }

        public string Update(conversacion employee)
        {
            throw new NotImplementedException();
        }
    }
}
