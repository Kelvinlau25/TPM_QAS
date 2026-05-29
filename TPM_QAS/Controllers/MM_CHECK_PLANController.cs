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

namespace TPM_QAS.Controllers
{
    public class MM_CHECK_PLANController : Controller
    {
        DB dbmain = new DB();
        MM_CHECK_PLAN_DAL dbdal = new MM_CHECK_PLAN_DAL();

        #region LIST

        [SessionExpire]
        public async Task<ActionResult> MM_CHECK_PLAN_LST(string Deleted = "0")
        {
            TempData["STATUS_DDL"] = await dbmain.PSP_COMMON_DDL("CMN_LST_STATUS", "", "", "", "", "", "");

            CommonFunction common = new CommonFunction();
            var param = new { Table = "PVIEW_MM_CHECKPLAN", TableID = "", Search = "", Value = "", SortField = "MM_CHECKPLAN_H_ID", Direction = "0", FrmRowno = "1", ToRowno = "500000", Deleted = Deleted };
            List<CheckPlanModel> model = await common.PSP_COMMON_DAPPER<CheckPlanModel>("PSP_COMMON_LIST", CommandType.StoredProcedure, param) ?? new List<CheckPlanModel>();

            ViewBag.Deleted = Deleted;
            return View(model);
        }

        #endregion

        #region VIEW/EDIT

        [SessionExpire]
        public async Task<ActionResult> MM_CHECK_PLAN_DETAIL(string id)
        {
            ViewBag.Tittle = "Checking Plan";
            var model = new CheckPlanModel();

            DataTable dt = await dbdal.getCheckPlan_Data(id, "H"); //get header data

            if (dt != null && dt.Rows.Count > 0)
            {
                model.MM_CHECKPLAN_H_ID = Convert.ToInt32(dt.Rows[0]["MM_CHECKPLAN_H_ID"]);
                model.MM_PRODGROUP_H_ID = Convert.ToInt32(dt.Rows[0]["MM_PRODGROUP_H_ID"]);
                model.PROD_GROUP = dt.Rows[0]["PROD_GROUP"] != DBNull.Value ? dt.Rows[0]["PROD_GROUP"].ToString() : "";
                model.PATTERN_TYPE = dt.Rows[0]["PATTERN_TYPE"] != DBNull.Value ? dt.Rows[0]["PATTERN_TYPE"].ToString() : "";
                model.COMP_GROUP = dt.Rows[0]["COMP_GROUP"] != DBNull.Value ? dt.Rows[0]["COMP_GROUP"].ToString() : "";

                model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";
            }

            DataTable dtl = await dbdal.getCheckPlan_Data(id, "D"); //get d data
            List<CheckPlanPropLstModel> listItemsAdd = new List<CheckPlanPropLstModel>();
            if (dtl != null && dtl.Rows.Count > 0)
            {
                for (int i = 0; i < dtl.Rows.Count; i++)
                {
                    CheckPlanPropLstModel infoObjAdd = new CheckPlanPropLstModel();

                    infoObjAdd.MM_CHECKPLAN_D_ID = Convert.ToInt32(dtl.Rows[i]["MM_CHECKPLAN_D_ID"]);
                    infoObjAdd.MM_CHECKPLAN_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_CHECKPLAN_H_ID"]);
                    infoObjAdd.MM_PROPERTIES_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_PROPERTIES_H_ID"]);
                    infoObjAdd.PROPERTIES = dtl.Rows[i]["PROPERTIES"] != DBNull.Value ? dtl.Rows[i]["PROPERTIES"].ToString() : "";
                    infoObjAdd.PROP_ITEM = dtl.Rows[i]["PROP_ITEM"] != DBNull.Value ? dtl.Rows[i]["PROP_ITEM"].ToString() : "";

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

            model.CheckPlanPropLstModel = listItemsAdd;

            model.DropdownProdGroup = await LoadDllData(0, "", "PROD_GROUP");

            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MM_CHECK_PLAN_DETAIL(string ActionType, CheckPlanModel model)
        {
            ViewBag.Tittle = "Checking Plan";
            model.DropdownProdGroup = await LoadDllData(0, "", "PROD_GROUP");

            try
            {
                if (ActionType == "postback")
                {
                    ModelState.Clear();
                }
                if (ActionType == "Back")
                {
                    ModelState.Clear();
                    return RedirectToAction("MM_CHECK_PLAN_LST", "MM_CHECK_PLAN");
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
        public async Task<ActionResult> InsertUpdateCheckPlan(CheckPlanModel model)
        {
            bool success = true;
            string message = "";


            if (ModelState.IsValid)
            {
                if (model.MM_PRODGROUP_H_ID == 0 ||
                    model.PROD_GROUP == null || model.PROD_GROUP == "" ||
                    model.PATTERN_TYPE == null || model.PATTERN_TYPE == "" ||
                    model.COMP_GROUP == null || model.COMP_GROUP == "" ||
                    model.CheckPlanPropLstModel == null || model.CheckPlanPropLstModel.Count < 1)
                {
                    success = false;
                    message += "Required field cannot empty.";
                }

                if (success)
                {
                    // insert h
                    string result1 = await dbdal.CheckPlan_H_Maint(model);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                    else
                    {
                        // insert d
                        foreach (var item in model.CheckPlanPropLstModel)
                        {
                            int propid = Convert.ToInt32(item.MM_PROPERTIES_H_ID);
                            string properties = item.PROPERTIES != null ? item.PROPERTIES.ToString() : "";
                            string propitem = item.PROP_ITEM != null ? item.PROP_ITEM.ToString() : "";
                            string rectype = item.RECORD_TYP != null ? item.RECORD_TYP.ToString() : "";

                            if (rectype == "0")
                            {
                                rectype = "3";
                            }

                            string result2 = await dbdal.CheckPlan_D_Maint(item.MM_CHECKPLAN_D_ID, Convert.ToInt32(result1), propid, rectype);
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
        public async Task<ActionResult> MM_CHECK_PLAN_ADD(string id = "")
        {
            ViewBag.Tittle = "Checking Plan";
            var model = new CheckPlanModel();
            List<CheckPlanPropLstModel> listItemsAdd = new List<CheckPlanPropLstModel>();

            if (!string.IsNullOrWhiteSpace(id) && CommonMethod.isNumeric(id))
            {
                DataTable dt = await dbdal.getCheckPlan_Data(id, "H"); //get header data

                if (dt != null && dt.Rows.Count > 0)
                {
                    model.MM_CHECKPLAN_H_ID = Convert.ToInt32(dt.Rows[0]["MM_CHECKPLAN_H_ID"]);
                    model.MM_PRODGROUP_H_ID = Convert.ToInt32(dt.Rows[0]["MM_PRODGROUP_H_ID"]);
                    model.PROD_GROUP = dt.Rows[0]["PROD_GROUP"] != DBNull.Value ? dt.Rows[0]["PROD_GROUP"].ToString() : "";
                    model.PATTERN_TYPE = dt.Rows[0]["PATTERN_TYPE"] != DBNull.Value ? dt.Rows[0]["PATTERN_TYPE"].ToString() : "";
                    model.COMP_GROUP = dt.Rows[0]["COMP_GROUP"] != DBNull.Value ? dt.Rows[0]["COMP_GROUP"].ToString() : "";

                    model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                    model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                    model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                    model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                    model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                    model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                    model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";

                    DataTable dtl = await dbdal.getCheckPlan_Data(id, "D"); //get d data
                    if (dtl != null && dtl.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtl.Rows.Count; i++)
                        {
                            CheckPlanPropLstModel infoObjAdd = new CheckPlanPropLstModel();

                            infoObjAdd.MM_CHECKPLAN_D_ID = Convert.ToInt32(dtl.Rows[i]["MM_CHECKPLAN_D_ID"]);
                            infoObjAdd.MM_CHECKPLAN_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_CHECKPLAN_H_ID"]);
                            infoObjAdd.MM_PROPERTIES_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_PROPERTIES_H_ID"]);
                            infoObjAdd.PROPERTIES = dtl.Rows[i]["PROPERTIES"] != DBNull.Value ? dtl.Rows[i]["PROPERTIES"].ToString() : "";
                            infoObjAdd.PROP_ITEM = dtl.Rows[i]["PROP_ITEM"] != DBNull.Value ? dtl.Rows[i]["PROP_ITEM"].ToString() : "";

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
                model = new CheckPlanModel();
            }

            model.CheckPlanPropLstModel = listItemsAdd;
            model.DropdownProdGroup = await LoadDllData(0, "", "PROD_GROUP");
            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MM_CHECK_PLAN_ADD(string ActionType, CheckPlanModel model)
        {
            try
            {
                ViewBag.Tittle = "Checking Plan";
                model.DropdownProdGroup = await LoadDllData(0, "", "PROD_GROUP");

                if (ActionType == "postback")
                {
                    ModelState.Clear();
                }
                if (ActionType == "Back")
                {
                    ModelState.Clear();
                    return RedirectToAction("MM_CHECK_PLAN_LST", "MM_CHECK_PLAN");
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
        public async Task<ActionResult> DraftCheckingPlan(CheckPlanModel model)
        {
            bool success = true;
            string message = "";


            if (ModelState.IsValid)
            {
                if (model.MM_PRODGROUP_H_ID == 0 ||
                    model.PROD_GROUP == null || model.PROD_GROUP == "" ||
                    model.PATTERN_TYPE == null || model.PATTERN_TYPE == "" ||
                    model.COMP_GROUP == null || model.COMP_GROUP == "")
                {
                    success = false;
                    message += "Required field cannot empty.";
                }

                if (success)
                {
                    // insert h
                    string result1 = await dbdal.CheckPlan_H_Maint(model);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                    else
                    {
                        if (model.CheckPlanPropLstModel != null && model.CheckPlanPropLstModel.Count > 0)
                        {
                            // insert d
                            foreach (var item in model.CheckPlanPropLstModel)
                            {
                                int propid = Convert.ToInt32(item.MM_PROPERTIES_H_ID);
                                string properties = item.PROPERTIES != null ? item.PROPERTIES.ToString() : "";
                                string propitem = item.PROP_ITEM != null ? item.PROP_ITEM.ToString() : "";
                                string rectype = item.RECORD_TYP != null ? item.RECORD_TYP.ToString() : "";

                                string result2 = await dbdal.CheckPlan_D_Maint(item.MM_CHECKPLAN_D_ID, Convert.ToInt32(result1), propid, rectype);
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
                    message += "Error in saving data. ";
                }
            }
            else
            {
                success = false;
                message += "Error in saving data : Modal State not valid. ";
            }

            var data = new { success = success, message = message };

            return Json(data);
        }

        #endregion

        #region POPUP

        [SessionExpire]
        public async Task<ActionResult> MM_CHECK_PLAN_PROPERTY_POPOUT(string id)
        {
            if (TempData["Error"] != null) { ViewBag.Error = TempData["Error"].ToString(); }
            if (TempData["Result"] != null) { ViewBag.Result = TempData["Result"].ToString(); }

            var model = new CheckPlanModel();

            if (id == null || id == "")
            {
                id = "0";
            }

            DataTable dt = await dbdal.getPropertyPOPUP(Convert.ToInt32(id));

            List<PropertiesModel> listItemsAdd = new List<PropertiesModel>();
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    PropertiesModel infoObjAdd = new PropertiesModel();

                    infoObjAdd.MM_PROPERTIES_H_ID = Convert.ToInt32(dt.Rows[i]["MM_PROPERTIES_H_ID"]);
                    infoObjAdd.PROPERTIES = dt.Rows[i]["PROPERTIES"] != DBNull.Value ? dt.Rows[i]["PROPERTIES"].ToString() : "";
                    infoObjAdd.PROP_ITEM = dt.Rows[i]["PROP_ITEM"] != DBNull.Value ? dt.Rows[i]["PROP_ITEM"].ToString() : "";


                    infoObjAdd.IS_EXIST = "N";

                    // Adding.
                    listItemsAdd.Add(infoObjAdd);

                }

            }

            model.PropertiesModel = listItemsAdd;

            return View(model);
        }

        #endregion

        #region DELETE

        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> deleteCheckPlan(List<string> lstid)
        {
            bool success = true;
            string message = "";

            CheckPlanModel modelh = new CheckPlanModel();

            if (lstid.Count < 1)
            {
                success = false;
                message = "Please select a record first to delete.";
            }

            if (success && lstid.Count > 0)
            {
                foreach (var item in lstid)
                {
                    modelh.MM_CHECKPLAN_H_ID = Convert.ToInt32(item);
                    modelh.RECORD_TYP = "5";

                    modelh.MM_PRODGROUP_H_ID = 0;
                    modelh.PATTERN_TYPE = "";
                    modelh.COMP_GROUP = "";

                    //delete h
                    string result1 = await dbdal.CheckPlan_H_Maint(modelh);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                    else
                    {
                        //delete d
                        string result2 = await dbdal.CheckPlan_D_Maint(0, Convert.ToInt32(item), 0, "2");
                        if (!(int.TryParse(result2, out int num2)))
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
        public async Task<ActionResult> MM_CHECK_PLAN_AUDIT(string id)
        {
            TempData["SQ_ID"] = id;
            CommonFunction common = new CommonFunction();
            var param = new { pTable = "PVIEW_MM_CHECKPLAN_A", pKeyValue = id };
            List<AuditTrailModel> AuditTrailModel = await common.PSP_COMMON_DAPPER<AuditTrailModel>("PSP_COMMON_AUDIT", CommandType.StoredProcedure, param) ?? new List<AuditTrailModel>();

            return View(AuditTrailModel);
        }

        #endregion

        #region DDL

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