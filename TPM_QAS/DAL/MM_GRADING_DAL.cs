using System;
using System.Collections.Generic;
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
    public class MM_GRADING_DAL : Database
    {
        public async Task<DataTable> getGrading_Data(string ID, string type)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_GRADING_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", ID)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pType", type)).Direction = ParameterDirection.Input;
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

        public async Task<string> Grading_H_Maint(GradingModel model)
        {
            string result = "0";

            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();
            string createdby = USERID;
            string loc = HttpContext.Request.UserHostAddress.ToString();
            string pc = Environment.MachineName;

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_GRADING_H_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@pID", model.MM_GRADING_H_ID)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pProdTypeID", model.MM_PRODTYPE_H_ID)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCompGroup", model.COMP_GROUP)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pProdLine", model.PROD_LINE)).Direction = ParameterDirection.Input;

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

        public async Task<string> Grading_D1_Maint(int pid, int phid, int seq, string property, int propitemID, int priority, int fullprior, 
            decimal lspec, decimal uspec, decimal lpcl, decimal upcl, decimal centreline, string grade, int rounding, string rectype)
        {
            string result = "0";

            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();
            string createdby = USERID;
            string loc = HttpContext.Request.UserHostAddress.ToString();
            string pc = Environment.MachineName;

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_GRADING_D1_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@pID", pid)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@phid", phid)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pSeq", seq)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pProperty", property)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pPropItemID", propitemID)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pPriority", priority)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pFinalPrio", fullprior)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pLspec", lspec)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pUspec", uspec)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pLpcl", lpcl)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pUpcl", upcl)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCentreLine", centreline)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pGrade", grade)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pRounding", rounding)).Direction = ParameterDirection.Input;

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

        public async Task<string> Grading_D2_Maint(int pid, int phid, int seq, string section, string field, int priority, int fullprior, 
            decimal lspec, decimal uspec, decimal lpcl, decimal upcl, decimal calculation, string grade, int rounding, string rectype)
        {
            string result = "0";

            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();
            string createdby = USERID;
            string loc = HttpContext.Request.UserHostAddress.ToString();
            string pc = Environment.MachineName;

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_GRADING_D2_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@pID", pid)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@phid", phid)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pseq", seq)).Direction = ParameterDirection.Input; 
                        cmd.Parameters.Add(new SqlParameter("@pSection", section)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pField", field)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pPriority", priority)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pFinalPrio", fullprior)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pLspec", lspec)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pUspec", uspec)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pLpcl", lpcl)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pUpcl", upcl)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCalculation", calculation)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pGrade", grade)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pRounding", rounding)).Direction = ParameterDirection.Input;

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

        public async Task<string> Grading_D3_Maint(int pid, int phid, string properties, string propitem, string passgrade, string rectype)
        {
            string result = "0";

            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();
            string createdby = USERID;
            string loc = HttpContext.Request.UserHostAddress.ToString();
            string pc = Environment.MachineName;

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_GRADING_D3_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@pID", pid)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Phid", phid)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pProperty", properties)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pPropItem", propitem)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pPassGrade", passgrade)).Direction = ParameterDirection.Input;

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
    }
}