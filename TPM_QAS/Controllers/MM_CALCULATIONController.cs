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
    public class MM_CALCULATIONController : Controller
    {
        DB dbmain = new DB();
        MM_CALCULATION_DAL dbdal = new MM_CALCULATION_DAL();

        #region LIST

        [SessionExpire]
        public async Task<ActionResult> MM_CALCULATION_LST(string Deleted = "0")
        {
            TempData["STATUS_DDL"] = await dbmain.PSP_COMMON_DDL("CMN_LST_STATUS", "", "", "", "", "", "");

            CommonFunction common = new CommonFunction();
            var param = new { Table = "PVIEW_MM_CALC", TableID = "", Search = "", Value = "", SortField = "MM_CALC_ID", Direction = "0", FrmRowno = "1", ToRowno = "500000", Deleted = Deleted };
            List<CalculationModel> model = await common.PSP_COMMON_DAPPER<CalculationModel>("PSP_COMMON_LIST", CommandType.StoredProcedure, param) ?? new List<CalculationModel>();

            ViewBag.Deleted = Deleted;
            return View(model);
        }

        #endregion

        #region VIEW/EDIT

        [SessionExpire]
        public async Task<ActionResult> MM_CALCULATION_DETAIL(string id)
        {
            ViewBag.Tittle = "Calculation";
            var model = new CalculationModel();

            DataTable dt = await dbdal.getCalculation_Data(id);
            if (dt != null && dt.Rows.Count > 0)
            {
                model.MM_CALC_ID = Convert.ToInt32(dt.Rows[0]["MM_CALC_ID"]);
                model.MM_PROPERTIES_H_ID = Convert.ToInt32(dt.Rows[0]["MM_PROPERTIES_H_ID"]);
                model.PROPERTIES = dt.Rows[0]["PROPERTIES"] != DBNull.Value ? dt.Rows[0]["PROPERTIES"].ToString() : "";
                model.PROP_ITEM = dt.Rows[0]["PROP_ITEM"] != DBNull.Value ? dt.Rows[0]["PROP_ITEM"].ToString() : "";
                model.COMP_GROUP = dt.Rows[0]["COMP_GROUP"] != DBNull.Value ? dt.Rows[0]["COMP_GROUP"].ToString() : "";
                model.FORMULA = dt.Rows[0]["FORMULA"] != DBNull.Value ? dt.Rows[0]["FORMULA"].ToString() : "";

                model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";
            }

            model.DropdownProperty = await LoadDllData(0, "", "PROPERTIES");
            model.DropdownItem = await LoadDllData(0, model.PROPERTIES, "PROP_ITEM");
            ViewBag.FROM = "PRE";

            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MM_CALCULATION_DETAIL(string ActionType, CalculationModel model)
        {
            ViewBag.Tittle = "Calculation";
            try
            {
                DataTable dt = await dbdal.getCalculation_Data(model.MM_CALC_ID.ToString()); //get header only
                if (dt != null && dt.Rows.Count > 0)
                {
                    model.MM_CALC_ID = Convert.ToInt32(dt.Rows[0]["MM_CALC_ID"]);
                    model.MM_PROPERTIES_H_ID = Convert.ToInt32(dt.Rows[0]["MM_PROPERTIES_H_ID"]);
                    //model.PROPERTIES = dt.Rows[0]["PROPERTIES"] != DBNull.Value ? dt.Rows[0]["PROPERTIES"].ToString() : "";
                    model.PROP_ITEM = dt.Rows[0]["PROP_ITEM"] != DBNull.Value ? dt.Rows[0]["PROP_ITEM"].ToString() : "";
                    model.COMP_GROUP = dt.Rows[0]["COMP_GROUP"] != DBNull.Value ? dt.Rows[0]["COMP_GROUP"].ToString() : "";
                    model.FORMULA = dt.Rows[0]["FORMULA"] != DBNull.Value ? dt.Rows[0]["FORMULA"].ToString() : "";

                    model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                    model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                    model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                    model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                    model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                    model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                    model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";

                }

                model.DropdownProperty = await LoadDllData(0, "", "PROPERTIES");

                if (model.PROPERTIES != "" && model.PROPERTIES != null && !model.PROPERTIES.Contains("Select an option"))
                {
                    model.DropdownItem = await LoadDllData(0, model.PROPERTIES, "PROP_ITEM");
                }
                else
                {
                    model.DropdownItem = new List<SelectListItem>();
                }

                if (ActionType == "postback")
                {
                    ModelState.Clear();
                }
                if (ActionType == "Back")
                {
                    ModelState.Clear();
                    return RedirectToAction("MM_CALCULATION_LST", "MM_CALCULATION");
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

            ViewBag.FROM = "POST";
            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> UpdateInsertCalculation(CalculationModel model)
        {
            bool success = true;
            string message = "";


            if (ModelState.IsValid)
            {
                if (model.PROPERTIES == null || model.PROPERTIES == "" ||
                    model.COMP_GROUP == null || model.COMP_GROUP == "" ||
                    model.FORMULA == null || model.FORMULA == "" ||
                    model.MM_PROPERTIES_H_ID == 0)
                {
                    success = false;
                    message += "Required field cannot empty.";
                }

                if (success)
                {
                    // insert h
                    string result1 = await dbdal.Calculation_H_Maint(model);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
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
                message += "Error in saving data : Modal State not valid.";
            }

            var data = new { success = success, message = message };

            return Json(data);
        }

        #endregion

        #region ADD

        [SessionExpire]
        public async Task<ActionResult> MM_CALCULATION_ADD(string id = "")
        {
            ViewBag.Tittle = "Calculation";
            var model = new CalculationModel();

            model.DropdownProperty = await LoadDllData(0, "", "PROPERTIES");
            model.DropdownItem = new List<SelectListItem>();

            if (!string.IsNullOrWhiteSpace(id) && CommonMethod.isNumeric(id))
            {
                DataTable dt = await dbdal.getCalculation_Data(id); //get header data

                if (dt != null && dt.Rows.Count > 0)
                {
                    model.MM_CALC_ID = Convert.ToInt32(dt.Rows[0]["MM_CALC_ID"]);
                    model.MM_PROPERTIES_H_ID = Convert.ToInt32(dt.Rows[0]["MM_PROPERTIES_H_ID"]);
                    model.PROPERTIES = dt.Rows[0]["PROPERTIES"] != DBNull.Value ? dt.Rows[0]["PROPERTIES"].ToString() : "";
                    model.PROP_ITEM = dt.Rows[0]["PROP_ITEM"] != DBNull.Value ? dt.Rows[0]["PROP_ITEM"].ToString() : "";
                    model.COMP_GROUP = dt.Rows[0]["COMP_GROUP"] != DBNull.Value ? dt.Rows[0]["COMP_GROUP"].ToString() : "";
                    model.FORMULA = dt.Rows[0]["FORMULA"] != DBNull.Value ? dt.Rows[0]["FORMULA"].ToString() : "";

                    model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                    model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                    model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                    model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                    model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                    model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                    model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";

                    model.DropdownItem = await LoadDllData(0, model.PROPERTIES, "PROP_ITEM");
                }

            }
            else
            {
                model = new CalculationModel();
                model.DropdownProperty = await LoadDllData(0, "", "PROPERTIES");
                model.DropdownItem = new List<SelectListItem>();
            }

            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MM_CALCULATION_ADD(string ActionType, CalculationModel model)
        {
            ViewBag.Tittle = "Calculation";
            try
            {
                model.DropdownProperty = await LoadDllData(0, "", "PROPERTIES");
                if (model.PROPERTIES != "" && model.PROPERTIES != null && !model.PROPERTIES.Contains("Select an option"))
                {
                    model.DropdownItem = await LoadDllData(0, model.PROPERTIES, "PROP_ITEM");
                }
                else
                {
                    model.DropdownItem = new List<SelectListItem>();
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
        public async Task<ActionResult> DraftCalculation(CalculationModel model)
        {
            bool success = true;
            string message = "";

            model.FORMULA = model.FORMULA != null ? model.FORMULA.ToString() : "";

            //if (ModelState.IsValid)
            //{
            if (model.PROPERTIES == null || model.PROPERTIES == "" ||
                model.COMP_GROUP == null || model.COMP_GROUP == "" ||
                model.MM_PROPERTIES_H_ID == 0)
            {
                success = false;
                message += "Required field cannot empty.";
            }

            if (success)
            {
                // insert h
                string result1 = await dbdal.Calculation_H_Maint(model);
                if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                {
                    success = false;
                    message += result1;
                }
            }
            else
            {
                success = false;
                message += "Error in saving data.";
            }
            //}
            //else
            //{
            //    success = false;
            //    message += "Error in saving data : Modal State not valid.";
            //}

            var data = new { success = success, message = message };

            return Json(data);
        }

        #endregion

        #region DELETE

        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> deleteCalculation(List<string> lstid)
        {
            bool success = true;
            string message = "";

            CalculationModel model = new CalculationModel();

            if (lstid.Count < 1)
            {
                success = false;
                message += "Please select a record first to delete.";
            }

            if (success && lstid.Count > 0)
            {
                foreach (var item in lstid)
                {
                    model.MM_CALC_ID = Convert.ToInt32(item);
                    model.RECORD_TYP = "5";

                    model.MM_PROPERTIES_H_ID = 0;
                    model.COMP_GROUP = "";
                    model.FORMULA = "";

                    string resultd = await dbdal.Calculation_H_Maint(model);
                    if (!(int.TryParse(resultd, out int num2) && resultd != "0"))
                    {
                        success = false;
                        message += resultd;
                    }
                }
            }

            var data = new { success = success, message = message };

            return Json(data);
        }

        #endregion

        #region AUDIT

        [SessionExpire]
        public async Task<ActionResult> MM_CALCULATION_AUDIT(string id)
        {
            TempData["SQ_ID"] = id;
            CommonFunction common = new CommonFunction();
            var param = new { pTable = "PVIEW_MM_CALC_A", pKeyValue = id };
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