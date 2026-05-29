using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Threading.Tasks;
using DatabaseModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.ComponentModel.DataAnnotations.Schema;
using TPM_QAS.Helpers;
using TPM_QAS.DAL;

namespace DBModel
{
    public class DB : TPM_QAS.DAL.Database
    {
        public DB()
        {
        }

        public async Task<DataTable> List(string Table, string TableID, string Search,
        string Value, string SortField, string Direction,
        string FrmRowno, string ToRowno, string Deleted)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_COMMON_LIST", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@Table", Table)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@TableID", TableID)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Search", Search)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Value", Value)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@SortField", SortField)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Direction", Direction)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@FrmRowno", FrmRowno)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ToRowno", ToRowno)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Deleted", Deleted)).Direction = ParameterDirection.Input;

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

        public async Task<DataTable> PSP_CMMS_LIST(string Table, string TableID, string Search,
        string Value, string SortField, string Direction,
        string FrmRowno, string ToRowno, string Deleted)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_CMMS_LIST", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@Table", Table)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@TableID", TableID)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Search", Search)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Value", Value)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@SortField", SortField)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Direction", Direction)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@FrmRowno", FrmRowno)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ToRowno", ToRowno)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@Deleted", Deleted)).Direction = ParameterDirection.Input;
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
        
        public async Task<DataTable> AuditList(string Table, string pid)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_COMMON_AUDIT", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pTable", Table)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pKeyValue", pid)).Direction = ParameterDirection.Input;
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

        public async Task<List<SelectListItem>> PSP_COMMON_DDL(string type, string p1, string p2, string p3, string p4, string p5, string p6)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            List<SelectListItem> items = new List<SelectListItem>();
            DataTable dt = new DataTable();

            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_ALLDDL_SEL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@pID", type)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ptext", p1)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ptext2", p2)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ptext3", p3)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ptext4", p4)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ptext5", p5)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@ptext6", p6)).Direction = ParameterDirection.Input;

                        reader = await cmd.ExecuteReaderAsync();
                        dt.Load(reader);
                        reader.Close();

                    }
                }

            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
                err = null;
                dt = null;
            }

            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    SelectListItem infoObj = new SelectListItem();
                    infoObj.Text = dt.Rows[i]["TEXT"].ToString();
                    infoObj.Value = dt.Rows[i]["VALUE"].ToString();
                    items.Add(infoObj);
                }
            }
            return items;
        }

        public async Task<DataTable> getDllData(int ID, string act, string category)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_GET_DLL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", ID)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pACTION", act)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCATEGORY", category)).Direction = ParameterDirection.Input;
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

        public async Task<DataTable> getSectionDllData(int ID, string act, string category)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_CE_E_SECTION_DLL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pHdrId ", ID)).Direction = ParameterDirection.Input;
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

        public async Task<DataTable> getPicDllData(int ID, string section, string act, string category)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_CE_E_PIC_DLL", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pID", ID)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pSECTION", section)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pACTION", act)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCATEGORY", category)).Direction = ParameterDirection.Input;

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
        
        public async Task<DataTable> getRoleList(string EMPNO)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME.ToString();

            try
            {
                string constr = await GetConnectionStringAsync();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_GET_ROLE_LIST", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add(new SqlParameter("@pEMPNO", EMPNO)).Direction = ParameterDirection.Input;
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

        // Function to check if any role in 'userRole' DataTable matches the provided 'roleToMatch'.
        // 'userRole' is the DataTable containing roles to check against.
        // 'roleToMatch' can be a single string or a List<string>.
        public bool HasMatchingRole(DataTable userRole, object roleToMatch)
        {
            string appRoleNameToMatch;
            if (roleToMatch is string) // If the roleToMatch is a single string
            {
                appRoleNameToMatch = roleToMatch.ToString();
                return userRole.AsEnumerable().Any(row => row.Field<string>("APP_ROLE_NAME") == appRoleNameToMatch);
            }
            else if (roleToMatch is List<string>) // If the roleToMatch is a List<string>
            {
                List<string> rolesList = roleToMatch as List<string>;
                return userRole.AsEnumerable().Any(row => rolesList.Contains(row.Field<string>("APP_ROLE_NAME")));
            }
            else
            {
                throw new ArgumentException("roleToMatch must be either a single string or a List<string>.");
            }
        }
        public bool HasMatchingRoleAndSection(DataTable userRole, string role, string section)
        {
            var matched = false;
            foreach (DataRow row in userRole.Rows)
            {
                string appRoleName = row["APP_ROLE_NAME"].ToString();
                string sectionName = row["SECTION"].ToString();
                if (appRoleName == role && sectionName == section)
                {
                    matched = true;
                }
            }
            return matched;
        }

    }

    public class SimilarityModel
    {
        public class SimilarityRequestModel
        {
            public int idMmMainDropdown { get; set; }
            public String input { get; set; }

            public List<String> compareList { get; set; }
            public String api_key { get; set; }

        }
        public class SimilarityResponseModel
        {

            public String info { get; set; }
            public int score { get; set; }
            public List<Object[]> similarityList { get; set; }
            public Object test { get; set; }
        }

        public readonly string m_clientAddress = WebTPM_QAS.DAL.Database.GetAppSettingStatic("CompareWordClientAddress"].ToString();
        public readonly string Api_key = WebTPM_QAS.DAL.Database.GetAppSettingStatic("api_key"].ToString();

        public async Task<SimilarityResponseModel> GetSimilarity(string i_no, List<String> i_comparelist)
        {
            var similaritymodel = new SimilarityModel.SimilarityRequestModel()
            {
                idMmMainDropdown = 0,
                compareList = i_comparelist,
                input = i_no,
                api_key = Api_key
            };
            SimilarityModel.SimilarityResponseModel similarityResponse = await retrieveSimilarityResponseAsync(similaritymodel);

            return similarityResponse;
        }

        private async Task<SimilarityResponseModel> retrieveSimilarityResponseAsync(SimilarityRequestModel similarityRequestModel)
        {
            try
            {
                string credentialUser = WebTPM_QAS.DAL.Database.GetAppSettingStatic("proxyEmail"].ToString();
                string credentialPassword = WebTPM_QAS.DAL.Database.GetAppSettingStatic("proxyPassword"].ToString();
                //First create a proxy object

                var httpClientHandler = new HttpClientHandler();
                if (!String.IsNullOrEmpty(credentialUser) && !String.IsNullOrEmpty(credentialPassword))
                {
                    var proxy = new WebProxy
                    {
                        Address = new Uri($"http://10.252.133.21:8080"),
                        BypassProxyOnLocal = false,
                        UseDefaultCredentials = false,

                        // *** These creds are given to the proxy server, not the web server ***
                        Credentials = new NetworkCredential(
                        userName: credentialUser,
                        password: credentialPassword)
                    };

                    // Now create a client handler which uses that proxy
                    httpClientHandler = new HttpClientHandler
                    {
                        Proxy = proxy,

                    };
                }

                var json = JsonConvert.SerializeObject(similarityRequestModel);
                var buffer = System.Text.Encoding.UTF8.GetBytes(json);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using (var client = new HttpClient(httpClientHandler, false))
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    HttpResponseMessage response = await client.PostAsync(m_clientAddress, byteContent);

                    var resp = await response.Content.ReadAsStringAsync();
                    SimilarityResponseModel respObj = JsonConvert.DeserializeObject<SimilarityResponseModel>(resp);

                    return respObj;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }
    }

}