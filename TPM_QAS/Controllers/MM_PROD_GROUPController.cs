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
    public class MM_PROD_GROUPController : Controller
    {
        DB dbmain = new DB();
        MM_PROD_GROUP_DAL dbdal = new MM_PROD_GROUP_DAL();

        #region LIST

        [SessionExpire]
        public async Task<ActionResult> MM_PROD_GROUP_LST(string Deleted = "0")
        {
            TempData["STATUS_DDL"] = await dbmain.PSP_COMMON_DDL("CMN_LST_STATUS", "", "", "", "", "", "");

            CommonFunction common = new CommonFunction();
            var param = new { Table = "PVIEW_MM_PRODGROUP", TableID = "", Search = "", Value = "", SortField = "MM_PRODGROUP_H_ID", Direction = "0", FrmRowno = "1", ToRowno = "500000", Deleted = Deleted };
            List<ProdGroupModel> model = await common.PSP_COMMON_DAPPER<ProdGroupModel>("PSP_COMMON_LIST", CommandType.StoredProcedure, param) ?? new List<ProdGroupModel>();

            ViewBag.Deleted = Deleted;
            return View(model);
        }

        #endregion

        #region VIEW/EDIT

        [SessionExpire]
        public async Task<ActionResult> MM_PROD_GROUP_DETAIL(string id)
        {
            ViewBag.Tittle = "Product Group";
            var model = new ProdGroupModel();

            DataTable dt = await dbdal.getProdGroup_Data(id, "H"); //get header data

            if (dt != null && dt.Rows.Count > 0)
            {
                model.MM_PRODGROUP_H_ID = Convert.ToInt32(dt.Rows[0]["MM_PRODGROUP_H_ID"]);
                model.PROD_GROUP = dt.Rows[0]["PROD_GROUP"] != DBNull.Value ? dt.Rows[0]["PROD_GROUP"].ToString() : "";

                model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";
            }

            DataTable dtl = await dbdal.getProdGroup_Data(id, "D"); //get d data
            List<ProdTypeModel> listItemsAdd = new List<ProdTypeModel>();
            if (dtl != null && dtl.Rows.Count > 0)
            {
                for (int i = 0; i < dtl.Rows.Count; i++)
                {
                    ProdTypeModel infoObjAdd = new ProdTypeModel();

                    infoObjAdd.MM_PRODGROUP_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_PRODGROUP_H_ID"]);
                    infoObjAdd.MM_PRODGROUP_D_ID = Convert.ToInt32(dtl.Rows[i]["MM_PRODGROUP_D_ID"]);
                    infoObjAdd.MM_PRODTYPE_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_PRODTYPE_H_ID"]);
                    infoObjAdd.PROD_TYPE = dtl.Rows[i]["PROD_TYPE"] != DBNull.Value ? dtl.Rows[i]["PROD_TYPE"].ToString() : "";
                    infoObjAdd.COMP_GROUP = dtl.Rows[i]["COMP_GROUP"] != DBNull.Value ? dtl.Rows[i]["COMP_GROUP"].ToString() : "";

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

            model.ProdTypeModel = listItemsAdd;

            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MM_PROD_GROUP_DETAIL(string ActionType, ProdGroupModel model)
        {
            ViewBag.Tittle = "Product Group";
            try
            {
                if (ActionType == "postback")
                {
                    ModelState.Clear();
                }
                if (ActionType == "Back")
                {
                    ModelState.Clear();
                    return RedirectToAction("MM_PROD_GROUP_LST", "MM_PROD_GROUP");
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
        public async Task<ActionResult> InsertUpdateProductGroup(ProdGroupModel model)
        {
            bool success = true;
            string message = "";


            if (ModelState.IsValid)
            {
                if (model.PROD_GROUP == null || model.PROD_GROUP == "" ||
                    model.ProdTypeModel == null || model.ProdTypeModel.Count < 1)
                {
                    success = false;
                    message += "Required field cannot empty.";
                }

                if (success)
                {
                    // insert h
                    string result1 = await dbdal.ProdGroup_H_Maint(model);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                    else
                    {
                        // insert d
                        foreach (var item in model.ProdTypeModel)
                        {
                            int prodtypeid = item.MM_PRODTYPE_H_ID;
                            string prodtype = item.PROD_TYPE != null ? item.PROD_TYPE.ToString() : "";
                            string rectype = item.RECORD_TYP != null ? item.RECORD_TYP.ToString() : "";

                            if (rectype == "0")
                            {
                                rectype = "3";
                            }

                            string result2 = await dbdal.ProdGroup_D_Maint(item.MM_PRODGROUP_D_ID, Convert.ToInt32(result1), prodtypeid, rectype);
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
        public async Task<ActionResult> MM_PROD_GROUP_ADD(string id = "")
        {
            ViewBag.Tittle = "Product Group";
            var model = new ProdGroupModel();
            List<ProdTypeModel> listItemsAdd = new List<ProdTypeModel>();

            if (!string.IsNullOrWhiteSpace(id) && CommonMethod.isNumeric(id))
            {
                DataTable dt = await dbdal.getProdGroup_Data(id, "H"); //get header data

                if (dt != null && dt.Rows.Count > 0)
                {
                    model.MM_PRODGROUP_H_ID = Convert.ToInt32(dt.Rows[0]["MM_PRODGROUP_H_ID"]);
                    model.PROD_GROUP = dt.Rows[0]["PROD_GROUP"] != DBNull.Value ? dt.Rows[0]["PROD_GROUP"].ToString() : "";

                    model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                    model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                    model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                    model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                    model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                    model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                    model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";

                    DataTable dtl = await dbdal.getProdGroup_Data(id, "D"); //get d data
                    if (dtl != null && dtl.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtl.Rows.Count; i++)
                        {
                            ProdTypeModel infoObjAdd = new ProdTypeModel();

                            infoObjAdd.MM_PRODGROUP_D_ID = Convert.ToInt32(dtl.Rows[i]["MM_PRODGROUP_D_ID"]);
                            infoObjAdd.MM_PRODGROUP_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_PRODGROUP_H_ID"]);
                            infoObjAdd.MM_PRODTYPE_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_PRODTYPE_H_ID"]);
                            infoObjAdd.PROD_TYPE = dtl.Rows[i]["PROD_TYPE"] != DBNull.Value ? dtl.Rows[i]["PROD_TYPE"].ToString() : "";
                            infoObjAdd.PROD_TYPE = dtl.Rows[i]["COMP_GROUP"] != DBNull.Value ? dtl.Rows[i]["COMP_GROUP"].ToString() : "";

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
                model = new ProdGroupModel();
            }

            model.ProdTypeModel = listItemsAdd;
            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MM_PROD_GROUP_ADD(string ActionType, ProdGroupModel model)
        {
            ViewBag.Tittle = "Product Group";
            try
            {
                if (ActionType == "postback")
                {
                    ModelState.Clear();
                }
                if (ActionType == "Back")
                {
                    ModelState.Clear();
                    return RedirectToAction("MM_PROD_GROUP_LST", "MM_PROD_GROUP");
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
        public async Task<ActionResult> DraftProductGroup(ProdGroupModel model)
        {
            bool success = true;
            string message = "";


            if (ModelState.IsValid)
            {
                if (model.PROD_GROUP == null || model.PROD_GROUP == "")
                {
                    success = false;
                    message += "Required field cannot empty.";
                }

                if (success)
                {
                    // insert h
                    string result1 = await dbdal.ProdGroup_H_Maint(model);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                    else
                    {
                        if (model.ProdTypeModel != null && model.ProdTypeModel.Count > 0)
                        {
                            // insert d
                            foreach (var item in model.ProdTypeModel)
                            {
                                int prodtypeid = item.MM_PRODTYPE_H_ID;
                                string prodtype = item.PROD_TYPE != null ? item.PROD_TYPE.ToString() : "";
                                string rectype = item.RECORD_TYP != null ? item.RECORD_TYP.ToString() : "";

                                string result2 = await dbdal.ProdGroup_D_Maint(item.MM_PRODGROUP_D_ID, Convert.ToInt32(result1), prodtypeid, rectype);
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
        public async Task<ActionResult> MM_PROD_GROUP_PRODTYPE_POPOUT(string id)
        {
            if (TempData["Error"] != null) { ViewBag.Error = TempData["Error"].ToString(); }
            if (TempData["Result"] != null) { ViewBag.Result = TempData["Result"].ToString(); }

            var model = new ProdGroupModel();

            if (id == null || id == "")
            {
                id = "0";
            }

            DataTable dt = await dbdal.getProdTypePOPUP(Convert.ToInt32(id));

            List<ProdTypeModel> listItemsAdd = new List<ProdTypeModel>();
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ProdTypeModel infoObjAdd = new ProdTypeModel();

                    infoObjAdd.MM_PRODTYPE_H_ID = Convert.ToInt32(dt.Rows[i]["MM_PRODTYPE_H_ID"]);
                    infoObjAdd.PROD_TYPE = dt.Rows[i]["PROD_TYPE"] != DBNull.Value ? dt.Rows[i]["PROD_TYPE"].ToString() : "";
                    infoObjAdd.COMP_GROUP = dt.Rows[i]["COMP_GROUP"] != DBNull.Value ? dt.Rows[i]["COMP_GROUP"].ToString() : "";

                    infoObjAdd.IS_EXIST = "N";

                    // Adding.
                    listItemsAdd.Add(infoObjAdd);

                }

            }

            model.ProdTypeModel = listItemsAdd;

            return View(model);
        }

        #endregion

        #region DELETE

        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> deleteProdGroup(List<string> lstid)
        {
            bool success = true;
            string message = "";

            ProdGroupModel modelh = new ProdGroupModel();

            if (lstid.Count < 1)
            {
                success = false;
                message = "Please select a record first to delete.";
            }

            if (success && lstid.Count > 0)
            {
                foreach (var item in lstid)
                {
                    modelh.MM_PRODGROUP_H_ID = Convert.ToInt32(item);
                    modelh.RECORD_TYP = "5";

                    modelh.PROD_GROUP = "";

                    //delete h
                    string result1 = await dbdal.ProdGroup_H_Maint(modelh);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                    else
                    {
                        //delete d
                        string result2 = await dbdal.ProdGroup_D_Maint(0, Convert.ToInt32(item), 0, "2");
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
        public async Task<ActionResult> MM_PROD_GROUP_AUDIT(string id)
        {
            TempData["SQ_ID"] = id;
            CommonFunction common = new CommonFunction();
            var param = new { pTable = "PVIEW_MM_PRODGROUP_A", pKeyValue = id };
            List<AuditTrailModel> AuditTrailModel = await common.PSP_COMMON_DAPPER<AuditTrailModel>("PSP_COMMON_AUDIT", CommandType.StoredProcedure, param) ?? new List<AuditTrailModel>();

            return View(AuditTrailModel);
        }

        #endregion

    }
}