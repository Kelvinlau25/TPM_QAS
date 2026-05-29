using DBModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TPM_QAS.DAL;
using TPM_QAS.Filters;
using TPM_QAS.Helpers;
using TPM_QAS.Models;

namespace TPM_QAS.Controllers
{
    public class MM_APP_ROLEController : Controller
    {
        DB dbmain = new DB();
        MM_APP_ROLE_DAL dbdal = new MM_APP_ROLE_DAL();

        #region LIST

        [SessionExpire]
        public async Task<ActionResult> MM_APP_ROLE_LST(string Deleted = "0")
        {
            TempData["STATUS_DDL"] = await dbmain.PSP_COMMON_DDL("CMN_LST_STATUS", "", "", "", "", "", "");

            CommonFunction common = new CommonFunction();
            var param = new { Table = "PVIEW_MM_APPROLE", TableID = "", Search = "", Value = "", SortField = "MM_APPROLE_H_ID", Direction = "0", FrmRowno = "1", ToRowno = "500000", Deleted = Deleted };
            List<AppRoleModel> model = await common.PSP_COMMON_DAPPER<AppRoleModel>("PSP_COMMON_LIST", System.Data.CommandType.StoredProcedure, param) ?? new List<AppRoleModel>();

            ViewBag.Deleted = Deleted;
            return View(model);
        }

        #endregion

        #region VIEW/EDIT

        [SessionExpire]
        public async Task<ActionResult>MM_APP_ROLE_DETAIL(string id)
        {
            ViewBag.Tittle = "Approval Role";
            var model = new AppRoleModel();

            DataTable dt = await dbdal.getAppRole_Data(id, "H"); //get header data

            if (dt != null && dt.Rows.Count > 0)
            {
                model.MM_APPROLE_H_ID = Convert.ToInt32(dt.Rows[0]["MM_APPROLE_H_ID"]);
                model.ROLE_NAME_ID = Convert.ToInt32(dt.Rows[0]["ROLE_NAME_ID"]);
                model.ROLE_NAME = dt.Rows[0]["ROLE_NAME"] != DBNull.Value ? dt.Rows[0]["ROLE_NAME"].ToString() : "";
                model.REMARKS = dt.Rows[0]["REMARKS"] != DBNull.Value ? dt.Rows[0]["REMARKS"].ToString() : "";

                model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";
            }

            DataTable dtl = await dbdal.getAppRole_Data(id, "D"); //get d data
            List<AppRoleEmpLstModel> listItemsAdd = new List<AppRoleEmpLstModel>();
            if (dtl != null && dtl.Rows.Count > 0)
            {
                for (int i = 0; i < dtl.Rows.Count; i++)
                {
                    AppRoleEmpLstModel infoObjAdd = new AppRoleEmpLstModel();

                    infoObjAdd.MM_APPROLE_D_ID = Convert.ToInt32(dtl.Rows[i]["MM_APPROLE_D_ID"]);
                    infoObjAdd.MM_APPROLE_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_APPROLE_H_ID"]);
                    infoObjAdd.EMP_ID = Convert.ToInt32(dtl.Rows[i]["EMP_ID"]);
                    infoObjAdd.EMP_NAME = dtl.Rows[i]["EMP_NAME"] != DBNull.Value ? dtl.Rows[i]["EMP_NAME"].ToString() : "";
                    infoObjAdd.EMP_DEPT = dtl.Rows[i]["EMP_DEPT"] != DBNull.Value ? dtl.Rows[i]["EMP_DEPT"].ToString() : "";
                    infoObjAdd.EMP_SECTION = dtl.Rows[i]["EMP_SECTION"] != DBNull.Value ? dtl.Rows[i]["EMP_SECTION"].ToString() : "";
                    infoObjAdd.EMP_EMAIL = dtl.Rows[i]["EMP_EMAIL"] != DBNull.Value ? dtl.Rows[i]["EMP_EMAIL"].ToString() : "";

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

            model.AppRoleEmpLstModel = listItemsAdd;

            model.DropdownApprovalName = await LoadDllData(0, "", "APP_ROLE");

            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult>MM_APP_ROLE_DETAMM_APP_ROLE_DETAILIL(string ActionType, AppRoleModel model)
        {
            ViewBag.Tittle = "Approval Role";
            model.DropdownApprovalName = await LoadDllData(0, "", "APP_ROLE");

            try
            {
                if (ActionType == "postback")
                {
                    ModelState.Clear();
                }
                if (ActionType == "Back")
                {
                    ModelState.Clear();
                    return RedirectToAction("MM_APP_ROLE_LST", "MM_APP_ROLE");
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
        public async Task<ActionResult>InsertUpdateAppRole(AppRoleModel model)
        {
            bool success = true;
            string message = "";


            if (ModelState.IsValid)
            {
                if (model.ROLE_NAME_ID == 0 ||
                    model.AppRoleEmpLstModel == null || model.AppRoleEmpLstModel.Count < 1)
                {
                    success = false;
                    message += "Required field cannot empty.";
                }

                if (success)
                {
                    // insert h

                    model.REMARKS = model.REMARKS != null ? model.REMARKS.ToString() : "";
                    string result1 = await dbdal.AppRole_H_Maint(model);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                    else
                    {
                        // insert d
                        foreach (var item in model.AppRoleEmpLstModel)
                        {
                            int empid = Convert.ToInt32(item.EMP_ID);
                            string empname = item.EMP_NAME != null ? item.EMP_NAME.ToString() : "";
                            string empdept = item.EMP_DEPT != null ? item.EMP_DEPT.ToString() : "";
                            string empsect = item.EMP_SECTION != null ? item.EMP_SECTION.ToString() : "";
                            string empemail = item.EMP_EMAIL != null ? item.EMP_EMAIL.ToString() : "";
                            string rectype = item.RECORD_TYP != null ? item.RECORD_TYP.ToString() : "";

                            if (rectype == "0")
                            {
                                rectype = "3";
                            }

                            string result2 = await dbdal.AppRole_D_Maint(item.MM_APPROLE_D_ID, Convert.ToInt32(result1), empid, empname, empdept, empsect, empemail, rectype);
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
        public async Task<ActionResult>MM_APP_ROLE_ADD(string id = "")
        {
            ViewBag.Tittle = "Approval Role";
            var model = new AppRoleModel();
            List<AppRoleEmpLstModel> listItemsAdd = new List<AppRoleEmpLstModel>();

            if (!string.IsNullOrWhiteSpace(id) && CommonMethod.isNumeric(id))
            {
                DataTable dt = await dbdal.getAppRole_Data(id, "H"); //get header data

                if (dt != null && dt.Rows.Count > 0)
                {
                    model.MM_APPROLE_H_ID = Convert.ToInt32(dt.Rows[0]["MM_APPROLE_H_ID"]);
                    model.ROLE_NAME_ID = Convert.ToInt32(dt.Rows[0]["ROLE_NAME_ID"]);
                    model.ROLE_NAME = dt.Rows[0]["ROLE_NAME"] != DBNull.Value ? dt.Rows[0]["ROLE_NAME"].ToString() : "";
                    model.REMARKS = dt.Rows[0]["REMARKS"] != DBNull.Value ? dt.Rows[0]["REMARKS"].ToString() : "";

                    model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                    model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                    model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                    model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                    model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                    model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                    model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";

                    DataTable dtl = await dbdal.getAppRole_Data(id, "D"); //get d data
                    if (dtl != null && dtl.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtl.Rows.Count; i++)
                        {
                            AppRoleEmpLstModel infoObjAdd = new AppRoleEmpLstModel();

                            infoObjAdd.MM_APPROLE_D_ID = Convert.ToInt32(dtl.Rows[i]["MM_APPROLE_D_ID"]);
                            infoObjAdd.MM_APPROLE_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_APPROLE_H_ID"]);
                            infoObjAdd.EMP_ID = Convert.ToInt32(dtl.Rows[i]["EMP_ID"]);
                            infoObjAdd.EMP_NAME = dtl.Rows[i]["EMP_NAME"] != DBNull.Value ? dtl.Rows[i]["EMP_NAME"].ToString() : "";
                            infoObjAdd.EMP_DEPT = dtl.Rows[i]["EMP_DEPT"] != DBNull.Value ? dtl.Rows[i]["EMP_DEPT"].ToString() : "";
                            infoObjAdd.EMP_SECTION = dtl.Rows[i]["EMP_SECTION"] != DBNull.Value ? dtl.Rows[i]["EMP_SECTION"].ToString() : "";
                            infoObjAdd.EMP_EMAIL = dtl.Rows[i]["EMP_EMAIL"] != DBNull.Value ? dtl.Rows[i]["EMP_EMAIL"].ToString() : "";

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
                model = new AppRoleModel();
            }

            model.AppRoleEmpLstModel = listItemsAdd;
            model.DropdownApprovalName = await LoadDllData(0, "", "APP_ROLE");
            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult>MM_APP_ROLE_ADD(string ActionType, AppRoleModel model)
        {
            try
            {
                ViewBag.Tittle = "Approval Role";
                model.DropdownApprovalName = await LoadDllData(0, "", "APP_ROLE");

                if (ActionType == "postback")
                {
                    ModelState.Clear();
                }
                if (ActionType == "Back")
                {
                    ModelState.Clear();
                    return RedirectToAction("MM_APP_ROLE_LST", "MM_APP_ROLE");
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
        public async Task<ActionResult>DraftAppRole(AppRoleModel model)
        {
            bool success = true;
            string message = "";


            if (ModelState.IsValid)
            {
                if (model.ROLE_NAME_ID == 0 )
                {
                    success = false;
                    message += "Required field cannot empty.";
                }

                if (success)
                {
                    // insert h

                    model.REMARKS = model.REMARKS != null ? model.REMARKS.ToString() : "";
                    string result1 = await dbdal.AppRole_H_Maint(model);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                    else
                    {
                        if (model.AppRoleEmpLstModel != null && model.AppRoleEmpLstModel.Count > 0)
                        {
                            // insert d
                            foreach (var item in model.AppRoleEmpLstModel)
                            {
                                int empid = Convert.ToInt32(item.EMP_ID);
                                string empname = item.EMP_NAME != null ? item.EMP_NAME.ToString() : "";
                                string empdept = item.EMP_DEPT != null ? item.EMP_DEPT.ToString() : "";
                                string empsect = item.EMP_SECTION != null ? item.EMP_SECTION.ToString() : "";
                                string empemail = item.EMP_EMAIL != null ? item.EMP_EMAIL.ToString() : "";
                                string rectype = item.RECORD_TYP != null ? item.RECORD_TYP.ToString() : "";

                                string result2 = await dbdal.AppRole_D_Maint(item.MM_APPROLE_D_ID, Convert.ToInt32(result1), empid, empname, empdept, empsect, empemail, rectype);
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
        public async Task<ActionResult> MM_APP_ROLE_EMP_POPOUT(string id)
        {
            if (TempData["Error"] != null) { ViewBag.Error = TempData["Error"].ToString(); }
            if (TempData["Result"] != null) { ViewBag.Result = TempData["Result"].ToString(); }

            var model = new AppRoleModel();

            CommonFunction common = new CommonFunction();
            var param = new { Table = "PVIEW_MM_APPROLE_EMP_POPUP", TableID = "", Search = "", Value = "", SortField = "EMP_NAME", Direction = "1", FrmRowno = "1", ToRowno = "500000", Deleted = 0 };
            List<AppRoleEmpLstModel> AppRoleEmpLstModel = await common.PSP_COMMON_DAPPER<AppRoleEmpLstModel>("PSP_COMMON_LIST", CommandType.StoredProcedure, param) ?? new List<AppRoleEmpLstModel>();

            if (id != "0" && id != null && id != "")
            {
                DataTable dtchk = await dbdal.getAppRole_Data(id, "D");

                if (dtchk != null && dtchk.Rows.Count > 0)
                {
                    var itemsToRemove = new List<AppRoleEmpLstModel>();

                    foreach (var emp in AppRoleEmpLstModel)
                    {
                        int EMP_ID = emp.EMP_ID;

                        DataRow[] matchingRows = dtchk.Select("EMP_ID = '" + EMP_ID + "'");

                        if (matchingRows.Length > 0)
                        {
                            emp.IS_EXIST = "Y";
                            itemsToRemove.Add(emp);
                        }
                    }

                    foreach (var emp in itemsToRemove)
                    {
                        AppRoleEmpLstModel.Remove(emp);
                    }
                }
            }

            model.AppRoleEmpLstModel = AppRoleEmpLstModel;

            return View(model);
        }

        #endregion

        #region DELETE

        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult>deleteAppRole(List<string> lstid)
        {
            bool success = true;
            string message = "";

            AppRoleModel modelh = new AppRoleModel();

            if (lstid.Count < 1)
            {
                success = false;
                message = "Please select a record first to delete.";
            }

            if (success && lstid.Count > 0)
            {
                foreach (var item in lstid)
                {
                    modelh.MM_APPROLE_H_ID = Convert.ToInt32(item);
                    modelh.RECORD_TYP = "5";

                    modelh.ROLE_NAME_ID = 0;
                    modelh.REMARKS = "";

                    //delete h
                    string result1 = await dbdal.AppRole_H_Maint(modelh);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                    else
                    {
                        //delete d
                        string result2 = await dbdal.AppRole_D_Maint(0, Convert.ToInt32(item), 0, "", "", "", "", "2");
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
        public async Task<ActionResult> MM_APP_ROLE_AUDIT(string id)
        {
            TempData["SQ_ID"] = id;
            CommonFunction common = new CommonFunction();
            var param = new { pTable = "PVIEW_MM_APPROLE_A", pKeyValue = id };
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