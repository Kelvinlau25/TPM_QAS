using DBModel;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using TPM_QAS.DAL;
using TPM_QAS.Filters;
using TPM_QAS.Helpers;
using TPM_QAS.Models;

namespace TPM_QAS.Controllers
{
    public class COA_MM_SUPPLIERController : Controller
    {
        DB dbmain = new DB();
        COA_MM_SUPPLIER_DAL dbdal = new COA_MM_SUPPLIER_DAL();

        #region LIST

        [SessionExpire]
        public async Task<ActionResult> COA_MM_SUPPLIER_LST(string Deleted = "0")
        {
            ViewBag.Tittle = "Supplier Raw Material Specifications";
            if(Deleted == "1")
            {
                TempData["STATUS_DDL"] = await dbmain.PSP_COMMON_DDL("CMN_LST_STATUS", "", "", "", "", "", "");
            }
            else
            {
                TempData["STATUS_DDL"] = await dbmain.PSP_COMMON_DDL("CMN_LST_STATUS2", "", "", "", "", "", "");
            }
            
            CommonFunction common = new CommonFunction();
            var param = new { Table = "PVIEW_MM_SUPPLIER", TableID = "", Search = "", Value = "", SortField = "MM_SUPPLIER_H_ID", Direction = "0", FrmRowno = "1", ToRowno = "500000", Deleted = Deleted };
            List<COASupplierModel> model = await common.PSP_COMMON_DAPPER<COASupplierModel>("PSP_COMMON_LIST", CommandType.StoredProcedure, param) ?? new List<COASupplierModel>();

            ViewBag.Deleted = Deleted;
            return View(model);
        }

        #endregion

        #region VIEW/EDIT

        [SessionExpire]
        public async Task<ActionResult> COA_MM_SUPPLIER_DETAIL(string id)
        {
            ViewBag.Tittle = "Supplier Raw Material Specifications";
            var model = new COASupplierModel();

            DataTable dt = await dbdal.getSupplier_Data(id, "H"); //get header data

            if (dt != null && dt.Rows.Count > 0)
            {
                model.MM_SUPPLIER_H_ID = Convert.ToInt32(dt.Rows[0]["MM_SUPPLIER_H_ID"]);
                model.SUPPLIER_NAME = dt.Rows[0]["SUPPLIER_NAME"] != DBNull.Value ? dt.Rows[0]["SUPPLIER_NAME"].ToString() : "";

                model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";
            }

            DataTable dtl = await dbdal.getSupplier_Data(id, "D"); //get d data
            List<SupplierLstModel> listItemsAdd = new List<SupplierLstModel>();
            if (dtl != null && dtl.Rows.Count > 0)
            {
                for (int i = 0; i < dtl.Rows.Count; i++)
                {
                    SupplierLstModel infoObjAdd = new SupplierLstModel();

                    infoObjAdd.MM_SUPPLIER_D_ID = Convert.ToInt32(dtl.Rows[i]["MM_SUPPLIER_D_ID"]);
                    infoObjAdd.MM_SUPPLIER_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_SUPPLIER_H_ID"]);
                    infoObjAdd.KEY_TYPE = dtl.Rows[i]["KEY_TYPE"] != DBNull.Value ? dtl.Rows[i]["KEY_TYPE"].ToString() : "";
                    infoObjAdd.SUPPLIER_KEY = dtl.Rows[i]["SUPPLIER_KEY"] != DBNull.Value ? dtl.Rows[i]["SUPPLIER_KEY"].ToString() : "";

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

            model.SupplierLstModel = listItemsAdd;

            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> COA_MM_SUPPLIER_DETAIL(string ActionType, COASupplierModel model)
        {
            ViewBag.Tittle = "Supplier Raw Material Specifications";
            try
            {
                if (ActionType == "postback")
                {
                    ModelState.Clear();
                }
                if (ActionType == "Back")
                {
                    ModelState.Clear();
                    return RedirectToAction("COA_MM_SUPPLIER_LST", "COA_MM_SUPPLIER");
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
        public async Task<ActionResult> InsertUpdateSupplier(COASupplierModel model)
        {
            bool success = true;
            string message = "";


            if (ModelState.IsValid)
            {
                if (model.SUPPLIER_NAME == null ||
                    model.SupplierLstModel == null || model.SupplierLstModel.Count < 1)
                {
                    success = false;
                    message += "Required field cannot empty.";
                }

                if (success)
                {
                    // insert h
                    string result1 = await dbdal.Supplier_H_Maint(model);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                    else
                    {
                        // insert d
                        foreach (var item in model.SupplierLstModel)
                        {
                            string keytype = item.KEY_TYPE != null ? item.KEY_TYPE.ToString() : ""; 
                            string suppkey = item.SUPPLIER_KEY != null ? item.SUPPLIER_KEY.ToString() : "";
                            string rectype = item.RECORD_TYP != null ? item.RECORD_TYP.ToString() : "";

                            if (rectype == "0")
                            {
                                rectype = "3";
                            }

                            string result2 = await dbdal.Supplier_D_Maint(item.MM_SUPPLIER_D_ID, Convert.ToInt32(result1), keytype, suppkey, rectype);
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
                message += "Error in saving data : Modal State not valid. ";

                var allErrors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in allErrors)
                {
                    message += error.ErrorMessage;
                }
            }

            var data = new { success = success, message = message };

            return Json(data);
        }

        #endregion

        #region ADD

        [SessionExpire]
        public async Task<ActionResult> COA_MM_SUPPLIER_ADD(string id = "")
        {
            ViewBag.Tittle = "Supplier Raw Material Specifications";
            var model = new COASupplierModel();
            List<SupplierLstModel> listItemsAdd = new List<SupplierLstModel>();

            DataTable dtl = await dbdal.getSupplier_Data("0", "A"); //get d data
            if (dtl != null && dtl.Rows.Count > 0)
            {
                for (int i = 0; i < dtl.Rows.Count; i++)
                {
                    SupplierLstModel infoObjAdd = new SupplierLstModel();

                    infoObjAdd.KEY_TYPE = dtl.Rows[i]["KEY_TYPE"] != DBNull.Value ? dtl.Rows[i]["KEY_TYPE"].ToString() : "";
                    infoObjAdd.SUPPLIER_KEY = dtl.Rows[i]["SUPPLIER_KEY"] != DBNull.Value ? dtl.Rows[i]["SUPPLIER_KEY"].ToString() : "";

                    // Adding.
                    listItemsAdd.Add(infoObjAdd);

                }
            }

            model = new COASupplierModel();
            model.SupplierLstModel = listItemsAdd;
            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> COA_MM_SUPPLIER_ADD(string ActionType, COASupplierModel model)
        {
            ViewBag.Tittle = "Supplier Raw Material Specifications";
            try
            {
                if (ActionType == "postback")
                {
                    ModelState.Clear();
                }
                if (ActionType == "Back")
                {
                    ModelState.Clear();
                    return RedirectToAction("COA_MM_SUPPLIER_LST", "COA_MM_SUPPLIER");
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

            return View(model);
        }

        #endregion

        #region DELETE

        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> deleteSupplier(List<string> lstid)
        {
            bool success = true;
            string message = "";

            COASupplierModel modelh = new COASupplierModel();

            if (lstid.Count < 1)
            {
                success = false;
                message = "Please select a record first to delete.";
            }

            if (success && lstid.Count > 0)
            {
                foreach (var item in lstid)
                {
                    modelh.MM_SUPPLIER_H_ID = Convert.ToInt32(item);
                    modelh.RECORD_TYP = "5";

                    modelh.SUPPLIER_NAME = "";

                    //delete h
                    string result1 = await dbdal.Supplier_H_Maint(modelh);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                    else
                    {
                        //delete d
                        string result2 = await dbdal.Supplier_D_Maint(0, Convert.ToInt32(item), "", "", "2");
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
        public async Task<ActionResult> COA_MM_SUPPLIER_AUDIT(string id)
        {
            AuditTrailModels AuditTrailModels = new AuditTrailModels();
            List<SqlParameter> _pMssql = new List<SqlParameter>();
            _pMssql.Add(new SqlParameter("@TABLE", "PVIEW_MM_SUPPLIER_A"));
            _pMssql.Add(new SqlParameter("@KEY_VALUE", id));
            _pMssql.Add(new SqlParameter("@SortColumn", "UPDATED_DATE"));
            _pMssql.Add(new SqlParameter("@SortType", "DESC"));

            string dbname = "";
            string isTest = TPM_QAS.DAL.Database.GetAppSettingStatic("isTest"];

            if (string.Equals(isTest, "TRUE", StringComparison.OrdinalIgnoreCase))
            {
                dbname = TPM_QAS.DAL.Database.GetAppSettingStatic("DEV"];
            }
            else
            {
                dbname = TPM_QAS.DAL.Database.GetAppSettingStatic("LIVE"];
            }

            AuditTrailModels = await AuditTrailHelper.AuditTrailStoreProcedureSqlAsync("PSP_GET_AUDIT_TRAIL", CommandType.StoredProcedure, _pMssql, dbname);


            ViewBag.JsonResult = AuditTrailModels.JsonData;
            ViewBag.KeyNames = AuditTrailModels.ListData;
            ViewBag.hid = id;

            return View();

            //TempData["SQ_ID"] = id;
            //CommonFunction common = new CommonFunction();
            //var param = new { pTable = "PVIEW_MM_SUPPLIER_A", pKeyValue = id };
            //List<AuditTrailModel> AuditTrailModel = await common.PSP_COMMON_DAPPER<AuditTrailModel>("PSP_COMMON_AUDIT", CommandType.StoredProcedure, param) ?? new List<AuditTrailModel>();

            //return View(AuditTrailModel);
        }

        #endregion
    }
}