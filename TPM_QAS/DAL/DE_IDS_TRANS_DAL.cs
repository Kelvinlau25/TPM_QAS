using Dapper;
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
    public class DE_IDS_TRANS_DAL : Database
    {
        public async Task<List<DailyIDSTransVM>> getProdTypeList() //Simulation By 
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            List<DailyIDSTransVM> chop = new List<DailyIDSTransVM>();

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_PRODTYPE_LST", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                chop.Add(new DailyIDSTransVM
                                {
                                    //ID_MM_MAINTRANS = Convert.ToInt32(reader["ID_MM_MAINTRANS"].ToString()),
                                    PRODTYPE = reader["ITEM_NAME"].ToString(),
                                    LOTNO = reader["LOT_NUMBER"].ToString(),
                                    PACKEDDATE = reader["PACKED_DATE"].ToString(),
                                    PACKEDDATEDATE = Convert.ToDateTime(reader["PDD"]),
                                    //QUANTITY = reader["QUANTITY"].ToString(),
                                    QUANTITY = Convert.ToInt32(reader["PACK_QTY"].ToString())
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
                return null;
            }

            return chop;
        }

        public async Task<DailyIDSTransVM>  dataByProdtype(string itemname, string lotno)  //view function
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_PRODTYPE_SEARCHBY", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pItemName", itemname)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pLotNo", lotno)).Direction = ParameterDirection.Input;

                        DailyIDSTransVM ProcCodeModel = null;

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                ProcCodeModel = new DailyIDSTransVM();
                                ProcCodeModel.PRODTYPE = reader["ITEM_NAME"].ToString();
                                ProcCodeModel.LOTNO = reader["LOT_NUMBER"].ToString();
                                ProcCodeModel.PACKEDDATE2 = reader["PACKED_DATE"].ToString();
                                ProcCodeModel.QUANTITY = Convert.ToInt32(reader["PACK_QTY"].ToString());
                                ProcCodeModel.prodline = reader["PROD_LINE"].ToString();
                                ProcCodeModel.BBS = reader["BBS"].ToString(); //blank black speck
                                ProcCodeModel.BYI = reader["BYI"].ToString(); //blank YI
                                ProcCodeModel.REPACKLOT = reader["REPACKLOT"].ToString(); //blank YI
                            }
                        }
                        return ProcCodeModel;
                    }
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

        public async Task<List<MoldingModel>> GetDataMolding(string prodtype, string paction, string lotno)
        {
            string spec = "";

            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            List<MoldingModel> chop = new List<MoldingModel>();
            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("PSP_E_IDS_SEL_KL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", prodtype)).Direction = ParameterDirection.Input; //OLD SP
                        cmd.Parameters.Add(new SqlParameter("@plotno", lotno)).Direction = ParameterDirection.Input; //OLD SP
                        cmd.Parameters.Add(new SqlParameter("@PPACKEDDATE", paction)).Direction = ParameterDirection.Input; //OLD SP

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                spec = reader["SPECIFICATION"].ToString();
                                if (spec.Length > 3 && spec.Substring(0, 3) == " , ")
                                {
                                    spec = spec.Substring(2);

                                }

                                chop.Add(new MoldingModel
                                {
                                    prodtype = reader["PROPTYPEMAIN"].ToString(), //old SP
                                    Properties = reader["PROPERTIES"].ToString(),
                                    PropItem = reader["PROPITEM"].ToString(), //old SP
                                    Unit = reader["UNIT"].ToString(),
                                    TypeInd = reader["TYPEIND"].ToString(),
                                    RegressionResult = reader["REGRESSIONRESULT"].ToString(),
                                    Average = reader["AVERAGE"].ToString(),
                                    COQAdj = reader["COQAdj"].ToString(),
                                    Grade = reader["GRADE"].ToString(),
                                    PCL_Result = reader["PCL_RESULT"].ToString(),
                                    MachineName = reader["machinename"].ToString(), //old SP
                                    DataChecking = reader["DATACHECKING"].ToString(),
                                    COQInd = reader["COQind"].ToString(),
                                    ProdGroup = reader["PRODGROUP"].ToString(),
                                    MainReading = reader["MAINREADING"].ToString(),
                                    StatusInd = reader["StatusInd"].ToString(),
                                    RegressFormula = reader["REGRESSFORMULA"].ToString(), //add by kl ong on 2022-03-04
                                    RegressInd = reader["REGRESSIND"].ToString(), //add by kl ong on 2022-03-04
                                    Count = Convert.ToInt32(reader["COUNT"].ToString()), //new SP COUNTS, old SP COUNT
                                    Specification = spec,
                                    PCL_Spec = reader["PCL_SPEC"].ToString(),
                                    lotno = lotno
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

        public async Task<List<FirstBagModel>> GetFirstbagList(string lotno, string pind)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            List<FirstBagModel> FBag = new List<FirstBagModel>();
            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_FIRSTBAG_YIMFR", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@PLOTNO", lotno)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PIND", pind)).Direction = ParameterDirection.Input;

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                FBag.Add(new FirstBagModel
                                {
                                    ID = Convert.ToInt32(reader["ID"]),
                                    TONNAGE = reader["TONNAGE"].ToString(),
                                    TAGNO = reader["TAGNO"].ToString(),
                                    GRADE = reader["GRADE"].ToString(),
                                    PROPERTIES = reader["PROPERTIES"].ToString(),
                                    READING = reader["READING"].ToString()
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

            return FBag;
        }
        public async Task<List<FirstBagModel>> GetFirstbagListArc(string lotno, string pind)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            List<FirstBagModel> FBag = new List<FirstBagModel>();
            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_FIRSTBAG_YIMFR_ARC", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@PLOTNO", lotno)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PIND", pind)).Direction = ParameterDirection.Input;

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                FBag.Add(new FirstBagModel
                                {
                                    ID = Convert.ToInt32(reader["ID"]),
                                    TONNAGE = reader["TONNAGE"].ToString(),
                                    TAGNO = reader["TAGNO"].ToString(),
                                    GRADE = reader["GRADE"].ToString(),
                                    PROPERTIES = reader["PROPERTIES"].ToString(),
                                    READING = reader["READING"].ToString()
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

            return FBag;
        }

        public async Task<List<PropGradeSpecModel>> GetPropGradeSpec(string lotno, string pind)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            List<PropGradeSpecModel> PropSpec = new List<PropGradeSpecModel>();
            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_PROP_GRADE_SPEC", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@PLOTNO", lotno)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PIND", pind)).Direction = ParameterDirection.Input;

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                PropSpec.Add(new PropGradeSpecModel
                                {
                                    PROPERTIES = reader["PROPERTIES"].ToString(),
                                    PROP_ITEM = reader["PROP_ITEM"].ToString(),
                                    L_SPEC = reader["L_SPEC"].ToString(),
                                    U_SPEC = reader["U_SPEC"].ToString(),
                                    GRADE = reader["GRADE"].ToString(),
                                    PRIORITY = reader["PRIORITY"].ToString(),
                                    FINAL_PRIORITY = reader["FINAL_PRIORITY"].ToString(),
                                    ROUNDING = reader["ROUNDING"].ToString(),                                 
                                    GRADINGIND = reader["GRADINGIND"].ToString()
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

            return PropSpec;
        }
        public async Task<List<PropGradeSpecModel>> GetPropGradeSpecArc(string lotno, string pind)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            List<PropGradeSpecModel> PropSpec = new List<PropGradeSpecModel>();
            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_PROP_GRADE_SPEC_ARC", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@PLOTNO", lotno)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PIND", pind)).Direction = ParameterDirection.Input;

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                PropSpec.Add(new PropGradeSpecModel
                                {
                                    PROPERTIES = reader["PROPERTIES"].ToString(),
                                    PROP_ITEM = reader["PROP_ITEM"].ToString(),
                                    L_SPEC = reader["L_SPEC"].ToString(),
                                    U_SPEC = reader["U_SPEC"].ToString(),
                                    GRADE = reader["GRADE"].ToString(),
                                    PRIORITY = reader["PRIORITY"].ToString(),
                                    FINAL_PRIORITY = reader["FINAL_PRIORITY"].ToString(),
                                    ROUNDING = reader["ROUNDING"].ToString(),
                                    GRADINGIND = reader["GRADINGIND"].ToString()
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

            return PropSpec;
        }

        public async Task<DataTable>CheckUserAppRole(string empno)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_GET_USER_APP_LEVEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@PempNo", empno)).Direction = ParameterDirection.Input;
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

        public async Task<string> deleteIDSTrans(int idh)
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
                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_DELETE_LOTNO", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", idh)).Direction = ParameterDirection.Input;
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

        public async Task<DataTable>DataChk(string prod, string avg, string ind, string prop, string plotno)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_D_GRADE_CHK", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@PPRODTYPE", prod)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PAVERAGE", avg)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PIND", ind)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PPROP", prop)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PLOTNO", plotno)).Direction = ParameterDirection.Input;

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

        public async Task<DataTable>GetDataDLLALL(string id, string text, string text2, string text3, string text4, string text5, string text6)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_ALLDDL_SEL2", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", id)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ptext", text)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ptext2", text2)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ptext3", text3)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ptext4", text4)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ptext5", text5)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ptext6", text6)).Direction = ParameterDirection.Input;
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

        public async Task<List<tagnomodel>> gettagno(string id, string text, string text2, string text3,
            string text4, string text5, string text6) //FOR LISTING FUNCTION
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            List<tagnomodel> exchrate = new List<tagnomodel>();

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("PSP_ALLDDL_SEL2", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", id)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ptext", text)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ptext2", text2)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ptext3", text3)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ptext4", text4)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ptext5", text5)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ptext6", text6)).Direction = ParameterDirection.Input;

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                exchrate.Add(new tagnomodel
                                {
                                    res2 = reader["res2"].ToString(),
                                    res4 = reader["res4"].ToString(),
                                    res5 = reader["res5"].ToString(),
                                    res7 = reader["res7"].ToString(),
                                    res3 = reader["res3"].ToString(),
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

            return exchrate;
        }

        public async Task<List<fieldnamemodel>> getfielname(string id, string text, string text2, string text3,
            string text4, string text5, string text6) //FOR LISTING FUNCTION
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            List<fieldnamemodel> exchrate = new List<fieldnamemodel>();

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("PSP_ALLDDL_SEL2", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", id)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ptext", text)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ptext2", text2)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ptext3", text3)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ptext4", text4)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ptext5", text5)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ptext6", text6)).Direction = ParameterDirection.Input;

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                exchrate.Add(new fieldnamemodel
                                {
                                    fieldname = reader["fieldname"].ToString().Trim(),
                                    calcratio = reader["calcratio"].ToString()

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

            return exchrate;
        }

        public async Task<DataTable>DataChk56(string pdate, string pgroup, string pprodtype, string pprodline)
        {

            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();
            
            try
            {
                string result = "";
                string return_value = "0";

                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    //using (SqlCommand cmd = new SqlCommand("PSP_E_IDS_CHK", con))
                    using (SqlCommand cmd = new SqlCommand("PSP_E_IDS_MANDATORY", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@PDATE", pdate)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pProdgroup", pgroup)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pProdtype", pprodtype)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pProdline", pprodline)).Direction = ParameterDirection.Input;
                        reader = await cmd.ExecuteReaderAsync();
                        dt.Load(reader);
                        reader.Close();

                        result = return_value;
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

        public async Task<DataTable> DataChkMand(string pdate, string pgroup, string pprodtype, string pprodline)
        {

            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string result = "";
                string return_value = "0";

                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_E_IDS_MANDATORY", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@PDATE", pdate)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pProdgroup", pgroup)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pProdtype", pprodtype)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pProdline", pprodline)).Direction = ParameterDirection.Input;
                        reader = await cmd.ExecuteReaderAsync();
                        dt.Load(reader);
                        reader.Close();

                        result = return_value;
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

        public async Task<DataTable>GetData1(string id, string paction, string plotno, string pdate)
        {
            string result = "";
            string return_value = "0";

            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_E_IDS_SEL_DRAFT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", id)).Direction = ParameterDirection.Input;
                        if (paction == "1")
                        {
                            cmd.Parameters.Add(new SqlParameter("@plotno", plotno)).Direction = ParameterDirection.Input;
                            cmd.Parameters.Add(new SqlParameter("@PPACKEDDATE", pdate)).Direction = ParameterDirection.Input;
                        }

                        reader = await cmd.ExecuteReaderAsync();
                        dt.Load(reader);
                        reader.Close();

                        result = return_value;

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

        public async Task<DataTable>getYiFormula(string lotno)
        {
            string result = "";
            string return_value = "0";

            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_REGRESSION_SEL2", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", lotno)).Direction = ParameterDirection.Input;
                        reader = await cmd.ExecuteReaderAsync();
                        dt.Load(reader);
                        reader.Close();

                        result = return_value;

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

        public async Task<List<MachineNameModel>> getMachineName(string lotno, string propitem, string prodgroup) //Simulation By 
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            List<MachineNameModel> chop = new List<MachineNameModel>();

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("PSP_PP_MACHINE_DATA", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@lotno", lotno)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@propitem", propitem)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PRODGROUP", prodgroup)).Direction = ParameterDirection.Input;

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                chop.Add(new MachineNameModel
                                {

                                    MachineName = reader["MACHINENAME"].ToString(),
                                    Result = reader["result"].ToString(),
                                    UpdateDate = reader["UPDATED_DATE"].ToString(),
                                    ProdGroup = reader["PRODGROUP"].ToString(),

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

        public async Task<DataTable>GetMachineData(string lotNo)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_MM_MACHINE_TRANS_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@PLOTNO", lotNo)).Direction = ParameterDirection.Input;
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

        public async Task<DataTable>GetReg(string machinename, string propitem, string prodgroup, string lotno)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_GET_REGRESSRESULT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@Pmachinename", machinename)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PPROPITEM", propitem)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pprodgroup", prodgroup)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@plotno", lotno)).Direction = ParameterDirection.Input;
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

        public async Task<DailyIDSTransVM> getMoldDetail(string lotno, string propitem, string prodtype)  //view function
        {

            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            DailyIDSTransVM ProcCodeModel = null;

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("PSP_E_IDS_SEL_2", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", prodtype)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@plotno", lotno)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@propItem", propitem)).Direction = ParameterDirection.Input;

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                ProcCodeModel = new DailyIDSTransVM();
                                ProcCodeModel.id_ids_d = Convert.ToInt32(reader["ID_IDS_D"]);
                                ProcCodeModel.typeinds = reader["typeind"].ToString();
                                ProcCodeModel.mainreading = reader["MAINREADING"].ToString();
                                ProcCodeModel.maxcount = Convert.ToInt32(reader["count"]);
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
                return null;
            }

            return ProcCodeModel;
        }

        public async Task<DailyIDSTransVM> getPropItemDet(string lotno, string propitem, string prodtype)  //view function
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            DailyIDSTransVM ProcCodeModel = null;

            try
            {
                string constr = await GetConnectionStringAsync();
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_E_IDS_PROPITEM_DET", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", prodtype)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@plotno", lotno)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@propItem", propitem)).Direction = ParameterDirection.Input;

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                ProcCodeModel = new DailyIDSTransVM();
                                ProcCodeModel.id_ids_d = Convert.ToInt32(reader["ID_IDS_D"]);
                                ProcCodeModel.typeinds = reader["TYPEIND"].ToString();
                                ProcCodeModel.mainreading = reader["MAINREADING"].ToString();
                                ProcCodeModel.maxcount = Convert.ToInt32(reader["MAXCOUNT"]);
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
                return null;
            }

            return ProcCodeModel;
        }

        public async Task<List<MoldingModel>> GetData(string prodtype, string paction, string lotno)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            List<MoldingModel> chop = new List<MoldingModel>();

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("PSP_E_IDS_SEL_KL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", prodtype)).Direction = ParameterDirection.Input; //OLD SP
                        cmd.Parameters.Add(new SqlParameter("@plotno", lotno)).Direction = ParameterDirection.Input; //OLD SP
                        cmd.Parameters.Add(new SqlParameter("@PPACKEDDATE", paction)).Direction = ParameterDirection.Input; //OLD SP

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                chop.Add(new MoldingModel
                                {
                                    //ID_IDS_D = Convert.ToInt32(reader["ID_IDS_D"].ToString()),
                                    //PropTypeMain = reader["PRODTYPE"].ToString(), //new SP
                                    prodtype = reader["PROPTYPEMAIN"].ToString(), //old SP
                                    Properties = reader["PROPERTIES"].ToString(),
                                    //PropItem = reader["MAIN_PROPERTIES"].ToString(), ///new SP
                                    PropItem = reader["PROPITEM"].ToString(), //old SP
                                    Unit = reader["UNIT"].ToString(),
                                    TypeInd = reader["TYPEIND"].ToString(),
                                    RegressionResult = reader["REGRESSIONRESULT"].ToString(),
                                    Average = reader["AVERAGE"].ToString(),
                                    COQAdj = reader["COQAdj"].ToString(),
                                    Grade = reader["GRADE"].ToString(),
                                    //MachineName = reader["m/c"].ToString(), //new SP
                                    MachineName = reader["machinename"].ToString(), //old SP
                                    DataChecking = reader["DATACHECKING"].ToString(),
                                    COQInd = reader["COQind"].ToString(),
                                    ProdGroup = reader["PRODGROUP"].ToString(),
                                    MainReading = reader["MAINREADING"].ToString(),
                                    StatusInd = reader["StatusInd"].ToString(),
                                    RegressFormula = reader["REGRESSFORMULA"].ToString(), //add by kl ong on 2022-03-04
                                    RegressInd = reader["REGRESSIND"].ToString(), //add by kl ong on 2022-03-04
                                    Count = Convert.ToInt32(reader["COUNT"].ToString()), //new SP COUNTS, old SP COUNT
                                    Specification = reader["SPECIFICATION"].ToString(),
                                    lotno = lotno
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

        public async Task<string> lockCheck(DailyIDSTransVM m, string type, string id)
        {
            string result = "";
            string return_value = "0";

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
                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_DEADLOCK_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@PID", id)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PPRODTYPE", m.PRODTYPE)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PPACKEDDATE", m.PACKEDDATE2)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PLOTNO", m.LOTNO)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PQUANTITY", m.QUANTITY)).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new SqlParameter("@pCreatedBy", m.CREATED_BY)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCreatedLoc", m.CREATED_LOC)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pType", type)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ReturnID", SqlDbType.VarChar, 10)).Direction = ParameterDirection.Output;

                        reader = await cmd.ExecuteReaderAsync();
                        dt.Load(reader);
                        reader.Close();

                        return_value = cmd.Parameters["@ReturnID"].Value.ToString();
                        if (type == "1" && return_value == "NG")
                        {
                            result = "Other transaction is in progress, please retry in 30 second.";
                            return result;
                        }
                        result = dt.Rows.Count > 0 ? dt.Rows[0]["LOCKFLAG"].ToString() : "0";
                        return result + "-" + return_value;

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

        public async Task<string> IDS_H_Maint(DailyIDSTransVM m, string rectype)  //view function
        {
            string result = "";
            string return_value = "0";

            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();
            string createdby = USERID;
            string loc = HttpContext.Request.UserHostAddress.ToString();
            string pc = Environment.MachineName;

            try
            {
                string cdate = "";
                if (m.CREATED_DATE != null) cdate = m.CREATED_DATE.ToString();

                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_H_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID_IDS_H", m.ID_IDS_H)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PFullNg", m.FullNG)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PPRODTYPE", m.PRODTYPE)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PPACKEDDATE", m.PACKEDDATE)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PLOTNO", m.LOTNO)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PQUANTITY", m.QUANTITY)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PTESTEDBY", m.TESTEDBY)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PUPDATEDBY", m.TESTEDBY)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PSTATUS", m.STATUS == null ? "" : m.STATUS)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PGRADE", m.GRADE == null ? "" : m.GRADE)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PPRODTYPCHGIND", m.PRODTYPCHGIND)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PAFTERPRODTYPCHG", m.AFTERPRODTYPCHG)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PPACKEDDATE2", m.PACKEDDATE2)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PFLOT", m.FLOT)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PMABS_P", m.MABS_P)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PCROSS_LINE", m.CROSS_LINE)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PREJECT", m.REJECT)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PCREATEDDATE", cdate)).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new SqlParameter("@PIDIDSDAPP", m.ID_IDS_D_APP)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PREPORTBY", m.REPORT_BY)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PAPPRMOLDNAME", m.APPR_MOLD_NAME)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PAPPRAPPEANAME", m.APPR_APPEA_NAME)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PAPPRCHKBYNAME", m.APPR_CHKBY_NAME)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PAPPRVERFBYNAME", m.APPR_VERFBY_NAME)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PAPPRKEYEDBYNAME", m.APPR_KEYBY_NAME)).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new SqlParameter("@PAPPMOLDDATE", m.MOLD_DATE)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PAPPAPPEADATE", m.APPEARANCE_DATE)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PAPPCHKBYDATE", m.CHECKED_BY_DATE)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PAPPVERFBYDATE", m.VERIFIED_BY_DATE)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PAPPKEYBYDATE", m.KEYED_BY_DATE)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PLOTSTATUS", m.LOT_STATUS)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PSILONO", m.SILO_NO)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PONLINEMFR", m.ONLINE_MFR)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PONLINEYI", m.ONLINE_YI)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PONLINEYPBP", m.ONLINE_YPBP)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PTWINPALLET", m.TWIN_PELLET)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PFISHEYELEVEL", m.FISHEYE_LEVEL)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PBSSPOT1", m.BS_SPOT_1)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PBSSPOT2", m.BS_SPOT_2)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PBSSPOT3", m.BS_SPOT_3)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PPRODREMARK", m.PRODUCTION_REMARK)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PQAREMARK", m.QA_REMARK)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@POWGRADE", m.OVERWRITE_GRADE)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PBSMACH", m.BS_MACH)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PCOQADJRMK", m.COQ_ADJUST_RMK)).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new SqlParameter("@pRecType", rectype)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCreatedBy", m.CREATED_BY)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCreatedLoc", m.CREATED_LOC)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ReturnID", SqlDbType.Int)).Direction = ParameterDirection.Output;
                        cmd.ExecuteReader();


                        return_value = Convert.ToString(cmd.Parameters["@ReturnID"].Value);

                        result = return_value;

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

        public async Task<string> IDS_D_MAINT(int pid, string hid, string properties, string propitem, string unit, string typeind, string mandatory, string type, string machinename, string regressionind, string formula,
                                    string coqind, string tagnofrom, string tagnoto, string reading1, string reading2, string reading3, string reading4, string reading5, string reading6, string average,
                                    string regressionresult, string coqadj, string time, string temp, string PRHPERC, string grade, string pcl_result, string rectype, string createdby, string createdloc)
        {
            string result = "";
            string return_value = "0";

            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();
            string loc = HttpContext.Request.UserHostAddress.ToString();
            string pc = Environment.MachineName;

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_D_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@PID_IDS_D", pid)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pID_IDS_H", hid)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PPROPERTIES", properties)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PPROPITEM", propitem)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PUNIT", unit == null || unit == "" ? "" : unit)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PTYPEIND", typeind)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PMANDATORY", mandatory)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PTYPE", type)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PMACHINENAME", machinename)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PREGRESSIONIND", regressionind)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PREGRESSIONFORMULA", formula)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PCOQIND", coqind)).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new SqlParameter("@PTAGNOFROM", tagnofrom)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PTAGNOTO", tagnoto)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PREADING1", reading1)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PREADING2", reading2)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PREADING3", reading3)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PREADING4", reading4)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PREADING5", reading5)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PREADING6", reading6)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PAVERAGE", average)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PREGRESSIONRESULT", regressionresult)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PCOQADJ", coqadj)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PTIME", time)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PTEMP", temp)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PRHPERC", PRHPERC)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PGRADE", grade)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PPCL_RESULT", pcl_result)).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new SqlParameter("@pRecType", rectype)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCreatedBy", createdby)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCreatedLoc", createdloc)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ReturnID", SqlDbType.Int)).Direction = ParameterDirection.Output;
                        cmd.ExecuteReader();


                        return_value = Convert.ToString(cmd.Parameters["@ReturnID"].Value);

                        result = return_value;

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

        public async Task<DataTable>GetDataTable(string idH, string paction, string lotno, string pdate, string pgrp)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            string psp = "";
            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                if (paction == "1") { psp = "PSP_E_IDS_SEL"; }
                else if (paction == "2") { psp = "PSP_E_IDS_EDIT_SEL"; }
                else if (paction == "3") { psp = "PSP_IDS_SL_EDIT_SEL"; }
                else if (paction == "4") { psp = "PSP_IDS_DL_EDIT_SEL"; }
                else if (paction == "5") { psp = "PSP_E_IDSA_EDIT_SEL"; }
                else if (paction == "6") { psp = "PSP_E_IDS_ALL_SEL"; }
                else if (paction == "7") { psp = "PSP_E_IDS_SUM_GRD_SEL"; }
                else if (paction == "8") { psp = "PSP_E_IDS_SUM_GRD_SEL3"; }
                else if (paction == "9") { psp = "PSP_E_IDS_SUM_GRD_SEL2"; }


                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(psp, con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", idH)).Direction = ParameterDirection.Input;
                        if (paction == "1")
                        {
                            cmd.Parameters.Add(new SqlParameter("@plotno", lotno)).Direction = ParameterDirection.Input;
                            cmd.Parameters.Add(new SqlParameter("@PPACKEDDATE", pdate)).Direction = ParameterDirection.Input;
                        }
                        else if (paction == "6")
                        {
                            cmd.Parameters.Add(new SqlParameter("@plotno", lotno)).Direction = ParameterDirection.Input;
                            cmd.Parameters.Add(new SqlParameter("@PPACKEDDATE", pdate)).Direction = ParameterDirection.Input;
                            cmd.Parameters.Add(new SqlParameter("@pprodgroup", pgrp)).Direction = ParameterDirection.Input;
                        }

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

        public async Task<DataTable> GetFinalPriority(string idH, string gradingInd)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_GET_FINAL_PRIORIY", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", idH)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pGradingInd", gradingInd)).Direction = ParameterDirection.Input;
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

        public async Task<DataTable> GetMoldSegre(string idH)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_GET_MOLD_SEGREGATION", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", idH)).Direction = ParameterDirection.Input;
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

        public async Task<DataTable>Get1stYI(string pID)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_GET_FIRSTYI", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", pID)).Direction = ParameterDirection.Input;
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

        public async Task<DataTable>GetCAPCE(string prodline)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_GET_CAPCE", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@PPRODLINE", prodline)).Direction = ParameterDirection.Input;
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

        public async Task<string> IDS_SUM_Maint(SummaryModel m)  //view function
        {
            string result = "";

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
                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_SUM_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID_IDS_H", m.ID_IDS_H)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PGRADE", m.GRADE)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PTAGNOFROM", m.TAGNOFROM)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PTAGNOTO", m.TAGNOTO)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PTONFROM", m.TONNAGEFROM)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PTONTO", m.TONNAGETO)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PABNORMALITIES", m.ABNORMALITIES)).Direction = ParameterDirection.Input;
                        cmd.ExecuteReader();


                        return result;

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

        public async Task<string> IDS_COMPLETE_FRACTION(string pididsh)  //view function
        {
            string result = "";

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
                    using (SqlCommand cmd = new SqlCommand("PSP_SPLITLOT_FRACTION", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter("@pID", pididsh)).Direction = ParameterDirection.Input;
                        cmd.ExecuteReader();


                        return result;

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

        public async Task<DataTable>IDS_SUM_SEL(string idsh)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_SUM_SEL2", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", idsh)).Direction = ParameterDirection.Input;
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

        public async Task<DataTable> IDS_SUM_SEL_RPT(string idsh)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_SUM_RPT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", idsh)).Direction = ParameterDirection.Input;
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

        public async Task<string> IDS_SL_MAINT(string idssl, string idD, string reading1, string reading2, string reading3, string reading4, string reading5, string reading6, string average, string grade
                                , string rectype, string createdby, string createdloc)
        {
            string result = "";
            string return_value = "0";

            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();
            string loc = HttpContext.Request.UserHostAddress.ToString();
            string pc = Environment.MachineName;

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_SL_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID_IDS_SL", idssl)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PID_IDS_D", idD)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PREADING1", reading1)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PREADING2", reading2)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PREADING3", reading3)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PREADING4", reading4)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PREADING5", reading5)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PREADING6", reading6)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PAVERAGE", average)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PTESTEDDATETIME", "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PUPDATEDBY", createdby)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PGRADE", grade)).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new SqlParameter("@pRecType", rectype)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCreatedBy", createdby)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCreatedLoc", createdloc)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ReturnID", SqlDbType.Int)).Direction = ParameterDirection.Output;
                        cmd.ExecuteReader();


                        return_value = Convert.ToString(cmd.Parameters["@ReturnID"].Value);

                        result = return_value;

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


        public async Task<string> IDS_DL_MAINT(string idDL, string idD, string tagfrom, string tagto, string grade, string rectype, string createdby, string createdloc)
        {
            string result = "";
            string return_value = "0";

            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();
            string loc = HttpContext.Request.UserHostAddress.ToString();
            string pc = Environment.MachineName;

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_DL_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID_IDS_DL", idDL)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PID_IDS_D", idD)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PTAGNOFROM", tagfrom)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PTAGNOTO", tagto)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Pgrade", grade)).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new SqlParameter("@pRecType", rectype)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCreatedBy", createdby)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCreatedLoc", createdloc)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ReturnID", SqlDbType.Int)).Direction = ParameterDirection.Output;
                        cmd.ExecuteReader();


                        return_value = Convert.ToString(cmd.Parameters["@ReturnID"].Value);

                        result = return_value;

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

        public async Task<string> IDS_DLL_MAINT(string idDLL, string idDL, string col, string reading, string grade, string properties, string ratio, string beforeregress, string regressformula, string rectype, string createdby, string createdloc)
        {
            string result = "";
            string return_value = "0";

            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();
            string loc = HttpContext.Request.UserHostAddress.ToString();
            string pc = Environment.MachineName;

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_DLL_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID_IDS_DLL", idDLL)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pID_IDS_DL", idDL)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PCol", col)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Preading", reading)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Pgrade", grade)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PPROP", properties)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Pratio", ratio)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Pbeforeregress", beforeregress)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Pregressformula", regressformula)).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new SqlParameter("@pRecType", rectype)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCreatedBy", createdby)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCreatedLoc", createdloc)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ReturnID", SqlDbType.Int)).Direction = ParameterDirection.Output;
                        cmd.ExecuteReader();


                        return_value = Convert.ToString(cmd.Parameters["@ReturnID"].Value);

                        result = return_value;

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

        public async Task<DataTable>HowManyDataChk(string properties, string propitem, string prodtype, string prodline)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_TEST_DATA_CHK", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@PPROPERTIES", properties)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PPROPITEM", propitem)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PPRODTYPE", prodtype)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PPRODLINENO", prodline)).Direction = ParameterDirection.Input;

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

        public async Task<DailyIDSTransVM> getSummaryData(string id)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            DailyIDSTransVM data = new DailyIDSTransVM();
            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("PSP_E_IDS_EDIT_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", id)).Direction = ParameterDirection.Input;

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                data = new DailyIDSTransVM
                                {
                                    ID_IDS_H = Convert.ToInt32(reader["ID_IDS_H"].ToString()),
                                    PRODTYPE = reader["PRODTYPE"].ToString() ?? "",
                                    LOTNO = reader["LOTNO"].ToString() ?? "",
                                    FullNG = reader["FullNG"].ToString() ?? "",
                                    PACKEDDATE = reader["PACKEDDATE"].ToString() ?? "",
                                    PACKEDDATE2 = reader["PACKEDDATE2"].ToString() ?? "",
                                    QUANTITY = Convert.ToInt32(reader["QUANTITY"].ToString()),
                                    TESTEDBY = reader["TESTEDBY"].ToString() ?? "",
                                    UPDATEDBY = reader["UPDATEDBY"].ToString() ?? "",
                                    STATUS = reader["STATUS"].ToString() ?? "",
                                    GRADE = reader["H_GRADE"].ToString() ?? "",
                                    OVERWRITE_GRADE = reader["OVERWRITE_GRADE"].ToString() ?? "",
                                    BS_MACH = reader["BS_MACH"].ToString() ?? "",
                                    MABS_P = reader["MABS_P"].ToString() == "1" ? true : false,
                                    FLOT = reader["FLOT"].ToString() == "1" ? true : false,
                                    CROSS_LINE = reader["CROSS_LINE"].ToString() == "1" ? true : false,
                                    REJECT = reader["REJECT"].ToString() == "1" ? true : false,
                                    prodline = reader["prodline"].ToString() ?? "", // nabila 16/03/2022
                                    prodgroup = reader["prodgroup"].ToString() ?? "",  // nabila 16/03/2022
                                    CREATED_DATE = Convert.ToDateTime(reader["CREATED_DATE"]),  // LiewKarWei 09/02/2023

                                    REPORT_BY = reader["REPORT_BY"].ToString(),
                                    APPR_APPEA_NAME = reader["APPR_APPEA_NAME"].ToString(),
                                    APPR_MOLD_NAME = reader["APPR_MOLD_NAME"].ToString(),
                                    APPR_CHKBY_NAME = reader["APPR_CHKBY_NAME"].ToString(),
                                    APPR_VERFBY_NAME = reader["APPR_VERFBY_NAME"].ToString(),
                                    APPR_KEYBY_NAME = reader["APPR_KEYBY_NAME"].ToString(),

                                    MOLD_DATE = reader["MOLD_DATE"].ToString(),
                                    APPEARANCE_DATE = reader["APPEARANCE_DATE"].ToString(),
                                    CHECKED_BY_DATE = reader["CHECKED_BY_DATE"].ToString(),
                                    VERIFIED_BY_DATE = reader["VERIFIED_BY_DATE"].ToString(),
                                    KEYED_BY_DATE = reader["KEYED_BY_DATE"].ToString(),
                                    LOT_STATUS = reader["LOT_STATUS"].ToString(),
                                    SILO_NO = reader["SILO_NO"].ToString(),
                                    ONLINE_MFR = reader["ONLINE_MFR"].ToString(),
                                    ONLINE_YI = reader["ONLINE_YI"].ToString(),
                                    ONLINE_YPBP = reader["ONLINE_YPBP"].ToString(),
                                    TWIN_PELLET = reader["TWIN_PELLET"].ToString(),
                                    FISHEYE_LEVEL = reader["FISHEYE_LEVEL"].ToString(),
                                    BS_SPOT_1 = reader["BS_SPOT_1"].ToString(),
                                    BS_SPOT_2 = reader["BS_SPOT_2"].ToString(),
                                    BS_SPOT_3 = reader["BS_SPOT_3"].ToString(),
                                    QA_REMARK = reader["QA_REMARK"].ToString(),
                                    COQ_ADJUST_RMK = reader["COQ_ADJUST_RMK"].ToString(),
                                    PRODUCTION_REMARK = reader["PRODUCTION_REMARK"].ToString(),
                                    BBS = reader["BBS"].ToString(),
                                    BYI = reader["BYI"].ToString(),
                                    REPACKLOT = reader["REPACKLOT"].ToString(),

                                    RECORD_TYP = reader["RECORD_TYP"].ToString(),
                                    CREATED_BY = reader["CREATED_BY"].ToString(),

                                    CREATED_LOC = reader["CREATED_LOC"].ToString(),
                                    UPDATED_BY = reader["UPDATED_BY"].ToString(),
                                    UPDATED_DATE = Convert.ToDateTime(reader["UPDATED_DATE"]),
                                    UPDATED_LOC = reader["UPDATED_LOC"].ToString(),

                                    IS_ARCHIVE = false
                                };
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
                return null;
            }

            return data;

          
        }
        public async Task<DailyIDSTransVM> getSummaryDataArc(string id)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            DailyIDSTransVM data = new DailyIDSTransVM();
            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("PSP_E_IDS_EDIT_SEL_ARC", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", id)).Direction = ParameterDirection.Input;

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                data = new DailyIDSTransVM
                                {
                                    ID_IDS_H = Convert.ToInt32(reader["ID_IDS_H"].ToString()),
                                    PRODTYPE = reader["PRODTYPE"].ToString() ?? "",
                                    LOTNO = reader["LOTNO"].ToString() ?? "",
                                    FullNG = reader["FullNG"].ToString() ?? "",
                                    PACKEDDATE = reader["PACKEDDATE"].ToString() ?? "",
                                    PACKEDDATE2 = reader["PACKEDDATE2"].ToString() ?? "",
                                    QUANTITY = Convert.ToInt32(reader["QUANTITY"].ToString()),
                                    TESTEDBY = reader["TESTEDBY"].ToString() ?? "",
                                    UPDATEDBY = reader["UPDATEDBY"].ToString() ?? "",
                                    STATUS = reader["STATUS"].ToString() ?? "",
                                    GRADE = reader["H_GRADE"].ToString() ?? "",
                                    OVERWRITE_GRADE = reader["OVERWRITE_GRADE"].ToString() ?? "",
                                    BS_MACH = reader["BS_MACH"].ToString() ?? "",
                                    MABS_P = reader["MABS_P"].ToString() == "1" ? true : false,
                                    FLOT = reader["FLOT"].ToString() == "1" ? true : false,
                                    prodline = reader["prodline"].ToString() ?? "", // nabila 16/03/2022
                                    prodgroup = reader["prodgroup"].ToString() ?? "",  // nabila 16/03/2022
                                    CREATED_DATE = Convert.ToDateTime(reader["CREATED_DATE"]),  // LiewKarWei 09/02/2023

                                    REPORT_BY = reader["REPORT_BY"].ToString(),
                                    APPR_APPEA_NAME = reader["APPR_APPEA_NAME"].ToString(),
                                    APPR_MOLD_NAME = reader["APPR_MOLD_NAME"].ToString(),
                                    APPR_CHKBY_NAME = reader["APPR_CHKBY_NAME"].ToString(),
                                    APPR_VERFBY_NAME = reader["APPR_VERFBY_NAME"].ToString(),
                                    APPR_KEYBY_NAME = reader["APPR_KEYBY_NAME"].ToString(),

                                    MOLD_DATE = reader["MOLD_DATE"].ToString(),
                                    APPEARANCE_DATE = reader["APPEARANCE_DATE"].ToString(),
                                    CHECKED_BY_DATE = reader["CHECKED_BY_DATE"].ToString(),
                                    VERIFIED_BY_DATE = reader["VERIFIED_BY_DATE"].ToString(),
                                    KEYED_BY_DATE = reader["KEYED_BY_DATE"].ToString(),
                                    LOT_STATUS = reader["LOT_STATUS"].ToString(),
                                    SILO_NO = reader["SILO_NO"].ToString(),
                                    ONLINE_MFR = reader["ONLINE_MFR"].ToString(),
                                    ONLINE_YI = reader["ONLINE_YI"].ToString(),
                                    ONLINE_YPBP = reader["ONLINE_YPBP"].ToString(),
                                    TWIN_PELLET = reader["TWIN_PELLET"].ToString(),
                                    FISHEYE_LEVEL = reader["FISHEYE_LEVEL"].ToString(),
                                    BS_SPOT_1 = reader["BS_SPOT_1"].ToString(),
                                    BS_SPOT_2 = reader["BS_SPOT_2"].ToString(),
                                    BS_SPOT_3 = reader["BS_SPOT_3"].ToString(),
                                    QA_REMARK = reader["QA_REMARK"].ToString(),
                                    COQ_ADJUST_RMK = reader["COQ_ADJUST_RMK"].ToString(),
                                    PRODUCTION_REMARK = reader["PRODUCTION_REMARK"].ToString(),
                                    BBS = reader["BBS"].ToString(),
                                    BYI = reader["BYI"].ToString(),
                                    REPACKLOT = reader["REPACKLOT"].ToString(),

                                    RECORD_TYP = reader["RECORD_TYP"].ToString(),
                                    CREATED_BY = reader["CREATED_BY"].ToString(),

                                    CREATED_LOC = reader["CREATED_LOC"].ToString(),
                                    UPDATED_BY = reader["UPDATED_BY"].ToString(),
                                    UPDATED_DATE = Convert.ToDateTime(reader["UPDATED_DATE"]),
                                    UPDATED_LOC = reader["UPDATED_LOC"].ToString(),

                                    IS_ARCHIVE = true
                                };
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
                return null;
            }

            return data;
        }

        public async Task<List<MoldingModel>> getMouldingData(string prodtype, string paction, string lotno) //Simulation By 
        {
            string spec = "";

            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            List<MoldingModel> chop = new List<MoldingModel>();

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_MOLD_PROP_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@P_prodtype", prodtype)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@P_lotno", lotno)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@P_type", paction)).Direction = ParameterDirection.Input;

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                spec = reader["SPECIFICATION"].ToString();
                                if (spec.Length >= 3 && spec.Substring(0, 3) == " , ")
                                {
                                    spec = spec.Substring(2);
                                }

                                chop.Add(new MoldingModel
                                {

                                    prodtype = reader["PRODTYPE"].ToString(),
                                    Properties = reader["MAIN_PROPERTIES"].ToString(),
                                    PropItem = reader["PROPITEM"].ToString(),
                                    Unit = reader["UNIT"].ToString(),
                                    TypeInd = reader["TYPEIND"].ToString(),
                                    RegressionResult = DBNull.Value.Equals(reader["REGRESSIONRESULT"]) ? "" : reader["REGRESSIONRESULT"].ToString(),
                                    Average = reader["AVERAGE"].ToString(),
                                    COQAdj = reader["COQADJ"].ToString(),
                                    Grade = reader["GRADE"].ToString(),
                                    PCL_Result = reader["PCL_RESULT"].ToString(),
                                    MachineName = reader["MACHINENAME"].ToString(),
                                    DataChecking = reader["DATACHECKING"].ToString(),
                                    COQInd = reader["COQind"].ToString(),
                                    ProdGroup = reader["PRODGROUP"].ToString(),
                                    MainReading = reader["MAINREADING"].ToString(),
                                    StatusInd = reader["STATUSIND"].ToString(),
                                    RegressFormula = reader["REGRESSIONFORMULA"].ToString(),
                                    RegressInd = reader["REGRESSIONIND"].ToString(),
                                    Count = Convert.ToInt32(reader["COUNTS"].ToString()),
                                    Specification = spec,
                                    PCL_Spec = reader["PCL_SPEC"].ToString(),
                                    rounding = Convert.ToInt32(reader["ROUNDING"].ToString()),
                                    ID_IDS_D = Convert.ToInt32(reader["ID_IDS_D"].ToString()),
                                    ID_IDS_H = Convert.ToInt32(reader["ID_IDS_H"].ToString()),
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
        public async Task<List<MoldingModel>> getMouldingDataArc(string prodtype, string paction, string lotno) //Simulation By 
        {
            string spec = "";

            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            List<MoldingModel> chop = new List<MoldingModel>();

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_MOLD_PROP_SEL_ARC", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@P_prodtype", prodtype)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@P_lotno", lotno)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@P_type", paction)).Direction = ParameterDirection.Input;

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                spec = reader["SPECIFICATION"].ToString();
                                if (spec.Length >= 3 && spec.Substring(0, 3) == " , ")
                                {
                                    spec = spec.Substring(2);
                                }

                                chop.Add(new MoldingModel
                                {

                                    prodtype = reader["PRODTYPE"].ToString(),
                                    Properties = reader["MAIN_PROPERTIES"].ToString(),
                                    PropItem = reader["PROPITEM"].ToString(),
                                    Unit = reader["UNIT"].ToString(),
                                    TypeInd = reader["TYPEIND"].ToString(),
                                    RegressionResult = DBNull.Value.Equals(reader["REGRESSIONRESULT"]) ? "" : reader["REGRESSIONRESULT"].ToString(),
                                    Average = reader["AVERAGE"].ToString(),
                                    COQAdj = reader["COQADJ"].ToString(),
                                    Grade = reader["GRADE"].ToString(),
                                    MachineName = reader["MACHINENAME"].ToString(),
                                    DataChecking = reader["DATACHECKING"].ToString(),
                                    COQInd = reader["COQind"].ToString(),
                                    ProdGroup = reader["PRODGROUP"].ToString(),
                                    MainReading = reader["MAINREADING"].ToString(),
                                    StatusInd = reader["STATUSIND"].ToString(),
                                    RegressFormula = reader["REGRESSIONFORMULA"].ToString(),
                                    RegressInd = reader["REGRESSIONIND"].ToString(),
                                    Count = Convert.ToInt32(reader["COUNTS"].ToString()),
                                    Specification = spec,
                                    rounding = Convert.ToInt32(reader["ROUNDING"].ToString()),
                                    ID_IDS_D = Convert.ToInt32(reader["ID_IDS_D"].ToString()),
                                    ID_IDS_H = Convert.ToInt32(reader["ID_IDS_H"].ToString()),
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

        public async Task<DataTable>getTransHistData(string id, string prodtype, string lotno)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_SL_EDIT_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", id)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pProdtype", prodtype)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pLotno", lotno)).Direction = ParameterDirection.Input;
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
        public async Task<DataTable> getTransHistDataArc(string id, string prodtype, string lotno)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_SL_EDIT_SEL_ARC", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", id)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pProdtype", prodtype)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pLotno", lotno)).Direction = ParameterDirection.Input;
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

        public async Task<DataTable>getIDSDTransHistData(string id, string prodtype, string lotno)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_SL_IDSD_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", id)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pProdtype", prodtype)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pLotno", lotno)).Direction = ParameterDirection.Input;
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

        public async Task<List<AppearanceTableModel>> getAppearanceList(string id) //Simulation By 
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            List<AppearanceTableModel> chop = new List<AppearanceTableModel>();

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("PSP_E_IDSA_EDIT_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", id)).Direction = ParameterDirection.Input;

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                chop.Add(new AppearanceTableModel
                                {

                                    Properties = reader["PROPERTIES"].ToString(),
                                    PropertyItem = reader["PROPITEM"].ToString() ?? "",
                                    Mgrade = reader["MGRADE"].ToString(),

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
        public async Task<List<AppearanceTableModel>> getAppearanceListArc(string id) //Simulation By 
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            List<AppearanceTableModel> chop = new List<AppearanceTableModel>();

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("PSP_E_IDSA_EDIT_SEL_ARC", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", id)).Direction = ParameterDirection.Input;

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                chop.Add(new AppearanceTableModel
                                {

                                    Properties = reader["PROPERTIES"].ToString(),
                                    PropertyItem = reader["PROPITEM"].ToString() ?? "",
                                    Mgrade = reader["MGRADE"].ToString(),

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

        public async Task<DataTable>GetAppearanceTableList(string prodtype, string lotno, int type)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_APP_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@prodtype", prodtype)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@lotno", lotno)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@type", type)).Direction = ParameterDirection.Input;
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
        public async Task<DataTable> GetAppearanceTableListArc(string prodtype, string lotno, int type)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_APP_SEL_ARC", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@prodtype", prodtype)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@lotno", lotno)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@type", type)).Direction = ParameterDirection.Input;
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

        public async Task<string> IDS_Seg_Maint(string id, string id_ids_d, string from, string to, string reading, string grade, string gradeInd, string rec_type, string type, string updatedby, string updated_loc)  //view function
        {
            string result = "";
            string return_value = "0";

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
                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_SEG_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID_IDS_Seg", id)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pID_IDS_D", id_ids_d)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PTagNoFrom", from)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PTagNoTo", to)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Preading", reading)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Pgrade", grade)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PgradeInd", gradeInd)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Ptype", type)).Direction = ParameterDirection.Input;

                        cmd.Parameters.Add(new SqlParameter("@pRecType", rec_type)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCreatedBy", updatedby)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCreatedLoc", updated_loc)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ReturnID", SqlDbType.Int)).Direction = ParameterDirection.Output;
                        cmd.ExecuteReader();


                        return_value = Convert.ToString(cmd.Parameters["@ReturnID"].Value);

                        result = return_value;

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

        public async Task<DataTable>CheckSegL(string id_ids_d)
        {
            string result = "";
            string return_value = "0";

            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_SEG_MAINT_Chk", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID_IDS_D", id_ids_d)).Direction = ParameterDirection.Input;
                        reader = await cmd.ExecuteReaderAsync();
                        dt.Load(reader);
                        reader.Close();
                        result = return_value;
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

        public async Task<string> DeleteDraft(string id_ids_h)
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
                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_DELETE", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@PID_IDS_H", id_ids_h)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ReturnID", SqlDbType.Int)).Direction = ParameterDirection.Output;
                        cmd.ExecuteReader();

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

        public async Task<string> IDS_D_UPD(string id_ids_d, string finalresult, string grade)
        {
            string result = "";
            string return_value = "0";

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
                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_D_UPD", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID_IDS_D", id_ids_d)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PQTY", finalresult)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PGRADE", grade)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ReturnID", SqlDbType.Int)).Direction = ParameterDirection.Output;
                        cmd.ExecuteReader();


                        return_value = Convert.ToString(cmd.Parameters["@ReturnID"].Value);

                        result = return_value;

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

        public async Task<dynamic> COQCopy(string lot, string prodtype, string type)
        {
            string prodline = lot.Substring(1, 2);
            string result = "";
            string return_value = "0";
            string days = "";

            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_COQ_COPY", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@lotno", lot)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pprodtype", prodtype)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pprodline", prodline)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@type", type)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ReturnID", SqlDbType.Int)).Direction = ParameterDirection.Output;
                        reader = await cmd.ExecuteReaderAsync();
                        dt.Load(reader);
                        reader.Close();

                        result = return_value;
                        days = Convert.ToString(command.Parameters["@ReturnID"].Value);
                        if (type == "2")
                        {
                            return days;
                        }
                        return dt;

                    }
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

        public async Task<string> IDS_SUM_MAINT_ALL(List<SummaryModel> IDSSUM)
        {
            string result = "";
            string return_value = "0";

            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();
            string createdby = USERID;
            string loc = HttpContext.Request.UserHostAddress.ToString();
            string pc = Environment.MachineName;

            var dataTable = new DataTable();

            // Get the properties of the first item
            var properties = typeof(SummaryModel).GetProperties();

            // Add columns to the DataTable
            foreach (var property in properties)
            {
                dataTable.Columns.Add(property.Name, property.PropertyType);
            }
            foreach (var item in IDSSUM)
            {
                var row = dataTable.NewRow();
                foreach (var property in properties)
                {
                    row[property.Name] = property.GetValue(item);
                }
                dataTable.Rows.Add(row);
            }

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_SUM_MAINT_ALL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@TBLIDSSUM", dataTable)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@RETURNMSG", SqlDbType.VarChar, 800)).Direction = ParameterDirection.Output;
                        cmd.ExecuteReader();


                        return_value = Convert.ToString(cmd.Parameters["@RETURNMSG"].Value);

                        result = return_value;

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

        public async Task<string> IDS_FIRSTBAG(List<FirstBagModel> FirstBagList, string IDH, string rec_type, string updatedby, string updated_loc)
        {
            string result = "";
            string return_value = "0";

            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();
            string loc = HttpContext.Request.UserHostAddress.ToString();
            string pc = Environment.MachineName;

            var dtFBList = new DataTable();
            var properties = typeof(FirstBagModel).GetProperties();

            foreach (var property in properties)
            {
                dtFBList.Columns.Add(property.Name, property.PropertyType);
            }
            foreach (var item in FirstBagList)
            {
                var row = dtFBList.NewRow();
                foreach (var property in properties)
                {
                    row[property.Name] = property.GetValue(item);
                }
                dtFBList.Rows.Add(row);
            }

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_FIRSTBAG_MAINT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter("@PTBLFIRSTBAG", dtFBList)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PIDH", IDH)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pRecType", rec_type)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCreatedBy", updatedby)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCreatedLoc", updated_loc)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@PRETURNMSG", SqlDbType.VarChar, 800)).Direction = ParameterDirection.Output;
                        cmd.ExecuteReader();


                        return_value = Convert.ToString(cmd.Parameters["@PRETURNMSG"].Value);

                        result = return_value;

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

        public async Task<List<MoldingModel>> getLatestCOQ(string prodtype, string lotno) //Simulation By 
        {
            string spec = "";

            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            List<MoldingModel> chop = new List<MoldingModel>();

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("PSP_IDS_GET_LATEST_COQ", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pPRODUCTCODE", prodtype)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pLOTNO", lotno)).Direction = ParameterDirection.Input;

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                chop.Add(new MoldingModel
                                {
                                    Properties = reader["PROPERTIES"].ToString(),
                                    PropItem = reader["PROPITEM"].ToString(),                                    
                                    COQAdj = reader["COQREADING"].ToString(),
                                    lotno = reader["LOTNO"].ToString(),
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



        //JH250808
        public async Task<(List<DailyIDSTransVM> ListModel, int totalRecords)> getIDSTransListing(
            string searchTerm = "", string tableName = "PVIEW_IDS_H", string columnsToSearch = "LOTNO", int start = 0, int length = 10,
            string sortColumn = "", string sortDirection = "ID_IDS_H", string filterQuery = ""
        )
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            List<DailyIDSTransVM> ListModel = new List<DailyIDSTransVM>();
            int totalRecords = 0;

            var obj = new
            {
                TableName = tableName,
                SearchColumns = columnsToSearch,
                SearchTerm = searchTerm,
                Start = start,
                Length = length,
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                FilterQuery = filterQuery
            };

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {

                    await con.OpenAsync();

                    // Execute stored procedure with QueryMultiple
                    var result = await con.QueryMultipleAsync(
                        "PSP_DATATABLE_SERVER_LISTING",
                        obj,
                        commandType: CommandType.StoredProcedure
                    );

                    // Read the paginated data
                    ListModel = result.Read<DailyIDSTransVM>().ToList();

                    // Read the total record count as integer
                    totalRecords = result.ReadFirstOrDefault<int>();
                }

            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
            }

            return (ListModel, totalRecords);
        }


    }
}