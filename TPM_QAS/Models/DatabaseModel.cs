using Oracle.ManagedDataAccess.Client;
using System;
using Microsoft.Data.SqlClient;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TPM_QAS.Models;

namespace DatabaseModel
{
    public class Database
    {
        protected string sql { get; set; }
        public SqlCommand command;
        protected SqlConnection c;
        public SqlDataReader reader;
        protected SqlTransaction tran;
        protected SqlDataAdapter sqladp;
        protected string Message;
        protected OracleCommand cmd;
        protected OracleConnection conn;
        protected OracleDataAdapter dataAdapter;

        public void database()
        {
        }

        public void OpenConnection()
        {
            // Note: In .NET 8 Core, connection strings are managed via IConfiguration/appsettings.json
            // These legacy methods are kept for compatibility but should be updated to use DI
            command = new SqlCommand();
            c = new SqlConnection();
            command.Connection = c;
        }

        public void OpenPackingConnection()
        {
            command = new SqlCommand();
            c = new SqlConnection();
            command.Connection = c;
        }

        public void OpenAclConnection()
        {
            command = new SqlCommand();
            c = new SqlConnection();
            command.Connection = c;
        }

        public void OpenAclTPMConnection()
        {
            command = new SqlCommand();
            c = new SqlConnection();
            command.Connection = c;
        }

        public void OpenOracleQasConnection()
        {
            cmd = new OracleCommand();
            conn = new OracleConnection();
            cmd.Connection = conn;
        }

        public void OpenOracleICCPConnection()
        {
            cmd = new OracleCommand();
            conn = new OracleConnection();
            cmd.Connection = conn;
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

        public void CloseOracleConnection()
        {
            conn.Close();
            conn.Dispose();
            conn = null;
        }
    }
}
