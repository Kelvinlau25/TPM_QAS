using DBModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TPM_QAS.Filters;
using TPM_QAS.Helpers;
using TPM_QAS.DAL;
using TPM_QAS.Models;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TPM_QAS.Controllers
{
    public class MM_PROD_TYPEController : Controller
    {
        DB dbmain = new DB();
        MM_PROD_TYPE_DAL dbdal = new MM_PROD_TYPE_DAL();

        #region LIST

        [SessionExpire]
        public async Task<ActionResult> MM_PROD_TYPE_LST(string Deleted = "0")
        {
            TempData["STATUS_DDL"] = await dbmain.PSP_COMMON_DDL("CMN_LST_STATUS", "", "", "", "", "", "");

            CommonFunction common = new CommonFunction();
            var param = new { Table = "PVIEW_MM_PRODTYPE", TableID = "", Search = "", Value = "", SortField = "MM_PRODTYPE_H_ID", Direction = "0", FrmRowno = "1", ToRowno = "500000", Deleted = Deleted };
            List<ProdTypeModel> model = await common.PSP_COMMON_DAPPER<ProdTypeModel>("PSP_COMMON_LIST", CommandType.StoredProcedure, param) ?? new List<ProdTypeModel>();

            ViewBag.Deleted = Deleted;
            return View(model);
        }

        #endregion

        #region VIEW/EDIT

        [SessionExpire]
        public async Task<ActionResult> MM_PROD_TYPE_DETAIL(string id)
        {
            ViewBag.Tittle = "Product Type";
            var model = new ProdTypeModel();

            DataTable dt = await dbdal.getProdType_Data(id, "H"); //get header data

            if (dt != null && dt.Rows.Count > 0)
            {
                model.MM_PRODTYPE_H_ID = Convert.ToInt32(dt.Rows[0]["MM_PRODTYPE_H_ID"]);
                model.PROD_TYPE = dt.Rows[0]["PROD_TYPE"] != DBNull.Value ? dt.Rows[0]["PROD_TYPE"].ToString() : "";
                model.COMP_GROUP = dt.Rows[0]["COMP_GROUP"] != DBNull.Value ? dt.Rows[0]["COMP_GROUP"].ToString() : "";

                model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";

            }

            DataTable dtproperty = await dbdal.getProdType_Data(id, "D"); //get d data
            List<ProdTypeMainPropLstModel> listItemsAdd = new List<ProdTypeMainPropLstModel>();
            if (dtproperty != null && dtproperty.Rows.Count > 0)
            {
                for (int i = 0; i < dtproperty.Rows.Count; i++)
                {
                    ProdTypeMainPropLstModel infoObjAdd = new ProdTypeMainPropLstModel();

                    infoObjAdd.MM_PRODTYPE_D_ID = Convert.ToInt32(dtproperty.Rows[i]["MM_PRODTYPE_D_ID"]);
                    infoObjAdd.MM_PRODTYPE_H_ID = Convert.ToInt32(dtproperty.Rows[i]["MM_PRODTYPE_H_ID"]);
                    infoObjAdd.MM_PROPERTIES_H_ID = Convert.ToInt32(dtproperty.Rows[i]["MM_PROPERTIES_H_ID"]);
                    infoObjAdd.PROPERTIES = dtproperty.Rows[i]["PROPERTIES"] != DBNull.Value ? dtproperty.Rows[i]["PROPERTIES"].ToString() : "";
                    infoObjAdd.PROP_ITEM = dtproperty.Rows[i]["PROP_ITEM"] != DBNull.Value ? dtproperty.Rows[i]["PROP_ITEM"].ToString() : "";

                    infoObjAdd.RECORD_TYP = dtproperty.Rows[i]["RECORD_TYP"] != DBNull.Value ? dtproperty.Rows[i]["RECORD_TYP"].ToString() : "";
                    infoObjAdd.CREATED_BY = dtproperty.Rows[i]["CREATED_BY"] != DBNull.Value ? dtproperty.Rows[i]["CREATED_BY"].ToString() : "";
                    infoObjAdd.CREATED_DATE = dtproperty.Rows[i]["CREATED_DATE"] != DBNull.Value ? dtproperty.Rows[i]["CREATED_DATE"].ToString() : "";
                    infoObjAdd.CREATED_LOC = dtproperty.Rows[i]["CREATED_LOC"] != DBNull.Value ? dtproperty.Rows[i]["CREATED_LOC"].ToString() : "";
                    infoObjAdd.UPDATED_BY = dtproperty.Rows[i]["UPDATED_BY"] != DBNull.Value ? dtproperty.Rows[i]["UPDATED_BY"].ToString() : "";
                    infoObjAdd.UPDATED_DATE = dtproperty.Rows[i]["UPDATED_DATE"] != DBNull.Value ? dtproperty.Rows[i]["UPDATED_DATE"].ToString() : "";
                    infoObjAdd.UPDATED_LOC = dtproperty.Rows[i]["UPDATED_LOC"] != DBNull.Value ? dtproperty.Rows[i]["UPDATED_LOC"].ToString() : "";

                    // Adding.
                    listItemsAdd.Add(infoObjAdd);

                }

            }

            DataTable dtproditem = await dbdal.getProdType_Data(id, "D1"); //get d data
            List<ProdTypeProdItemLstModel> listItemsAdd2 = new List<ProdTypeProdItemLstModel>();
            if (dtproditem != null && dtproditem.Rows.Count > 0)
            {
                for (int i = 0; i < dtproditem.Rows.Count; i++)
                {
                    ProdTypeProdItemLstModel infoObjAdd = new ProdTypeProdItemLstModel();

                    infoObjAdd.MM_PRODTYPE_D1_ID = Convert.ToInt32(dtproditem.Rows[i]["MM_PRODTYPE_D1_ID"]);
                    infoObjAdd.MM_PRODTYPE_H_ID = Convert.ToInt32(dtproditem.Rows[i]["MM_PRODTYPE_H_ID"]);
                    infoObjAdd.PROP_TYPE_MAIN = dtproditem.Rows[i]["PROP_TYPE_MAIN"] != DBNull.Value ? dtproditem.Rows[i]["PROP_TYPE_MAIN"].ToString() : "";
                    infoObjAdd.PROP_TYPE_SUB = dtproditem.Rows[i]["PROP_TYPE_SUB"] != DBNull.Value ? dtproditem.Rows[i]["PROP_TYPE_SUB"].ToString() : "";
                    infoObjAdd.PROP_CLASS = dtproditem.Rows[i]["PROP_CLASS"] != DBNull.Value ? dtproditem.Rows[i]["PROP_CLASS"].ToString() : "";
                    infoObjAdd.PROP_COLOUR = dtproditem.Rows[i]["PROP_COLOUR"] != DBNull.Value ? dtproditem.Rows[i]["PROP_COLOUR"].ToString() : "";
                    infoObjAdd.PROP_PACK = dtproditem.Rows[i]["PROP_PACK"] != DBNull.Value ? dtproditem.Rows[i]["PROP_PACK"].ToString() : "";

                    infoObjAdd.RECORD_TYP = dtproditem.Rows[i]["RECORD_TYP"] != DBNull.Value ? dtproditem.Rows[i]["RECORD_TYP"].ToString() : "";
                    infoObjAdd.CREATED_BY = dtproditem.Rows[i]["CREATED_BY"] != DBNull.Value ? dtproditem.Rows[i]["CREATED_BY"].ToString() : "";
                    infoObjAdd.CREATED_DATE = dtproditem.Rows[i]["CREATED_DATE"] != DBNull.Value ? dtproditem.Rows[i]["CREATED_DATE"].ToString() : "";
                    infoObjAdd.CREATED_LOC = dtproditem.Rows[i]["CREATED_LOC"] != DBNull.Value ? dtproditem.Rows[i]["CREATED_LOC"].ToString() : "";
                    infoObjAdd.UPDATED_BY = dtproditem.Rows[i]["UPDATED_BY"] != DBNull.Value ? dtproditem.Rows[i]["UPDATED_BY"].ToString() : "";
                    infoObjAdd.UPDATED_DATE = dtproditem.Rows[i]["UPDATED_DATE"] != DBNull.Value ? dtproditem.Rows[i]["UPDATED_DATE"].ToString() : "";
                    infoObjAdd.UPDATED_LOC = dtproditem.Rows[i]["UPDATED_LOC"] != DBNull.Value ? dtproditem.Rows[i]["UPDATED_LOC"].ToString() : "";

                    // Adding.
                    listItemsAdd2.Add(infoObjAdd);
                }
            }

            model.ProdTypeMainPropLstModel = listItemsAdd;
            model.ProdTypeProdItemLstModel = listItemsAdd2;

            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MM_PROD_TYPE_DETAIL(string ActionType, ProdTypeModel model)
        {
            ViewBag.Tittle = "Product Type";
            try
            {
                if (ActionType == "postback")
                {
                    ModelState.Clear();
                }
                if (ActionType == "Back")
                {
                    ModelState.Clear();
                    return RedirectToAction("MM_PROD_TYPE_LST", "MM_PROD_TYPE");
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
        public async Task<ActionResult> InsertUpdateProdType(ProdTypeModel model)
        {
            bool success = true;
            string message = "";


            if (ModelState.IsValid)
            {
                if (model.PROD_TYPE == null || model.PROD_TYPE == "" ||
                    model.COMP_GROUP == null || model.COMP_GROUP == "" ||
                    model.ProdTypeMainPropLstModel == null || model.ProdTypeMainPropLstModel.Count < 1 ||
                    model.ProdTypeProdItemLstModel == null || model.ProdTypeProdItemLstModel.Count < 1)
                {
                    success = false;
                    message += "Required field cannot empty.";
                }

                if (success)
                {
                    // insert h
                    string result1 = await dbdal.ProdType_H_Maint(model);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                    else
                    {
                        //delete existing d first
                        string result9 = await dbdal.ProdType_D_Maint(0, Convert.ToInt32(result1), 0, "2");
                        if (!(int.TryParse(result9, out int num9)))
                        {
                            success = false;
                            message += result9;
                        }
                        else
                        {
                            if (model.ProdTypeMainPropLstModel != null && model.ProdTypeMainPropLstModel.Count > 0)
                            {
                                // insert d
                                foreach (var item in model.ProdTypeMainPropLstModel)
                                {
                                    int propid = Convert.ToInt32(item.MM_PROPERTIES_H_ID);
                                    string properties = item.PROPERTIES != null ? item.PROPERTIES.ToString() : "";
                                    string propitem = item.PROP_ITEM != null ? item.PROP_ITEM.ToString() : "";
                                    string rectype = item.RECORD_TYP != null ? item.RECORD_TYP.ToString() : "";

                                    if (rectype == "0")
                                    {
                                        rectype = "3";
                                    }

                                    string result2 = await dbdal.ProdType_D_Maint(item.MM_PRODTYPE_D_ID, Convert.ToInt32(result1), propid, rectype);
                                    if (!(int.TryParse(result2, out int num2) && result2 != "0"))
                                    {
                                        success = false;
                                        message += result2;
                                    }

                                }
                            }
                        }

                        //delete existing d1 first
                        string result8 = await dbdal.ProdType_D1_Maint(0, Convert.ToInt32(result1), "", "", "", "", "", "2");
                        if (!(int.TryParse(result8, out int num8)))
                        {
                            success = false;
                            message += result8;
                        }
                        else
                        {
                            if (model.ProdTypeProdItemLstModel != null && model.ProdTypeProdItemLstModel.Count > 0)
                            {
                                // insert d1
                                foreach (var item in model.ProdTypeProdItemLstModel)
                                {
                                    string propmain = item.PROP_TYPE_MAIN != null ? item.PROP_TYPE_MAIN.ToString() : "";
                                    string propsub = item.PROP_TYPE_SUB != null ? item.PROP_TYPE_SUB.ToString() : "";
                                    string propclass = item.PROP_CLASS != null ? item.PROP_CLASS.ToString() : "";
                                    string propcolour = item.PROP_COLOUR != null ? item.PROP_COLOUR.ToString() : "";
                                    string proppack = item.PROP_PACK != null ? item.PROP_PACK.ToString() : "";
                                    string rectype = item.RECORD_TYP != null ? item.RECORD_TYP.ToString() : "";

                                    if (rectype == "0")
                                    {
                                        rectype = "3";
                                    }

                                    string result2 = await dbdal.ProdType_D1_Maint(item.MM_PRODTYPE_D1_ID, Convert.ToInt32(result1), propmain, propsub, propclass, propcolour, proppack, rectype);
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
        public async Task<ActionResult> MM_PROD_TYPE_ADD(string id = "")
        {
            ViewBag.Tittle = "Product Type";
            var model = new ProdTypeModel();
            List<ProdTypeMainPropLstModel> listItemsAdd = new List<ProdTypeMainPropLstModel>();
            List<ProdTypeProdItemLstModel> listItemsAdd2 = new List<ProdTypeProdItemLstModel>();

            if (!string.IsNullOrWhiteSpace(id) && CommonMethod.isNumeric(id))
            {
                DataTable dt = await dbdal.getProdType_Data(id, "H"); //get header data

                if (dt != null && dt.Rows.Count > 0)
                {
                    model.MM_PRODTYPE_H_ID = Convert.ToInt32(dt.Rows[0]["MM_PRODTYPE_H_ID"]);
                    model.PROD_TYPE = dt.Rows[0]["PROD_TYPE"] != DBNull.Value ? dt.Rows[0]["PROD_TYPE"].ToString() : "";
                    model.COMP_GROUP = dt.Rows[0]["COMP_GROUP"] != DBNull.Value ? dt.Rows[0]["COMP_GROUP"].ToString() : "";

                    model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                    model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                    model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                    model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                    model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                    model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                    model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";

                    DataTable dtproperty = await dbdal.getProdType_Data(id, "D"); //get d data
                    if (dtproperty != null && dtproperty.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtproperty.Rows.Count; i++)
                        {
                            ProdTypeMainPropLstModel infoObjAdd = new ProdTypeMainPropLstModel();

                            infoObjAdd.MM_PRODTYPE_D_ID = Convert.ToInt32(dtproperty.Rows[i]["MM_PRODTYPE_D_ID"]);
                            infoObjAdd.MM_PRODTYPE_H_ID = Convert.ToInt32(dtproperty.Rows[i]["MM_PRODTYPE_H_ID"]);
                            infoObjAdd.MM_PROPERTIES_H_ID = Convert.ToInt32(dtproperty.Rows[i]["MM_PROPERTIES_H_ID"]);
                            infoObjAdd.PROPERTIES = dtproperty.Rows[i]["PROPERTIES"] != DBNull.Value ? dtproperty.Rows[i]["PROPERTIES"].ToString() : "";
                            infoObjAdd.PROP_ITEM = dtproperty.Rows[i]["PROP_ITEM"] != DBNull.Value ? dtproperty.Rows[i]["PROP_ITEM"].ToString() : "";

                            infoObjAdd.RECORD_TYP = dtproperty.Rows[i]["RECORD_TYP"] != DBNull.Value ? dtproperty.Rows[i]["RECORD_TYP"].ToString() : "";
                            infoObjAdd.CREATED_BY = dtproperty.Rows[i]["CREATED_BY"] != DBNull.Value ? dtproperty.Rows[i]["CREATED_BY"].ToString() : "";
                            infoObjAdd.CREATED_DATE = dtproperty.Rows[i]["CREATED_DATE"] != DBNull.Value ? dtproperty.Rows[i]["CREATED_DATE"].ToString() : "";
                            infoObjAdd.CREATED_LOC = dtproperty.Rows[i]["CREATED_LOC"] != DBNull.Value ? dtproperty.Rows[i]["CREATED_LOC"].ToString() : "";
                            infoObjAdd.UPDATED_BY = dtproperty.Rows[i]["UPDATED_BY"] != DBNull.Value ? dtproperty.Rows[i]["UPDATED_BY"].ToString() : "";
                            infoObjAdd.UPDATED_DATE = dtproperty.Rows[i]["UPDATED_DATE"] != DBNull.Value ? dtproperty.Rows[i]["UPDATED_DATE"].ToString() : "";
                            infoObjAdd.UPDATED_LOC = dtproperty.Rows[i]["UPDATED_LOC"] != DBNull.Value ? dtproperty.Rows[i]["UPDATED_LOC"].ToString() : "";

                            // Adding.
                            listItemsAdd.Add(infoObjAdd);

                        }
                    }

                    DataTable dtproditem = await dbdal.getProdType_Data(id, "D1"); //get d data
                    if (dtproditem != null && dtproditem.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtproditem.Rows.Count; i++)
                        {
                            ProdTypeProdItemLstModel infoObjAdd = new ProdTypeProdItemLstModel();

                            infoObjAdd.MM_PRODTYPE_D1_ID = Convert.ToInt32(dtproditem.Rows[i]["MM_PRODTYPE_D1_ID"]);
                            infoObjAdd.MM_PRODTYPE_H_ID = Convert.ToInt32(dtproditem.Rows[i]["MM_PRODTYPE_H_ID"]);
                            infoObjAdd.PROP_TYPE_MAIN = dtproditem.Rows[i]["PROP_TYPE_MAIN"] != DBNull.Value ? dtproditem.Rows[i]["PROP_TYPE_MAIN"].ToString() : "";
                            infoObjAdd.PROP_TYPE_SUB = dtproditem.Rows[i]["PROP_TYPE_SUB"] != DBNull.Value ? dtproditem.Rows[i]["PROP_TYPE_SUB"].ToString() : "";
                            infoObjAdd.PROP_CLASS = dtproditem.Rows[i]["PROP_CLASS"] != DBNull.Value ? dtproditem.Rows[i]["PROP_CLASS"].ToString() : "";
                            infoObjAdd.PROP_COLOUR = dtproditem.Rows[i]["PROP_COLOUR"] != DBNull.Value ? dtproditem.Rows[i]["PROP_COLOUR"].ToString() : "";
                            infoObjAdd.PROP_PACK = dtproditem.Rows[i]["PROP_PACK"] != DBNull.Value ? dtproditem.Rows[i]["PROP_PACK"].ToString() : "";

                            infoObjAdd.RECORD_TYP = dtproditem.Rows[i]["RECORD_TYP"] != DBNull.Value ? dtproditem.Rows[i]["RECORD_TYP"].ToString() : "";
                            infoObjAdd.CREATED_BY = dtproditem.Rows[i]["CREATED_BY"] != DBNull.Value ? dtproditem.Rows[i]["CREATED_BY"].ToString() : "";
                            infoObjAdd.CREATED_DATE = dtproditem.Rows[i]["CREATED_DATE"] != DBNull.Value ? dtproditem.Rows[i]["CREATED_DATE"].ToString() : "";
                            infoObjAdd.CREATED_LOC = dtproditem.Rows[i]["CREATED_LOC"] != DBNull.Value ? dtproditem.Rows[i]["CREATED_LOC"].ToString() : "";
                            infoObjAdd.UPDATED_BY = dtproditem.Rows[i]["UPDATED_BY"] != DBNull.Value ? dtproditem.Rows[i]["UPDATED_BY"].ToString() : "";
                            infoObjAdd.UPDATED_DATE = dtproditem.Rows[i]["UPDATED_DATE"] != DBNull.Value ? dtproditem.Rows[i]["UPDATED_DATE"].ToString() : "";
                            infoObjAdd.UPDATED_LOC = dtproditem.Rows[i]["UPDATED_LOC"] != DBNull.Value ? dtproditem.Rows[i]["UPDATED_LOC"].ToString() : "";

                            // Adding.
                            listItemsAdd2.Add(infoObjAdd);

                        }
                    }
                }
            }
            else
            {
                model = new ProdTypeModel();
            }

            model.ProdTypeMainPropLstModel = listItemsAdd;
            model.ProdTypeProdItemLstModel = listItemsAdd2;
            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MM_PROD_TYPE_ADD(string ActionType, ProdTypeModel model)
        {
            ViewBag.Tittle = "Product Type";
            try
            {
                if (ActionType == "postback")
                {
                    ModelState.Clear();
                }
                if (ActionType == "Back")
                {
                    ModelState.Clear();
                    return RedirectToAction("MM_PROD_TYPE_LST", "MM_PROD_TYPE");
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
        public async Task<ActionResult> DraftProdType(ProdTypeModel model)
        {
            bool success = true;
            string message = "";


            if (ModelState.IsValid)
            {
                if (model.PROD_TYPE == null || model.PROD_TYPE == "")
                {
                    success = false;
                    message += "Required field cannot empty. ";
                }

                if (success)
                {
                    // insert h
                    string result1 = await dbdal.ProdType_H_Maint(model);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                    else
                    {
                        if (model.ProdTypeMainPropLstModel != null && model.ProdTypeMainPropLstModel.Count > 0)
                        {
                            // insert d
                            foreach (var item in model.ProdTypeMainPropLstModel)
                            {
                                int propid = Convert.ToInt32(item.MM_PROPERTIES_H_ID);
                                string properties = item.PROPERTIES != null ? item.PROPERTIES.ToString() : "";
                                string propitem = item.PROP_ITEM != null ? item.PROP_ITEM.ToString() : "";
                                string rectype = item.RECORD_TYP != null ? item.RECORD_TYP.ToString() : "";

                                string result2 = await dbdal.ProdType_D_Maint(item.MM_PRODTYPE_D_ID, Convert.ToInt32(result1), propid, rectype);
                                if (!(int.TryParse(result2, out int num2) && result2 != "0"))
                                {
                                    success = false;
                                    message += result2;
                                }

                            }
                        }

                        if (model.ProdTypeProdItemLstModel != null && model.ProdTypeProdItemLstModel.Count > 0)
                        {
                            // insert d1
                            foreach (var item in model.ProdTypeProdItemLstModel)
                            {
                                string propmain = item.PROP_TYPE_MAIN != null ? item.PROP_TYPE_MAIN.ToString() : "";
                                string propsub = item.PROP_TYPE_SUB != null ? item.PROP_TYPE_SUB.ToString() : "";
                                string propclass = item.PROP_CLASS != null ? item.PROP_CLASS.ToString() : "";
                                string propcolour = item.PROP_COLOUR != null ? item.PROP_COLOUR.ToString() : "";
                                string proppack = item.PROP_PACK != null ? item.PROP_PACK.ToString() : "";
                                string rectype = item.RECORD_TYP != null ? item.RECORD_TYP.ToString() : "";

                                string result2 = await dbdal.ProdType_D1_Maint(item.MM_PRODTYPE_D1_ID, Convert.ToInt32(result1), propmain, propsub, propclass, propcolour, proppack, rectype);
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
        public async Task<ActionResult> MM_PROD_TYPE_PROPERTY_POPOUT(string id)
        {
            if (TempData["Error"] != null) { ViewBag.Error = TempData["Error"].ToString(); }
            if (TempData["Result"] != null) { ViewBag.Result = TempData["Result"].ToString(); }

            var model = new ProdTypeModel();

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

        [SessionExpire]
        public async Task<ActionResult> MM_PROD_TYPE_PROD_ITEM_POPOUT(string id, string search)
        {
            if (TempData["Error"] != null) { ViewBag.Error = TempData["Error"].ToString(); }
            if (TempData["Result"] != null) { ViewBag.Result = TempData["Result"].ToString(); }

            var model = new ProdTypeModel();

            if (id == null || id == "")
            {
                id = "0";
            }

            DataTable dt = await dbdal.getProdPOPUP(Convert.ToInt32(id), search);

            List<ProdTypePropTypeLstModel> listItemsAdd = new List<ProdTypePropTypeLstModel>();
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ProdTypePropTypeLstModel infoObjAdd = new ProdTypePropTypeLstModel();

                    infoObjAdd.PROPTYPEMAIN = dt.Rows[i]["PROPTYPEMAIN"] != DBNull.Value ? dt.Rows[i]["PROPTYPEMAIN"].ToString() : "";
                    infoObjAdd.PROPTYPESUB = dt.Rows[i]["PROPTYPESUB"] != DBNull.Value ? dt.Rows[i]["PROPTYPESUB"].ToString() : "";
                    infoObjAdd.PROPCLS = dt.Rows[i]["PROPCLS"] != DBNull.Value ? dt.Rows[i]["PROPCLS"].ToString() : "";
                    infoObjAdd.PROPCOLOR = dt.Rows[i]["PROPCOLOR"] != DBNull.Value ? dt.Rows[i]["PROPCOLOR"].ToString() : "";
                    infoObjAdd.PROPPACK = dt.Rows[i]["PROPPACK"] != DBNull.Value ? dt.Rows[i]["PROPPACK"].ToString() : "";
                    infoObjAdd.RECORD_TYP = dt.Rows[i]["RECORD_TYP"] != DBNull.Value ? dt.Rows[i]["RECORD_TYP"].ToString() : "";

                    infoObjAdd.IS_EXIST = "N";

                    // Adding.
                    listItemsAdd.Add(infoObjAdd);

                }

            }

            model.ProdTypePropTypeLstModel = listItemsAdd;

            return View(model);
        }

        [SessionExpire]
        public async Task<ActionResult> MM_PROD_TYPE_DUPLICATE_POPOUT()
        {
            if (TempData["Error"] != null) { ViewBag.Error = TempData["Error"].ToString(); }
            if (TempData["Result"] != null) { ViewBag.Result = TempData["Result"].ToString(); }

            var model = new ProdTypeModel();

            CommonFunction common = new CommonFunction();
            var param = new { Table = "PVIEW_MM_PRODTYPE", TableID = "", Search = "", Value = "", SortField = "MM_PRODTYPE_H_ID", Direction = "0", FrmRowno = "1", ToRowno = "500000", Deleted = 0 };
            List<ProdTypeDuplLstModel> ProdTypeDuplLstModel = await common.PSP_COMMON_DAPPER<ProdTypeDuplLstModel>("PSP_COMMON_LIST", CommandType.StoredProcedure, param) ?? new List<ProdTypeDuplLstModel>();

            model.ProdTypeDuplLstModel = ProdTypeDuplLstModel;

            return View(model);
        }

        public async Task<ActionResult> getDuplicateProdType(string id)
        {
            bool success = true;
            string message = "";
            var model = new ProdTypeModel();

            List<ProdTypeMainPropLstModel> listItemsAdd1 = new List<ProdTypeMainPropLstModel>();
            List<ProdTypeProdItemLstModel> listItemsAdd2 = new List<ProdTypeProdItemLstModel>();

            DataTable dt = await dbdal.getProdType_Data(id, "H"); //get header data

            if (dt != null && dt.Rows.Count > 0)
            {
                model.MM_PRODTYPE_H_ID = Convert.ToInt32(dt.Rows[0]["MM_PRODTYPE_H_ID"]);
                model.PROD_TYPE = dt.Rows[0]["PROD_TYPE"] != DBNull.Value ? dt.Rows[0]["PROD_TYPE"].ToString() : "";
                model.COMP_GROUP = dt.Rows[0]["COMP_GROUP"] != DBNull.Value ? dt.Rows[0]["COMP_GROUP"].ToString() : "";

                model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";

                //D1
                DataTable dtproperty = await dbdal.getProdType_Data(id, "D"); //get d data
                if (dtproperty != null && dtproperty.Rows.Count > 0)
                {
                    for (int i = 0; i < dtproperty.Rows.Count; i++)
                    {
                        if(dtproperty.Rows[i]["PROPERTIES"].ToString() != "")  // exclude deleted properties
                        {
                            ProdTypeMainPropLstModel infoObjAdd = new ProdTypeMainPropLstModel();

                            infoObjAdd.MM_PRODTYPE_D_ID = Convert.ToInt32(dtproperty.Rows[i]["MM_PRODTYPE_D_ID"]);
                            infoObjAdd.MM_PRODTYPE_H_ID = Convert.ToInt32(dtproperty.Rows[i]["MM_PRODTYPE_H_ID"]);
                            infoObjAdd.MM_PROPERTIES_H_ID = Convert.ToInt32(dtproperty.Rows[i]["MM_PROPERTIES_H_ID"]);
                            infoObjAdd.PROPERTIES = dtproperty.Rows[i]["PROPERTIES"] != DBNull.Value ? dtproperty.Rows[i]["PROPERTIES"].ToString() : "";
                            infoObjAdd.PROP_ITEM = dtproperty.Rows[i]["PROP_ITEM"] != DBNull.Value ? dtproperty.Rows[i]["PROP_ITEM"].ToString() : "";

                            infoObjAdd.RECORD_TYP = dtproperty.Rows[i]["RECORD_TYP"] != DBNull.Value ? dtproperty.Rows[i]["RECORD_TYP"].ToString() : "";
                            infoObjAdd.CREATED_BY = dtproperty.Rows[i]["CREATED_BY"] != DBNull.Value ? dtproperty.Rows[i]["CREATED_BY"].ToString() : "";
                            infoObjAdd.CREATED_DATE = dtproperty.Rows[i]["CREATED_DATE"] != DBNull.Value ? dtproperty.Rows[i]["CREATED_DATE"].ToString() : "";
                            infoObjAdd.CREATED_LOC = dtproperty.Rows[i]["CREATED_LOC"] != DBNull.Value ? dtproperty.Rows[i]["CREATED_LOC"].ToString() : "";
                            infoObjAdd.UPDATED_BY = dtproperty.Rows[i]["UPDATED_BY"] != DBNull.Value ? dtproperty.Rows[i]["UPDATED_BY"].ToString() : "";
                            infoObjAdd.UPDATED_DATE = dtproperty.Rows[i]["UPDATED_DATE"] != DBNull.Value ? dtproperty.Rows[i]["UPDATED_DATE"].ToString() : "";
                            infoObjAdd.UPDATED_LOC = dtproperty.Rows[i]["UPDATED_LOC"] != DBNull.Value ? dtproperty.Rows[i]["UPDATED_LOC"].ToString() : "";

                            // Adding.
                            listItemsAdd1.Add(infoObjAdd);
                        }
                    }
                }

                //D2
                DataTable dtproditem = await dbdal.getProdType_Data(id, "D1"); //get d data
                if (dtproditem != null && dtproditem.Rows.Count > 0)
                {
                    for (int i = 0; i < dtproditem.Rows.Count; i++)
                    {
                        ProdTypeProdItemLstModel infoObjAdd = new ProdTypeProdItemLstModel();

                        infoObjAdd.MM_PRODTYPE_D1_ID = Convert.ToInt32(dtproditem.Rows[i]["MM_PRODTYPE_D1_ID"]);
                        infoObjAdd.MM_PRODTYPE_H_ID = Convert.ToInt32(dtproditem.Rows[i]["MM_PRODTYPE_H_ID"]);
                        infoObjAdd.PROP_TYPE_MAIN = dtproditem.Rows[i]["PROP_TYPE_MAIN"] != DBNull.Value ? dtproditem.Rows[i]["PROP_TYPE_MAIN"].ToString() : "";
                        infoObjAdd.PROP_TYPE_SUB = dtproditem.Rows[i]["PROP_TYPE_SUB"] != DBNull.Value ? dtproditem.Rows[i]["PROP_TYPE_SUB"].ToString() : "";
                        infoObjAdd.PROP_CLASS = dtproditem.Rows[i]["PROP_CLASS"] != DBNull.Value ? dtproditem.Rows[i]["PROP_CLASS"].ToString() : "";
                        infoObjAdd.PROP_COLOUR = dtproditem.Rows[i]["PROP_COLOUR"] != DBNull.Value ? dtproditem.Rows[i]["PROP_COLOUR"].ToString() : "";
                        infoObjAdd.PROP_PACK = dtproditem.Rows[i]["PROP_PACK"] != DBNull.Value ? dtproditem.Rows[i]["PROP_PACK"].ToString() : "";

                        infoObjAdd.RECORD_TYP = dtproditem.Rows[i]["RECORD_TYP"] != DBNull.Value ? dtproditem.Rows[i]["RECORD_TYP"].ToString() : "";
                        infoObjAdd.CREATED_BY = dtproditem.Rows[i]["CREATED_BY"] != DBNull.Value ? dtproditem.Rows[i]["CREATED_BY"].ToString() : "";
                        infoObjAdd.CREATED_DATE = dtproditem.Rows[i]["CREATED_DATE"] != DBNull.Value ? dtproditem.Rows[i]["CREATED_DATE"].ToString() : "";
                        infoObjAdd.CREATED_LOC = dtproditem.Rows[i]["CREATED_LOC"] != DBNull.Value ? dtproditem.Rows[i]["CREATED_LOC"].ToString() : "";
                        infoObjAdd.UPDATED_BY = dtproditem.Rows[i]["UPDATED_BY"] != DBNull.Value ? dtproditem.Rows[i]["UPDATED_BY"].ToString() : "";
                        infoObjAdd.UPDATED_DATE = dtproditem.Rows[i]["UPDATED_DATE"] != DBNull.Value ? dtproditem.Rows[i]["UPDATED_DATE"].ToString() : "";
                        infoObjAdd.UPDATED_LOC = dtproditem.Rows[i]["UPDATED_LOC"] != DBNull.Value ? dtproditem.Rows[i]["UPDATED_LOC"].ToString() : "";

                        // Adding.
                        listItemsAdd2.Add(infoObjAdd);

                    }
                }


            }
            else
            {
                success = false;
                message = "Fail to get data";
            }
            var data = new { success = success, message = message, modelh = model, d1 = listItemsAdd1, d2 = listItemsAdd2 };

            return Json(data);
        }


        #endregion

        #region DELETE

        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> deleteProdType(List<string> lstid)
        {
            bool success = true;
            string message = "";

            ProdTypeModel modelh = new ProdTypeModel();

            if (lstid.Count < 1)
            {
                success = false;
                message += "Please select a record first to delete. ";
            }

            if (success && lstid.Count > 0)
            {
                foreach (var item in lstid)
                {
                    modelh.MM_PRODTYPE_H_ID = Convert.ToInt32(item);
                    modelh.RECORD_TYP = "5";

                    modelh.PROD_TYPE = "";
                    modelh.COMP_GROUP = "";

                    //delete h
                    string result1 = await dbdal.ProdType_H_Maint(modelh);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                    else
                    {
                        //delete d
                        string result2 = await dbdal.ProdType_D_Maint(0, Convert.ToInt32(item), 0, "2");
                        if (!(int.TryParse(result2, out int num2)))
                        {
                            success = false;
                            message += result2;
                        }
                        else
                        {
                            //delete d1
                            string result3 = await dbdal.ProdType_D1_Maint(0, Convert.ToInt32(item), "", "", "", "", "", "2");
                            if (!(int.TryParse(result3, out int num3)))
                            {
                                success = false;
                                message += result3;
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
        public async Task<ActionResult> MM_PROD_TYPE_AUDIT(string id)
        {
            TempData["SQ_ID"] = id;
            CommonFunction common = new CommonFunction();
            var param = new { pTable = "PVIEW_MM_PRODTYPE_A", pKeyValue = id };
            List<AuditTrailModel> AuditTrailModel = await common.PSP_COMMON_DAPPER<AuditTrailModel>("PSP_COMMON_AUDIT", CommandType.StoredProcedure, param) ?? new List<AuditTrailModel>();

            return View(AuditTrailModel);
        }

        #endregion
    }


}