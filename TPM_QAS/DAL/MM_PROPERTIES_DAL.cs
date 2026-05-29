using System;
using System.Collections.Generic;
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
    public class MM_PROPERTIES_DAL : Database
    {
        public async Task<DataTable> getProperties_Data(string ID, string type)
        {
            ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_PROPERTIES_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", ID)).Direction = System.Data.ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pType", type)).Direction = System.Data.ParameterDirection.Input;
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

        public async Task<string> Properties_H_Maint(PropertiesModel model)
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
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_PROPERTIES_H_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@pID", model.MM_PROPERTIES_H_ID)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PProp", model.PROPERTIES)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Pitem", model.PROP_ITEM)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Pseq", model.SEQUENCE)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Pref", model.REF_METHOD)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Prem", model.REMARKS)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Poracolumn", model.OraColumn)).Direction = ParameterDirection.Input;

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

        public async Task<string> Properties_D_Maint(int pid, int phid, string name, string path, string testtime, string lotno, string poperator, 
                                        string read1, string read2, string read3, string read4, string read5, string read6,
                                        decimal totalread, string avg, string rectype)
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
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_PROPERTIES_D_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@pID", pid)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Phid", phid)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pMachineName", name)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pMachinePath", path)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pTestTime", testtime)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pLotNo", lotno)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pOperator", poperator)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pRead_1", read1)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pRead_2", read2)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pRead_3", read3)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pRead_4", read4)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pRead_5", read5)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pRead_6", read6)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pTotalRead", totalread)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pAvg", avg)).Direction = ParameterDirection.Input;

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

        public async Task<string> Properties_H_SEQ_Maint(int phid, string prop, string propitem, int seq, string rectype)
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
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_PROPERTIES_H_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@pID", phid)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PProp", prop)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Pitem", propitem)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Pseq", seq)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Pref", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Prem", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Poracolumn", "")).Direction = ParameterDirection.Input;

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

        public async Task<DataTable> getProperties_Seq(string type)
        {
            ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_PROPERTIES_SEQ", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pType", type)).Direction = System.Data.ParameterDirection.Input;
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