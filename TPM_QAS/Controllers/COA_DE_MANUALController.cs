using DBModel;
using DocumentFormat.OpenXml.Spreadsheet;
using iTextSharp.text.pdf;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using TPM_QAS.DAL;
using TPM_QAS.Filters;
using TPM_QAS.Helpers;
using TPM_QAS.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using DocumentFormat.OpenXml.Office2010.Excel;
using Quartz.Xml.JobSchedulingData20;

namespace TPM_QAS.Controllers
{
    public class COA_DE_MANUALController : Controller
    {
        DB dbmain = new DB();
        COA_DE_MANUAL_DAL dbdal = new COA_DE_MANUAL_DAL();
        COA_DE_OCR_DAL dbocr = new COA_DE_OCR_DAL();

        #region LIST

        [SessionExpire]
        public async Task<ActionResult> COA_DE_MANUAL_LST(string Deleted = "0")
        {
            ViewBag.Tittle = "COA Manual Data Entry";

            if (Deleted == "1")
            {
                TempData["STATUS_DDL"] = await dbmain.PSP_COMMON_DDL("CO_DE_OCR", "", "", "", "", "", "");
            }
            else
            {
                TempData["STATUS_DDL"] = await dbmain.PSP_COMMON_DDL("CO_DE_OCR2", "", "", "", "", "", "");
            }

            CommonFunction common = new CommonFunction();
            var param = new { Table = "PVIEW_DE_COA_MANUAL", TableID = "", Search = "", Value = "", SortField = "DE_OCR_H_ID", Direction = "0", FrmRowno = "1", ToRowno = "500000", Deleted = Deleted };
            List<COAManualModel> model = await common.PSP_COMMON_DAPPER<COAManualModel>("PSP_COMMON_LIST", CommandType.StoredProcedure, param) ?? new List<COAManualModel>();

            ViewBag.Deleted = Deleted;

            var aclUser = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            if (aclUser != null)
            {
                string emp_no = aclUser.EMP_NO?.ToString() ?? "";

                DataTable chkApproval = await dbocr.CheckUserAppRole(emp_no);
                if (chkApproval != null && chkApproval.Rows.Count > 0)
                {
                    ViewBag.isManager = "Y";
                }

                //for KS and Izzah
                if (aclUser.USER_ID?.ToString() == "800133" || aclUser.USER_ID?.ToString() == "800179")
                {
                    ViewBag.isManager = "Y";
                }
            }

            return View(model);
        }

        #endregion

        #region VIEW/EDIT

        [SessionExpire]
        
        public async Task<ActionResult> COA_DE_MANUAL_DETAIL(string id)
        {
            ViewBag.Tittle = "COA Manual Data Entry";
            var model = new COAOCRModel();

            model.DropdownSupplier = await LoadDllData(0, "", "COA_SUPPLIER");
            model.DropdownMaterial = new List<SelectListItem>();

            model.OCRLstModelDetail = new Dictionary<Tuple<string, int>, List<OCRLstModel>>();

            DataTable dt = await dbocr.getDEOCR_Data(id, "H"); //get header data

            if (dt != null && dt.Rows.Count > 0)
            {
                model.DE_OCR_H_ID = Convert.ToInt32(dt.Rows[0]["DE_OCR_H_ID"]);
                model.ENTRY_TYPE = dt.Rows[0]["ENTRY_TYPE"] != DBNull.Value ? dt.Rows[0]["ENTRY_TYPE"].ToString() : "";
                model.OCR_INDICATOR = dt.Rows[0]["OCR_INDICATOR"] != DBNull.Value ? dt.Rows[0]["OCR_INDICATOR"].ToString() : "";
                model.FILE_NAME = dt.Rows[0]["FILE_NAME"] != DBNull.Value ? dt.Rows[0]["FILE_NAME"].ToString() : "";
                model.FINAL_RESULT = dt.Rows[0]["FINAL_RESULT"] != DBNull.Value ? dt.Rows[0]["FINAL_RESULT"].ToString() : "";
                model.LOT_NO = dt.Rows[0]["LOT_LIST"] != DBNull.Value ? dt.Rows[0]["LOT_LIST"].ToString() : "";

                model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";
            }

            List<SupplierTab> dbselect = await dbocr.getsupplierlist(id, "S");
            foreach (var item in dbselect)
            {
                DataTable dtl = await dbocr.getDEOCR_Data(id, "D", item.MM_SUPPLIER_H_ID, item.LOT_NO); //get d data
                if (dtl != null && dtl.Rows.Count > 0)
                {
                    model.DropdownMaterial = await LoadDllData(0, dtl.Rows[0]["MM_SUPPLIER_H_ID"].ToString(), "COA_SUPP_MAT");
                    
                    model.MM_SUPPLIER_H_ID = Convert.ToInt32(dtl.Rows[0]["MM_SUPPLIER_H_ID"]);
                    model.MM_MATERIAL_H_ID = Convert.ToInt32(dtl.Rows[0]["MM_MATERIAL_H_ID"]);
                    model.TPM_CODE = dtl.Rows[0]["TPM_CODE"] != DBNull.Value ? dtl.Rows[0]["TPM_CODE"].ToString() : "";

                    List<OCRLstModel> listItemsAdd = new List<OCRLstModel>();
                    for (int i = 0; i < dtl.Rows.Count; i++)
                    {
                        OCRLstModel infoObjAdd = new OCRLstModel();

                        infoObjAdd.DE_OCR_D_ID = Convert.ToInt32(dtl.Rows[i]["DE_OCR_D_ID"]);
                        infoObjAdd.DE_OCR_H_ID = Convert.ToInt32(dtl.Rows[i]["DE_OCR_H_ID"]);
                        infoObjAdd.MM_MATERIAL_D2_ID = Convert.ToInt32(dtl.Rows[i]["MM_MATERIAL_D2_ID"]);
                        infoObjAdd.MM_MATERIAL_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_MATERIAL_H_ID"]);
                        infoObjAdd.MATERIAL_NAME = dtl.Rows[i]["MATERIAL_NAME"] != DBNull.Value ? dtl.Rows[i]["MATERIAL_NAME"].ToString() : "";
                        infoObjAdd.MM_SUPPLIER_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_SUPPLIER_H_ID"]);
                        infoObjAdd.SUPPLIER_NAME = dtl.Rows[i]["SUPPLIER_NAME"] != DBNull.Value ? dtl.Rows[i]["SUPPLIER_NAME"].ToString() : "";
                        infoObjAdd.FILE_NAME = dtl.Rows[i]["FILE_NAME"] != DBNull.Value ? dtl.Rows[i]["FILE_NAME"].ToString() : ""; 
                        infoObjAdd.LOT_NO = dtl.Rows[i]["LOT_NO"] != DBNull.Value ? dtl.Rows[i]["LOT_NO"].ToString() : "";
                        infoObjAdd.TPM_CODE = dtl.Rows[i]["TPM_CODE"] != DBNull.Value ? dtl.Rows[i]["TPM_CODE"].ToString() : "";
                        infoObjAdd.SUPPLIER_SPEC_TYPE = dtl.Rows[i]["SUPPLIER_SPEC_TYPE"] != DBNull.Value ? dtl.Rows[i]["SUPPLIER_SPEC_TYPE"].ToString() : "";
                        infoObjAdd.SUPPLIER_MATERIAL_NAME = dtl.Rows[i]["SUPPLIER_MATERIAL_NAME"] != DBNull.Value ? dtl.Rows[i]["SUPPLIER_MATERIAL_NAME"].ToString() : "";
                        infoObjAdd.SUPPLIER_SPEC_NAME = dtl.Rows[i]["SUPPLIER_SPEC_NAME"] != DBNull.Value ? dtl.Rows[i]["SUPPLIER_SPEC_NAME"].ToString() : "";
                        infoObjAdd.SUPPLIER_SPEC_VALUE = dtl.Rows[i]["SUPPLIER_SPEC_VALUE"] != DBNull.Value ? dtl.Rows[i]["SUPPLIER_SPEC_VALUE"].ToString() : "";
                        infoObjAdd.OCR_INSPECTION_ITEM = dtl.Rows[i]["OCR_INSPECTION_ITEM"] != DBNull.Value ? dtl.Rows[i]["OCR_INSPECTION_ITEM"].ToString() : "";
                        infoObjAdd.OCR_INSPECTION_ITEM_SCORE = dtl.Rows[i]["OCR_INSPECTION_ITEM_SCORE"] != DBNull.Value ? dtl.Rows[i]["OCR_INSPECTION_ITEM_SCORE"].ToString() : "";
                        infoObjAdd.OCR_RESULT = dtl.Rows[i]["OCR_RESULT"] != DBNull.Value ? dtl.Rows[i]["OCR_RESULT"].ToString() : "";
                        infoObjAdd.OCR_RESULT_SCORE = dtl.Rows[i]["OCR_RESULT_SCORE"] != DBNull.Value ? dtl.Rows[i]["OCR_RESULT_SCORE"].ToString() : "";
                        infoObjAdd.COMPARISON_RESULT = dtl.Rows[i]["COMPARISON_RESULT"] != DBNull.Value ? dtl.Rows[i]["COMPARISON_RESULT"].ToString() : "";

                        infoObjAdd.RECORD_TYP = dtl.Rows[i]["RECORD_TYP"] != DBNull.Value ? dtl.Rows[i]["RECORD_TYP"].ToString() : "";
                        infoObjAdd.CREATED_BY = dtl.Rows[i]["CREATED_BY"] != DBNull.Value ? dtl.Rows[i]["CREATED_BY"].ToString() : "";
                        infoObjAdd.CREATED_DATE = dtl.Rows[i]["CREATED_DATE"] != DBNull.Value ? dtl.Rows[i]["CREATED_DATE"].ToString() : "";
                        infoObjAdd.CREATED_LOC = dtl.Rows[i]["CREATED_LOC"] != DBNull.Value ? dtl.Rows[i]["CREATED_LOC"].ToString() : "";
                        infoObjAdd.UPDATED_BY = dtl.Rows[i]["UPDATED_BY"] != DBNull.Value ? dtl.Rows[i]["UPDATED_BY"].ToString() : "";
                        infoObjAdd.UPDATED_DATE = dtl.Rows[i]["UPDATED_DATE"] != DBNull.Value ? dtl.Rows[i]["UPDATED_DATE"].ToString() : "";
                        infoObjAdd.UPDATED_LOC = dtl.Rows[i]["UPDATED_LOC"] != DBNull.Value ? dtl.Rows[i]["UPDATED_LOC"].ToString() : "";

                        listItemsAdd.Add(infoObjAdd);
                    }

                    if (listItemsAdd != null && listItemsAdd.Count > 0)
                    {
                        model.OCRLstModelDetail.Add(
                            new Tuple<string, int>(item.LOT_NO, item.MM_SUPPLIER_H_ID),
                            listItemsAdd
                        );
                    }
                }
            }

            string emp_no = (HttpContext.Session.GetObject<ACL_UserObj>("AclUser")).EMP_NO.ToString();

            DataTable chkApproval = await dbocr.CheckUserAppRole(emp_no);
            if (chkApproval != null && chkApproval.Rows.Count > 0)
            {
                ViewBag.isManager = "Y";
            }

            //for KS and Izzah
            if ((HttpContext.Session.GetObject<ACL_UserObj>("AclUser")).USER_ID.ToString() == "800133" || (HttpContext.Session.GetObject<ACL_UserObj>("AclUser")).USER_ID.ToString() == "800179")
            {
                ViewBag.isManager = "Y";
            }

            return View(model);
        }


        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> COA_DE_MANUAL_DETAIL(string ActionType, COAManualModel model)
        {
            model.DropdownSupplier = await LoadDllData(0, "", "COA_SUPPLIER");
            model.DropdownMaterial = new List<SelectListItem>();

            ViewBag.Tittle = "COA Manual Data Entry";
            try
            {
                if (ActionType == "postback")
                {
                    ModelState.Clear();
                }
                if (ActionType == "Back")
                {
                    ModelState.Clear();
                    return RedirectToAction("COA_DE_MANUAL_LST", "COA_DE_MANUAL");
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

            return View(model);
        }

        #endregion

        #region ADD

        [SessionExpire]
        public async Task<ActionResult> COA_DE_MANUAL_ADD()
        {
            ViewBag.Tittle = "COA Manual Data Entry";
            var model = new COAManualModel();

            model.DropdownSupplier = await LoadDllData(0, "", "COA_SUPPLIER");
            model.DropdownMaterial = new List<SelectListItem>();
            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> COA_DE_MANUAL_ADD(string ActionType, COAManualModel model)
        {
            ViewBag.Tittle = "COA Manual Data Entry";
            try
            {
                if (ActionType == "postback")
                {
                    ModelState.Clear();
                }
                if (ActionType == "Back")
                {
                    ModelState.Clear();
                    return RedirectToAction("COA_DE_MANUAL_LST", "COA_DE_MANUAL");
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

            model.DropdownSupplier = await LoadDllData(0, "", "COA_SUPPLIER");
            model.DropdownMaterial = new List<SelectListItem>();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> UploadManualFile(IFormFile DataUploadFile, string supplierID, string material, string lotno, string coaindicator)
        {
            string EMP_NO = (HttpContext.Session.GetObject<ACL_UserObj>("AclUser")).EMP_NO.ToString();

            string message = "";
            List<MMSpecLstModel> listItemsAdd = new List<MMSpecLstModel>();
            try
            {
                if (DataUploadFile == null || DataUploadFile.Length == 0)
                {
                    return Json(new { success = false, message = "No file uploaded." });
                }

                string[] validFileTypes = new[] { ".pdf", ".PDF" };
                string ext = System.IO.Path.GetExtension(DataUploadFile.FileName);
                string oriext = System.IO.Path.GetExtension(DataUploadFile.FileName);

                bool isValidFile = false;

                foreach (string i in validFileTypes)
                {
                    if (ext == i)
                    {
                        isValidFile = true;
                    }
                }

                if (isValidFile == false)
                {
                    return Json(new { success = false, message = "Only allowed to upload file with extension of .pdf" });
                }

                if (isValidFile == true)
                {
                    //save pdf file in server

                    string filename = DataUploadFile.FileName;
                    string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "~/Splitpath/");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    string[] lotNumbers = lotno.Split(',');
                    foreach (var lot1 in lotNumbers)
                    {
                        filename = "COA_" + lot1 + "_" + EMP_NO + ".pdf";
                        string pagePath = Path.Combine(uploadPath, filename);
                        string filePath = Path.Combine(uploadPath, filename);
                        using (var _fs = new FileStream(filePath, FileMode.Create)) {{ DataUploadFile.CopyTo(_fs); }}
                    }

                    string path = "";
                    string tpm_code = "";
                    string type = "";

                    List<List<MMSpecLstModel>> MMSpecLstModel = new List<List<MMSpecLstModel>>();
                    List<HeaderMMOCRData> HeaderMMOCRData = new List<HeaderMMOCRData>();
                    
                    DataTable dt = await dbocr.getKeyInfo("GET_SPECVAL", material, supplierID);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        tpm_code = dt.Rows[0]["TPM_CODE"] != DBNull.Value ? dt.Rows[0]["TPM_CODE"].ToString() : "";
                    }

                    if(coaindicator == "3")
                    {
                        bool hasnewlot = false;
                        string combinedlot = "";

                        DataTable dt_a = await dbocr.getKeyInfo("CHECK_ALLLOTNO", lotno.Replace(",", "*"));
                        if (dt_a != null && dt_a.Rows.Count > 0)
                        {
                           
                            foreach (var eachlot in lotNumbers)
                            {
                                //hasnewlot = false;
                                bool isrejected = false;
                                bool approvelot = false;

                                DataTable dt_lot = await dbocr.getKeyInfo("CHECK_LOTNO", eachlot);
                                if (dt_lot != null && dt_lot.Rows.Count > 0)
                                {
                                    path = dt_lot.Rows[0]["ENTRY_TYPE"] != DBNull.Value ? dt_lot.Rows[0]["ENTRY_TYPE"].ToString() : "0";

                                    for (int m = 0; m < dt_lot.Rows.Count; m++)
                                    {
                                        if (dt_lot.Rows[m]["COMPARISON_RESULT"].ToString() == "REJECTED")
                                        {
                                            type = "R";
                                            isrejected = true;
                                            break;
                                        }

                                    }

                                    if (!isrejected)  // all approve
                                    {
                                        type = "A";
                                        approvelot = true;
                                    }

                                    if (approvelot)
                                    {
                                        // all approve
                                        List<MMSpecLstModel> listItemsAdd2 = new List<MMSpecLstModel>();
                                        if (dt_lot != null && dt_lot.Rows.Count > 0)
                                        {
                                            for (int i = 0; i < dt_lot.Rows.Count; i++)
                                            {
                                                MMSpecLstModel infoObjAdd = CreateMMData(dt_lot.Rows[i], approvelot);
                                                listItemsAdd2.Add(infoObjAdd);
                                            }
                                        }

                                        MMSpecLstModel.Add(listItemsAdd2);
                                    }
                                    else
                                    {
                                        List<MMSpecLstModel> listItemsAdd2 = new List<MMSpecLstModel>();
                                        if (dt != null && dt.Rows.Count > 0)
                                        {
                                            
                                            for (int i = 0; i < dt.Rows.Count; i++)
                                            {
                                                MMSpecLstModel infoObjAdd = CreateMMData(dt.Rows[i], approvelot);
                                                listItemsAdd2.Add(infoObjAdd);
                                            }
                                        }

                                        MMSpecLstModel.Add(listItemsAdd2);
                                    }

                                    HeaderMMOCRData headerinfoObjAdd = new HeaderMMOCRData();
                                    headerinfoObjAdd.lot_no = eachlot;
                                    headerinfoObjAdd.tabname = eachlot;
                                    headerinfoObjAdd.is_lot_exist = type;
                                    headerinfoObjAdd.path = path;

                                    HeaderMMOCRData.Add(headerinfoObjAdd);
                                }
                                else
                                {
                                    hasnewlot = true;
                                    if (combinedlot != "")
                                    {
                                        combinedlot = combinedlot + "," + eachlot;
                                    }
                                    else
                                    {
                                        combinedlot = eachlot;
                                    }
                                }
                            }

                            if (hasnewlot) // ADD NONEXIST LOT
                            {
                                type = "";
                                if (dt != null && dt.Rows.Count > 0)
                                {
                                    for (int i = 0; i < dt.Rows.Count; i++)
                                    {
                                        MMSpecLstModel infoObjAdd = CreateMMData(dt.Rows[i], false);
                                        listItemsAdd.Add(infoObjAdd);
                                    }
                                }

                                MMSpecLstModel.Add(listItemsAdd);

                                HeaderMMOCRData headerinfoObjAdd = new HeaderMMOCRData();
                                headerinfoObjAdd.lot_no = combinedlot;
                                headerinfoObjAdd.tabname = combinedlot;
                                headerinfoObjAdd.is_lot_exist = type;
                                headerinfoObjAdd.path = path;

                                HeaderMMOCRData.Add(headerinfoObjAdd);
                            }
                        }
                        else // ALL LOT IS NON EXIST
                        {
                            bool approvelot = false;
                            type = "";

                            if (dt != null && dt.Rows.Count > 0)
                            {
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    MMSpecLstModel infoObjAdd = CreateMMData(dt.Rows[i], approvelot);
                                    listItemsAdd.Add(infoObjAdd);
                                }
                            }

                            MMSpecLstModel.Add(listItemsAdd);

                            HeaderMMOCRData headerinfoObjAdd = new HeaderMMOCRData();
                            headerinfoObjAdd.lot_no = lotno;
                            headerinfoObjAdd.tabname = lotno;
                            headerinfoObjAdd.is_lot_exist = type;
                            headerinfoObjAdd.path = path;

                            HeaderMMOCRData.Add(headerinfoObjAdd);
                        }
                    }
                    else
                    {
                        bool isrejected = false;
                        bool approvelot = false;
                        DataTable dt_lot = await dbocr.getKeyInfo("CHECK_LOTNO", lotno);
                        if (dt_lot != null && dt_lot.Rows.Count > 0)
                        {
                            path = dt_lot.Rows[0]["ENTRY_TYPE"] != DBNull.Value ? dt_lot.Rows[0]["ENTRY_TYPE"].ToString() : "0";

                            for (int m = 0; m < dt_lot.Rows.Count; m++)
                            {
                                if (dt_lot.Rows[m]["COMPARISON_RESULT"].ToString() == "REJECTED")
                                {
                                    type = "R";
                                    isrejected = true;
                                    break;
                                }
                            }

                            if (!isrejected)  // all approve
                            {
                                type = "A";
                                approvelot = true;
                            }
                        }
                        else
                        {
                            type = "";
                        }

                        if (approvelot)
                        {
                            // all approve
                            if (dt_lot != null && dt_lot.Rows.Count > 0)
                            {
                                for (int i = 0; i < dt_lot.Rows.Count; i++)
                                {
                                    MMSpecLstModel infoObjAdd = CreateMMData(dt_lot.Rows[i], approvelot);
                                    listItemsAdd.Add(infoObjAdd);
                                }
                            }
                        }
                        else
                        {
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    MMSpecLstModel infoObjAdd = CreateMMData(dt.Rows[i], approvelot);
                                    listItemsAdd.Add(infoObjAdd);
                                }
                            }
                            
                        }

                        MMSpecLstModel.Add(listItemsAdd);

                        HeaderMMOCRData headerinfoObjAdd = new HeaderMMOCRData();
                        headerinfoObjAdd.lot_no = lotno;
                        headerinfoObjAdd.tabname = lotno;
                        headerinfoObjAdd.is_lot_exist = type;
                        headerinfoObjAdd.path = path;

                        HeaderMMOCRData.Add(headerinfoObjAdd);
                    }

                    COAOCRModel finalResult = new COAOCRModel
                    {
                        HeaderMMOCRData = HeaderMMOCRData,
                        MMSpecLstModel = MMSpecLstModel,
                    };

                    return Json(new { 
                        success = true, 
                        message = message,
                        d1 = finalResult,
                        orifilename = DataUploadFile.FileName,
                        tpm_code = tpm_code,
                        filename = filename
                    });
                    
                }

                return Json(new { success = true, message = "File uploaded successfully." });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }

        }

        #endregion

        #region FUNCTION

        public MMSpecLstModel CreateMMData(DataRow row, bool approve)
        {
            MMSpecLstModel infoObjAdd = new MMSpecLstModel();
            infoObjAdd.MM_MATERIAL_D2_ID = Convert.ToInt32(row["MM_MATERIAL_D2_ID"]);
            infoObjAdd.SUPPLIER_SPEC_NAME = row["SUPPLIER_SPEC_NAME"] != DBNull.Value ? row["SUPPLIER_SPEC_NAME"].ToString() : "";
            infoObjAdd.SUPPLIER_SPEC_VALUE = row["SUPPLIER_SPEC_VALUE"] != DBNull.Value ? row["SUPPLIER_SPEC_VALUE"].ToString() : "";
            infoObjAdd.SUPPLIER_SPEC_TYPE = row["SUPPLIER_SPEC_TYPE"] != DBNull.Value ? row["SUPPLIER_SPEC_TYPE"].ToString() : "";
            infoObjAdd.TPM_CODE = row["TPM_CODE"] != DBNull.Value ? row["TPM_CODE"].ToString() : "";

            if (approve)
            {
                infoObjAdd.OCR_RESULT = row["OCR_RESULT"] != DBNull.Value ? row["OCR_RESULT"].ToString() : "";
                infoObjAdd.COMPARISON_RESULT = row["COMPARISON_RESULT"] != DBNull.Value ? row["COMPARISON_RESULT"].ToString() : "";
            }

            return infoObjAdd;
        }

        #endregion

        #region AUDIT

        [SessionExpire]
        public async Task<ActionResult> COA_DE_MANUAL_AUDIT(string id)
        {
            AuditTrailModels AuditTrailModels = new AuditTrailModels();
            List<SqlParameter> _pMssql = new List<SqlParameter>();
            _pMssql.Add(new SqlParameter("@TABLE", "PVIEW_DE_OCR_A"));
            _pMssql.Add(new SqlParameter("@KEY_VALUE", id));
            _pMssql.Add(new SqlParameter("@SortColumn", "UPDATED_DATE"));
            _pMssql.Add(new SqlParameter("@SortType", "DESC"));

            string dbname = "";
            string isTest = TPM_QAS.DAL.Database.GetAppSettingStatic("isTest");

            if (string.Equals(isTest, "TRUE", StringComparison.OrdinalIgnoreCase))
            {
                dbname = TPM_QAS.DAL.Database.GetAppSettingStatic("DEV");
            }
            else
            {
                dbname = TPM_QAS.DAL.Database.GetAppSettingStatic("LIVE");
            }

            AuditTrailModels = await AuditTrailHelper.AuditTrailStoreProcedureSqlAsync("PSP_GET_AUDIT_TRAIL", CommandType.StoredProcedure, _pMssql, dbname);


            ViewBag.JsonResult = AuditTrailModels.JsonData;
            ViewBag.KeyNames = AuditTrailModels.ListData;
            ViewBag.hid = id;

            return View();
        }

        #endregion

        #region DDL

        public async Task<ActionResult> fillSupplier()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items = await LoadInnerDllData(0, "", "COA_SUPPLIER");

            return Json(items);
        }

        public async Task<ActionResult> fillMaterial(string supp)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items = await LoadInnerDllData(0, supp, "COA_SUPP_MAT");

            return Json(items);
        }

        private async Task<List<SelectListItem>> LoadInnerDllData(int ID, string act, string category)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            listItems.Add(new SelectListItem { Text = "Select an option..", Value = "0" });

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