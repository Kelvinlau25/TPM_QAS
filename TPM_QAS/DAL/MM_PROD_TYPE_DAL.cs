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
    public class MM_PROD_TYPE_DAL : Database
    {
        public async Task<DataTable> getProdType_Data(string ID, string type)
        {
            ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_PRODTYPE_SEL", con))
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

        public async Task<DataTable> getPropertyPOPUP(int idh)
        {
            ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_PRODTYPE_PROP_POPUP", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pIDH", idh)).Direction = System.Data.ParameterDirection.Input;
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
        public async Task<DataTable> getProdPOPUP(int idh, string search)
        {
            ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_PRODTYPE_PROD_POPUP", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pIDH", idh)).Direction = System.Data.ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pSearch ", search)).Direction = System.Data.ParameterDirection.Input;
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

        public async Task<string> ProdType_H_Maint(ProdTypeModel model)
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
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_PRODTYPE_H_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@pID", model.MM_PRODTYPE_H_ID)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PProdType", model.PROD_TYPE)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PComp", model.COMP_GROUP)).Direction = ParameterDirection.Input;

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

        public async Task<string> ProdType_D_Maint(int pid, int phid, int propertiesid, string rectype)
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
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_PRODTYPE_D_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@pID", pid)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Phid", phid)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Ppropid", propertiesid)).Direction = ParameterDirection.Input;

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

        public async Task<string> ProdType_D1_Maint(int pid, int phid, string propmain, string propsub, string propcls, string propcolor, string proppack, string rectype)
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
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_PRODTYPE_D1_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@pID", pid)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Phid", phid)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pPropTypeMain", propmain)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pPropTypeSub", propsub)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pPropCls", propcls)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pPropColor", propcolor)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pPropPack", proppack)).Direction = ParameterDirection.Input;

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