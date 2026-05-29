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
    public class MM_PRODPACKController : Controller
    {
        DB dbmain = new DB();
        MM_PRODPACK_DAL dbdal = new MM_PRODPACK_DAL();

        #region LIST

        [SessionExpire]
        public async Task<ActionResult> MM_PRODPACK_LST(string Deleted = "0")
        {
            TempData["STATUS_DDL"] = await dbmain.PSP_COMMON_DDL("CMN_LST_STATUS", "", "", "", "", "", "");

            CommonFunction common = new CommonFunction();
            var param = new { Table = "PVIEW_MM_PRODPACK", TableID = "", Search = "", Value = "", SortField = "MM_PRODPACK_ID", Direction = "0", FrmRowno = "1", ToRowno = "500000", Deleted = Deleted };
            List<ProdPackModel> model = await common.PSP_COMMON_DAPPER<ProdPackModel>("PSP_COMMON_LIST", CommandType.StoredProcedure, param) ?? new List<ProdPackModel>();

            ViewBag.Deleted = Deleted;
            return View(model);
        }

        #endregion

        #region VIEW/EDIT

        [SessionExpire]
        public async Task<ActionResult> MM_PRODPACK_DETAIL(string id)
        {
            ViewBag.Tittle = "Product Packing";
            var model = new ProdPackModel();

            DataTable dt = await dbdal.getProdPack_Data(id);
            if (dt != null && dt.Rows.Count > 0)
            {
                model.MM_PRODPACK_ID = Convert.ToInt32(dt.Rows[0]["MM_PRODPACK_ID"]);
                model.PACKING_TYPE_ID = Convert.ToInt32(dt.Rows[0]["PACKING_TYPE_ID"]);
                model.PACKING_TYPE = dt.Rows[0]["PACKING_TYPE"] != DBNull.Value ? dt.Rows[0]["PACKING_TYPE"].ToString() : "";
                model.PROD_PACK_NAME_STRING = dt.Rows[0]["PROD_PACK_NAME_STRING"] != DBNull.Value ? dt.Rows[0]["PROD_PACK_NAME_STRING"].ToString() : "";

                model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";

                model.PROD_PACK_NAME = model.PROD_PACK_NAME_STRING.Split(',').ToList();
            }

            model.DropdownPackType = await LoadDllData(0, "", "PACKING_TYPE");
            model.DropdownPackName = await LoadDllData(0, "", "PROD_PACK_NAME");

            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MM_PRODPACK_DETAIL(string ActionType, ProdPackModel model)
        {
            ViewBag.Tittle = "Product Packing";
            try
            {
                DataTable dt = await dbdal.getProdPack_Data(model.MM_PRODPACK_ID.ToString()); //get header only
                if (dt != null && dt.Rows.Count > 0)
                {
                    model.MM_PRODPACK_ID = Convert.ToInt32(dt.Rows[0]["MM_PRODPACK_ID"]);
                    model.PACKING_TYPE_ID = Convert.ToInt32(dt.Rows[0]["PACKING_TYPE_ID"]);
                    model.PACKING_TYPE = dt.Rows[0]["PACKING_TYPE"] != DBNull.Value ? dt.Rows[0]["PACKING_TYPE"].ToString() : "";
                    model.PROD_PACK_NAME_STRING = dt.Rows[0]["PROD_PACK_NAME_STRING"] != DBNull.Value ? dt.Rows[0]["PROD_PACK_NAME_STRING"].ToString() : "";

                    model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                    model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                    model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                    model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                    model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                    model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                    model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";

                    model.PROD_PACK_NAME = model.PROD_PACK_NAME_STRING.Split(',').ToList();
                }

                model.DropdownPackType = await LoadDllData(0, "", "PACKING_TYPE");
                model.DropdownPackName = await LoadDllData(0, "", "PROD_PACK_NAME");

                if (ActionType == "postback")
                {
                    ModelState.Clear();
                }
                if (ActionType == "Back")
                {
                    ModelState.Clear();
                    return RedirectToAction("MM_PRODPACK_LST", "MM_PRODPACK");
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
        public async Task<ActionResult> InsertUpdateProdPack(ProdPackModel model)
        {
            bool success = true;
            string message = "";


            if (ModelState.IsValid)
            {
                if (model.PACKING_TYPE_ID == 0 || 
                    model.PROD_PACK_NAME_STRING == null || model.PROD_PACK_NAME_STRING == "")
                {
                    success = false;
                    message += "Required field cannot empty.";
                }

                if (success)
                {
                    // insert h
                    string result1 = await dbdal.ProdPack_H_Maint(model);
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
        public async Task<ActionResult> MM_PRODPACK_ADD(string id = "")
        {
            ViewBag.Tittle = "Product Packing";
            var model = new ProdPackModel();

            model.DropdownPackType = await LoadDllData(0, "", "PACKING_TYPE");
            model.DropdownPackName = await LoadDllData(0, "", "PROD_PACK_NAME");
            
            if (!string.IsNullOrWhiteSpace(id) && CommonMethod.isNumeric(id))
            {
                DataTable dt = await dbdal.getProdPack_Data(id); //get header data

                if (dt != null && dt.Rows.Count > 0)
                {
                    model.MM_PRODPACK_ID = Convert.ToInt32(dt.Rows[0]["MM_PRODPACK_ID"]);
                    model.PACKING_TYPE_ID = Convert.ToInt32(dt.Rows[0]["PACKING_TYPE_ID"]);
                    model.PACKING_TYPE = dt.Rows[0]["PACKING_TYPE"] != DBNull.Value ? dt.Rows[0]["PACKING_TYPE"].ToString() : "";
                    model.PROD_PACK_NAME_STRING = dt.Rows[0]["PROD_PACK_NAME_STRING"] != DBNull.Value ? dt.Rows[0]["PROD_PACK_NAME_STRING"].ToString() : "";

                    model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                    model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                    model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                    model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                    model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                    model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                    model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";

                    model.PROD_PACK_NAME = model.PROD_PACK_NAME_STRING.Split(',').ToList();
                }

            }
            else
            {
                model = new ProdPackModel();
                model.DropdownPackType = await LoadDllData(0, "", "PACKING_TYPE");
                model.DropdownPackName = await LoadDllData(0, "", "PROD_PACK_NAME");
            }

            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MM_PRODPACK_ADD(string ActionType, ProdPackModel model)
        {
            ViewBag.Tittle = "Product Packing";
            try
            {
                model.DropdownPackType = await LoadDllData(0, "", "PACKING_TYPE");
                model.DropdownPackName = await LoadDllData(0, "", "PROD_PACK_NAME");

            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> DraftProdPack(ProdPackModel model)
        {
            bool success = true;
            string message = "";

            if (ModelState.IsValid)
            {
                if (model.PACKING_TYPE_ID == 0)
                {
                    success = false;
                    message += "Required field cannot empty.";
                }

                if (success)
                {
                    model.PROD_PACK_NAME_STRING = model.PROD_PACK_NAME_STRING != null ? model.PROD_PACK_NAME_STRING.ToString() : "";

                    // insert h
                    string result1 = await dbdal.ProdPack_H_Maint(model);
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
        public async Task<ActionResult> deleteProdPack(List<string> lstid)
        {
            bool success = true;
            string message = "";

            ProdPackModel model = new ProdPackModel();

            if (lstid.Count < 1)
            {
                success = false;
                message += "Please select a record first to delete.";
            }

            if (success && lstid.Count > 0)
            {
                foreach (var item in lstid)
                {
                    model.MM_PRODPACK_ID = Convert.ToInt32(item);
                    model.RECORD_TYP = "5";

                    model.PACKING_TYPE_ID = 0;
                    model.PROD_PACK_NAME_STRING = "";

                    string resultd = await dbdal.ProdPack_H_Maint(model);
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
        public async Task<ActionResult> MM_PRODPACK_AUDIT(string id)
        {
            TempData["SQ_ID"] = id;
            CommonFunction common = new CommonFunction();
            var param = new { pTable = "PVIEW_MM_PRODPACK_A", pKeyValue = id };
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