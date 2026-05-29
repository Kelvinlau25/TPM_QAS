using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TPM_QAS.Helpers;
using TPM_QAS.Models;

namespace TPM_QAS.DAL
{
    public class COA_GA_RAWMAT_ABNORM_DAL : Database
    {
        public async Task<DataTable> getdataforabnorm(string ID, string datefr, string dateto)
        {
            ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_GA_ABNORM_DET_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@PID", ID)).Direction = System.Data.ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pdatefrom", datefr)).Direction = System.Data.ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pdateto", dateto)).Direction = System.Data.ParameterDirection.Input;
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

        public async Task<string> checkAbnorm(int pdid, string rectype)
        {
            string result = "OK";

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
                    using (SqlCommand cmd = new SqlCommand("PSP_COA_DETECT_ABNORM", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@pID", pdid)).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new SqlParameter("@pRecType", rectype)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCreatedBy", createdby)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCreatedLoc", loc)).Direction = ParameterDirection.Input;
                        //cmd.Parameters.Add(new SqlParameter("@ReturnID", SqlDbType.Int)).Direction = ParameterDirection.Output;
                        cmd.ExecuteReader();


                        //result = Convert.ToString(cmd.Parameters["@ReturnID"].Value);

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

        //    public async Task<string> COAMM_MATERIAL_H_Maint(COAMMMaterialModel model)
        //    {
        //        string result = "0";

        //        ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
        //        string USERID = userobj.EMP_NAME.ToString();
        //        string createdby = USERID;
        //        string loc = HttpContext.Current.Request.UserHostAddress.ToString();
        //        string pc = Environment.MachineName;

        //        try
        //        {
        //            string constr = await GetConnectionStringAsync();

        //            using (SqlConnection con = new SqlConnection(constr))
        //            {
        //                using (SqlCommand cmd = new SqlCommand("PSP_MM_MATERIAL_H_MAINT", con))
        //                {
        //                    await con.OpenAsync();
        //                    cmd.CommandType = CommandType.StoredProcedure;
        //                    cmd.CommandTimeout = 0;

        //                    cmd.Parameters.Add(new SqlParameter("@pID", model.MM_MATERIAL_H_ID)).Direction = ParameterDirection.Input;
        //                    cmd.Parameters.Add(new SqlParameter("@PMaterialName", model.MATERIAL_NAME)).Direction = ParameterDirection.Input;
        //                    cmd.Parameters.Add(new SqlParameter("@PMaterialAbbr", model.MATERIAL_ABBR)).Direction = ParameterDirection.Input;
        //                    cmd.Parameters.Add(new SqlParameter("@PMaterialCode", model.MATERIAL_CODE)).Direction = ParameterDirection.Input;
        //                    cmd.Parameters.Add(new SqlParameter("@PTPMCode", model.TPM_CODE)).Direction = ParameterDirection.Input;

        //                    cmd.Parameters.Add(new SqlParameter("@pRecType", model.RECORD_TYP)).Direction = ParameterDirection.Input;
        //                    cmd.Parameters.Add(new SqlParameter("@pCreatedBy", createdby)).Direction = ParameterDirection.Input;
        //                    cmd.Parameters.Add(new SqlParameter("@pCreatedLoc", loc)).Direction = ParameterDirection.Input;
        //                    cmd.Parameters.Add(new SqlParameter("@ReturnID", SqlDbType.Int)).Direction = ParameterDirection.Output;
        //                    cmd.ExecuteReader();


        //                    result = Convert.ToString(cmd.Parameters["@ReturnID"].Value);

        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            ErrorLogSys err = new ErrorLogSys();
        //            await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
        //            err = null;

        //            result = ex.Message;
        //        }

        //        return result;
        //    }

        //    public async Task<string> COAMM_MATERIAL_D1_Maint(int pMM_MATERIAL_D1_ID, int pMM_MATERIAL_H_ID, string pMATERIAL_NAME,
        //            string pTPM_SPEC_NAME, string pTPM_SPEC_VALUE, string pTPM_SPEC_TYPE, string pTPM_SPEC_UOM, string pfirst, string rectype)
        //    {
        //        string result = "0";

        //        ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
        //        string USERID = userobj.EMP_NAME.ToString();
        //        string createdby = USERID;
        //        string loc = HttpContext.Current.Request.UserHostAddress.ToString();
        //        string pc = Environment.MachineName;

        //        try
        //        {
        //            string constr = await GetConnectionStringAsync();

        //            using (SqlConnection con = new SqlConnection(constr))
        //            {
        //                using (SqlCommand cmd = new SqlCommand("PSP_MM_MATERIAL_D1_MAINT", con))
        //                {
        //                    await con.OpenAsync();
        //                    cmd.CommandType = CommandType.StoredProcedure;
        //                    cmd.CommandTimeout = 0;

        //                    cmd.Parameters.Add(new SqlParameter("@PMM_MATERIAL_D1_ID", pMM_MATERIAL_D1_ID)).Direction = ParameterDirection.Input;
        //                    cmd.Parameters.Add(new SqlParameter("@PMM_MATERIAL_H_ID", pMM_MATERIAL_H_ID)).Direction = ParameterDirection.Input;
        //                    cmd.Parameters.Add(new SqlParameter("@PMATERIAL_NAME", pMATERIAL_NAME)).Direction = ParameterDirection.Input;
        //                    cmd.Parameters.Add(new SqlParameter("@PTPM_SPEC_NAME", pTPM_SPEC_NAME)).Direction = ParameterDirection.Input;
        //                    cmd.Parameters.Add(new SqlParameter("@PTPM_SPEC_VALUE", pTPM_SPEC_VALUE)).Direction = ParameterDirection.Input;
        //                    cmd.Parameters.Add(new SqlParameter("@PTPM_SPEC_TYPE", pTPM_SPEC_TYPE)).Direction = ParameterDirection.Input;
        //                    cmd.Parameters.Add(new SqlParameter("@PTPM_SPEC_UOM", pTPM_SPEC_UOM)).Direction = ParameterDirection.Input;
        //                    cmd.Parameters.Add(new SqlParameter("@pFIRST", pfirst)).Direction = ParameterDirection.Input;
        //                    cmd.Parameters.Add(new SqlParameter("@pRecType", rectype)).Direction = ParameterDirection.Input;
        //                    cmd.Parameters.Add(new SqlParameter("@pCreatedBy", createdby)).Direction = ParameterDirection.Input;
        //                    cmd.Parameters.Add(new SqlParameter("@pCreatedLoc", loc)).Direction = ParameterDirection.Input;
        //                    cmd.Parameters.Add(new SqlParameter("@ReturnID", SqlDbType.Int)).Direction = ParameterDirection.Output;
        //                    cmd.ExecuteReader();

        //                    result = Convert.ToString(cmd.Parameters["@ReturnID"].Value);
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            ErrorLogSys err = new ErrorLogSys();
        //            await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
        //            err = null;

        //            result = ex.Message;
        //        }

        //        return result;
        //    }

        //    public async Task<string> COAMM_MATERIAL_D2_Maint(int pMM_MATERIAL_D2_ID, int pMM_MATERIAL_H_ID, int pMM_SUPPLIER_H_ID, int pMM_MATERIAL_D1_ID, string pMATERIAL_NAME,
        //            string pSUPP_MATERIAL_NAME,  string pSUPP_SPEC_NAME, string pSUPP_SPEC_VALUE, string pSUPP_SPEC_TYPE, string pSUPP_SPEC_UOM, string pfirst, string rectype)
        //    {
        //        string result = "0";

        //        ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
        //        string USERID = userobj.EMP_NAME.ToString();
        //        string createdby = USERID;
        //        string loc = HttpContext.Current.Request.UserHostAddress.ToString();
        //        string pc = Environment.MachineName;

        //        try
        //        {
        //            string constr = await GetConnectionStringAsync();

        //            using (SqlConnection con = new SqlConnection(constr))
        //            {
        //                using (SqlCommand cmd = new SqlCommand("PSP_MM_MATERIAL_D2_MAINT", con))
        //                {
        //                    await con.OpenAsync();
        //                    cmd.CommandType = CommandType.StoredProcedure;
        //                    cmd.CommandTimeout = 0;

        //                    cmd.Parameters.Add(new SqlParameter("@PMM_MATERIAL_D2_ID", pMM_MATERIAL_D2_ID)).Direction = ParameterDirection.Input;
        //                    cmd.Parameters.Add(new SqlParameter("@PMM_MATERIAL_H_ID", pMM_MATERIAL_H_ID)).Direction = ParameterDirection.Input;
        //                    cmd.Parameters.Add(new SqlParameter("@PMM_SUPPLIER_H_ID", pMM_SUPPLIER_H_ID)).Direction = ParameterDirection.Input;
        //                    cmd.Parameters.Add(new SqlParameter("@PMM_MATERIAL_D1_ID", pMM_MATERIAL_D1_ID)).Direction = ParameterDirection.Input;
        //                    cmd.Parameters.Add(new SqlParameter("@PMATERIAL_NAME", pMATERIAL_NAME)).Direction = ParameterDirection.Input;
        //                    cmd.Parameters.Add(new SqlParameter("@PSUPP_MATERIAL_NAME", pSUPP_MATERIAL_NAME)).Direction = ParameterDirection.Input;
        //                    cmd.Parameters.Add(new SqlParameter("@PSUPP_SPEC_NAME", pSUPP_SPEC_NAME)).Direction = ParameterDirection.Input;
        //                    cmd.Parameters.Add(new SqlParameter("@PSUPP_SPEC_VALUE", pSUPP_SPEC_VALUE)).Direction = ParameterDirection.Input;
        //                    cmd.Parameters.Add(new SqlParameter("@PSUPP_SPEC_TYPE", pSUPP_SPEC_TYPE)).Direction = ParameterDirection.Input;
        //                    cmd.Parameters.Add(new SqlParameter("@PSUPP_SPEC_UOM", pSUPP_SPEC_UOM)).Direction = ParameterDirection.Input;
        //                    cmd.Parameters.Add(new SqlParameter("@pFIRST", pfirst)).Direction = ParameterDirection.Input;
        //                    cmd.Parameters.Add(new SqlParameter("@pRecType", rectype)).Direction = ParameterDirection.Input;
        //                    cmd.Parameters.Add(new SqlParameter("@pCreatedBy", createdby)).Direction = ParameterDirection.Input;
        //                    cmd.Parameters.Add(new SqlParameter("@pCreatedLoc", loc)).Direction = ParameterDirection.Input;
        //                    cmd.Parameters.Add(new SqlParameter("@ReturnID", SqlDbType.Int)).Direction = ParameterDirection.Output;
        //                    cmd.ExecuteReader();

        //                    result = Convert.ToString(cmd.Parameters["@ReturnID"].Value);
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            ErrorLogSys err = new ErrorLogSys();
        //            await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
        //            err = null;

        //            result = ex.Message;
        //        }

        //        return result;
        //    }       

        //    public async Task<DataTable> getDllData(int ID, string act, string category)
        //    {
        //        ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
        //        string USERID = userobj.EMP_NAME.ToString();

        //        try
        //        {
        //            string constr = await GetConnectionStringAsync();
        //            DataTable dt = new DataTable();

        //            using (SqlConnection con = new SqlConnection(constr))
        //            {
        //                using (SqlCommand cmd = new SqlCommand("PSP_COA_GET_DLL", con))
        //                {
        //                    await con.OpenAsync();
        //                    cmd.CommandType = CommandType.StoredProcedure;
        //                    cmd.CommandTimeout = 0;
        //                    cmd.Parameters.Clear();

        //                    cmd.Parameters.Add(new SqlParameter("@pID", ID)).Direction = System.Data.ParameterDirection.Input;
        //                    cmd.Parameters.Add(new SqlParameter("@pACTION", act)).Direction = System.Data.ParameterDirection.Input;
        //                    cmd.Parameters.Add(new SqlParameter("@pCATEGORY", category)).Direction = System.Data.ParameterDirection.Input;
        //                    reader = await cmd.ExecuteReaderAsync();
        //                    dt.Load(reader);
        //                    reader.Close();

        //                }
        //            }

        //            if (dt != null && dt.Rows.Count > 0)
        //            {
        //                return dt;
        //            }
        //            else
        //            {
        //                return null;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            ErrorLogSys err = new ErrorLogSys();
        //            await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
        //            err = null;
        //            return null;
        //        }
        //    }
    }
}