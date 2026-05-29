using DocumentFormat.OpenXml.Office2010.Excel;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Data;
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
    public class DATA_INT_DAL : Database
    {

        #region RAW DATA

        public async Task<DataTable> getProdLine()
        {

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_INT_RAW_DATA_PRODLINE", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

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
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, "INTEGRATION");
                err = null;
                return null;
            }

        }

        public async Task<DataTable> getMonthlyGradeData(string prodline)
        {

            try
            {
                string constr = await GetConnectionStringAsync(false, "LIVE_SQL_TPM_PACKING");
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_TPM_MONTHLY_W_GRADE_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@PProdLine", prodline)).Direction = ParameterDirection.Input;
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
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, "INTEGRATION");
                err = null;
                return null;
            }

        }

        public async Task<string>  setIntRawData_H(string type)
        {
            string result = "0";
            string userID = "INTEGRATION";
            string createdby = userID;
            string loc = HttpContextHelper.Current.Connection.RemoteIpAddress?.ToString().ToString();
            string pc = Environment.MachineName;

            try
            {
                
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_INT_SET_H", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@PIntType", type)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCreatedBy", createdby)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCreatedLoc", loc)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ReturnID", SqlDbType.Int)).Direction = ParameterDirection.Output;
                        cmd.ExecuteReader();


                        result = Convert.ToString(cmd.Parameters["@ReturnID"].Value);

                    }
                }
                
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, userID);
                err = null;

                result = ex.Message;
            }

            return result;
        }

        public async Task<string>  setIntRawDataMaint(int idh, GradeDataModel model, string prodLine)
        {
            string result = "0";
            string userID = "INTEGRATION";
            string createdby = userID;
            string loc = HttpContextHelper.Current.Connection.RemoteIpAddress?.ToString().ToString();
            string pc = Environment.MachineName;

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_INT_RAW_DATA_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@pIDH", idh)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ITEM_NAME", model.PROD_CODE)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ITEM_DECSRIPTION", model.PROD_DESC)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@LOT_GRADE", model.GRADE)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@LOT_NUMBER", model.LOT_NO)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PACKED_DATE", model.PACK_DATE)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PACK_QTY", model.PACK_QTY)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@TAGNO", model.TAG_NO)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PROD_LINE", prodLine)).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new SqlParameter("@ReturnID", SqlDbType.Int)).Direction = ParameterDirection.Output;
                        cmd.ExecuteReader();


                        result = Convert.ToString(cmd.Parameters["@ReturnID"].Value);

                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, userID);
                err = null;

                result = ex.Message;
            }

            return result;
        }

        #endregion

        #region Machine Data

        /// <summary>
        /// Get Machine Path Data
        /// </summary>
        public async Task<DataTable> GetMachinePathData()
        {

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_MACHINE_PATH_INT_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

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
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, "INTEGRATION");
                err = null;
                return null;
            }
        }

        public async Task<string>  IntegrateMachineData(int idh, string colData, string rowData, string machineName, string properties, string propItem, string loc)
        {
            string result = "0";
            string userID = "INTEGRATION";
            string createdby = userID;
            //string loc = HttpContextHelper.Current.Connection.RemoteIpAddress?.ToString().ToString();
            string pc = Environment.MachineName;

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_MACHINE_TRANS_INT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@pIDH", idh)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Pmachinename", machineName)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Pproperties", properties)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Ppropitem", propItem)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@coldata", colData)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@rowData", rowData)).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new SqlParameter("@pRecType", 1)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCreatedBy", "ADMIN")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCreatedLoc", loc)).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new SqlParameter("@ReturnID", SqlDbType.Int)).Direction = ParameterDirection.Output;
                        cmd.ExecuteReader();


                        result = Convert.ToString(cmd.Parameters["@ReturnID"].Value);

                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, userID);
                err = null;

                result = ex.Message;
            }

            return result;
        }

        #endregion

        #region Final Data
        public async Task<DataTable> GetIDSData()
        {

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_INT_IDS_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

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
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, "INTEGRATION");
                err = null;
                return null;
            }
        }

        public async Task<DataTable> GetListLotno()
        {
            try
            {
                string constr = await GetConnectionStringAsync(false, "LIVE_ORA_QAS");
                DataTable dt = new DataTable();

                using (OracleConnection myConnection = new OracleConnection(constr))
                {
                    using (OracleCommand cmd = new OracleCommand("SP_TPM_INSPECTION_LST_LOTNO", myConnection))
                    {
                        myConnection.Open();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new OracleParameter("SREFData", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;
                        OracleDataReader rdr = cmd.ExecuteReader();

                        dt.Load(rdr);
                    }

                }

                return dt;
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, "FINALINTEGRATION");
                err = null;
                return null;
            }

        }

        public async Task<int> SaveHeaderData(DataRow row)
        {
            try
            {
                string constr = await GetConnectionStringAsync(false, "LIVE_ORA_QAS");

                using (OracleConnection con = new OracleConnection(constr))
                {
                    using (OracleCommand cmd = new OracleCommand("SP_TPM_INSPECTION_PLAN_H_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new OracleParameter("PORGANIZATION_CODE", row["Org_code"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PPLAN_NAME", row["Plan_name"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PPLAN_DESCRIPTION", row["Plan_Desc"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PPLAN_TYPE", row["Plan_Type"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PLOT_SPLIT_REQUIRED", row["lot"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("PITEM_NUMBER", row["PRODTYPE"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PTPM_LOT_NUMBER", row["LOTNO"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PQUANTITY", row["qty"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PGRADE", row["grade"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("PMELT_FLOW_RATE", row["MFR"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PCHARPY_IMPACT_STRENGTH", row["Charpy"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PDEFLECTION_TEMP_LOAD", row["HDT"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PTENSILE_STRENGTH", row["Tensile_Strength"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PTENSILE_ELONGATION", row["Tensile_Elongation"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("PTENSILE_MODULUS", row["Tensile_Modulus"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PFLEXURAL_STRENGTH", row["Flexural_Strength"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PFLEXURAL_MODULUS", row["Flexural_Modulus"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PHAZE", row["Haze"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PGLOSS", row["Gloss"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("PFISHEYE_FILM", row["Fisheye_Film"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PFISHEYE_PLATE", row["Fisheye_Plate"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PPELLET_WEIGHT", row["Pellet_Weight"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PDENSITY", row["Density"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PPOWDER_CONTENT", row["Powder_Content"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("PGLASS_CONTENT", row["Glass_Content"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PBROMINE_CONTENT", row["Bromine_Content"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PMAISTURE_CONTENT", row["Moisture_Content"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PMACARONI", row["Macaroni"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PTWIN_PELLET", row["Twin_Pellet"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("PLONG_PELLET", row["Long_Pellet"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PCOLOR_L", row["ColorL"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PCOLOR_A", row["ColorA"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PCOLOR_B", row["ColorB"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PFLAMMABILITY", row["Flammability"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PPLATE_YI", row["PLATEYI"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("PTAG_NO_FROM", row["TAGNOFROM"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PTAG_NO_TO", row["TAGNOTO"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PYI", row["Yellowness_Index"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PBS1", row["BS_Spot1"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PBS2", row["BS_Spot2"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PBS3", row["BS_Spot3"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("PLAST_UPDATE_DATE", row["UPDATED_DATE"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PLAST_UPDATED_BY", row["UPDATED_BY"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PCREATION_DATE", row["CREATED_DATE"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PCREATED_BY", row["CREATED_BY"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.Int64, 20)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new OracleParameter("MSG", OracleDbType.Varchar2, 1000)).Direction = ParameterDirection.Output;

                        cmd.ExecuteReader();


                        int result = Convert.ToInt32(cmd.Parameters["RETURN_VALUE"].Value.ToString());
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, "INTEGRATION");
                err = null;

                return -1;
            }
        }

         public async Task IntegrateFinalData(int ID_IDS_H)
        {
            string userID = "INTEGRATION";
            string createdby = userID;
            string loc = HttpContextHelper.Current.Connection.RemoteIpAddress?.ToString().ToString();
            string pc = Environment.MachineName;

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_H_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID_IDS_H", ID_IDS_H)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PFullNg", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PPRODTYPE", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PPACKEDDATE", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PLOTNO", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PQUANTITY", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PTESTEDBY", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PUPDATEDBY", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PSTATUS", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PGRADE", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PPRODTYPCHGIND", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PAFTERPRODTYPCHG", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PPACKEDDATE2", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PFLOT", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PMABS_P", "")).Direction = ParameterDirection.Input;


                        cmd.Parameters.Add(new SqlParameter("@pRecType", 2)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCreatedBy", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCreatedLoc", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ReturnID", SqlDbType.Int)).Direction = ParameterDirection.Output;
                        //cmd.ExecuteReader();

                        int result = await cmd.ExecuteNonQueryAsync();
                        Message = result > 0 ? "Success" : "Failure";

                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, userID);
                err = null;
            }
        }

        #region YI Data
        public async Task<DataTable> GetYIData(int ID_IDS_H, string grade, string lot)
        {

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_INT_IDS_YIBS_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", ID_IDS_H)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@prop", "YI")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@lead", 1)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@grade", grade)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@lot", lot)).Direction = ParameterDirection.Input;
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
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, "INTEGRATION");
                err = null;
                return null;
            }
        }
         public async Task SaveYIData(DataRow yiDataRow, int TRANSACTION_ID_HDR, int seq, int qty)
        {
            string result = "0";
            string userID = "INTEGRATION";
            string createdby = userID;
            string loc = HttpContextHelper.Current.Connection.RemoteIpAddress?.ToString().ToString();
            string pc = Environment.MachineName;

            try
            {
                string constr = await GetConnectionStringAsync(false, "LIVE_ORA_QAS");

                using (OracleConnection con = new OracleConnection(constr))
                {
                    using (OracleCommand cmd = new OracleCommand("SP_TPM_INSPECTION_YI_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new OracleParameter("PID", TRANSACTION_ID_HDR)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PDID", seq)).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("Plot", yiDataRow["lotno"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("Pserial", yiDataRow["serial"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("PTAGNOFROM", yiDataRow["dlTAGNOFROM"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PTAGNOTO", yiDataRow["TAGNOTO"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("Preading", yiDataRow["reading"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("Pqty", qty)).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("pUPDATE_DATE", DateTime.Parse(yiDataRow["UPDATED_DATE"].ToString()).ToString("dd/MM/yyyy hh:mm:ss tt"))).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("pUPDATED_BY", yiDataRow["UPDATED_BY"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("pCREATION_DATE", DateTime.Parse(yiDataRow["CREATED_DATE"].ToString()).ToString("dd/MM/yyyy hh:mm:ss tt"))).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("pCreatedBy", yiDataRow["CREATED_BY"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.Int64, 20)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new OracleParameter("MSG", OracleDbType.Varchar2, 1000)).Direction = ParameterDirection.Output;
                        cmd.ExecuteReader();


                        result = Convert.ToString(cmd.Parameters["RETURN_VALUE"].Value.ToString());
                        string msgs = Convert.ToString(cmd.Parameters["MSG"].Value.ToString());

                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, userID);
                err = null;

                result = ex.Message;
            }
        }
        #endregion

        #region BS Data
        public async Task<DataTable> GetBSData(int ID_IDS_H, string grade, string lot)
        {
            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_INT_IDS_YIBS_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", ID_IDS_H)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@prop", "Black Speck")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@lead", 3)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@grade", grade)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@lot", lot)).Direction = ParameterDirection.Input;
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
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, "INTEGRATION");
                err = null;
                return null;
            }
        }

         public async Task SaveBSData(DataRow bsDataRow, int TRANSACTION_ID_HDR, int seq, int qty)
        {
            string result = "0";
            string userID = "INTEGRATION";
            string createdby = userID;
            string loc = HttpContextHelper.Current.Connection.RemoteIpAddress?.ToString().ToString();
            string pc = Environment.MachineName;

            try
            {
                string constr = await GetConnectionStringAsync(false, "LIVE_ORA_QAS");

                using (OracleConnection con = new OracleConnection(constr))
                {
                    using (OracleCommand cmd = new OracleCommand("SP_TPM_INSPECTION_BS_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new OracleParameter("PID", TRANSACTION_ID_HDR)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PDID", seq)).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("lot", bsDataRow["lotno"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("serial", bsDataRow["serial"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("TAGNOFROM", bsDataRow["dlTAGNOFROM"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("TAGNOTO", bsDataRow["TAGNOTO"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("Spot1", bsDataRow["Spot-1"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("Spot2", bsDataRow["Spot-2"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("Spot3", bsDataRow["Spot-3"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("Pqty", qty)).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("pUPDATE_DATE", DateTime.Parse(bsDataRow["UPDATED_DATE"].ToString()).ToString("dd/MM/yyyy hh:mm:ss tt"))).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("pUPDATED_BY", bsDataRow["UPDATED_BY"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("pCREATION_DATE", DateTime.Parse(bsDataRow["CREATED_DATE"].ToString()).ToString("dd/MM/yyyy hh:mm:ss tt"))).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("pCreatedBy", bsDataRow["CREATED_BY"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.Int64, 20)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new OracleParameter("MSG", OracleDbType.Varchar2, 1000)).Direction = ParameterDirection.Output;

                        cmd.ExecuteReader();


                        result = Convert.ToString(cmd.Parameters["RETURN_VALUE"].Value.ToString());

                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, userID);
                err = null;

                result = ex.Message;
            }
        }
        #endregion

        #region COQ Data
        public async Task<DataTable> GetCOQData(int ID_IDS_H)
        {
            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_INT_IDS_COQ_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", ID_IDS_H)).Direction = ParameterDirection.Input;
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
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, "INTEGRATION");
                err = null;
                return null;
            }
        }

        public async Task SaveCOQData(DataRow coqDataRow, int TRANSACTION_ID_HDR, int seq)
        {
            string result = "0";
            string userID = "INTEGRATION";
            string createdby = userID;
            string loc = HttpContextHelper.Current.Connection.RemoteIpAddress?.ToString().ToString();
            string pc = Environment.MachineName;

            try
            {
                string constr = await GetConnectionStringAsync(false, "LIVE_ORA_QAS");

                using (OracleConnection con = new OracleConnection(constr))
                {
                    using (OracleCommand cmd = new OracleCommand("SP_TPM_INSPECTION_COQ_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new OracleParameter("PID", TRANSACTION_ID_HDR)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PDID", seq)).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("Plot", coqDataRow["lotno"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("MFR", coqDataRow["MFR"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("CHARPY", coqDataRow["CHARPY"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("HDT", coqDataRow["HDT"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("Tensile_Strength", coqDataRow["Tensile_Strength"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("Tensile_Elongation", coqDataRow["Tensile_Elongation"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("Tensile_Modulus", coqDataRow["Tensile_Modulus"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("Flexural_Strength", coqDataRow["Flexural_Strength"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("Flexural_Modulus", coqDataRow["Flexural_Modulus"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("Haze", coqDataRow["Haze"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("Gloss", coqDataRow["Gloss"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("Fisheye_Film", coqDataRow["Fisheye_Film"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("Fisheye_Plate", coqDataRow["Fisheye_Plate"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("Pellet_Weight", coqDataRow["Pellet_Weight"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("Density", coqDataRow["Density"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("Powder_Content", coqDataRow["Powder_Content"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("Glass_Content", coqDataRow["Glass_Content"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("Bromine_Content", coqDataRow["Bromine_Content"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("Moisture_Content", coqDataRow["Moisture_Content"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("Macaroni", coqDataRow["Macaroni"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("Twin_Pellet", coqDataRow["Twin_Pellet"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("Long_Pellet", coqDataRow["Long_Pellet"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("ColorL", coqDataRow["ColorL"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("ColorA", coqDataRow["ColorA"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("ColorB", coqDataRow["ColorB"].ToString())).Direction = ParameterDirection.Input;


                        cmd.Parameters.Add(new OracleParameter("pUPDATE_DATE", DateTime.Parse(coqDataRow["UPDATED_DATE"].ToString()).ToString("dd/MM/yyyy hh:mm:ss tt"))).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("pUPDATED_BY", coqDataRow["UPDATED_BY"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("pCREATION_DATE", DateTime.Parse(coqDataRow["CREATED_DATE"].ToString()).ToString("dd/MM/yyyy hh:mm:ss tt"))).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("pCreatedBy", coqDataRow["CREATED_BY"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.Int64, 20)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new OracleParameter("MSG", OracleDbType.Varchar2, 1000)).Direction = ParameterDirection.Output;
                        cmd.ExecuteReader();


                        result = Convert.ToString(cmd.Parameters["RETURN_VALUE"].Value.ToString());

                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, userID);
                err = null;

                result = ex.Message;
            }
        }

        #endregion

        #endregion

        #region EXTERNAL Final Data
        public async Task<DataTable> GetExtIDSData()
        {

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_EXT_INT_IDS_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

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
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, "INTEGRATION");
                err = null;
                return null;
            }
        }

        public async Task<int> SaveExtHeaderData(DataRow row)
        {
            try
            {
                string constr = await GetConnectionStringAsync(false, "LIVE_ORA_QAS");

                using (OracleConnection con = new OracleConnection(constr))
                {
                    using (OracleCommand cmd = new OracleCommand("SP_TPM_INSPECTION_PLAN_H_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new OracleParameter("PORGANIZATION_CODE", row["Org_code"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PPLAN_NAME", row["Plan_name"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PPLAN_DESCRIPTION", row["Plan_Desc"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PPLAN_TYPE", row["Plan_Type"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PLOT_SPLIT_REQUIRED", row["lot"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("PITEM_NUMBER", row["PRODUCT_CODE"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PTPM_LOT_NUMBER", row["LOTNO"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PQUANTITY", row["QUANTITY"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PGRADE", row["GRADE"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("PMELT_FLOW_RATE", row["MFR"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PCHARPY_IMPACT_STRENGTH", row["Charpy"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PDEFLECTION_TEMP_LOAD", row["HDT"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PTENSILE_STRENGTH", row["Tensile_Strength"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PTENSILE_ELONGATION", row["Tensile_Elongation"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("PTENSILE_MODULUS", row["Tensile_Modulus"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PFLEXURAL_STRENGTH", row["Flexural_Strength"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PFLEXURAL_MODULUS", row["Flexural_Modulus"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PHAZE", row["Haze"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PGLOSS", row["Gloss"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("PFISHEYE_FILM", row["Fisheye_Film"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PFISHEYE_PLATE", row["Fisheye_Plate"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PPELLET_WEIGHT", row["Pellet_Weight"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PDENSITY", row["Density"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PPOWDER_CONTENT", row["Powder_Content"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("PGLASS_CONTENT", row["Glass_Content"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PBROMINE_CONTENT", row["Bromine_Content"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PMAISTURE_CONTENT", row["Moisture_Content"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PMACARONI", row["Macaroni"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PTWIN_PELLET", row["Twin_Pellet"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("PLONG_PELLET", row["Long_Pellet"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PCOLOR_L", row["ColorL"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PCOLOR_A", row["ColorA"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PCOLOR_B", row["ColorB"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PFLAMMABILITY", row["Flammability"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PPLATE_YI", row["PLATEYI"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("PTAG_NO_FROM", row["TAGNOFROM"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PTAG_NO_TO", row["TAGNOTO"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PYI", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PBS1", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PBS2", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PBS3", "")).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("PLAST_UPDATE_DATE", row["UPDATED_DATE"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PLAST_UPDATED_BY", row["UPDATED_BY"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PCREATION_DATE", row["CREATED_DATE"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PCREATED_BY", row["CREATED_BY"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.Int64, 20)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new OracleParameter("MSG", OracleDbType.Varchar2, 1000)).Direction = ParameterDirection.Output;

                        cmd.ExecuteReader();


                        int result = Convert.ToInt32(cmd.Parameters["RETURN_VALUE"].Value.ToString());
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, "INTEGRATION");
                err = null;

                return -1;
            }
        }

        public async Task IntegrateExtFinalData(int ID_EXT_IDS_H)
        {
            string userID = "INTEGRATION";
            string createdby = userID;
            string loc = HttpContextHelper.Current.Connection.RemoteIpAddress?.ToString().ToString();
            string pc = Environment.MachineName;

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_EXT_IDS_H_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@PID_EXT_IDS_H", ID_EXT_IDS_H)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PID_COCA_H", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PMM_PRODGROUP_H_ID", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PCOMPANY", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PCURRCOMPANY", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PPRODUCTCODE", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PPRODUCTTYPE", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PCOLORCODE", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PLOTNO", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PQUANTITY", "")).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new SqlParameter("@PPRODDATE", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PPRODLINELOTNO", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PPREVPRODTYPE", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PNATRESINPC1", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PNATRESINPC2", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PNATRESINPC3", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PNATRESINPT1", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PNATRESINPT2", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PNATRESINPT3", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PNATRESINLOT1", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PNATRESINLOT2", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PNATRESINLOT3", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PNATRESINGRD1", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PNATRESINGRD2", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PNATRESINGRD3", "")).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new SqlParameter("@PCOLOUR", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PDISPERSION", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@POVERWRITEGRADE", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PTESTEDBY", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PUPDATEDBY", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PSTATUS", "")).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new SqlParameter("@PAPPRSTATUS", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PCONTSTATUS", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PCONTSTATUSDATE", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PCONTCHKBY", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PCONTCHKBYDATE", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PCONTAPPRBY", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PCONTAPPRBYDATE", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PCONTREMARK", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PPSYITEST", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PPSFISHEYE", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PPSBROMINE", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PPSPROPERTIES", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@POVERWRITEMOLD", "")).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new SqlParameter("@PTPMGRADE", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PTPMGRADEDATE", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PTPMCHKBY", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PTPMCHKBYDATE", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PTPMAPPRBY", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PTPMAPPRBYDATE", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PTPMREMARK", "")).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new SqlParameter("@pRecType", "2")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCreatedBy", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCreatedLoc", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCreatedEmail", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ReturnID", SqlDbType.Int)).Direction = ParameterDirection.Output;

                        int result = await cmd.ExecuteNonQueryAsync();
                        Message = result > 0 ? "Success" : "Failure";

                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, userID);
                err = null;
            }
        }

        #region YI Data
        public async Task<DataTable> GetEXTYIData(int ID_EXT_IDS_H, string grade, string lot)
        {

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_INT_EXT_IDS_YIBS_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", ID_EXT_IDS_H)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@prop", "YI")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@lead", 1)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@grade", grade)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@lot", lot)).Direction = ParameterDirection.Input;
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
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, "INTEGRATION");
                err = null;
                return null;
            }
        }
        public async Task SaveEXTYIData(DataRow yiDataRow, int TRANSACTION_ID_HDR, int seq, int qty)
        {
            string result = "0";
            string userID = "INTEGRATION";
            string createdby = userID;
            string loc = HttpContextHelper.Current.Connection.RemoteIpAddress?.ToString().ToString();
            string pc = Environment.MachineName;

            try
            {
                string constr = await GetConnectionStringAsync(false, "LIVE_ORA_QAS");

                using (OracleConnection con = new OracleConnection(constr))
                {
                    using (OracleCommand cmd = new OracleCommand("SP_TPM_INSPECTION_YI_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new OracleParameter("PID", TRANSACTION_ID_HDR)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PDID", seq)).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("Plot", yiDataRow["LOTNO"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("Pserial", yiDataRow["SERIAL"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("PTAGNOFROM", yiDataRow["BAGNOFROM"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PTAGNOTO", yiDataRow["BAGNOTO"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("Preading", yiDataRow["READING"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("Pqty", qty)).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("pUPDATE_DATE", DateTime.Parse(yiDataRow["UPDATED_DATE"].ToString()).ToString("dd/MM/yyyy hh:mm:ss tt"))).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("pUPDATED_BY", yiDataRow["UPDATED_BY"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("pCREATION_DATE", DateTime.Parse(yiDataRow["CREATED_DATE"].ToString()).ToString("dd/MM/yyyy hh:mm:ss tt"))).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("pCreatedBy", yiDataRow["CREATED_BY"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.Int64, 20)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new OracleParameter("MSG", OracleDbType.Varchar2, 1000)).Direction = ParameterDirection.Output;
                        cmd.ExecuteReader();


                        result = Convert.ToString(cmd.Parameters["RETURN_VALUE"].Value.ToString());
                        string msgs = Convert.ToString(cmd.Parameters["MSG"].Value.ToString());

                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, userID);
                err = null;

                result = ex.Message;
            }
        }
        #endregion

        #region BS Data
        public async Task<DataTable> GetEXTBSData(int ID_IDS_H, string grade, string lot)
        {
            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_INT_EXT_IDS_YIBS_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", ID_IDS_H)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@prop", "Black Speck")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@lead", 3)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@grade", grade)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@lot", lot)).Direction = ParameterDirection.Input;
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
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, "INTEGRATION");
                err = null;
                return null;
            }
        }

        public async Task SaveEXTBSData(DataRow bsDataRow, int TRANSACTION_ID_HDR, int seq, int qty)
        {
            string result = "0";
            string userID = "INTEGRATION";
            string createdby = userID;
            string loc = HttpContextHelper.Current.Connection.RemoteIpAddress?.ToString().ToString();
            string pc = Environment.MachineName;

            try
            {
                string constr = await GetConnectionStringAsync(false, "LIVE_ORA_QAS");

                using (OracleConnection con = new OracleConnection(constr))
                {
                    using (OracleCommand cmd = new OracleCommand("SP_TPM_INSPECTION_BS_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new OracleParameter("PID", TRANSACTION_ID_HDR)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("PDID", seq)).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("lot", bsDataRow["LOTNO"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("serial", bsDataRow["SERIAL"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("TAGNOFROM", bsDataRow["BAGNOFROM"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("TAGNOTO", bsDataRow["BAGNOTO"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("Spot1", bsDataRow["Spot-1"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("Spot2", bsDataRow["Spot-2"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("Spot3", bsDataRow["Spot-3"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("Pqty", qty)).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("pUPDATE_DATE", DateTime.Parse(bsDataRow["UPDATED_DATE"].ToString()).ToString("dd/MM/yyyy hh:mm:ss tt"))).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("pUPDATED_BY", bsDataRow["UPDATED_BY"].ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("pCREATION_DATE", DateTime.Parse(bsDataRow["CREATED_DATE"].ToString()).ToString("dd/MM/yyyy hh:mm:ss tt"))).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new OracleParameter("pCreatedBy", bsDataRow["CREATED_BY"].ToString())).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.Int64, 20)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new OracleParameter("MSG", OracleDbType.Varchar2, 1000)).Direction = ParameterDirection.Output;

                        cmd.ExecuteReader();


                        result = Convert.ToString(cmd.Parameters["RETURN_VALUE"].Value.ToString());

                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, userID);
                err = null;

                result = ex.Message;
            }
        }
        #endregion

        #endregion

        #region grn
        public async Task<DataTable> getUngradedGRN()
        {
            ACL_UserObj userobj = HttpContextHelper.Current.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            List<OraMaterialList> oramat = new List<OraMaterialList>();
            try
            {
                string constr = await GetConnectionStringERPCOAAsync();
                DataTable dt = new DataTable();

                using (OracleConnection myConnection = new OracleConnection(constr))
                {
                    using (OracleCommand cmd = new OracleCommand("SP_RM_REC_PENDING_LIST", myConnection))
                    {
                        myConnection.Open();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new OracleParameter("REFCURSOR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;
                        OracleDataReader rdr = cmd.ExecuteReader();

                        dt.Load(rdr);
                        return dt;
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

        }

        public async Task<string> setIntGRNDataMaint(int idh, COAGRNDETAIL model)
        {
            string result = "0";
            string userID = "INTEGRATION";
            string createdby = userID;
            string loc = HttpContextHelper.Current.Connection.RemoteIpAddress?.ToString().ToString();
            string pc = Environment.MachineName;

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_INT_COA_GRN_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@pGRNDATE", model.GRN_DATE)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PITEMCODE", model.ITEM_CODE)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pITEMDESC", model.ITEM_DESC)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PVENDORLOT", model.VENDOR_LOTNO)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PUOM", model.UOM)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PQTY", model.LOTQTY)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PORG", model.ORGANIZATION)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pRecType", "1")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCreatedBy", "IntegrationSch")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCreatedLoc", loc)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pLast", model.last)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ReturnID", SqlDbType.Int)).Direction = ParameterDirection.Output;
                        cmd.ExecuteReader();

                        result = Convert.ToString(cmd.Parameters["@ReturnID"].Value);

                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, userID);
                err = null;

                result = ex.Message;
            }

            return result;
        }


        #endregion

        #region W Grade Production Listing
        public async Task<DataTable> getWGradePL()
        {
            ACL_UserObj userobj = HttpContextHelper.Current.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            string year = DateTime.Now.Year.ToString();
            string month = DateTime.Now.Month.ToString();
            string Subinventory_From = " ";
            string Subinventory_To = "zzzz";

            string Company = "TPM";
            string User_id = "INTEGRATION";

            string constr = await GetConnectionStringERPCOAAsync();
            DataTable combinedDt = new DataTable();

            try
            {
                using (OracleConnection myConnection = new OracleConnection(constr))
                {
                    myConnection.Open();

                    // Helper function to get DataTable per range
                    async Task<DataTable> GetDataByProdLineRange(string prodLineFrom, string prodLineTo)
                    {
                        using (OracleCommand cmd = new OracleCommand("SP_TPM_MONTHLY_W_GRADE_INT", myConnection))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 0;
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new OracleParameter("P_YEAR", year)).Direction = ParameterDirection.Input;
                            cmd.Parameters.Add(new OracleParameter("P_MONTH", month)).Direction = ParameterDirection.Input;
                            cmd.Parameters.Add(new OracleParameter("P_SUBINVENTORY_FROM", Subinventory_From)).Direction = ParameterDirection.Input;
                            cmd.Parameters.Add(new OracleParameter("P_SUBINVENTORY_TO", Subinventory_To)).Direction = ParameterDirection.Input;
                            cmd.Parameters.Add(new OracleParameter("P_PROD_LINE_FROM", prodLineFrom)).Direction = ParameterDirection.Input;
                            cmd.Parameters.Add(new OracleParameter("P_PROD_LINE_TO", prodLineTo)).Direction = ParameterDirection.Input;
                            cmd.Parameters.Add(new OracleParameter("P_ORGANIZATION", Company)).Direction = ParameterDirection.Input;
                            cmd.Parameters.Add(new OracleParameter("P_USER_ID", User_id)).Direction = ParameterDirection.Input;

                            cmd.Parameters.Add(new OracleParameter("REFCURSOR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                            using (OracleDataReader rdr = (OracleDataReader)await cmd.ExecuteReaderAsync())
                            {
                                DataTable dt = new DataTable();
                                dt.Load(rdr);
                                return dt;
                            }
                        }
                    }

                    // First range
                    DataTable dt1 = await GetDataByProdLineRange("0", "7");

                    // Second range
                    DataTable dt2 = await GetDataByProdLineRange("20", "28");

                    // Combine results
                    combinedDt = dt1.Clone(); // Clone schema

                    foreach (DataRow row in dt1.Rows)
                        combinedDt.ImportRow(row);

                    foreach (DataRow row in dt2.Rows)
                        combinedDt.ImportRow(row);

                    return combinedDt;
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

        public async Task<string> setIntWGradePLDataMaint(int idh, DataTable data)
        {
            string result = "0";
            string userID = "INTEGRATION";
            string createdby = userID;
            string loc = HttpContextHelper.Current.Connection.RemoteIpAddress?.ToString().ToString();
            string pc = Environment.MachineName;

            string year = DateTime.Now.Year.ToString();
            string month = DateTime.Now.Month.ToString();

            DataTable reordered = new DataTable();
            reordered.Columns.Add("ITEM_NAME", typeof(string));
            reordered.Columns.Add("ITEM_DECSRIPTION", typeof(string));
            reordered.Columns.Add("LOT_GRADE", typeof(string));
            reordered.Columns.Add("LOT_NUMBER", typeof(string));
            reordered.Columns.Add("PACKED_DATE", typeof(DateTime));
            reordered.Columns.Add("OPENING_BALANCE", typeof(int));
            reordered.Columns.Add("BALANCE_ADJUST", typeof(int));
            reordered.Columns.Add("PROD", typeof(int));
            reordered.Columns.Add("DIS_ADJUST", typeof(int));
            reordered.Columns.Add("TOTAL_IN", typeof(int));
            reordered.Columns.Add("FOR_GAN", typeof(int));
            reordered.Columns.Add("FOR_SEA", typeof(int));
            reordered.Columns.Add("DIS_ADJUST2", typeof(int));
            reordered.Columns.Add("TOTAL_OUT", typeof(int));
            reordered.Columns.Add("MONTHLY_BALANCE", typeof(int));
            reordered.Columns.Add("WAREHOUSE", typeof(string));
            reordered.Columns.Add("YEAR", typeof(string));
            reordered.Columns.Add("MONTH", typeof(string));
            reordered.Columns.Add("USER_ID", typeof(string));
            reordered.Columns.Add("PROD_LINE", typeof(int));
            if (data != null)
            {
                foreach (DataRow row in data.Rows)
                {
                    reordered.Rows.Add(
                        row["ITEM_NAME"],
                        row["ITEM_DECSRIPTION"],
                        row["LOT_GRADE"],
                        row["LOT_NUMBER"],
                        row["PACKED_DATE"],
                        row["OPENING_BALANCE"],
                        row["BALANCE_ADJUST"],
                        row["PROD"],
                        row["DIS_ADJUST"],
                        row["TOTAL_IN"],
                        row["FOR_GAN"],
                        row["FOR_SEA"],
                        row["DIS_ADJUST2"],
                        row["TOTAL_OUT"],
                        row["MONTHLY_BALANCE"],
                        row["WAREHOUSE"],
                        row["YEAR"],
                        row["MONTH"],
                        row["USER_ID"],
                        row["PROD_LINE"]
                    );
                }
            }

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_INT_W_GRADE_PL_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        // Pass the DataTable as structured parameter
                        SqlParameter tvpParam = cmd.Parameters.AddWithValue("@TBL_DETAIL", reordered);
                        tvpParam.SqlDbType = SqlDbType.Structured;
                        tvpParam.TypeName = "dbo.MONTHLY_OVERALL_W_GRADE_REPORT_Type";

                        cmd.Parameters.Add(new SqlParameter("@pYEAR", year)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pMONTH", month)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pUSER_ID", userID)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ReturnID", SqlDbType.Int)).Direction = ParameterDirection.Output;
                        cmd.ExecuteReader();

                        result = Convert.ToString(cmd.Parameters["@ReturnID"].Value);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, userID);
                err = null;

                result = ex.Message;
            }

            return result;
        }

        #endregion

    }
}