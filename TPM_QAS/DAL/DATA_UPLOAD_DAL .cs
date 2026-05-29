using Oracle.ManagedDataAccess.Client;
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
    public class DATA_UPLOAD_DAL : Database
    {

        public async Task<DataTable> getORAMaterialList()
        {
            List<OraMaterialList> oramat = new List<OraMaterialList>();
            try
            {
                //string constr = ConfigurationManager.ConnectionStrings["ORA_ICCP"].ConnectionString; 
                string constr = await GetConnectionStringORACOAAsync();
                DataTable dt = new DataTable();

                using (OracleConnection myConnection = new OracleConnection(constr))
                {
                    using (OracleCommand cmd = new OracleCommand("SP_TPM_COA_GET_MM_MATERIAL", myConnection))
                    {
                        myConnection.Open();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new OracleParameter("SREFData", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;
                        OracleDataReader rdr = cmd.ExecuteReader();

                        dt.Load(rdr);
                        return dt;
                    }

                }                
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, "getoradata");
                err = null;
                return null;
            }

        }

    }
}