using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TPM_QAS.Helpers;
using TPM_QAS.Models;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace TPM_QAS.DAL
{
    public class COA_DE_OCR_DAL : Database
    {
        public async Task<DataTable> getDEOCR_Data(string ID, string type, int pdid = 0, string lotno = "")
        {
            ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_DE_OCR_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", ID)).Direction = System.Data.ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pType", type)).Direction = System.Data.ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@psupplierID", pdid)).Direction = System.Data.ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@plotno", lotno)).Direction = System.Data.ParameterDirection.Input;

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

        public async Task<DataTable> getKeyInfo(string type, string cat = "", string cat2 = "", string cat3 = "")
        {
            ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_DE_GET_KEYINFO", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pType", type)).Direction = System.Data.ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCat", cat)).Direction = System.Data.ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCat2", cat2)).Direction = System.Data.ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCat3", cat3)).Direction = System.Data.ParameterDirection.Input;
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

        public async Task<List<SupplierTab>> getsupplierlist(string ID, string type)
        {
            ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
            string USERID = userobj.EMP_NAME.ToString();

            List<SupplierTab> chop = new List<SupplierTab>();

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("PSP_DE_OCR_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter("@pID", ID)).Direction = System.Data.ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pType", type)).Direction = System.Data.ParameterDirection.Input;

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                chop.Add(new SupplierTab
                                {
                                    MM_SUPPLIER_H_ID = Convert.ToInt32(reader["MM_SUPPLIER_H_ID"]),
                                    SUPPLIER_NAME = reader["SUPPLIER_NAME"] != DBNull.Value ? reader["SUPPLIER_NAME"].ToString() : "",
                                    LOT_NO = reader["LOT_NO"] != DBNull.Value ? reader["LOT_NO"].ToString() : "",
                                    TABNAME = reader["TABNAME"] != DBNull.Value ? reader["TABNAME"].ToString() : "",
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
            }

            return chop;
        }

        //HANA 19122025 Add SP to get the specification values for analysis result
        public async Task<String> getMatSpecList(string supplier)
        {
            ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
            string USERID = userobj.EMP_NAME.ToString();

            string matspeclist = "";

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("PSP_DE_GET_MATSPEC_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter("@psuppID", supplier)).Direction = System.Data.ParameterDirection.Input;

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                matspeclist = Convert.ToString(reader["SPEC"]);
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

            return matspeclist;
        }

        public async Task<string> DE_OCR_H_Maint(COAOCRModel model)
        {
            string result = "0";

            ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
            string USERID = userobj.EMP_NAME.ToString();
            string createdby = USERID;
            string loc = HttpContext.Current.Request.UserHostAddress.ToString();
            string pc = Environment.MachineName;

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_DE_OCR_H_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@pID", model.DE_OCR_H_ID)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pEntryType", model.ENTRY_TYPE)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pOCRindicator", model.OCR_INDICATOR)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pFileName", model.FINAL_FILE_NAME)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pResult", model.FINAL_RESULT)).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new SqlParameter("@pRecType", model.RECORD_TYP)).Direction = ParameterDirection.Input;
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
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;

                result = ex.Message;
            }

            return result;
        }

        public async Task<string> DE_OCR_D_Maint(int phid, int mat_id, int supp_id, string lot_no, int d2_id, string spec, decimal cfs_spec,
                                        string val_result, decimal cfs_val_result, string comp_result, string file_name, string rectype)
        {
            string result = "0";

            ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
            string USERID = userobj.EMP_NAME.ToString();
            string createdby = USERID;
            string loc = HttpContext.Current.Request.UserHostAddress.ToString();
            string pc = Environment.MachineName;

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_DE_OCR_D_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@phid", phid)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pMM_MATERIAL_H_ID", mat_id)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pMM_SUPPLIER_H_ID", supp_id)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pFILE_NAME", file_name)).Direction = ParameterDirection.Input; 
                        cmd.Parameters.Add(new SqlParameter("@pLOT_NO", lot_no)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pMM_MATERIAL_D2_ID", d2_id)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pOCR_INSPECTION_ITEM", spec)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pOCR_INSPECTION_ITEM_SCORE", cfs_spec)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pOCR_RESULT", val_result)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pOCR_RESULT_SCORE", cfs_val_result)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCOMPARISON_RESULT", comp_result)).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new SqlParameter("@pRecType", rectype)).Direction = ParameterDirection.Input;
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
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;

                result = ex.Message;
            }

            return result;
        }

        public async Task<DataTable> CheckUserAppRole(string empno)
        {
            ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_GET_COA_USER_APP_LEVEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@PempNo", empno)).Direction = System.Data.ParameterDirection.Input;
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
    }
}