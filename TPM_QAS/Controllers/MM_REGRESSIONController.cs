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
    public class MM_REGRESSIONController : Controller
    {
        DB dbmain = new DB();
        MM_REGRESSION_DAL dbdal = new MM_REGRESSION_DAL();

        #region LIST

        [SessionExpire]
        public async Task<ActionResult> MM_REGRESSION_LST(string Deleted = "0")
        {
            TempData["STATUS_DDL"] = await dbmain.PSP_COMMON_DDL("CMN_LST_STATUS", "", "", "", "", "", "");

            CommonFunction common = new CommonFunction();
            var param = new { Table = "PVIEW_MM_REGRESSION", TableID = "", Search = "", Value = "", SortField = "MM_REGRESSION_H_ID", Direction = "0", FrmRowno = "1", ToRowno = "500000", Deleted = Deleted };
            List<RegressionModel> model = await common.PSP_COMMON_DAPPER<RegressionModel>("PSP_COMMON_LIST", CommandType.StoredProcedure, param) ?? new List<RegressionModel>();

            ViewBag.Deleted = Deleted;
            return View(model);
        }

        #endregion

        #region VIEW/EDIT

        [SessionExpire]
        public async Task<ActionResult> MM_REGRESSION_DETAIL(string id)
        {
            ViewBag.Tittle = "Regression";
            var model = new RegressionModel();

            DataTable dt = await dbdal.getRegressionData(id, "H"); //get header only
            if (dt != null && dt.Rows.Count > 0)
            {
                model.MM_REGRESSION_H_ID = Convert.ToInt32(dt.Rows[0]["MM_REGRESSION_H_ID"]);
                model.COMP_GROUP = dt.Rows[0]["COMP_GROUP"] != DBNull.Value ? dt.Rows[0]["COMP_GROUP"].ToString() : "";
                model.PROD_LINE = dt.Rows[0]["PROD_LINE"] != DBNull.Value ? dt.Rows[0]["PROD_LINE"].ToString() : "";
                model.PROPERTIES = dt.Rows[0]["PROPERTIES"] != DBNull.Value ? dt.Rows[0]["PROPERTIES"].ToString() : "";
                model.COMPOUNDER = dt.Rows[0]["COMPOUNDER"] != DBNull.Value ? dt.Rows[0]["COMPOUNDER"].ToString() : "";
                model.PROPERTIES = dt.Rows[0]["PROPERTIES"] != DBNull.Value ? dt.Rows[0]["PROPERTIES"].ToString() : "";
                model.PROP_ITEM_ID = Convert.ToInt32(dt.Rows[0]["PROP_ITEM_ID"]);
                model.PROP_ITEM = dt.Rows[0]["PROP_ITEM"] != DBNull.Value ? dt.Rows[0]["PROP_ITEM"].ToString() : "";

                if (model.COMP_GROUP == "TPM")
                {
                    model.PROPERTIES_TPM = model.PROPERTIES;
                }
                else
                {
                    model.PROPERTIES_COMP = model.PROPERTIES;
                }

                model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";
            }

            DataTable dtl = await dbdal.getRegressionData(id, "D"); //get d data
            List<RegressionLstModel> listItemsAdd = new List<RegressionLstModel>();
            if (dtl != null && dtl.Rows.Count > 0)
            {
                for (int i = 0; i < dtl.Rows.Count; i++)
                {
                    RegressionLstModel infoObjAdd = new RegressionLstModel();

                    //infoObjAdd.MM_REGRESSION_D_ID = Convert.ToInt32(dtl.Rows[i]["MM_REGRESSION_D_ID"]);
                    infoObjAdd.MM_REGRESSION_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_REGRESSION_H_ID"]);
                    infoObjAdd.MACHINE_NAME_ID = Convert.ToInt32(dtl.Rows[i]["MACHINE_NAME_ID"]);
                    infoObjAdd.MACHINE_NAME = dtl.Rows[i]["MACHINE_NAME"] != DBNull.Value ? dtl.Rows[i]["MACHINE_NAME"].ToString() : "";
                    infoObjAdd.PROD_GROUP_ID_STRING = dtl.Rows[i]["PROD_GROUP_ID_STRING"] != DBNull.Value ? dtl.Rows[i]["PROD_GROUP_ID_STRING"].ToString() : "";

                    if (model.COMP_GROUP == "TPM")
                    {
                        infoObjAdd.MM_PRODGROUP_H_ID_TPM_STRING = infoObjAdd.PROD_GROUP_ID_STRING; //Convert.ToInt32(dtl.Rows[i]["MM_PRODGROUP_H_ID"]);
                        //infoObjAdd.PROD_GROUP_TPM = dtl.Rows[i]["PROD_GROUP"] != DBNull.Value ? dtl.Rows[i]["PROD_GROUP"].ToString() : "";
                        infoObjAdd.FORMULA_TPM = dtl.Rows[i]["FORMULA"] != DBNull.Value ? dtl.Rows[i]["FORMULA"].ToString() : "";
                    }
                    else
                    {
                        infoObjAdd.MM_PRODGROUP_H_ID_COMP_STRING = infoObjAdd.PROD_GROUP_ID_STRING; //Convert.ToInt32(dtl.Rows[i]["MM_PRODGROUP_H_ID"]);
                        //infoObjAdd.PROD_GROUP_COMP = dtl.Rows[i]["PROD_GROUP"] != DBNull.Value ? dtl.Rows[i]["PROD_GROUP"].ToString() : "";
                        infoObjAdd.FORMULA_COMP = dtl.Rows[i]["FORMULA"] != DBNull.Value ? dtl.Rows[i]["FORMULA"].ToString() : "";
                    }

                    // Adding.
                    listItemsAdd.Add(infoObjAdd);

                }

            }

            model.RegressionLstModel = listItemsAdd;

            model.DropdownProperty = await LoadDllData(0, "", "PROPERTIES");
            model.DropdownPropItem = await LoadDllData(0, model.PROPERTIES, "PROP_ITEM");
            model.DropdownCompounder = await LoadDllData(0, "", "COMPOUNDER");

            ViewBag.FROM = "PRE";
            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MM_REGRESSION_DETAIL(string ActionType, RegressionModel model)
        {
            ViewBag.Tittle = "Regression";
            try
            {
                DataTable dt = await dbdal.getRegressionData(model.MM_REGRESSION_H_ID.ToString(), "H"); 
                if (dt != null && dt.Rows.Count > 0)
                {
                    model.MM_REGRESSION_H_ID = Convert.ToInt32(dt.Rows[0]["MM_REGRESSION_H_ID"]);
                    model.COMP_GROUP = dt.Rows[0]["COMP_GROUP"] != DBNull.Value ? dt.Rows[0]["COMP_GROUP"].ToString() : "";
                    model.PROD_LINE = dt.Rows[0]["PROD_LINE"] != DBNull.Value ? dt.Rows[0]["PROD_LINE"].ToString() : "";
                    model.COMPOUNDER = dt.Rows[0]["COMPOUNDER"] != DBNull.Value ? dt.Rows[0]["COMPOUNDER"].ToString() : "";
                    //model.PROPERTIES = dt.Rows[0]["PROPERTIES"] != DBNull.Value ? dt.Rows[0]["PROPERTIES"].ToString() : "";
                    model.PROP_ITEM_ID = Convert.ToInt32(dt.Rows[0]["PROP_ITEM_ID"]);
                    model.PROP_ITEM = dt.Rows[0]["PROP_ITEM"] != DBNull.Value ? dt.Rows[0]["PROP_ITEM"].ToString() : "";

                    //if (model.COMP_GROUP == "TPM")
                    //{
                    //    model.PROPERTIES_TPM = model.PROPERTIES;
                    //}
                    //else
                    //{
                    //    model.PROPERTIES_COMP = model.PROPERTIES;
                    //}

                    model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                    model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                    model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                    model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                    model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                    model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                    model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";
                }

                DataTable dtl = await dbdal.getRegressionData(model.MM_REGRESSION_H_ID.ToString(), "D"); //get d data
                List<RegressionLstModel> listItemsAdd = new List<RegressionLstModel>();
                if (dtl != null && dtl.Rows.Count > 0)
                {
                    for (int i = 0; i < dtl.Rows.Count; i++)
                    {
                        RegressionLstModel infoObjAdd = new RegressionLstModel();

                        //infoObjAdd.MM_REGRESSION_D_ID = Convert.ToInt32(dtl.Rows[i]["MM_REGRESSION_D_ID"]);
                        infoObjAdd.MM_REGRESSION_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_REGRESSION_H_ID"]);
                        infoObjAdd.MACHINE_NAME_ID = Convert.ToInt32(dtl.Rows[i]["MACHINE_NAME_ID"]);
                        infoObjAdd.MACHINE_NAME = dtl.Rows[i]["MACHINE_NAME"] != DBNull.Value ? dtl.Rows[i]["MACHINE_NAME"].ToString() : "";
                        infoObjAdd.PROD_GROUP_ID_STRING = dtl.Rows[i]["PROD_GROUP_ID_STRING"] != DBNull.Value ? dtl.Rows[i]["PROD_GROUP_ID_STRING"].ToString() : "";

                        if (model.COMP_GROUP == "TPM")
                        {
                            infoObjAdd.MM_PRODGROUP_H_ID_TPM_STRING = infoObjAdd.PROD_GROUP_ID_STRING; //Convert.ToInt32(dtl.Rows[i]["MM_PRODGROUP_H_ID"]);
                            //infoObjAdd.PROD_GROUP_TPM = dtl.Rows[i]["PROD_GROUP"] != DBNull.Value ? dtl.Rows[i]["PROD_GROUP"].ToString() : "";
                            infoObjAdd.FORMULA_TPM = dtl.Rows[i]["FORMULA"] != DBNull.Value ? dtl.Rows[i]["FORMULA"].ToString() : "";
                        }
                        else
                        {
                            infoObjAdd.MM_PRODGROUP_H_ID_COMP_STRING = infoObjAdd.PROD_GROUP_ID_STRING; //Convert.ToInt32(dtl.Rows[i]["MM_PRODGROUP_H_ID"]);
                            //infoObjAdd.PROD_GROUP_COMP = dtl.Rows[i]["PROD_GROUP"] != DBNull.Value ? dtl.Rows[i]["PROD_GROUP"].ToString() : "";
                            infoObjAdd.FORMULA_COMP = dtl.Rows[i]["FORMULA"] != DBNull.Value ? dtl.Rows[i]["FORMULA"].ToString() : "";
                        }

                        // Adding.
                        listItemsAdd.Add(infoObjAdd);

                    }

                }

                model.RegressionLstModel = listItemsAdd;

                model.DropdownProperty = await LoadDllData(0, "", "PROPERTIES");
                model.DropdownCompounder = await LoadDllData(0, "", "COMPOUNDER");

                if (model.PROPERTIES_TPM != "" && model.PROPERTIES_TPM != null && !model.PROPERTIES_TPM.Contains("Select an option"))
                {
                    model.DropdownPropItem = await LoadDllData(0, model.PROPERTIES_TPM, "PROP_ITEM");
                }
                else
                {
                    if (model.PROPERTIES_COMP != "" && model.PROPERTIES_COMP != null && !model.PROPERTIES_COMP.Contains("Select an option"))
                    {
                        model.DropdownPropItem = await LoadDllData(0, model.PROPERTIES_COMP, "PROP_ITEM");
                    }
                    else
                    {
                        model.DropdownPropItem = new List<SelectListItem>();
                    }
                    
                }

                if (ActionType == "postback")
                {
                    ModelState.Clear();
                }
                if (ActionType == "Back")
                {
                    ModelState.Clear();
                    return RedirectToAction("MM_REGRESSION_LST", "MM_REGRESSION");
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
        public async Task<ActionResult> InsertUpdateRegression(RegressionModel model)
        {
            bool success = true;
            string message = "";


            if (ModelState.IsValid)
            {
                if (model.COMP_GROUP == null || model.COMP_GROUP == "")
                {
                    success = false;
                    message += "Please select Company Group";
                }

                if (success)
                {
                    if(model.COMP_GROUP == "TPM")
                    {
                        if (model.PROPERTIES_TPM == null || model.PROPERTIES_TPM == "" ||
                            model.PROP_ITEM_ID == 0 ||
                            model.RegressionLstModel == null || model.RegressionLstModel.Count < 1)
                        {
                            success = false;
                            message += "Required field cannot empty.";
                        }

                        if (success)
                        {
                            // insert h
                            string result1 = await dbdal.Regression_H_TPM_Maint(model);
                            if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                            {
                                success = false;
                                message += result1;
                            }
                            else
                            {
                                //delete existing d first
                                string result3 = await dbdal.Regression_D_TPM_Maint(0, Convert.ToInt32(result1), 0, 0, "","", "2");
                                if (!(int.TryParse(result3, out int num3)))
                                {
                                    success = false;
                                    message += result3;
                                }
                                else
                                {
                                    // insert d
                                    foreach (var item in model.RegressionLstModel)
                                    {
                                        int prodgroupid = item.MM_PRODGROUP_H_ID_TPM;
                                        string prodgroup = item.PROD_GROUP_TPM != null ? item.PROD_GROUP_TPM.ToString() : "";
                                        int machineid = item.MACHINE_NAME_ID;
                                        string machinename = item.MACHINE_NAME != null ? item.MACHINE_NAME.ToString() : "";
                                        string formula = item.FORMULA_TPM != null ? item.FORMULA_TPM.ToString() : "";
                                        string regressind = item.REGRESSIND != null ? item.REGRESSIND.ToString() : "";

                                        string rectype = item.RECORD_TYP != null ? item.RECORD_TYP.ToString() : "";

                                        if (rectype == "0")
                                        {
                                            rectype = "3";
                                        }

                                        string result2 = await dbdal.Regression_D_TPM_Maint(item.MM_REGRESSION_D_ID, Convert.ToInt32(result1), prodgroupid, machineid, formula, regressind, rectype);
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
                    else if(model.COMP_GROUP == "COMPOUNDER")
                    {
                        if (model.COMPOUNDER == null || model.COMPOUNDER == "" ||
                            model.PROPERTIES_COMP == null || model.PROPERTIES_COMP == "" ||
                            model.RegressionLstModel == null || model.RegressionLstModel.Count < 1)
                        {
                            success = false;
                            message += "Required field cannot empty.";
                        }

                        if (success)
                        {
                            // insert h
                            string result1 = await dbdal.Regression_H_COMP_Maint(model);
                            if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                            {
                                success = false;
                                message += result1;
                            }
                            else
                            {
                                //delete existing d first
                                string result3 = await dbdal.Regression_D_COMP_Maint(0, Convert.ToInt32(result1), 0, "","", "2");
                                if (!(int.TryParse(result3, out int num3)))
                                {
                                    success = false;
                                    message += result3;
                                }
                                else
                                {
                                    // insert d
                                    foreach (var item in model.RegressionLstModel)
                                    {
                                        int prodgroupid = item.MM_PRODGROUP_H_ID_COMP;
                                        string prodgroup = item.PROD_GROUP_COMP != null ? item.PROD_GROUP_COMP.ToString() : "";
                                        string formula = item.FORMULA_COMP != null ? item.FORMULA_COMP.ToString() : "";
                                        string regressind = item.REGRESSIND != null ? item.REGRESSIND.ToString() : "";


                                        string rectype = item.RECORD_TYP != null ? item.RECORD_TYP.ToString() : "";

                                        if (rectype == "0")
                                        {
                                            rectype = "3";
                                        }

                                        string result2 = await dbdal.Regression_D_COMP_Maint(item.MM_REGRESSION_D_ID, Convert.ToInt32(result1), prodgroupid, formula, regressind, rectype);
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
                        message += "Error in saving data.";
                    }
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
        public async Task<ActionResult> MM_REGRESSION_ADD(string id = "")
        {
            ViewBag.Tittle = "Regression";
            var model = new RegressionModel();
            List<RegressionLstModel> listItemsAdd = new List<RegressionLstModel>();

            model.DropdownProperty = await LoadDllData(0, "", "PROPERTIES");
            model.DropdownPropItem = new List<SelectListItem>();
            model.DropdownCompounder = await LoadDllData(0, "", "COMPOUNDER");

            if (!string.IsNullOrWhiteSpace(id) && CommonMethod.isNumeric(id))
            {
                DataTable dt = await dbdal.getRegressionData(id, "H"); //get header data

                if (dt != null && dt.Rows.Count > 0)
                {
                    model.MM_REGRESSION_H_ID = Convert.ToInt32(dt.Rows[0]["MM_REGRESSION_H_ID"]);
                    model.COMP_GROUP = dt.Rows[0]["COMP_GROUP"] != DBNull.Value ? dt.Rows[0]["COMP_GROUP"].ToString() : "";
                    model.PROD_LINE = dt.Rows[0]["PROD_LINE"] != DBNull.Value ? dt.Rows[0]["PROD_LINE"].ToString() : "";
                    model.COMPOUNDER = dt.Rows[0]["COMPOUNDER"] != DBNull.Value ? dt.Rows[0]["COMPOUNDER"].ToString() : "";
                    model.PROPERTIES = dt.Rows[0]["PROPERTIES"] != DBNull.Value ? dt.Rows[0]["PROPERTIES"].ToString() : "";
                    model.PROP_ITEM_ID = Convert.ToInt32(dt.Rows[0]["PROP_ITEM_ID"]);
                    model.PROP_ITEM = dt.Rows[0]["PROP_ITEM"] != DBNull.Value ? dt.Rows[0]["PROP_ITEM"].ToString() : "";

                    if(model.COMP_GROUP == "TPM")
                    {
                        model.PROPERTIES_TPM = model.PROPERTIES;
                    }
                    else
                    {
                        model.PROPERTIES_COMP = model.PROPERTIES;
                    }

                    model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                    model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                    model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                    model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                    model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                    model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                    model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";

                    model.DropdownPropItem = await LoadDllData(0, model.PROPERTIES, "PROP_ITEM");

                    DataTable dtl = await dbdal.getRegressionData(id, "D"); //get d data
                    if (dtl != null && dtl.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtl.Rows.Count; i++)
                        {
                            RegressionLstModel infoObjAdd = new RegressionLstModel();

                            //infoObjAdd.MM_REGRESSION_D_ID = Convert.ToInt32(dtl.Rows[i]["MM_REGRESSION_D_ID"]);
                            infoObjAdd.MM_REGRESSION_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_REGRESSION_H_ID"]);
                            infoObjAdd.MACHINE_NAME_ID = Convert.ToInt32(dtl.Rows[i]["MACHINE_NAME_ID"]);
                            infoObjAdd.MACHINE_NAME = dtl.Rows[i]["MACHINE_NAME"] != DBNull.Value ? dtl.Rows[i]["MACHINE_NAME"].ToString() : "";
                            infoObjAdd.PROD_GROUP_ID_STRING = dtl.Rows[i]["PROD_GROUP_ID_STRING"] != DBNull.Value ? dtl.Rows[i]["PROD_GROUP_ID_STRING"].ToString() : "";

                            if (model.COMP_GROUP == "TPM")
                            {
                                infoObjAdd.MM_PRODGROUP_H_ID_TPM_STRING = infoObjAdd.PROD_GROUP_ID_STRING; //Convert.ToInt32(dtl.Rows[i]["MM_PRODGROUP_H_ID"]);
                                //infoObjAdd.PROD_GROUP_TPM = dtl.Rows[i]["PROD_GROUP"] != DBNull.Value ? dtl.Rows[i]["PROD_GROUP"].ToString() : "";
                                infoObjAdd.FORMULA_TPM = dtl.Rows[i]["FORMULA"] != DBNull.Value ? dtl.Rows[i]["FORMULA"].ToString() : "";
                            }
                            else
                            {
                                infoObjAdd.MM_PRODGROUP_H_ID_COMP_STRING = infoObjAdd.PROD_GROUP_ID_STRING; //Convert.ToInt32(dtl.Rows[i]["MM_PRODGROUP_H_ID"]);
                                //infoObjAdd.PROD_GROUP_COMP = dtl.Rows[i]["PROD_GROUP"] != DBNull.Value ? dtl.Rows[i]["PROD_GROUP"].ToString() : "";
                                infoObjAdd.FORMULA_COMP = dtl.Rows[i]["FORMULA"] != DBNull.Value ? dtl.Rows[i]["FORMULA"].ToString() : "";
                            }

                            // Adding.
                            listItemsAdd.Add(infoObjAdd);

                        }
                    }
                }
            }
            else
            {
                model = new RegressionModel();

                model.DropdownProperty = await LoadDllData(0, "", "PROPERTIES");
                model.DropdownPropItem = new List<SelectListItem>();
                model.DropdownCompounder = await LoadDllData(0, "", "COMPOUNDER");
            }

            model.RegressionLstModel = listItemsAdd;
            ViewBag.FROM = "PRE";
            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MM_REGRESSION_ADD(string ActionType, RegressionModel model)
        {
            ViewBag.Tittle = "Regression";
            try
            {
                model.DropdownProperty = await LoadDllData(0, "", "PROPERTIES");
                model.DropdownCompounder = await LoadDllData(0, "", "COMPOUNDER");

                if (model.PROPERTIES_TPM != "" && model.PROPERTIES_TPM != null && !model.PROPERTIES_TPM.Contains("Select an option"))
                {
                    model.DropdownPropItem = await LoadDllData(0, model.PROPERTIES_TPM, "PROP_ITEM");
                }
                else
                {
                    if (model.PROPERTIES_COMP != "" && model.PROPERTIES_COMP != null && !model.PROPERTIES_COMP.Contains("Select an option"))
                    {
                        model.DropdownPropItem = await LoadDllData(0, model.PROPERTIES_COMP, "PROP_ITEM");
                    }
                    else
                    {
                        model.DropdownPropItem = new List<SelectListItem>();
                    }
                    
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
        public async Task<ActionResult> DraftRegression(RegressionModel model)
        {
            bool success = true;
            string message = "";


            if (ModelState.IsValid)
            {
                if (model.COMP_GROUP == null || model.COMP_GROUP == "")
                {
                    success = false;
                    message += "Please select Company Group";
                }

                if (success)
                {
                    if (model.COMP_GROUP == "TPM")
                    {
                        if (model.PROPERTIES_TPM == null || model.PROPERTIES_TPM == "" ||
                            model.PROP_ITEM_ID == 0)
                        {
                            success = false;
                            message += "Required field cannot empty.";
                        }

                        if (success)
                        {
                            // insert h
                            string result1 = await dbdal.Regression_H_TPM_Maint(model);
                            if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                            {
                                success = false;
                                message += result1;
                            }
                            else
                            {
                                if (model.RegressionLstModel != null && model.RegressionLstModel.Count > 0)
                                {
                                    //delete existing d first
                                    string result3 = await dbdal.Regression_D_TPM_Maint(0, Convert.ToInt32(result1), 0,0,"", "", "2");
                                    if (!(int.TryParse(result3, out int num3)))
                                    {
                                        success = false;
                                        message += result3;
                                    }
                                    else
                                    {
                                        // insert d
                                        foreach (var item in model.RegressionLstModel)
                                        {

                                            int prodgroupid = item.MM_PRODGROUP_H_ID_TPM;
                                            string prodgroup = item.PROD_GROUP_TPM != null ? item.PROD_GROUP_TPM.ToString() : "";
                                            int machineid = item.MACHINE_NAME_ID;
                                            string machinename = item.MACHINE_NAME != null ? item.MACHINE_NAME.ToString() : "";
                                            string formula = item.FORMULA_TPM != null ? item.FORMULA_TPM.ToString() : "";
                                            string regressind = item.REGRESSIND != null ? item.REGRESSIND.ToString() : "";


                                            string rectype = item.RECORD_TYP != null ? item.RECORD_TYP.ToString() : "";


                                            string result2 = await dbdal.Regression_D_TPM_Maint(item.MM_REGRESSION_D_ID, Convert.ToInt32(result1), prodgroupid, machineid, formula, regressind, rectype);
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
                    else if(model.COMP_GROUP == "COMPOUNDER")
                    {
                        if (model.COMPOUNDER == null || model.COMPOUNDER == "" ||
                            model.PROPERTIES_COMP == null || model.PROPERTIES_COMP == "")
                        {
                            success = false;
                            message += "Required field cannot empty.";
                        }

                        if (success)
                        {
                            // insert h
                            string result1 = await dbdal.Regression_H_COMP_Maint(model);
                            if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                            {
                                success = false;
                                message += result1;
                            }
                            else
                            {
                                if (model.RegressionLstModel != null && model.RegressionLstModel.Count > 0)
                                {
                                    //delete existing d first
                                    string result3 = await dbdal.Regression_D_COMP_Maint(0, Convert.ToInt32(result1), 0,"","", "2");
                                    if (!(int.TryParse(result3, out int num3)))
                                    {
                                        success = false;
                                        message += result3;
                                    }
                                    else
                                    {
                                        // insert d
                                        foreach (var item in model.RegressionLstModel)
                                        {

                                            int prodgroupid = item.MM_PRODGROUP_H_ID_COMP;
                                            string prodgroup = item.PROD_GROUP_COMP != null ? item.PROD_GROUP_COMP.ToString() : "";
                                            string formula = item.FORMULA_COMP != null ? item.FORMULA_COMP.ToString() : "";
                                            string regressind = item.REGRESSIND != null ? item.REGRESSIND.ToString() : "";

                                            string rectype = item.RECORD_TYP != null ? item.RECORD_TYP.ToString() : "";

                                            string result2 = await dbdal.Regression_D_COMP_Maint(item.MM_REGRESSION_D_ID, Convert.ToInt32(result1), prodgroupid, formula, regressind, rectype);
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
                        message += "Error in saving data.";
                    }
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
        public async Task<ActionResult> deleteRegression(List<string> lstid)
        {
            bool success = true;
            string message = "";

            RegressionModel model = new RegressionModel();

            if (lstid.Count < 1)
            {
                success = false;
                message += "Please select a record first to delete.";
            }

            if (success && lstid.Count > 0)
            {
                foreach (var item in lstid)
                {
                    model.MM_REGRESSION_H_ID = Convert.ToInt32(item);
                    model.RECORD_TYP = "5";

                    model.COMP_GROUP = "";
                    model.PROD_LINE = "";
                    model.PROPERTIES_TPM = "";
                    model.PROP_ITEM_ID = 0;

                    //delete h
                    string result1 = await dbdal.Regression_H_TPM_Maint(model);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                    else
                    {
                        //delete d
                        string result2 = await dbdal.Regression_D_TPM_Maint(0, Convert.ToInt32(item), 0, 0, "","", "2");
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
        public async Task<ActionResult> MM_REGRESSION_AUDIT(string id)
        {
            TempData["SQ_ID"] = id;
            CommonFunction common = new CommonFunction();
            var param = new { pTable = "PVIEW_MM_REGRESSION_A", pKeyValue = id };
            List<AuditTrailModel> AuditTrailModel = await common.PSP_COMMON_DAPPER<AuditTrailModel>("PSP_COMMON_AUDIT", CommandType.StoredProcedure, param) ?? new List<AuditTrailModel>();

            return View(AuditTrailModel);
        }

        #endregion

        #region DDL

        public async Task<ActionResult> fillProdGroup()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items = await LoadInnerDllData(0, "", "PROD_GROUP");
            items.RemoveAll(item => item.Text == "Select an option.." && item.Value == "");
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> fillMachine(string propitemid)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items = await LoadInnerDllDataMach(0, propitemid, "MACHINE_NAME");

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        private async Task<List<SelectListItem>> LoadInnerDllDataMach(int ID, string act, string category)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            listItems.Add(new SelectListItem { Text = "Select an option..", Value = "" });
            listItems.Add(new SelectListItem { Text = "Manual Input", Value = "0" });

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