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
    public class MM_BIS_DESG_CODE_DAL : Database
    {
        public async Task<MM_BIS_DESG_CODE_VIEWMODEL> GetBisDesgCodeHeaderData(string pID)
        {
            ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
            string aclUser = userobj.USER_ID.ToString();
            string loc = System.Web.HttpContext.Current.Request.UserHostAddress.ToString();

            MM_BIS_DESG_CODE_VIEWMODEL model = new MM_BIS_DESG_CODE_VIEWMODEL();
            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_BIS_DESG_CODE_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@pID", pID)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pType", "H")).Direction = ParameterDirection.Input;

                        reader = await cmd.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            model.ID_MM_BIS_DESG_CODE_H = System.DBNull.Value == reader["ID_MM_BIS_DESG_CODE_H"] ? 0 : Convert.ToInt32(reader["ID_MM_BIS_DESG_CODE_H"]);
                            model.BIS_DESG_CAT = System.DBNull.Value == reader["BIS_DESG_CAT"] ? "" : reader["BIS_DESG_CAT"].ToString();

                            model.RECORD_TYP = System.DBNull.Value == reader["RECORD_TYP"] ? "" : reader["RECORD_TYP"].ToString();
                            model.CREATED_BY = System.DBNull.Value == reader["CREATED_BY"] ? "" : reader["CREATED_BY"].ToString();
                            model.CREATED_DATE = System.DBNull.Value == reader["CREATED_DATE"] ? DateTime.MinValue : Convert.ToDateTime(reader["CREATED_DATE"]);
                            model.CREATED_LOC = System.DBNull.Value == reader["CREATED_LOC"] ? "" : reader["CREATED_LOC"].ToString();
                            model.UPDATED_BY = System.DBNull.Value == reader["UPDATED_BY"] ? "" : reader["UPDATED_BY"].ToString();
                            model.UPDATED_DATE = System.DBNull.Value == reader["UPDATED_DATE"] ? DateTime.MinValue : Convert.ToDateTime(reader["UPDATED_DATE"]);
                            model.UPDATED_LOC = System.DBNull.Value == reader["UPDATED_LOC"] ? "" : reader["UPDATED_LOC"].ToString();
                        }
                        reader.Close();
                    }
                }                
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, aclUser);
                err = null;
                return null;
            }
            return model;
        }

        public async Task<List<MM_BIS_DESG_CODE>> GetBisDesgCodeDetailData(string pID)
        {
            ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
            string aclUser = userobj.USER_ID.ToString();
            string loc = System.Web.HttpContext.Current.Request.UserHostAddress.ToString();

            List<MM_BIS_DESG_CODE> mlist = new List<MM_BIS_DESG_CODE>();
            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_BIS_DESG_CODE_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@pID", pID)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pType", "D")).Direction = ParameterDirection.Input;

                        reader = await cmd.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            MM_BIS_DESG_CODE itemM = new MM_BIS_DESG_CODE();
                            itemM.ID_MM_BIS_DESG_CODE_D = System.DBNull.Value == reader["ID_MM_BIS_DESG_CODE_D"] ? 0 : Convert.ToInt16(reader["ID_MM_BIS_DESG_CODE_D"]);
                            itemM.ID_MM_BIS_DESG_CODE_H = System.DBNull.Value == reader["ID_MM_BIS_DESG_CODE_H"] ? 0 : Convert.ToInt16(reader["ID_MM_BIS_DESG_CODE_H"]);
                            itemM.BIS_DESG_CODE = System.DBNull.Value == reader["BIS_DESG_CODE"] ? "" : reader["BIS_DESG_CODE"].ToString();
                            itemM.BIS_MIN_RANGE = System.DBNull.Value == reader["BIS_MIN_RANGE"] ? 0 : Convert.ToDecimal(reader["BIS_MIN_RANGE"]);
                            itemM.BIS_MAX_RANGE = System.DBNull.Value == reader["BIS_MAX_RANGE"] ? 0 : Convert.ToDecimal(reader["BIS_MAX_RANGE"]);

                            itemM.RECORD_TYP = System.DBNull.Value == reader["RECORD_TYP"] ? "" : reader["RECORD_TYP"].ToString();
                            itemM.CREATED_BY = System.DBNull.Value == reader["CREATED_BY"] ? "" : reader["CREATED_BY"].ToString();
                            itemM.CREATED_DATE = System.DBNull.Value == reader["CREATED_DATE"] ? DateTime.MinValue : Convert.ToDateTime(reader["CREATED_DATE"]);
                            itemM.CREATED_LOC = System.DBNull.Value == reader["CREATED_LOC"] ? "" : reader["CREATED_LOC"].ToString();
                            itemM.UPDATED_BY = System.DBNull.Value == reader["UPDATED_BY"] ? "" : reader["UPDATED_BY"].ToString();
                            itemM.UPDATED_DATE = System.DBNull.Value == reader["UPDATED_DATE"] ? DateTime.MinValue : Convert.ToDateTime(reader["UPDATED_DATE"]);
                            itemM.UPDATED_LOC = System.DBNull.Value == reader["UPDATED_LOC"] ? "" : reader["UPDATED_LOC"].ToString();
                            mlist.Add(itemM);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, aclUser);
                err = null;
                return null;
            }
            return mlist;
        }

        public async Task<string> BisDesgCodeHeaderMaint(int pID, string pCategory, string record_typ)
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
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_BIS_DESG_CODE_H_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@pID", pID)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCategory", pCategory)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pRecType", record_typ)).Direction = ParameterDirection.Input;

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

        public async Task<string> BisDesgCodeDetailMaint(int pID, int pHID, string pCode, decimal pMin, decimal pMax, string record_typ)
        {
            string result = "0";

            ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
            string aclUser = userobj.USER_ID.ToString();
            string loc = System.Web.HttpContext.Current.Request.UserHostAddress.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_BIS_DESG_CODE_D_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@pID", pID)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pHID", pHID)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCode", pCode)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pMin", pMin)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pMax", pMax)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pRecType", record_typ)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCreatedBy", aclUser)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCreatedLoc", loc)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ReturnID", SqlDbType.Int)).Direction = ParameterDirection.Output;

                        cmd.ExecuteReader();


                        result = Convert.ToString(cmd.Parameters["@ReturnID"].Value);

                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, aclUser);
                err = null;
                return result = ex.Message.ToString();
            }
        }

    }
}