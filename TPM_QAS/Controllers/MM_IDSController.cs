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
using DocumentFormat.OpenXml.Office2010.Excel;

namespace TPM_QAS.Controllers
{
    public class MM_IDSController : Controller
    {
        DB dbmain = new DB();
        MM_IDS_DAL dbdal = new MM_IDS_DAL();

        #region LIST

        [SessionExpire]
        public async Task<ActionResult> MM_IDS_LST(string Deleted = "0")
        {
            TempData["STATUS_DDL"] = await dbmain.PSP_COMMON_DDL("CMN_LST_STATUS", "", "", "", "", "", "");

            var model = new List<IDSModel>();
            List<IDSModel> listItemsAdd = new List<IDSModel>();

            DataTable dtl = await dbdal.getIDS_MainLst_Data(Deleted); //get d data
            if (dtl != null && dtl.Rows.Count > 0)
            {
                for (int i = 0; i < dtl.Rows.Count; i++)
                {
                    IDSModel infoObjAdd = new IDSModel();

                    infoObjAdd.MM_IDS_MAIN_LST_ID = Convert.ToInt32(dtl.Rows[i]["MM_IDS_MAIN_LST_ID"]);
                    infoObjAdd.PROD_GROUP = dtl.Rows[i]["PROD_GROUP"] != DBNull.Value ? dtl.Rows[i]["PROD_GROUP"].ToString() : "";
                    infoObjAdd.COMP_GROUP = dtl.Rows[i]["COMP_GROUP"] != DBNull.Value ? dtl.Rows[i]["COMP_GROUP"].ToString() : "";
                    infoObjAdd.SAMPLING_TYPE_DESC = dtl.Rows[i]["SAMPLING_TYPE_DESC"] != DBNull.Value ? dtl.Rows[i]["SAMPLING_TYPE_DESC"].ToString() : "";
                    infoObjAdd.PROD_LINE = dtl.Rows[i]["PROD_LINE"] != DBNull.Value ? dtl.Rows[i]["PROD_LINE"].ToString() : "";

                    infoObjAdd.RECORD_TYP = dtl.Rows[i]["RECORD_TYP"] != DBNull.Value ? dtl.Rows[i]["RECORD_TYP"].ToString() : "";
                    infoObjAdd.REC_TYPE_DESC = dtl.Rows[i]["REC_TYPE_DESC"] != DBNull.Value ? dtl.Rows[i]["REC_TYPE_DESC"].ToString() : "";
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
            model = listItemsAdd;


            ViewBag.Deleted = Deleted;
            return View(model);
        }

        [SessionExpire]
        public async Task<ActionResult> OLD_MM_IDS_LST(string Deleted = "0")
        {
            TempData["STATUS_DDL"] = await dbmain.PSP_COMMON_DDL("CMN_LST_STATUS", "", "", "", "", "", "");

            CommonFunction common = new CommonFunction();
            var param = new { Table = "PVIEW_MM_IDS_IDSSEC", TableID = "", Search = "", Value = "", SortField = "LEFT(idh, 2), idh_numeric", Direction = "0", FrmRowno = "1", ToRowno = "500000", Deleted = Deleted };
            List<IDSModel> model = await common.PSP_COMMON_DAPPER<IDSModel>("PSP_COMMON_LIST", CommandType.StoredProcedure, param) ?? new List<IDSModel>();

            ViewBag.Deleted = Deleted;
            return View(model);
        }

        [SessionExpire]
        public async Task<ActionResult> MM_IDS_LST_2(string id, string Deleted = "0")
        {
            TempData["STATUS_DDL"] = await dbmain.PSP_COMMON_DDL("CMN_LST_STATUS", "", "", "", "", "", "");

            var model = new List<IDSModel>();
            List<IDSModel> listItemsAdd = new List<IDSModel>();

            DataTable dtl = await dbdal.getIDS_MainLst_Sel(Convert.ToInt32(id), Deleted); //get d data
            if (dtl != null && dtl.Rows.Count > 0)
            {
                for (int i = 0; i < dtl.Rows.Count; i++)
                {
                    IDSModel infoObjAdd = new IDSModel();

                    infoObjAdd.IDH = dtl.Rows[i]["IDH"] != DBNull.Value ? dtl.Rows[i]["IDH"].ToString() : "";
                    infoObjAdd.PROD_GROUP = dtl.Rows[i]["PROD_GROUP"] != DBNull.Value ? dtl.Rows[i]["PROD_GROUP"].ToString() : "";
                    infoObjAdd.COMP_GROUP = dtl.Rows[i]["COMP_GROUP"] != DBNull.Value ? dtl.Rows[i]["COMP_GROUP"].ToString() : "";
                    infoObjAdd.PROPERTIES = dtl.Rows[i]["PROPERTIES"] != DBNull.Value ? dtl.Rows[i]["PROPERTIES"].ToString() : "";
                    infoObjAdd.PROP_ITEM = dtl.Rows[i]["PROP_ITEM"] != DBNull.Value ? dtl.Rows[i]["PROP_ITEM"].ToString() : "";
                    infoObjAdd.SAMPLING_TYPE = dtl.Rows[i]["SAMPLING_TYPE"] != DBNull.Value ? dtl.Rows[i]["SAMPLING_TYPE"].ToString() : ""; 
                    infoObjAdd.SAMPLING_TYPE_DESC = dtl.Rows[i]["SAMPLING_TYPE_DESC"] != DBNull.Value ? dtl.Rows[i]["SAMPLING_TYPE_DESC"].ToString() : "";
                    infoObjAdd.INDICATOR = dtl.Rows[i]["INDICATOR"] != DBNull.Value ? dtl.Rows[i]["INDICATOR"].ToString() : ""; 
                    infoObjAdd.PROD_LINE = dtl.Rows[i]["PROD_LINE"] != DBNull.Value ? dtl.Rows[i]["PROD_LINE"].ToString() : "";
                    infoObjAdd.RECORD_TYP = dtl.Rows[i]["RECORD_TYP"] != DBNull.Value ? dtl.Rows[i]["RECORD_TYP"].ToString() : "";
                    infoObjAdd.REC_TYPE_DESC = dtl.Rows[i]["REC_TYPE_DESC"] != DBNull.Value ? dtl.Rows[i]["REC_TYPE_DESC"].ToString() : "";
                    infoObjAdd.IDH_NUMERIC = Convert.ToInt32(dtl.Rows[i]["IDH_NUMERIC"]);

                    // Adding.
                    listItemsAdd.Add(infoObjAdd);

                }
            }
            model = listItemsAdd;
            ViewBag.Deleted = Deleted;
            return View(model);
        }

        #endregion

        #region VIEW/EDIT

        [SessionExpire]
        public async Task<ActionResult> MM_IDS_DETAIL(string id)
        {
            ViewBag.Tittle = "IDS";
            var model = new IDSModel();

            model.IDSMixModel = new IDSMixModel();
            model.IDSMixModel.IDSMixLstModel = new List<IDSMixLstModel>();

            model.IDSDirectModel = new IDSDirectModel();
            model.IDSDirectModel.IDSDirectLstModel = new List<IDSDirectLstModel>();

            model.DropdownProdGroup = await LoadDllData(0, "", "PROD_GROUP");
            model.DropdownProperty = await LoadDllData(0, "", "PROPERTIES");
            model.DropdownPropItem = new List<SelectListItem>();
            model.DropdownProdLine = await LoadDllData(0, "", "PROD_LINE");
            model.DropdownUnit = await LoadDllData(0, "", "UNIT");
            model.DropdownIndicator = await LoadDllData(0, "", "INDICATOR");
            model.DropdownSectionName = await LoadDllData(0, "", "SECTION");

            if (!string.IsNullOrEmpty(id) && char.IsLetter(id[0]))
            {
                string type = id[0].ToString();
                string idh = id.Substring(2);

                model.IDH = idh;

                if (type.ToUpper() == "M") 
                {
                    model.SAMPLING_TYPE = "MIX";

                    if (!string.IsNullOrWhiteSpace(idh) && CommonMethod.isNumeric(idh))
                    {
                        List<IDSMixLstModel> listItemsAdd = new List<IDSMixLstModel>();
                        DataTable dt = await dbdal.getIDS_Mix_Data(idh, "H"); //get header data

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            model.COMP_GROUP = dt.Rows[0]["COMP_GROUP"] != DBNull.Value ? dt.Rows[0]["COMP_GROUP"].ToString() : "";
                            model.IDSMixModel.COMP_GROUP = model.COMP_GROUP;
                            model.IDSMixModel.MM_IDS_H_ID = Convert.ToInt32(dt.Rows[0]["MM_IDS_H_ID"]);
                            model.IDSMixModel.SAMPLING_TYPE = dt.Rows[0]["SAMPLING_TYPE"] != DBNull.Value ? dt.Rows[0]["SAMPLING_TYPE"].ToString() : "";
                            model.IDSMixModel.MM_PRODGROUP_ID_H = Convert.ToInt32(dt.Rows[0]["MM_PRODGROUP_ID_H"]);
                            model.IDSMixModel.PROD_GROUP = dt.Rows[0]["PROD_GROUP"] != DBNull.Value ? dt.Rows[0]["PROD_GROUP"].ToString() : "";
                            model.IDSMixModel.PROPERTIES = dt.Rows[0]["PROPERTIES"] != DBNull.Value ? dt.Rows[0]["PROPERTIES"].ToString() : "";
                            model.IDSMixModel.MM_PROPERTIES_H_ID = Convert.ToInt32(dt.Rows[0]["MM_PROPERTIES_H_ID"]);
                            model.IDSMixModel.PROP_ITEM = dt.Rows[0]["PROP_ITEM"] != DBNull.Value ? dt.Rows[0]["PROP_ITEM"].ToString() : "";
                            model.IDSMixModel.PROD_LINE = dt.Rows[0]["PROD_LINE"] != DBNull.Value ? dt.Rows[0]["PROD_LINE"].ToString() : "";
                            model.IDSMixModel.UNIT = dt.Rows[0]["UNIT"] != DBNull.Value ? dt.Rows[0]["UNIT"].ToString() : "";
                            model.IDSMixModel.INDICATOR = dt.Rows[0]["INDICATOR"] != DBNull.Value ? dt.Rows[0]["INDICATOR"].ToString() : "";

                            model.IDSMixModel.SHRINK_DOWN = dt.Rows[0]["SHRINK_DOWN"] != DBNull.Value ? dt.Rows[0]["SHRINK_DOWN"].ToString() : "";
                            model.IDSMixModel.READING_TYPE = dt.Rows[0]["READING_TYPE"] != DBNull.Value ? dt.Rows[0]["READING_TYPE"].ToString() : "";
                            model.IDSMixModel.READING_VALUE = Convert.ToInt32(dt.Rows[0]["READING_VALUE"]);
                            model.IDSMixModel.REPEAT_VALUE = Convert.ToInt32(dt.Rows[0]["REPEAT_VALUE"]);
                            model.IDSMixModel.HORIZONTAL = Convert.ToDecimal(dt.Rows[0]["HORIZONTAL"]);
                            model.IDSMixModel.VERTICAL = Convert.ToDecimal(dt.Rows[0]["VERTICAL"]);
                            model.IDSMixModel.SEGREGATION = dt.Rows[0]["SEGREGATION"] != DBNull.Value ? dt.Rows[0]["SEGREGATION"].ToString() : "";
                            model.IDSMixModel.SEGREGATION_DESC = dt.Rows[0]["SEGREGATION_DESC"] != DBNull.Value ? dt.Rows[0]["SEGREGATION_DESC"].ToString() : "";
                            model.IDSMixModel.BEFORESEGSET = Convert.ToInt32(dt.Rows[0]["BEFORESEGSET"]);
                            model.IDSMixModel.AFTERSEGSET = Convert.ToInt32(dt.Rows[0]["AFTERSEGSET"]);
                            model.IDSMixModel.COQ = dt.Rows[0]["COQ"] != DBNull.Value ? dt.Rows[0]["COQ"].ToString() : "";
                            model.IDSMixModel.COQ_DESC = dt.Rows[0]["COQ_DESC"] != DBNull.Value ? dt.Rows[0]["COQ_DESC"].ToString() : "";
                            model.IDSMixModel.AFTERCOQSET = Convert.ToInt32(dt.Rows[0]["AFTERCOQSET"]);

                            model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                            model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                            model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                            model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                            model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                            model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                            model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";

                            model.DropdownPropItem = await LoadDllData(0, model.IDSMixModel.PROPERTIES, "PROP_ITEM");

                            DataTable dtl = await dbdal.getIDS_Mix_Data(idh, "D"); //get d data
                            if (dtl != null && dtl.Rows.Count > 0)
                            {
                                for (int i = 0; i < dtl.Rows.Count; i++)
                                {
                                    IDSMixLstModel infoObjAdd = new IDSMixLstModel();

                                    infoObjAdd.MM_IDS_D_ID = Convert.ToInt32(dtl.Rows[i]["MM_IDS_D_ID"]);
                                    infoObjAdd.MM_IDS_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_IDS_H_ID"]);
                                    infoObjAdd.MACHINE_ID = Convert.ToInt32(dtl.Rows[i]["MACHINE_ID"]);
                                    infoObjAdd.MACHINE_NAME = dtl.Rows[i]["MACHINE_NAME"] != DBNull.Value ? dtl.Rows[i]["MACHINE_NAME"].ToString() : "";
                                    infoObjAdd.MACHINE_PATH = dtl.Rows[i]["MACHINE_PATH"] != DBNull.Value ? dtl.Rows[i]["MACHINE_PATH"].ToString() : "";

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
                            model.IDSMixModel.IDSMixLstModel = listItemsAdd;
                        }
                    }
                }
                else if(type == "D")
                {
                    model.SAMPLING_TYPE = "DIRECT";
                    if (!string.IsNullOrWhiteSpace(idh) && CommonMethod.isNumeric(idh))
                    {
                        List<IDSDirectLstModel> listItemsAdd = new List<IDSDirectLstModel>();

                        DataTable dt = await dbdal.getIDS_Direct_Data(idh, "H"); //get header data

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            model.COMP_GROUP = dt.Rows[0]["COMP_GROUP"] != DBNull.Value ? dt.Rows[0]["COMP_GROUP"].ToString() : "";
                            model.IDSDirectModel.COMP_GROUP = model.COMP_GROUP;

                            model.IDSDirectModel.MM_IDSSECTION_H_ID = Convert.ToInt32(dt.Rows[0]["MM_IDSSECTION_H_ID"]);
                            model.IDSDirectModel.SAMPLING_TYPE = dt.Rows[0]["SAMPLING_TYPE"] != DBNull.Value ? dt.Rows[0]["SAMPLING_TYPE"].ToString() : "";
                            model.IDSDirectModel.SECTION = dt.Rows[0]["SECTION"] != DBNull.Value ? dt.Rows[0]["SECTION"].ToString() : "";

                            model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                            model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                            model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                            model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                            model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                            model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                            model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";

                            DataTable dtl = await dbdal.getIDS_Direct_Data(idh, "D"); //get d data
                            if (dtl != null && dtl.Rows.Count > 0)
                            {
                                for (int i = 0; i < dtl.Rows.Count; i++)
                                {
                                    IDSDirectLstModel infoObjAdd = new IDSDirectLstModel();

                                    infoObjAdd.MM_IDSSECTION_D_ID = Convert.ToInt32(dtl.Rows[i]["MM_IDSSECTION_D_ID"]);
                                    infoObjAdd.MM_IDSSECTION_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_IDSSECTION_H_ID"]);
                                    infoObjAdd.FIELD_NAME = dtl.Rows[i]["FIELD_NAME"] != DBNull.Value ? dtl.Rows[i]["FIELD_NAME"].ToString() : "";
                                    infoObjAdd.PROPERTIES = dtl.Rows[i]["PROPERTIES"] != DBNull.Value ? dtl.Rows[i]["PROPERTIES"].ToString() : "";


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

                            model.IDSDirectModel.IDSDirectLstModel = listItemsAdd;
                        }
                    }
                }
                
            }
            

            ViewBag.FROM = "PRE";
            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MM_IDS_DETAIL(string ActionType, IDSModel model)
        {
            ViewBag.Tittle = "IDS";
            try
            {
                model.DropdownProdGroup = await LoadDllData(0, "", "PROD_GROUP");
                model.DropdownProperty = await LoadDllData(0, "", "PROPERTIES");
                model.DropdownPropItem = new List<SelectListItem>();
                model.DropdownProdLine = await LoadDllData(0, "", "PROD_LINE");
                model.DropdownUnit = await LoadDllData(0, "", "UNIT");
                model.DropdownIndicator = await LoadDllData(0, "", "INDICATOR");
                model.DropdownSectionName = await LoadDllData(0, "", "SECTION");


                if (model.SAMPLING_TYPE == "MIX")
                {
                    List<IDSMixLstModel> listItemsAdd = new List<IDSMixLstModel>();
                    DataTable dt = await dbdal.getIDS_Mix_Data(model.IDH.ToString(), "H"); //get header data

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        model.IDSMixModel.MM_IDS_H_ID = Convert.ToInt32(dt.Rows[0]["MM_IDS_H_ID"]);
                        model.IDSMixModel.SAMPLING_TYPE = dt.Rows[0]["SAMPLING_TYPE"] != DBNull.Value ? dt.Rows[0]["SAMPLING_TYPE"].ToString() : "";
                        model.IDSMixModel.MM_PRODGROUP_ID_H = Convert.ToInt32(dt.Rows[0]["MM_PRODGROUP_ID_H"]);
                        model.IDSMixModel.PROD_GROUP = dt.Rows[0]["PROD_GROUP"] != DBNull.Value ? dt.Rows[0]["PROD_GROUP"].ToString() : "";
                        //model.IDSMixModel.PROPERTIES = dt.Rows[0]["PROPERTIES"] != DBNull.Value ? dt.Rows[0]["PROPERTIES"].ToString() : "";
                        model.IDSMixModel.MM_PROPERTIES_H_ID = Convert.ToInt32(dt.Rows[0]["MM_PROPERTIES_H_ID"]);
                        model.IDSMixModel.PROP_ITEM = dt.Rows[0]["PROP_ITEM"] != DBNull.Value ? dt.Rows[0]["PROP_ITEM"].ToString() : "";
                        model.IDSMixModel.PROD_LINE = dt.Rows[0]["PROD_LINE"] != DBNull.Value ? dt.Rows[0]["PROD_LINE"].ToString() : "";
                        model.IDSMixModel.UNIT = dt.Rows[0]["UNIT"] != DBNull.Value ? dt.Rows[0]["UNIT"].ToString() : "";
                        model.IDSMixModel.INDICATOR = dt.Rows[0]["INDICATOR"] != DBNull.Value ? dt.Rows[0]["INDICATOR"].ToString() : "";

                        model.IDSMixModel.SHRINK_DOWN = dt.Rows[0]["SHRINK_DOWN"] != DBNull.Value ? dt.Rows[0]["SHRINK_DOWN"].ToString() : "";
                        model.IDSMixModel.READING_TYPE = dt.Rows[0]["READING_TYPE"] != DBNull.Value ? dt.Rows[0]["READING_TYPE"].ToString() : "";
                        model.IDSMixModel.READING_VALUE = Convert.ToInt32(dt.Rows[0]["READING_VALUE"]);
                        model.IDSMixModel.REPEAT_VALUE = Convert.ToInt32(dt.Rows[0]["REPEAT_VALUE"]);
                        model.IDSMixModel.HORIZONTAL = Convert.ToDecimal(dt.Rows[0]["HORIZONTAL"]);
                        model.IDSMixModel.VERTICAL = Convert.ToDecimal(dt.Rows[0]["VERTICAL"]);
                        model.IDSMixModel.SEGREGATION = dt.Rows[0]["SEGREGATION"] != DBNull.Value ? dt.Rows[0]["SEGREGATION"].ToString() : "";
                        model.IDSMixModel.SEGREGATION_DESC = dt.Rows[0]["SEGREGATION_DESC"] != DBNull.Value ? dt.Rows[0]["SEGREGATION_DESC"].ToString() : "";
                        model.IDSMixModel.BEFORESEGSET = Convert.ToInt32(dt.Rows[0]["BEFORESEGSET"]);
                        model.IDSMixModel.AFTERSEGSET = Convert.ToInt32(dt.Rows[0]["AFTERSEGSET"]);
                        model.IDSMixModel.COQ = dt.Rows[0]["COQ"] != DBNull.Value ? dt.Rows[0]["COQ"].ToString() : "";
                        model.IDSMixModel.COQ_DESC = dt.Rows[0]["COQ_DESC"] != DBNull.Value ? dt.Rows[0]["COQ_DESC"].ToString() : "";
                        model.IDSMixModel.AFTERCOQSET = Convert.ToInt32(dt.Rows[0]["AFTERCOQSET"]);

                        model.IDSMixModel.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                        model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                        model.IDSMixModel.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                        model.IDSMixModel.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                        model.IDSMixModel.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                        model.IDSMixModel.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                        model.IDSMixModel.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                        model.IDSMixModel.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";

                        model.DropdownPropItem = await LoadDllData(0, model.IDSMixModel.PROPERTIES, "PROP_ITEM");

                        DataTable dtl = await dbdal.getIDS_Mix_Data(model.IDSMixModel.MM_IDS_H_ID.ToString(), "D"); //get d data
                        if (dtl != null && dtl.Rows.Count > 0)
                        {
                            for (int i = 0; i < dtl.Rows.Count; i++)
                            {
                                IDSMixLstModel infoObjAdd = new IDSMixLstModel();

                                infoObjAdd.MM_IDS_D_ID = Convert.ToInt32(dtl.Rows[i]["MM_IDS_D_ID"]);
                                infoObjAdd.MM_IDS_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_IDS_H_ID"]);
                                infoObjAdd.MACHINE_ID = Convert.ToInt32(dtl.Rows[i]["MACHINE_ID"]);
                                infoObjAdd.MACHINE_NAME = dtl.Rows[i]["MACHINE_NAME"] != DBNull.Value ? dtl.Rows[i]["MACHINE_NAME"].ToString() : "";
                                infoObjAdd.MACHINE_PATH = dtl.Rows[i]["MACHINE_PATH"] != DBNull.Value ? dtl.Rows[i]["MACHINE_PATH"].ToString() : "";

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
                        model.IDSMixModel.IDSMixLstModel = listItemsAdd;
                    }
                }
                else if(model.SAMPLING_TYPE == "DIRECT")
                {
                    List<IDSDirectLstModel> listItemsAdd = new List<IDSDirectLstModel>();

                    DataTable dt = await dbdal.getIDS_Direct_Data(model.IDH.ToString(), "H"); //get header data

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        model.IDSDirectModel.MM_IDSSECTION_H_ID = Convert.ToInt32(dt.Rows[0]["MM_IDSSECTION_H_ID"]);
                        model.IDSDirectModel.SAMPLING_TYPE = dt.Rows[0]["SAMPLING_TYPE"] != DBNull.Value ? dt.Rows[0]["SAMPLING_TYPE"].ToString() : "";
                        model.IDSDirectModel.SECTION = dt.Rows[0]["SECTION"] != DBNull.Value ? dt.Rows[0]["SECTION"].ToString() : "";

                        model.IDSDirectModel.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                        model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                        model.IDSDirectModel.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                        model.IDSDirectModel.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                        model.IDSDirectModel.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                        model.IDSDirectModel.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                        model.IDSDirectModel.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                        model.IDSDirectModel.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";

                        DataTable dtl = await dbdal.getIDS_Direct_Data(model.IDSDirectModel.MM_IDSSECTION_H_ID.ToString(), "D"); //get d data
                        if (dtl != null && dtl.Rows.Count > 0)
                        {
                            for (int i = 0; i < dtl.Rows.Count; i++)
                            {
                                IDSDirectLstModel infoObjAdd = new IDSDirectLstModel();

                                infoObjAdd.MM_IDSSECTION_D_ID = Convert.ToInt32(dtl.Rows[i]["MM_IDSSECTION_D_ID"]);
                                infoObjAdd.MM_IDSSECTION_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_IDSSECTION_H_ID"]);
                                infoObjAdd.FIELD_NAME = dtl.Rows[i]["FIELD_NAME"] != DBNull.Value ? dtl.Rows[i]["FIELD_NAME"].ToString() : "";
                                infoObjAdd.PROPERTIES = dtl.Rows[i]["PROPERTIES"] != DBNull.Value ? dtl.Rows[i]["PROPERTIES"].ToString() : "";


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

                        model.IDSDirectModel.IDSDirectLstModel = listItemsAdd;
                    }
                }

                if (ActionType == "postback")
                {
                    ModelState.Clear();
                }
                if (ActionType == "Back")
                {
                    ModelState.Clear();
                    return RedirectToAction("MM_IDS_LST", "MM_IDS");
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
        public async Task<ActionResult> InsertUpdateIDS(IDSModel model)
        {
            bool success = true;
            string message = "";

            if(model.SAMPLING_TYPE == "MIX")
            {
                if (model.IDSMixModel.PROPERTIES == null || model.IDSMixModel.PROPERTIES == "" ||
                    model.IDSMixModel.INDICATOR == null || model.IDSMixModel.INDICATOR == "" ||
                    model.IDSMixModel.MM_PRODGROUP_ID_H == 0 
                    //model.IDSMixModel.IDSMixLstModel == null || model.IDSMixModel.IDSMixLstModel.Count < 1
                    )
                {
                    success = false;
                    message += "Required field cannot empty.";
                }

                if (success)
                {
                    string indicator = model.IDSMixModel.INDICATOR;

                    if (indicator == "Pass/Fail")
                    {
                        model.IDSMixModel.READING_TYPE = null;
                        model.IDSMixModel.READING_VALUE = 0;
                        model.IDSMixModel.SEGREGATION = null;
                        model.IDSMixModel.BEFORESEGSET = 0;
                        model.IDSMixModel.AFTERSEGSET = 0;
                        model.IDSMixModel.COQ = null;
                        model.IDSMixModel.AFTERCOQSET = 0;
                    }
                    else
                    {
                        string readtype = model.IDSMixModel.READING_TYPE;

                        if (readtype == "SINGLE")
                        {
                            model.IDSMixModel.READING_VALUE = 0;
                        }
                    }

                    // insert h
                    string result1 = await dbdal.IDS_Mix_H_Maint(model);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                    else
                    {
                        if(model.IDSMixModel.INDICATOR == "Machine Input")
                        {
                            //delete existing d first
                            string result3 = await dbdal.IDS_Mix_D_Maint(0, Convert.ToInt32(result1), 0, "2");
                            if (!(int.TryParse(result3, out int num3)))
                            {
                                success = false;
                                message += result3;
                            }
                            else
                            {
                                // insert d
                                foreach (var item in model.IDSMixModel.IDSMixLstModel)
                                {
                                    int machineID = Convert.ToInt32(item.MACHINE_ID);
                                    string machinename = item.MACHINE_NAME != null ? item.MACHINE_NAME.ToString() : "";
                                    string machinepath = item.MACHINE_PATH != null ? item.MACHINE_PATH.ToString() : "";

                                    string rectype = item.RECORD_TYP != null ? item.RECORD_TYP.ToString() : "";

                                    if (rectype == "0")
                                    {
                                        rectype = "3";
                                    }

                                    string result2 = await dbdal.IDS_Mix_D_Maint(item.MM_IDS_D_ID, Convert.ToInt32(result1), machineID, rectype);
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
                            //delete existing d first
                            string result3 = await dbdal.IDS_Mix_D_Maint(0, Convert.ToInt32(result1), 0, "2");
                            if (!(int.TryParse(result3, out int num3)))
                            {
                                success = false;
                                message += result3;
                            }
                        }
                        
                    }
                }
               
            }
            else if(model.SAMPLING_TYPE == "DIRECT")
            {
                if (model.IDSDirectModel.SECTION == null || model.IDSDirectModel.SECTION == "" ||
                    model.IDSDirectModel.IDSDirectLstModel == null || model.IDSDirectModel.IDSDirectLstModel.Count < 1)
                {
                    success = false;
                    message += "Required field cannot empty.";
                }

                if (success)
                {
                    // insert h
                    string result1 = await dbdal.IDS_Direct_H_Maint(model);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                    else
                    {
                        //delete existing d first
                        string result3 = await dbdal.IDS_Direct_D_Maint(0, Convert.ToInt32(result1), "", "", "2");
                        if (!(int.TryParse(result3, out int num3)))
                        {
                            success = false;
                            message += result3;
                        }
                        else
                        {
                            // insert d
                            foreach (var item in model.IDSDirectModel.IDSDirectLstModel)
                            {
                                string fieldname = item.FIELD_NAME != null ? item.FIELD_NAME.ToString() : "";
                                string properties = item.PROPERTIES != null ? item.PROPERTIES.ToString() : "";

                                string rectype = item.RECORD_TYP != null ? item.RECORD_TYP.ToString() : "";

                                if (rectype == "0")
                                {
                                    rectype = "3";
                                }

                                string result2 = await dbdal.IDS_Direct_D_Maint(item.MM_IDSSECTION_D_ID, Convert.ToInt32(result1), fieldname, properties, rectype);
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
                message += "Error in saving data : Invalid sampling type.";
            }

            var data = new { success = success, message = message };

            return Json(data);
        }

        #endregion

        #region ADD

        [SessionExpire]
        public async Task<ActionResult> MM_IDS_ADD(string id = "")
        {
            ViewBag.Tittle = "IDS";
            var model = new IDSModel();

            model.IDSMixModel = new IDSMixModel();
            model.IDSMixModel.IDSMixLstModel = new List<IDSMixLstModel>();

            model.IDSDirectModel = new IDSDirectModel();
            model.IDSDirectModel.IDSDirectLstModel = new List<IDSDirectLstModel>();

            CommonFunction common = new CommonFunction();
            DataTable dtp = await common.PSP_COMMON_SQL("SELECT SUB_CATEGORY, ITEMS FROM MM_COMMONGROUP_H A LEFT JOIN MM_COMMONGROUP_D B ON A.MM_COMMONGROUP_H_ID = B.MM_COMMONGROUP_H_ID  AND B.RECORD_TYP NOT IN (5,8) WHERE A.RECORD_TYP NOT IN (5,8) AND UPPER(A.CATEGORY) = 'IDS PASS'");

            if (dtp != null && dtp.Rows.Count > 0)
            {
                foreach (DataRow row in dtp.Rows)
                {
                    string subCategory = row["SUB_CATEGORY"].ToString().ToUpper();
                    decimal items = Convert.ToDecimal(row["ITEMS"]);

                    switch (subCategory)
                    {
                        case "HORIZONTAL":
                            model.IDSMixModel.HORIZONTAL = items;
                            break;

                        case "VERTICAL":
                            model.IDSMixModel.VERTICAL = items;
                            break;
                    }
                }
            }

            model.DropdownProdGroup = await LoadDllData(0, "", "PROD_GROUP");
            model.DropdownProperty = await LoadDllData(0, "", "PROPERTIES");
            model.DropdownPropItem = new List<SelectListItem>();
            model.DropdownProdLine = await LoadDllData(0, "", "PROD_LINE");
            model.DropdownUnit = await LoadDllData(0, "", "UNIT");
            model.DropdownIndicator = await LoadDllData(0, "", "INDICATOR");
            model.DropdownSectionName = await LoadDllData(0, "", "SECTION");

            if (!string.IsNullOrEmpty(id) && char.IsLetter(id[0]))
            {
                string type = id[0].ToString();
                string idh = id.Substring(2);

                model.IDH = idh;

                if (type == "M")
                {
                    model.SAMPLING_TYPE = "MIX";
                    if (!string.IsNullOrWhiteSpace(idh) && CommonMethod.isNumeric(idh))
                    {
                        List<IDSMixLstModel> listItemsAdd = new List<IDSMixLstModel>();
                        DataTable dt = await dbdal.getIDS_Mix_Data(idh, "H"); //get header data

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            model.IDSMixModel.MM_IDS_H_ID = Convert.ToInt32(dt.Rows[0]["MM_IDS_H_ID"]);
                            model.IDSMixModel.SAMPLING_TYPE = dt.Rows[0]["SAMPLING_TYPE"] != DBNull.Value ? dt.Rows[0]["SAMPLING_TYPE"].ToString() : "";
                            model.IDSMixModel.MM_PRODGROUP_ID_H = Convert.ToInt32(dt.Rows[0]["MM_PRODGROUP_ID_H"]);
                            model.IDSMixModel.PROD_GROUP = dt.Rows[0]["PROD_GROUP"] != DBNull.Value ? dt.Rows[0]["PROD_GROUP"].ToString() : "";
                            model.IDSMixModel.PROPERTIES = dt.Rows[0]["PROPERTIES"] != DBNull.Value ? dt.Rows[0]["PROPERTIES"].ToString() : "";
                            model.IDSMixModel.MM_PROPERTIES_H_ID = Convert.ToInt32(dt.Rows[0]["MM_PROPERTIES_H_ID"]);
                            model.IDSMixModel.PROP_ITEM = dt.Rows[0]["PROP_ITEM"] != DBNull.Value ? dt.Rows[0]["PROP_ITEM"].ToString() : "";
                            model.IDSMixModel.PROD_LINE = dt.Rows[0]["PROD_LINE"] != DBNull.Value ? dt.Rows[0]["PROD_LINE"].ToString() : "";
                            model.IDSMixModel.UNIT = dt.Rows[0]["UNIT"] != DBNull.Value ? dt.Rows[0]["UNIT"].ToString() : "";
                            model.IDSMixModel.INDICATOR = dt.Rows[0]["INDICATOR"] != DBNull.Value ? dt.Rows[0]["INDICATOR"].ToString() : "";

                            model.IDSMixModel.SHRINK_DOWN = dt.Rows[0]["SHRINK_DOWN"] != DBNull.Value ? dt.Rows[0]["SHRINK_DOWN"].ToString() : "";
                            model.IDSMixModel.READING_TYPE = dt.Rows[0]["READING_TYPE"] != DBNull.Value ? dt.Rows[0]["READING_TYPE"].ToString() : "";
                            model.IDSMixModel.READING_VALUE = Convert.ToInt32(dt.Rows[0]["READING_VALUE"]);
                            model.IDSMixModel.REPEAT_VALUE = Convert.ToInt32(dt.Rows[0]["REPEAT_VALUE"]);
                            model.IDSMixModel.HORIZONTAL = Convert.ToDecimal(dt.Rows[0]["HORIZONTAL"]);
                            model.IDSMixModel.VERTICAL = Convert.ToDecimal(dt.Rows[0]["VERTICAL"]);
                            model.IDSMixModel.SEGREGATION = dt.Rows[0]["SEGREGATION"] != DBNull.Value ? dt.Rows[0]["SEGREGATION"].ToString() : "";
                            model.IDSMixModel.SEGREGATION_DESC = dt.Rows[0]["SEGREGATION_DESC"] != DBNull.Value ? dt.Rows[0]["SEGREGATION_DESC"].ToString() : "";
                            model.IDSMixModel.BEFORESEGSET = Convert.ToInt32(dt.Rows[0]["BEFORESEGSET"]);
                            model.IDSMixModel.AFTERSEGSET = Convert.ToInt32(dt.Rows[0]["AFTERSEGSET"]);
                            model.IDSMixModel.COQ = dt.Rows[0]["COQ"] != DBNull.Value ? dt.Rows[0]["COQ"].ToString() : "";
                            model.IDSMixModel.COQ_DESC = dt.Rows[0]["COQ_DESC"] != DBNull.Value ? dt.Rows[0]["COQ_DESC"].ToString() : "";
                            model.IDSMixModel.AFTERCOQSET = Convert.ToInt32(dt.Rows[0]["AFTERCOQSET"]);

                            model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                            model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                            model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                            model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                            model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                            model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                            model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";

                            model.DropdownPropItem = await LoadDllData(0, model.IDSMixModel.PROPERTIES, "PROP_ITEM");

                            DataTable dtl = await dbdal.getIDS_Mix_Data(idh, "D"); //get d data
                            if (dtl != null && dtl.Rows.Count > 0)
                            {
                                for (int i = 0; i < dtl.Rows.Count; i++)
                                {
                                    IDSMixLstModel infoObjAdd = new IDSMixLstModel();

                                    infoObjAdd.MM_IDS_D_ID = Convert.ToInt32(dtl.Rows[i]["MM_IDS_D_ID"]);
                                    infoObjAdd.MM_IDS_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_IDS_H_ID"]);
                                    infoObjAdd.MACHINE_ID = Convert.ToInt32(dtl.Rows[i]["MACHINE_ID"]);
                                    infoObjAdd.MACHINE_NAME = dtl.Rows[i]["MACHINE_NAME"] != DBNull.Value ? dtl.Rows[i]["MACHINE_NAME"].ToString() : "";
                                    infoObjAdd.MACHINE_PATH = dtl.Rows[i]["MACHINE_PATH"] != DBNull.Value ? dtl.Rows[i]["MACHINE_PATH"].ToString() : "";

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
                            model.IDSMixModel.IDSMixLstModel = listItemsAdd;
                        }
                    }
                }
                else if(type == "D")
                {
                    model.SAMPLING_TYPE = "DIRECT";
                    if (!string.IsNullOrWhiteSpace(idh) && CommonMethod.isNumeric(idh))
                    {
                        List<IDSDirectLstModel> listItemsAdd = new List<IDSDirectLstModel>();

                        DataTable dt = await dbdal.getIDS_Direct_Data(idh, "H"); //get header data

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            model.IDSDirectModel.MM_IDSSECTION_H_ID = Convert.ToInt32(dt.Rows[0]["MM_IDSSECTION_H_ID"]);
                            model.IDSDirectModel.SAMPLING_TYPE = dt.Rows[0]["SAMPLING_TYPE"] != DBNull.Value ? dt.Rows[0]["SAMPLING_TYPE"].ToString() : "";
                            model.IDSDirectModel.SECTION = dt.Rows[0]["SECTION"] != DBNull.Value ? dt.Rows[0]["SECTION"].ToString() : "";

                            model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                            model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                            model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                            model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                            model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                            model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                            model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";

                            DataTable dtl = await dbdal.getIDS_Direct_Data(idh, "D"); //get d data
                            if (dtl != null && dtl.Rows.Count > 0)
                            {
                                for (int i = 0; i < dtl.Rows.Count; i++)
                                {
                                    IDSDirectLstModel infoObjAdd = new IDSDirectLstModel();

                                    infoObjAdd.MM_IDSSECTION_D_ID = Convert.ToInt32(dtl.Rows[i]["MM_IDSSECTION_D_ID"]);
                                    infoObjAdd.MM_IDSSECTION_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_IDSSECTION_H_ID"]);
                                    infoObjAdd.FIELD_NAME = dtl.Rows[i]["FIELD_NAME"] != DBNull.Value ? dtl.Rows[i]["FIELD_NAME"].ToString() : "";
                                    infoObjAdd.PROPERTIES = dtl.Rows[i]["PROPERTIES"] != DBNull.Value ? dtl.Rows[i]["PROPERTIES"].ToString() : "";


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

                            model.IDSDirectModel.IDSDirectLstModel = listItemsAdd;
                        }
                    }
                }
            }

            
            ViewBag.FROM = "PRE";
            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MM_IDS_ADD(string ActionType, IDSModel model)
        {
            ViewBag.Tittle = "IDS";
            try
            {
                CommonFunction common = new CommonFunction();
                DataTable dt = await common.PSP_COMMON_SQL("SELECT SUB_CATEGORY, ITEMS FROM MM_COMMONGROUP_H A LEFT JOIN MM_COMMONGROUP_D B ON A.MM_COMMONGROUP_H_ID = B.MM_COMMONGROUP_H_ID  AND B.RECORD_TYP NOT IN (5,8) WHERE A.RECORD_TYP NOT IN (5,8) AND UPPER(A.CATEGORY) = 'IDS PASS'");

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string subCategory = row["SUB_CATEGORY"].ToString().ToUpper();
                        decimal items = Convert.ToDecimal(row["ITEMS"]);

                        switch (subCategory)
                        {
                            case "HORIZONTAL":
                                model.IDSMixModel.HORIZONTAL = items;
                                break;

                            case "VERTICAL":
                                model.IDSMixModel.VERTICAL = items;
                                break;
                        }
                    }
                }

                model.DropdownProdGroup = await LoadDllData(0, "", "PROD_GROUP");
                model.DropdownProperty = await LoadDllData(0, "", "PROPERTIES");
                model.DropdownProdLine = await LoadDllData(0, "", "PROD_LINE");
                model.DropdownUnit = await LoadDllData(0, "", "UNIT");
                model.DropdownIndicator = await LoadDllData(0, "", "INDICATOR");
                model.DropdownSectionName = await LoadDllData(0, "", "SECTION");

                if (model.IDSMixModel.PROPERTIES != "" || model.IDSMixModel.PROPERTIES != null || !model.IDSMixModel.PROPERTIES.Contains("Select an option"))
                {
                    model.DropdownPropItem = await LoadDllData(0, model.IDSMixModel.PROPERTIES, "PROP_ITEM");
                }
                else
                {
                    model.DropdownPropItem = new List<SelectListItem>();
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
        public async Task<ActionResult> DraftIDS(IDSModel model)
        {
            bool success = true;
            string message = "";

            if(model.SAMPLING_TYPE == "MIX")
            {
                if (model.IDSMixModel.PROPERTIES == null || model.IDSMixModel.PROPERTIES == "" ||
                    model.IDSMixModel.INDICATOR == null || model.IDSMixModel.INDICATOR == "" ||
                    model.IDSMixModel.PROD_LINE == null || model.IDSMixModel.PROD_LINE == "" ||
                    model.IDSMixModel.MM_PRODGROUP_ID_H == 0)
                {
                    success = false;
                    message += "Required field cannot empty. ";
                }

                if (success)
                {
                    string indicator = model.IDSMixModel.INDICATOR;

                    if (indicator == "Machine Input" || indicator == "Pass/Fail")
                    {
                        model.IDSMixModel.SHRINK_DOWN = null;
                    }

                    if (indicator == "Pass/Fail")
                    {
                        model.IDSMixModel.READING_TYPE = null;
                        model.IDSMixModel.READING_VALUE = 0;
                        model.IDSMixModel.SEGREGATION = null;
                        model.IDSMixModel.BEFORESEGSET = 0;
                        model.IDSMixModel.AFTERSEGSET = 0;
                        model.IDSMixModel.COQ = null;
                        model.IDSMixModel.AFTERCOQSET = 0;
                    }
                    else
                    {
                        string readtype = model.IDSMixModel.READING_TYPE;

                        if (readtype == "SINGLE")
                        {
                            model.IDSMixModel.READING_VALUE = 0;
                        }
                    }

                    // insert h
                    string result1 = await dbdal.IDS_Mix_H_Maint(model);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                    else
                    {
                        if(model.IDSMixModel.INDICATOR == "Machine Input"){

                            if (model.IDSMixModel.IDSMixLstModel != null && model.IDSMixModel.IDSMixLstModel.Count > 0)
                            {
                                //delete existing d first
                                string result3 = await dbdal.IDS_Mix_D_Maint(0, Convert.ToInt32(result1), 0, "2");
                                if (!(int.TryParse(result3, out int num3)))
                                {
                                    success = false;
                                    message += result3;
                                }
                                else
                                {
                                    // insert d
                                    foreach (var item in model.IDSMixModel.IDSMixLstModel)
                                    {
                                        int machineID = Convert.ToInt32(item.MACHINE_ID);
                                        string machinename = item.MACHINE_NAME != null ? item.MACHINE_NAME.ToString() : "";
                                        string machinepath = item.MACHINE_PATH != null ? item.MACHINE_PATH.ToString() : "";

                                        string rectype = item.RECORD_TYP != null ? item.RECORD_TYP.ToString() : "";

                                        string result2 = await dbdal.IDS_Mix_D_Maint(item.MM_IDS_D_ID, Convert.ToInt32(result1), machineID, rectype);
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
                            //delete existing d first
                            string result3 = await dbdal.IDS_Mix_D_Maint(0, Convert.ToInt32(result1), 0, "2");
                            if (!(int.TryParse(result3, out int num3)))
                            {
                                success = false;
                                message += result3;
                            }
                        }
                        

                    }
                }
            }
            else if (model.SAMPLING_TYPE == "DIRECT") {

                if (model.IDSDirectModel.SECTION == null || model.IDSDirectModel.SECTION == "")
                {
                    success = false;
                    message += "Please select Section Name";
                }

                if (success)
                {
                    // insert h
                    string result1 = await dbdal.IDS_Direct_H_Maint(model);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                    else
                    {
                        if (model.IDSDirectModel.IDSDirectLstModel != null && model.IDSDirectModel.IDSDirectLstModel.Count > 0)
                        {
                            //delete existing d first
                            string result3 = await dbdal.IDS_Direct_D_Maint(0, Convert.ToInt32(result1), "", "", "2");
                            if (!(int.TryParse(result3, out int num3)))
                            {
                                success = false;
                                message += result3;
                            }
                            else
                            {
                                // insert d
                                foreach (var item in model.IDSDirectModel.IDSDirectLstModel)
                                {
                                    string fieldname = item.FIELD_NAME != null ? item.FIELD_NAME.ToString() : "";
                                    string properties = item.PROPERTIES != null ? item.PROPERTIES.ToString() : "";

                                    string rectype = item.RECORD_TYP != null ? item.RECORD_TYP.ToString() : "";

                                    string result2 = await dbdal.IDS_Direct_D_Maint(item.MM_IDSSECTION_D_ID, Convert.ToInt32(result1), fieldname, properties, rectype);
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
            }
            else
            {
                success = false;
                message += "Error in saving data : Invalid sampling type.";
            }

            

            var data = new { success = success, message = message };

            return Json(data);
        }

        #endregion

        #region POPUP

        [SessionExpire]
        public async Task<ActionResult> IDS_MACHINE_POPOUT(string id, string prop, string propitem)
        {
            if (TempData["Error"] != null) { ViewBag.Error = TempData["Error"].ToString(); }
            if (TempData["Result"] != null) { ViewBag.Result = TempData["Result"].ToString(); }

            var model = new IDSModel();

            if (id == null || id == "")
            {
                id = "0";
            }

            List<IDSMachinePopupModel> listItemsAdd = new List<IDSMachinePopupModel>();

            if (prop != null && prop!="" && propitem != null && propitem != "")
            {
                DataTable dtmachine = await dbdal.getMachine_Data(Convert.ToInt32(id), prop, propitem); 
                
                if (dtmachine != null && dtmachine.Rows.Count > 0)
                {
                    for (int i = 0; i < dtmachine.Rows.Count; i++)
                    {
                        IDSMachinePopupModel infoObjAdd = new IDSMachinePopupModel();

                        infoObjAdd.MACHINE_ID = Convert.ToInt32(dtmachine.Rows[i]["MACHINE_ID"]);
                        infoObjAdd.MACHINE_NAME = dtmachine.Rows[i]["MACHINE_NAME"].ToString();
                        infoObjAdd.MACHINE_FIELD = dtmachine.Rows[i]["MACHINE_FIELD"].ToString();
                        infoObjAdd.PROPERTIES = dtmachine.Rows[i]["PROPERTIES"].ToString();
                        infoObjAdd.PROP_ITEM = dtmachine.Rows[i]["PROP_ITEM"].ToString();

                        infoObjAdd.IS_EXIST = "N";

                        // Adding.
                        listItemsAdd.Add(infoObjAdd);

                    }
                }
            }

            model.IDSMachinePopupModel = listItemsAdd;

            return View(model);
        }

        [SessionExpire]
        public async Task<ActionResult> IDS_PROPERTY_POPOUT(string id)
        {
            if (TempData["Error"] != null) { ViewBag.Error = TempData["Error"].ToString(); }
            if (TempData["Result"] != null) { ViewBag.Result = TempData["Result"].ToString(); }

            var model = new IDSModel();

            CommonFunction common = new CommonFunction();
            List<IDSPropertiesPopupModel> IDSPropertiesPopupModel = await common.PSP_COMMON_DAPPER<IDSPropertiesPopupModel>("SELECT DISTINCT PROPERTIES FROM MM_PROPERTIES_H A WHERE A.RECORD_TYP <> 8 AND A.RECORD_TYP <> 5", CommandType.Text) ?? new List<IDSPropertiesPopupModel>();

            model.IDSPropertiesPopupModel = IDSPropertiesPopupModel;

            return View(model);
        }

        #endregion

        #region DELETE

        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> deleteIDS(List<string> lstid)
        {
            bool success = true;
            string message = "";

            IDSModel model = new IDSModel();
            model.IDSMixModel = new IDSMixModel();
            model.IDSDirectModel = new IDSDirectModel();

            if (lstid.Count < 1)
            {
                success = false;
                message += "Please select a record first to delete.";
            }

            if (success && lstid.Count > 0)
            {
                foreach (var item in lstid)
                {
                    if (!string.IsNullOrEmpty(item) && char.IsLetter(item[0]))
                    {
                        string type = item[0].ToString();
                        string id = item.Substring(2);

                        if(type == "M") 
                        {
                            model.IDSMixModel.MM_IDS_H_ID = Convert.ToInt32(id);
                            model.IDSMixModel.RECORD_TYP = "5";

                            model.IDSMixModel.SAMPLING_TYPE = "";
                            model.IDSMixModel.MM_PRODGROUP_ID_H = 0;
                            model.IDSMixModel.MM_PROPERTIES_H_ID = 0;
                            model.IDSMixModel.PROD_LINE = "";
                            model.IDSMixModel.UNIT = "";
                            model.IDSMixModel.INDICATOR = "";
                            
                            model.IDSMixModel.REPEAT_VALUE = 0;
                            model.IDSMixModel.HORIZONTAL = 0;
                            model.IDSMixModel.VERTICAL = 0;                         

                            string resulth = await dbdal.IDS_Mix_H_Maint(model);
                            if (!(int.TryParse(resulth, out int num1) && resulth != "0"))
                            {
                                success = false;
                                message += resulth;
                            }
                            else
                            {
                                string resultd = await dbdal.IDS_Mix_D_Maint(0, Convert.ToInt32(id), 0, "2");
                                if (!(int.TryParse(resultd, out int num2)))
                                {
                                    success = false;
                                    message += resultd;
                                }
                            }
                        }
                        else if(type == "D")
                        {
                            model.IDSDirectModel.MM_IDSSECTION_H_ID = Convert.ToInt32(id);
                            model.IDSDirectModel.RECORD_TYP = "5";

                            model.IDSDirectModel.SAMPLING_TYPE = "";
                            model.IDSDirectModel.SECTION = "";

                            string resulth = await dbdal.IDS_Direct_H_Maint(model);
                            if (!(int.TryParse(resulth, out int num1) && resulth != "0"))
                            {
                                success = false;
                                message += resulth;
                            }
                            else
                            {
                                string resultd = await dbdal.IDS_Direct_D_Maint(0, Convert.ToInt32(id), "", "", "2");
                                if (!(int.TryParse(resultd, out int num2)))
                                {
                                    success = false;
                                    message += resultd;
                                }
                            }
                        }
                        else
                        {
                            success = false;
                            message += "Invalid Sampling Type";
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
        public async Task<ActionResult> MM_IDS_AUDIT(string id)
        {
            string type = id[0].ToString();
            string idh = id.Substring(2);

            TempData["SQ_ID"] = idh;
            CommonFunction common = new CommonFunction();
            object param;

            if (type == "M")
            {
                param = new { pTable = "PVIEW_MM_IDS_A", pKeyValue = idh };
            }
            else
            {
                param = new { pTable = "PVIEW_MM_IDSSECTION_A", pKeyValue = idh };
            }

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