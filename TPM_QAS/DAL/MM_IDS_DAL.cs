using DocumentFormat.OpenXml.Wordprocessing;
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
    public class MM_IDS_DAL : Database
    {
        public async Task<DataTable> getIDS_MainLst_Data(string deleted = "0")
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();
            string createdby = USERID;
            string loc = HttpContext.Request.UserHostAddress.ToString();
            string pc = Environment.MachineName;

            DataTable dt = new DataTable();
            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_IDS_MAIN_LST_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pDeleted", deleted)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCreatedBy", createdby)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCreatedLoc", loc)).Direction = ParameterDirection.Input;
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

        public async Task<DataTable> getIDS_MainLst_Sel(int id, string deleted)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();
            string createdby = USERID;
            string loc = HttpContext.Request.UserHostAddress.ToString();
            string pc = Environment.MachineName;

            DataTable dt = new DataTable();
            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_IDS_MAIN_LST_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", id)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pDeleted", deleted)).Direction = ParameterDirection.Input;
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

        #region MIX SAMPLE

        public async Task<DataTable> getIDS_Mix_Data(string ID, string type)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            DataTable dt = new DataTable();
            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_IDS_SEL", con))
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

        public async Task<string> IDS_Mix_H_Maint(IDSModel model)
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
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_IDS_H_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@pID", model.IDSMixModel.MM_IDS_H_ID)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pSampType", model.IDSMixModel.SAMPLING_TYPE)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCompGroup", model.IDSMixModel.COMP_GROUP)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pProdGroupID", model.IDSMixModel.MM_PRODGROUP_ID_H)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pPropItemID", model.IDSMixModel.MM_PROPERTIES_H_ID)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pProdLine", model.IDSMixModel.PROD_LINE)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pUnit", model.IDSMixModel.UNIT)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pIndicator", model.IDSMixModel.INDICATOR)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pShrinkDown", model.IDSMixModel.SHRINK_DOWN)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pReadType", model.IDSMixModel.READING_TYPE)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pReadVal", model.IDSMixModel.READING_VALUE)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pRepeatVal", model.IDSMixModel.REPEAT_VALUE)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pHorizontal", model.IDSMixModel.HORIZONTAL)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pVertical", model.IDSMixModel.VERTICAL)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pSegregation", model.IDSMixModel.SEGREGATION)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pBfrSegr", model.IDSMixModel.BEFORESEGSET)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pAfterSegr", model.IDSMixModel.AFTERSEGSET)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCOQ", model.IDSMixModel.COQ)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pAfterCOQ", model.IDSMixModel.AFTERCOQSET)).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new SqlParameter("@pRecType", model.IDSMixModel.RECORD_TYP)).Direction = ParameterDirection.Input;
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

        public async Task<string> IDS_Mix_D_Maint(int pid, int phid, int machineID, string rectype)
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
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_IDS_D_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@pID", pid)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Phid", phid)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pMachineID", machineID)).Direction = ParameterDirection.Input;

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

        #region DIRECT SAMPLE

        public async Task<DataTable> getIDS_Direct_Data(string ID, string type)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_IDSSECTION_H_SEL", con))
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

        public async Task<string> IDS_Direct_H_Maint(IDSModel model)
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
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_IDSSECTION_H_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@pID", model.IDSDirectModel.MM_IDSSECTION_H_ID)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pSampType", model.IDSDirectModel.SAMPLING_TYPE)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCompGroup", model.IDSDirectModel.COMP_GROUP)).Direction = ParameterDirection.Input; 
                        cmd.Parameters.Add(new SqlParameter("@pSection", model.IDSDirectModel.SECTION)).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new SqlParameter("@pRecType", model.IDSDirectModel.RECORD_TYP)).Direction = ParameterDirection.Input;
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

        public async Task<string> IDS_Direct_D_Maint(int pid, int phid, string fieldname, string properties, string rectype)
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
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_IDSSECTION_D_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@pID", pid)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Phid", phid)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pFieldName", fieldname)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pProperties", properties)).Direction = ParameterDirection.Input;

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

        public async Task<DataTable> getMachine_Data(int idh, string prop, string propitem)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_MACHINE_LIST", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pIDH", idh)).Direction = ParameterDirection.Input; 
                        cmd.Parameters.Add(new SqlParameter("@prop", prop)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@propitem", propitem)).Direction = ParameterDirection.Input;
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