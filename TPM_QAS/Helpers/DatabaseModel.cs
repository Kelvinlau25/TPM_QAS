using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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

        // constructor
        // method

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

                        //command = new SqlCommand();
                        //c = new SqlConnection(connectionResult.ConnectionStringDBResult);
                        //command.Connection = c;
                        //c.Open();

                        return connectionResult.ConnectionStringDBResult;

                    }
                    else //web api sent error response 
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