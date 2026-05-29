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
    public class MM_CHECK_ROUTINEController : Controller
    {
        DB dbmain = new DB();
        MM_CHECK_ROUTINE_DAL dbdal = new MM_CHECK_ROUTINE_DAL();

        #region LIST

        [SessionExpire]
        public async Task<ActionResult> MM_CHECK_ROUTINE_LST(string Deleted = "0")
        {
            TempData["STATUS_DDL"] = await dbmain.PSP_COMMON_DDL("CMN_LST_STATUS", "", "", "", "", "", "");

            CommonFunction common = new CommonFunction();
            var param = new { Table = "PVIEW_MM_CHECKROUTINE", TableID = "", Search = "", Value = "", SortField = "MM_CHECKROUTINE_H_ID", Direction = "0", FrmRowno = "1", ToRowno = "500000", Deleted = Deleted };
            List<CheckRoutineModel> model = await common.PSP_COMMON_DAPPER<CheckRoutineModel>("PSP_COMMON_LIST", CommandType.StoredProcedure, param) ?? new List<CheckRoutineModel>();

            ViewBag.Deleted = Deleted;
            return View(model);
        }

        #endregion

        #region VIEW/EDIT

        [SessionExpire]
        public async Task<ActionResult> MM_CHECK_ROUTINE_DETAIL(string id)
        {
            ViewBag.Tittle = "Checking Routine";
            var model = new CheckRoutineModel();

            DataTable dt = await dbdal.getCheckRoutine_Data(id, "H"); //get header data

            if (dt != null && dt.Rows.Count > 0)
            {
                model.MM_CHECKROUTINE_H_ID = Convert.ToInt32(dt.Rows[0]["MM_CHECKROUTINE_H_ID"]);
                model.MM_PRODGROUP_H_ID = Convert.ToInt32(dt.Rows[0]["MM_PRODGROUP_H_ID"]);
                model.PROD_GROUP = dt.Rows[0]["PROD_GROUP"] != DBNull.Value ? dt.Rows[0]["PROD_GROUP"].ToString() : "";
                model.PATTERN = dt.Rows[0]["PATTERN"] != DBNull.Value ? dt.Rows[0]["PATTERN"].ToString() : "";
                model.WEEKORDAY = dt.Rows[0]["WEEKORDAY"] != DBNull.Value ? dt.Rows[0]["WEEKORDAY"].ToString() : "";
                model.WHOLEWEEK = dt.Rows[0]["WHOLEWEEK"] != DBNull.Value ? dt.Rows[0]["WHOLEWEEK"].ToString() : "";

                model.MONDAY = dt.Rows[0]["MONDAY"] != DBNull.Value ? dt.Rows[0]["MONDAY"].ToString() : "";
                model.TUESDAY = dt.Rows[0]["TUESDAY"] != DBNull.Value ? dt.Rows[0]["TUESDAY"].ToString() : "";
                model.WEDNESDAY = dt.Rows[0]["WEDNESDAY"] != DBNull.Value ? dt.Rows[0]["WEDNESDAY"].ToString() : "";
                model.THURSDAY = dt.Rows[0]["THURSDAY"] != DBNull.Value ? dt.Rows[0]["THURSDAY"].ToString() : "";
                model.FRIDAY = dt.Rows[0]["FRIDAY"] != DBNull.Value ? dt.Rows[0]["FRIDAY"].ToString() : "";
                model.SATURDAY = dt.Rows[0]["SATURDAY"] != DBNull.Value ? dt.Rows[0]["SATURDAY"].ToString() : "";
                model.SUNDAY = dt.Rows[0]["SUNDAY"] != DBNull.Value ? dt.Rows[0]["SUNDAY"].ToString() : "";

                model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";
            }

            DataTable dtl = await dbdal.getCheckRoutine_Data(id, "D"); //get d data
            List<CheckRoutineProdLstModel> listItemsAdd = new List<CheckRoutineProdLstModel>();
            if (dtl != null && dtl.Rows.Count > 0)
            {
                for (int i = 0; i < dtl.Rows.Count; i++)
                {
                    CheckRoutineProdLstModel infoObjAdd = new CheckRoutineProdLstModel();

                    infoObjAdd.MM_CHECKROUTINE_D_ID = Convert.ToInt32(dtl.Rows[i]["MM_CHECKROUTINE_D_ID"]);
                    infoObjAdd.MM_CHECKROUTINE_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_CHECKROUTINE_H_ID"]);
                    infoObjAdd.PRODLINENO_ID = Convert.ToInt32(dtl.Rows[i]["PRODLINENO_ID"]);
                    infoObjAdd.PRODLINE = dtl.Rows[i]["PRODLINE"] != DBNull.Value ? dtl.Rows[i]["PRODLINE"].ToString() : "";
                    infoObjAdd.PRODLINENO = dtl.Rows[i]["PRODLINENO"] != DBNull.Value ? dtl.Rows[i]["PRODLINENO"].ToString() : "";

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

            model.CheckRoutineProdLstModel = listItemsAdd;

            model.DropdownProdGroup = await LoadDllData(0, "", "PROD_GROUP");

            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MM_CHECK_ROUTINE_DETAIL(string ActionType, CheckRoutineModel model)
        {
            ViewBag.Tittle = "Checking Routine";
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
                    return RedirectToAction("MM_CHECK_ROUTINE_LST", "MM_CHECK_ROUTINE");
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
        public async Task<ActionResult> InsertUpdateCheckRoutine(CheckRoutineModel model)
        {
            bool success = true;
            string message = "";


            if (ModelState.IsValid)
            {
                if (model.MM_PRODGROUP_H_ID == 0 ||
                    model.PROD_GROUP == null || model.PROD_GROUP == "" ||
                    model.PATTERN == null || model.PATTERN == "" ||
                    model.SEQUENCE == null || model.SEQUENCE == "" ||
                    model.CheckRoutineProdLstModel == null || model.CheckRoutineProdLstModel.Count < 1)
                {
                    success = false;
                    message += "Required field cannot empty.";
                }

                if (success)
                {
                    // insert h
                    string result1 = await dbdal.CheckRoutine_H_Maint(model);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                    else
                    {
                        // insert d
                        foreach (var item in model.CheckRoutineProdLstModel)
                        {
                            int prodid = Convert.ToInt32(item.PRODLINENO_ID);
                            string prodline = item.PRODLINE != null ? item.PRODLINE.ToString() : "";
                            string lineno = item.PRODLINENO != null ? item.PRODLINENO.ToString() : "";
                            string rectype = item.RECORD_TYP != null ? item.RECORD_TYP.ToString() : "";

                            if (rectype == "0")
                            {
                                rectype = "3";
                            }

                            string result2 = await dbdal.CheckRoutine_D_Maint(item.MM_CHECKROUTINE_D_ID, Convert.ToInt32(result1), prodid, rectype);
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
        public async Task<ActionResult> MM_CHECK_ROUTINE_ADD(string id = "")
        {
            ViewBag.Tittle = "Checking Routine";
            var model = new CheckRoutineModel();
            List<CheckRoutineProdLstModel> listItemsAdd = new List<CheckRoutineProdLstModel>();

            if (!string.IsNullOrWhiteSpace(id) && CommonMethod.isNumeric(id))
            {
                DataTable dt = await dbdal.getCheckRoutine_Data(id, "H"); //get header data

                if (dt != null && dt.Rows.Count > 0)
                {
                    model.MM_CHECKROUTINE_H_ID = Convert.ToInt32(dt.Rows[0]["MM_CHECKROUTINE_H_ID"]);
                    model.MM_PRODGROUP_H_ID = Convert.ToInt32(dt.Rows[0]["MM_PRODGROUP_H_ID"]);
                    model.PROD_GROUP = dt.Rows[0]["PROD_GROUP"] != DBNull.Value ? dt.Rows[0]["PROD_GROUP"].ToString() : "";
                    model.PATTERN = dt.Rows[0]["PATTERN"] != DBNull.Value ? dt.Rows[0]["PATTERN"].ToString() : "";
                    model.WEEKORDAY = dt.Rows[0]["WEEKORDAY"] != DBNull.Value ? dt.Rows[0]["WEEKORDAY"].ToString() : "";
                    model.WHOLEWEEK = dt.Rows[0]["WHOLEWEEK"] != DBNull.Value ? dt.Rows[0]["WHOLEWEEK"].ToString() : "";

                    model.MONDAY = dt.Rows[0]["MONDAY"] != DBNull.Value ? dt.Rows[0]["MONDAY"].ToString() : "";
                    model.TUESDAY = dt.Rows[0]["TUESDAY"] != DBNull.Value ? dt.Rows[0]["TUESDAY"].ToString() : "";
                    model.WEDNESDAY = dt.Rows[0]["WEDNESDAY"] != DBNull.Value ? dt.Rows[0]["WEDNESDAY"].ToString() : "";
                    model.THURSDAY = dt.Rows[0]["THURSDAY"] != DBNull.Value ? dt.Rows[0]["THURSDAY"].ToString() : "";
                    model.FRIDAY = dt.Rows[0]["FRIDAY"] != DBNull.Value ? dt.Rows[0]["FRIDAY"].ToString() : "";
                    model.SATURDAY = dt.Rows[0]["SATURDAY"] != DBNull.Value ? dt.Rows[0]["SATURDAY"].ToString() : "";
                    model.SUNDAY = dt.Rows[0]["SUNDAY"] != DBNull.Value ? dt.Rows[0]["SUNDAY"].ToString() : "";

                    model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                    model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                    model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                    model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                    model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                    model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                    model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";

                    DataTable dtl = await dbdal.getCheckRoutine_Data(id, "D"); //get d data
                    if (dtl != null && dtl.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtl.Rows.Count; i++)
                        {
                            CheckRoutineProdLstModel infoObjAdd = new CheckRoutineProdLstModel();

                            infoObjAdd.MM_CHECKROUTINE_D_ID = Convert.ToInt32(dtl.Rows[i]["MM_CHECKROUTINE_D_ID"]);
                            infoObjAdd.MM_CHECKROUTINE_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_CHECKROUTINE_H_ID"]);
                            infoObjAdd.PRODLINENO_ID = Convert.ToInt32(dtl.Rows[i]["PRODLINENO_ID"]);
                            infoObjAdd.PRODLINE = dtl.Rows[i]["PRODLINE"] != DBNull.Value ? dtl.Rows[i]["PRODLINE"].ToString() : "";
                            infoObjAdd.PRODLINENO = dtl.Rows[i]["PRODLINENO"] != DBNull.Value ? dtl.Rows[i]["PRODLINENO"].ToString() : "";

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
                model = new CheckRoutineModel();
            }

            model.CheckRoutineProdLstModel = listItemsAdd;

            model.DropdownProdGroup = await LoadDllData(0, "", "PROD_GROUP");

            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MM_CHECK_ROUTINE_ADD(string ActionType, CheckRoutineModel model)
        {
            try
            {
                ViewBag.Tittle = "Checking Routine";
                model.DropdownProdGroup = await LoadDllData(0, "", "PROD_GROUP");

                if (ActionType == "postback")
                {
                    ModelState.Clear();
                }
                if (ActionType == "Back")
                {
                    ModelState.Clear();
                    return RedirectToAction("MM_CHECK_ROUTINE_LST", "MM_CHECK_ROUTINE");
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
        public async Task<ActionResult> DraftCheckingRoutine(CheckRoutineModel model)
        {
            bool success = true;
            string message = "";


            if (ModelState.IsValid)
            {
                if (model.MM_PRODGROUP_H_ID == 0 ||
                    model.PROD_GROUP == null || model.PROD_GROUP == "" ||
                    model.PATTERN == null || model.PATTERN == "")
                {
                    success = false;
                    message += "Required field cannot empty.";
                }

                if (success)
                {
                    // insert h
                    string result1 = await dbdal.CheckRoutine_H_Maint(model);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                    else
                    {
                        if (model.CheckRoutineProdLstModel != null && model.CheckRoutineProdLstModel.Count > 0)
                        {
                            // insert d
                            foreach (var item in model.CheckRoutineProdLstModel)
                            {
                                int prodid = Convert.ToInt32(item.PRODLINENO_ID);
                                string prodline = item.PRODLINE != null ? item.PRODLINE.ToString() : "";
                                string lineno = item.PRODLINENO != null ? item.PRODLINENO.ToString() : "";
                                string rectype = item.RECORD_TYP != null ? item.RECORD_TYP.ToString() : "";

                                string result2 = await dbdal.CheckRoutine_D_Maint(item.MM_CHECKROUTINE_D_ID, Convert.ToInt32(result1), prodid, rectype);
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
        public async Task<ActionResult> MM_CHECK_ROUTINE_PRODLINE_POPOUT(string id)
        {
            if (TempData["Error"] != null) { ViewBag.Error = TempData["Error"].ToString(); }
            if (TempData["Result"] != null) { ViewBag.Result = TempData["Result"].ToString(); }

            var model = new CheckRoutineModel();

            if (id == null || id == "")
            {
                id = "0";
            }

            DataTable dt = await dbdal.getProdlinePOPUP(Convert.ToInt32(id));

            List<ProductLineModel> listItemsAdd = new List<ProductLineModel>();
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ProductLineModel infoObjAdd = new ProductLineModel();

                    infoObjAdd.PRODLINENO_ID = Convert.ToInt32(dt.Rows[i]["PRODLINENO_ID"]);
                    infoObjAdd.PRODLINE = dt.Rows[i]["PRODLINE"] != DBNull.Value ? dt.Rows[i]["PRODLINE"].ToString() : "";
                    infoObjAdd.PRODLINENO = dt.Rows[i]["PRODLINENO"] != DBNull.Value ? dt.Rows[i]["PRODLINENO"].ToString() : "";


                    infoObjAdd.IS_EXIST = "N";

                    // Adding.
                    listItemsAdd.Add(infoObjAdd);

                }

            }

            model.ProductLineModel = listItemsAdd;

            return View(model);
        }

        #endregion

        #region DELETE

        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> deleteCheckRoutine(List<string> lstid)
        {
            bool success = true;
            string message = "";

            CheckRoutineModel modelh = new CheckRoutineModel();

            if (lstid.Count < 1)
            {
                success = false;
                message = "Please select a record first to delete.";
            }

            if (success && lstid.Count > 0)
            {
                foreach (var item in lstid)
                {
                    modelh.MM_CHECKROUTINE_H_ID = Convert.ToInt32(item);
                    modelh.RECORD_TYP = "5";

                    modelh.PROD_GROUP = "";
                    modelh.PATTERN = "";
                    modelh.WEEKORDAY = "";
                    modelh.WHOLEWEEK = "";

                    modelh.MONDAY = "";
                    modelh.TUESDAY = "";
                    modelh.WEDNESDAY = "";
                    modelh.THURSDAY = "";
                    modelh.FRIDAY = "";
                    modelh.SATURDAY = "";
                    modelh.SUNDAY = "";

                    //delete h
                    string result1 = await dbdal.CheckRoutine_H_Maint(modelh);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                    else
                    {
                        //delete d
                        string result2 = await dbdal.CheckRoutine_D_Maint(0, Convert.ToInt32(item), 0, "2");
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
        public async Task<ActionResult> MM_CHECK_ROUTINE_AUDIT(string id)
        {
            TempData["SQ_ID"] = id;
            CommonFunction common = new CommonFunction();
            var param = new { pTable = "PVIEW_MM_CHECKROUTINE_A", pKeyValue = id };
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