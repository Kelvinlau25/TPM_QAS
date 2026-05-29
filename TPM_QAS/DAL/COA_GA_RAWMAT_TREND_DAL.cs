using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using TPM_QAS.Helpers;
using TPM_QAS.Models;

namespace TPM_QAS.DAL
{
    public class COA_GA_RAWMAT_TREND_DAL : Database
    {
        public async Task<List<SelectListItem>> getInspItem(string suppID, string matID)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            List<SelectListItem> chop = new List<SelectListItem>();

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("PSP_GA_TREND_INSPITEM", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter("@PSuppID", suppID)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pMatID", matID)).Direction = ParameterDirection.Input;

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                chop.Add(new SelectListItem
                                {
                                    Text = reader["NAME_TEXT"] != DBNull.Value ? reader["NAME_TEXT"].ToString() : "",
                                    Value = reader["ID_VALUE"] != DBNull.Value ? reader["ID_VALUE"].ToString() : "",
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

        public async Task<DataTable> getTrendAnalysis(RawMatTrendModel model)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_GA_TREND_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pDateFrom", model.DATE_FROM)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pDateTo", model.DATE_TO)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pMat", model.MM_MATERIAL_H_ID)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pSupp", model.MM_SUPPLIER_H_ID_STRING)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pSpec", model.MM_MATERIAL_D2_ID_STRING)).Direction = ParameterDirection.Input;

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

        public async Task<DataTable> getTrendAnalysisExcel(RawMatTrendModel model)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_GA_TREND_SEL_EXCEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pDateFrom", model.DATE_FROM)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pDateTo", model.DATE_TO)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pMat", model.MM_MATERIAL_H_ID)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pSupp", model.MM_SUPPLIER_H_ID_STRING)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pSpec", model.MM_MATERIAL_D2_ID_STRING)).Direction = ParameterDirection.Input;

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