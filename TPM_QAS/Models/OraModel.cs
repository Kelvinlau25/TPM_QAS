using TPM_QAS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using TPM_QAS.DAL;
using System.Data.SqlClient;
using TPM_QAS.Helpers;
using Oracle.ManagedDataAccess.Client;

namespace OraModel
{

}

//public class Ora : OracleModel.Oracle
public class Ora : Database
{
    public Ora()
    {

    }


    public async Task<List<DataSet>> YI(ReportYiVM m, string addtable)  //DATEFORMAT = 02/09/2021
    {
        ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
        string USERID = userobj.EMP_NAME.ToString();

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

        //m.datefrom = "02/09/2021";
        //m.dateto = "03/09/2021";
        //m.prodtype = "920-555--25";
        //m.prodlinefrom = "06";
        //m.prodlineto = "07";
        //List<DataSet> ds = new List<DataSet>();
        //OpenConn("ORA_REPORT");

        //if (m.xaxis == "LotNo")
        //{
        //    //Rem by KL Ong on 20221229: the same amount of data is duplicate
        //    //ds.Add(ExecuteReaderDS("select r.segment1 as ProdType, r.YELLOWNESS_INDEX as YI from PVIEW_ABS_PROPERTIES_INSP_RPT r where r.pack_date between to_date('" + m.datefrom + "', 'dd/mm/yyyy') and to_date('" + m.dateto + "', 'dd/mm/yyyy') and r.segment1 = '" + m.prodtype + "' and r.PROCESS_LINE between '" + m.prodlinefrom + "' AND '" + m.prodlineto + "'"));
        //    //ds.Add(ExecuteReaderDS("select r.segment1 as Product_Type, r.LOT_NUMBER as Lot_Number , r.pack_date as Packed_Date, r.YELLOWNESS_INDEX as YI, r.QUANTITY, r.Grade from PVIEW_ABS_PROPERTIES_INSP_RPT r where r.pack_date between to_date('" + m.datefrom + "', 'dd/mm/yyyy') and to_date('" + m.dateto + "', 'dd/mm/yyyy') and r.segment1 = '" + m.prodtype + "' and r.PROCESS_LINE between '" + m.prodlinefrom + "' AND '" + m.prodlineto + "'"));
        //    ds.Add(ExecuteReaderDS("select r.segment1 as Product_Type, r.LOT_NUMBER as Lot_Number , r.pack_date as Packed_Date " +  addtable + ", r.QUANTITY, r.Grade from PVIEW_ABS_PROPERTIES_INSP_RPT r where r.pack_date between to_date('" + m.datefrom + "', 'yyyy-mm-dd') and to_date('" + m.dateto + "', 'yyyy-mm-dd') and r.segment1 = '" + m.prodtype + "' and r.PROCESS_LINE between '" + m.prodlinefrom + "' AND '" + m.prodlineto + "'"));

        //    //SELECT column_name FROM USER_TAB_COLUMNS WHERE table_name = 'PVIEW_ABS_PROPERTIES_INSP_RPT'
        //}

        //CloseConnection();

        //return ds;
    }

    public async Task<DataTable> OraColumnsName()
    {
        ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
        string USERID = userobj.EMP_NAME.ToString();

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

    public async Task<DataTable> MonthlyNG(ReportNGVM m)
    {
        ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
        string USERID = userobj.EMP_NAME.ToString();

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

    public async Task<DataTable> MonthlyNGCap7(ReportNGVM m)
    {
        ACL_UserObj userobj = (ACL_UserObj)HttpContext.Current.Session["AclUser"];
        string USERID = userobj.EMP_NAME.ToString();

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