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
using Oracle.ManagedDataAccess.Client;
using TPM_QAS.Models;


namespace DatabaseModel
{
    public class Database
    {
        protected string sql { get; set; }
        public SqlCommand command;
        //protected SqlCommand command;
        protected SqlConnection c;
        public SqlDataReader reader;
        protected SqlTransaction tran;
        protected SqlDataAdapter sqladp;
        protected string Message;
        protected OracleCommand cmd;
        protected OracleConnection conn;
        protected OracleDataAdapter dataAdapter;

        // constructor
        public void database()
        {
        }
        // method
        public void OpenConnection()
        {
            command = new SqlCommand();
            c = new SqlConnection(ConfigurationManager.ConnectionStrings["SQL_QAS"].ConnectionString);
            command.Connection = c;
            c.Open();
        }

        public void OpenPackingConnection()
        {
            command = new SqlCommand();
            c = new SqlConnection(ConfigurationManager.ConnectionStrings["SQL_TPM"].ConnectionString);
            command.Connection = c;
            c.Open();
        }

        public void OpenAclConnection()
        {
            command = new SqlCommand();
            c = new SqlConnection(ConfigurationManager.ConnectionStrings["DBAccess"].ConnectionString);
            command.Connection = c;
            c.Open();
        }

        public void OpenAclTPMConnection()
        {
            command = new SqlCommand();
            c = new SqlConnection(ConfigurationManager.ConnectionStrings["DbTPMAcl"].ConnectionString);
            command.Connection = c;
            c.Open();
        }

        public void OpenOracleQasConnection()
        {
            cmd = new OracleCommand();
            conn = new OracleConnection(ConfigurationManager.ConnectionStrings["ORCL_QAS"].ToString());
            cmd.Connection = conn;
            conn.Open();
        }

        public void OpenOracleICCPConnection()
        {
            cmd = new OracleCommand();
            conn = new OracleConnection(ConfigurationManager.ConnectionStrings["ORA_ICCP"].ToString());
            cmd.Connection = conn;
            conn.Open();
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