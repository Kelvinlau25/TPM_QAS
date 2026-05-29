using ClosedXML.Excel;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using TPM_QAS.DAL;
using TPM_QAS.Enum;
using TPM_QAS.Interface;
using Database = TPM_QAS.DAL.Database;
using Path = System.IO.Path;

namespace TPM_QAS.Helpers
{
    public class CommonFunction : Controller, IExport, ISQL_DAL, IDAPPER_DAL, ISetting, IORA_DAL
    {
        Database databaseConnection = new Database();

        public CommonFunction(string uSERID = "")
        {
            USERID = uSERID;
            UploadPath = "uploads";
        }

        public string USERID { get; set; }
        public string UploadPath { get; set; }
        public string ConnectionString_SQL { get; set; }
        public string ConnectionString_ORA { get; set; }

        #region Upload
        public ActionResult Upload(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                try
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", UploadPath, fileName);

                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    return new JsonResult(new { success = true, message = "File uploaded successfully!" });
                }
                catch (Exception ex)
                {
                    return new JsonResult(new { success = false, message = "Error uploading file: " + ex.Message });
                }
            }
            else
            {
                return new JsonResult(new { success = false, message = "File is null or empty" });
            }
        }
        #endregion

        #region Export
        public virtual FileContentResult ExportToExcel(DataTable dt, string FileName)
        {
            try
            {
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt).Columns().AdjustToContents();
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                return null;
            }
        }

        public virtual string RenderViewToString(ControllerContext context, string viewPath, object model = null,
            bool partial = false)
        {
            // Note: View rendering to string in .NET Core requires a different approach.
            // This is a placeholder - use a Razor view rendering service if needed.
            return string.Empty;
        }
        #endregion

        #region PSP_COMMON_ORACLE
        public async Task<DataTable> PSP_COMMON_ORA(string Query)
        {
            DataTable dt = new DataTable();
            try
            {
                string constr = await databaseConnection.GetConnectionStringAsync(false);
                using (OracleConnection cORA = new OracleConnection(constr))
                {
                    using (OracleCommand cmd = new OracleCommand(Query, cORA))
                    {
                        await cORA.OpenAsync();
                        cmd.CommandTimeout = 0;
                        OracleDataReader readerORA = cmd.ExecuteReader();
                        dt.Load(readerORA);
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                return dt;
            }
        }

        public async Task<DataTable> PSP_COMMON_ORA(string Query, EnumType.ExecutionType ExecutionType)
        {
            DataTable dt = new DataTable();
            try
            {
                string constr = await databaseConnection.GetConnectionStringAsync(false);
                using (OracleConnection cORA = new OracleConnection(constr))
                {
                    using (OracleCommand cmd = new OracleCommand(Query, cORA))
                    {
                        await cORA.OpenAsync();
                        cmd.CommandTimeout = 0;
                        if (ExecutionType == EnumType.ExecutionType.ExecuteReader)
                        {
                            OracleDataReader readerORA = cmd.ExecuteReader();
                            dt.Load(readerORA);
                        }
                        else
                        {
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                return dt;
            }
        }

        public async Task<DataTable> PSP_COMMON_ORA(string Query, string ConnectionString)
        {
            DataTable dt = new DataTable();
            try
            {
                string constr = ConnectionString;
                using (OracleConnection cORA = new OracleConnection(constr))
                {
                    using (OracleCommand cmd = new OracleCommand(Query, cORA))
                    {
                        await cORA.OpenAsync();
                        cmd.CommandTimeout = 0;
                        OracleDataReader readerORA = cmd.ExecuteReader();
                        dt.Load(readerORA);
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                return dt;
            }
        }

        public async Task<DataTable> PSP_COMMON_ORA(string Query, CommandType type = CommandType.Text)
        {
            DataTable dt = new DataTable();
            try
            {
                string constr = await databaseConnection.GetConnectionStringAsync(false);
                using (OracleConnection cORA = new OracleConnection(constr))
                {
                    using (OracleCommand cmd = new OracleCommand(Query, cORA))
                    {
                        await cORA.OpenAsync();
                        cmd.CommandType = type;
                        cmd.CommandTimeout = 0;
                        OracleDataReader readerORA = cmd.ExecuteReader();
                        dt.Load(readerORA);
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                return dt;
            }
        }

        public async Task<DataTable> PSP_COMMON_ORA(string Query, CommandType type = CommandType.Text, List<OracleParameter> ListofParam = null)
        {
            DataTable dt = new DataTable();
            try
            {
                string constr = await databaseConnection.GetConnectionStringAsync(false);
                using (OracleConnection cORA = new OracleConnection(constr))
                {
                    using (OracleCommand cmd = new OracleCommand(Query, cORA))
                    {
                        await cORA.OpenAsync();
                        cmd.CommandType = type;
                        cmd.CommandTimeout = 0;

                        if (ListofParam != null && ListofParam.Count > 0)
                        {
                            foreach (OracleParameter item in ListofParam)
                            {
                                cmd.Parameters.Add(item);
                            }
                        }
                        OracleDataReader readerORA = cmd.ExecuteReader();
                        dt.Load(readerORA);
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                return dt;
            }
        }

        public async Task<DataTable> PSP_COMMON_ORA(string Query, CommandType type = CommandType.Text, List<OracleParameter> ListofParam = null, string ConnectionString = "")
        {
            DataTable dt = new DataTable();
            try
            {
                string constr = ConnectionString;
                using (OracleConnection cORA = new OracleConnection(constr))
                {
                    using (OracleCommand cmd = new OracleCommand(Query, cORA))
                    {
                        await cORA.OpenAsync();
                        cmd.CommandType = type;
                        cmd.CommandTimeout = 0;

                        if (ListofParam != null && ListofParam.Count > 0)
                        {
                            foreach (OracleParameter item in ListofParam)
                            {
                                cmd.Parameters.Add(item);
                            }
                        }
                        OracleDataReader readerORA = cmd.ExecuteReader();
                        dt.Load(readerORA);
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                return dt;
            }
        }
        #endregion

        #region PSP_COMMON_SQL
        public async Task<DataTable> PSP_COMMON_SQL(string Query)
        {
            DataTable dt = new DataTable();
            try
            {
                string constr = await databaseConnection.GetConnectionStringAsync(false);
                using (SqlConnection c = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(Query, c))
                    {
                        await c.OpenAsync();
                        cmd.CommandTimeout = 0;
                        SqlDataReader reader = cmd.ExecuteReader();
                        dt.Load(reader);
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                return dt;
            }
        }

        public async Task<DataTable> PSP_COMMON_SQL(string Query, EnumType.ExecutionType ExecutionType)
        {
            DataTable dt = new DataTable();
            try
            {
                string constr = await databaseConnection.GetConnectionStringAsync(false);
                using (SqlConnection c = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(Query, c))
                    {
                        await c.OpenAsync();
                        cmd.CommandTimeout = 0;
                        if (ExecutionType == EnumType.ExecutionType.ExecuteReader)
                        {
                            SqlDataReader reader = await cmd.ExecuteReaderAsync();
                            dt.Load(reader);
                        }
                        else
                        {
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                return dt;
            }
        }

        public async Task<DataTable> PSP_COMMON_SQL(string Query, string ConnectionString = "")
        {
            DataTable dt = new DataTable();
            try
            {
                string constr = ConnectionString;
                using (SqlConnection c = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(Query, c))
                    {
                        await c.OpenAsync();
                        cmd.CommandTimeout = 0;
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();
                        dt.Load(reader);
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                return dt;
            }
        }

        public async Task<DataTable> PSP_COMMON_SQL(string Query, CommandType type = CommandType.Text)
        {
            DataTable dt = new DataTable();
            try
            {
                string constr = await databaseConnection.GetConnectionStringAsync(false);
                using (SqlConnection c = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(Query, c))
                    {
                        await c.OpenAsync();
                        cmd.CommandType = type;
                        cmd.CommandTimeout = 0;
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();
                        dt.Load(reader);
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                return dt;
            }
        }

        public async Task<DataTable> PSP_COMMON_SQL(string Query, CommandType type = CommandType.Text, List<SqlParameter> ListofParam = null)
        {
            DataTable dt = new DataTable();
            try
            {
                string constr = await databaseConnection.GetConnectionStringAsync(false);
                using (SqlConnection c = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(Query, c))
                    {
                        await c.OpenAsync();
                        cmd.CommandType = type;
                        cmd.CommandTimeout = 0;

                        if (ListofParam != null && ListofParam.Count > 0)
                        {
                            foreach (SqlParameter param in ListofParam)
                            {
                                cmd.Parameters.Add(param);
                            }
                        }

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();
                        dt.Load(reader);
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                return dt;
            }
        }

        public async Task<DataTable> PSP_COMMON_SQL(string Query, EnumType.ExecutionType ExecutionType, CommandType type = CommandType.Text, List<SqlParameter> ListofParam = null)
        {
            DataTable dt = new DataTable();
            try
            {
                string constr = await databaseConnection.GetConnectionStringAsync(false);
                using (SqlConnection c = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(Query, c))
                    {
                        await c.OpenAsync();
                        cmd.CommandType = type;
                        cmd.CommandTimeout = 0;

                        if (ListofParam != null && ListofParam.Count > 0)
                        {
                            foreach (SqlParameter param in ListofParam)
                            {
                                cmd.Parameters.Add(param);
                            }
                        }
                        if (ExecutionType == EnumType.ExecutionType.ExecuteReader)
                        {
                            SqlDataReader reader = await cmd.ExecuteReaderAsync();
                            dt.Load(reader);
                        }
                        else
                        {
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                return dt;
            }
        }

        public async Task<DataTable> PSP_COMMON_SQL(string Query, CommandType type = CommandType.Text, List<SqlParameter> ListofParam = null, string ConnectionString = "")
        {
            DataTable dt = new DataTable();
            try
            {
                string constr = ConnectionString;
                using (SqlConnection c = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(Query, c))
                    {
                        await c.OpenAsync();
                        cmd.CommandType = type;
                        cmd.CommandTimeout = 0;

                        if (ListofParam != null && ListofParam.Count > 0)
                        {
                            foreach (SqlParameter param in ListofParam)
                            {
                                cmd.Parameters.Add(param);
                            }
                        }

                        SqlDataReader reader = cmd.ExecuteReader();
                        dt.Load(reader);
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                return dt;
            }
        }

        public async Task<DataTable> PSP_COMMON_SQL(string Query, EnumType.ExecutionType ExecutionType, CommandType type = CommandType.Text, List<SqlParameter> ListofParam = null, string ConnectionString = "")
        {
            DataTable dt = new DataTable();
            try
            {
                string constr = ConnectionString;
                using (SqlConnection c = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(Query, c))
                    {
                        await c.OpenAsync();
                        cmd.CommandType = type;
                        cmd.CommandTimeout = 0;

                        if (ListofParam != null && ListofParam.Count > 0)
                        {
                            foreach (SqlParameter param in ListofParam)
                            {
                                cmd.Parameters.Add(param);
                            }
                        }

                        if (ExecutionType == EnumType.ExecutionType.ExecuteReader)
                        {
                            SqlDataReader reader = await cmd.ExecuteReaderAsync();
                            dt.Load(reader);
                        }
                        else
                        {
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                return dt;
            }
        }

        public async Task<DataTable> PSP_COMMON_SQL(string Query, CommandType type = CommandType.Text, List<SqlParameter> ListofParam = null, string ConnectionString = "", string DbName = "")
        {
            DataTable dt = new DataTable();
            try
            {
                string constr = await databaseConnection.GetConnectionStringAsync(false, DbName);
                using (SqlConnection c = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(Query, c))
                    {
                        await c.OpenAsync();
                        cmd.CommandType = type;
                        cmd.CommandTimeout = 0;

                        if (ListofParam != null && ListofParam.Count > 0)
                        {
                            foreach (SqlParameter param in ListofParam)
                            {
                                cmd.Parameters.Add(param);
                            }
                        }

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();
                        dt.Load(reader);
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                return dt;
            }
        }
        #endregion

        #region PSP_COMMON_DAPPER_SQL
        public async Task<List<T>> PSP_COMMON_DAPPER<T>(string Query)
        {
            List<T> EmpList = null;
            try
            {
                string constr = await databaseConnection.GetConnectionStringAsync(false);
                using (SqlConnection c = new SqlConnection(constr))
                {
                    EmpList = (await c.QueryAsync<T>(Query, null, commandType: CommandType.Text).ConfigureAwait(false)).AsList();
                }
                return EmpList;
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                return EmpList;
            }
        }

        public async Task<List<T>> PSP_COMMON_DAPPER<T>(string Query, string ConnectionString)
        {
            List<T> EmpList = null;
            try
            {
                string constr = ConnectionString;
                using (SqlConnection c = new SqlConnection(constr))
                {
                    EmpList = (await c.QueryAsync<T>(Query, null, commandType: CommandType.Text).ConfigureAwait(false)).AsList();
                }
                return EmpList;
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                return EmpList;
            }
        }

        public async Task<List<T>> PSP_COMMON_DAPPER<T>(string Query, CommandType Commandtype)
        {
            List<T> EmpList = null;
            try
            {
                string constr = await databaseConnection.GetConnectionStringAsync(false);
                using (SqlConnection c = new SqlConnection(constr))
                {
                    EmpList = (await c.QueryAsync<T>(Query, null, commandType: Commandtype).ConfigureAwait(false)).AsList();
                }
                return EmpList;
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                return EmpList;
            }
        }

        public async Task<List<T>> PSP_COMMON_DAPPER<T>(string Query, CommandType Commandtype, object ListofParam)
        {
            List<T> ObjModel = null;
            try
            {
                string constr = await databaseConnection.GetConnectionStringAsync(false);
                using (SqlConnection c = new SqlConnection(constr))
                {
                    ObjModel = (await c.QueryAsync<T>(Query, ListofParam, commandType: Commandtype).ConfigureAwait(false)).AsList();
                }
                return ObjModel;
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                return ObjModel;
            }
        }
        #endregion

        #region PSP_XML_VALUE_CONVERT_TO_DATATABLE
        public async Task<DataTable> PSP_LOAD_COMMON_SETTING(string Val, string Query)
        {
            List<SqlParameter> ListofParamSQL = new List<SqlParameter>();
            ListofParamSQL = new List<SqlParameter>();
            ListofParamSQL.Add(new SqlParameter("@INDICATOR", Val));
            return await this.PSP_COMMON_SQL(Query, CommandType.StoredProcedure, ListofParamSQL, USERID);
        }
        #endregion

        public string getContainerName()
        {
            string containerName = "";
            try
            {
                bool isTest = Database.GetAppSettingStatic("isTest").ToLower() == "true";
                if (isTest)
                {
                    containerName = "tpmqas-test";
                }
                else
                {
                    containerName = "tpmqas-live";
                }

                return containerName.ToLower();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
