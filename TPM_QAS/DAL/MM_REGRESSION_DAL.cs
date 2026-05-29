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
    public class MM_REGRESSION_DAL : Database
    {
        public async Task<DataTable> getRegressionData(string ID, string type)
        {
            ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_REGRESSION_SEL", con))
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

        #region TPM
        public async Task<string> Regression_H_TPM_Maint(RegressionModel model)
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
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_REGRESSION_H_TPM_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@pID", model.MM_REGRESSION_H_ID)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCompGroup", model.COMP_GROUP)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pProdLine", model.PROD_LINE)).Direction = ParameterDirection.Input; 
                        cmd.Parameters.Add(new SqlParameter("@pProperties", model.PROPERTIES_TPM)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pPropItemID", model.PROP_ITEM_ID)).Direction = ParameterDirection.Input;

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

        public async Task<string> Regression_D_TPM_Maint(int pid, int phid, int prodgroupid, int machineid, string formula, string regressind, string rectype)
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
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_REGRESSION_D_TPM_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@pID", pid)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Phid", phid)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pProdGroupID", prodgroupid)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pMachineID", machineid)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pFormula", formula)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pRegressInd", regressind)).Direction = ParameterDirection.Input;

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

        #endregion

        #region COMPOUNDER
        public async Task<string> Regression_H_COMP_Maint(RegressionModel model)
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
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_REGRESSION_H_COMP_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@pID", model.MM_REGRESSION_H_ID)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCompGroup", model.COMP_GROUP)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCompounder", model.COMPOUNDER)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pProperties", model.PROPERTIES_COMP)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pPropItemID", model.PROP_ITEM_ID)).Direction = ParameterDirection.Input;

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

        public async Task<string> Regression_D_COMP_Maint(int pid, int phid, int prodgroupid, string formula, string regressind, string rectype)
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
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_REGRESSION_D_COMP_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@pID", pid)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Phid", phid)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pProdGroupID", prodgroupid)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pFormula", formula)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pRegressInd", regressind)).Direction = ParameterDirection.Input;

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

        #endregion

    }
}