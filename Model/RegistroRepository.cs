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
    public class RegistroRepository : IRepository<registro>
    {
        private string DbConnectionString { get; set; }

        public RegistroRepository()
        {
            var configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("config.json", false)
                   .Build();
            var connectionString = configuration.GetSection("connectionString").Value;
            this.DbConnectionString = connectionString;
        }

        public string Delete(registro employee)
        {
            throw new NotImplementedException();
        }

        public IList<registro> Get()
        {
            List<registro> registros = new List<registro>();
            try
            {
                var resultTable = new DataTable();
                using (var conn = new SqlConnection(this.DbConnectionString))
                {
                    var sql = "SELECT * FROM registro";
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
                    var result = new registro()
                    {
                        id = item.Field<long>("id"),
                        identificacion = item.Field<string>("identificacion"),
                        nombres = item.Field<string>("nombres"),
                        numero_telefonico = item.Field<string>("numero_telefonico"),
                        fecha_registro = item.IsNull("fecha_registro") ? (DateTime?)null : item.Field<DateTime>("fecha_registro")
                    };
                    registros.Add(result);
                }
            }
            catch (Exception)
            {
                registros = new List<registro>();
            }
            return registros;
        }

        /// <summary>
        /// Obtener informacion por ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public registro GetById(int? id)
        {
            registro registro = new registro();
            var registros = this.Get();
            if (id.HasValue && registros.Any())
            {
                registro = registros.FirstOrDefault(i => i.id.Equals(id.Value));
            }
            return registro;
        }

        /// <summary>
        /// Obtener informacion por identificacion
        /// </summary>
        /// <param name="identification"></param>
        /// <returns></returns>
        public registro GetByIdentification(string identification)
        {
            registro registro = new registro();
            var registros = this.Get();
            if (registros.Any())
            {
                registro = registros.LastOrDefault(i => i.identificacion.Equals(identification, StringComparison.InvariantCultureIgnoreCase));
            }
            return registro;
        }

        public string InsertNew(registro employee)
        {
            throw new NotImplementedException();
        }

        public string Update(registro employee)
        {
            throw new NotImplementedException();
        }
    }
}
