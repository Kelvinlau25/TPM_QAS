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

namespace TPM_QAS.Controllers
{
    public class MM_COMMON_GROUPController : Controller
    {
        DB dbmain = new DB();
        MM_COMMON_GROUP_DAL dbdal = new MM_COMMON_GROUP_DAL();

        #region LIST

        [SessionExpire]
        public async Task<ActionResult> MM_COMMON_GROUP_LST(string Deleted = "0")
        {
            ViewBag.Tittle = "Common Group";
            TempData["STATUS_DDL"] = await dbmain.PSP_COMMON_DDL("CMN_LST_STATUS", "", "", "", "", "", "");

            CommonFunction common = new CommonFunction();
            var param = new { Table = "PVIEW_MM_COMMONGROUP", TableID = "", Search = "", Value = "", SortField = "MM_COMMONGROUP_H_ID", Direction = "0", FrmRowno = "1", ToRowno = "500000", Deleted = Deleted };
            List<CommonGroupModel> model = await common.PSP_COMMON_DAPPER<CommonGroupModel>("PSP_COMMON_LIST", CommandType.StoredProcedure, param) ?? new List<CommonGroupModel>();

            ViewBag.Deleted = Deleted;
            return View(model);
        }

        #endregion

        #region VIEW/EDIT

        [SessionExpire]
        public async Task<ActionResult> MM_COMMON_GROUP_DETAIL(string id)
        {
            ViewBag.Tittle = "Common Group";
            var model = new CommonGroupModel();

            DataTable dt = await dbdal.getCommonGroup_Data(id, "H"); //get header data

            if (dt != null && dt.Rows.Count > 0)
            {
                model.MM_COMMONGROUP_H_ID = Convert.ToInt32(dt.Rows[0]["MM_COMMONGROUP_H_ID"]);
                model.CATEGORY = dt.Rows[0]["CATEGORY"] != DBNull.Value ? dt.Rows[0]["CATEGORY"].ToString() : "";

                model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";
            }

            DataTable dtl = await dbdal.getCommonGroup_Data(id, "D"); //get d data
            List<CommonGroupLstModel> listItemsAdd = new List<CommonGroupLstModel>();
            if (dtl != null && dtl.Rows.Count > 0)
            {
                for (int i = 0; i < dtl.Rows.Count; i++)
                {
                    CommonGroupLstModel infoObjAdd = new CommonGroupLstModel();

                    infoObjAdd.MM_COMMONGROUP_D_ID = Convert.ToInt32(dtl.Rows[i]["MM_COMMONGROUP_D_ID"]);
                    infoObjAdd.MM_COMMONGROUP_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_COMMONGROUP_H_ID"]);
                    infoObjAdd.SUB_CATEGORY = dtl.Rows[i]["SUB_CATEGORY"] != DBNull.Value ? dtl.Rows[i]["SUB_CATEGORY"].ToString() : "";
                    infoObjAdd.ITEMS = dtl.Rows[i]["ITEMS"] != DBNull.Value ? dtl.Rows[i]["ITEMS"].ToString() : "";

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

            model.CommonGroupLstModel = listItemsAdd;

            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MM_COMMON_GROUP_DETAIL(string ActionType, CommonGroupModel model)
        {
            ViewBag.Tittle = "Common Group";
            try
            {
                if (ActionType == "postback")
                {
                    ModelState.Clear();
                }
                if (ActionType == "Back")
                {
                    ModelState.Clear();
                    return RedirectToAction("MM_COMMON_GROUP_LST", "MM_COMMON_GROUP");
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
        public async Task<ActionResult> InsertUpdateCommonGroup(CommonGroupModel model)
        {
            bool success = true;
            string message = "";


            if (ModelState.IsValid)
            {
                if (model.CATEGORY == null || model.CATEGORY == "" ||
                    model.CommonGroupLstModel == null || model.CommonGroupLstModel.Count < 1)
                {
                    success = false;
                    message += "Required field cannot empty.";
                }

                if (success)
                {
                    // insert h
                    string result1 = await dbdal.CommonGroup_H_Maint(model);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                    else
                    {
                        // insert d
                        foreach (var item in model.CommonGroupLstModel)
                        {
                            string subcategory = item.SUB_CATEGORY != null ? item.SUB_CATEGORY.ToString() : "";
                            string itemname = item.ITEMS != null ? item.ITEMS.ToString() : "";
                            string rectype = item.RECORD_TYP != null ? item.RECORD_TYP.ToString() : "";

                            if (rectype == "0")
                            {
                                rectype = "3";
                            }

                            string result2 = await dbdal.CommonGroup_D_Maint(item.MM_COMMONGROUP_D_ID, Convert.ToInt32(result1), subcategory, itemname, rectype);
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
        public async Task<ActionResult> MM_COMMON_GROUP_ADD(string id = "")
        {
            ViewBag.Tittle = "Common Group";
            var model = new CommonGroupModel();
            List<CommonGroupLstModel> listItemsAdd = new List<CommonGroupLstModel>();

            if (!string.IsNullOrWhiteSpace(id) && CommonMethod.isNumeric(id))
            {
                DataTable dt = await dbdal.getCommonGroup_Data(id, "H"); //get header data

                if (dt != null && dt.Rows.Count > 0)
                {
                    model.MM_COMMONGROUP_H_ID = Convert.ToInt32(dt.Rows[0]["MM_COMMONGROUP_H_ID"]);
                    model.CATEGORY = dt.Rows[0]["CATEGORY"] != DBNull.Value ? dt.Rows[0]["CATEGORY"].ToString() : "";

                    model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                    model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                    model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                    model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                    model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                    model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                    model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";

                    DataTable dtl = await dbdal.getCommonGroup_Data(id, "D"); //get d data
                    if (dtl != null && dtl.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtl.Rows.Count; i++)
                        {
                            CommonGroupLstModel infoObjAdd = new CommonGroupLstModel();

                            infoObjAdd.MM_COMMONGROUP_D_ID = Convert.ToInt32(dtl.Rows[i]["MM_COMMONGROUP_D_ID"]);
                            infoObjAdd.MM_COMMONGROUP_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_COMMONGROUP_H_ID"]);
                            infoObjAdd.SUB_CATEGORY = dtl.Rows[i]["SUB_CATEGORY"] != DBNull.Value ? dtl.Rows[i]["SUB_CATEGORY"].ToString() : "";
                            infoObjAdd.ITEMS = dtl.Rows[i]["ITEMS"] != DBNull.Value ? dtl.Rows[i]["ITEMS"].ToString() : "";

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
                model = new CommonGroupModel();
            }

            model.CommonGroupLstModel = listItemsAdd;
            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MM_COMMON_GROUP_ADD(string ActionType, CommonGroupModel model)
        {
            ViewBag.Tittle = "Common Group";
            try
            {
                if (ActionType == "postback")
                {
                    ModelState.Clear();
                }
                if (ActionType == "Back")
                {
                    ModelState.Clear();
                    return RedirectToAction("MM_COMMON_GROUP_LST", "MM_COMMON_GROUP");
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
        public async Task<ActionResult> DraftCommonGroup(CommonGroupModel model)
        {
            bool success = true;
            string message = "";


            if (ModelState.IsValid)
            {
                if (model.CATEGORY == null || model.CATEGORY == "")
                {
                    success = false;
                    message += "Required field cannot empty.";
                }

                if (success)
                {
                    // insert h
                    string result1 = await dbdal.CommonGroup_H_Maint(model);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                    else
                    {
                        if (model.CommonGroupLstModel != null && model.CommonGroupLstModel.Count > 0)
                        {
                            // insert d
                            foreach (var item in model.CommonGroupLstModel)
                            {
                                string subcategory = item.SUB_CATEGORY != null ? item.SUB_CATEGORY.ToString() : "";
                                string itemname = item.ITEMS != null ? item.ITEMS.ToString() : "";
                                string rectype = item.RECORD_TYP != null ? item.RECORD_TYP.ToString() : "";

                                string result2 = await dbdal.CommonGroup_D_Maint(item.MM_COMMONGROUP_D_ID, Convert.ToInt32(result1), subcategory, itemname, rectype);
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

        #region POPUP

        [SessionExpire]
        public async Task<ActionResult> MM_COMMON_GROUP_COLOR_POPOUT(string type)
        {
            if (TempData["Error"] != null) { ViewBag.Error = TempData["Error"].ToString(); }
            if (TempData["Result"] != null) { ViewBag.Result = TempData["Result"].ToString(); }

            var model = new CommonGroupModel();

            DataTable dt = await dbdal.getPropertyColorPOPUP(type);

            List<PropertyColorLstModel> listItemsAdd = new List<PropertyColorLstModel>();
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    PropertyColorLstModel infoObjAdd = new PropertyColorLstModel();

                    infoObjAdd.PROPCOLOR = dt.Rows[i]["PROPCOLOR"] != DBNull.Value ? dt.Rows[i]["PROPCOLOR"].ToString() : "";

                    // Adding.
                    listItemsAdd.Add(infoObjAdd);

                }

            }

            model.PropertyColorLstModel = listItemsAdd;

            return View(model);
        }

        #endregion

        #region DELETE

        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> deleteCommonGroup(List<string> lstid)
        {
            bool success = true;
            string message = "";

            CommonGroupModel modelh = new CommonGroupModel();

            if (lstid.Count < 1)
            {
                success = false;
                message = "Please select a record first to delete.";
            }

            if (success && lstid.Count > 0)
            {
                foreach (var item in lstid)
                {
                    modelh.MM_COMMONGROUP_H_ID = Convert.ToInt32(item);
                    modelh.RECORD_TYP = "5";

                    modelh.CATEGORY = "";

                    //delete h
                    string result1 = await dbdal.CommonGroup_H_Maint(modelh);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                    else
                    {
                        //delete d
                        string result2 = await dbdal.CommonGroup_D_Maint(0, Convert.ToInt32(item), "", "", "2");
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
        public async Task<ActionResult> MM_COMMON_GROUP_AUDIT(string id)
        {
            TempData["SQ_ID"] = id;
            CommonFunction common = new CommonFunction();
            var param = new { pTable = "PVIEW_MM_COMMONGROUP_A", pKeyValue = id };
            List<AuditTrailModel> AuditTrailModel = await common.PSP_COMMON_DAPPER<AuditTrailModel>("PSP_COMMON_AUDIT", CommandType.StoredProcedure, param) ?? new List<AuditTrailModel>();

            return View(AuditTrailModel);
        }

        #endregion
    }
}