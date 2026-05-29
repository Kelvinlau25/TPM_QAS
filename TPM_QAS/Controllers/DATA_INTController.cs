using DBModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using TPM_QAS.Filters;
using TPM_QAS.Helpers;
using TPM_QAS.DAL;
using TPM_QAS.Models;
using System.IO;
using DocumentFormat.OpenXml.Bibliography;

namespace TPM_QAS.Controllers
{
    public class DATA_INTController : Controller
    {
        DB dbmain = new DB();
        DATA_INT_DAL dbdal = new DATA_INT_DAL();

        [SessionExpire]
        public async Task<ActionResult> DATA_INT(string Deleted = "0")
        {
            TempData["STATUS_DDL"] = await dbmain.PSP_COMMON_DDL("DATA_INT", "", "", "", "", "", "");

            var model = new DataIntModel();

            CommonFunction common = new CommonFunction();
            var param = new { Table = "PVIEW_LIST_INTEGRATION", TableID = "", Search = "", Value = "", SortField = "ID_INTEGRATION", Direction = "1", FrmRowno = "1", ToRowno = "500000", Deleted = Deleted };
            List<DataIntLstModel> datalist = await common.PSP_COMMON_DAPPER<DataIntLstModel>("PSP_COMMON_LIST", CommandType.StoredProcedure, param) ?? new List<DataIntLstModel>();

            var param2 = new { Table = "PVIEW_DATA_INT_RESULT", TableID = "", Search = "", Value = "", SortField = "ID_INT_H", Direction = "0", FrmRowno = "1", ToRowno = "500000", Deleted = Deleted };
            List<DataIntLogModel> datalog = await common.PSP_COMMON_DAPPER<DataIntLogModel>("PSP_COMMON_LIST", CommandType.StoredProcedure, param2) ?? new List<DataIntLogModel>();

            model.DataIntLstModel = datalist;
            model.DataIntLogModel = datalog;

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> RunIntegration(DataIntModel model)
        {
            bool success = true;
            string message = "";
            string prodlinemsg = "";

            string idh = "0";

            var modelgrade = new GradeDataModel();

            if (model.DataIntLstModel == null || model.DataIntLstModel.Count < 1)
            {
                success = false;
                message += "No integration selected";
            }

            if (success)
            {
                foreach (var item in model.DataIntLstModel)
                {
                    int int_id = item.ID_INTEGRATION;
                    string int_type = item.INT_TYPE;

                    #region RAW DATA
                    if (int_id == 1)
                    {
                        DataTable prodline = await dbdal.getProdLine();

                        if (prodline != null && prodline.Rows.Count > 0)
                        {
                            for (int i = 0; i < prodline.Rows.Count; i++)
                            {
                                string pline = prodline.Rows[i]["PRODLINENO"].ToString();

                                DataTable grade = await dbdal.getMonthlyGradeData(pline);

                                if (grade != null && grade.Rows.Count > 0)
                                {
                                    for (int j = 0; j < grade.Rows.Count; j++)
                                    {
                                        modelgrade.PROD_CODE = grade.Rows[j]["PROD_CODE"].ToString();
                                        modelgrade.PROD_DESC = grade.Rows[j]["PROD_DESC"].ToString();
                                        modelgrade.GRADE = grade.Rows[j]["GRADE"].ToString();
                                        modelgrade.LOT_NO = grade.Rows[j]["LOT_NO"].ToString();
                                        modelgrade.PACK_DATE = grade.Rows[j]["PACK_DATE"].ToString();
                                        modelgrade.PACK_QTY = grade.Rows[j]["PACK_QTY"].ToString();
                                        modelgrade.TAG_NO = grade.Rows[j]["TAG_NO"].ToString();


                                        if (int.TryParse(idh, out int num3) && idh != "0")  // already has idh
                                        {
                                            string result2 = await dbdal.setIntRawDataMaint(Convert.ToInt32(idh), modelgrade, pline);
                                            if (!(int.TryParse(result2, out int num2) && result2 != "0"))
                                            {
                                                success = false;
                                                message += result2;
                                            }
                                        }
                                        else // set header fist
                                        {
                                            idh = await dbdal.setIntRawData_H(int_type);
                                            if (!(int.TryParse(idh, out int num) && idh != "0"))
                                            {
                                                success = false;
                                                message += idh;
                                            }

                                            string result2 = await dbdal.setIntRawDataMaint(Convert.ToInt32(idh), modelgrade, pline);
                                            if (!(int.TryParse(result2, out int num2) && result2 != "0"))
                                            {
                                                success = false;
                                                message += result2;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (prodlinemsg == "")
                                    {
                                        prodlinemsg += "Data has been successfully integrated for Raw Data Integration but fail to get grade data detail for below product line:<br>" + pline + "<br>";
                                    }
                                    else
                                    {
                                        prodlinemsg += pline + "<br>";
                                    }

                                }
                            }

                        }
                        else
                        {
                            success = false;
                            message += "Fail to get existing product line.";
                        }
                    }

                    #endregion

                    #region MACHINE DATA
                    else if (int_id == 2)
                    {
                        DataTable machineDataTable = await dbdal.GetMachinePathData();
                        if (machineDataTable != null)
                        {
                            foreach (DataRow row in machineDataTable.Rows)
                            {
                                // Create Run/Backup folders and move csv files to RUN folder                   
                                DirectoryInitializer(row["cvspath"].ToString(), out string destination, out string backup);

                                if (Directory.Exists(destination))
                                {
                                    // Loop "moved" csv files in run folder
                                    DirectoryInfo directoryInfo = new DirectoryInfo(destination);
                                    foreach (FileInfo excelFile in directoryInfo.GetFiles("*.csv"))
                                    {
                                        // Get current row and column data
                                        GetRowAndColumnDataSch(row, excelFile, destination, out List<string> rowData, out string colData);

                                        // Integrate data from machine
                                        for (int i = 0; i < rowData.Count(); i++)
                                        {

                                            if (int.TryParse(idh, out int num3) && idh != "0")  // already has idh
                                            {
                                                string result2 = await dbdal.IntegrateMachineData(Convert.ToInt32(idh), colData, rowData[i].ToString(), row["MACHINENAME"].ToString(), row["PROPERTIES"].ToString(), row["PROPITEM"].ToString(), "RunIntegration()");
                                                if (!(int.TryParse(result2, out int num2) && result2 != "0"))
                                                {
                                                    success = false;
                                                    message += result2;
                                                }
                                            }
                                            else // set header fist
                                            {
                                                idh = await dbdal.setIntRawData_H(int_type);
                                                if (!(int.TryParse(idh, out int num) && idh != "0"))
                                                {
                                                    success = false;
                                                    message += idh;
                                                }

                                                string result2 = await dbdal.IntegrateMachineData(Convert.ToInt32(idh), colData, rowData[i].ToString(), row["MACHINENAME"].ToString(), row["PROPERTIES"].ToString(), row["PROPITEM"].ToString(), "RunIntegration()");
                                                if (!(int.TryParse(result2, out int num2) && result2 != "0"))
                                                {
                                                    success = false;
                                                    message += result2;
                                                }
                                            }

                                        }
                                        // Backup and remove excel file
                                        DirectoryFinalizer(destination, backup, excelFile);
                                    }
                                }
                            }
                        }
                        else
                        {
                            success = false;
                            message += "No machine path registered.";
                        }

                    }
                    #endregion

                    #region FINAL DATA
                    else if (int_id == 3)
                    {
                        try
                        {
                            // Get IDS data 
                            DataTable idsDataTable = await dbdal.GetIDSData();
                            if (idsDataTable != null)
                            {
                                DataTable idsDataTable2 = idsDataTable.Clone();

                                //lehaadd
                                foreach (DataRow row in idsDataTable.Rows)
                                {
                                    if (row["STATUS"].ToString() == "Completed")
                                    {
                                        idsDataTable2.ImportRow(row);
                                    }
                                }
                                //lehaaddend

                                //string prodtype = "";
                                //string packeddate = "";
                                //Boolean skip = false, first = false, firstC = false;
                                //List<string> date = new List<string>();

                                ////filter data before integration
                                //foreach (DataRow row in idsDataTable.Rows)
                                //{
                                //    first = false;
                                //    skip = false;
                                //    if (prodtype == "")
                                //    {
                                //        first = true;
                                //        prodtype = row["PRODTYPE"].ToString();
                                //        packeddate = row["PACKEDDATE"].ToString();
                                //    }
                                //    if ((row["PRODTYPE"].ToString() != prodtype && prodtype != "") || (row["PACKEDDATE"].ToString() != packeddate && packeddate != ""))
                                //    {
                                //        first = true;
                                //        firstC = false;
                                //        skip = false;
                                //        prodtype = row["PRODTYPE"].ToString();
                                //        packeddate = row["PACKEDDATE"].ToString();
                                //    }
                                //    if (skip == true)
                                //    {
                                //        continue;
                                //    }
                                //    if (first == true && row["STATUS"].ToString() == "Completed")
                                //    {
                                //        firstC = true;
                                //    }
                                //    if (row["PRODTYPE"].ToString() == prodtype && row["STATUS"].ToString() != "Completed" && firstC == false)
                                //    {
                                //        skip = true;
                                //        continue;
                                //    }
                                //    else if (row["PRODTYPE"].ToString() == prodtype && row["STATUS"].ToString() == "Completed" && firstC == true)
                                //    {
                                //        idsDataTable2.ImportRow(row);
                                //    }
                                //    if (row["STATUS"].ToString() == "Incomplete" && first == false && firstC == true)
                                //    {
                                //        date.Add(row["PACKEDDATE"].ToString() + "," + row["PRODTYPE"].ToString());
                                //    }
                                //}


                                ////remove row have same date of incomplete subsequent row
                                //for (int i = 0; i < date.Count; i++)
                                //{
                                //    for (int j = 0; j < idsDataTable2.Rows.Count; j++)
                                //    {
                                //        DataRow dr = idsDataTable2.Rows[j];
                                //        if (dr["PACKEDDATE"].ToString() == date[i].Split(',')[0] && dr["PRODTYPE"].ToString() == date[i].Split(',')[1])
                                //        {
                                //            dr.Delete();
                                //        }
                                //    }
                                //    idsDataTable2.AcceptChanges();
                                //}

                                foreach (DataRow row in idsDataTable2.Rows)
                                {
                                    int qty = int.Parse(row["QUANTITY"].ToString());
                                    int ID_IDS_H = int.Parse(row["ID_IDS_H"].ToString());
                                    int TRANSACTION_ID_HDR = await dbdal.SaveHeaderData(row);

                                    // check if successfully inserted
                                    if (TRANSACTION_ID_HDR == -1)
                                    {
                                        continue;
                                    }

                                    // YI Data
                                    DataTable yiDataTable = await dbdal.GetYIData(ID_IDS_H, row["grade"].ToString(), row["lot"].ToString());
                                    int yiSeq = 200000;
                                    if (yiDataTable != null)
                                    {
                                        foreach (DataRow yiDataRow in yiDataTable.Rows)
                                        {
                                            yiSeq += 1;
                                            await dbdal.SaveYIData(yiDataRow, TRANSACTION_ID_HDR, yiSeq, qty);
                                        }
                                    }

                                    // BS Data
                                    DataTable bsDataTable = await dbdal.GetBSData(ID_IDS_H, row["grade"].ToString(), row["lot"].ToString());
                                    int bsSeq = 300000;
                                    if (bsDataTable != null)
                                    {
                                        foreach (DataRow coqDataRow in bsDataTable.Rows)
                                        {
                                            bsSeq += 1;
                                            await dbdal.SaveBSData(coqDataRow, TRANSACTION_ID_HDR, bsSeq, qty);
                                        }
                                    }

                                    // COQ Data
                                    DataTable coqDataTable = await dbdal.GetCOQData(ID_IDS_H);
                                    int coqSeq = 400000;
                                    if (coqDataTable != null)
                                    {
                                        foreach (DataRow coqDataRow in coqDataTable.Rows)
                                        {
                                            coqSeq += 1;
                                            await dbdal.SaveCOQData(coqDataRow, TRANSACTION_ID_HDR, coqSeq);
                                        }
                                    }

                                    // Update IDS_H table status to 1
                                    await dbdal.IntegrateFinalData(ID_IDS_H);

                                    //NISA - TO SET HEADER IN INT LOG
                                    idh = await dbdal.setIntRawData_H(int_type);
                                    if (!(int.TryParse(idh, out int num) && idh != "0"))
                                    {
                                        success = false;
                                        message += idh;
                                    }
                                }                           

                                ViewBag.Result = "Data successfully integrated";
                            
                            } else
                            {
                                success = false;
                                message = "No data to integrate.";
                            }
                        }
                        catch (Exception ex)
                        {
                            success = false;
                            message += ex.Message;
                            // Console.WriteLine(ex.Message);
                            //ViewBag.Result = "Error occured";
                        }
                    }
                    #endregion

                    #region EXTERNAL FINAL DATA
                    else if (int_id == 4)
                    {
                        try
                        {
                            // Get IDS data 
                            DataTable idsDataTable = await dbdal.GetExtIDSData();

                            foreach (DataRow row in idsDataTable.Rows)
                            {
                                int qty = int.Parse(row["QUANTITY"].ToString());
                                int ID_EXT_IDS_H = int.Parse(row["ID_EXT_IDS_H"].ToString());
                                int TRANSACTION_ID_HDR = await dbdal.SaveExtHeaderData(row);

                                // check if successfully inserted
                                if (TRANSACTION_ID_HDR == -1)
                                {
                                    continue;
                                }

                                // YI Data
                                DataTable yiDataTable = await dbdal.GetEXTYIData(ID_EXT_IDS_H, row["GRADE"].ToString(), row["lot"].ToString());
                                int yiSeq = 200000;
                                foreach (DataRow yiDataRow in yiDataTable.Rows)
                                {
                                    yiSeq += 1;
                                    await dbdal.SaveEXTYIData(yiDataRow, TRANSACTION_ID_HDR, yiSeq, qty);
                                }

                                // BS Data
                                DataTable bsDataTable = await dbdal.GetEXTBSData(ID_EXT_IDS_H, row["GRADE"].ToString(), row["lot"].ToString());
                                int bsSeq = 300000;
                                foreach (DataRow bsDataRow in bsDataTable.Rows)
                                {
                                    bsSeq += 1;
                                    await dbdal.SaveEXTBSData(bsDataRow, TRANSACTION_ID_HDR, bsSeq, qty);
                                }

                                // Update IDS_H table status to 1
                                await dbdal.IntegrateExtFinalData(ID_EXT_IDS_H);

                                //NISA - TO SET HEADER IN INT LOG
                                idh = await dbdal.setIntRawData_H(int_type);
                                if (!(int.TryParse(idh, out int num) && idh != "0"))
                                {
                                    success = false;
                                    message += idh;
                                }
                            }

                            ViewBag.Result = "Data successfully integrated";
                        }
                        catch (Exception ex)
                        {
                            success = false;
                            message += ex.Message;
                            // Console.WriteLine(ex.Message);
                            //ViewBag.Result = "Error occured";
                        }
                    }
                    #endregion

                    #region COA GRN DATA
                    else if (int_id == 5)
                    {
                        DataTable dtungradedOra = await dbdal.getUngradedGRN();

                        if (dtungradedOra != null && dtungradedOra.Rows.Count > 0)
                        {
                            for (int i = 0; i < dtungradedOra.Rows.Count; i++)
                            {
                                COAGRNDETAIL oraungraded = new COAGRNDETAIL();

                                oraungraded.GRN_DATE = Convert.ToDateTime(dtungradedOra.Rows[i]["GRN Date"].ToString());
                                oraungraded.ITEM_CODE = dtungradedOra.Rows[i]["Item Code"].ToString().Trim();
                                oraungraded.ITEM_DESC = dtungradedOra.Rows[i]["Item Description"].ToString().Trim();
                                oraungraded.VENDOR_LOTNO = dtungradedOra.Rows[i]["Vendor Lot Number"].ToString().Trim();
                                oraungraded.LOTQTY = dtungradedOra.Rows[i]["Lot Quantity"].ToString().Trim();
                                oraungraded.UOM = dtungradedOra.Rows[i]["UOM"].ToString().Trim();
                                oraungraded.ORGANIZATION = dtungradedOra.Rows[i]["Organization"].ToString().Trim();
                                oraungraded.last = i == (dtungradedOra.Rows.Count - 1) ? "Y" : "N";

                                if (int.TryParse(idh, out int num3) && idh != "0")  // already has idh
                                {
                                    string result2 = await dbdal.setIntGRNDataMaint(Convert.ToInt32(idh), oraungraded);
                                    if (!(int.TryParse(result2, out int num2) && result2 != "0"))
                                    {
                                        success = false;
                                        message += result2;
                                    }
                                }
                                else // set header fist
                                {
                                    idh = await dbdal.setIntRawData_H(int_type);
                                    if (!(int.TryParse(idh, out int num) && idh != "0"))
                                    {
                                        success = false;
                                        message += idh;
                                    }

                                    string result2 = await dbdal.setIntGRNDataMaint(Convert.ToInt32(idh), oraungraded);
                                    if (!(int.TryParse(result2, out int num2) && result2 != "0"))
                                    {
                                        success = false;
                                        message += result2;
                                    }

                                }
                            }
                        }
                    }

                    #endregion

                    #region W GRADE
                    else if (int_id == 6)
                    {
                        DataTable dtwgradeplOra = await dbdal.getWGradePL();

                        if (dtwgradeplOra != null && dtwgradeplOra.Rows.Count > 0)
                        {
                            idh = await dbdal.setIntRawData_H(int_type);
                            if (!(int.TryParse(idh, out int num) && idh != "0"))
                            {
                                success = false;
                                message += idh;
                            }

                            string result2 = await dbdal.setIntWGradePLDataMaint(Convert.ToInt32(idh), dtwgradeplOra);
                            if (!(int.TryParse(result2, out int num2) && result2 != "0"))
                            {
                                success = false;
                                message += result2;
                            }
                        }
                    }

                    #endregion
                }
            }

            var data = new { success = success, message = message, prodlinemsg = prodlinemsg };

            return Json(data);
        }


        public ActionResult RawDataIntegrationSch()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> RawDataIntegrationSch(string data)
        {
            bool success = true;
            string message = "";
            string prodlinemsg = "";

            string idh = "0";

            var modelgrade = new GradeDataModel();
            try
            {
                DataTable prodline = await dbdal.getProdLine();

                if (prodline != null && prodline.Rows.Count > 0)
                {
                    for (int i = 0; i < prodline.Rows.Count; i++)
                    {
                        string pline = prodline.Rows[i]["PRODLINENO"].ToString();

                        DataTable grade = await dbdal.getMonthlyGradeData(pline);

                        if (grade != null && grade.Rows.Count > 0)
                        {
                            for (int j = 0; j < grade.Rows.Count; j++)
                            {
                                modelgrade.PROD_CODE = grade.Rows[j]["PROD_CODE"].ToString();
                                modelgrade.PROD_DESC = grade.Rows[j]["PROD_DESC"].ToString();
                                modelgrade.GRADE = grade.Rows[j]["GRADE"].ToString();
                                modelgrade.LOT_NO = grade.Rows[j]["LOT_NO"].ToString();
                                modelgrade.PACK_DATE = grade.Rows[j]["PACK_DATE"].ToString();
                                modelgrade.PACK_QTY = grade.Rows[j]["PACK_QTY"].ToString();
                                modelgrade.TAG_NO = grade.Rows[j]["TAG_NO"].ToString();

                                string result2 = await dbdal.setIntRawDataMaint(Convert.ToInt32(idh), modelgrade, pline);
                                if (!(int.TryParse(result2, out int num2) && result2 != "0"))
                                {
                                    success = false;
                                    message += result2;
                                }
                            }
                        }
                        else
                        {
                            if (prodlinemsg == "")
                            {
                                prodlinemsg += "Data has been successfully integrated for Raw Data Integration but fail to get grade data detail for below product line:<br>" + pline + "<br>";
                            }
                            else
                            {
                                prodlinemsg += pline + "<br>";
                            }

                        }
                    }

                    if (success)
                    {
                        ViewBag.Result = "Data successfully integrated";
                    } else
                    {
                        ViewBag.Result = prodlinemsg;
                    }
                    
                }
                else
                {
                    success = false;
                    message += "Fail to get existing product line.";
                    ViewBag.Result = message;//"Data successfully integrated";
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.Result = "Error occured";
            }

            return Content(@"<script>window.open('', '_top');top.window.close();</script>", "text/html");
        }

        public ActionResult MachineDataIntegrationSch()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> MachineDataIntegrationSch(string data)
        {
            bool success = true;
            string message = "";

            string idh = "0";
            try
            {
                DataTable machineDataTable = await dbdal.GetMachinePathData();
                foreach (DataRow row in machineDataTable.Rows)
                {
                    // Create Run/Backup folders and move csv files to RUN folder                   
                    DirectoryInitializer(row["cvspath"].ToString(), out string destination, out string backup);

                    if (Directory.Exists(destination))
                    {
                        // Loop "moved" csv files in run folder
                        DirectoryInfo directoryInfo = new DirectoryInfo(destination);
                        foreach (FileInfo excelFile in directoryInfo.GetFiles("*.csv"))
                        {
                            // Get current row and column data
                            GetRowAndColumnDataSch(row, excelFile, destination, out List<string> rowData, out string colData);

                            // Integrate data from machine
                            for (int i = 0; i < rowData.Count(); i++)
                            {

                                string result2 = await dbdal.IntegrateMachineData(Convert.ToInt32(idh), colData, rowData[i].ToString(), row["MACHINENAME"].ToString(), row["PROPERTIES"].ToString(), row["PROPITEM"].ToString(), "MachineDataIntegrationSch()");
                                if (!(int.TryParse(result2, out int num2) && result2 != "0"))
                                {
                                    success = false;
                                    message += result2;
                                }

                            }
                            // Backup and remove excel file
                            DirectoryFinalizer(destination, backup, excelFile);
                        }
                    }
                }

                if (success)
                {
                    ViewBag.Result = "Data successfully integrated";
                } else
                {
                    ViewBag.Result = message;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.Result = "Error occured";
            }

            return Content(@"<script>window.open('', '_top');top.window.close();</script>", "text/html");
        }

        public ActionResult FinalDataIntegrationSch()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> FinalDataIntegrationSch(string data)
        {
            try
            {
                // Get IDS data 
                DataTable idsDataTable = await dbdal.GetIDSData();
                DataTable oralistlotno = await dbdal.GetListLotno();
                if (idsDataTable != null)
                {
                    DataTable idsDataTable2 = idsDataTable.Clone();

                    // Create a HashSet for faster lookup of existing LOTNOs
                    HashSet<string> existingLotnos = new HashSet<string>(
                        oralistlotno.Rows.Cast<DataRow>()
                                         .Select(r => r["TPM_LOT_NUMBER"].ToString())
                    );

                    //lehaadd
                    foreach (DataRow row in idsDataTable.Rows)
                    {
                        string lotno = row["LOTNO"].ToString();
                        if (row["STATUS"].ToString() == "Completed" && !existingLotnos.Contains(lotno))
                        {

                            idsDataTable2.ImportRow(row);
                        }
                    }
                    //lehaaddend
                    // leha comment end


                    //    string prodtype = "";
                    //Boolean skip = false, first = false, firstC = true;
                    //List<string> date = new List<string>();

                    ////filter data before integration

                    //foreach (DataRow row in idsDataTable.Rows)
                    //{
                    //    if (row["STATUS"].ToString() == "REJECTED") continue;
                    //    first = false;
                    //    if (prodtype == "")
                    //    {
                    //        first = true;
                    //        prodtype = row["PRODTYPE"].ToString();
                    //    }
                    //    if (row["PRODTYPE"].ToString() != prodtype && prodtype != "")
                    //    {
                    //        first = true;
                    //        firstC = false;
                    //        skip = false;
                    //        prodtype = row["PRODTYPE"].ToString();
                    //    }
                    //    if (skip == true)
                    //    {
                    //        continue;
                    //    }
                    //    if (first == true && row["STATUS"].ToString() == "Completed")
                    //    {
                    //        firstC = true;
                    //    }
                    //    if (row["PRODTYPE"].ToString() == prodtype && row["STATUS"].ToString() != "Completed" && firstC == false)
                    //    {
                    //        skip = true;
                    //        continue;
                    //    }
                    //    else if (row["PRODTYPE"].ToString() == prodtype && row["STATUS"].ToString() == "Completed" && firstC == true)
                    //    {
                    //        idsDataTable2.ImportRow(row);
                    //    }
                    //    if (row["STATUS"].ToString() == "Incomplete" && first == false && firstC == true)
                    //    {
                    //        date.Add(row["PACKEDDATE"].ToString());
                    //    }
                    //}

                    ////remove row have same date of incomplete subsequent row
                    //for (int i = 0; i < date.Count; i++)
                    //{
                    //    for (int j = 0; j < idsDataTable2.Rows.Count; j++)
                    //    {
                    //        DataRow dr = idsDataTable2.Rows[j];
                    //        if (dr["PACKEDDATE"].ToString() == date[i])
                    //        {
                    //            dr.Delete();
                    //        }
                    //    }
                    //    idsDataTable2.AcceptChanges();
                    //}

                    // leha comment end

                    foreach (DataRow row in idsDataTable2.Rows)
                    {
                        int qty = int.Parse(row["QUANTITY"].ToString());
                        int ID_IDS_H = int.Parse(row["ID_IDS_H"].ToString());
                        int TRANSACTION_ID_HDR = await dbdal.SaveHeaderData(row);

                        // check if successfully inserted
                        if (TRANSACTION_ID_HDR == -1)
                        {
                            continue;
                        }

                        // YI Data
                        DataTable yiDataTable = await dbdal.GetYIData(ID_IDS_H, row["grade"].ToString(), row["lot"].ToString());
                        int yiSeq = 200000;
                        if (yiDataTable != null)
                        {
                            foreach (DataRow yiDataRow in yiDataTable.Rows)
                            {

                                yiSeq += 1;
                                await dbdal.SaveYIData(yiDataRow, TRANSACTION_ID_HDR, yiSeq, qty);
                            }
                        }



                        // BS Data
                        DataTable bsDataTable = await dbdal.GetBSData(ID_IDS_H, row["grade"].ToString(), row["lot"].ToString());
                        int bsSeq = 300000;
                        if (bsDataTable != null)
                        {
                            foreach (DataRow coqDataRow in bsDataTable.Rows)
                            {

                                bsSeq += 1;
                                await dbdal.SaveBSData(coqDataRow, TRANSACTION_ID_HDR, bsSeq, qty);
                            }
                        }



                        // COQ Data
                        DataTable coqDataTable = await dbdal.GetCOQData(ID_IDS_H);
                        int coqSeq = 400000;
                        if (coqDataTable != null)
                        {
                            foreach (DataRow coqDataRow in coqDataTable.Rows)
                            {

                                coqSeq += 1;
                                await dbdal.SaveCOQData(coqDataRow, TRANSACTION_ID_HDR, coqSeq);
                            }
                        }



                        // Update IDS_H table status to 1
                        await dbdal.IntegrateFinalData(ID_IDS_H);
                    }

                    ViewBag.Result = "Data successfully integrated";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.Result = "Error occured";
            }

            return Content(@"<script>window.open('', '_top');top.window.close();</script>", "text/html");
        }

        #region EXTERNAL FINAL DATA
        public ActionResult ExtFinalDataIntegrationSch()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ExtFinalDataIntegrationSch(string data)
        {
            try
            {
                // Get IDS data 
                DataTable idsDataTable = await dbdal.GetExtIDSData();                

                foreach (DataRow row in idsDataTable.Rows)
                {
                    int qty = int.Parse(row["QUANTITY"].ToString());
                    int ID_EXT_IDS_H = int.Parse(row["ID_EXT_IDS_H"].ToString());
                    int TRANSACTION_ID_HDR = await dbdal.SaveExtHeaderData(row);

                    // check if successfully inserted
                    if (TRANSACTION_ID_HDR == -1)
                    {
                        continue;
                    }

                    // YI Data
                    DataTable yiDataTable = await dbdal.GetEXTYIData(ID_EXT_IDS_H, row["GRADE"].ToString(), row["lot"].ToString());
                    int yiSeq = 200000;
                    foreach (DataRow yiDataRow in yiDataTable.Rows)
                    {
                        yiSeq += 1;
                        await dbdal.SaveEXTYIData(yiDataRow, TRANSACTION_ID_HDR, yiSeq, qty);
                    }

                    // BS Data
                    DataTable bsDataTable = await dbdal.GetEXTBSData(ID_EXT_IDS_H, row["GRADE"].ToString(), row["lot"].ToString());
                    int bsSeq = 300000;
                    foreach (DataRow bsDataRow in bsDataTable.Rows)
                    {
                        bsSeq += 1;
                        await dbdal.SaveEXTBSData(bsDataRow, TRANSACTION_ID_HDR, bsSeq, qty);
                    }                   

                    // Update IDS_H table status to 1
                    await dbdal.IntegrateExtFinalData(ID_EXT_IDS_H);
                }

                ViewBag.Result = "Data successfully integrated";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.Result = "Error occured";
            }

            return Content(@"<script>window.open('', '_top');top.window.close();</script>", "text/html");
        }
        #endregion

        #region COA UNGRADEDGRN

        public ActionResult GRNIntegrationSch()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<ActionResult> GRNIntegrationSch(string data)
        {
            bool success = true;
            string message = "";

            string idh = "0";

            try
            {
                DataTable dtungradedOra = await dbdal.getUngradedGRN();

                if (dtungradedOra != null && dtungradedOra.Rows.Count > 0)
                {
                    for (int i = 0; i < dtungradedOra.Rows.Count; i++)
                    {
                        COAGRNDETAIL oraungraded = new COAGRNDETAIL();

                        oraungraded.GRN_DATE = Convert.ToDateTime(dtungradedOra.Rows[i]["GRN Date"].ToString());
                        oraungraded.ITEM_CODE = dtungradedOra.Rows[i]["Item Code"].ToString().Trim();
                        oraungraded.ITEM_DESC = dtungradedOra.Rows[i]["Item Description"].ToString().Trim();
                        oraungraded.VENDOR_LOTNO = dtungradedOra.Rows[i]["Vendor Lot Number"].ToString().Trim();
                        oraungraded.LOTQTY = dtungradedOra.Rows[i]["Lot Quantity"].ToString().Trim();
                        oraungraded.UOM = dtungradedOra.Rows[i]["UOM"].ToString().Trim();
                        oraungraded.ORGANIZATION = dtungradedOra.Rows[i]["Organization"].ToString().Trim();
                        oraungraded.last = i == (dtungradedOra.Rows.Count - 1) ? "Y" : "N";

                        string result2 =  await dbdal.setIntGRNDataMaint(Convert.ToInt32(idh), oraungraded);
                        if (!(int.TryParse(result2, out int num2) && result2 != "0"))
                        {
                            success = false;
                            message += result2;
                        }
                    }
                }
                
                if (success)
                {
                    ViewBag.Result = "Data successfully integrated";
                }
                else
                {
                    ViewBag.Result = "Failed to integrate GRN";
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.Result = "Error occured";
            }

            return Content(@"<script>window.open('', '_top');top.window.close();</script>", "text/html");
        }

        #endregion

        #region W Grade PL Scheduler

        public ActionResult WGradeIntegrationSch()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> WGradeIntegrationSch(string data)
        {
            bool success = true;
            string message = "";

            string idh = "0";

            try
            {
                DataTable dtwgradeplOra = await dbdal.getWGradePL();

                if (dtwgradeplOra != null && dtwgradeplOra.Rows.Count > 0)
                {
                    string result2 = await dbdal.setIntWGradePLDataMaint(Convert.ToInt32(idh), dtwgradeplOra);
                    if (!(int.TryParse(result2, out int num2) && result2 != "0"))
                    {
                        success = false;
                        message += result2;
                    }
                }

                if (success)
                {
                    ViewBag.Result = "Data successfully integrated";
                }
                else
                {
                    ViewBag.Result = "Failed to integrate W Grade";
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.Result = "Error occured";
            }

            return Content(@"<script>window.open('', '_top');top.window.close();</script>", "text/html");
        }

        #endregion

        /// <summary>
        /// Intialize directory
        /// </summary>
        /// <param name="path"></param>
        /// <param name="destination"></param>
        /// <param name="backup"></param>
        public void DirectoryInitializer(string path, out string destination, out string backup)
        {
            destination = "";
            backup = "";

            // check if directory exist
            if (Directory.Exists(path))
            {
                // Create Directory Info and path string
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                destination = path + "\\Run";
                backup = path + "\\Backup";

                // Check if Run and Backup directory exist. Create them if not exist
                if (Directory.Exists(destination) == false) { Directory.CreateDirectory(destination); }
                if (Directory.Exists(backup) == false) { Directory.CreateDirectory(backup); }

                try
                {
                    foreach (FileInfo file in directoryInfo.GetFiles("*.csv"))
                    {
                        file.MoveTo(Path.Combine(destination, file.Name));
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
            }
        }

        /// <summary>
        /// Finalized backup files
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="backup"></param>
        /// <param name="file"></param>
        public void DirectoryFinalizer(string destination, string backup, FileInfo file)
        {
            // Create Path
            string savePath = Path.Combine(backup, file.Name);
            string deletePath = Path.Combine(destination, file.Name);

            // Move to backup location if not exist, else delete current file
            if (System.IO.File.Exists(savePath) == false)
            {
                file.MoveTo(savePath);
            }
            else
            {
                file.Delete();
            }
        }

        /// <summary>
        /// Used to read excel file and get row and column data
        /// </summary>
        /// <param name="dataRow">Data Row from data table in monthly data</param>
        /// <param name="excelFile">The excel file to read</param>
        /// <param name="destination">Path Location of Excel File</param>
        /// <param name="rowData">Output data of rows (Values)</param>
        /// <param name="colData">Output data of columns (Column Names)</param>
        public void GetRowAndColumnDataSch(DataRow dataRow, FileInfo excelFile, string destination, out List<string> rowData, out string colData)
        {
            // Set default value as empty
            string rowD = "";
            rowData = new List<string>();
            colData = "";
            string prop = dataRow["PROPERTIES"].ToString();

            try
            {
                // Intialize data
                string fileName = excelFile.Name;
                string cellRow = "";
                List<List<string>> stringlistlist = new List<List<string>>();
                StreamReader reader = new StreamReader(destination + "\\" + fileName);

                // Loop reader and add to text
                while (reader.Peek() != -1)
                {
                    List<string> cellList = new List<string>();
                    cellRow = reader.ReadLine();
                    //Split cell
                    cellList = cellRow.Split(',').ToList();
                    if (prop == "YI")
                    {
                        if (cellList.Count > 1)
                        {
                            if (cellList[1] != "" && cellList[2] != "")
                            {
                                stringlistlist.Add(cellList);
                            }
                        }
                        else
                        {
                            //nisa - add logic for exclude input file 
                            break;
                            //reader.Close();
                        }

                    }
                    else stringlistlist.Add(cellList);
                }
                reader.Close();

                // Check if more than 1 cell
                if (cellRow.Contains(",") == false)
                {
                    // If less than one cell, skip process
                    return;
                }

                for (int j = 0; j < stringlistlist.Count(); j++)
                {
                    // Loop columns in dataRow
                    for (int i = 0; i < dataRow.ItemArray.Length; i++)
                    {
                        // Check if column containts the word column
                        if (dataRow[i].ToString().Contains("Column"))
                        {
                            // Get column number - from reading
                            int colNum = int.Parse(dataRow[i].ToString().Substring(6));

                            // Loop cellList and add to data if number is same

                            for (int k = 0; k < stringlistlist[j].Count(); k++) //KL Ong 20220215 K change to j
                            {
                                if (colNum == k)
                                {
                                    if (j == 0)
                                    {
                                        colData += dataRow.Table.Columns[i].ColumnName + ",";
                                    }

                                    rowD += "'" + stringlistlist[j][k] + "',";
                                }
                            }

                        }
                    }
                    rowData.Add(rowD.Remove(rowD.Length - 1, 1));
                    rowD = "";
                }

                colData = colData.Remove(colData.Length - 1, 1);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("StartIndex cannot be less than zero"))
                {
                    Console.WriteLine(ex.Message);
                }
                else
                {
                    Console.WriteLine(ex.Message);
                }

            }
        }

    }
}