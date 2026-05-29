using ClosedXML.Excel;
using Dapper;
using DatabaseModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TPM_QAS.DAL;
using TPM_QAS.Enum;
using TPM_QAS.Interface;
using Database = TPM_QAS.DAL.Database;
using Path = System.IO.Path;

namespace TPM_QAS.Helpers
{
    /// <summary>
    /// Azham 17/1/2023
    /// PLEASE READ THIS BEFORE USED
    /// CommonFunction
    /// Common function for minimize code and faster the development
    /// Require some external plugin and setting
    /// Please select the connection string GetConnectionString("DEV") , GetConnectionString("ORCL_M2_INSP")
    /// Plugin Requirement :
    /// 1. Oracle.ManagedDataAcess Version = 19.6.0 (Maximum)
    /// 2. Dapper Version = 2.0.123
    /// 3. Closed XML Version = 0.96.0
    /// 4. itext7 Version = 7.1.11
    /// 5. itext7.pdfhtml Version = 4.0.5
    /// Must include class :
    /// Folder : Interface
    /// 1. IExport
    /// 2. ISQL_DAL
    /// 3. IDAPPER_DAL
    /// 4. ISetting
    /// 5. IORA_DAL
    /// Folder : Enum
    /// 1. EnumType
    /// TO DO :
    /// 1. Please change namescape name M2_2ndGrade.Helpers to <ProjectName>.Helpers
    /// 2. Copy this file to folder Helpers
    /// HOW TO USE :
    /// 
    /// </summary>
    public class CommonFunction : Controller, IExport, ISQL_DAL, IDAPPER_DAL, ISetting, IORA_DAL
    {

        Database databaseConnection = new Database();

        public CommonFunction(string uSERID = "")
        {
            USERID = uSERID;
            UploadPath = "~/App_Data/uploads";
        }

        public string USERID { get; set; }
        public string UploadPath { get; set; }
        public string ConnectionString_SQL
        { get; set; }
        public string ConnectionString_ORA { get; set; }

        #region Upload
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    // get file name
                    string fileName = Path.GetFileName(file.FileName);
                    // set file path
                    string path = Path.Combine(Server.MapPath(UploadPath), fileName);
                    // save the file
                    file.SaveAs(path);
                    // return success message
                    return Json(new { success = true, message = "File uploaded successfully!" });
                }
                catch (Exception ex)
                {
                    // return error message
                    return Json(new { success = false, message = "Error uploading file: " + ex.Message });
                }
            }
            else
            {
                // return error message if file is null or empty
                return Json(new { success = false, message = "File is null or empty" });
            }
        }
        #endregion
        #region Export
        /// <summary>
        /// Azham 17/1/2023
        /// ExportToExcel
        /// Convert datatable to excel file
        /// Require external plugin Closed XML
        /// Closed XML recommended version : v0.96.0
        /// </summary>
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
                throw;
            }
        }
        /// <summary>
        /// Azham 17/1/2023
        /// ExportToPDF
        /// Export html view to PDF file
        /// Require external plugin itext7 and itext7.pdfhtml
        /// itext7 recommended version : 7.1.11
        /// itext7.pdfhtml recommended version : 4.0.5
        /// Parameter ViewName : name or your view example index
        /// Parameter ControllerContext : controller context
        /// Parameter modelMaster : modal object
        /// Parameter pagesize : type of rotation page
        /// Parameter PDF_TYPE : indicator PDF in potrait or lanscape
        /// </summary>
        //public virtual byte[] ExportToPDF(string ViewName, ControllerContext ControllerContext, object modelMaster, PageSize pagesize, EnumType.PDF_TYPE PDF_TYPE)
        //{
        //    try
        //    {
        //        if (PDF_TYPE == EnumType.PDF_TYPE.PORTRAIT)
        //        {
        //            using (var workStream = new MemoryStream())
        //            using (var pdfWriter = new PdfWriter(workStream))
        //            {
        //                PdfDocument pdfDocument = new PdfDocument(pdfWriter);
        //                pdfDocument.SetDefaultPageSize(pagesize.Rotate());

        //                string PacklistSEHtmlString = RenderViewToString(ControllerContext, ViewName, modelMaster, false);
        //                PacklistSEHtmlString = Regex.Replace(PacklistSEHtmlString, System.Environment.NewLine, "");

        //                var document = HtmlConverter.ConvertToDocument(PacklistSEHtmlString, pdfWriter, new ConverterProperties());
        //                document.Close();

        //                return workStream.ToArray();
        //            }
        //        }
        //        else
        //        {
        //            using (var workStream = new MemoryStream())
        //            using (var pdfWriter = new PdfWriter(workStream))
        //            {
        //                PdfDocument pdfDocument = new PdfDocument(pdfWriter);
        //                pdfDocument.SetDefaultPageSize(pagesize.Rotate());
        //                PdfMerger merger = new PdfMerger(pdfDocument);

        //                string PacklistSEHtmlString = RenderViewToString(ControllerContext, ViewName, modelMaster, false);
        //                PacklistSEHtmlString = Regex.Replace(PacklistSEHtmlString, System.Environment.NewLine, "");

        //                MemoryStream baos = new MemoryStream();
        //                PdfDocument temp = new PdfDocument(new PdfWriter(baos));
        //                temp.SetDefaultPageSize(pagesize.Rotate());
        //                var document = HtmlConverter.ConvertToDocument(PacklistSEHtmlString, temp, new ConverterProperties());
        //                document.Close();


        //                ReaderProperties rp = new ReaderProperties();
        //                baos = new MemoryStream(baos.ToArray());
        //                temp = new PdfDocument(new PdfReader(baos, rp));
        //                merger.Merge(temp, 1, temp.GetNumberOfPages());
        //                temp.Close();
        //                merger.Close();

        //                return workStream.ToArray();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogSys err = new ErrorLogSys();
        //        err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
        //        err = null;
        //        return null;
        //        throw;
        //    }
        //}
        /// <summary>
        /// Azham 01/2/2022
        /// RenderViewToString
        /// Convert HTML View to string for export
        /// </summary>
        public virtual string RenderViewToString(ControllerContext context, string viewPath, object model = null,
            bool partial = false)
        {
            try
            {
                // first find the ViewEngine for this view
                ViewEngineResult viewEngineResult = null;
                if (partial)
                    viewEngineResult = ViewEngines.Engines.FindPartialView(context, viewPath);
                else
                    viewEngineResult = ViewEngines.Engines.FindView(context, viewPath, null);

                if (viewEngineResult == null)
                    throw new FileNotFoundException("View cannot be found.");

                // get the view and attach the model to view data
                var view = viewEngineResult.View;
                context.Controller.ViewData.Model = model;

                string result = null;

                using (var sw = new StringWriter())
                {
                    var ctx = new ViewContext(context, view, context.Controller.ViewData, context.Controller.TempData, sw);
                    view.Render(ctx, sw);
                    result = sw.ToString();
                }

                return result;
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                return null;
                throw;
            }

        }
        #endregion

        #region PSP_COMMON_ORACLE
        /// <summary>
        /// PSP_COMMON_ORACLE
        /// Azham 17/1/2023
        /// ORM get data from oracle database and pass back the result as database
        /// </summary>
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
            }
        }
        #endregion
        #region PSP_COMMON_SQL
        /// <summary>
        /// PSP_COMMON_SQL
        /// Azham 17/1/2023
        /// ORM get data from MSSQL database and pass back the result as database
        /// </summary>

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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
            }
        }
        #endregion
        #region PSP_COMMON_DAPPER_SQL
        /// <summary>
        /// Azham 17/1/2023
        /// PSP_COMMON_DAPPER
        /// ORM  get data from database and convert datatable to Model Object 
        /// Require external plugin Dapper   - Download from Microsoft Nuget
        /// Dapper recommended version : v2.0.123     
        /// </summary>
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
                throw;
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
                throw;
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
                throw;
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
                throw;
            }
        }
        #endregion
        #region PSP_XML_VALUE_CONVERT_TO_DATATABLE
        /// <summary>
        /// Azham 17/1/2023
        /// Get Master Setting Value   - Work will XML table
        /// </summary>
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
                bool isTest = System.Configuration.ConfigurationManager.AppSettings["isTest"].ToLower() == "true";
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