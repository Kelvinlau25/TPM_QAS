using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using CompareAttribute = System.ComponentModel.DataAnnotations.CompareAttribute;
using TPM_QAS.DAL;
using System.Threading.Tasks;

namespace HomeModel
{
    public class SideBarContent
    {

        public int ID_ACL_RESOURCE { get; set; }
        public int RESOURCE_PARENT_ID { get; set; }
        public string RESOURCE_DESC { get; set; }
        public string RESOURCE_name { get; set; }
        public string RESOURCE_VIEW { get; set; }
        public string RESOURCE_CONTROLLER { get; set; }
        public int LAYER { get; set; }
        public int ACTION { get; set; }
    }

    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "* Please Enter Old Password.")]
        [Display(Name = "Old Password")]
        public string OLD_PASSWORD { get; set; }
        [Required(ErrorMessage = "* Please Enter New Password.")]
        [Display(Name = "New Password")]
        public string NEW_PASSWORD { get; set; }
        [Required(ErrorMessage = "* Please Confirm Password by Enter New Password again.")]
        [NotMapped]
        [Compare("NEW_PASSWORD")]
        [Display(Name = "Confirm New Password")]
        public string CONFIRM_NEW_PASSWORD { get; set; }
    }
    public class AuthenticatorModel
    {
        public int ID_ACL_USER { get; set; }
        public string USER_ID { get; set; }
        public string USR_EMAIL { get; set; }
        public string COMPANY { get; set; }
        public string EMP_NO { get; set; }
        public string EMP_NAME { get; set; }
        public int ID_ACL_ROLE { get; set; }
        public string ROLE_NAME { get; set; }
        public string ROLE_DESC { get; set; }
        public int ID_ACL_RESOURCE { get; set; }
        public string RESOURCE_NAME { get; set; }
        public string RESOURCE_DESC { get; set; }
        public bool VALID_USER { get; set; }
        [Required(ErrorMessage = "* Please Enter Username.")]
        [Display(Name = "Username")]
        public string LOGIN_ID { get; set; }
        [Required(ErrorMessage = "* Please Enter password.")]
        [Display(Name = "Password")]
        public string PASSWORD { get; set; }

    }

    public class DB : Database
    {
        public DB()
        {

        }

        public async Task<AuthenticatorModel> ValidateUserInfo(string userAD, string systemName)
        {
            AuthenticatorModel AuthenticatorModel = null;
            AuthenticatorModel = new AuthenticatorModel();
            AuthenticatorModel.VALID_USER = false;

            string constr = await GetConnectionStringAsync(true);

            using (SqlConnection con = new SqlConnection(constr))
            {

                using (SqlCommand cmd = new SqlCommand("PSP_ACL_USER_SEL", con))
                {
                    await con.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.Clear();

                    cmd.Parameters.Add(new SqlParameter("@pUserID", userAD)).Direction = System.Data.ParameterDirection.Input;
                    cmd.Parameters.Add(new SqlParameter("@pSystemName", systemName)).Direction = System.Data.ParameterDirection.Input;

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (reader.HasRows)
                            {

                                AuthenticatorModel.ID_ACL_USER = Convert.ToInt16(reader["ID_ACL_USER"]);
                                AuthenticatorModel.ID_ACL_ROLE = Convert.ToInt16(reader["ID_ACL_ROLE"]);
                                AuthenticatorModel.ID_ACL_RESOURCE = Convert.ToInt16(reader["ID_ACL_RESOURCE"]);
                                AuthenticatorModel.USER_ID = reader["USER_ID"].ToString();
                                AuthenticatorModel.USR_EMAIL = reader["USR_EMAIL"].ToString();
                                AuthenticatorModel.COMPANY = reader["COMPANY"].ToString();
                                AuthenticatorModel.EMP_NO = reader["EMP_NO"].ToString();
                                AuthenticatorModel.EMP_NAME = reader["EMP_NAME"].ToString();
                                AuthenticatorModel.ROLE_NAME = reader["ROLE_NAME"].ToString();
                                AuthenticatorModel.ROLE_DESC = reader["ROLE_DESC"].ToString();
                                AuthenticatorModel.RESOURCE_NAME = reader["RESOURCE_NAME"].ToString();
                                AuthenticatorModel.RESOURCE_DESC = reader["RESOURCE_DESC"].ToString();
                                AuthenticatorModel.PASSWORD = reader["USR_PASSWORD"].ToString();
                                AuthenticatorModel.VALID_USER = true;
                            }
                            else
                            {
                                AuthenticatorModel.VALID_USER = false;
                            }
                        }
                    }
                }
            }
            return AuthenticatorModel;

        }

        #region menu
        public async Task<DataTable> sideBarDB(Int64 roleID, string SystemName)
        {
            string constr = await GetConnectionStringAsync(true);
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("PSP_ACL_SIDEBAR_PERMISSION", con))
                {
                    await con.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.Clear();

                    cmd.Parameters.Add(new SqlParameter("@ID_ACL_ROLE", roleID)).Direction = System.Data.ParameterDirection.Input;
                    cmd.Parameters.Add(new SqlParameter("@pSystemName", SystemName)).Direction = System.Data.ParameterDirection.Input;
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

        public async Task<DataTable> oldPassword(int userID)
        {
            string constr = await GetConnectionStringAsync(true);
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("PSP_ACL_CHANGE_PASSWORD", con))
                {
                    await con.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.Clear();

                    cmd.Parameters.Add(new SqlParameter("@userID", userID)).Direction = System.Data.ParameterDirection.Input;
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

        public async Task<string> NewPassWord(int userID, string newPassword)
        {
            string constr = await GetConnectionStringAsync(true);
            DataTable dt = new DataTable();
            string result = "0";

            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("PSP_ACL_CHANGE_PASSWORD_MAINT", con))
                {
                    await con.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.Clear();

                    cmd.Parameters.Add(new SqlParameter("@userID", userID)).Direction = System.Data.ParameterDirection.Input;
                    cmd.Parameters.Add(new SqlParameter("@newPassword", newPassword)).Direction = System.Data.ParameterDirection.Input;
                    cmd.Parameters.Add(new SqlParameter("@returnID", SqlDbType.VarChar, 1)).Direction = System.Data.ParameterDirection.Output;
                    reader = await cmd.ExecuteReaderAsync();
                    dt.Load(reader);
                    reader.Close();

                    return result = Convert.ToString(cmd.Parameters["@ReturnID"].Value);

                }
            }
        }
        #endregion

    }
}