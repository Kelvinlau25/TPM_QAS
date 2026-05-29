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
    public class MM_BIS_GRADE_DAL : Database
    {
        public async Task<MM_BIS_GRADE_VIEWMODEL> GetBisGradeHeaderData(string pID)
        {
            ACL_UserObj userobj = HttpContextHelper.Current.Session.GetObject<ACL_UserObj>("AclUser");
            string aclUser = userobj.USER_ID.ToString();
            string loc = HttpContextHelper.Current.Connection.RemoteIpAddress?.ToString().ToString();

            MM_BIS_GRADE_VIEWMODEL model = new MM_BIS_GRADE_VIEWMODEL();
            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_BIS_GRADE_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@pID", pID)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pType", "H")).Direction = ParameterDirection.Input;

                        reader = await cmd.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            model.ID_MM_BIS_GRADE_H = System.DBNull.Value == reader["ID_MM_BIS_GRADE_H"] ? 0 : Convert.ToInt32(reader["ID_MM_BIS_GRADE_H"]);
                            model.BIS_GRADE_CODE = System.DBNull.Value == reader["BIS_GRADE_CODE"] ? "" : reader["BIS_GRADE_CODE"].ToString();

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

        public async Task<List<MM_BIS_GRADE>> GetBisGradeDetailData(string pID)
        {
            ACL_UserObj userobj = HttpContextHelper.Current.Session.GetObject<ACL_UserObj>("AclUser");
            string aclUser = userobj.USER_ID.ToString();
            string loc = HttpContextHelper.Current.Connection.RemoteIpAddress?.ToString().ToString();

            List<MM_BIS_GRADE> mlist = new List<MM_BIS_GRADE>();
            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_BIS_GRADE_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@pID", pID)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pType", "D")).Direction = ParameterDirection.Input;

                        reader = await cmd.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            MM_BIS_GRADE itemM = new MM_BIS_GRADE();
                            itemM.ID_MM_BIS_GRADE_D = System.DBNull.Value == reader["ID_MM_BIS_GRADE_D"] ? 0 : Convert.ToInt16(reader["ID_MM_BIS_GRADE_D"]);
                            itemM.ID_MM_BIS_GRADE_H = System.DBNull.Value == reader["ID_MM_BIS_GRADE_H"] ? 0 : Convert.ToInt16(reader["ID_MM_BIS_GRADE_H"]);
                            itemM.BIS_GRADE_PROD_CODE = System.DBNull.Value == reader["BIS_GRADE_PROD_CODE"] ? "" : reader["BIS_GRADE_PROD_CODE"].ToString();
                            itemM.BIS_SINGLE_DESIGN_CODE = System.DBNull.Value == reader["BIS_SINGLE_DESIGN_CODE"] ? "" : reader["BIS_SINGLE_DESIGN_CODE"].ToString();
                            itemM.RECORD_TYP = System.DBNull.Value == reader["RECORD_TYP"] ? "" : reader["RECORD_TYP"].ToString();
                            itemM.CREATED_BY = System.DBNull.Value == reader["CREATED_BY"] ? "" : reader["CREATED_BY"].ToString();
                            itemM.CREATED_DATE = System.DBNull.Value == reader["CREATED_DATE"] ? DateTime.MinValue : Convert.ToDateTime(reader["CREATED_DATE"]);
                            itemM.CREATED_LOC = System.DBNull.Value == reader["CREATED_LOC"] ? "" : reader["CREATED_LOC"].ToString();
                            itemM.UPDATED_BY = System.DBNull.Value == reader["UPDATED_BY"] ? "" : reader["UPDATED_BY"].ToString();
                            itemM.UPDATED_DATE = System.DBNull.Value == reader["UPDATED_DATE"] ? DateTime.MinValue : Convert.ToDateTime(reader["UPDATED_DATE"]);
                            itemM.UPDATED_LOC = System.DBNull.Value == reader["UPDATED_LOC"] ? "" : reader["UPDATED_LOC"].ToString();
                            mlist.Add(itemM);
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
            return mlist;
        }

        public async Task<string> BisGradeHeaderMaint(int pID, string pDSCode, string record_typ)
        {
            string result = "0";

            ACL_UserObj userobj = HttpContextHelper.Current.Session.GetObject<ACL_UserObj>("AclUser");
            string aclUser = userobj.USER_ID.ToString();
            string loc = HttpContextHelper.Current.Connection.RemoteIpAddress?.ToString().ToString();

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_BIS_GRADE_H_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@pID", pID)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pDSCode", pDSCode)).Direction = ParameterDirection.Input;
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

        public async Task<string> BisGradeDetailMaint(int pID, int pHID, string pCode, string pSingle, string record_typ)
        {
            string result = "0";

            ACL_UserObj userobj = HttpContextHelper.Current.Session.GetObject<ACL_UserObj>("AclUser");
            string aclUser = userobj.USER_ID.ToString();
            string loc = HttpContextHelper.Current.Connection.RemoteIpAddress?.ToString().ToString();

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_BIS_GRADE_D_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@pID", pID)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pHID", pHID)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCode", pCode)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pSingle", pSingle)).Direction = ParameterDirection.Input;
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