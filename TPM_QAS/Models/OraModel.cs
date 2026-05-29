using TPM_QAS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using TPM_QAS.DAL;
using Microsoft.Data.SqlClient;
using TPM_QAS.Helpers;
using Oracle.ManagedDataAccess.Client;
using Microsoft.AspNetCore.Http;

namespace OraModel
{
}

public class Ora : Database
{
    public Ora()
    {
    }

    private ACL_UserObj GetCurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        return httpContextAccessor?.HttpContext?.Session?.GetObject<ACL_UserObj>("AclUser");
    }

    public async Task<List<DataSet>> YI(ReportYiVM m, string addtable, IHttpContextAccessor httpContextAccessor = null)
    {
        string USERID = "";
        try
        {
            var userobj = GetCurrentUser(httpContextAccessor);
            USERID = userobj?.EMP_NAME?.ToString() ?? "";
        }
        catch { }

        List<DataSet> ds = new List<DataSet>();
        DataTable dt = new DataTable();

        try
        {
            string constr = await GetConnectionStringAsync(false, "LIVE_ORA_REPORT");

            if (m.xaxis == "LotNo")
            {
                using (OracleConnection con = new OracleConnection(constr))
                {
                    using (OracleCommand cmd = new OracleCommand("select r.segment1 as Product_Type, r.LOT_NUMBER as Lot_Number , r.pack_date as Packed_Date " + addtable + ", r.QUANTITY, r.Grade from PVIEW_ABS_PROPERTIES_INSP_RPT r where r.pack_date between to_date('" + m.datefrom + "', 'yyyy-mm-dd') and to_date('" + m.dateto + "', 'yyyy-mm-dd') and r.segment1 = '" + m.prodtype + "' and r.PROCESS_LINE between '" + m.prodlinefrom + "' AND '" + m.prodlineto + "'", con))
                    {
                        await con.OpenAsync();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Clear();

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                }

                DataSet dsItem = new DataSet();
                dsItem.Tables.Add(dt);
                ds.Add(dsItem);
            }
        }
        catch (Exception ex)
        {
            ErrorLogSys err = new ErrorLogSys();
            await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, USERID);
            err = null;
            dt = null;
        }

        return ds;
    }

    public async Task<DataTable> OraColumnsName(IHttpContextAccessor httpContextAccessor = null)
    {
        string USERID = "";
        try
        {
            var userobj = GetCurrentUser(httpContextAccessor);
            USERID = userobj?.EMP_NAME?.ToString() ?? "";
        }
        catch { }

        try
        {
            string constr = await GetConnectionStringAsync(false, "LIVE_ORA_REPORT");
            DataTable dt = new DataTable();
            using (OracleConnection con = new OracleConnection(constr))
            {
                using (OracleCommand cmd = new OracleCommand("SELECT column_name FROM USER_TAB_COLUMNS WHERE table_name = 'PVIEW_ABS_PROPERTIES_INSP_RPT'", con))
                {
                    await con.OpenAsync();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.Clear();

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
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

    public async Task<DataTable> MonthlyNG(ReportNGVM m, IHttpContextAccessor httpContextAccessor = null)
    {
        string USERID = "";
        try
        {
            var userobj = GetCurrentUser(httpContextAccessor);
            USERID = userobj?.EMP_NAME?.ToString() ?? "";
        }
        catch { }

        try
        {
            string constr = await GetConnectionStringAsync(false, "LIVE_ORA_REPORT");
            DataTable dt = new DataTable();
            using (OracleConnection con = new OracleConnection(constr))
            {
                using (OracleCommand cmd = new OracleCommand("select r.segment1 as Product_Type , r.LOT_NUMBER as Lot_No, r.pack_date as pack_date, r.QUANTITY as Qty, r.Grade, r.Internal_Remarks" +
                            " from PVIEW_ABS_PROPERTIES_INSP_RPT r " +
                            "where pack_date between to_date('" + m.datefrom + "', 'dd/mm/yyyy') and to_date('" + m.dateto + "', 'dd/mm/yyyy')and grade = 'R' and process_line != 07", con))
                {
                    await con.OpenAsync();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.Clear();

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
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

    public async Task<DataTable> MonthlyNGCap7(ReportNGVM m, IHttpContextAccessor httpContextAccessor = null)
    {
        string USERID = "";
        try
        {
            var userobj = GetCurrentUser(httpContextAccessor);
            USERID = userobj?.EMP_NAME?.ToString() ?? "";
        }
        catch { }

        try
        {
            string constr = await GetConnectionStringAsync(false, "LIVE_ORA_REPORT");
            DataTable dt = new DataTable();
            using (OracleConnection con = new OracleConnection(constr))
            {
                using (OracleCommand cmd = new OracleCommand("select r.segment1 as Product_Type , r.LOT_NUMBER as Lot_No, r.pack_date as pack_date, r.QUANTITY as Qty, r.Grade, r.Internal_Remarks" +
                            " from PVIEW_ABS_PROPERTIES_INSP_RPT r " +
                            "where pack_date between to_date('" + m.datefrom + "', 'dd/mm/yyyy') and to_date('" + m.dateto + "', 'dd/mm/yyyy')and grade = 'R' and process_line = 07", con))
                {
                    await con.OpenAsync();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.Clear();

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
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
