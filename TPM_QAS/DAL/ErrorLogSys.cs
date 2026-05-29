using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace TPM_QAS.DAL
{
    public class ErrorLogSys : Database
    {
        public async Task<bool> ErrorLog_Add_V2(string FunctionName, Exception exData, string CreatedBy, string OptnalRef = "")
        {
            try
            {
                string constr = await GetConnectionStringAsync();

                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("PSP_ERROR_LOGS", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;

                        cmd.Parameters.Add(new SqlParameter("@pFunctionName", FunctionName)).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pErrorMSG", OptnalRef + exData.Message.ToString())).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pSource", exData.Source?.ToString() ?? "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pStackTrace", exData.StackTrace?.ToString() ?? "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pTargetSite", exData.TargetSite?.ToString() ?? "")).Direction = ParameterDirection.Input;
                        cmd.Parameters.Add(new SqlParameter("@pCreateBy", CreatedBy)).Direction = ParameterDirection.Input;
                        cmd.ExecuteReader();

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
                return false;
            }
        }

        internal void ErrorLog_Add_V2(string name, Exception ex, object uSERID)
        {
            throw new NotImplementedException();
        }
    }
}
