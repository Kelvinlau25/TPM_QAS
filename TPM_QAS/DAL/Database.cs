using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Concurrent;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TPM_QAS.Helpers.DataModel;

namespace TPM_QAS.DAL
{
    public class Database
    {
        protected string sql { get; set; }
        protected SqlCommand command;
        protected SqlConnection c;
        public SqlDataReader reader;
        protected SqlTransaction tran;
        protected SqlDataAdapter sqladp;
        protected OracleCommand commandORA;
        protected OracleConnection cORA;
        public OracleDataReader readerORA;
        protected OracleTransaction tranORA;
        protected OracleDataAdapter sqladpORA;
        protected string Message;
        protected string ConnectionName;

        private static readonly ConcurrentDictionary<string, string> dic = new ConcurrentDictionary<string, string>();

        // Static configuration holder - set during startup
        private static IConfiguration _configuration;

        public static void SetConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected static string GetAppSetting(string key)
        {
            return _configuration?["AppSettings:" + key] ?? string.Empty;
        }

        // Public static accessor for use outside DAL classes
        public static string GetAppSettingStatic(string key)
        {
            return GetAppSetting(key);
        }

        public async Task<string> GetConnectionStringAsync(bool isACL = false, string dbName = null)
        {
            try
            {
                dbName = string.IsNullOrWhiteSpace(dbName) ? GetSystemName(isACL) : dbName;
                string connectionString = GetConnectionString(key: dbName);

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    DatabaseConnectionString dbConnectionString = new DatabaseConnectionString();
                    connectionString = await dbConnectionString.OpenAclConnection(dbName);
                    connectionString = EnsureTrustServerCertificate(connectionString);
                    AddConnectionString(key: dbName, value: connectionString);
                }

                return connectionString;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private string GetSystemName(bool isACL)
        {
            try
            {
                if (isACL)
                {
                    return GetAppSetting("ACL");
                }
                else
                {
                    string isTest = GetAppSetting("isTest");

                    if (string.Equals(isTest, "TRUE", StringComparison.OrdinalIgnoreCase))
                    {
                        return GetAppSetting("DEV");
                    }
                    else
                    {
                        return GetAppSetting("LIVE");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetConnectionString(string key)
        {
            try
            {
                if (dic.TryGetValue(key, out string value))
                {
                    return value;
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AddConnectionString(string key, string value)
        {
            try
            {
                dic.TryAdd(key, value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> GetConnectionStringORACOAAsync(bool isACL = false, string dbName = null)
        {
            try
            {
                dbName = string.IsNullOrWhiteSpace(dbName) ? GetSystemNameORACOA(isACL) : dbName;
                string connectionString = GetConnectionString(key: dbName);

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    DatabaseConnectionString dbConnectionString = new DatabaseConnectionString();
                    connectionString = await dbConnectionString.OpenAclConnection(dbName);
                    connectionString = EnsureTrustServerCertificate(connectionString);
                    AddConnectionString(key: dbName, value: connectionString);
                }

                return connectionString;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public async Task<string> GetConnectionStringERPCOAAsync(bool isACL = false, string dbName = null)
        {
            try
            {
                dbName = string.IsNullOrWhiteSpace(dbName) ? GetSystemNameERPCOA(isACL) : dbName;
                string connectionString = GetConnectionString(key: dbName);

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    DatabaseConnectionString dbConnectionString = new DatabaseConnectionString();
                    connectionString = await dbConnectionString.OpenAclConnection(dbName);
                    connectionString = EnsureTrustServerCertificate(connectionString);
                    AddConnectionString(key: dbName, value: connectionString);
                }

                return connectionString;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private string GetSystemNameORACOA(bool isACL)
        {
            try
            {
                if (isACL)
                {
                    return GetAppSetting("ACL");
                }
                else
                {
                    string isTest = GetAppSetting("isTest");

                    if (string.Equals(isTest, "TRUE", StringComparison.OrdinalIgnoreCase))
                    {
                        return GetAppSetting("DEV_ORA_COA");
                    }
                    else
                    {
                        return GetAppSetting("LIVE_ORA_COA");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GetSystemNameERPCOA(bool isACL)
        {
            try
            {
                if (isACL)
                {
                    return GetAppSetting("ACL");
                }
                else
                {
                    return GetAppSetting("LIVE_ERP_COA");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected static string EnsureTrustServerCertificate(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                return connectionString;

            if (connectionString.IndexOf("TrustServerCertificate", StringComparison.OrdinalIgnoreCase) < 0)
            {
                var trimmed = connectionString.TrimEnd(';');
                connectionString = trimmed + ";TrustServerCertificate=True";
            }

            return connectionString;
        }
    }
}
