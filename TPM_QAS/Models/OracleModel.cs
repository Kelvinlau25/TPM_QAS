using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

namespace OracleModel
{
    public class Oracle
    {
        protected string sql { get; set; }
        protected OracleCommand cmd;
        protected OracleConnection conn;
        protected OracleDataAdapter dataAdapter;
        protected string Message;

        public void Database()
        {
        }

        public void OpenConn(string connStr)
        {
            cmd = new OracleCommand();
            // In .NET 8, connection strings should come from IConfiguration
            conn = new OracleConnection();
            cmd.Connection = conn;
        }

        public DataSet ExecuteReaderDS(string query)
        {
            try
            {
                DataSet dataSet = new DataSet();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = query;

                using (OracleDataAdapter dataAdapter = new OracleDataAdapter())
                {
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataSet);
                }
                return dataSet;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return null;
            }
        }

        public DataTable ExecuteReaderDT(string query)
        {
            try
            {
                DataTable dataTable = new DataTable();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = query;

                using (OracleDataAdapter dataAdapter = new OracleDataAdapter())
                {
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                }
                return dataTable;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return null;
            }
        }

        public string ExecuteNonQuery()
        {
            string i = null;
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                i = ex.Message;
            }
            return i;
        }

        public void CloseConnection()
        {
            conn.Close();
            conn.Dispose();
            conn = null;
        }
    }
}
