using DBModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using TPM_QAS.DAL;
using TPM_QAS.Models;
using TPM_QAS.Filters;
using TPM_QAS.Helpers;

namespace TPM_QAS.Controllers
{
    public class MM_GRADINGController : Controller
    {
        DB dbmain = new DB();
        MM_GRADING_DAL dbdal = new MM_GRADING_DAL();

        #region LIST

        [SessionExpire]
        public async Task<ActionResult> MM_GRADING_LST(string Deleted = "0")
        {
            TempData["STATUS_DDL"] = await dbmain.PSP_COMMON_DDL("CMN_LST_STATUS", "", "", "", "", "", "");

            CommonFunction common = new CommonFunction();
            var param = new { Table = "PVIEW_MM_GRADING", TableID = "", Search = "", Value = "", SortField = "MM_GRADING_H_ID", Direction = "0", FrmRowno = "1", ToRowno = "500000", Deleted = Deleted };
            List<GradingModel> model = await common.PSP_COMMON_DAPPER<GradingModel>("PSP_COMMON_LIST", CommandType.StoredProcedure, param) ?? new List<GradingModel>();

            ViewBag.Deleted = Deleted;
            return View(model);
        }

        #endregion

        #region VIEW/EDIT

        [SessionExpire]
        public async Task<ActionResult> MM_GRADING_DETAIL(string id)
        {
            ViewBag.Tittle = "Grading";
            var model = new GradingModel();

            List<GradingNormalLstModel> listItemsAdd1 = new List<GradingNormalLstModel>();
            List<GradingAppearanceLstModel> listItemsAdd2 = new List<GradingAppearanceLstModel>();
            List<GradingPassingLstModel> listItemsAdd3 = new List<GradingPassingLstModel>();
            model.DropdownItem = new List<SelectListItem>();

            DataTable dt = await dbdal.getGrading_Data(id, "H"); //get header data

            if (dt != null && dt.Rows.Count > 0)
            {
                model.MM_GRADING_H_ID = Convert.ToInt32(dt.Rows[0]["MM_GRADING_H_ID"]);
                model.MM_PRODTYPE_H_ID = Convert.ToInt32(dt.Rows[0]["MM_PRODTYPE_H_ID"]);
                model.PROD_TYPE = dt.Rows[0]["PROD_TYPE"] != DBNull.Value ? dt.Rows[0]["PROD_TYPE"].ToString() : "";
                model.COMP_GROUP = dt.Rows[0]["COMP_GROUP"] != DBNull.Value ? dt.Rows[0]["COMP_GROUP"].ToString() : "";
                model.PROD_LINE = dt.Rows[0]["PROD_LINE"] != DBNull.Value ? dt.Rows[0]["PROD_LINE"].ToString() : "";

                model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";
            }

            //D1
            DataTable dtl = await dbdal.getGrading_Data(id, "D1"); //get d data
            if (dtl != null && dtl.Rows.Count > 0)
            {
                for (int i = 0; i < dtl.Rows.Count; i++)
                {
                    GradingNormalLstModel infoObjAdd = new GradingNormalLstModel();

                    infoObjAdd.MM_GRADING_D1_ID = Convert.ToInt32(dtl.Rows[i]["MM_GRADING_D1_ID"]);
                    infoObjAdd.MM_GRADING_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_GRADING_H_ID"]);
                    infoObjAdd.PROPERTY = dtl.Rows[i]["PROPERTY"] != DBNull.Value ? dtl.Rows[i]["PROPERTY"].ToString() : "";
                    infoObjAdd.MM_PROPERTIES_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_PROPERTIES_H_ID"]);
                    infoObjAdd.PROP_ITEM = dtl.Rows[i]["PROP_ITEM"] != DBNull.Value ? dtl.Rows[i]["PROP_ITEM"].ToString() : "";
                    infoObjAdd.PRIORITY = Convert.ToInt32(dtl.Rows[i]["PRIORITY"]);
                    infoObjAdd.FINAL_PRIORITY = Convert.ToInt32(dtl.Rows[i]["FINAL_PRIORITY"]);
                    infoObjAdd.L_SPEC = Convert.ToDecimal(dtl.Rows[i]["L_SPEC"]);
                    infoObjAdd.U_SPEC = Convert.ToDecimal(dtl.Rows[i]["U_SPEC"]);
                    infoObjAdd.L_PCL = Convert.ToDecimal(dtl.Rows[i]["L_PCL"]);
                    infoObjAdd.U_PCL = Convert.ToDecimal(dtl.Rows[i]["U_PCL"]);
                    infoObjAdd.CENTRE_LINE = Convert.ToDecimal(dtl.Rows[i]["CENTRE_LINE"]);
                    infoObjAdd.GRADE = dtl.Rows[i]["GRADE"] != DBNull.Value ? dtl.Rows[i]["GRADE"].ToString() : "";
                    infoObjAdd.ROUNDING = Convert.ToInt32(dtl.Rows[i]["ROUNDING"]);

                    infoObjAdd.RECORD_TYP = dtl.Rows[i]["RECORD_TYP"] != DBNull.Value ? dtl.Rows[i]["RECORD_TYP"].ToString() : "";
                    infoObjAdd.CREATED_BY = dtl.Rows[i]["CREATED_BY"] != DBNull.Value ? dtl.Rows[i]["CREATED_BY"].ToString() : "";
                    infoObjAdd.CREATED_DATE = dtl.Rows[i]["CREATED_DATE"] != DBNull.Value ? dtl.Rows[i]["CREATED_DATE"].ToString() : "";
                    infoObjAdd.CREATED_LOC = dtl.Rows[i]["CREATED_LOC"] != DBNull.Value ? dtl.Rows[i]["CREATED_LOC"].ToString() : "";
                    infoObjAdd.UPDATED_BY = dtl.Rows[i]["UPDATED_BY"] != DBNull.Value ? dtl.Rows[i]["UPDATED_BY"].ToString() : "";
                    infoObjAdd.UPDATED_DATE = dtl.Rows[i]["UPDATED_DATE"] != DBNull.Value ? dtl.Rows[i]["UPDATED_DATE"].ToString() : "";
                    infoObjAdd.UPDATED_LOC = dtl.Rows[i]["UPDATED_LOC"] != DBNull.Value ? dtl.Rows[i]["UPDATED_LOC"].ToString() : "";

                    // Adding.
                    listItemsAdd1.Add(infoObjAdd);

                }
            }

            //D2
            DataTable dtl2 = await dbdal.getGrading_Data(id, "D2"); //get d data
            if (dtl2 != null && dtl2.Rows.Count > 0)
            {
                for (int i = 0; i < dtl2.Rows.Count; i++)
                {
                    GradingAppearanceLstModel infoObjAdd = new GradingAppearanceLstModel();

                    infoObjAdd.MM_GRADING_D2_ID = Convert.ToInt32(dtl2.Rows[i]["MM_GRADING_D2_ID"]);
                    infoObjAdd.MM_GRADING_H_ID = Convert.ToInt32(dtl2.Rows[i]["MM_GRADING_H_ID"]);
                    infoObjAdd.SECTION = dtl2.Rows[i]["SECTION"] != DBNull.Value ? dtl2.Rows[i]["SECTION"].ToString() : "";
                    infoObjAdd.FIELD_NAME = dtl2.Rows[i]["FIELD_NAME"] != DBNull.Value ? dtl2.Rows[i]["FIELD_NAME"].ToString() : "";
                    infoObjAdd.PRIORITY = Convert.ToInt32(dtl2.Rows[i]["PRIORITY"]);
                    infoObjAdd.FINAL_PRIORITY = Convert.ToInt32(dtl2.Rows[i]["FINAL_PRIORITY"]);
                    infoObjAdd.L_SPEC = Convert.ToDecimal(dtl2.Rows[i]["L_SPEC"]);
                    infoObjAdd.U_SPEC = Convert.ToDecimal(dtl2.Rows[i]["U_SPEC"]);
                    infoObjAdd.L_PCL = Convert.ToDecimal(dtl2.Rows[i]["L_PCL"]);
                    infoObjAdd.U_PCL = Convert.ToDecimal(dtl2.Rows[i]["U_PCL"]);
                    infoObjAdd.CALCULATION = Convert.ToDecimal(dtl2.Rows[i]["CALCULATION"]);
                    infoObjAdd.GRADE = dtl2.Rows[i]["GRADE"] != DBNull.Value ? dtl2.Rows[i]["GRADE"].ToString() : "";
                    infoObjAdd.ROUNDING = Convert.ToInt32(dtl2.Rows[i]["ROUNDING"]);

                    infoObjAdd.RECORD_TYP = dtl2.Rows[i]["RECORD_TYP"] != DBNull.Value ? dtl2.Rows[i]["RECORD_TYP"].ToString() : "";
                    infoObjAdd.CREATED_BY = dtl2.Rows[i]["CREATED_BY"] != DBNull.Value ? dtl2.Rows[i]["CREATED_BY"].ToString() : "";
                    infoObjAdd.CREATED_DATE = dtl2.Rows[i]["CREATED_DATE"] != DBNull.Value ? dtl2.Rows[i]["CREATED_DATE"].ToString() : "";
                    infoObjAdd.CREATED_LOC = dtl2.Rows[i]["CREATED_LOC"] != DBNull.Value ? dtl2.Rows[i]["CREATED_LOC"].ToString() : "";
                    infoObjAdd.UPDATED_BY = dtl2.Rows[i]["UPDATED_BY"] != DBNull.Value ? dtl2.Rows[i]["UPDATED_BY"].ToString() : "";
                    infoObjAdd.UPDATED_DATE = dtl2.Rows[i]["UPDATED_DATE"] != DBNull.Value ? dtl2.Rows[i]["UPDATED_DATE"].ToString() : "";
                    infoObjAdd.UPDATED_LOC = dtl2.Rows[i]["UPDATED_LOC"] != DBNull.Value ? dtl2.Rows[i]["UPDATED_LOC"].ToString() : "";

                    // Adding.
                    listItemsAdd2.Add(infoObjAdd);

                }
            }

            //D3
            DataTable dtl3 = await dbdal.getGrading_Data(id, "D3"); //get d data
            if (dtl3 != null && dtl3.Rows.Count > 0)
            {
                for (int i = 0; i < dtl3.Rows.Count; i++)
                {
                    GradingPassingLstModel infoObjAdd = new GradingPassingLstModel();

                    model.PROPERTIES = dtl3.Rows[0]["PROPERTIES"] != DBNull.Value ? dtl3.Rows[0]["PROPERTIES"].ToString() : "";
                    model.DropdownItem = await LoadDllData(0, model.PROPERTIES, "PROP_ITEM");

                    infoObjAdd.MM_GRADING_D3_ID = Convert.ToInt32(dtl3.Rows[i]["MM_GRADING_D3_ID"]);
                    infoObjAdd.MM_GRADING_H_ID = Convert.ToInt32(dtl3.Rows[i]["MM_GRADING_H_ID"]);
                    infoObjAdd.PROPERTIES = dtl3.Rows[i]["PROPERTIES"] != DBNull.Value ? dtl3.Rows[i]["PROPERTIES"].ToString() : "";
                    infoObjAdd.PROP_ITEM = dtl3.Rows[i]["PROP_ITEM"] != DBNull.Value ? dtl3.Rows[i]["PROP_ITEM"].ToString() : "";
                    infoObjAdd.PASS_GRADE = dtl3.Rows[i]["PASS_GRADE"] != DBNull.Value ? dtl3.Rows[i]["PASS_GRADE"].ToString() : "";

                    infoObjAdd.RECORD_TYP = dtl3.Rows[i]["RECORD_TYP"] != DBNull.Value ? dtl3.Rows[i]["RECORD_TYP"].ToString() : "";
                    infoObjAdd.CREATED_BY = dtl3.Rows[i]["CREATED_BY"] != DBNull.Value ? dtl3.Rows[i]["CREATED_BY"].ToString() : "";
                    infoObjAdd.CREATED_DATE = dtl3.Rows[i]["CREATED_DATE"] != DBNull.Value ? dtl3.Rows[i]["CREATED_DATE"].ToString() : "";
                    infoObjAdd.CREATED_LOC = dtl3.Rows[i]["CREATED_LOC"] != DBNull.Value ? dtl3.Rows[i]["CREATED_LOC"].ToString() : "";
                    infoObjAdd.UPDATED_BY = dtl3.Rows[i]["UPDATED_BY"] != DBNull.Value ? dtl3.Rows[i]["UPDATED_BY"].ToString() : "";
                    infoObjAdd.UPDATED_DATE = dtl3.Rows[i]["UPDATED_DATE"] != DBNull.Value ? dtl3.Rows[i]["UPDATED_DATE"].ToString() : "";
                    infoObjAdd.UPDATED_LOC = dtl3.Rows[i]["UPDATED_LOC"] != DBNull.Value ? dtl3.Rows[i]["UPDATED_LOC"].ToString() : "";

                    // Adding.
                    listItemsAdd3.Add(infoObjAdd);

                }
            }

            model.GradingNormalLstModel = listItemsAdd1;
            model.GradingAppearanceLstModel = listItemsAdd2;
            model.GradingPassingLstModel = listItemsAdd3;

            //model.DropdownProdType = await LoadDllData(0, "", "PROD_TYPE");
            if(model.COMP_GROUP != null && model.COMP_GROUP != "")
            {
                model.DropdownProdType = await LoadDllData(0, model.COMP_GROUP, "PROD_TYPE_COMP");
            }
            else
            {
                model.DropdownProdType = new List<SelectListItem>();
            }
            
            model.DropdownProperty = await LoadDllData(0, "", "PROPERTIES");

            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MM_GRADING_DETAIL(string ActionType, GradingModel model)
        {
            ViewBag.Tittle = "GRADING";
            try
            {
                if (ActionType == "postback")
                {
                    ModelState.Clear();
                }
                if (ActionType == "Back")
                {
                    ModelState.Clear();
                    return RedirectToAction("MM_GRADING_LST", "MM_GRADING");
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
        public async Task<ActionResult> InsertUpdateGrading(GradingModel model)
        {
            bool success = true;
            string message = "";

            if (ModelState.IsValid)
            {
                if (model.PROD_TYPE == null || model.PROD_TYPE == "" ||
                    model.COMP_GROUP == null || model.COMP_GROUP == "" ||
                    ((model.GradingNormalLstModel == null || model.GradingNormalLstModel.Count < 1) &&
                    (model.GradingAppearanceLstModel == null || model.GradingAppearanceLstModel.Count < 1) &&
                    (model.GradingPassingLstModel == null || model.GradingPassingLstModel.Count < 1)))
                {
                    success = false;
                    message += "Required field cannot empty.";
                }

                if (success)
                {
                    // insert h
                    model.PROD_LINE = model.PROD_LINE != null ? model.PROD_LINE.ToString() : "";
                    string result1 = await dbdal.Grading_H_Maint(model);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                    else
                    {
                        //delete existing d first
                        string result9 = await dbdal.Grading_D1_Maint(0, Convert.ToInt32(result1), 0, "", 0, 0, 0, 0, 0, 0, 0, 0, "", 0, "2");
                        if (!(int.TryParse(result9, out int num9)))
                        {
                            success = false;
                            message += result9;
                        }
                        else
                        {
                            // insert d1
                            if (model.GradingNormalLstModel != null && model.GradingNormalLstModel.Count > 0)
                            {
                                foreach (var item in model.GradingNormalLstModel)
                                {
                                    int seq = Convert.ToInt32(item.SEQUENCE);
                                    string property = item.PROPERTY != null ? item.PROPERTY.ToString() : "";
                                    int propitemID = Convert.ToInt32(item.MM_PROPERTIES_H_ID);
                                    string propitem = item.PROP_ITEM != null ? item.PROP_ITEM.ToString() : "";
                                    int priority = Convert.ToInt32(item.PRIORITY);
                                    int fullprior = Convert.ToInt32(item.FINAL_PRIORITY);
                                    decimal lspec = Convert.ToDecimal(item.L_SPEC);
                                    decimal uspec = Convert.ToDecimal(item.U_SPEC);
                                    decimal lpcl = Convert.ToDecimal(item.L_PCL);
                                    decimal upcl = Convert.ToDecimal(item.U_PCL);
                                    decimal centreline = Convert.ToDecimal(item.CENTRE_LINE);
                                    string grade = item.GRADE != null ? item.GRADE.ToString() : "";
                                    int rounding = Convert.ToInt32(item.ROUNDING);

                                    string rectype = item.RECORD_TYP != null ? item.RECORD_TYP.ToString() : "";

                                    if (rectype == "0")
                                    {
                                        rectype = "3";
                                    }

                                    string result2 = await dbdal.Grading_D1_Maint(item.MM_GRADING_D1_ID, Convert.ToInt32(result1), seq, property, propitemID, priority, fullprior, 
                                                                            lspec, uspec, lpcl, upcl, centreline, grade, rounding, rectype);
                                    if (!(int.TryParse(result2, out int num2) && result2 != "0"))
                                    {
                                        success = false;
                                        message += result2;
                                    }
                                }
                            }
                        }

                        //delete existing d first
                        string result8 = await dbdal.Grading_D2_Maint(0, Convert.ToInt32(result1),0, "", "", 0, 0, 0, 0, 0, 0, 0, "", 0, "2");
                        if (!(int.TryParse(result8, out int num8)))
                        {
                            success = false;
                            message += result8;
                        }
                        else
                        {
                            // insert d2
                            if (model.GradingAppearanceLstModel != null && model.GradingAppearanceLstModel.Count > 0)
                            {
                                foreach (var item in model.GradingAppearanceLstModel)
                                {
                                    int seq = Convert.ToInt32(item.SEQUENCE); 
                                    string section = item.SECTION != null ? item.SECTION.ToString() : "";
                                    string field = item.FIELD_NAME != null ? item.FIELD_NAME.ToString() : "";
                                    int priority = Convert.ToInt32(item.PRIORITY);
                                    int fullprior = Convert.ToInt32(item.FINAL_PRIORITY);
                                    decimal lspec = Convert.ToDecimal(item.L_SPEC);
                                    decimal uspec = Convert.ToDecimal(item.U_SPEC);
                                    decimal lpcl = Convert.ToDecimal(item.L_PCL);
                                    decimal upcl = Convert.ToDecimal(item.U_PCL);
                                    decimal calculation = Convert.ToDecimal(item.CALCULATION);
                                    string grade = item.GRADE != null ? item.GRADE.ToString() : "";
                                    int rounding = Convert.ToInt32(item.ROUNDING);

                                    string rectype = item.RECORD_TYP != null ? item.RECORD_TYP.ToString() : "";

                                    if (rectype == "0")
                                    {
                                        rectype = "3";
                                    }

                                    string result2 = await dbdal.Grading_D2_Maint(item.MM_GRADING_D2_ID, Convert.ToInt32(result1), seq, section, field, priority, fullprior, 
                                                                            lspec, uspec, lpcl, upcl, calculation, grade, rounding, rectype);
                                    if (!(int.TryParse(result2, out int num2) && result2 != "0"))
                                    {
                                        success = false;
                                        message += result2;
                                    }

                                }

                            }
                        }

                        //delete existing d first
                        string result7 = await dbdal.Grading_D3_Maint(0, Convert.ToInt32(result1), "", "", "", "2");
                        if (!(int.TryParse(result7, out int num7)))
                        {
                            success = false;
                            message += result7;
                        }
                        else
                        {
                            // insert d3
                            if (model.GradingPassingLstModel != null && model.GradingPassingLstModel.Count > 0)
                            {
                                foreach (var item in model.GradingPassingLstModel)
                                {
                                    string properties = item.PROPERTIES != null ? item.PROPERTIES.ToString() : "";
                                    string propitem = item.PROP_ITEM != null ? item.PROP_ITEM.ToString() : "";
                                    string passgrade = item.PASS_GRADE != null ? item.PASS_GRADE.ToString() : "";

                                    string rectype = item.RECORD_TYP != null ? item.RECORD_TYP.ToString() : "";

                                    if (rectype == "0")
                                    {
                                        rectype = "3";
                                    }

                                    string result2 = await dbdal.Grading_D3_Maint(item.MM_GRADING_D3_ID, Convert.ToInt32(result1), properties, propitem, passgrade, rectype);
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
                message += "Error in saving data : Modal State not valid. Please ensure all data has fill in. ";
            }

            var data = new { success = success, message = message };

            return Json(data);
        }

        #endregion

        #region ADD

        [SessionExpire]
        public async Task<ActionResult> MM_GRADING_ADD(string id = "")
        {
            ViewBag.Tittle = "Grading";
            var model = new GradingModel();
            List<GradingNormalLstModel> listItemsAdd1 = new List<GradingNormalLstModel>();
            List<GradingAppearanceLstModel> listItemsAdd2 = new List<GradingAppearanceLstModel>();
            List<GradingPassingLstModel> listItemsAdd3 = new List<GradingPassingLstModel>();

            model.DropdownItem = new List<SelectListItem>();

            if (!string.IsNullOrWhiteSpace(id) && CommonMethod.isNumeric(id))
            {
                DataTable dt = await dbdal.getGrading_Data(id, "H"); //get header data

                if (dt != null && dt.Rows.Count > 0)
                {
                    model.MM_GRADING_H_ID = Convert.ToInt32(dt.Rows[0]["MM_GRADING_H_ID"]);
                    model.MM_PRODTYPE_H_ID = Convert.ToInt32(dt.Rows[0]["MM_PRODTYPE_H_ID"]);
                    model.PROD_TYPE = dt.Rows[0]["PROD_TYPE"] != DBNull.Value ? dt.Rows[0]["PROD_TYPE"].ToString() : "";
                    model.COMP_GROUP = dt.Rows[0]["COMP_GROUP"] != DBNull.Value ? dt.Rows[0]["COMP_GROUP"].ToString() : "";
                    model.PROD_LINE = dt.Rows[0]["PROD_LINE"] != DBNull.Value ? dt.Rows[0]["PROD_LINE"].ToString() : "";

                    model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                    model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                    model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                    model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                    model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                    model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                    model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";

                    DataTable dtl = await dbdal.getGrading_Data(id, "D1"); //get d data
                    if (dtl != null && dtl.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtl.Rows.Count; i++)
                        {
                            GradingNormalLstModel infoObjAdd = new GradingNormalLstModel();

                            infoObjAdd.MM_GRADING_D1_ID = Convert.ToInt32(dtl.Rows[i]["MM_GRADING_D1_ID"]);
                            infoObjAdd.MM_GRADING_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_GRADING_H_ID"]);
                            infoObjAdd.PROPERTY = dtl.Rows[i]["PROPERTY"] != DBNull.Value ? dtl.Rows[i]["PROPERTY"].ToString() : "";
                            infoObjAdd.MM_PROPERTIES_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_PROPERTIES_H_ID"]);
                            infoObjAdd.PROP_ITEM = dtl.Rows[i]["PROP_ITEM"] != DBNull.Value ? dtl.Rows[i]["PROP_ITEM"].ToString() : "";
                            infoObjAdd.PRIORITY = Convert.ToInt32(dtl.Rows[i]["PRIORITY"]);
                            infoObjAdd.FINAL_PRIORITY = Convert.ToInt32(dtl.Rows[i]["FINAL_PRIORITY"]);
                            infoObjAdd.L_SPEC = Convert.ToDecimal(dtl.Rows[i]["L_SPEC"]);
                            infoObjAdd.U_SPEC = Convert.ToDecimal(dtl.Rows[i]["U_SPEC"]);
                            infoObjAdd.L_PCL = Convert.ToDecimal(dtl.Rows[i]["L_PCL"]);
                            infoObjAdd.U_PCL = Convert.ToDecimal(dtl.Rows[i]["U_PCL"]);
                            infoObjAdd.CENTRE_LINE = Convert.ToDecimal(dtl.Rows[i]["CENTRE_LINE"]);
                            infoObjAdd.GRADE = dtl.Rows[i]["GRADE"] != DBNull.Value ? dtl.Rows[i]["GRADE"].ToString() : "";
                            infoObjAdd.ROUNDING = Convert.ToInt32(dtl.Rows[i]["ROUNDING"]);

                            infoObjAdd.RECORD_TYP = dtl.Rows[i]["RECORD_TYP"] != DBNull.Value ? dtl.Rows[i]["RECORD_TYP"].ToString() : "";
                            infoObjAdd.CREATED_BY = dtl.Rows[i]["CREATED_BY"] != DBNull.Value ? dtl.Rows[i]["CREATED_BY"].ToString() : "";
                            infoObjAdd.CREATED_DATE = dtl.Rows[i]["CREATED_DATE"] != DBNull.Value ? dtl.Rows[i]["CREATED_DATE"].ToString() : "";
                            infoObjAdd.CREATED_LOC = dtl.Rows[i]["CREATED_LOC"] != DBNull.Value ? dtl.Rows[i]["CREATED_LOC"].ToString() : "";
                            infoObjAdd.UPDATED_BY = dtl.Rows[i]["UPDATED_BY"] != DBNull.Value ? dtl.Rows[i]["UPDATED_BY"].ToString() : "";
                            infoObjAdd.UPDATED_DATE = dtl.Rows[i]["UPDATED_DATE"] != DBNull.Value ? dtl.Rows[i]["UPDATED_DATE"].ToString() : "";
                            infoObjAdd.UPDATED_LOC = dtl.Rows[i]["UPDATED_LOC"] != DBNull.Value ? dtl.Rows[i]["UPDATED_LOC"].ToString() : "";

                            // Adding.
                            listItemsAdd1.Add(infoObjAdd);

                        }
                    }

                    DataTable dtl2 = await dbdal.getGrading_Data(id, "D2"); //get d data
                    if (dtl2 != null && dtl2.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtl2.Rows.Count; i++)
                        {
                            GradingAppearanceLstModel infoObjAdd = new GradingAppearanceLstModel();

                            infoObjAdd.MM_GRADING_D2_ID = Convert.ToInt32(dtl2.Rows[i]["MM_GRADING_D2_ID"]);
                            infoObjAdd.MM_GRADING_H_ID = Convert.ToInt32(dtl2.Rows[i]["MM_GRADING_H_ID"]);
                            infoObjAdd.SECTION = dtl2.Rows[i]["SECTION"] != DBNull.Value ? dtl2.Rows[i]["SECTION"].ToString() : "";
                            infoObjAdd.FIELD_NAME = dtl2.Rows[i]["FIELD_NAME"] != DBNull.Value ? dtl2.Rows[i]["FIELD_NAME"].ToString() : "";
                            infoObjAdd.PRIORITY = Convert.ToInt32(dtl2.Rows[i]["PRIORITY"]);
                            infoObjAdd.FINAL_PRIORITY = Convert.ToInt32(dtl2.Rows[i]["FINAL_PRIORITY"]);
                            infoObjAdd.L_SPEC = Convert.ToDecimal(dtl2.Rows[i]["L_SPEC"]);
                            infoObjAdd.U_SPEC = Convert.ToDecimal(dtl2.Rows[i]["U_SPEC"]);
                            infoObjAdd.L_PCL = Convert.ToDecimal(dtl2.Rows[i]["L_PCL"]);
                            infoObjAdd.U_PCL = Convert.ToDecimal(dtl2.Rows[i]["U_PCL"]);
                            infoObjAdd.CALCULATION = Convert.ToDecimal(dtl2.Rows[i]["CALCULATION"]);
                            infoObjAdd.GRADE = dtl2.Rows[i]["GRADE"] != DBNull.Value ? dtl2.Rows[i]["GRADE"].ToString() : "";
                            infoObjAdd.ROUNDING = Convert.ToInt32(dtl2.Rows[i]["ROUNDING"]);

                            infoObjAdd.RECORD_TYP = dtl2.Rows[i]["RECORD_TYP"] != DBNull.Value ? dtl2.Rows[i]["RECORD_TYP"].ToString() : "";
                            infoObjAdd.CREATED_BY = dtl2.Rows[i]["CREATED_BY"] != DBNull.Value ? dtl2.Rows[i]["CREATED_BY"].ToString() : "";
                            infoObjAdd.CREATED_DATE = dtl2.Rows[i]["CREATED_DATE"] != DBNull.Value ? dtl2.Rows[i]["CREATED_DATE"].ToString() : "";
                            infoObjAdd.CREATED_LOC = dtl2.Rows[i]["CREATED_LOC"] != DBNull.Value ? dtl2.Rows[i]["CREATED_LOC"].ToString() : "";
                            infoObjAdd.UPDATED_BY = dtl2.Rows[i]["UPDATED_BY"] != DBNull.Value ? dtl2.Rows[i]["UPDATED_BY"].ToString() : "";
                            infoObjAdd.UPDATED_DATE = dtl2.Rows[i]["UPDATED_DATE"] != DBNull.Value ? dtl2.Rows[i]["UPDATED_DATE"].ToString() : "";
                            infoObjAdd.UPDATED_LOC = dtl2.Rows[i]["UPDATED_LOC"] != DBNull.Value ? dtl2.Rows[i]["UPDATED_LOC"].ToString() : "";

                            // Adding.
                            listItemsAdd2.Add(infoObjAdd);

                        }
                    }

                    DataTable dtl3 = await dbdal.getGrading_Data(id, "D3"); //get d data
                    if (dtl3 != null && dtl3.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtl3.Rows.Count; i++)
                        {
                            GradingPassingLstModel infoObjAdd = new GradingPassingLstModel();

                            model.PROPERTIES = dtl3.Rows[0]["PROPERTIES"] != DBNull.Value ? dtl3.Rows[0]["PROPERTIES"].ToString() : "";
                            model.DropdownItem = await LoadDllData(0, model.PROPERTIES, "PROP_ITEM");

                            infoObjAdd.MM_GRADING_D3_ID = Convert.ToInt32(dtl3.Rows[i]["MM_GRADING_D3_ID"]);
                            infoObjAdd.MM_GRADING_H_ID = Convert.ToInt32(dtl3.Rows[i]["MM_GRADING_H_ID"]);
                            infoObjAdd.PROPERTIES = dtl3.Rows[i]["PROPERTIES"] != DBNull.Value ? dtl3.Rows[i]["PROPERTIES"].ToString() : "";
                            infoObjAdd.PROP_ITEM = dtl3.Rows[i]["PROP_ITEM"] != DBNull.Value ? dtl3.Rows[i]["PROP_ITEM"].ToString() : "";
                            infoObjAdd.PASS_GRADE = dtl3.Rows[i]["PASS_GRADE"] != DBNull.Value ? dtl3.Rows[i]["PASS_GRADE"].ToString() : "";

                            infoObjAdd.RECORD_TYP = dtl3.Rows[i]["RECORD_TYP"] != DBNull.Value ? dtl3.Rows[i]["RECORD_TYP"].ToString() : "";
                            infoObjAdd.CREATED_BY = dtl3.Rows[i]["CREATED_BY"] != DBNull.Value ? dtl3.Rows[i]["CREATED_BY"].ToString() : "";
                            infoObjAdd.CREATED_DATE = dtl3.Rows[i]["CREATED_DATE"] != DBNull.Value ? dtl3.Rows[i]["CREATED_DATE"].ToString() : "";
                            infoObjAdd.CREATED_LOC = dtl3.Rows[i]["CREATED_LOC"] != DBNull.Value ? dtl3.Rows[i]["CREATED_LOC"].ToString() : "";
                            infoObjAdd.UPDATED_BY = dtl3.Rows[i]["UPDATED_BY"] != DBNull.Value ? dtl3.Rows[i]["UPDATED_BY"].ToString() : "";
                            infoObjAdd.UPDATED_DATE = dtl3.Rows[i]["UPDATED_DATE"] != DBNull.Value ? dtl3.Rows[i]["UPDATED_DATE"].ToString() : "";
                            infoObjAdd.UPDATED_LOC = dtl3.Rows[i]["UPDATED_LOC"] != DBNull.Value ? dtl3.Rows[i]["UPDATED_LOC"].ToString() : "";

                            // Adding.
                            listItemsAdd3.Add(infoObjAdd);

                        }
                    }
                }
            }
            else
            {
                model = new GradingModel();
                model.DropdownItem = new List<SelectListItem>();
            }

            model.GradingNormalLstModel = listItemsAdd1;
            model.GradingAppearanceLstModel = listItemsAdd2;
            model.GradingPassingLstModel = listItemsAdd3;

            if (model.COMP_GROUP != null && model.COMP_GROUP != "")
            {
                model.DropdownProdType = await LoadDllData(0, model.COMP_GROUP, "PROD_TYPE_COMP");
            }
            else
            {
                model.DropdownProdType = new List<SelectListItem>();
            }

            model.DropdownProperty = await LoadDllData(0, "", "PROPERTIES");
            

            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MM_GRADING_ADD(string ActionType, GradingModel model)
        {
            ViewBag.Tittle = "Grading";
            try
            {
                if (ActionType == "postback")
                {
                    ModelState.Clear();
                }
                if (ActionType == "Back")
                {
                    ModelState.Clear();
                    return RedirectToAction("MM_GRADING_LST", "MM_GRADING");
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
        public async Task<ActionResult> DraftGrading(GradingModel model)
        {
            bool success = true;
            string message = "";


            if (ModelState.IsValid)
            {
                if (model.PROD_TYPE == null || model.PROD_TYPE == "" ||
                    model.COMP_GROUP == null || model.COMP_GROUP == "")
                {
                    success = false;
                    message += "Required field cannot empty. ";
                }

                if (success)
                {
                    // insert h
                    model.PROD_LINE = model.PROD_LINE != null ? model.PROD_LINE.ToString() : "";
                    string result1 = await dbdal.Grading_H_Maint(model);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                    else
                    {
                        //delete existing d first
                        string result9 = await dbdal.Grading_D1_Maint(0, Convert.ToInt32(result1), 0, "", 0, 0, 0, 0, 0, 0, 0, 0, "", 0, "2");
                        if (!(int.TryParse(result9, out int num9)))
                        {
                            success = false;
                            message += result9;
                        }
                        else
                        {
                            if (model.GradingNormalLstModel != null && model.GradingNormalLstModel.Count > 0)
                            {
                                // insert d1
                                foreach (var item in model.GradingNormalLstModel)
                                {
                                    int seq = Convert.ToInt32(item.SEQUENCE);
                                    string property = item.PROPERTY != null ? item.PROPERTY.ToString() : "";
                                    int propitemID = Convert.ToInt32(item.MM_PROPERTIES_H_ID);
                                    string propitem = item.PROP_ITEM != null ? item.PROP_ITEM.ToString() : "";
                                    int priority = Convert.ToInt32(item.PRIORITY);
                                    int fullprior = Convert.ToInt32(item.FINAL_PRIORITY);
                                    decimal lspec = Convert.ToDecimal(item.L_SPEC);
                                    decimal uspec = Convert.ToDecimal(item.U_SPEC);
                                    decimal lpcl = Convert.ToDecimal(item.L_PCL);
                                    decimal upcl = Convert.ToDecimal(item.U_PCL);
                                    decimal centreline = Convert.ToDecimal(item.CENTRE_LINE);
                                    string grade = item.GRADE != null ? item.GRADE.ToString() : "";
                                    int rounding = Convert.ToInt32(item.ROUNDING);

                                    string rectype = item.RECORD_TYP != null ? item.RECORD_TYP.ToString() : "";

                                    if (rectype == "0")
                                    {
                                        rectype = "3";
                                    }

                                    string result2 = await dbdal.Grading_D1_Maint(item.MM_GRADING_D1_ID, Convert.ToInt32(result1), seq, property, propitemID, priority, fullprior, 
                                                                            lspec, uspec, lpcl, upcl, centreline, grade, rounding, rectype);
                                    if (!(int.TryParse(result2, out int num2) && result2 != "0"))
                                    {
                                        success = false;
                                        message += result2;
                                    }

                                }
                            }
                        }

                        //delete existing d first
                        string result8 = await dbdal.Grading_D2_Maint(0, Convert.ToInt32(result1), 0, "", "", 0, 0, 0, 0, 0, 0, 0, "", 0, "2");
                        if (!(int.TryParse(result8, out int num8)))
                        {
                            success = false;
                            message += result8;
                        }
                        else
                        {
                            if (model.GradingAppearanceLstModel != null && model.GradingAppearanceLstModel.Count > 0)
                            {
                                // insert d2
                                foreach (var item in model.GradingAppearanceLstModel)
                                {
                                    int seq = Convert.ToInt32(item.SEQUENCE); 
                                    string section = item.SECTION != null ? item.SECTION.ToString() : "";
                                    string field = item.FIELD_NAME != null ? item.FIELD_NAME.ToString() : "";
                                    int priority = Convert.ToInt32(item.PRIORITY);
                                    int fullprior = Convert.ToInt32(item.FINAL_PRIORITY);
                                    decimal lspec = Convert.ToDecimal(item.L_SPEC);
                                    decimal uspec = Convert.ToDecimal(item.U_SPEC);
                                    decimal lpcl = Convert.ToDecimal(item.L_PCL);
                                    decimal upcl = Convert.ToDecimal(item.U_PCL);
                                    decimal calculation = Convert.ToDecimal(item.CALCULATION);
                                    string grade = item.GRADE != null ? item.GRADE.ToString() : "";
                                    int rounding = Convert.ToInt32(item.ROUNDING);

                                    string rectype = item.RECORD_TYP != null ? item.RECORD_TYP.ToString() : "";

                                    if (rectype == "0")
                                    {
                                        rectype = "3";
                                    }

                                    string result2 = await dbdal.Grading_D2_Maint(item.MM_GRADING_D2_ID, Convert.ToInt32(result1), seq, section, field, priority, fullprior, 
                                                                            lspec, uspec, lpcl, upcl, calculation, grade, rounding, rectype);
                                    if (!(int.TryParse(result2, out int num2) && result2 != "0"))
                                    {
                                        success = false;
                                        message += result2;
                                    }

                                }
                            }
                        }

                        //delete existing d first
                        string result7 = await dbdal.Grading_D3_Maint(0, Convert.ToInt32(result1), "", "", "", "2");
                        if (!(int.TryParse(result7, out int num7)))
                        {
                            success = false;
                            message += result7;
                        }
                        else
                        {
                            if (model.GradingPassingLstModel != null && model.GradingPassingLstModel.Count > 0)
                            {
                                //delete existing d first
                                string result3 = await dbdal.Grading_D3_Maint(0, Convert.ToInt32(result1), "", "", "", "2");
                                if (!(int.TryParse(result3, out int num3)))
                                {
                                    success = false;
                                    message += result3;
                                }
                                else
                                {
                                    // insert d3
                                    foreach (var item in model.GradingPassingLstModel)
                                    {
                                        string properties = item.PROPERTIES != null ? item.PROPERTIES.ToString() : "";
                                        string propitem = item.PROP_ITEM != null ? item.PROP_ITEM.ToString() : "";
                                        string passgrade = item.PASS_GRADE != null ? item.PASS_GRADE.ToString() : "";

                                        string rectype = item.RECORD_TYP != null ? item.RECORD_TYP.ToString() : "";

                                        if (rectype == "0")
                                        {
                                            rectype = "3";
                                        }

                                        string result2 = await dbdal.Grading_D3_Maint(item.MM_GRADING_D3_ID, Convert.ToInt32(result1), properties, propitem, passgrade, rectype);
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
                }
                else
                {
                    success = false;
                    message += "Error in saving data. ";
                }
            }
            else
            {
                success = false;
                message += "Error in saving data : Modal State not valid. Please ensure all data has fill in. ";
            }

            var data = new { success = success, message = message };

            return Json(data);
        }

        #endregion

        #region popup

        [SessionExpire]
        public async Task<ActionResult> MM_GRADING_DUPLICATE_POPOUT()
        {
            if (TempData["Error"] != null) { ViewBag.Error = TempData["Error"].ToString(); }
            if (TempData["Result"] != null) { ViewBag.Result = TempData["Result"].ToString(); }

            var model = new GradingModel();

            CommonFunction common = new CommonFunction();
            var param = new { Table = "PVIEW_MM_GRADING", TableID = "", Search = "", Value = "", SortField = "MM_GRADING_H_ID", Direction = "0", FrmRowno = "1", ToRowno = "500000", Deleted = 0 };
            List<GradingDuplLstModel> GradingDuplLstModel = await common.PSP_COMMON_DAPPER<GradingDuplLstModel>("PSP_COMMON_LIST", CommandType.StoredProcedure, param) ?? new List<GradingDuplLstModel>();

            model.GradingDuplLstModel = GradingDuplLstModel;

            return View(model);
        }

        public async Task<ActionResult> getDuplicateGrading(string id)
        {
            bool success = true;
            string message = "";
            var model = new GradingModel();

            List<GradingNormalLstModel> listItemsAdd1 = new List<GradingNormalLstModel>();
            List<GradingAppearanceLstModel> listItemsAdd2 = new List<GradingAppearanceLstModel>();
            List<GradingPassingLstModel> listItemsAdd3 = new List<GradingPassingLstModel>();

            DataTable dt = await dbdal.getGrading_Data(id, "H"); //get header data

            if (dt != null && dt.Rows.Count > 0)
            {
                model.MM_GRADING_H_ID = Convert.ToInt32(dt.Rows[0]["MM_GRADING_H_ID"]);
                model.MM_PRODTYPE_H_ID = Convert.ToInt32(dt.Rows[0]["MM_PRODTYPE_H_ID"]);
                model.PROD_TYPE = dt.Rows[0]["PROD_TYPE"] != DBNull.Value ? dt.Rows[0]["PROD_TYPE"].ToString() : "";
                model.COMP_GROUP = dt.Rows[0]["COMP_GROUP"] != DBNull.Value ? dt.Rows[0]["COMP_GROUP"].ToString() : "";
                model.PROD_LINE = dt.Rows[0]["PROD_LINE"] != DBNull.Value ? dt.Rows[0]["PROD_LINE"].ToString() : "";

                model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";

                //D1
                DataTable dtl = await dbdal.getGrading_Data(id, "D1"); //get d data
                if (dtl != null && dtl.Rows.Count > 0)
                {
                    for (int i = 0; i < dtl.Rows.Count; i++)
                    {
                        GradingNormalLstModel infoObjAdd = new GradingNormalLstModel();

                        infoObjAdd.MM_GRADING_D1_ID = Convert.ToInt32(dtl.Rows[i]["MM_GRADING_D1_ID"]);
                        infoObjAdd.MM_GRADING_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_GRADING_H_ID"]);
                        infoObjAdd.PROPERTY = dtl.Rows[i]["PROPERTY"] != DBNull.Value ? dtl.Rows[i]["PROPERTY"].ToString() : "";
                        infoObjAdd.MM_PROPERTIES_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_PROPERTIES_H_ID"]);
                        infoObjAdd.PROP_ITEM = dtl.Rows[i]["PROP_ITEM"] != DBNull.Value ? dtl.Rows[i]["PROP_ITEM"].ToString() : "";
                        infoObjAdd.PRIORITY = Convert.ToInt32(dtl.Rows[i]["PRIORITY"]);
                        infoObjAdd.FINAL_PRIORITY = Convert.ToInt32(dtl.Rows[i]["FINAL_PRIORITY"]);
                        infoObjAdd.L_SPEC = Convert.ToDecimal(dtl.Rows[i]["L_SPEC"]);
                        infoObjAdd.U_SPEC = Convert.ToDecimal(dtl.Rows[i]["U_SPEC"]);
                        infoObjAdd.L_PCL = Convert.ToDecimal(dtl.Rows[i]["L_PCL"]);
                        infoObjAdd.U_PCL = Convert.ToDecimal(dtl.Rows[i]["U_PCL"]);
                        infoObjAdd.CENTRE_LINE = Convert.ToDecimal(dtl.Rows[i]["CENTRE_LINE"]);
                        infoObjAdd.GRADE = dtl.Rows[i]["GRADE"] != DBNull.Value ? dtl.Rows[i]["GRADE"].ToString() : "";
                        infoObjAdd.ROUNDING = Convert.ToInt32(dtl.Rows[i]["ROUNDING"]);

                        infoObjAdd.RECORD_TYP = dtl.Rows[i]["RECORD_TYP"] != DBNull.Value ? dtl.Rows[i]["RECORD_TYP"].ToString() : "";
                        infoObjAdd.CREATED_BY = dtl.Rows[i]["CREATED_BY"] != DBNull.Value ? dtl.Rows[i]["CREATED_BY"].ToString() : "";
                        infoObjAdd.CREATED_DATE = dtl.Rows[i]["CREATED_DATE"] != DBNull.Value ? dtl.Rows[i]["CREATED_DATE"].ToString() : "";
                        infoObjAdd.CREATED_LOC = dtl.Rows[i]["CREATED_LOC"] != DBNull.Value ? dtl.Rows[i]["CREATED_LOC"].ToString() : "";
                        infoObjAdd.UPDATED_BY = dtl.Rows[i]["UPDATED_BY"] != DBNull.Value ? dtl.Rows[i]["UPDATED_BY"].ToString() : "";
                        infoObjAdd.UPDATED_DATE = dtl.Rows[i]["UPDATED_DATE"] != DBNull.Value ? dtl.Rows[i]["UPDATED_DATE"].ToString() : "";
                        infoObjAdd.UPDATED_LOC = dtl.Rows[i]["UPDATED_LOC"] != DBNull.Value ? dtl.Rows[i]["UPDATED_LOC"].ToString() : "";

                        // Adding.
                        listItemsAdd1.Add(infoObjAdd);

                    }
                }

                //D2
                DataTable dtl2 = await dbdal.getGrading_Data(id, "D2"); //get d data
                if (dtl2 != null && dtl2.Rows.Count > 0)
                {
                    for (int i = 0; i < dtl2.Rows.Count; i++)
                    {
                        GradingAppearanceLstModel infoObjAdd = new GradingAppearanceLstModel();

                        infoObjAdd.MM_GRADING_D2_ID = Convert.ToInt32(dtl2.Rows[i]["MM_GRADING_D2_ID"]);
                        infoObjAdd.MM_GRADING_H_ID = Convert.ToInt32(dtl2.Rows[i]["MM_GRADING_H_ID"]);
                        infoObjAdd.SECTION = dtl2.Rows[i]["SECTION"] != DBNull.Value ? dtl2.Rows[i]["SECTION"].ToString() : "";
                        infoObjAdd.FIELD_NAME = dtl2.Rows[i]["FIELD_NAME"] != DBNull.Value ? dtl2.Rows[i]["FIELD_NAME"].ToString() : "";
                        infoObjAdd.PRIORITY = Convert.ToInt32(dtl2.Rows[i]["PRIORITY"]);
                        infoObjAdd.FINAL_PRIORITY = Convert.ToInt32(dtl2.Rows[i]["FINAL_PRIORITY"]);
                        infoObjAdd.L_SPEC = Convert.ToDecimal(dtl2.Rows[i]["L_SPEC"]);
                        infoObjAdd.U_SPEC = Convert.ToDecimal(dtl2.Rows[i]["U_SPEC"]);
                        infoObjAdd.L_PCL = Convert.ToDecimal(dtl2.Rows[i]["L_PCL"]);
                        infoObjAdd.U_PCL = Convert.ToDecimal(dtl2.Rows[i]["U_PCL"]);
                        infoObjAdd.CALCULATION = Convert.ToDecimal(dtl2.Rows[i]["CALCULATION"]);
                        infoObjAdd.GRADE = dtl2.Rows[i]["GRADE"] != DBNull.Value ? dtl2.Rows[i]["GRADE"].ToString() : "";
                        infoObjAdd.ROUNDING = Convert.ToInt32(dtl2.Rows[i]["ROUNDING"]);

                        infoObjAdd.RECORD_TYP = dtl2.Rows[i]["RECORD_TYP"] != DBNull.Value ? dtl2.Rows[i]["RECORD_TYP"].ToString() : "";
                        infoObjAdd.CREATED_BY = dtl2.Rows[i]["CREATED_BY"] != DBNull.Value ? dtl2.Rows[i]["CREATED_BY"].ToString() : "";
                        infoObjAdd.CREATED_DATE = dtl2.Rows[i]["CREATED_DATE"] != DBNull.Value ? dtl2.Rows[i]["CREATED_DATE"].ToString() : "";
                        infoObjAdd.CREATED_LOC = dtl2.Rows[i]["CREATED_LOC"] != DBNull.Value ? dtl2.Rows[i]["CREATED_LOC"].ToString() : "";
                        infoObjAdd.UPDATED_BY = dtl2.Rows[i]["UPDATED_BY"] != DBNull.Value ? dtl2.Rows[i]["UPDATED_BY"].ToString() : "";
                        infoObjAdd.UPDATED_DATE = dtl2.Rows[i]["UPDATED_DATE"] != DBNull.Value ? dtl2.Rows[i]["UPDATED_DATE"].ToString() : "";
                        infoObjAdd.UPDATED_LOC = dtl2.Rows[i]["UPDATED_LOC"] != DBNull.Value ? dtl2.Rows[i]["UPDATED_LOC"].ToString() : "";

                        // Adding.
                        listItemsAdd2.Add(infoObjAdd);

                    }
                }

                //D3
                DataTable dtl3 = await dbdal.getGrading_Data(id, "D3"); //get d data
                if (dtl3 != null && dtl3.Rows.Count > 0)
                {
                    for (int i = 0; i < dtl3.Rows.Count; i++)
                    {
                        GradingPassingLstModel infoObjAdd = new GradingPassingLstModel();

                        model.PROPERTIES = dtl3.Rows[0]["PROPERTIES"] != DBNull.Value ? dtl3.Rows[0]["PROPERTIES"].ToString() : "";
                        model.DropdownItem = await LoadDllData(0, model.PROPERTIES, "PROP_ITEM");

                        infoObjAdd.MM_GRADING_D3_ID = Convert.ToInt32(dtl3.Rows[i]["MM_GRADING_D3_ID"]);
                        infoObjAdd.MM_GRADING_H_ID = Convert.ToInt32(dtl3.Rows[i]["MM_GRADING_H_ID"]);
                        infoObjAdd.PROPERTIES = dtl3.Rows[i]["PROPERTIES"] != DBNull.Value ? dtl3.Rows[i]["PROPERTIES"].ToString() : "";
                        infoObjAdd.PROP_ITEM = dtl3.Rows[i]["PROP_ITEM"] != DBNull.Value ? dtl3.Rows[i]["PROP_ITEM"].ToString() : "";
                        infoObjAdd.PASS_GRADE = dtl3.Rows[i]["PASS_GRADE"] != DBNull.Value ? dtl3.Rows[i]["PASS_GRADE"].ToString() : "";

                        infoObjAdd.RECORD_TYP = dtl3.Rows[i]["RECORD_TYP"] != DBNull.Value ? dtl3.Rows[i]["RECORD_TYP"].ToString() : "";
                        infoObjAdd.CREATED_BY = dtl3.Rows[i]["CREATED_BY"] != DBNull.Value ? dtl3.Rows[i]["CREATED_BY"].ToString() : "";
                        infoObjAdd.CREATED_DATE = dtl3.Rows[i]["CREATED_DATE"] != DBNull.Value ? dtl3.Rows[i]["CREATED_DATE"].ToString() : "";
                        infoObjAdd.CREATED_LOC = dtl3.Rows[i]["CREATED_LOC"] != DBNull.Value ? dtl3.Rows[i]["CREATED_LOC"].ToString() : "";
                        infoObjAdd.UPDATED_BY = dtl3.Rows[i]["UPDATED_BY"] != DBNull.Value ? dtl3.Rows[i]["UPDATED_BY"].ToString() : "";
                        infoObjAdd.UPDATED_DATE = dtl3.Rows[i]["UPDATED_DATE"] != DBNull.Value ? dtl3.Rows[i]["UPDATED_DATE"].ToString() : "";
                        infoObjAdd.UPDATED_LOC = dtl3.Rows[i]["UPDATED_LOC"] != DBNull.Value ? dtl3.Rows[i]["UPDATED_LOC"].ToString() : "";

                        // Adding.
                        listItemsAdd3.Add(infoObjAdd);

                    }
                }


            }
            else
            {
                success = false;
                message = "Fail to get data";
            }
            var data = new { success = success, message = message, modelh = model, d1 = listItemsAdd1, d2 = listItemsAdd2, d3 = listItemsAdd3 };

            return Json(data);
        }

        #endregion

        #region DELETE

        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> deleteGrading(List<string> lstid)
        {
            bool success = true;
            string message = "";

            GradingModel modelh = new GradingModel();

            if (lstid.Count < 1)
            {
                success = false;
                message += "Please select a record first to delete. ";
            }

            if (success && lstid.Count > 0)
            {
                foreach (var item in lstid)
                {
                    modelh.MM_GRADING_H_ID = Convert.ToInt32(item);
                    modelh.RECORD_TYP = "5";

                    modelh.MM_PRODTYPE_H_ID = 0;
                    modelh.COMP_GROUP = "";
                    modelh.PROD_LINE = "";

                    //delete h
                    string result1 = await dbdal.Grading_H_Maint(modelh);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                    else
                    {
                        //delete d1
                        string result2 = await dbdal.Grading_D1_Maint(0, Convert.ToInt32(item),0, "", 0, 0, 0, 0, 0, 0, 0, 0, "", 0, "2");
                        if (!(int.TryParse(result2, out int num2)))
                        {
                            success = false;
                            message += result2;
                        }
                        else
                        {
                            //delete d2
                            string result3 = await dbdal.Grading_D2_Maint(0, Convert.ToInt32(item), 0, "", "", 0, 0, 0, 0, 0, 0, 0, "", 0, "2");
                            if (!(int.TryParse(result3, out int num3)))
                            {
                                success = false;
                                message += result3;
                            }
                            else
                            {
                                //delete d3
                                string result4 = await dbdal.Grading_D3_Maint(0, Convert.ToInt32(item), "", "", "", "2");
                                if (!(int.TryParse(result4, out int num4)))
                                {
                                    success = false;
                                    message += result4;
                                }
                            }
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
        public async Task<ActionResult> MM_GRADING_AUDIT(string id)
        {
            TempData["SQ_ID"] = id;
            CommonFunction common = new CommonFunction();
            var param = new { pTable = "PVIEW_MM_GRADING_A", pKeyValue = id };
            List<AuditTrailModel> AuditTrailModel = await common.PSP_COMMON_DAPPER<AuditTrailModel>("PSP_COMMON_AUDIT", CommandType.StoredProcedure, param) ?? new List<AuditTrailModel>();

            return View(AuditTrailModel);
        }

        #endregion

        #region DDL

        public async Task<ActionResult> fillProperty()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items = await LoadInnerDllData(0, "", "PROPERTIES");

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> fillPropItem(string property)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items = await LoadInnerDllData(0, property, "PROP_ITEM");

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> fillSection()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items = await LoadInnerDllData(0, "", "SECTION");

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> fillFieldName(string section)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items = await LoadInnerDllData(0, section, "FIELD_NAME");

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> fillProdType(string comp)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items = await LoadDllData(0, comp, "PROD_TYPE_COMP");

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        private async Task<List<SelectListItem>> LoadInnerDllData(int ID, string act, string category)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            listItems.Add(new SelectListItem { Text = "Select an option..", Value = "" });

            DataTable dt = await dbmain.getDllData(ID, act, category);
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    SelectListItem infoObj = new SelectListItem();
                    infoObj.Text = dt.Rows[i]["NAME_TEXT"].ToString();
                    infoObj.Value = dt.Rows[i]["ID_VALUE"].ToString();

                    listItems.Add(infoObj);
                }
            }
            return listItems;
        }

        private async Task<List<SelectListItem>> LoadDllData(int ID, string act, string category)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();

            DataTable dt = await dbmain.getDllData(ID, act, category);
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    SelectListItem infoObj = new SelectListItem();
                    infoObj.Text = dt.Rows[i]["NAME_TEXT"].ToString();
                    infoObj.Value = dt.Rows[i]["ID_VALUE"].ToString();

                    listItems.Add(infoObj);
                }
            }
            return listItems;
        }

        #endregion
    }
}