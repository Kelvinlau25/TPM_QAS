using DBModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TPM_QAS.Filters;
using TPM_QAS.Helpers;
using TPM_QAS.DAL;
using TPM_QAS.Models;
using System.IO;
using ClosedXML.Excel;
using OfficeOpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

namespace TPM_QAS.Controllers
{
    public class MM_PROPERTIESController : Controller
    {
        DB dbmain = new DB();
        MM_PROPERTIES_DAL dbdal = new MM_PROPERTIES_DAL();
        Ora o = new Ora();

        #region LIST

        [SessionExpire]
        public async Task<ActionResult> MM_PROPERTIES_LST(string Deleted = "0")
        {
            TempData["STATUS_DDL"] = await dbmain.PSP_COMMON_DDL("CMN_LST_STATUS", "", "", "", "", "", "");

            CommonFunction common = new CommonFunction();
            var param = new { Table = "PVIEW_MM_PROPERTIES", TableID = "", Search = "", Value = "", SortField = "MM_PROPERTIES_H_ID", Direction = "0", FrmRowno = "1", ToRowno = "500000", Deleted = Deleted };
            List<PropertiesModel> model = await common.PSP_COMMON_DAPPER<PropertiesModel>("PSP_COMMON_LIST", CommandType.StoredProcedure, param) ?? new List<PropertiesModel>();

            ViewBag.Deleted = Deleted;
            return View(model);
        }

        #endregion

        #region VIEW/EDIT

        [SessionExpire]
        public async Task<ActionResult> MM_PROPERTIES_DETAIL(string id)
        {
            ViewBag.Tittle = "Properties";
            var model = new PropertiesModel();

            List<PropertiesMachineLstModel> listItemsAdd = new List<PropertiesMachineLstModel>();

            DataTable dt = await dbdal.getProperties_Data(id, "H"); //get header only

            if (dt != null && dt.Rows.Count > 0)
            {
                model.MM_PROPERTIES_H_ID = Convert.ToInt32(dt.Rows[0]["MM_PROPERTIES_H_ID"]);
                model.PROPERTIES = dt.Rows[0]["PROPERTIES"] != DBNull.Value ? dt.Rows[0]["PROPERTIES"].ToString() : "";
                model.PROP_ITEM = dt.Rows[0]["PROP_ITEM"] != DBNull.Value ? dt.Rows[0]["PROP_ITEM"].ToString() : "";
                model.REF_METHOD = dt.Rows[0]["REF_METHOD"] != DBNull.Value ? dt.Rows[0]["REF_METHOD"].ToString() : "";
                model.REMARKS = dt.Rows[0]["REMARKS"] != DBNull.Value ? dt.Rows[0]["REMARKS"].ToString() : "";
                model.OraColumn = dt.Rows[0]["ORA_COLUMN"] != DBNull.Value ? dt.Rows[0]["ORA_COLUMN"].ToString() : "";

                model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";
            }

            DataTable dtl = await dbdal.getProperties_Data(id, "D"); //get d data
            if (dtl != null && dtl.Rows.Count > 0)
            {
                for (int i = 0; i < dtl.Rows.Count; i++)
                {
                    PropertiesMachineLstModel infoObjAdd = new PropertiesMachineLstModel();

                    infoObjAdd.MM_PROPERTIES_D_ID = Convert.ToInt32(dtl.Rows[i]["MM_PROPERTIES_D_ID"]);
                    infoObjAdd.MM_PROPERTIES_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_PROPERTIES_H_ID"]);
                    infoObjAdd.MACHINE_NAME = dtl.Rows[i]["MACHINE_NAME"] != DBNull.Value ? dtl.Rows[i]["MACHINE_NAME"].ToString() : "";
                    infoObjAdd.MACHINE_PATH = dtl.Rows[i]["MACHINE_PATH"] != DBNull.Value ? dtl.Rows[i]["MACHINE_PATH"].ToString() : "";
                    infoObjAdd.TESTING_TIME = dtl.Rows[i]["TESTING_TIME"] != DBNull.Value ? dtl.Rows[i]["TESTING_TIME"].ToString() : "";
                    infoObjAdd.LOT_NO = dtl.Rows[i]["LOT_NO"] != DBNull.Value ? dtl.Rows[i]["LOT_NO"].ToString() : "";
                    infoObjAdd.OPERATOR = dtl.Rows[i]["OPERATOR"] != DBNull.Value ? dtl.Rows[i]["OPERATOR"].ToString() : "";
                    infoObjAdd.READING_1 = dtl.Rows[i]["READING_1"] != DBNull.Value ? dtl.Rows[i]["READING_1"].ToString() : "";
                    infoObjAdd.READING_2 = dtl.Rows[i]["READING_2"] != DBNull.Value ? dtl.Rows[i]["READING_2"].ToString() : "";
                    infoObjAdd.READING_3 = dtl.Rows[i]["READING_3"] != DBNull.Value ? dtl.Rows[i]["READING_3"].ToString() : "";
                    infoObjAdd.READING_4 = dtl.Rows[i]["READING_4"] != DBNull.Value ? dtl.Rows[i]["READING_4"].ToString() : "";
                    infoObjAdd.READING_5 = dtl.Rows[i]["READING_5"] != DBNull.Value ? dtl.Rows[i]["READING_5"].ToString() : "";
                    infoObjAdd.READING_6 = dtl.Rows[i]["READING_6"] != DBNull.Value ? dtl.Rows[i]["READING_6"].ToString() : "";

                    if(dtl.Rows[i]["TTL_READING"] != null && dtl.Rows[i]["TTL_READING"].ToString() != "")
                    {
                        infoObjAdd.TTL_READING = Convert.ToDecimal(dtl.Rows[i]["TTL_READING"]);
                    }
                    else
                    {
                        infoObjAdd.TTL_READING = 0;
                    }

                    infoObjAdd.AVERAGE = dtl.Rows[i]["AVERAGE"] != DBNull.Value ? dtl.Rows[i]["AVERAGE"].ToString() : "";

                    infoObjAdd.RECORD_TYP = dtl.Rows[i]["RECORD_TYP"] != DBNull.Value ? dtl.Rows[i]["RECORD_TYP"].ToString() : "";
                    infoObjAdd.CREATED_BY = dtl.Rows[i]["CREATED_BY"] != DBNull.Value ? dtl.Rows[i]["CREATED_BY"].ToString() : "";
                    infoObjAdd.CREATED_DATE = dtl.Rows[i]["CREATED_DATE"] != DBNull.Value ? dtl.Rows[i]["CREATED_DATE"].ToString() : "";
                    infoObjAdd.CREATED_LOC = dtl.Rows[i]["CREATED_LOC"] != DBNull.Value ? dtl.Rows[i]["CREATED_LOC"].ToString() : "";
                    infoObjAdd.UPDATED_BY = dtl.Rows[i]["UPDATED_BY"] != DBNull.Value ? dtl.Rows[i]["UPDATED_BY"].ToString() : "";
                    infoObjAdd.UPDATED_DATE = dtl.Rows[i]["UPDATED_DATE"] != DBNull.Value ? dtl.Rows[i]["UPDATED_DATE"].ToString() : "";
                    infoObjAdd.UPDATED_LOC = dtl.Rows[i]["UPDATED_LOC"] != DBNull.Value ? dtl.Rows[i]["UPDATED_LOC"].ToString() : "";

                    // Adding.
                    listItemsAdd.Add(infoObjAdd);

                }

            }

            model.PropertiesMachineLstModel = listItemsAdd;
            model.DropdownColumn = await LoadDummyColumn();
            model.DropdownOraColumn = await LoadOraColumnName();

            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MM_PROPERTIES_DETAIL(string ActionType, PropertiesModel model)
        {
            ViewBag.Tittle = "Properties";
            try
            {
                if (ActionType == "postback")
                {
                    ModelState.Clear();
                }
                if (ActionType == "Back")
                {
                    ModelState.Clear();
                    return RedirectToAction("MM_PROPERTIES_LST", "MM_PROPERTIES");
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> InsertUpdateProperties(PropertiesModel model)
        {
            bool success = true;
            string message = "";


            if (ModelState.IsValid)
            {
                if (model.PROPERTIES == null || model.PROPERTIES == "" ||
                    model.PROP_ITEM == null || model.PROP_ITEM == "")
                {
                    success = false;
                    message += "Required field cannot empty.";
                }

                if (success)
                {
                    model.REF_METHOD = model.REF_METHOD != null ? model.REF_METHOD.ToString() : "";
                    model.REMARKS = model.REMARKS != null ? model.REMARKS.ToString() : "";
                    model.OraColumn = model.OraColumn != null ? model.OraColumn.ToString() : "";

                    model.SEQUENCE = 0;
                    // insert h
                    string result1 = await dbdal.Properties_H_Maint(model);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                    else
                    {
                        if(model.PropertiesMachineLstModel != null && model.PropertiesMachineLstModel.Count > 0)
                        {
                            // insert d
                            foreach (var item in model.PropertiesMachineLstModel)
                            {
                                string machinename = item.MACHINE_NAME != null ? item.MACHINE_NAME.ToString() : "";
                                string path = item.MACHINE_PATH != null ? item.MACHINE_PATH.ToString() : "";
                                string testtime = item.TESTING_TIME != null ? item.TESTING_TIME.ToString() : "";
                                string lotno = item.LOT_NO != null ? item.LOT_NO.ToString() : "";
                                string operatorclmn = item.OPERATOR != null ? item.OPERATOR.ToString() : "";
                                string r1 = item.READING_1 != null ? item.READING_1.ToString() : "";
                                string r2 = item.READING_2 != null ? item.READING_2.ToString() : "";
                                string r3 = item.READING_3 != null ? item.READING_3.ToString() : "";
                                string r4 = item.READING_4 != null ? item.READING_4.ToString() : "";
                                string r5 = item.READING_5 != null ? item.READING_5.ToString() : "";
                                string r6 = item.READING_6 != null ? item.READING_6.ToString() : "";
                                decimal total = Convert.ToDecimal(item.TTL_READING);
                                string avg = item.AVERAGE != null ? item.AVERAGE.ToString() : "";

                                string rectype = item.RECORD_TYP != null ? item.RECORD_TYP.ToString() : "";

                                if (rectype == "0")
                                {
                                    rectype = "3";
                                }

                                string result2 = await dbdal.Properties_D_Maint(item.MM_PROPERTIES_D_ID, Convert.ToInt32(result1), machinename, path, testtime, lotno, operatorclmn,
                                                                            r1, r2, r3, r4, r5, r6,
                                                                            total, avg, rectype);
                                if (!(int.TryParse(result2, out int num2) && result2 != "0"))
                                {
                                    success = false;
                                    message += result2;
                                }

                            }
                        }
                    }
                }
                else
                {
                    success = false;
                    message += "Error in saving data.";
                }
            }
            else
            {
                success = false;
                message += "Error in saving data : Modal State not valid.";
            }

            var data = new { success = success, message = message };

            return Json(data);
        }

        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> UpdatePropertiesHeader(PropertiesModel model)
        {
            bool success = true;
            string message = "";


            if (ModelState.IsValid)
            {
                if (model.PROPERTIES == null || model.PROPERTIES == "" ||
                    model.PROP_ITEM == null || model.PROP_ITEM == "")
                {
                    success = false;
                    message += "Required field cannot empty.";
                }

                if (success)
                {
                    model.REF_METHOD = model.REF_METHOD != null ? model.REF_METHOD.ToString() : "";
                    model.REMARKS = model.REMARKS != null ? model.REMARKS.ToString() : "";
                    model.OraColumn = model.OraColumn != null ? model.OraColumn.ToString() : "";

                    model.SEQUENCE = 0;

                    // insert h
                    string result1 = await dbdal.Properties_H_Maint(model);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                    else
                    {
                        if (model.PropertiesMachineLstModel != null && model.PropertiesMachineLstModel.Count > 0)
                        {
                            // insert d
                            foreach (var item in model.PropertiesMachineLstModel)
                            {
                                string machinename = item.MACHINE_NAME != null ? item.MACHINE_NAME.ToString() : "";
                                
                                string rectype = item.RECORD_TYP != null ? item.RECORD_TYP.ToString() : "";

                                //only update existing row
                                if (rectype == "0")
                                {
                                    string result2 = await dbdal.Properties_D_Maint(item.MM_PROPERTIES_D_ID, Convert.ToInt32(result1), machinename, "", "", "", "",
                                                                             "", "", "", "", "", "",
                                                                             0, "", "4");
                                    if (!(int.TryParse(result2, out int num2) && result2 != "0"))
                                    {
                                        success = false;
                                        message += result2;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    success = false;
                    message += "Error in saving data.";
                }
            }
            else
            {
                success = false;
                message += "Error in saving data : Modal State not valid.";
            }

            var data = new { success = success, message = message };

            return Json(data);
        }

        #endregion

        #region ADD

        [SessionExpire]
        public async Task<ActionResult> MM_PROPERTIES_ADD(string id = "")
        {
            ViewBag.Tittle = "Properties";
            var model = new PropertiesModel();

            List<PropertiesMachineLstModel> listItemsAdd = new List<PropertiesMachineLstModel>();

            if (!string.IsNullOrWhiteSpace(id) && CommonMethod.isNumeric(id))
            {
                DataTable dt = await dbdal.getProperties_Data(id, "H"); //get header data

                if (dt != null && dt.Rows.Count > 0)
                {
                    model.MM_PROPERTIES_H_ID = Convert.ToInt32(dt.Rows[0]["MM_PROPERTIES_H_ID"]);
                    model.PROPERTIES = dt.Rows[0]["PROPERTIES"] != DBNull.Value ? dt.Rows[0]["PROPERTIES"].ToString() : "";
                    model.PROP_ITEM = dt.Rows[0]["PROP_ITEM"] != DBNull.Value ? dt.Rows[0]["PROP_ITEM"].ToString() : "";
                    model.REF_METHOD = dt.Rows[0]["REF_METHOD"] != DBNull.Value ? dt.Rows[0]["REF_METHOD"].ToString() : "";
                    model.REMARKS = dt.Rows[0]["REMARKS"] != DBNull.Value ? dt.Rows[0]["REMARKS"].ToString() : "";
                    model.OraColumn = dt.Rows[0]["ORA_COLUMN"] != DBNull.Value ? dt.Rows[0]["ORA_COLUMN"].ToString() : "";

                    model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                    model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                    model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                    model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                    model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                    model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                    model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";

                    DataTable dtl = await dbdal.getProperties_Data(id, "D"); //get d data
                    if (dtl != null && dtl.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtl.Rows.Count; i++)
                        {
                            PropertiesMachineLstModel infoObjAdd = new PropertiesMachineLstModel();

                            infoObjAdd.MM_PROPERTIES_D_ID = Convert.ToInt32(dtl.Rows[i]["MM_PROPERTIES_D_ID"]);
                            infoObjAdd.MM_PROPERTIES_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_PROPERTIES_H_ID"]);
                            infoObjAdd.MACHINE_NAME = dtl.Rows[i]["MACHINE_NAME"] != DBNull.Value ? dtl.Rows[i]["MACHINE_NAME"].ToString() : "";
                            infoObjAdd.MACHINE_PATH = dtl.Rows[i]["MACHINE_PATH"] != DBNull.Value ? dtl.Rows[i]["MACHINE_PATH"].ToString() : "";
                            infoObjAdd.TESTING_TIME = dtl.Rows[i]["TESTING_TIME"] != DBNull.Value ? dtl.Rows[i]["TESTING_TIME"].ToString() : "";
                            infoObjAdd.LOT_NO = dtl.Rows[i]["LOT_NO"] != DBNull.Value ? dtl.Rows[i]["LOT_NO"].ToString() : "";
                            infoObjAdd.OPERATOR = dtl.Rows[i]["OPERATOR"] != DBNull.Value ? dtl.Rows[i]["OPERATOR"].ToString() : "";
                            infoObjAdd.READING_1 = dtl.Rows[i]["READING_1"] != DBNull.Value ? dtl.Rows[i]["READING_1"].ToString() : "";
                            infoObjAdd.READING_2 = dtl.Rows[i]["READING_2"] != DBNull.Value ? dtl.Rows[i]["READING_2"].ToString() : "";
                            infoObjAdd.READING_3 = dtl.Rows[i]["READING_3"] != DBNull.Value ? dtl.Rows[i]["READING_3"].ToString() : "";
                            infoObjAdd.READING_4 = dtl.Rows[i]["READING_4"] != DBNull.Value ? dtl.Rows[i]["READING_4"].ToString() : "";
                            infoObjAdd.READING_5 = dtl.Rows[i]["READING_5"] != DBNull.Value ? dtl.Rows[i]["READING_5"].ToString() : "";
                            infoObjAdd.READING_6 = dtl.Rows[i]["READING_6"] != DBNull.Value ? dtl.Rows[i]["READING_6"].ToString() : "";

                            if (dtl.Rows[i]["TTL_READING"] != null && dtl.Rows[i]["TTL_READING"].ToString() != "")
                            {
                                infoObjAdd.TTL_READING = Convert.ToDecimal(dtl.Rows[i]["TTL_READING"]);
                            }
                            else
                            {
                                infoObjAdd.TTL_READING = 0;
                            }
                            infoObjAdd.AVERAGE = dtl.Rows[i]["AVERAGE"] != DBNull.Value ? dtl.Rows[i]["AVERAGE"].ToString() : "";

                            infoObjAdd.RECORD_TYP = dtl.Rows[i]["RECORD_TYP"] != DBNull.Value ? dtl.Rows[i]["RECORD_TYP"].ToString() : "";
                            infoObjAdd.CREATED_BY = dtl.Rows[i]["CREATED_BY"] != DBNull.Value ? dtl.Rows[i]["CREATED_BY"].ToString() : "";
                            infoObjAdd.CREATED_DATE = dtl.Rows[i]["CREATED_DATE"] != DBNull.Value ? dtl.Rows[i]["CREATED_DATE"].ToString() : "";
                            infoObjAdd.CREATED_LOC = dtl.Rows[i]["CREATED_LOC"] != DBNull.Value ? dtl.Rows[i]["CREATED_LOC"].ToString() : "";
                            infoObjAdd.UPDATED_BY = dtl.Rows[i]["UPDATED_BY"] != DBNull.Value ? dtl.Rows[i]["UPDATED_BY"].ToString() : "";
                            infoObjAdd.UPDATED_DATE = dtl.Rows[i]["UPDATED_DATE"] != DBNull.Value ? dtl.Rows[i]["UPDATED_DATE"].ToString() : "";
                            infoObjAdd.UPDATED_LOC = dtl.Rows[i]["UPDATED_LOC"] != DBNull.Value ? dtl.Rows[i]["UPDATED_LOC"].ToString() : "";

                            // Adding.
                            listItemsAdd.Add(infoObjAdd);

                        }
                    }
                }
            }
            else
            {
                model = new PropertiesModel();
            }

            model.PropertiesMachineLstModel = listItemsAdd;
            model.DropdownColumn = await LoadDummyColumn();
            model.DropdownOraColumn = await LoadOraColumnName();
            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MM_PROPERTIES_ADD(string ActionType, PropertiesModel model)
        {
            ViewBag.Tittle = "Properties";
            try
            {
                if (ActionType == "postback")
                {
                    ModelState.Clear();
                }
                if (ActionType == "Back")
                {
                    ModelState.Clear();
                    return RedirectToAction("MM_PROPERTIES_LST", "MM_PROPERTIES");
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> DraftProperties(PropertiesModel model)
        {
            bool success = true;
            string message = "";


            if (ModelState.IsValid)
            {
                if (model.PROPERTIES == null || model.PROPERTIES == "" ||
                    model.PROP_ITEM == null || model.PROP_ITEM == "")
                {
                    success = false;
                    message += "Required field cannot empty.";
                }

                if (success)
                {
                    // insert h
                    model.REF_METHOD = model.REF_METHOD != null ? model.REF_METHOD.ToString() : "";
                    model.REMARKS = model.REMARKS != null ? model.REMARKS.ToString() : "";
                    model.OraColumn = model.OraColumn != null ? model.OraColumn.ToString() : "";

                    model.SEQUENCE = 0;
                    string result1 = await dbdal.Properties_H_Maint(model);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                    else
                    {
                        if (model.PropertiesMachineLstModel != null && model.PropertiesMachineLstModel.Count > 0)
                        {
                            // insert d
                            foreach (var item in model.PropertiesMachineLstModel)
                            {
                                string machinename = item.MACHINE_NAME != null ? item.MACHINE_NAME.ToString() : "";
                                string path = item.MACHINE_PATH != null ? item.MACHINE_PATH.ToString() : "";
                                string testtime = item.TESTING_TIME != null ? item.TESTING_TIME.ToString() : "";
                                string lotno = item.LOT_NO != null ? item.LOT_NO.ToString() : "";
                                string operatorclmn = item.OPERATOR != null ? item.OPERATOR.ToString() : "";
                                string r1 = item.READING_1 != null ? item.READING_1.ToString() : "";
                                string r2 = item.READING_2 != null ? item.READING_2.ToString() : "";
                                string r3 = item.READING_3 != null ? item.READING_3.ToString() : "";
                                string r4 = item.READING_4 != null ? item.READING_4.ToString() : "";
                                string r5 = item.READING_5 != null ? item.READING_5.ToString() : "";
                                string r6 = item.READING_6 != null ? item.READING_6.ToString() : "";
                                decimal total = Convert.ToDecimal(item.TTL_READING);
                                string avg = item.AVERAGE != null ? item.AVERAGE.ToString() : "";

                                string rectype = item.RECORD_TYP != null ? item.RECORD_TYP.ToString() : "";

                                string result2 = await dbdal.Properties_D_Maint(item.MM_PROPERTIES_D_ID, Convert.ToInt32(result1), machinename, path, testtime, lotno, operatorclmn,
                                                                            r1, r2, r3, r4, r5, r6,
                                                                            total, avg, rectype);
                                if (!(int.TryParse(result2, out int num2) && result2 != "0"))
                                {
                                    success = false;
                                    message += result2;
                                }

                            }
                        }

                    }
                }
                else
                {
                    success = false;
                    message += "Error in saving data.";
                }
            }
            else
            {
                success = false;
                message += "Error in saving data : Modal State not valid.";
            }

            var data = new { success = success, message = message };

            return Json(data);
        }

        #endregion

        #region IDS REPORT SEQ

        [SessionExpire]
        public async Task<ActionResult> MM_PROPERTIES_REPORT_SEQ()
        {
            
            ViewBag.Tittle = "IDS Report Sequence";
            var model = new PropertiesModel();

            List<TPMSeqLstModel> listItemsAdd1 = new List<TPMSeqLstModel>();
            List<CompSeqLstModel> listItemsAdd2 = new List<CompSeqLstModel>();

            //D1
            DataTable dtl = await dbdal.getProperties_Seq("TPM"); //get d data
            if (dtl != null && dtl.Rows.Count > 0)
            {
                for (int i = 0; i < dtl.Rows.Count; i++)
                {
                    TPMSeqLstModel infoObjAdd = new TPMSeqLstModel();

                    infoObjAdd.MM_PROPERTIES_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_PROPERTIES_H_ID"]);
                    infoObjAdd.PROPERTIES = dtl.Rows[i]["PROPERTIES"] != DBNull.Value ? dtl.Rows[i]["PROPERTIES"].ToString() : "";
                    infoObjAdd.PROP_ITEM = dtl.Rows[i]["PROP_ITEM"] != DBNull.Value ? dtl.Rows[i]["PROP_ITEM"].ToString() : "";
                    infoObjAdd.TPM_SEQUENCE = Convert.ToInt32(dtl.Rows[i]["TPM_SEQUENCE"]);

                    // Adding.
                    listItemsAdd1.Add(infoObjAdd);

                }
            }

            //D2
            DataTable dtl2 = await dbdal.getProperties_Seq("COMP"); //get d data
            if (dtl2 != null && dtl2.Rows.Count > 0)
            {
                for (int i = 0; i < dtl2.Rows.Count; i++)
                {
                    CompSeqLstModel infoObjAdd = new CompSeqLstModel();

                    infoObjAdd.MM_PROPERTIES_H_ID = Convert.ToInt32(dtl2.Rows[i]["MM_PROPERTIES_H_ID"]);
                    infoObjAdd.PROPERTIES = dtl2.Rows[i]["PROPERTIES"] != DBNull.Value ? dtl2.Rows[i]["PROPERTIES"].ToString() : "";
                    infoObjAdd.PROP_ITEM = dtl2.Rows[i]["PROP_ITEM"] != DBNull.Value ? dtl2.Rows[i]["PROP_ITEM"].ToString() : "";
                    infoObjAdd.COMP_SEQUENCE = Convert.ToInt32(dtl2.Rows[i]["COMP_SEQUENCE"]);
                    
                    // Adding.
                    listItemsAdd2.Add(infoObjAdd);

                }
            }


            model.TPMSeqLstModel = listItemsAdd1;
            model.CompSeqLstModel = listItemsAdd2;
            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> UpdateReportSeq(PropertiesModel model)
        {
            bool success = true;
            string message = "";


            if (ModelState.IsValid)
            {
                if (model != null && model.TPMSeqLstModel.Count > 0 && model.CompSeqLstModel.Count > 0)
                {

                    foreach (var item in model.TPMSeqLstModel)
                    {
                        item.RECORD_TYP = "6";

                        string result2 = await dbdal.Properties_H_SEQ_Maint(item.MM_PROPERTIES_H_ID, item.PROPERTIES, item.PROP_ITEM, item.TPM_SEQUENCE,item.RECORD_TYP);
                        if (!(int.TryParse(result2, out int num2) && result2 != "0"))
                        {
                            success = false;
                            message += result2;
                        }
                    }

                    foreach (var item in model.CompSeqLstModel)
                    {
                        item.RECORD_TYP = "7";

                        string result2 = await dbdal.Properties_H_SEQ_Maint(item.MM_PROPERTIES_H_ID, item.PROPERTIES, item.PROP_ITEM, item.COMP_SEQUENCE, item.RECORD_TYP);
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
                success = false;
                message += "Error in saving data : Modal State not valid.";
            }

            var data = new { success = success, message = message };

            return Json(data);
        }

        #endregion

        #region DELETE

        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> deleteProperties(List<string> lstid)
        {
            bool success = true;
            string message = "";

            PropertiesModel model = new PropertiesModel();

            if (lstid.Count < 1)
            {
                success = false;
                message += "Please select a record first to delete.";
            }

            if (success && lstid.Count > 0)
            {
                foreach (var item in lstid)
                {
                    model.MM_PROPERTIES_H_ID = Convert.ToInt32(item);
                    model.RECORD_TYP = "5";

                    model.PROPERTIES = "";
                    model.PROP_ITEM = "";
                    model.REF_METHOD = "";
                    model.REMARKS = "";
                    model.OraColumn = "";
                    model.SEQUENCE = 0;

                    string resultd = await dbdal.Properties_H_Maint(model);
                    if (!(int.TryParse(resultd, out int num2) && resultd != "0"))
                    {
                        success = false;
                        message += resultd;
                    }
                    else
                    {
                        //delete d
                        string result2 = await dbdal.Properties_D_Maint(0, Convert.ToInt32(item), "", "", "", "", "",
                                                                            "", "", "", "", "", "",
                                                                            0, "", "2");
                        if (!(int.TryParse(result2, out int num3)))
                        {
                            success = false;
                            message += result2;
                        }
                    }
                }
            }

            var data = new { success = success, message = message };

            return Json(data);
        }

        #endregion

        #region AUDIT

        [SessionExpire]
        public async Task<ActionResult> MM_PROPERTIES_AUDIT(string id)
        {
            TempData["SQ_ID"] = id;
            CommonFunction common = new CommonFunction();
            var param = new { pTable = "PVIEW_MM_PROPERTIES_A", pKeyValue = id };
            List<AuditTrailModel> AuditTrailModel = await common.PSP_COMMON_DAPPER<AuditTrailModel>("PSP_COMMON_AUDIT", CommandType.StoredProcedure, param) ?? new List<AuditTrailModel>();

            return View(AuditTrailModel);
        }

        #endregion

        #region DDL

        private async Task<List<SelectListItem>> LoadOraColumnName()
        {
            List<SelectListItem> listItems = new List<SelectListItem>();

            DataTable dt = await o.OraColumnsName();
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    SelectListItem infoObj = new SelectListItem();
                    infoObj.Text = dt.Rows[i]["column_name"].ToString();
                    infoObj.Value = dt.Rows[i]["column_name"].ToString();

                    listItems.Add(infoObj);
                }
            }
            return listItems;
        }

        private async Task<List<SelectListItem>> LoadDummyColumn()
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            listItems.Add(new SelectListItem { Text = "Select an option..", Value = "" });

            for (int i = 0; i < 40; i++)
            {
                SelectListItem infoObj = new SelectListItem();
                infoObj.Text = "Column " + i.ToString();
                infoObj.Value = "Column " + i.ToString();
                listItems.Add(infoObj);
            }

            return listItems;
        }

        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> getColumnDDL(string path)
        {
            bool success = true;
            string message = "";

            List<SelectListItem> listItems = new List<SelectListItem>();
            List<CSVFields> listCSVFields = new List<CSVFields>();

            if (path != null && path != "")
            {
                if (Directory.Exists(path))
                {

                    var csvFiles = Directory.GetFiles(path, "*.csv");

                    if (csvFiles.Length > 0)
                    {
                        // Order the files by last write time and get the latest one
                        var latestCsvFile = csvFiles
                            .Select(f => new FileInfo(f))
                            .OrderByDescending(f => f.LastWriteTime)
                            .First();

                        string filename = latestCsvFile.FullName.ToString();

                        string extension = System.IO.Path.GetExtension(filename).ToLower();
                        if (extension == ".csv")
                        {
                            try
                            {
                                using (var reader = new StreamReader(filename))
                                {
                                    bool firstRow = true;
                                    while (!reader.EndOfStream)
                                    {
                                        var line = reader.ReadLine();
                                        var values = line.Split(',');

                                        if (values.Length > 1)
                                        {
                                            //column ddl
                                            listItems.Add(new SelectListItem { Text = "Select an option..", Value = "" }); ;
                                            for (int i = 0; i < values.Length; i++)
                                            {
                                                SelectListItem infoObj = new SelectListItem();
                                                infoObj.Text = "Column " + i.ToString();
                                                infoObj.Value = "Column " + i.ToString();
                                                listItems.Add(infoObj);
                                            }

                                            if (firstRow)
                                            {
                                                for (int i = 0; i < values.Length; i++)
                                                {
                                                    CSVFields infoObj2 = new CSVFields();
                                                    infoObj2.COLUMNS = "Column " + i.ToString();
                                                    ; infoObj2.VALUE = values[i];
                                                    listCSVFields.Add(infoObj2);
                                                }
                                                firstRow = false;
                                            }
                                        }
                                        else
                                        {
                                            success = false;
                                            message = "CSV file must have more than 1 column of data.";
                                            break;
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                success = false;
                                message = "Error reading Excel file: " + ex.Message;

                            }
                        }
                        else
                        {
                            success = false;
                            message = "Can only read file with extension of .csv";
                        }

                    }
                    else
                    {
                        success = false;
                        message = "No CSV files found in the path given.";
                    }

                    
                }
                else
                {
                    success = false;
                    message = "Path not found.";
                }
            }
            else
            {
                success = false;
                message = "Invalid Machine Path.";
            }

            var data = new { success = success, message = message, columnList = listItems, fieldvalue = listCSVFields };

            return Json(data);
        }

        #endregion
    }


}