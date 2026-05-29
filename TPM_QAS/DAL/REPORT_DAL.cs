using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using TPM_QAS.Helpers;
using TPM_QAS.Models;

namespace TPM_QAS.DAL
{
    public class REPORT_DAL : Database
    {

        public async Task<List<SelectListItem>> GetProdTypeList() //Simulation By 
        {
            ACL_UserObj userobj = HttpContextHelper.Current.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            List<SelectListItem> items = new List<SelectListItem>();
            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "select distinct PRODTYPE from IDS_H order by prodtype";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        await con.OpenAsync();
                       
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                items.Add(new SelectListItem
                                {
                                    Text = reader["PRODTYPE"].ToString(),
                                    Value = reader["PRODTYPE"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                return null;
            }

            return items;
        }

        public async Task<SummaryVM> GetSummaryData(string id)
        {
            ACL_UserObj userobj = HttpContextHelper.Current.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            SummaryVM data = new SummaryVM();

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    
                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_SUM_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@P_id", id)).Direction = ParameterDirection.Input;

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                data = new SummaryVM
                                {
                                    PRODTYPE = reader["PRODTYPE"].ToString() ?? "",
                                    LOTNO = reader["LOTNO"].ToString() ?? "",
                                    FullNG = reader["FullNG"].ToString() ?? "",
                                    PACKEDDATE = reader["PACKEDDATE"].ToString() ?? "",
                                    PACKEDDATE2 = reader["PACKEDDATE2"].ToString() ?? "",
                                    QUANTITY = reader["QUANTITY"].ToString() ?? "",
                                    TESTEDBY = reader["TESTEDBY"].ToString() ?? "",
                                    UPDATEDBY = reader["UPDATED_BY"].ToString() ?? "",
                                    STATUS = reader["STATUS"].ToString() ?? "",
                                    GRADE = reader["GRADE"].ToString() ?? "",
                                    BS_MACH = reader["BS_MACH"].ToString() ?? "",
                                    COQ_ADJ_RMK = reader["COQ_ADJ_RMK"].ToString() ?? "",
                                    BBS = reader["BBS"].ToString() ?? "",
                                    BYI = reader["BYI"].ToString() ?? "",
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                return null;
            }

            return data;
        }

        public async Task<List<MoldAndPropVM>> GetMoldingPropertyData(string prodtype, string lotno, string type)
        {
            ACL_UserObj userobj = HttpContextHelper.Current.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            List<MoldAndPropVM> model = new List<MoldAndPropVM>();

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_MOLD_PROP_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@P_prodtype", prodtype)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@P_lotno", lotno)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@P_type", type)).Direction = ParameterDirection.Input;

                        if (type == "99")
                        {
                            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    model.Add(new MoldAndPropVM()
                                    {
                                        MAIN_PROPERTIES = reader["A_PROPERTIES"]?.ToString() ?? "",
                                        PROPITEM = reader["A_PROP_ITEM"]?.ToString() ?? "",
                                        MACHINENAME = reader["MACHINENAME"]?.ToString() ?? "",
                                        UNIT = reader["UNIT"]?.ToString() ?? "",
                                        AVERAGE = reader["AVERAGE"]?.ToString() ?? "",
                                        REGRESSIONRESULT = reader["REGRESSIONRESULT"]?.ToString() ?? "",
                                        SPECIFICATION = reader["SPECIFICATION"]?.ToString() ?? "",
                                        GRADE = reader["GRADE"]?.ToString() ?? "",
                                        COQADJ = reader["COQADJ"]?.ToString() ?? "",
                                        //STATUSIND = reader["STATUSIND"]?.ToString() ?? "",
                                        SEQUENCE = reader["SEQUENCE"]?.ToString() ?? ""
                                    });
                                }
                            }
                        }
                        else
                        {
                            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    model.Add(new MoldAndPropVM()
                                    {
                                        MAIN_PROPERTIES = reader["MAIN_PROPERTIES"]?.ToString() ?? "",
                                        PROPITEM = reader["PROPITEM"]?.ToString() ?? "",
                                        MACHINENAME = reader["MACHINENAME"]?.ToString() ?? "",
                                        UNIT = reader["UNIT"]?.ToString() ?? "",
                                        AVERAGE = reader["AVERAGE"]?.ToString() ?? "",
                                        REGRESSIONRESULT = reader["REGRESSIONRESULT"]?.ToString() ?? "",
                                        SPECIFICATION = reader["SPECIFICATION"]?.ToString() ?? "",
                                        GRADE = reader["GRADE"]?.ToString() ?? "",
                                        COQADJ = reader["COQADJ"]?.ToString() ?? "",
                                        STATUSIND = reader["STATUSIND"]?.ToString() ?? ""
                                    });
                                }
                            }
                        }
                        
                        
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
            }

            return model;

        }

        // Get ID for history transaction table
        public async Task<string> GetID_IDS_D(string prodtype, string lotno)
        {
            string result = "0";

            ACL_UserObj userobj = HttpContextHelper.Current.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();
            string createdby = USERID;
            string loc = HttpContextHelper.Current.Connection.RemoteIpAddress?.ToString().ToString();
            string pc = Environment.MachineName;

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_ID_IDS_D_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@prodtype", prodtype)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@lotno", lotno)).Direction = ParameterDirection.Input;

                        string id_ids_d = "";
                        //while (reader.Read())
                        //{
                        //    id_ids_d = reader["ID_IDS_D"].ToString();
                        //}

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                id_ids_d = reader["ID_IDS_D"].ToString();
                            }
                        }

                        return id_ids_d;

                    }
                }

            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;

                result = ex.Message;
                return null;
            }
        }

        public async Task<DataTable> getReportData(string id, string prodtype, string lotno, string type)
        {
            ACL_UserObj userobj = HttpContextHelper.Current.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_REPORT_DATA", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@P_id", id)).Direction = ParameterDirection.Input; 
                        cmd.Parameters.Add(new SqlParameter("@P_prodtype", prodtype)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@P_lotno", lotno)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@P_type", type)).Direction = ParameterDirection.Input;
                        reader = await cmd.ExecuteReaderAsync();
                        dt.Load(reader);
                        reader.Close();

                    }
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                return null;
            }
        }

        public async Task<DataTable> getFirstBag(string id)
        {
            ACL_UserObj userobj = HttpContextHelper.Current.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_RPT_FIRSTBAG_YI_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@PIDH", id)).Direction = ParameterDirection.Input;
                        reader = await cmd.ExecuteReaderAsync();
                        dt.Load(reader);
                        reader.Close();

                    }
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                return null;
            }
        }

        // History Transaction Table        
        public async Task<DataTable> GetHistoryTransactionData(string idListAsStr, string prodtype, string lotno)
        {
            ACL_UserObj userobj = HttpContextHelper.Current.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_SL_EDIT_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", idListAsStr)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pProdtype", prodtype)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pLotno", lotno)).Direction = ParameterDirection.Input;
                        reader = await cmd.ExecuteReaderAsync();
                        dt.Load(reader);
                        reader.Close();

                    }
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                return null;
            }
        }

        // Appearance        
        public async Task<DataTable> GetAppearanceTable(string prodtype, string lotno, int type)
        {
            ACL_UserObj userobj = HttpContextHelper.Current.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_APP_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@prodtype", prodtype)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@lotno", lotno)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@type", type)).Direction = ParameterDirection.Input;
                        reader = await cmd.ExecuteReaderAsync();
                        dt.Load(reader);
                        reader.Close();

                    }
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                return null;
            }
        }

        public async Task<List<ReportNGVM>> getNGReport(ReportNGVM m)
        {
            ACL_UserObj userobj = HttpContextHelper.Current.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            List<ReportNGVM> model = new List<ReportNGVM>();

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("PSP_RPT_MONTHLYNG", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@dateFrom", m.datefrom)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@dateTo", m.dateto)).Direction = ParameterDirection.Input;

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                model.Add(new ReportNGVM
                                {
                                    abnormality = reader["abnormality"].ToString(),
                                    type = reader["prodtype"].ToString(),
                                    lotno = reader["lotno"].ToString(),
                                    packkeddate = reader["packeddate"].ToString(),
                                    quantity = Convert.ToInt32(reader["quantity"]),
                                    ng_qty = Convert.ToInt32(reader["ng_quantity"]),
                                    prodline = reader["prodline"].ToString(),
                                    rowspan = Convert.ToInt32(reader["ROWSPAN"]),
                                    rownumber = Convert.ToInt32(reader["RowNumber"]),


                                });
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                return null;
            }
            return model;
        }

        public async Task<List<ReportSummary>> getSummaryNG(ReportSummary m)
        {
            ACL_UserObj userobj = HttpContextHelper.Current.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            List<ReportSummary> model = new List<ReportSummary>();

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("PSP_RPT_MONTHLYNG_SUM", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@dateFrom", m.datefrom)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@dateTo", m.dateto)).Direction = ParameterDirection.Input;

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                model.Add(new ReportSummary
                                {
                                    process = reader["process"].ToString(),
                                    inspect_lot = Convert.ToInt32(reader["inspected_lot"].ToString()),
                                    inspect_qty = Convert.ToInt32(reader["inspected_qty"].ToString()),
                                    ng_lot = Convert.ToInt32(reader["ng_lot"].ToString()),
                                    ng_qty = Convert.ToInt32(reader["ng_qty"])

                                });
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                return null;
            }
            return model;
        }

        public async Task<DataTable> getLHSpec(string prodtype, string listproperties, string prodlinefrom, string prodlineto)
        {
            ACL_UserObj userobj = HttpContextHelper.Current.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_YILHSPEC_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pProdType", prodtype)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pProperties", listproperties)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pProdlineFR", prodlinefrom)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pProdlineTO", prodlineto)).Direction = ParameterDirection.Input;
                        reader = await cmd.ExecuteReaderAsync();
                        dt.Load(reader);
                        reader.Close();

                    }
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                return null;
            }
        }

        public async Task<DataTable> getDllData(int ID, string act, string category)
        {
            ACL_UserObj userobj = HttpContextHelper.Current.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_GET_DLL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", ID)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pACTION", act)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCATEGORY", category)).Direction = ParameterDirection.Input;
                        reader = await cmd.ExecuteReaderAsync();
                        dt.Load(reader);
                        reader.Close();

                    }
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                return null;
            }
        }

        #region COA GRN

        public async Task<DataTable> getdtcoa(string graded)
        {
            ACL_UserObj userobj = HttpContextHelper.Current.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_GET_COA_GRADED", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear(); 
                        cmd.Parameters.Add(new SqlParameter("@pGraded", graded)).Direction = ParameterDirection.Input;

                        reader = await cmd.ExecuteReaderAsync();
                        dt.Load(reader);
                        reader.Close();

                    }
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                return null;
            }
        }
        public async Task<DataTable> getdtcoaExcel(string graded)
        {
            ACL_UserObj userobj = HttpContextHelper.Current.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_GET_COA_GRADED_EXCEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter("@pGraded", graded)).Direction = ParameterDirection.Input;

                        reader = await cmd.ExecuteReaderAsync();
                        dt.Load(reader);
                        reader.Close();

                    }
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                return null;
            }
        }
        #endregion


        #region W GRADE PL

        public async Task<DataTable> getdtwgradepl()
        {
            ACL_UserObj userobj = HttpContextHelper.Current.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            string year = DateTime.Now.Year.ToString();
            string month = DateTime.Now.Month.ToString();

            string Grade_FROM = "W";
            string Grade_TO = "W";
            string warehouse_FROM = "-";
            string warehouse_TO = "-";

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_TPM_MONTHLY_W_GRADE_LISTING", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter("@pUSER_ID", USERID)).Direction = ParameterDirection.Input;

                        reader = await cmd.ExecuteReaderAsync();
                        dt.Load(reader);
                        reader.Close();
                    }
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                return null;
            }
        }
        public async Task<DataTable> getdtwgradepl_Excel()
        {
            ACL_UserObj userobj = HttpContextHelper.Current.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            string year = DateTime.Now.Year.ToString();
            string month = DateTime.Now.Month.ToString();

            string Grade_FROM = "W";
            string Grade_TO = "W";
            string warehouse_FROM = "-";
            string warehouse_TO = "-";

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_TPM_MONTHLY_W_GRADE_LISTING_EXCEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter("@pUSER_ID", USERID)).Direction = ParameterDirection.Input;

                        reader = await cmd.ExecuteReaderAsync();
                        dt.Load(reader);
                        reader.Close();
                    }
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                return null;
            }
        }
        #endregion

    }
}