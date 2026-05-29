using System;
using Microsoft.Data.SqlClient;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TPM_QAS.Models.ConnectionString;

namespace TPM_QAS.Helpers.DataModel
{
    public class DatabaseConnectionString
    {
        protected string sql { get; set; }
        protected SqlCommand command;
        protected SqlConnection c;
        public SqlDataReader reader;
        protected SqlTransaction tran;
        protected SqlDataAdapter sqladp;
        protected string Message;

        public async Task<string> OpenAclConnection(string parameter)
        {
            try
            {
                ConnectionString connectionName = new ConnectionString();
                ConnectionString connectionResult = new ConnectionString();

                connectionName.ConnectionStringDBName = parameter;

                using (var client = new HttpClient())
                {
                    ByteArrayContent clientbodystr = new StringContent(JsonConvert.SerializeObject(connectionName), Encoding.UTF8, "application/json");

                    HttpResponseMessage respone = await client.PostAsync("http://cld-tgm-app001.toray.my:140/api/hello/GetConnectionByParams", clientbodystr);

                    respone.EnsureSuccessStatusCode();
                    if (respone.IsSuccessStatusCode)
                    {
                        var readTask = await respone.Content.ReadAsStringAsync();
                        connectionResult = JsonConvert.DeserializeObject<ConnectionString>(readTask);
                        return connectionResult.ConnectionStringDBResult;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception Ex)
            {
                return Ex.ToString();
            }
        }

        public string ExecuteNonQuery()
        {
            string i = null;
            try
            {
                command.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                i = e.Message;
            }
            return i;
        }

        public void ExecuteReader()
        {
            try
            {
                reader = command.ExecuteReader();
            }
            catch (SqlException e)
            {
                Message = e.Message;
            }
        }

        public void CloseReader()
        {
            reader.Close();
            reader.Dispose();
            reader = null;
        }

        public void CloseConnection()
        {
            c.Close();
            c.Dispose();
            c = null;
        }
    }
}
