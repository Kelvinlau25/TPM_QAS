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
using Newtonsoft.Json;
using System.Configuration;
using System.Data.SqlClient;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Bibliography;

namespace TPM_QAS.Controllers
{
    public class COA_MM_MATERIALController : Controller
    {
        DB dbmain = new DB();
        COA_MM_MATERIAL_DAL dbdal = new COA_MM_MATERIAL_DAL();

        #region LIST

        [SessionExpire]
        public async Task<ActionResult> COA_MM_MATERIAL_LST(string Deleted = "0")
        {
            if (Deleted == "1")
            {
                TempData["STATUS_DDL"] = await dbmain.PSP_COMMON_DDL("CMN_LST_STATUS", "", "", "", "", "", "");
            }
            else
            {
                TempData["STATUS_DDL"] = await dbmain.PSP_COMMON_DDL("CMN_LST_STATUS2", "", "", "", "", "", "");
            }

            CommonFunction common = new CommonFunction();
            var param = new { Table = "PVIEW_MM_MATERIAL", TableID = "", Search = "", Value = "", SortField = "MM_MATERIAL_H_ID", Direction = "0", FrmRowno = "1", ToRowno = "500000", Deleted = Deleted };
            List<COAMMMaterialModel> model = await common.PSP_COMMON_DAPPER<COAMMMaterialModel>("PSP_COMMON_LIST", CommandType.StoredProcedure, param) ?? new List<COAMMMaterialModel>();

            ViewBag.Deleted = Deleted;
            return View(model);
        }

        #endregion

        //#region VIEW/EDIT

        [SessionExpire]
        public async Task<ActionResult> COA_MM_MATERIAL_DETAIL(string id)
        {
            ViewBag.Tittle = "Raw Material Specification";
            var model = new COAMMMaterialModel();

            List<SelectListItem> SuppList = await LoadDllData(0, "", "SUPPLIER");
            List<SelectListItem> UOMLIST = await LoadDllData(0, "", "UNIT");
            ViewBag.SerializedSupplierName = JsonConvert.SerializeObject(SuppList);
            ViewBag.SerializedUOM = JsonConvert.SerializeObject(UOMLIST);

            DataTable dt = await dbdal.getCOAMMMaterialSel(id, "H"); //get header data
            
            if (dt != null && dt.Rows.Count > 0)
            {
                model.MM_MATERIAL_H_ID = Convert.ToInt32(dt.Rows[0]["MM_MATERIAL_H_ID"]);
                model.MATERIAL_NAME = dt.Rows[0]["MATERIAL_NAME"] != DBNull.Value ? dt.Rows[0]["MATERIAL_NAME"].ToString() : "";
                model.MATERIAL_ABBR = dt.Rows[0]["MATERIAL_ABBR"] != DBNull.Value ? dt.Rows[0]["MATERIAL_ABBR"].ToString() : "";
                model.MATERIAL_CODE = dt.Rows[0]["MATERIAL_CODE"] != DBNull.Value ? dt.Rows[0]["MATERIAL_CODE"].ToString() : "";
                model.TPM_CODE = dt.Rows[0]["TPM_CODE"] != DBNull.Value ? dt.Rows[0]["TPM_CODE"].ToString() : "";

                model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";
            }

            DataTable dtd1 = await dbdal.getCOAMMMaterialSel(id, "D1"); //get d1 data

            List<TPMMatSpecList> listItemsAdd = new List<TPMMatSpecList>();
            if (dtd1 != null && dtd1.Rows.Count > 0)
            {
                for (int i = 0; i < dtd1.Rows.Count; i++)
                {
                    TPMMatSpecList infoObjAdd = new TPMMatSpecList();

                    infoObjAdd.MM_MATERIAL_D1_ID = Convert.ToInt32(dtd1.Rows[i]["MM_MATERIAL_D1_ID"]);
                    infoObjAdd.MM_MATERIAL_H_ID = Convert.ToInt32(dtd1.Rows[i]["MM_MATERIAL_H_ID"]);
                    infoObjAdd.TPM_SPEC_NAME = dtd1.Rows[i]["TPM_SPEC_NAME"] != DBNull.Value ? dtd1.Rows[i]["TPM_SPEC_NAME"].ToString() : "";
                    infoObjAdd.TPM_SPEC_VALUE = dtd1.Rows[i]["TPM_SPEC_VALUE"] != DBNull.Value ? dtd1.Rows[i]["TPM_SPEC_VALUE"].ToString() : "";
                    infoObjAdd.TPM_SPEC_TYPE = dtd1.Rows[i]["TPM_SPEC_TYPE"] != DBNull.Value ? dtd1.Rows[i]["TPM_SPEC_TYPE"].ToString() : "";
                    infoObjAdd.TPM_SPEC_UOM = dtd1.Rows[i]["TPM_SPEC_UOM"] != DBNull.Value ? dtd1.Rows[i]["TPM_SPEC_UOM"].ToString() : "";
                    listItemsAdd.Add(infoObjAdd);
                }
            }

            DataTable dtd2 = await dbdal.getCOAMMMaterialSel(id, "D2"); //get d2 data
            List<SUPPMatSpecList> listItems2Add = new List<SUPPMatSpecList>();
            if (dtd2 != null && dtd2.Rows.Count > 0)
            {
                for (int i = 0; i < dtd2.Rows.Count; i++)
                {
                    SUPPMatSpecList infoObjAdd2 = new SUPPMatSpecList();

                    infoObjAdd2.MM_MATERIAL_D2_ID = Convert.ToInt32(dtd2.Rows[i]["MM_MATERIAL_D2_ID"]);
                    infoObjAdd2.MM_MATERIAL_D1_ID = Convert.ToInt32(dtd2.Rows[i]["MM_MATERIAL_D1_ID"]);
                    infoObjAdd2.MM_MATERIAL_H_ID = Convert.ToInt32(dtd2.Rows[i]["MM_MATERIAL_H_ID"]);
                    infoObjAdd2.MM_SUPPLIER_H_ID = Convert.ToInt32(dtd2.Rows[i]["MM_SUPPLIER_H_ID"]);
                    infoObjAdd2.SUPPLIER_NAME = dtd2.Rows[i]["SUPPLIER_NAME"] != DBNull.Value ? dtd2.Rows[i]["SUPPLIER_NAME"].ToString() : "";
                    infoObjAdd2.SUPP_MATERIAL_NAME = dtd2.Rows[i]["SUPPLIER_MATERIAL_NAME"] != DBNull.Value ? dtd2.Rows[i]["SUPPLIER_MATERIAL_NAME"].ToString() : "";
                    infoObjAdd2.TPM_SPEC_NAME = dtd2.Rows[i]["TPM_SPEC_NAME"] != DBNull.Value ? dtd2.Rows[i]["TPM_SPEC_NAME"].ToString() : "";
                    infoObjAdd2.SUPP_SPEC_NAME = dtd2.Rows[i]["SUPPLIER_SPEC_NAME"] != DBNull.Value ? dtd2.Rows[i]["SUPPLIER_SPEC_NAME"].ToString() : "";
                    infoObjAdd2.SUPP_SPEC_VALUE = dtd2.Rows[i]["SUPPLIER_SPEC_VALUE"] != DBNull.Value ? dtd2.Rows[i]["SUPPLIER_SPEC_VALUE"].ToString() : "";
                    infoObjAdd2.SUPP_SPEC_TYPE = dtd2.Rows[i]["SUPPLIER_SPEC_TYPE"] != DBNull.Value ? dtd2.Rows[i]["SUPPLIER_SPEC_TYPE"].ToString() : "";
                    infoObjAdd2.SUPP_SPEC_UOM = dtd2.Rows[i]["SUPPLIER_SPEC_UOM"] != DBNull.Value ? dtd2.Rows[i]["SUPPLIER_SPEC_UOM"].ToString() : "";
                    listItems2Add.Add(infoObjAdd2);
                }
            }

            model.TPMMatSpecList = listItemsAdd;
            model.SUPPMatSpecList = listItems2Add;
            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        //public async Task<ActionResult> ENT_EXT_IDS_ADD(DailyIDSTransVM m, List<allbagandprop> allbagandprop, List<allbagandprop> allbagandpropYI, string action)
        //m: model, TPMModel: TPMModel, SUPPModel: SUPPModel, action: action
        public async Task<ActionResult> InsertUpdateCOAMaterial(COAMMMaterialModel m, List<TPMMatSpecList> TPMModel, List<SUPPMatSpecList> SUPPModel, string action)
        {
            bool success = true;
            string message = "";

            if (ModelState.IsValid)
            {
                if (m.MATERIAL_NAME == "" && m.MATERIAL_ABBR == "" && m.MATERIAL_CODE == "" && m.TPM_CODE == "")
                {
                    success = false;
                    message += "Required field cannot empty.";
                }

                if (success)
                {
                    // insert h
                    string result1 = await dbdal.COAMM_MATERIAL_H_Maint(m);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += "Error in saving data : " + result1;                        
                    }
                    else
                    {
                        string resultd1 = "";
                        m.MM_MATERIAL_H_ID = Convert.ToInt32(result1);
                        await COA_MM_MATERIAL_D_MAINT(m, TPMModel, SUPPModel, resultd1);

                        if (resultd1 != "")
                        {
                            success = false;
                            message += "Error in saving data : " + resultd1;
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

        public async Task COA_MM_MATERIAL_D_MAINT(COAMMMaterialModel m, List<TPMMatSpecList> TPMModel, List<SUPPMatSpecList> SUPPModel, string resultd1)
        {
            try
            {
                bool cont = true;
                string message = "";
                int cnt = 0;
                //NEED TO CHECK IF ALL IN TABLE IS IN DATATABLE, IF NOT NEED TO DELETE
                DataTable dtd1 = await dbdal.getCOAMMMaterialSel(m.MM_MATERIAL_H_ID.ToString(), "D1"); //get d1 data
                DataTable dtd2 = await dbdal.getCOAMMMaterialSel(m.MM_MATERIAL_H_ID.ToString(), "D2"); //get d1 data

                //item start
                List<TPMMatSpecList> listItemsINDB = new List<TPMMatSpecList>();
                if (dtd1 != null && dtd1.Rows.Count > 0)
                {
                    for (int i = 0; i < dtd1.Rows.Count; i++)
                    {
                        TPMMatSpecList infoObjAdd = new TPMMatSpecList();

                        infoObjAdd.MM_MATERIAL_H_ID = Convert.ToInt32(dtd1.Rows[i]["MM_MATERIAL_H_ID"]);
                        infoObjAdd.TPM_SPEC_NAME = dtd1.Rows[i]["TPM_SPEC_NAME"] != DBNull.Value ? dtd1.Rows[i]["TPM_SPEC_NAME"].ToString() : "";
                        infoObjAdd.TPM_SPEC_UOM = dtd1.Rows[i]["TPM_SPEC_UOM"] != DBNull.Value ? dtd1.Rows[i]["TPM_SPEC_UOM"].ToString() : "";
                        infoObjAdd.TPM_SPEC_UOM_ID = dtd1.Rows[i]["TPM_SPEC_UOM_ID"] != DBNull.Value ? dtd1.Rows[i]["TPM_SPEC_UOM_ID"].ToString() : "";
                        infoObjAdd.TPM_SPEC_VALUE = dtd1.Rows[i]["TPM_SPEC_VALUE"] != DBNull.Value ? dtd1.Rows[i]["TPM_SPEC_VALUE"].ToString() : "";
                        infoObjAdd.MM_MATERIAL_D1_ID = Convert.ToInt32(dtd1.Rows[i]["MM_MATERIAL_D1_ID"]);
                        listItemsINDB.Add(infoObjAdd);
                    }
                }

                var itemsOnlyInDB = listItemsINDB
                    .Where(dbItem => !TPMModel.Any(modelItem =>
                        modelItem.TPM_SPEC_NAME == dbItem.TPM_SPEC_NAME))
                    .ToList();
                if (itemsOnlyInDB.Count > 0)
                {
                    foreach (var item in itemsOnlyInDB)
                    {
                        string tpmdetresult = await dbdal.COAMM_MATERIAL_D1_Maint(0, m.MM_MATERIAL_H_ID, m.MATERIAL_NAME,
                        item.TPM_SPEC_NAME, "", "", "", "N", "2"); //delete at db

                    }
                }
               
                var duplicateGroups = listItemsINDB
                    .GroupBy(item => new { item.TPM_SPEC_NAME, item.TPM_SPEC_VALUE, item.TPM_SPEC_UOM })
                    .Where(group => group.Count() > 1)
                    .ToList();

                if (duplicateGroups.Any())
                {
                    foreach (var group in duplicateGroups)
                    {
                        var NAMEtodel = group.Key.TPM_SPEC_NAME.ToString();
                        var VALUEtodel = group.Key.TPM_SPEC_VALUE.ToString();
                        var UOMtodel = group.Key.TPM_SPEC_UOM.ToString();

                        var dbdup = listItemsINDB.FirstOrDefault(item =>
                            item.TPM_SPEC_NAME == NAMEtodel &&
                            item.TPM_SPEC_VALUE == VALUEtodel &&
                            item.TPM_SPEC_UOM == UOMtodel);

                        if (dbdup != null)
                        {
                            string tpmdetresult = await dbdal.COAMM_MATERIAL_D1_Maint(dbdup.MM_MATERIAL_D1_ID, m.MM_MATERIAL_H_ID, m.MATERIAL_NAME,
                            NAMEtodel, VALUEtodel, "", dbdup.TPM_SPEC_UOM_ID, "N", "2"); //delete at db

                        }
                    }
                }
                //item end
                //supplier start
                //1.remove supplier totally 
                //2.change suppler spec
                List<SUPPMatSpecList> listsuppitemINDB = new List<SUPPMatSpecList>();
                List<SUPPMatSpecList> listsuppinDB = new List<SUPPMatSpecList>();
                List<SUPPMatSpecList> listsuppINSYS = new List<SUPPMatSpecList>();

                if (dtd2 != null && dtd2.Rows.Count > 0)
                {
                    for (int i = 0; i < dtd2.Rows.Count; i++)
                    {
                        SUPPMatSpecList infoObjAdd = new SUPPMatSpecList();
                        infoObjAdd.MM_MATERIAL_H_ID = Convert.ToInt32(dtd2.Rows[i]["MM_MATERIAL_H_ID"]);
                        infoObjAdd.MM_SUPPLIER_H_ID = Convert.ToInt32(dtd2.Rows[i]["MM_SUPPLIER_H_ID"]); ;
                        listsuppitemINDB.Add(infoObjAdd);
                    }
                }

                listsuppinDB = listsuppitemINDB
                .GroupBy(x => x.MM_SUPPLIER_H_ID)
                .Select(g => g.First())
                .ToList();

                listsuppINSYS =  SUPPModel
                .GroupBy(x => x.MM_SUPPLIER_H_ID)
                .Select(g => g.First())
                .ToList();

                var listsuppONLYinDB = listsuppinDB
                    .Where(dbItem => !listsuppINSYS.Any(modelItem =>
                        modelItem.MM_SUPPLIER_H_ID == dbItem.MM_SUPPLIER_H_ID))
                    .ToList();

                if (listsuppONLYinDB.Count > 0)
                {
                    foreach (var item in listsuppONLYinDB)
                    {
                        string tpmdetsuppresult = await dbdal.COAMM_MATERIAL_D2_Maint(0, m.MM_MATERIAL_H_ID, item.MM_SUPPLIER_H_ID,
                        0, "", "", "", "", "", "", "N", "2"); //delete at db

                    }
                }

                //var suppOnlyInDB = listsuppINDB
                //    .Where(dbItem => !SUPPModel.Any(modelItem =>
                //        modelItem.MM_SUPPLIER_H_ID == dbItem.MM_SUPPLIER_H_ID))
                //    .ToList();
                //if (suppOnlyInDB.Count > 0)
                //{
                //    foreach (var item in suppOnlyInDB)
                //    {
                //        string tpmdetsuppresult = await dbdal.COAMM_MATERIAL_D2_Maint(0, m.MM_MATERIAL_H_ID, item.MM_SUPPLIER_H_ID,
                //        0, "", "", "","","","", "N", "2"); //delete at db

                //    }
                //}

                //if (duplicateSuppGroups.Any())
                //{
                //    foreach (var group in duplicateSuppGroups)
                //    {
                //        var supptodel = group.Key.MM_SUPPLIER_H_ID.ToString();

                //        var dbdup = listsuppINDB.FirstOrDefault(item => item.MM_SUPPLIER_H_ID.ToString() == supptodel);

                //        if (dbdup != null)
                //        {
                //            string tpmdetresult = await dbdal.COAMM_MATERIAL_D2_Maint(dbdup.MM_MATERIAL_D2_ID, m.MM_MATERIAL_H_ID, Convert.ToInt16(supptodel),
                //            0,"","", "", "", "","", "N", "2"); //delete at db

                //        }
                //    }
                //}
                //supplier end

                //NEED TO CHECK IF ALL IN TABLE IS IN DATATABLE, IF NOT NEED TO DELETE end

                foreach (var item in TPMModel)
                {
                    int detid = item.MM_MATERIAL_D1_ID;
                    string pfirst = cnt == 0 ? "Y" : "N";
                    string tpmdetresult = await dbdal.COAMM_MATERIAL_D1_Maint(0, m.MM_MATERIAL_H_ID, m.MATERIAL_NAME,
                        item.TPM_SPEC_NAME, item.TPM_SPEC_VALUE, item.TPM_SPEC_TYPE == null ? "" : item.TPM_SPEC_TYPE, item.TPM_SPEC_UOM == null ? "" : item.TPM_SPEC_UOM, pfirst, m.RECORD_TYP);

                    if (!(int.TryParse(tpmdetresult, out int num1) && tpmdetresult != "0"))
                    {
                        cont = false;
                        message += tpmdetresult;
                    }
                    else
                    {
                        item.MM_MATERIAL_D1_ID = Convert.ToInt32(tpmdetresult);
                    }
                    cnt = cnt + 1;
                }

                if (cont)
                {
                    if (SUPPModel != null)
                    {
                        //populate _d1_id base on name
                        foreach (var tpmitem in TPMModel)
                        {

                            foreach (var item in SUPPModel.Where(item => item.TPM_SPEC_NAME == tpmitem.TPM_SPEC_NAME))
                            {
                                item.MM_MATERIAL_D1_ID = tpmitem.MM_MATERIAL_D1_ID;
                            }

                        }
                        //-------
                        // need to select distinct supplier first
                        if (cont)
                        {
                            List<SUPPMatSpecList> supplierlist = new List<SUPPMatSpecList>();
                            supplierlist = SUPPModel.GroupBy(x => new { x.MM_SUPPLIER_H_ID })
                                .Select(g => new SUPPMatSpecList
                                {
                                    MM_SUPPLIER_H_ID = g.Key.MM_SUPPLIER_H_ID
                                }).ToList();

                            int cntitemsupp = 0;

                            foreach (var supplier in supplierlist) //foreach supplier first
                            {

                                List<SUPPMatSpecList> thissuppitems = SUPPModel
                                                            .Where(x => x.MM_SUPPLIER_H_ID == supplier.MM_SUPPLIER_H_ID)
                                                            .ToList();

                                foreach (var supitems in thissuppitems)
                                {
                                    int detid = supitems.MM_MATERIAL_D2_ID;
                                    string pfirst = cntitemsupp == 0 ? "Y" : "N";

                                    if (supitems.SUPP_SPEC_NAME != "" && supitems.SUPP_SPEC_NAME != null)
                                    {
                                        string tpmdetresult = await dbdal.COAMM_MATERIAL_D2_Maint(0, m.MM_MATERIAL_H_ID, supitems.MM_SUPPLIER_H_ID, supitems.MM_MATERIAL_D1_ID, m.MATERIAL_NAME, 
                                            supitems.SUPP_MATERIAL_NAME, supitems.SUPP_SPEC_NAME, supitems.SUPP_SPEC_VALUE,
                                            supitems.SUPP_SPEC_TYPE == null ? "" : supitems.SUPP_SPEC_TYPE,
                                            supitems.SUPP_SPEC_UOM == null ? "" : supitems.SUPP_SPEC_UOM, pfirst, m.RECORD_TYP);

                                        if (!(int.TryParse(tpmdetresult, out int num1) && tpmdetresult != "0"))
                                        {
                                            cont = false;
                                            message += tpmdetresult;
                                        }
                                        else
                                        {
                                            supitems.MM_MATERIAL_D2_ID = Convert.ToInt32(tpmdetresult);
                                        }
                                        cntitemsupp = cntitemsupp + 1;
                                    } else
                                    {
                                        string tpmdetresult = await dbdal.COAMM_MATERIAL_D2_Maint(0, m.MM_MATERIAL_H_ID, supitems.MM_SUPPLIER_H_ID, supitems.MM_MATERIAL_D1_ID, m.MATERIAL_NAME,
                                            supitems.SUPP_MATERIAL_NAME, supitems.SUPP_SPEC_NAME, supitems.SUPP_SPEC_VALUE,
                                            supitems.SUPP_SPEC_TYPE == null ? "" : supitems.SUPP_SPEC_TYPE,
                                            supitems.SUPP_SPEC_UOM == null ? "" : supitems.SUPP_SPEC_UOM, pfirst, "2");
                                    }
                                }
                            }
                        }
                    }
                }

                if (!(cont))
                {
                    resultd1 = message;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                resultd1 = "Error : " + ex.Message;
            }
        }

        //#endregion

       #region ADD

        [SessionExpire]
        public async Task<ActionResult> COA_MM_MATERIAL_ADD()
        {
            ViewBag.Tittle = "TPM Qaw Material Specification";
            var model = new COAMMMaterialModel();

            CommonFunction common = new CommonFunction();
            var param = new { Table = "PVIEW_MM_MATERIAL", TableID = "", Search = "", Value = "", SortField = "MM_MATERIAL_H_ID", Direction = "0", FrmRowno = "1", ToRowno = "500000", Deleted = "0" };
            List<COAMMMaterialModel> MaterialList = await common.PSP_COMMON_DAPPER<COAMMMaterialModel>("PSP_COMMON_LIST", CommandType.StoredProcedure, param) ?? new List<COAMMMaterialModel>();
            List<OraMaterialList> OraList = await dbdal.getMaterialListFrORAICCP();

            OraList = OraList.Where(ora =>
                !MaterialList.Any(mat => mat.MATERIAL_CODE == ora.MATERIAL_CODE && mat.TPM_CODE == ora.TPM_CODE 
                    && mat.MATERIAL_ABBR == ora.MATERIAL_ABBR && mat.MATERIAL_NAME == ora.MATERIAL_NAME)
            ).ToList();

            List<SelectListItem> SuppList = await LoadDllData(0, "", "SUPPLIER");
            List<SelectListItem> UOMLIST = await LoadDllData(0, "", "UNIT");

            ViewBag.SerializedOraMaterial = JsonConvert.SerializeObject(OraList);
            ViewBag.SerializedSupplierName = JsonConvert.SerializeObject(SuppList);
            ViewBag.SerializedUOM = JsonConvert.SerializeObject(UOMLIST);
            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> COA_MM_MATERIAL_ADD(string ActionType, COAMMMaterialModel model)
        {
            try
            {
                ViewBag.Tittle = "Raw Material Specification";
                //model.DropdownProdGroup = await LoadDllData(0, "", "PROD_GROUP");

                if (ActionType == "postback")
                {
                    ModelState.Clear();
                }
                if (ActionType == "Back")
                {
                    ModelState.Clear();
                    return RedirectToAction("COA_MM_MATERIAL_LST", "COA_MM_MATERIAL");
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
        public async Task<ActionResult> deleteCOAMaterial(List<string> lstid)
        {
            bool success = true;
            string message = "";

            COAMMMaterialModel modelh = new COAMMMaterialModel();

            if (lstid.Count < 1)
            {
                success = false;
                message = "Please select a record first to delete.";
            }

            if (success && lstid.Count > 0)
            {
                foreach (var item in lstid)
                {
                    modelh.MM_MATERIAL_H_ID = Convert.ToInt32(item);
                    modelh.RECORD_TYP = "5";
                    modelh.MATERIAL_NAME = "";
                    modelh.MATERIAL_ABBR = "";
                    modelh.MATERIAL_CODE = "";
                    modelh.TPM_CODE = "";

                    //delete h
                    string result1 = await dbdal.COAMM_MATERIAL_H_Maint(modelh);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                }
            }

            var data = new { success = success, message = message };

            return Json(data);
        }

        #endregion

        #region AUDIT

        [SessionExpire]
        public async Task<ActionResult> COA_MM_MATERIAL_AUDIT(string id)
        {
            AuditTrailModels AuditTrailModels = new AuditTrailModels();
            List<SqlParameter> _pMssql = new List<SqlParameter>();
            _pMssql.Add(new SqlParameter("@TABLE", "PVIEW_MM_MATERIAL_A"));
            _pMssql.Add(new SqlParameter("@KEY_VALUE", id));
            _pMssql.Add(new SqlParameter("@SortColumn", "UPDATED_DATE"));
            _pMssql.Add(new SqlParameter("@SortType", "DESC"));

            string dbname = "";
            string isTest = ConfigurationManager.AppSettings["isTest"];

            if (string.Equals(isTest, "TRUE", StringComparison.OrdinalIgnoreCase))
            {
                dbname = ConfigurationManager.AppSettings["DEV"];
            }
            else
            {
                dbname = ConfigurationManager.AppSettings["LIVE"];
            }

            AuditTrailModels = await AuditTrailHelper.AuditTrailStoreProcedureSqlAsync("PSP_GET_AUDIT_TRAIL", CommandType.StoredProcedure, _pMssql, dbname);


            ViewBag.JsonResult = AuditTrailModels.JsonData;
            ViewBag.KeyNames = AuditTrailModels.ListData;
            ViewBag.hid = id;

            return View();
            //TempData["SQ_ID"] = id;
            //CommonFunction common = new CommonFunction();
            //var param = new { pTable = "PVIEW_COA_MM_MATERIAL_A", pKeyValue = id };
            //List<AuditTrailModel> AuditTrailModel = await common.PSP_COMMON_DAPPER<AuditTrailModel>("PSP_COMMON_AUDIT", CommandType.StoredProcedure, param) ?? new List<AuditTrailModel>();

            //return View(AuditTrailModel);
        }

        #endregion

        #region DDL

        private async Task<List<SelectListItem>> LoadDllData(int ID, string act, string category)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();

            DataTable dt = await dbdal.getDllData(ID, act, category);
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