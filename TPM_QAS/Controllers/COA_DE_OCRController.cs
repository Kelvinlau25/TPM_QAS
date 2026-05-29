using DBModel;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Data;
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
using Newtonsoft.Json;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Security.Cryptography.Xml;
using Newtonsoft.Json.Linq;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Linq;
using System.Web.Services.Description;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.Data.SqlClient;
using DocumentFormat.OpenXml.Bibliography;
using iTextSharp.xmp.impl.xpath;

namespace TPM_QAS.Controllers
{
    public class COA_DE_OCRController : Controller
    {
        DB dbmain = new DB();
        COA_DE_OCR_DAL dbdal = new COA_DE_OCR_DAL();
        COA_GA_RAWMAT_ABNORM_DAL dbabnorm = new COA_GA_RAWMAT_ABNORM_DAL();
        AzureBlobHelper blob = new AzureBlobHelper();
        CommonFunction common = new CommonFunction();

        #region LIST

        [SessionExpire]
        public async Task<ActionResult> COA_DE_OCR_LST(string Deleted = "0")
        {
            DeleteOldFiles();
            ViewBag.Tittle = "COA OCR Data Entry";

            if (Deleted == "1")
            {
                TempData["STATUS_DDL"] = await dbmain.PSP_COMMON_DDL("CO_DE_OCR", "", "", "", "", "", "");
            }
            else
            {
                TempData["STATUS_DDL"] = await dbmain.PSP_COMMON_DDL("CO_DE_OCR2", "", "", "", "", "", "");
            }

            CommonFunction common = new CommonFunction();
            var param = new { Table = "PVIEW_DE_COA_OCR", TableID = "", Search = "", Value = "", SortField = "DE_OCR_H_ID", Direction = "0", FrmRowno = "1", ToRowno = "500000", Deleted = Deleted };
            List<COAOCRModel> model = await common.PSP_COMMON_DAPPER<COAOCRModel>("PSP_COMMON_LIST", CommandType.StoredProcedure, param) ?? new List<COAOCRModel>();

            ViewBag.Deleted = Deleted;

            string emp_no = (Session["AclUser"] as ACL_UserObj).EMP_NO.ToString();

            DataTable chkApproval = await dbdal.CheckUserAppRole(emp_no);
            if (chkApproval != null && chkApproval.Rows.Count > 0)
            {
                ViewBag.isManager = "Y";
            }

            //for KS and Izzah
            if ((Session["AclUser"] as ACL_UserObj).USER_ID.ToString() == "800133" || (Session["AclUser"] as ACL_UserObj).USER_ID.ToString() == "800179")
            {
                ViewBag.isManager = "Y";
            }

            return View(model);
        }

        #endregion

        #region VIEW/EDIT

        [SessionExpire]
        public async Task<ActionResult> COA_DE_OCR_DETAIL(string id)
        {
            ViewBag.Tittle = "COA OCR Data Entry";
            var model = new COAOCRModel();
            model.OCRLstModelDetail = new Dictionary<Tuple<string, int>, List<OCRLstModel>>();

            DataTable dt = await dbdal.getDEOCR_Data(id, "H"); //get header data

            if (dt != null && dt.Rows.Count > 0)
            {
                model.DE_OCR_H_ID = Convert.ToInt32(dt.Rows[0]["DE_OCR_H_ID"]);
                model.ENTRY_TYPE = dt.Rows[0]["ENTRY_TYPE"] != DBNull.Value ? dt.Rows[0]["ENTRY_TYPE"].ToString() : "";
                model.OCR_INDICATOR = dt.Rows[0]["OCR_INDICATOR"] != DBNull.Value ? dt.Rows[0]["OCR_INDICATOR"].ToString() : "";
                model.FILE_NAME = dt.Rows[0]["FILE_NAME"] != DBNull.Value ? dt.Rows[0]["FILE_NAME"].ToString() : "";
                model.FINAL_RESULT = dt.Rows[0]["FINAL_RESULT"] != DBNull.Value ? dt.Rows[0]["FINAL_RESULT"].ToString() : "";

                model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";
            }

            List<SupplierTab> dbselect = await dbdal.getsupplierlist(id, "S");
            foreach (var item in dbselect)
            {
                DataTable dtl = await dbdal.getDEOCR_Data(id, "D", item.MM_SUPPLIER_H_ID, item.LOT_NO); //get d data
                if (dtl != null && dtl.Rows.Count > 0)
                {
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
                            new Tuple<string, int>(item.TABNAME, item.MM_SUPPLIER_H_ID),
                            listItemsAdd
                        );
                    }
                }
            }

            string emp_no = (Session["AclUser"] as ACL_UserObj).EMP_NO.ToString();

            DataTable chkApproval = await dbdal.CheckUserAppRole(emp_no);
            if (chkApproval != null && chkApproval.Rows.Count > 0)
            {
                ViewBag.isManager = "Y";
            }

            //for KS and Izzah
            if ((Session["AclUser"] as ACL_UserObj).USER_ID.ToString() == "800133" || (Session["AclUser"] as ACL_UserObj).USER_ID.ToString() == "800179")
            {
                ViewBag.isManager = "Y";
            }

            return View(model);
        }


        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> COA_DE_OCR_DETAIL(string ActionType, COAOCRModel model)
        {
            ViewBag.Tittle = "COA OCR Data Entry";
            try
            {
                if (ActionType == "postback")
                {
                    ModelState.Clear();
                }
                if (ActionType == "Back")
                {
                    ModelState.Clear();
                    return RedirectToAction("COA_DE_OCR_LST", "COA_DE_OCR");
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
        public async Task<ActionResult> InsertUpdateDEOCR(COAOCRModel model, string loc = "")
        {
            string EMP_NO = (Session["AclUser"] as ACL_UserObj).EMP_NO.ToString();
            bool success = true;
            string message = "";
            string type = "";
            string isTest = TPM_QAS.DAL.Database.GetAppSettingStatic("isTest"];

            if (ModelState.IsValid)
            {
                if (model.MMOCRDataLst == null || model.MMOCRDataLst.Count < 1)
                {
                    success = false;
                    message += "Please fill in at least one data in each tab";
                }

                if (success)
                {
                    DateTime currentDateTime = DateTime.Now;
                    model.FINAL_FILE_NAME = "";

                    // insert h
                    string result1 = await dbdal.DE_OCR_H_Maint(model);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                    else
                    {
                        //delete existing d first
                        string result9 = await dbdal.DE_OCR_D_Maint(Convert.ToInt32(result1), 0, 0, "", 0, "", 0, "", 0, "", "", "2");
                        if (!(int.TryParse(result9, out int num9)))
                        {
                            success = false;
                            message += result9;
                        }
                        else
                        {
                            // insert d1
                            if (model.MMOCRDataLst != null && model.MMOCRDataLst.Count > 0)
                            {
                                model.MMOCRDataLst = model.MMOCRDataLst.OrderBy(x => x.lot_no).ToList();

                                string prevlot = ""; string prevfilename = ""; 

                                foreach (var item in model.MMOCRDataLst)
                                {
                                    int mat_id = Convert.ToInt32(item.MATERIAL_ID);
                                    int supp_id = Convert.ToInt32(item.supplier_id);
                                    string lot_no = item.lot_no != null ? item.lot_no.ToString() : "";
                                    int d2_id = Convert.ToInt32(item.MM_MATERIAL_D2_ID);

                                    string spec = item.ocr_spec != null ? item.ocr_spec.ToString() : "";
                                    decimal cfs_spec = Convert.ToDecimal(item.cfs_ocr_spec);
                                    string val_result = item.ocr_spec_value != null ? item.ocr_spec_value.ToString() : "";
                                    decimal cfs_val_result = Convert.ToDecimal(item.cfs_ocr_spec_value);
                                    string comp_result = item.compare_result != null ? item.compare_result.ToString() : "";

                                    string rectype = item.RECORD_TYP != null ? item.RECORD_TYP.ToString() : "";

                                    if (rectype == "0")
                                    {
                                        rectype = "3";
                                    }

                                    string ori_name = "COA_" + lot_no + "_" + EMP_NO + ".pdf";
                                    string file_name = "";
                                    if (loc == "add")
                                    {
                                        if (prevlot != lot_no)
                                        {
                                            file_name = "COA_" + lot_no + "_" + currentDateTime.ToString("yyyy-MM-dd_HHmmss") + ".pdf";
                                        }
                                        else
                                        {
                                            file_name = prevfilename;
                                        }
                                    }
                                    else
                                    {
                                        file_name = item.final_name != null ? item.final_name.ToString() : "";
                                    }

                                    // Added by ANAS on 03/10/2025 (Sanitize the filename)
                                    ori_name = FileNameHelper.SanitizeFileName(ori_name);
                                    file_name = FileNameHelper.SanitizeFileName(file_name);

                                    string result2 = await dbdal.DE_OCR_D_Maint(Convert.ToInt32(result1), mat_id, supp_id, lot_no, d2_id, spec, cfs_spec,
                                                                            val_result, cfs_val_result, comp_result, file_name, rectype);

                                    if (!(int.TryParse(result2, out int num2) && result2 != "0"))
                                    {
                                        success = false;
                                        message += result2;
                                    }
                                    else
                                    {
                                        // check abnormalities
                                        string result3 = await dbabnorm.checkAbnorm(Convert.ToInt32(result2), rectype);

                                        // move file from 
                                        if (loc == "add")
                                        {
                                            if (prevlot != lot_no)
                                            {  
                                                string firstPath = Server.MapPath("~/Splitpath/");
                                                string firstocrpath = Server.MapPath("~/COA_DE_OCR/FirstOCRFiles/");

                                                string firstpdf = Path.Combine(firstPath, ori_name);
                                                string firstocrfile = Path.Combine(firstocrpath, model.FILE_NAME);

                                                if (System.IO.File.Exists(firstpdf))
                                                {
                                                    //BLOB STORAGE//
                                                    byte[] fileBytes = System.IO.File.ReadAllBytes(firstpdf);
                                                    string partfilename = "COA Document/" + file_name;

                                                    var uploadResult = await blob.uploadBlobpdf(partfilename, common.getContainerName(), fileBytes);

                                                    // delete the local file after successful upload
                                                    if (uploadResult.Count > 0)
                                                    {
                                                        System.IO.File.Delete(firstpdf);
                                                    }
                                                }

                                                if (System.IO.File.Exists(firstocrfile))
                                                {
                                                    System.IO.File.Delete(firstocrfile);
                                                }


                                                // copy to TPM FILE
                                                //if (!(string.Equals(isTest, "TRUE", StringComparison.OrdinalIgnoreCase)))
                                                //{
                                                //    string destPdf = Path.Combine(tpmpath, file_name);
                                                //    string login = "tpm_qas_app";
                                                //    string domain = "TORAY";
                                                //    string password = "Toray123";

                                                //    using (UserImpersonation user = new UserImpersonation(login, domain, password))
                                                //    {
                                                //        if (user.ImpersonateValidUser())
                                                //        {
                                                //            if (!Directory.Exists(tpmpath))
                                                //            {
                                                //                Directory.CreateDirectory(tpmpath);
                                                //            }

                                                //            if (System.IO.File.Exists(secpdf))
                                                //            {
                                                //                System.IO.File.Copy(secpdf, destPdf, true);
                                                //            }
                                                //        }
                                                //    }
                                                //}
                                            }

                                            prevlot = lot_no;
                                            prevfilename = file_name;
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
                    message += "Error in saving data.";
                }
            }
            else
            {
                success = false;
                message += "Error in saving data : Modal State not valid. Please ensure all data has fill in. ";
            }

            var data = new { success = success, message = message, type = type };

            return Json(data);
        }

        #endregion

        #region ADD

        [SessionExpire]
        public async Task<ActionResult> COA_DE_OCR_ADD()
        {
            ViewBag.Tittle = "COA OCR Data Entry";
            var model = new COAOCRModel();
            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> COA_DE_OCR_ADD(string ActionType, COAOCRModel model)
        {
            ViewBag.Tittle = "COA OCR Data Entry";
            try
            {
                if (ActionType == "postback")
                {
                    ModelState.Clear();
                }
                if (ActionType == "Back")
                {
                    ModelState.Clear();
                    return RedirectToAction("COA_DE_OCR_LST", "COA_DE_OCR");
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> UploadOCRFile(IFormFile DataUploadFile, string ocrindicator, string coalang = "english")
        {
            string message = "";

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
                    return Json(new { success = false, message = "OCR fail to read file. Only upload file with extension of .pdf" });
                }

                if (isValidFile == true)
                {
                    HttpClientHandler handler = new HttpClientHandler();

                    AppConfig config = GetAppConfig();
                    if (config == null)
                    {
                        return Json(new { success = false, message = "Configuration error." });
                    }

                    MultipartFormDataContent formData = new MultipartFormDataContent();

                    // for first time ocr - to get material and supplier name
                    DataTable dt = await dbdal.getKeyInfo("OPT_KEY_INFO");
                    string fkeyinfo = "";

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            fkeyinfo = dt.Rows[i]["KEY_INFO"] != DBNull.Value ? dt.Rows[i]["KEY_INFO"].ToString() : "";
                            formData.Add(new StringContent(fkeyinfo), "Key_" + (i + 1));
                        }
                    }
                    //////////////////////////////////////////////

                    //#region for testing
                    //var result = "{\"key_info\":[{\"lot_no\":[\"27LQ5\",\"30MQ7\"],\"material_name\":[\"Activated Carbon\"],\"supplier_name_not_toray\":[\"CENTURY CHEMICALWORKS SDN.BHD.(9139-W)\",\"MALBON Simply Unique.\"]}],\"ocr\":[[{\"confidence_score\":0.9762595295906067,\"text\":\"CENTURY CHEMICALWORKS SDN.BHD.(9139-W)\"},{\"confidence_score\":0.9073800444602966,\"text\":\"MALBON Simply Unique.\"},{\"confidence_score\":0.9899501204490662,\"text\":\"Certificate of Analysis\"},{\"confidence_score\":0.9690288305282593,\"text\":\"http://www.century-chemical.com\"}]],\"token_consumption\":[{\"Completion Tokens\":60,\"Prompt Tokens\":5072,\"Total Cost (USD)\":0.026260000000000002,\"Total Tokens\":5132}],\"url\":null}\r\n";

                    //var data = JsonConvert.DeserializeObject<OcrResult>(result);

                    ////save pdf file in server
                    //DateTime currentDateTime = DateTime.Now;
                    //string filename = DataUploadFile.FileName;
                    //string uploadPath = Server.MapPath("~/COA_DE_OCR/FirstOCRFiles/");
                    //if (!Directory.Exists(uploadPath))
                    //{
                    //    Directory.CreateDirectory(uploadPath);
                    //}

                    //string filePath = Path.Combine(uploadPath, filename);
                    //DataUploadFile.SaveAs(filePath);

                    //List<FirstOCRLstModel> listItemsAdd = new List<FirstOCRLstModel>();

                    //if (ocrindicator == "1") //Single Lot no
                    //{
                    //    if (data?.key_info != null)
                    //    {
                    //        foreach (var item in data.key_info)
                    //        {
                    //            FirstOCRLstModel infoObjAdd = new FirstOCRLstModel();

                    //            infoObjAdd.lot_no = item.lot_no?.Count > 0 ? item.lot_no[0] : "Not captured by OCR";
                    //            infoObjAdd.material_name = item.material_name?.Count > 0 ? item.material_name[0] : "Not captured by OCR";
                    //            infoObjAdd.supplier_name = item.supplier_name_not_toray?.Count > 0 ? item.supplier_name_not_toray[0] : "Not captured by OCR";

                    //            listItemsAdd.Add(infoObjAdd);
                    //        }
                    //    }
                    //}
                    //else //if (ocrindicator == "3") //Multiple Lot No and Single Result
                    //{
                    //    if (data?.key_info != null)
                    //    {
                    //        foreach (var item in data.key_info)
                    //        {
                    //            string combinedlot = "";
                    //            if (item.lot_no != null && item.lot_no.Count() > 0)
                    //            {
                    //                combinedlot = string.Join(",\n", item.lot_no);
                    //            }
                    //            else
                    //            {
                    //                combinedlot = "Not captured by OCR";
                    //            }

                    //            FirstOCRLstModel infoObjAdd = new FirstOCRLstModel();
                    //            infoObjAdd.lot_no = combinedlot;
                    //            infoObjAdd.material_name = item.material_name?.Count > 0 ? item.material_name[0] : "Not captured by OCR";
                    //            infoObjAdd.supplier_name = item.supplier_name_not_toray?.Count > 0 ? item.supplier_name_not_toray[0] : "Not captured by OCR";

                    //            listItemsAdd.Add(infoObjAdd);
                    //        }
                    //    }
                    //}
                    //return Json(new { success = true, message = message, filename = filename, firstOCRresult = listItemsAdd, ocrindicator = ocrindicator });
                    //#endregion

                    #region real code
                    //save pdf file in server
                    DateTime currentDateTime = DateTime.Now;
                    //string filename = "COA_OCR_" + currentDateTime.ToString("yyyy-MM-dd_HHmmss") + ".pdf";
                    string filename = DataUploadFile.FileName;
                    string uploadPath = Server.MapPath("~/COA_DE_OCR/FirstOCRFiles/");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    string filePath = Path.Combine(uploadPath, filename);
                    DataUploadFile.SaveAs(filePath);

                    using (HttpClient client = new HttpClient(handler))
                    {
                        client.Timeout = TimeSpan.FromMinutes(6);  // Set timeout to 6 minutes

                        StreamContent fileContent = new StreamContent(DataUploadFile.OpenReadStream());
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");

                        formData.Add(fileContent, "image", filename);
                        formData.Add(new StringContent(config.TMSUserAPIKey), "apiKey");
                        formData.Add(new StringContent(coalang), "language");

                        //Send request to API
                        HttpResponseMessage response = await client.PostAsync(config.OCREndpoint, formData);
                        string responseString = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            var result = await response.Content.ReadAsStringAsync();
                            var data = JsonConvert.DeserializeObject<OcrResult>(result);

                            List<FirstOCRLstModel> listItemsAdd = new List<FirstOCRLstModel>();

                            if (ocrindicator == "1") //Single Lot no
                            {
                                if (data?.key_info != null)
                                {
                                    foreach (var item in data.key_info)
                                    {
                                        FirstOCRLstModel infoObjAdd = new FirstOCRLstModel();

                                        infoObjAdd.lot_no = item.lot_no?.Count > 0 ? item.lot_no[0] : "Not captured by OCR";
                                        infoObjAdd.material_name = item.material_name?.Count > 0 ? item.material_name[0] : "Not captured by OCR";
                                        infoObjAdd.supplier_name = item.supplier_name_not_toray?.Count > 0 ? item.supplier_name_not_toray[0] : "Not captured by OCR";

                                        listItemsAdd.Add(infoObjAdd);
                                    }
                                }
                            }
                            else //if (ocrindicator == "3") //Multiple Lot No and Single Result
                            {
                                if (data?.key_info != null)
                                {
                                    foreach (var item in data.key_info)
                                    {
                                        string combinedlot = "";
                                        if (item.lot_no != null && item.lot_no.Count() > 0)
                                        {
                                            combinedlot = string.Join(",\n", item.lot_no);
                                        }
                                        else
                                        {
                                            combinedlot = "Not captured by OCR";
                                        }

                                        FirstOCRLstModel infoObjAdd = new FirstOCRLstModel();
                                        infoObjAdd.lot_no = combinedlot;
                                        infoObjAdd.material_name = item.material_name?.Count > 0 ? item.material_name[0] : "Not captured by OCR";
                                        infoObjAdd.supplier_name = item.supplier_name_not_toray?.Count > 0 ? item.supplier_name_not_toray[0] : "Not captured by OCR";

                                        listItemsAdd.Add(infoObjAdd);
                                    }
                                }
                            }


                            return Json(new { success = true, message = message, filename = filename, firstOCRresult = listItemsAdd, ocrindicator = ocrindicator });
                        }
                        else
                        {
                            //return Json(new { success = false, message = "OCR API Error3. " + responseString });
                            return Json(new { success = false, message = "OCR API Error. " });
                        }
                    }
                    #endregion
                }

                return Json(new { success = true, message = "File uploaded successfully." });

            }
            catch (Exception ex)
            {
                //return Json(new { success = false, message = "OCR API Error1. " + ex.Message });
                return Json(new { success = false, message = "OCR API Error. " });
            }

        }

        [HttpPost]
        public async Task<ActionResult> getOCRFileData(IFormFile DataUploadFile, string modelObj, string ocrindicator, string coalang = "english")
        {
            try
            {
                if (DataUploadFile == null || DataUploadFile.Length == 0)
                {
                    return Json(new { success = false, message = "No file uploaded." });
                }
                string EMP_NO = (Session["AclUser"] as ACL_UserObj).EMP_NO.ToString();

                MultipartFormDataContent formData = new MultipartFormDataContent();
                List<KeyInfoData> KeyInfoData = new List<KeyInfoData>();
                List<List<MMOCRData>> allPagesData = new List<List<MMOCRData>>();
                List<HeaderMMOCRData> HeaderMMOCRData = new List<HeaderMMOCRData>();

                var model = JsonConvert.DeserializeObject<COAOCRModel>(modelObj);

                if (!(model.FirstOCRLstModel != null && model.FirstOCRLstModel.Count > 0))
                {
                    return Json(new { success = false, message = "Must have atleast one supplier" });
                }

                #region split pdf

                string splitpath = Path.Combine(Server.MapPath("~/Splitpath"));


                if (!Directory.Exists(splitpath))
                {
                    Directory.CreateDirectory(splitpath);
                }

                // Read uploaded PDF into memory
                byte[] uploadedBytes;
                using (var ms = new MemoryStream())
                {
                    await DataUploadFile.OpenReadStream().CopyToAsync(ms);
                    uploadedBytes = ms.ToArray();
                }


                try
                {
                    using (PdfReader reader = new PdfReader(uploadedBytes))
                    using (MemoryStream outputMemory = new MemoryStream())
                    using (Document document = new Document())
                    using (PdfCopy writer = new PdfCopy(document, outputMemory))
                    {
                        try
                        {
                            document.Open();

                            // check if has multiple page with same lot
                            var pagesByLot = model.FirstOCRLstModel
                           .Select((x, index) => new { x.lot_no, PageIndex = index + 1, record_typ = x.record_typ })
                           .Where(x => x.record_typ != "5") // skip removed pages
                           .GroupBy(x => x.lot_no);

                            foreach (var lotGroup in pagesByLot)
                            {
                                string lotNo = lotGroup.Key;

                                var pageIndexes = lotGroup.Select(x => x.PageIndex).ToList();
                                var lots = lotNo.Split('*');

                                using (MemoryStream msMerged = new MemoryStream())
                                {

                                    using (Document mergedDoc = new Document())
                                    using (PdfCopy mergedWriter = new PdfCopy(mergedDoc, msMerged))
                                    {

                                        mergedDoc.Open();
                                        foreach (int pageIndex in pageIndexes)
                                        {
                                            PdfImportedPage page = mergedWriter.GetImportedPage(reader, pageIndex);
                                            mergedWriter.AddPage(page);
                                        }
                                        mergedDoc.Close();
                                    }

                                    byte[] mergedBytes = msMerged.ToArray();

                                    // Save the same merged PDF for each lot
                                    foreach (string lot in lots)
                                    {
                                        string cleanedLot = Regex.Replace(lot, @"[\/\\:\*\?""<>|]", "");
                                        string fileformat = $"COA_{cleanedLot}_{EMP_NO}.pdf";

                                        // Added by ANAS on 03/10/2025 (To sanitize the filename)
                                        //fileformat = FileNameHelper.SanitizeFileName(fileformat);

                                        string mergedPath = Path.Combine(splitpath, fileformat);
                                        System.IO.File.WriteAllBytes(mergedPath, mergedBytes);
                                    }

                                    try
                                    {
                                        int indexpage = pageIndexes.First() - 1;
                                        //---------------PROCESS PAGE TO OCR-----------------------------
                                        ProcessPageResult result = await ProcessPage(mergedBytes, indexpage, modelObj, ocrindicator, coalang);

                                        byte[] processedPage = result.PdfPage;
                                        KeyInfoData = result.KeyInfoData;

                                        using (PdfReader processedReader = new PdfReader(processedPage))
                                        {
                                            PdfImportedPage processedPdfPage = writer.GetImportedPage(processedReader, 1);
                                            writer.AddPage(processedPdfPage);
                                        }

                                        #region get supplier data from MM

                                        List<MMOCRData> listItemsAdd = new List<MMOCRData>();
                                        string mat = "";
                                        for (int j = 0; j < model.FirstOCRLstModel.Count; j++)
                                        {
                                            if (j == indexpage)
                                            {
                                                mat = model.FirstOCRLstModel[j].material_name.ToString();
                                                string r_lot = model.FirstOCRLstModel[j].lot_no.ToString();
                                                string tpm_code = "";

                                                if (ocrindicator == "2")
                                                {
                                                    if (result.isExist == "N")
                                                    {
                                                        DataTable dtspecval = await dbdal.getKeyInfo("GET_SPECVAL", mat, model.FirstOCRLstModel[j].supplier_id.ToString());
                                                        if (dtspecval != null && dtspecval.Rows.Count > 0)
                                                        {
                                                            var groupedData = KeyInfoData.GroupBy(k => k.lot_no);
                                                            foreach (var group in groupedData)
                                                            {
                                                                List<KeyInfoData> lotgrp = group.ToList();
                                                                List<MMOCRData> listItemsAdd2 = new List<MMOCRData>();

                                                                r_lot = group.First().lot_no;

                                                                DataTable finaldt = MergeMMandOCR(dtspecval, lotgrp);

                                                                if (finaldt != null && finaldt.Rows.Count > 0)
                                                                {
                                                                    for (int k = 0; k < finaldt.Rows.Count; k++)
                                                                    {
                                                                        if (!string.IsNullOrEmpty(finaldt.Rows[k]["tpm_code"].ToString()))
                                                                        {
                                                                            tpm_code = finaldt.Rows[k]["tpm_code"].ToString();
                                                                        }

                                                                        MMOCRData infoObjAdd = CreateMMOCRData(finaldt.Rows[k], mat, Convert.ToInt32(model.FirstOCRLstModel[j].supplier_id));

                                                                        listItemsAdd2.Add(infoObjAdd);
                                                                    }
                                                                }

                                                                HeaderMMOCRData headerinfoObjAdd = CreateHeaderMMOCRData(
                                                                  Convert.ToInt32(model.FirstOCRLstModel[j].supplier_id),
                                                                  model.FirstOCRLstModel[j].supplier_name.ToString(),
                                                                  Convert.ToInt32(model.FirstOCRLstModel[j].material_id),
                                                                  mat,
                                                                  tpm_code,
                                                                  result,
                                                                  r_lot,
                                                                  ocrindicator
                                                                );

                                                                HeaderMMOCRData.Add(headerinfoObjAdd);
                                                                allPagesData.Add(listItemsAdd2);
                                                            }
                                                        }
                                                    }
                                                    else if (result.isExist == "IND23HASREJ" || result.isExist == "IND23APPNON")
                                                    {

                                                        string[] lotNumbers = r_lot.Split('*');
                                                        foreach (var lot in lotNumbers)
                                                        {
                                                            mat = model.FirstOCRLstModel[j].material_name.ToString();
                                                            bool isrejected = false;
                                                            r_lot = lot;

                                                            DataTable dt = await dbdal.getKeyInfo("CHECK_LOTNO", lot);

                                                            if (dt != null && dt.Rows.Count > 0)
                                                            {
                                                                for (int m = 0; m < dt.Rows.Count; m++)
                                                                {

                                                                    if (dt.Rows[m]["COMPARISON_RESULT"].ToString() == "REJECTED")
                                                                    {
                                                                        isrejected = true;

                                                                        result.isExist = "N";
                                                                        result.rejectedID = dt.Rows[m]["ENTRY_TYPE"] != DBNull.Value ? dt.Rows[m]["ENTRY_TYPE"].ToString() : "";
                                                                        //string supplier = dt.Rows[m]["SUPPLIER_NAME"] != DBNull.Value ? dt.Rows[m]["SUPPLIER_NAME"].ToString() : "";
                                                                        //mat = dt.Rows[m]["SUPPLIER_MATERIAL_NAME"] != DBNull.Value ? dt.Rows[m]["SUPPLIER_MATERIAL_NAME"].ToString() : "";
                                                                        //tpm_code = dt.Rows[m]["TPM_CODE"] != DBNull.Value ? dt.Rows[m]["TPM_CODE"].ToString() : "";
                                                                        int suppid = Convert.ToInt32(model.FirstOCRLstModel[j].supplier_id); //Convert.ToInt32(dt.Rows[m]["MM_SUPPLIER_H_ID"]);
                                                                        int matid = Convert.ToInt32(dt.Rows[m]["MM_MATERIAL_H_ID"]);

                                                                        DataTable dtspecval = await dbdal.getKeyInfo("GET_SPECVAL", mat, suppid.ToString());
                                                                        if (dtspecval != null && dtspecval.Rows.Count > 0)
                                                                        {
                                                                            List<KeyInfoData> filteredLotData = KeyInfoData
                                                                            .Where(k => k.lot_no == lot)
                                                                            .ToList();

                                                                            List<MMOCRData> listItemsAdd2 = new List<MMOCRData>();

                                                                            DataTable finaldt = MergeMMandOCR(dtspecval, filteredLotData);

                                                                            if (finaldt != null && finaldt.Rows.Count > 0)
                                                                            {
                                                                                for (int k = 0; k < finaldt.Rows.Count; k++)
                                                                                {
                                                                                    if (!string.IsNullOrEmpty(finaldt.Rows[k]["tpm_code"].ToString()))
                                                                                    {
                                                                                        tpm_code = finaldt.Rows[k]["tpm_code"].ToString();
                                                                                    }

                                                                                    MMOCRData infoObjAdd = CreateMMOCRData(finaldt.Rows[k], mat, Convert.ToInt32(model.FirstOCRLstModel[j].supplier_id));

                                                                                    listItemsAdd2.Add(infoObjAdd);
                                                                                }
                                                                            }

                                                                            HeaderMMOCRData headerinfoObjAdd = CreateHeaderMMOCRData(
                                                                              suppid,
                                                                              model.FirstOCRLstModel[j].supplier_name.ToString(),
                                                                              matid,
                                                                              mat,
                                                                              tpm_code,
                                                                              result,
                                                                              r_lot,
                                                                              ocrindicator
                                                                            );

                                                                            HeaderMMOCRData.Add(headerinfoObjAdd);
                                                                            allPagesData.Add(listItemsAdd2);
                                                                        }

                                                                        break;
                                                                    }
                                                                }

                                                                if (!isrejected)  // all approve
                                                                {
                                                                    //comment 020725 -- user request to capture ocr even exist and approved
                                                                    //List<MMOCRData> listItemsAdd2 = new List<MMOCRData>();
                                                                    //for (int k = 0; k < dt.Rows.Count; k++)
                                                                    //{
                                                                    //    MMOCRData infoObjAdd = new MMOCRData();

                                                                    //    infoObjAdd.lot_no = dt.Rows[k]["LOT_NO"] != DBNull.Value ? dt.Rows[k]["LOT_NO"].ToString() : "";
                                                                    //    infoObjAdd.mat_name = dt.Rows[k]["SUPPLIER_MATERIAL_NAME"] != DBNull.Value ? dt.Rows[k]["SUPPLIER_MATERIAL_NAME"].ToString() : "";
                                                                    //    infoObjAdd.supplier_id = Convert.ToInt32(dt.Rows[k]["MM_SUPPLIER_H_ID"]);
                                                                    //    infoObjAdd.mm_spec_type = dt.Rows[k]["SUPPLIER_SPEC_TYPE"] != DBNull.Value ? dt.Rows[k]["SUPPLIER_SPEC_TYPE"].ToString() : "";
                                                                    //    infoObjAdd.MM_MATERIAL_D2_ID = Convert.ToInt32(dt.Rows[k]["MM_MATERIAL_D2_ID"]);
                                                                    //    infoObjAdd.mm_spec = dt.Rows[k]["SUPPLIER_SPEC_NAME"] != DBNull.Value ? dt.Rows[k]["SUPPLIER_SPEC_NAME"].ToString() : "";
                                                                    //    infoObjAdd.mm_spec_val = dt.Rows[k]["SUPPLIER_SPEC_VALUE"] != DBNull.Value ? dt.Rows[k]["SUPPLIER_SPEC_VALUE"].ToString() : "";
                                                                    //    infoObjAdd.ocr_spec = dt.Rows[k]["OCR_INSPECTION_ITEM"] != DBNull.Value ? dt.Rows[k]["OCR_INSPECTION_ITEM"].ToString() : "";
                                                                    //    infoObjAdd.ocr_spec_value = dt.Rows[k]["OCR_RESULT"] != DBNull.Value ? dt.Rows[k]["OCR_RESULT"].ToString() : "";

                                                                    //    infoObjAdd.cfs_ocr_spec = string.IsNullOrEmpty(dt.Rows[k]["OCR_INSPECTION_ITEM_SCORE"].ToString())
                                                                    //    ? 0m : Math.Round(Convert.ToDecimal(dt.Rows[k]["OCR_INSPECTION_ITEM_SCORE"]), 4);

                                                                    //    infoObjAdd.cfs_ocr_spec_value = string.IsNullOrEmpty(dt.Rows[k]["OCR_RESULT_SCORE"].ToString())
                                                                    //    ? 0m : Math.Round(Convert.ToDecimal(dt.Rows[k]["OCR_RESULT_SCORE"]), 4);

                                                                    //    infoObjAdd.cfs_lot_no = 0;

                                                                    //    infoObjAdd.compare_result = dt.Rows[k]["COMPARISON_RESULT"] != DBNull.Value ? dt.Rows[k]["COMPARISON_RESULT"].ToString() : "";


                                                                    //    listItemsAdd2.Add(infoObjAdd);

                                                                    //}

                                                                    //mat = dt.Rows[0]["SUPPLIER_MATERIAL_NAME"] != DBNull.Value ? dt.Rows[0]["SUPPLIER_MATERIAL_NAME"].ToString() : "";
                                                                    //tpm_code = dt.Rows[0]["TPM_CODE"] != DBNull.Value ? dt.Rows[0]["TPM_CODE"].ToString() : "";
                                                                    //string supplier = dt.Rows[0]["SUPPLIER_NAME"] != DBNull.Value ? dt.Rows[0]["SUPPLIER_NAME"].ToString() : "";
                                                                    //int suppid = Convert.ToInt32(dt.Rows[0]["MM_SUPPLIER_H_ID"]);
                                                                    //int matid = Convert.ToInt32(dt.Rows[0]["MM_MATERIAL_H_ID"]);

                                                                    //HeaderMMOCRData headerinfoObjAdd = CreateHeaderMMOCRData(
                                                                    //  suppid,
                                                                    //  supplier,
                                                                    //  matid,
                                                                    //  mat,
                                                                    //  tpm_code,
                                                                    //  result,
                                                                    //  r_lot,
                                                                    //  ocrindicator
                                                                    //);

                                                                    //HeaderMMOCRData.Add(headerinfoObjAdd);
                                                                    //allPagesData.Add(listItemsAdd2);

                                                                    result.isExist = dt.Rows[0]["ENTRY_TYPE"] != DBNull.Value ? dt.Rows[0]["ENTRY_TYPE"].ToString() : "";
                                                                    result.rejectedID = "";
                                                                    

                                                                    DataTable dtspecval = await dbdal.getKeyInfo("GET_SPECVAL", mat, model.FirstOCRLstModel[j].supplier_id.ToString());
                                                                    if (dtspecval != null && dtspecval.Rows.Count > 0)
                                                                    {
                                                                        List<MMOCRData> listItemsAdd2 = new List<MMOCRData>();

                                                                        List<KeyInfoData> filteredLotData = KeyInfoData
                                                                                .Where(k => k.lot_no == lot)
                                                                                .ToList();

                                                                        DataTable finaldt = MergeMMandOCR(dtspecval, filteredLotData);

                                                                        if (finaldt != null && finaldt.Rows.Count > 0)
                                                                        {
                                                                            for (int k = 0; k < finaldt.Rows.Count; k++)
                                                                            {
                                                                                if (!string.IsNullOrEmpty(finaldt.Rows[k]["tpm_code"].ToString()))
                                                                                {
                                                                                    tpm_code = finaldt.Rows[k]["tpm_code"].ToString();
                                                                                }

                                                                                MMOCRData infoObjAdd = CreateMMOCRData(finaldt.Rows[k], mat, Convert.ToInt32(model.FirstOCRLstModel[j].supplier_id));

                                                                                listItemsAdd2.Add(infoObjAdd);
                                                                            }
                                                                        }


                                                                        HeaderMMOCRData headerinfoObjAdd = CreateHeaderMMOCRData(
                                                                             Convert.ToInt32(model.FirstOCRLstModel[j].supplier_id),
                                                                             model.FirstOCRLstModel[j].supplier_name.ToString(),
                                                                             Convert.ToInt32(model.FirstOCRLstModel[j].material_id),
                                                                             mat,
                                                                             tpm_code,
                                                                             result,
                                                                             r_lot,
                                                                             ocrindicator
                                                                        );

                                                                        HeaderMMOCRData.Add(headerinfoObjAdd);
                                                                        allPagesData.Add(listItemsAdd2);
                                                                    }

                                                                }

                                                            }
                                                            else
                                                            {
                                                                DataTable dtspecval = await dbdal.getKeyInfo("GET_SPECVAL", mat, model.FirstOCRLstModel[j].supplier_id.ToString());
                                                                if (dtspecval != null && dtspecval.Rows.Count > 0)
                                                                {
                                                                    List<MMOCRData> listItemsAdd2 = new List<MMOCRData>();

                                                                    List<KeyInfoData> filteredLotData = KeyInfoData
                                                                            .Where(k => k.lot_no == lot)
                                                                            .ToList();

                                                                    DataTable finaldt = MergeMMandOCR(dtspecval, filteredLotData);

                                                                    if (finaldt != null && finaldt.Rows.Count > 0)
                                                                    {
                                                                        for (int k = 0; k < finaldt.Rows.Count; k++)
                                                                        {
                                                                            if (!string.IsNullOrEmpty(finaldt.Rows[k]["tpm_code"].ToString()))
                                                                            {
                                                                                tpm_code = finaldt.Rows[k]["tpm_code"].ToString();
                                                                            }

                                                                            MMOCRData infoObjAdd = CreateMMOCRData(finaldt.Rows[k], mat, Convert.ToInt32(model.FirstOCRLstModel[j].supplier_id));

                                                                            listItemsAdd2.Add(infoObjAdd);
                                                                        }
                                                                    }

                                                                    result.isExist = "N";
                                                                    result.rejectedID = "";

                                                                    HeaderMMOCRData headerinfoObjAdd = CreateHeaderMMOCRData(
                                                                         Convert.ToInt32(model.FirstOCRLstModel[j].supplier_id),
                                                                         model.FirstOCRLstModel[j].supplier_name.ToString(),
                                                                         Convert.ToInt32(model.FirstOCRLstModel[j].material_id),
                                                                         mat,
                                                                         tpm_code,
                                                                         result,
                                                                         r_lot,
                                                                         ocrindicator
                                                                    );

                                                                    HeaderMMOCRData.Add(headerinfoObjAdd);
                                                                    allPagesData.Add(listItemsAdd2);
                                                                }

                                                            }


                                                        }
                                                    }
                                                    else if (result.isExist == "IND23ALLAPP")
                                                    {
                                                        string[] lotNumbers = r_lot.Split('*');
                                                        foreach (var lot in lotNumbers)
                                                        {
                                                            List<MMOCRData> listItemsAdd2 = new List<MMOCRData>();
                                                            DataTable dt = await dbdal.getKeyInfo("CHECK_LOTNO", lot);
                                                            if (dt != null && dt.Rows.Count > 0)
                                                            {
                                                                for (int k = 0; k < dt.Rows.Count; k++)
                                                                {
                                                                    MMOCRData infoObjAdd = new MMOCRData();

                                                                    infoObjAdd.lot_no = dt.Rows[k]["LOT_NO"] != DBNull.Value ? dt.Rows[k]["LOT_NO"].ToString() : "";
                                                                    infoObjAdd.mat_name = dt.Rows[k]["SUPPLIER_MATERIAL_NAME"] != DBNull.Value ? dt.Rows[k]["SUPPLIER_MATERIAL_NAME"].ToString() : "";
                                                                    infoObjAdd.supplier_id = Convert.ToInt32(dt.Rows[k]["MM_SUPPLIER_H_ID"]);
                                                                    infoObjAdd.mm_spec_type = dt.Rows[k]["SUPPLIER_SPEC_TYPE"] != DBNull.Value ? dt.Rows[k]["SUPPLIER_SPEC_TYPE"].ToString() : "";
                                                                    infoObjAdd.MM_MATERIAL_D2_ID = Convert.ToInt32(dt.Rows[k]["MM_MATERIAL_D2_ID"]);
                                                                    infoObjAdd.mm_spec = dt.Rows[k]["SUPPLIER_SPEC_NAME"] != DBNull.Value ? dt.Rows[k]["SUPPLIER_SPEC_NAME"].ToString() : "";
                                                                    infoObjAdd.mm_spec_val = dt.Rows[k]["SUPPLIER_SPEC_VALUE"] != DBNull.Value ? dt.Rows[k]["SUPPLIER_SPEC_VALUE"].ToString() : "";
                                                                    infoObjAdd.ocr_spec = dt.Rows[k]["OCR_INSPECTION_ITEM"] != DBNull.Value ? dt.Rows[k]["OCR_INSPECTION_ITEM"].ToString() : "";
                                                                    infoObjAdd.ocr_spec_value = dt.Rows[k]["OCR_RESULT"] != DBNull.Value ? dt.Rows[k]["OCR_RESULT"].ToString() : "";

                                                                    infoObjAdd.cfs_ocr_spec = string.IsNullOrEmpty(dt.Rows[k]["OCR_INSPECTION_ITEM_SCORE"].ToString())
                                                                    ? 0m : Math.Round(Convert.ToDecimal(dt.Rows[k]["OCR_INSPECTION_ITEM_SCORE"]), 4);

                                                                    infoObjAdd.cfs_ocr_spec_value = string.IsNullOrEmpty(dt.Rows[k]["OCR_RESULT_SCORE"].ToString())
                                                                    ? 0m : Math.Round(Convert.ToDecimal(dt.Rows[k]["OCR_RESULT_SCORE"]), 4);

                                                                    infoObjAdd.cfs_lot_no = 0;

                                                                    infoObjAdd.compare_result = dt.Rows[k]["COMPARISON_RESULT"] != DBNull.Value ? dt.Rows[k]["COMPARISON_RESULT"].ToString() : "";


                                                                    listItemsAdd2.Add(infoObjAdd);

                                                                }

                                                                result.isExist = dt.Rows[0]["ENTRY_TYPE"] != DBNull.Value ? dt.Rows[0]["ENTRY_TYPE"].ToString() : "";
                                                                r_lot = lot;
                                                                mat = dt.Rows[0]["SUPPLIER_MATERIAL_NAME"] != DBNull.Value ? dt.Rows[0]["SUPPLIER_MATERIAL_NAME"].ToString() : "";
                                                                tpm_code = dt.Rows[0]["TPM_CODE"] != DBNull.Value ? dt.Rows[0]["TPM_CODE"].ToString() : "";
                                                                string supplier = dt.Rows[0]["SUPPLIER_NAME"] != DBNull.Value ? dt.Rows[0]["SUPPLIER_NAME"].ToString() : "";
                                                                int suppid = Convert.ToInt32(dt.Rows[0]["MM_SUPPLIER_H_ID"]);
                                                                int matid = Convert.ToInt32(dt.Rows[0]["MM_MATERIAL_H_ID"]);

                                                                HeaderMMOCRData headerinfoObjAdd = CreateHeaderMMOCRData(
                                                                  suppid,
                                                                  supplier,
                                                                  matid,
                                                                  mat,
                                                                  tpm_code,
                                                                  result,
                                                                  r_lot,
                                                                  ocrindicator
                                                                );

                                                                HeaderMMOCRData.Add(headerinfoObjAdd);
                                                                allPagesData.Add(listItemsAdd2);
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (ocrindicator == "3")
                                                {
                                                    bool hasnewlot = false;
                                                    string combinedlot = "";
                                                    if (result.isExist == "N")
                                                    {
                                                        DataTable dtspecval = await dbdal.getKeyInfo("GET_SPECVAL", mat, model.FirstOCRLstModel[j].supplier_id.ToString());
                                                        if (dtspecval != null && dtspecval.Rows.Count > 0)
                                                        {
                                                            DataTable finaldt = MergeMMandOCR(dtspecval, KeyInfoData);

                                                            if (finaldt != null && finaldt.Rows.Count > 0)
                                                            {
                                                                for (int k = 0; k < finaldt.Rows.Count; k++)
                                                                {
                                                                    if (!string.IsNullOrEmpty(finaldt.Rows[k]["tpm_code"].ToString()))
                                                                    {
                                                                        tpm_code = finaldt.Rows[k]["tpm_code"].ToString();
                                                                    }

                                                                    MMOCRData infoObjAdd = CreateMMOCRData(finaldt.Rows[k], mat, Convert.ToInt32(model.FirstOCRLstModel[j].supplier_id));

                                                                    listItemsAdd.Add(infoObjAdd);
                                                                }
                                                            }


                                                            HeaderMMOCRData headerinfoObjAdd = CreateHeaderMMOCRData(
                                                                 Convert.ToInt32(model.FirstOCRLstModel[j].supplier_id),
                                                                 model.FirstOCRLstModel[j].supplier_name.ToString(),
                                                                 Convert.ToInt32(model.FirstOCRLstModel[j].material_id),
                                                                 mat,
                                                                 tpm_code,
                                                                 result,
                                                                 r_lot,
                                                                 ocrindicator
                                                            );

                                                            HeaderMMOCRData.Add(headerinfoObjAdd);
                                                            allPagesData.Add(listItemsAdd);
                                                        }
                                                    }
                                                    else if (result.isExist == "IND23HASREJ" || result.isExist == "IND23APPNON")
                                                    {
                                                        string[] lotNumbers = r_lot.Split('*');
                                                        foreach (var lot in lotNumbers)
                                                        {
                                                            mat = model.FirstOCRLstModel[j].material_name.ToString();
                                                            bool isrejected = false;
                                                            r_lot = lot;

                                                            DataTable dt = await dbdal.getKeyInfo("CHECK_LOTNO", lot);

                                                            if (dt != null && dt.Rows.Count > 0)
                                                            {
                                                                for (int m = 0; m < dt.Rows.Count; m++)
                                                                {

                                                                    if (dt.Rows[m]["COMPARISON_RESULT"].ToString() == "REJECTED")
                                                                    {
                                                                        isrejected = true;

                                                                        result.isExist = "N";
                                                                        result.rejectedID = dt.Rows[m]["ENTRY_TYPE"] != DBNull.Value ? dt.Rows[m]["ENTRY_TYPE"].ToString() : "";
                                                                        string supplier = model.FirstOCRLstModel[j].supplier_name.ToString();
                                                                        //mat = dt.Rows[m]["SUPPLIER_MATERIAL_NAME"] != DBNull.Value ? dt.Rows[m]["SUPPLIER_MATERIAL_NAME"].ToString() : "";
                                                                        //tpm_code = dt.Rows[m]["TPM_CODE"] != DBNull.Value ? dt.Rows[m]["TPM_CODE"].ToString() : "";
                                                                        int suppid = Convert.ToInt32(model.FirstOCRLstModel[j].supplier_id);
                                                                        //int matid = Convert.ToInt32(dt.Rows[m]["MM_MATERIAL_H_ID"]);

                                                                        DataTable dtspecval = await dbdal.getKeyInfo("GET_SPECVAL", mat, suppid.ToString());
                                                                        if (dtspecval != null && dtspecval.Rows.Count > 0)
                                                                        {
                                                                            //List<KeyInfoData> filteredLotData = KeyInfoData
                                                                            //.Where(k => k.lot_no == lot)
                                                                            //.ToList();

                                                                            List<MMOCRData> listItemsAdd2 = new List<MMOCRData>();

                                                                            DataTable finaldt = MergeMMandOCR(dtspecval, KeyInfoData);

                                                                            if (finaldt != null && finaldt.Rows.Count > 0)
                                                                            {
                                                                                for (int k = 0; k < finaldt.Rows.Count; k++)
                                                                                {
                                                                                    if (!string.IsNullOrEmpty(finaldt.Rows[k]["tpm_code"].ToString()))
                                                                                    {
                                                                                        tpm_code = finaldt.Rows[k]["tpm_code"].ToString();
                                                                                    }

                                                                                    MMOCRData infoObjAdd = CreateMMOCRData(finaldt.Rows[k], mat, Convert.ToInt32(model.FirstOCRLstModel[j].supplier_id));

                                                                                    listItemsAdd2.Add(infoObjAdd);
                                                                                }
                                                                            }

                                                                            HeaderMMOCRData headerinfoObjAdd = CreateHeaderMMOCRData(
                                                                              suppid,
                                                                              supplier,
                                                                              Convert.ToInt32(model.FirstOCRLstModel[j].material_id),
                                                                              mat,
                                                                              tpm_code,
                                                                              result,
                                                                              r_lot,
                                                                              ocrindicator
                                                                            );

                                                                            HeaderMMOCRData.Add(headerinfoObjAdd);
                                                                            allPagesData.Add(listItemsAdd2);
                                                                        }

                                                                        break;
                                                                    }
                                                                }

                                                                if (!isrejected)  // all approve
                                                                {
                                                                    //comment 020725 -- user request to capture ocr even exist and approved
                                                                    //List<MMOCRData> listItemsAdd2 = new List<MMOCRData>();
                                                                    //for (int k = 0; k < dt.Rows.Count; k++)
                                                                    //{
                                                                    //    MMOCRData infoObjAdd = new MMOCRData();

                                                                    //    infoObjAdd.lot_no = dt.Rows[k]["LOT_NO"] != DBNull.Value ? dt.Rows[k]["LOT_NO"].ToString() : "";
                                                                    //    infoObjAdd.mat_name = dt.Rows[k]["SUPPLIER_MATERIAL_NAME"] != DBNull.Value ? dt.Rows[k]["SUPPLIER_MATERIAL_NAME"].ToString() : "";
                                                                    //    infoObjAdd.supplier_id = Convert.ToInt32(dt.Rows[k]["MM_SUPPLIER_H_ID"]);
                                                                    //    infoObjAdd.mm_spec_type = dt.Rows[k]["SUPPLIER_SPEC_TYPE"] != DBNull.Value ? dt.Rows[k]["SUPPLIER_SPEC_TYPE"].ToString() : "";
                                                                    //    infoObjAdd.MM_MATERIAL_D2_ID = Convert.ToInt32(dt.Rows[k]["MM_MATERIAL_D2_ID"]);
                                                                    //    infoObjAdd.mm_spec = dt.Rows[k]["SUPPLIER_SPEC_NAME"] != DBNull.Value ? dt.Rows[k]["SUPPLIER_SPEC_NAME"].ToString() : "";
                                                                    //    infoObjAdd.mm_spec_val = dt.Rows[k]["SUPPLIER_SPEC_VALUE"] != DBNull.Value ? dt.Rows[k]["SUPPLIER_SPEC_VALUE"].ToString() : "";
                                                                    //    infoObjAdd.ocr_spec = dt.Rows[k]["OCR_INSPECTION_ITEM"] != DBNull.Value ? dt.Rows[k]["OCR_INSPECTION_ITEM"].ToString() : "";
                                                                    //    infoObjAdd.ocr_spec_value = dt.Rows[k]["OCR_RESULT"] != DBNull.Value ? dt.Rows[k]["OCR_RESULT"].ToString() : "";

                                                                    //    infoObjAdd.cfs_ocr_spec = string.IsNullOrEmpty(dt.Rows[k]["OCR_INSPECTION_ITEM_SCORE"].ToString())
                                                                    //    ? 0m : Math.Round(Convert.ToDecimal(dt.Rows[k]["OCR_INSPECTION_ITEM_SCORE"]), 4);

                                                                    //    infoObjAdd.cfs_ocr_spec_value = string.IsNullOrEmpty(dt.Rows[k]["OCR_RESULT_SCORE"].ToString())
                                                                    //    ? 0m : Math.Round(Convert.ToDecimal(dt.Rows[k]["OCR_RESULT_SCORE"]), 4);

                                                                    //    infoObjAdd.cfs_lot_no = 0;

                                                                    //    infoObjAdd.compare_result = dt.Rows[k]["COMPARISON_RESULT"] != DBNull.Value ? dt.Rows[k]["COMPARISON_RESULT"].ToString() : "";


                                                                    //    listItemsAdd2.Add(infoObjAdd);

                                                                    //}

                                                                    //HeaderMMOCRData headerinfoObjAdd = CreateHeaderMMOCRData(
                                                                    //  suppid,
                                                                    //  supplier,
                                                                    //  matid,
                                                                    //  mat,
                                                                    //  tpm_code,
                                                                    //  result,
                                                                    //  r_lot,
                                                                    //  ocrindicator
                                                                    //);

                                                                    //HeaderMMOCRData.Add(headerinfoObjAdd);
                                                                    //allPagesData.Add(listItemsAdd2);


                                                                    result.isExist = dt.Rows[0]["ENTRY_TYPE"] != DBNull.Value ? dt.Rows[0]["ENTRY_TYPE"].ToString() : "";
                                                                    result.rejectedID = "";
                                                                    //mat = dt.Rows[0]["SUPPLIER_MATERIAL_NAME"] != DBNull.Value ? dt.Rows[0]["SUPPLIER_MATERIAL_NAME"].ToString() : "";
                                                                    //tpm_code = dt.Rows[0]["TPM_CODE"] != DBNull.Value ? dt.Rows[0]["TPM_CODE"].ToString() : "";
                                                                    //string supplier = dt.Rows[0]["SUPPLIER_NAME"] != DBNull.Value ? dt.Rows[0]["SUPPLIER_NAME"].ToString() : "";
                                                                    //int suppid = Convert.ToInt32(dt.Rows[0]["MM_SUPPLIER_H_ID"]);
                                                                    //int matid = Convert.ToInt32(dt.Rows[0]["MM_MATERIAL_H_ID"]);

                                                                    //DataTable dtspecval = await dbdal.getKeyInfo("GET_SPECVAL", mat, suppid.ToString());
                                                                    DataTable dtspecval = await dbdal.getKeyInfo("GET_SPECVAL", mat, model.FirstOCRLstModel[j].supplier_id.ToString());
                                                                    if (dtspecval != null && dtspecval.Rows.Count > 0)
                                                                    {
                                                                        //List<KeyInfoData> filteredLotData = KeyInfoData
                                                                        //.Where(k => k.lot_no == lot)
                                                                        //.ToList();

                                                                        List<MMOCRData> listItemsAdd2 = new List<MMOCRData>();

                                                                        DataTable finaldt = MergeMMandOCR(dtspecval, KeyInfoData);

                                                                        if (finaldt != null && finaldt.Rows.Count > 0)
                                                                        {
                                                                            for (int k = 0; k < finaldt.Rows.Count; k++)
                                                                            {
                                                                                if (!string.IsNullOrEmpty(finaldt.Rows[k]["tpm_code"].ToString()))
                                                                                {
                                                                                    tpm_code = finaldt.Rows[k]["tpm_code"].ToString();
                                                                                }

                                                                                MMOCRData infoObjAdd = CreateMMOCRData(finaldt.Rows[k], mat, Convert.ToInt32(model.FirstOCRLstModel[j].supplier_id));

                                                                                listItemsAdd2.Add(infoObjAdd);
                                                                            }
                                                                        }

                                                                        HeaderMMOCRData headerinfoObjAdd = CreateHeaderMMOCRData(
                                                                             Convert.ToInt32(model.FirstOCRLstModel[j].supplier_id),
                                                                             model.FirstOCRLstModel[j].supplier_name.ToString(),
                                                                             Convert.ToInt32(model.FirstOCRLstModel[j].material_id),
                                                                             mat,
                                                                             tpm_code,
                                                                             result,
                                                                             r_lot,
                                                                             ocrindicator
                                                                        );

                                                                        HeaderMMOCRData.Add(headerinfoObjAdd);
                                                                        allPagesData.Add(listItemsAdd2);
                                                                    }
                                                                }

                                                            }
                                                            else
                                                            {
                                                                hasnewlot = true;
                                                                if (combinedlot != "")
                                                                {
                                                                    combinedlot = combinedlot + "*" + r_lot;
                                                                }
                                                                else
                                                                {
                                                                    combinedlot = r_lot;
                                                                }

                                                            }


                                                        }

                                                        if (hasnewlot) // has some lot nonexist yet
                                                        {
                                                            mat = model.FirstOCRLstModel[j].material_name.ToString();
                                                            r_lot = combinedlot;
                                                            DataTable dtspecval = await dbdal.getKeyInfo("GET_SPECVAL", mat, model.FirstOCRLstModel[j].supplier_id.ToString());
                                                            if (dtspecval != null && dtspecval.Rows.Count > 0)
                                                            {

                                                                DataTable finaldt = MergeMMandOCR(dtspecval, KeyInfoData);

                                                                if (finaldt != null && finaldt.Rows.Count > 0)
                                                                {
                                                                    for (int k = 0; k < finaldt.Rows.Count; k++)
                                                                    {
                                                                        if (!string.IsNullOrEmpty(finaldt.Rows[k]["tpm_code"].ToString()))
                                                                        {
                                                                            tpm_code = finaldt.Rows[k]["tpm_code"].ToString();
                                                                        }

                                                                        MMOCRData infoObjAdd = CreateMMOCRData(finaldt.Rows[k], mat, Convert.ToInt32(model.FirstOCRLstModel[j].supplier_id));

                                                                        listItemsAdd.Add(infoObjAdd);
                                                                    }
                                                                }

                                                                result.isExist = "N";
                                                                result.rejectedID = "";

                                                                HeaderMMOCRData headerinfoObjAdd = CreateHeaderMMOCRData(
                                                                     Convert.ToInt32(model.FirstOCRLstModel[j].supplier_id),
                                                                     model.FirstOCRLstModel[j].supplier_name.ToString(),
                                                                     Convert.ToInt32(model.FirstOCRLstModel[j].material_id),
                                                                     mat,
                                                                     tpm_code,
                                                                     result,
                                                                     r_lot,
                                                                     ocrindicator
                                                                );

                                                                HeaderMMOCRData.Add(headerinfoObjAdd);
                                                                allPagesData.Add(listItemsAdd);
                                                            }
                                                        }
                                                    }
                                                    else if (result.isExist == "IND23ALLAPP")
                                                    {
                                                        string[] lotNumbers = r_lot.Split('*');
                                                        foreach (var lot in lotNumbers)
                                                        {
                                                            List<MMOCRData> listItemsAdd2 = new List<MMOCRData>();
                                                            DataTable dt = await dbdal.getKeyInfo("CHECK_LOTNO", lot);
                                                            if (dt != null && dt.Rows.Count > 0)
                                                            {
                                                                for (int k = 0; k < dt.Rows.Count; k++)
                                                                {
                                                                    MMOCRData infoObjAdd = new MMOCRData();

                                                                    infoObjAdd.lot_no = dt.Rows[k]["LOT_NO"] != DBNull.Value ? dt.Rows[k]["LOT_NO"].ToString() : "";
                                                                    infoObjAdd.mat_name = dt.Rows[k]["SUPPLIER_MATERIAL_NAME"] != DBNull.Value ? dt.Rows[k]["SUPPLIER_MATERIAL_NAME"].ToString() : "";
                                                                    infoObjAdd.supplier_id = Convert.ToInt32(dt.Rows[k]["MM_SUPPLIER_H_ID"]);
                                                                    infoObjAdd.mm_spec_type = dt.Rows[k]["SUPPLIER_SPEC_TYPE"] != DBNull.Value ? dt.Rows[k]["SUPPLIER_SPEC_TYPE"].ToString() : "";
                                                                    infoObjAdd.MM_MATERIAL_D2_ID = Convert.ToInt32(dt.Rows[k]["MM_MATERIAL_D2_ID"]);
                                                                    infoObjAdd.mm_spec = dt.Rows[k]["SUPPLIER_SPEC_NAME"] != DBNull.Value ? dt.Rows[k]["SUPPLIER_SPEC_NAME"].ToString() : "";
                                                                    infoObjAdd.mm_spec_val = dt.Rows[k]["SUPPLIER_SPEC_VALUE"] != DBNull.Value ? dt.Rows[k]["SUPPLIER_SPEC_VALUE"].ToString() : "";
                                                                    infoObjAdd.ocr_spec = dt.Rows[k]["OCR_INSPECTION_ITEM"] != DBNull.Value ? dt.Rows[k]["OCR_INSPECTION_ITEM"].ToString() : "";
                                                                    infoObjAdd.ocr_spec_value = dt.Rows[k]["OCR_RESULT"] != DBNull.Value ? dt.Rows[k]["OCR_RESULT"].ToString() : "";

                                                                    infoObjAdd.cfs_ocr_spec = string.IsNullOrEmpty(dt.Rows[k]["OCR_INSPECTION_ITEM_SCORE"].ToString())
                                                                    ? 0m : Math.Round(Convert.ToDecimal(dt.Rows[k]["OCR_INSPECTION_ITEM_SCORE"]), 4);

                                                                    infoObjAdd.cfs_ocr_spec_value = string.IsNullOrEmpty(dt.Rows[k]["OCR_RESULT_SCORE"].ToString())
                                                                    ? 0m : Math.Round(Convert.ToDecimal(dt.Rows[k]["OCR_RESULT_SCORE"]), 4);

                                                                    infoObjAdd.cfs_lot_no = 0;

                                                                    infoObjAdd.compare_result = dt.Rows[k]["COMPARISON_RESULT"] != DBNull.Value ? dt.Rows[k]["COMPARISON_RESULT"].ToString() : "";


                                                                    listItemsAdd2.Add(infoObjAdd);

                                                                }

                                                                result.isExist = dt.Rows[0]["ENTRY_TYPE"] != DBNull.Value ? dt.Rows[0]["ENTRY_TYPE"].ToString() : "";
                                                                r_lot = lot;
                                                                mat = dt.Rows[0]["SUPPLIER_MATERIAL_NAME"] != DBNull.Value ? dt.Rows[0]["SUPPLIER_MATERIAL_NAME"].ToString() : "";
                                                                tpm_code = dt.Rows[0]["TPM_CODE"] != DBNull.Value ? dt.Rows[0]["TPM_CODE"].ToString() : "";
                                                                string supplier = dt.Rows[0]["SUPPLIER_NAME"] != DBNull.Value ? dt.Rows[0]["SUPPLIER_NAME"].ToString() : "";
                                                                int suppid = Convert.ToInt32(dt.Rows[0]["MM_SUPPLIER_H_ID"]);
                                                                int matid = Convert.ToInt32(dt.Rows[0]["MM_MATERIAL_H_ID"]);

                                                                HeaderMMOCRData headerinfoObjAdd = CreateHeaderMMOCRData(
                                                                  suppid,
                                                                  supplier,
                                                                  matid,
                                                                  mat,
                                                                  tpm_code,
                                                                  result,
                                                                  r_lot,
                                                                  ocrindicator
                                                                );

                                                                HeaderMMOCRData.Add(headerinfoObjAdd);
                                                                allPagesData.Add(listItemsAdd2);
                                                            }
                                                        }
                                                    }
                                                }
                                                else // ocr indicator 1
                                                {
                                                    //if (result.isExist == "N")
                                                    //{
                                                        DataTable dtspecval = await dbdal.getKeyInfo("GET_SPECVAL", mat, model.FirstOCRLstModel[j].supplier_id.ToString());
                                                        if (dtspecval != null && dtspecval.Rows.Count > 0)
                                                        {
                                                            DataTable finaldt = MergeMMandOCR(dtspecval, KeyInfoData);

                                                            if (finaldt != null && finaldt.Rows.Count > 0)
                                                            {
                                                                for (int k = 0; k < finaldt.Rows.Count; k++)
                                                                {
                                                                    if (!string.IsNullOrEmpty(finaldt.Rows[k]["tpm_code"].ToString()))
                                                                    {
                                                                        tpm_code = finaldt.Rows[k]["tpm_code"].ToString();
                                                                    }

                                                                    MMOCRData infoObjAdd = CreateMMOCRData(finaldt.Rows[k], mat, Convert.ToInt32(model.FirstOCRLstModel[j].supplier_id));

                                                                    listItemsAdd.Add(infoObjAdd);
                                                                }
                                                            }

                                                            HeaderMMOCRData headerinfoObjAdd = CreateHeaderMMOCRData(
                                                                 Convert.ToInt32(model.FirstOCRLstModel[j].supplier_id),
                                                                 model.FirstOCRLstModel[j].supplier_name.ToString(),
                                                                 Convert.ToInt32(model.FirstOCRLstModel[j].material_id),
                                                                 mat,
                                                                 tpm_code,
                                                                 result,
                                                                 r_lot,
                                                                 ocrindicator
                                                            );

                                                            HeaderMMOCRData.Add(headerinfoObjAdd);
                                                            allPagesData.Add(listItemsAdd);
                                                        }
                                                    //}
                                                    //else //all lot approved
                                                    //{
                                                    //    //comment 020725 -- user request to capture ocr even exist and approved
                                                    //    DataTable dt = await dbdal.getKeyInfo("CHECK_LOTNO", r_lot);
                                                    //    if (dt != null && dt.Rows.Count > 0)
                                                    //    {
                                                    //        for (int k = 0; k < dt.Rows.Count; k++)
                                                    //        {
                                                    //            MMOCRData infoObjAdd = new MMOCRData();

                                                    //            infoObjAdd.lot_no = dt.Rows[k]["LOT_NO"] != DBNull.Value ? dt.Rows[k]["LOT_NO"].ToString() : "";
                                                    //            infoObjAdd.mat_name = mat;
                                                    //            infoObjAdd.supplier_id = Convert.ToInt32(model.FirstOCRLstModel[j].supplier_id);
                                                    //            infoObjAdd.mm_spec_type = dt.Rows[k]["SUPPLIER_SPEC_TYPE"] != DBNull.Value ? dt.Rows[k]["SUPPLIER_SPEC_TYPE"].ToString() : "";
                                                    //            infoObjAdd.MM_MATERIAL_D2_ID = Convert.ToInt32(dt.Rows[k]["MM_MATERIAL_D2_ID"]);
                                                    //            infoObjAdd.mm_spec = dt.Rows[k]["SUPPLIER_SPEC_NAME"] != DBNull.Value ? dt.Rows[k]["SUPPLIER_SPEC_NAME"].ToString() : "";
                                                    //            infoObjAdd.mm_spec_val = dt.Rows[k]["SUPPLIER_SPEC_VALUE"] != DBNull.Value ? dt.Rows[k]["SUPPLIER_SPEC_VALUE"].ToString() : "";
                                                    //            infoObjAdd.ocr_spec = dt.Rows[k]["OCR_INSPECTION_ITEM"] != DBNull.Value ? dt.Rows[k]["OCR_INSPECTION_ITEM"].ToString() : "";
                                                    //            infoObjAdd.ocr_spec_value = dt.Rows[k]["OCR_RESULT"] != DBNull.Value ? dt.Rows[k]["OCR_RESULT"].ToString() : "";

                                                    //            infoObjAdd.cfs_ocr_spec = string.IsNullOrEmpty(dt.Rows[k]["OCR_INSPECTION_ITEM_SCORE"].ToString())
                                                    //            ? 0m : Math.Round(Convert.ToDecimal(dt.Rows[k]["OCR_INSPECTION_ITEM_SCORE"]), 4);

                                                    //            infoObjAdd.cfs_ocr_spec_value = string.IsNullOrEmpty(dt.Rows[k]["OCR_RESULT_SCORE"].ToString())
                                                    //            ? 0m : Math.Round(Convert.ToDecimal(dt.Rows[k]["OCR_RESULT_SCORE"]), 4);

                                                    //            infoObjAdd.cfs_lot_no = 0;

                                                    //            infoObjAdd.compare_result = dt.Rows[k]["COMPARISON_RESULT"] != DBNull.Value ? dt.Rows[k]["COMPARISON_RESULT"].ToString() : "";


                                                    //            listItemsAdd.Add(infoObjAdd);

                                                    //        }

                                                    //        mat = dt.Rows[0]["SUPPLIER_MATERIAL_NAME"] != DBNull.Value ? dt.Rows[0]["SUPPLIER_MATERIAL_NAME"].ToString() : "";
                                                    //        tpm_code = dt.Rows[0]["TPM_CODE"] != DBNull.Value ? dt.Rows[0]["TPM_CODE"].ToString() : "";
                                                    //        string supplier = dt.Rows[0]["SUPPLIER_NAME"] != DBNull.Value ? dt.Rows[0]["SUPPLIER_NAME"].ToString() : "";
                                                    //        int suppid = Convert.ToInt32(dt.Rows[0]["MM_SUPPLIER_H_ID"]);
                                                    //        int matid = Convert.ToInt32(dt.Rows[0]["MM_MATERIAL_H_ID"]);

                                                    //        HeaderMMOCRData headerinfoObjAdd = CreateHeaderMMOCRData(
                                                    //          suppid,
                                                    //          supplier,
                                                    //          matid,
                                                    //          mat,
                                                    //          tpm_code,
                                                    //          result,
                                                    //          r_lot,
                                                    //          ocrindicator
                                                    //        );

                                                    //        HeaderMMOCRData.Add(headerinfoObjAdd);
                                                    //        allPagesData.Add(listItemsAdd);
                                                    //    }
                                                    //}
                                                }

                                            }
                                        }

                                        #endregion
                                    }
                                    catch (InvalidOperationException ex)
                                    {
                                        return Json(new { success = false, message = "Error in PDF processing: " });

                                    }
                                }
                            }

                            document.Close();
                            COAOCRModel finalResult = new COAOCRModel
                            {
                                HeaderMMOCRData = HeaderMMOCRData,
                                MMOCRData = allPagesData,
                                ocrindicator = ocrindicator
                            };
                            return Json(new { success = true, filename = DataUploadFile.FileName, d1 = finalResult });
                        }
                        finally
                        {
                            if (document.IsOpen())
                                document.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Error in PDF processing. " });
                }


            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }



            #endregion
        }


        private async Task<ProcessPageResult> ProcessPage(byte[] pdfPage, int indexpage, string modelObj, string ocrindicator, string coalang = "english")
        {
            bool notexist = true;
            string existlotnoid = "N";
            string rejectedid = "";
            bool isrejected = false;
            var result = "";

            var model = JsonConvert.DeserializeObject<COAOCRModel>(modelObj);
            List<KeyInfoData> extractedData = new List<KeyInfoData>();
            try
            {

                if (model.FirstOCRLstModel != null && model.FirstOCRLstModel.Count > 0)
                {
                    for (int j = 0; j < model.FirstOCRLstModel.Count; j++)
                    {
                        if (j == indexpage)
                        {
                            string lotno = model.FirstOCRLstModel[j].lot_no.ToString();

                            if (ocrindicator == "2" || ocrindicator == "3")
                            {
                                DataTable dt_lot = await dbdal.getKeyInfo("CHECK_ALLLOTNO", lotno);

                                if (dt_lot != null && dt_lot.Rows.Count > 0)
                                {
                                    for (int m = 0; m < dt_lot.Rows.Count; m++)
                                    {
                                        if (dt_lot.Rows[m]["COMPARISON_RESULT"].ToString() == "REJECTED")
                                        {
                                            // has rejected so call ocr
                                            isrejected = true;
                                            existlotnoid = "IND23HASREJ";
                                            (extractedData, result) = await ExtractOCR(pdfPage, indexpage, model.FirstOCRLstModel[j].supplier_id.ToString(), coalang, ocrindicator, lotno, model.FirstOCRLstModel[j].material_id.ToString());

                                            return new ProcessPageResult
                                            {
                                                PdfPage = pdfPage,
                                                rejectedID = rejectedid,
                                                isExist = existlotnoid,
                                                KeyInfoData = extractedData,
                                                resultfromOCR = result
                                            };
                                        }
                                    }

                                    if (!isrejected)  // all approve so skip
                                    {
                                        existlotnoid = "IND23APPNON";
                                        (extractedData, result) = await ExtractOCR(pdfPage, indexpage, model.FirstOCRLstModel[j].supplier_id.ToString(), coalang, ocrindicator, lotno, model.FirstOCRLstModel[j].material_id.ToString());

                                        return new ProcessPageResult
                                        {
                                            PdfPage = pdfPage,
                                            rejectedID = rejectedid,
                                            isExist = existlotnoid,
                                            KeyInfoData = extractedData,
                                            resultfromOCR = result
                                        };


                                        ////check if there is non-exist lot
                                        //var lotList = lotno.Split('*').Distinct().ToList();

                                        //var existingLots = new HashSet<string>(
                                        //    dt_lot.AsEnumerable()
                                        //          .Select(row => row.Field<string>("LOT_NO"))
                                        //          .Where(x => !string.IsNullOrEmpty(x))
                                        //);


                                        //var missingLots = lotList.Where(l => !existingLots.Contains(l)).ToList();

                                        //if (missingLots.Count > 0)
                                        //{
                                        //    existlotnoid = "IND23APPNON";
                                        //    (extractedData, result) = await ExtractOCR(pdfPage, indexpage, model.FirstOCRLstModel[j].supplier_id.ToString(), coalang, ocrindicator, lotno, model.FirstOCRLstModel[j].material_id.ToString());

                                        //    return new ProcessPageResult
                                        //    {
                                        //        PdfPage = pdfPage,
                                        //        rejectedID = rejectedid,
                                        //        isExist = existlotnoid,
                                        //        KeyInfoData = extractedData,
                                        //        resultfromOCR = result
                                        //    };
                                        //}
                                        //else
                                        //{
                                        //    existlotnoid = "IND23ALLAPP";
                                        //    notexist = false;

                                        //    return new ProcessPageResult
                                        //    {
                                        //        PdfPage = pdfPage,
                                        //        rejectedID = rejectedid,
                                        //        isExist = existlotnoid,
                                        //        KeyInfoData = extractedData,
                                        //        resultfromOCR = result
                                        //    };
                                        //}


                                    }
                                }
                                else // all not exist so call ocr
                                {
                                    (extractedData, result) = await ExtractOCR(pdfPage, indexpage, model.FirstOCRLstModel[j].supplier_id.ToString(), coalang, ocrindicator, lotno, model.FirstOCRLstModel[j].material_id.ToString());

                                    return new ProcessPageResult
                                    {
                                        PdfPage = pdfPage,
                                        rejectedID = rejectedid,
                                        isExist = existlotnoid,
                                        KeyInfoData = extractedData,
                                        resultfromOCR = result
                                    };
                                }
                            }
                            else //ocrindicator == "1"
                            {
                                DataTable dt_lot = await dbdal.getKeyInfo("CHECK_LOTNO", lotno);

                                if (dt_lot != null && dt_lot.Rows.Count > 0)
                                {
                                    for (int m = 0; m < dt_lot.Rows.Count; m++)
                                    {
                                        if (dt_lot.Rows[m]["COMPARISON_RESULT"].ToString() == "REJECTED")
                                        {
                                            isrejected = true;
                                            rejectedid = dt_lot.Rows[0]["ENTRY_TYPE"] != DBNull.Value ? dt_lot.Rows[0]["ENTRY_TYPE"].ToString() : "";
                                            break;
                                        }
                                    }

                                    if (!isrejected)  // all approve
                                    {
                                        existlotnoid = dt_lot.Rows[0]["ENTRY_TYPE"] != DBNull.Value ? dt_lot.Rows[0]["ENTRY_TYPE"].ToString() : "";
                                        //notexist = false;

                                        //return new ProcessPageResult
                                        //{
                                        //    PdfPage = pdfPage,
                                        //    rejectedID = rejectedid,
                                        //    isExist = existlotnoid,
                                        //    KeyInfoData = extractedData,
                                        //    resultfromOCR = result
                                        //};
                                    }
                                }

                                if (notexist)
                                {
                                    (extractedData, result) = await ExtractOCR(pdfPage, indexpage, model.FirstOCRLstModel[j].supplier_id.ToString(), coalang, ocrindicator, lotno, model.FirstOCRLstModel[j].material_id.ToString());

                                    return new ProcessPageResult
                                    {
                                        PdfPage = pdfPage,
                                        rejectedID = rejectedid,
                                        isExist = existlotnoid,
                                        KeyInfoData = extractedData,
                                        resultfromOCR = result
                                    };
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }

            return new ProcessPageResult
            {
                PdfPage = pdfPage,
                rejectedID = rejectedid,
                isExist = existlotnoid,
                KeyInfoData = extractedData,
                resultfromOCR = result
            };
        }

        private async Task<(List<KeyInfoData> extractedData, string result)> ExtractOCR(byte[] pdfPage, int indexpage, string supplier, string coalang, string ocrindicator, string conflot, string matid)
        {
            HttpClientHandler handler = new HttpClientHandler();

            AppConfig config = GetAppConfig();
            if (config == null)
            {
                throw new InvalidOperationException("Configuration error.");
            }

            string result = "";
            List<KeyInfoData> extractedData = new List<KeyInfoData>();

            MultipartFormDataContent formData = new MultipartFormDataContent();
            formData.Add(new StringContent("lot_no"), "Key_1");

            //get key info
            DataTable dt = new DataTable();

            if (ocrindicator == "2")
            {
                dt = await dbdal.getKeyInfo("SUP_KEY_INFO_2", supplier, coalang, matid);
                //dt = await dbdal.getKeyInfo("MAT_KEY_INFO2", supplier, coalang, matid);
            }
            else
            {
                dt = await dbdal.getKeyInfo("MAT_KEY_INFO", supplier, coalang, matid);
            }

            string fkeyinfo = "";
            int keynum = 2;
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    fkeyinfo = dt.Rows[i]["KEY_INFO"] != DBNull.Value ? dt.Rows[i]["KEY_INFO"].ToString() : "";

                    //HANA 18112025 Modify to specify all lots to cater to missing lots needing manual keyin
                    if (fkeyinfo.ToLower().Contains("get all lot no and"))
                    {
                        fkeyinfo = fkeyinfo + " in " + conflot.Replace("*", ",");
                    }

                    formData.Add(new StringContent(fkeyinfo), "Key_" + keynum.ToString());
                    keynum++;
                }
            }

            using (HttpClient client = new HttpClient(handler) { Timeout = TimeSpan.FromMinutes(6) })
            using (MemoryStream pdfStream = new MemoryStream(pdfPage))
            {
                client.Timeout = TimeSpan.FromMinutes(6);

                StreamContent fileContent = new StreamContent(pdfStream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");

                formData.Add(fileContent, "image", $"page_{indexpage + 1}.pdf");
                formData.Add(new StringContent(config.TMSUserAPIKey), "apiKey");
                formData.Add(new StringContent(coalang), "language");

                //for testing -- need uncomment

                //Send request to API
                HttpResponseMessage response = await client.PostAsync(config.OCREndpoint, formData);
                string responseString = await response.Content.ReadAsStringAsync();
                //string responseString = "";

                if (response.IsSuccessStatusCode)
                //if (result =="")
                {
                    //result = "{\"key_info\":[{\"Get all lot no and analysis result for each lot\":[{\"Lot 27LQ5\":\"18.4\",\"Lot 30MQ7\":\"19.8\",\"Method\":\"sIr\",\"Parameter\":\"Loss on Drying\",\"Specification Max\":\"20\",\"Specification Min\":\"N/A\",\"UOM\":\"%\"},{\"Lot 27LQ5\":\"89.3\",\"Lot 30MQ7\":\"89.0\",\"Method\":\"JIS\",\"Parameter\":\"Caramel DP\",\"Specification Max\":\"N/A\",\"Specification Min\":\"88\",\"UOM\":\"%\"},{\"Lot 27LQ5\":\"10.9\",\"Lot 30MQ7\":\"10.0\",\"Method\":\"JIS\",\"Parameter\":\"pH\",\"Specification Max\":\"12\",\"Specification Min\":\"9\",\"UOM\":\"N/A\"},{\"Lot 27LQ5\":\"7.2\",\"Lot 30MQ7\":\"5.4\",\"Method\":\"JIS\",\"Parameter\":\"Ignition Residue\",\"Specification Max\":\"8\",\"Specification Min\":\"N/A\",\"UOM\":\"%\"},{\"Lot 27LQ5\":\"15.9\",\"Lot 30MQ7\":\"14.7\",\"Method\":\"JWWA\",\"Parameter\":\"On 200-mesh\",\"Specification Max\":\"20\",\"Specification Min\":\"N/A\",\"UOM\":\"% by mass\"},{\"Lot 27LQ5\":\"1.0\",\"Lot 30MQ7\":\"1.0\",\"Method\":\"JWWA\",\"Parameter\":\"Arsenic\",\"Specification Max\":\"5\",\"Specification Min\":\"N/A\",\"UOM\":\"ppm\"},{\"Lot 27LQ5\":\"960\",\"Lot 30MQ7\":\"950\",\"Method\":\"JWWA\",\"Parameter\":\"Iodine AP\",\"Specification Max\":\"N/A\",\"Specification Min\":\"930\",\"UOM\":\"mg/g\"}],\"lot_no\":[\"27LQ5\",\"30MQ7\"]}],\"ocr\":[[{\"confidence_score\":0.9762595295906067,\"text\":\"CENTURY CHEMICALWORKS SDN.BHD.(9139-W)\"},{\"confidence_score\":0.9073800444602966,\"text\":\"MALBON Simply Unique.\"},{\"confidence_score\":0.9899501204490662,\"text\":\"Certificate of Analysis\"},{\"confidence_score\":0.9972765445709229,\"text\":\"Customer\"},{\"confidence_score\":0.9563083648681641,\"text\":\"Toray Plastic Malaysia Sdn.Bhd.\"},{\"confidence_score\":0.9986993670463562,\"text\":\"Grade\"},{\"confidence_score\":0.9656727910041809,\"text\":\"S5W20\"},{\"confidence_score\":0.9866113066673279,\"text\":\"Shipment No.\"},{\"confidence_score\":0.978908121585846,\"text\":\"2025-06-CCB-106\"},{\"confidence_score\":0.9968275427818298,\"text\":\"Product\"},{\"confidence_score\":0.989020586013794,\"text\":\"Activated Carbon\"},{\"confidence_score\":0.9797061085700989,\"text\":\"Container No.\"},{\"confidence_score\":0.9772786498069763,\"text\":\"N/A\"},{\"confidence_score\":0.9412588477134705,\"text\":\"Date Issued\"},{\"confidence_score\":0.9888730645179749,\"text\":\"09-Jun-2025\"},{\"confidence_score\":0.9712868928909302,\"text\":\"Total Quantity\"},{\"confidence_score\":0.9875236749649048,\"text\":\"10\"},{\"confidence_score\":0.9506224989891052,\"text\":\"bags x500kg\"},{\"confidence_score\":0.9723523259162903,\"text\":\"Revision No.\"},{\"confidence_score\":0.9553823471069336,\"text\":\":0\"},{\"confidence_score\":0.8835700154304504,\"text\":\"PO No.\"},{\"confidence_score\":0.9950067400932312,\"text\":\"125497\"},{\"confidence_score\":0.9269091486930847,\"text\":\"Lot No.\"},{\"confidence_score\":0.9888975024223328,\"text\":\"UOM\"},{\"confidence_score\":0.9972281455993652,\"text\":\"Method\"},{\"confidence_score\":0.9975413084030151,\"text\":\"Specification\"},{\"confidence_score\":0.996679961681366,\"text\":\"Parameter\"},{\"confidence_score\":0.9966878890991211,\"text\":\"27LQ5\"},{\"confidence_score\":0.9942910075187683,\"text\":\"30MQ7\"},{\"confidence_score\":0.9987037777900696,\"text\":\"Min\"},{\"confidence_score\":0.9962282180786133,\"text\":\"Max\"},{\"confidence_score\":0.9649051427841187,\"text\":\"Loss on Drying\"},{\"confidence_score\":0.9973260164260864,\"text\":\"%\"},{\"confidence_score\":0.7750868201255798,\"text\":\"sIr\"},{\"confidence_score\":0.9976112842559814,\"text\":\"18.4\"},{\"confidence_score\":0.9967796206474304,\"text\":\"19.8\"},{\"confidence_score\":0.9974236488342285,\"text\":\"20\"},{\"confidence_score\":0.9570950269699097,\"text\":\"Caramel DP\"},{\"confidence_score\":0.9973673224449158,\"text\":\"%\"},{\"confidence_score\":0.9935832023620605,\"text\":\"JIS\"},{\"confidence_score\":0.9957271218299866,\"text\":\"89.3\"},{\"confidence_score\":0.9914940595626831,\"text\":\"89.0\"},{\"confidence_score\":0.9986171722412109,\"text\":\"88\"},{\"confidence_score\":0.9327272772789001,\"text\":\"pH\"},{\"confidence_score\":0.8570688962936401,\"text\":\"1\"},{\"confidence_score\":0.9953481554985046,\"text\":\"JIS\"},{\"confidence_score\":0.9940292835235596,\"text\":\"10.9\"},{\"confidence_score\":0.9932726621627808,\"text\":\"10.0\"},{\"confidence_score\":0.9665566682815552,\"text\":\"9\"},{\"confidence_score\":0.9994761943817139,\"text\":\"12\"},{\"confidence_score\":0.986318051815033,\"text\":\"Ignition Residue\"},{\"confidence_score\":0.9964337348937988,\"text\":\"%\"},{\"confidence_score\":0.9941542744636536,\"text\":\"JIS\"},{\"confidence_score\":0.9928336143493652,\"text\":\"7.2\"},{\"confidence_score\":0.9969430565834045,\"text\":\"5.4\"},{\"confidence_score\":0.992426872253418,\"text\":\"8\"},{\"confidence_score\":0.9494736194610596,\"text\":\"On 200-mesh\"},{\"confidence_score\":0.9722371101379395,\"text\":\"%by mass\"},{\"confidence_score\":0.9925987720489502,\"text\":\"JWWA\"},{\"confidence_score\":0.9948233962059021,\"text\":\"15.9\"},{\"confidence_score\":0.9928651452064514,\"text\":\"14.7\"},{\"confidence_score\":0.9982892274856567,\"text\":\"20\"},{\"confidence_score\":0.995501697063446,\"text\":\"Arsenic\"},{\"confidence_score\":0.9964707493782043,\"text\":\"ppm\"},{\"confidence_score\":0.9961187839508057,\"text\":\"JWWA\"},{\"confidence_score\":0.9927570819854736,\"text\":\"1.0\"},{\"confidence_score\":0.994346559047699,\"text\":\"1.0\"},{\"confidence_score\":0.9964616894721985,\"text\":\"5\"},{\"confidence_score\":0.9451319575309753,\"text\":\"lodine AP\"},{\"confidence_score\":0.9969618320465088,\"text\":\"mg/g\"},{\"confidence_score\":0.9953864216804504,\"text\":\"JWWA\"},{\"confidence_score\":0.9975396990776062,\"text\":\"960\"},{\"confidence_score\":0.9985983967781067,\"text\":\"950\"},{\"confidence_score\":0.9984841346740723,\"text\":\"930\"},{\"confidence_score\":0.9777320027351379,\"text\":\"Bag Quantity\"},{\"confidence_score\":0.9962202906608582,\"text\":\"Bag\"},{\"confidence_score\":0.997857391834259,\"text\":\"5\"},{\"confidence_score\":0.9991825222969055,\"text\":\"5\"},{\"confidence_score\":0.9511127471923828,\"text\":\"*All the results are as packed\"},{\"confidence_score\":0.9723792672157288,\"text\":\"Production Date\"},{\"confidence_score\":0.5805875062942505,\"text\":\"21224\"},{\"confidence_score\":0.9447699785232544,\"text\":\"Product Shelf Life\"},{\"confidence_score\":0.9951969385147095,\"text\":\"26/09/202629/10/2026\"},{\"confidence_score\":0.9715588092803955,\"text\":\"JIS\"},{\"confidence_score\":0.87225341796875,\"text\":\"Japanese industrialStandard (JiS K 1474)\"},{\"confidence_score\":0.9072321057319641,\"text\":\"JWWA\"},{\"confidence_score\":0.9197180271148682,\"text\":\"Japanese Water Works Association JWwA K 113)\"},{\"confidence_score\":0.995202898979187,\"text\":\"Prepared\"},{\"confidence_score\":0.9654549360275269,\"text\":\"Reviewed and approved by,\"},{\"confidence_score\":0.9801847338676453,\"text\":\"HODQ\"},{\"confidence_score\":0.9664464592933655,\"text\":\"General Manager\"},{\"confidence_score\":0.9120795130729675,\"text\":\"(PT19100096)\"},{\"confidence_score\":0.9775529503822327,\"text\":\"(M/1503/2707/96)\"},{\"confidence_score\":0.9931328296661377,\"text\":\"09-Jun-2025\"},{\"confidence_score\":0.9925304651260376,\"text\":\"09-Jun-2025\"},{\"confidence_score\":0.9084920287132263,\"text\":\"Authenticity verifi\"},{\"confidence_score\":0.9729986190795898,\"text\":\"Emergency contac\"},{\"confidence_score\":0.9273199439048767,\"text\":\"IMk.1 No.1026 Lorong Perusahaan Dua | Prai Industrial Estate 13600 Prai |PenangMalaysia \"},{\"confidence_score\":0.9729952216148376,\"text\":\"604-39079663907795,3992928\"},{\"confidence_score\":0.9922776818275452,\"text\":\"604-3907817\"},{\"confidence_score\":0.9747782945632935,\"text\":\"info@century-chemical.com\"},{\"confidence_score\":0.9690288305282593,\"text\":\"http://www.century-chemical.com\"}]],\"token_consumption\":[{\"Completion Tokens\":544,\"Prompt Tokens\":5073,\"Total Cost (USD)\":0.033525,\"Total Tokens\":5617}],\"url\":null}\r\n";
                    result = responseString;

                    var data = JsonConvert.DeserializeObject<OcrResult>(result);
                    JObject dataJson = JObject.Parse(result);
                    var keyInfoArray = dataJson["key_info"] as JArray;
                    var ocrArray = dataJson["ocr"] as JArray;

                    var flatOcrList = ocrArray?
                        .SelectMany(group => group is JArray innerArray ? innerArray : new JArray())
                        .OfType<JObject>()
                        .ToList();


                    //var ocrArray = dataJson["ocr"]?.FirstOrDefault() as JArray;

                    string GetConfidenceScore(string text)
                    {
                        if (string.IsNullOrWhiteSpace(text)) return "";

                        string normalizedText = Normalize(text);

                        var match = flatOcrList?.FirstOrDefault(o => Normalize(o["text"]?.ToString()) == normalizedText);
                        if (match != null)
                        {
                            return match["confidence_score"]?.ToString() ?? "";
                        }

                        var partialMatch = flatOcrList?.FirstOrDefault(o =>
                            Normalize(o["text"]?.ToString()).Contains(normalizedText));
                        if (partialMatch != null)
                        {
                            return partialMatch["confidence_score"]?.ToString() ?? "";
                        }

                        foreach (int len in new[] { 20, 10, 5, 3 })
                        {
                            string prefix = normalizedText.Substring(0, Math.Min(len, normalizedText.Length));
                            var matchLen = flatOcrList?.FirstOrDefault(o =>
                            {
                                string ocrText = Normalize(o["text"]?.ToString());
                                return ocrText.Substring(0, Math.Min(len, ocrText.Length)) == prefix;
                            });

                            if (matchLen != null) return matchLen["confidence_score"]?.ToString() ?? "";
                        }

                        return "";
                    }


                    if (keyInfoArray != null)
                    {
                        foreach (var keyInfo in keyInfoArray)
                        {
                            string lotNoValue = "";
                            string lotNoConfidence = "";
                            string newlotNoValue = "";
                            string newlotNoConfidence = "";

                            //replace lotno detected with confirmed lotno

                            JArray newLotNoArray = new JArray(conflot.Split('*'));

                            keyInfo["lot_no"] = newLotNoArray;


                            if (keyInfo["lot_no"] != null && keyInfo["lot_no"] is JArray lotNoArray)
                            {
                                lotNoValue = lotNoArray.FirstOrDefault()?.ToString() ?? "";
                                lotNoConfidence = GetConfidenceScore(lotNoValue);
                            }

                            foreach (var property in keyInfo)
                            {
                                string propertyName = property.Path;

                                if (propertyName.Contains("lot_no"))
                                {
                                    continue;
                                }

                                int count = 0;
                                var values = property.First as JArray;
                                if (values != null)
                                {
                                    foreach (var item in values)
                                    {
                                        // for multiple lot
                                        if (ocrindicator == "2")
                                        {
                                            if (keyInfo["lot_no"] != null && keyInfo["lot_no"] is JArray mullotarray)
                                            {

                                                int length = mullotarray.Count;

                                                if (count < length)
                                                {
                                                    lotNoValue = mullotarray[count]?.ToString() ?? "";
                                                    lotNoConfidence = GetConfidenceScore(lotNoValue);
                                                }
                                                else
                                                {
                                                    break;
                                                }

                                            }

                                        }

                                        if (item is JObject obj) //handling for object-type properties
                                        {
                                            if (ocrindicator == "2" && obj.Properties().Any(p => p.Name != "lot_no" && p.Value.Type == JTokenType.Object))
                                            {
                                                foreach (var prop in obj.Properties())
                                                {
                                                    if (prop.Name == "lot_no") continue;
                                                    var testResults = prop.Value as JObject;
                                                    if (testResults != null)
                                                    {
                                                        foreach (var test in testResults.Properties())
                                                        {
                                                            string fprop = test.Name;
                                                            string fval = ExtractFirstWordOrNumber(test.Value?.ToString() ?? "");

                                                            extractedData.Add(new KeyInfoData
                                                            {
                                                                property = fprop,
                                                                value = fval,
                                                                lot_no = lotNoValue,
                                                                cfs_property = GetConfidenceScore(fprop),
                                                                cfs_value = GetConfidenceScore(fval),
                                                                cfs_lot_no = lotNoConfidence
                                                            });
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                int newcount = 0;
                                                foreach (var subProp in obj)
                                                {
                                                    string fprop = subProp.Key;
                                                    string fval = ExtractFirstWordOrNumber(subProp.Value.FirstOrDefault()?.ToString() ?? "");

                                                    if (keyInfo["lot_no"] != null && keyInfo["lot_no"] is JArray mullotarray)
                                                    {
                                                        newlotNoValue = mullotarray[newcount]?.ToString() ?? "";
                                                        newlotNoConfidence = GetConfidenceScore(newlotNoValue);
                                                    }


                                                    if (ocrindicator == "2" && fprop == newlotNoValue)
                                                    {
                                                        // JSON EXAMPLE
                                                        //{
                                                        //    "30553S": {
                                                        //        "COLOR (APHA)": "160",
                                                        //        "I.V.": "1.9",
                                                        //        ...
                                                        //      },
                                                        //    "30556S": {
                                                        //        "COLOR (APHA)": "160",
                                                        //        "I.V.": "1.9",
                                                        //        ...
                                                        //      }
                                                        //}

                                                        JObject testResults = subProp.Value as JObject;
                                                        if (testResults != null)
                                                        {
                                                            foreach (var test in testResults)
                                                            {
                                                                fprop = test.Key;
                                                                fval = ExtractFirstWordOrNumber(test.Value?.ToString() ?? "");

                                                                extractedData.Add(new KeyInfoData
                                                                {
                                                                    property = fprop,
                                                                    value = fval,
                                                                    lot_no = newlotNoValue,
                                                                    cfs_property = GetConfidenceScore(fprop),
                                                                    cfs_value = GetConfidenceScore(fval),
                                                                    cfs_lot_no = GetConfidenceScore(newlotNoConfidence)
                                                                });
                                                            }
                                                        }

                                                        newcount++;

                                                    }
                                                    else
                                                    {
                                                        if (fval == "")
                                                        {
                                                            fval = ExtractFirstWordOrNumber(subProp.Value?.ToString() ?? "");
                                                        }
                                                        extractedData.Add(new KeyInfoData
                                                        {
                                                            property = fprop,
                                                            value = fval,
                                                            lot_no = lotNoValue,
                                                            cfs_property = GetConfidenceScore(fprop),
                                                            cfs_value = GetConfidenceScore(fval),
                                                            cfs_lot_no = lotNoConfidence
                                                        });
                                                    }
                                                }
                                            }

                                        }
                                        else if (item is JArray nestedArray)  // **New condition for nested array structure**
                                        {
                                            if (nestedArray.Count == 2)
                                            {
                                                string fprop = nestedArray[0]?.ToString() ?? "";
                                                string fval = nestedArray[1]?.ToString() ?? "";

                                                extractedData.Add(new KeyInfoData
                                                {
                                                    property = fprop,
                                                    value = fval,
                                                    lot_no = lotNoValue,
                                                    cfs_property = GetConfidenceScore(fprop),
                                                    cfs_value = GetConfidenceScore(fval),
                                                    cfs_lot_no = lotNoConfidence
                                                });
                                            }
                                        }
                                        else
                                        {
                                            string rawText = item.ToString();

                                            if (rawText.Contains(":")) // Detect "property: value" format
                                            {
                                                // "DAA : 11, 13, 32..." with lot_no as array
                                                if (ocrindicator == "2" && keyInfo.First is JProperty firstProp && keyInfo["lot_no"] is JArray mullotarray)
                                                {
                                                    int lotCount = mullotarray.Count;
                                                    var valueLines = firstProp.Value as JArray;

                                                    if (valueLines != null)
                                                    {
                                                        var propertyMap = new Dictionary<string, List<string>>();

                                                        foreach (var line in valueLines)
                                                        {
                                                            var splits = line.ToString().Split(new[] { ':' }, 2);
                                                            if (splits.Length != 2) continue;

                                                            string fprop = splits[0].Trim();
                                                            var valuesmat = splits[1].Split(',').Select(x => x.Trim()).ToList();
                                                            propertyMap[fprop] = valuesmat;
                                                        }

                                                        for (int i = 0; i < lotCount; i++)
                                                        {
                                                            string lotNo = mullotarray[i]?.ToString() ?? "";
                                                            foreach (var entry in propertyMap)
                                                            {
                                                                string fprop = entry.Key;
                                                                string fval = i < entry.Value.Count ? entry.Value[i] : "";

                                                                extractedData.Add(new KeyInfoData
                                                                {
                                                                    property = fprop,
                                                                    value = ExtractFirstWordOrNumber(fval),
                                                                    lot_no = lotNo,
                                                                    cfs_property = GetConfidenceScore(fprop),
                                                                    cfs_value = GetConfidenceScore(fval),
                                                                    cfs_lot_no = GetConfidenceScore(lotNo)
                                                                });
                                                            }
                                                        }

                                                        break;
                                                    }
                                                }
                                                else if (rawText.Count(c => c == ':') > 1 && rawText.Contains(",")) //HANA 02042026 Add handling for 1 string result
                                                {
                                                    var splitc = rawText.Split(new[] { ',' }, 2);

                                                    foreach (var c in splitc)
                                                    {
                                                        var splits = c.Split(new[] { ':' }, 2);
                                                        if (splits.Length == 2)
                                                        {
                                                            string fprop = splits[0].Trim();
                                                            string fval = ExtractFirstWordOrNumber(splits[1].Trim());

                                                            extractedData.Add(new KeyInfoData
                                                            {
                                                                property = fprop,
                                                                value = fval,
                                                                lot_no = lotNoValue,
                                                                cfs_property = GetConfidenceScore(fprop),
                                                                cfs_value = GetConfidenceScore(fval),
                                                                cfs_lot_no = lotNoConfidence
                                                            });
                                                        }
                                                    }

                                                    break;
                                                }

                                                var split = rawText.Split(new[] { ':' }, 2);
                                                if (split.Length == 2)
                                                {
                                                    string fprop = split[0].Trim();
                                                    string fval = ExtractFirstWordOrNumber(split[1].Trim());

                                                    extractedData.Add(new KeyInfoData
                                                    {
                                                        property = fprop,
                                                        value = fval,
                                                        lot_no = lotNoValue,
                                                        cfs_property = GetConfidenceScore(fprop),
                                                        cfs_value = GetConfidenceScore(fval),
                                                        cfs_lot_no = lotNoConfidence
                                                    });
                                                }
                                            }
                                            else
                                            {
                                                string fprop = ((JProperty)property).Name;
                                                string fval = ExtractFirstWordOrNumber(rawText.Trim());

                                                if (fprop.Contains(":"))
                                                {
                                                    string restfprop = fprop.Split(':')[1].Trim();
                                                    if (restfprop.Contains(","))
                                                    {
                                                        var propNames = fprop
                                                        .Split(',')
                                                        .Select(p => p.Split(':')[0].Trim())  // get the part before ':' and trim
                                                        .ToList();

                                                        if (values.Count == 1 && values[0].ToString().Contains(',')) //HANA 02042026 Add handling for 1 string result
                                                        {
                                                            var splitv = values[0].ToString().Split(new[] { ',' }, 2);

                                                            for (int i = 0; i < propNames.Count; i++)
                                                            {
                                                                fprop = propNames[i];
                                                                char spl;
                                                                if (splitv[i].ToString().Contains(':'))
                                                                {
                                                                    spl = ':';
                                                                }
                                                                else
                                                                {
                                                                    spl = ' ';
                                                                }

                                                                var lval = splitv[i]?.ToString().Trim().Split(spl);
                                                                fval = lval[lval.Length - 1];

                                                                extractedData.Add(new KeyInfoData
                                                                {
                                                                    property = fprop,
                                                                    value = fval,
                                                                    lot_no = lotNoValue,
                                                                    cfs_property = GetConfidenceScore(fprop),
                                                                    cfs_value = GetConfidenceScore(fval),
                                                                    cfs_lot_no = lotNoConfidence
                                                                });
                                                            }
                                                        }
                                                        else
                                                        {
                                                            for (int i = 0; i < values.Count && i < propNames.Count; i++)
                                                            {
                                                                fprop = propNames[i];
                                                                fval = ExtractFirstWordOrNumber(values[i]?.ToString() ?? "");

                                                                extractedData.Add(new KeyInfoData
                                                                {
                                                                    property = fprop,
                                                                    value = fval,
                                                                    lot_no = lotNoValue,
                                                                    cfs_property = GetConfidenceScore(fprop),
                                                                    cfs_value = GetConfidenceScore(fval),
                                                                    cfs_lot_no = lotNoConfidence
                                                                });
                                                            }
                                                        }

                                                        break;
                                                    }
                                                    else
                                                    {
                                                        fprop = fprop.Split(':')[0].Trim();

                                                        extractedData.Add(new KeyInfoData
                                                        {
                                                            property = fprop,
                                                            value = fval,
                                                            lot_no = lotNoValue,
                                                            cfs_property = GetConfidenceScore(fprop),
                                                            cfs_value = GetConfidenceScore(fval),
                                                            cfs_lot_no = lotNoConfidence
                                                        });
                                                    }
                                                }



                                            }
                                        }
                                        count++;
                                    }
                                }
                            }
                        }
                    }

                    if (extractedData.Count == 0)
                    {
                        extractedData.Add(new KeyInfoData
                        {
                            property = "",
                            value = "",
                            lot_no = "",
                            cfs_property = "",
                            cfs_value = "",
                            cfs_lot_no = ""
                        });
                    }

                    return (extractedData, result);
                }
                else
                {
                    //throw new InvalidOperationException("OCR API Error2. " + responseString);
                    throw new InvalidOperationException("OCR API Error. ");
                }
            }
        }

        public DataTable MergeMMandOCR(DataTable dtSpec, List<KeyInfoData> keyInfoList)
        {
            DataTable mergedTable = new DataTable();
            mergedTable.Columns.Add("MM_MATERIAL_D2_ID", typeof(string));
            mergedTable.Columns.Add("mm_spec_type", typeof(string));
            mergedTable.Columns.Add("mm_spec", typeof(string));
            mergedTable.Columns.Add("mm_spec_val", typeof(string));
            mergedTable.Columns.Add("ocr_spec", typeof(string));
            mergedTable.Columns.Add("cfs_ocr_spec", typeof(string));
            mergedTable.Columns.Add("ocr_spec_value", typeof(string));
            mergedTable.Columns.Add("cfs_ocr_spec_value", typeof(string));
            mergedTable.Columns.Add("lot_no", typeof(string));
            mergedTable.Columns.Add("cfs_lot_no", typeof(string));
            mergedTable.Columns.Add("tpm_code", typeof(string));

            HashSet<string> matchedProperties = new HashSet<string>();

            foreach (DataRow row in dtSpec.Rows)
            {
                string did = row["MM_MATERIAL_D2_ID"].ToString();
                string specName = row["SUPPLIER_SPEC_NAME"].ToString();
                string specValue = row["SUPPLIER_SPEC_VALUE"].ToString();
                string specType = row["SUPPLIER_SPEC_TYPE"].ToString();
                string tpm_code = row["TPM_CODE"].ToString();

                KeyInfoData keyInfo = GetKeyInfoMatch(specName, keyInfoList);

                //KeyInfoData keyInfo = keyInfoList.FirstOrDefault(k => IsSimilar(NormalizeString(k.property), NormalizeString(specName)));

                if (keyInfo != null)
                {
                    matchedProperties.Add(keyInfo.property);
                    if (keyInfo.property != "" && (keyInfo.value == "" || keyInfo.value == null)) // capture spec but not capture value
                    {
                        mergedTable.Rows.Add(did, specType, specName, specValue, keyInfo.property, keyInfo.cfs_property, "Not captured by OCR", "", keyInfo.lot_no, keyInfo.cfs_lot_no, tpm_code);
                    }
                    else
                    {
                        mergedTable.Rows.Add(did, specType, specName, specValue, keyInfo.property, keyInfo.cfs_property, keyInfo.value, keyInfo.cfs_value, keyInfo.lot_no, keyInfo.cfs_lot_no, tpm_code);
                    }

                }
                else
                {
                    mergedTable.Rows.Add(did, specType, specName, specValue, "Not captured by OCR", "", "", "", "", "", tpm_code);
                }
            }

            foreach (var keyInfo in keyInfoList)
            {
                if (!matchedProperties.Contains(keyInfo.property))
                {
                    mergedTable.Rows.Add("0", "", "No MM Spec Registered", "", keyInfo.property, keyInfo.cfs_property, keyInfo.value, keyInfo.cfs_value, keyInfo.lot_no, keyInfo.cfs_lot_no, "");
                }
            }

            return mergedTable;
        }

        public MMOCRData CreateMMOCRData(DataRow row, string mat, int supplierId)
        {
            MMOCRData infoObjAdd = new MMOCRData();

            infoObjAdd.lot_no = row["lot_no"] != DBNull.Value ? row["lot_no"].ToString() : "";
            infoObjAdd.mat_name = mat;
            infoObjAdd.supplier_id = supplierId;
            infoObjAdd.mm_spec_type = row["mm_spec_type"] != DBNull.Value ? row["mm_spec_type"].ToString() : "";
            infoObjAdd.MM_MATERIAL_D2_ID = Convert.ToInt32(row["MM_MATERIAL_D2_ID"]);
            infoObjAdd.mm_spec = row["mm_spec"] != DBNull.Value ? row["mm_spec"].ToString() : "";
            infoObjAdd.mm_spec_val = row["mm_spec_val"] != DBNull.Value ? row["mm_spec_val"].ToString() : "";
            infoObjAdd.ocr_spec = row["ocr_spec"] != DBNull.Value ? row["ocr_spec"].ToString() : "";
            infoObjAdd.ocr_spec_value = row["ocr_spec_value"] != DBNull.Value ? row["ocr_spec_value"].ToString() : "";

            if (infoObjAdd.ocr_spec_value == "ND")
            {
                infoObjAdd.ocr_spec_value = "0";
            }

            infoObjAdd.cfs_ocr_spec = string.IsNullOrEmpty(row["cfs_ocr_spec"].ToString())
                ? 0m : Math.Round(Convert.ToDecimal(row["cfs_ocr_spec"]), 4);

            infoObjAdd.cfs_ocr_spec_value = string.IsNullOrEmpty(row["cfs_ocr_spec_value"].ToString())
                ? 0m : Math.Round(Convert.ToDecimal(row["cfs_ocr_spec_value"]), 4);

            infoObjAdd.cfs_lot_no = string.IsNullOrEmpty(row["cfs_lot_no"].ToString())
                ? 0m : Math.Round(Convert.ToDecimal(row["cfs_lot_no"]), 4);

            if (infoObjAdd.ocr_spec != "Not captured by OCR" && infoObjAdd.ocr_spec_value != "Not captured by OCR")
            {
                infoObjAdd.compare_result = compareResult(infoObjAdd.mm_spec_type, infoObjAdd.mm_spec_val, infoObjAdd.ocr_spec_value);
            }
            else
            {
                infoObjAdd.compare_result = "TO BE CONFIRMED";
            }

            return infoObjAdd;
        }

        public HeaderMMOCRData CreateHeaderMMOCRData(int supplierId, string supplierName, int materialId, string mat, string tpmCode,
                                              ProcessPageResult result, string rLot, string ocrIndicator)
        {
            HeaderMMOCRData headerinfoObjAdd = new HeaderMMOCRData();

            headerinfoObjAdd.supplier_id = supplierId;
            headerinfoObjAdd.supplier_name = supplierName;
            headerinfoObjAdd.material_id = materialId;
            headerinfoObjAdd.material_name = mat;
            headerinfoObjAdd.tpm_code = tpmCode;
            headerinfoObjAdd.is_lot_exist = result.isExist;
            headerinfoObjAdd.has_rejectedID = result.rejectedID;
            headerinfoObjAdd.resultOCR = result.resultfromOCR;

            if (ocrIndicator == "3")
            {
                headerinfoObjAdd.lot_no = rLot.Replace("*", ",\n");
                headerinfoObjAdd.tabname = !string.IsNullOrEmpty(tpmCode)
                    ? $"{supplierName} ({tpmCode})"
                    : supplierName;
            }
            else
            {
                headerinfoObjAdd.lot_no = rLot;
                headerinfoObjAdd.tabname = !string.IsNullOrEmpty(tpmCode)
                    ? $"{rLot} ({tpmCode})"
                    : rLot;
            }

            return headerinfoObjAdd;
        }


        #endregion

        #region FUNCTION

        public string Normalize(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            input = input.ToLower();

            // Remove all non-alphanumeric characters (keep only letters and numbers)
            return new string(input.Where(c => char.IsLetterOrDigit(c)).ToArray());
        }

        private static string compareResult(string type, string mm_spec_val, string ocr_val)
        {
            string result = "APPROVED";

            if (ocr_val == "ND")
            {
                ocr_val = "0";
            }

            if (type == "Text")
            {
                if (ocr_val.ToLower().Trim() != mm_spec_val.ToLower().Trim())
                {
                    result = "REJECTED";
                }
            }
            else if (type == "Range")
            {
                if (!mm_spec_val.Contains("~"))
                {
                    return "Invalid range format";
                }

                if (!Regex.IsMatch(ocr_val.Trim(), @"^-?\d+(\.\d+)?$"))
                {
                    return "Invalid numeric values";
                }

                var parts = mm_spec_val.Split('~');
                if (parts.Length == 2)
                {
                    if (decimal.TryParse(parts[0].Trim(), out decimal minVal) &&
                        decimal.TryParse(parts[1].Trim(), out decimal maxVal) &&
                        decimal.TryParse(ocr_val, out decimal ocrValue))
                    {
                        if (ocrValue < minVal || ocrValue > maxVal)
                        {
                            result = "REJECTED";
                        }
                    }
                    else
                    {
                        result = "Invalid numeric values";
                    }
                }
                else
                {
                    result = "Invalid range format";
                }
            }
            else if (type == "Numeric")
            {
                if (!Regex.IsMatch(ocr_val.Trim(), @"^-?\d+(\.\d+)?$"))
                {
                    return "Invalid numeric values";
                }

                // Check if the value contains an operator (<, >, <=, >=, =)
                var match = Regex.Match(mm_spec_val.Trim(), @"^(<=|>=|<|>|=)?\s*(-?\s*\d+(\.\d+)?)$");

                if (match.Success)
                {
                    string operatorSymbol = match.Groups[1].Value;
                    string numberStr = match.Groups[2].Value.Replace(" ", "");

                    if (decimal.TryParse(numberStr, out decimal specValue) &&
                        decimal.TryParse(ocr_val, out decimal ocrValue))
                    {
                        if (string.IsNullOrEmpty(operatorSymbol))
                        {
                            operatorSymbol = "=";
                        }

                        switch (operatorSymbol)
                        {
                            case "<=":
                                if (!(ocrValue <= specValue)) result = "REJECTED";
                                break;
                            case ">=":
                                if (!(ocrValue >= specValue)) result = "REJECTED";
                                break;
                            case "<":
                                if (!(ocrValue < specValue)) result = "REJECTED";
                                break;
                            case ">":
                                if (!(ocrValue > specValue)) result = "REJECTED";
                                break;
                            case "=":
                                if (!(ocrValue == specValue)) result = "REJECTED";
                                break;
                        }
                    }
                    else
                    {
                        result = "Invalid numeric values";
                    }
                }
                else
                {
                    result = "Invalid numeric values";
                }
            }
            else
            {
                result = "Invalid Spec Type";
            }

            return result;
        }

        private static string oldExtractFirstWordOrNumber(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            // Remove leading non-alphanumeric characters
            input = Regex.Replace(input, @"^[^\w\d-]+", "");

            Match firstMatch = Regex.Match(input, @"^(-?\d*\.?\d+|[A-Za-z]+)");

            if (!firstMatch.Success)
                return "";

            string firstWord = firstMatch.Value;

            // If the first word is numeric (int or decimal), return it immediately
            if (Regex.IsMatch(firstWord, @"^-?\d*\.?\d+$"))
            {
                return firstWord;
            }

            string remainingText = input.Substring(firstWord.Length).Trim();

            // Match second word (must not contain special characters)
            Match secondMatch = Regex.Match(remainingText, @"^([\w\d]+)");

            // Allow if second word has only letters and/or numbers (no symbols)
            if (secondMatch.Success && !Regex.IsMatch(secondMatch.Value, @"[^A-Za-z0-9]"))
            {
                return firstWord + " " + secondMatch.Value;
            }


            //Match secondMatch = Regex.Match(remainingText, @"^([A-Za-z]+)");

            //if (secondMatch.Success)
            //{
            //    return firstWord + " " + secondMatch.Value;
            //}

            return firstWord;
        }

        //private static string ExtractFirstWordOrNumber(string input)
        //{
        //    if (string.IsNullOrWhiteSpace(input))
        //        return "";

        //    input = input.Trim();

        //    // Match only letters and hyphens in valid word structure (e.g., "well-defined solution")
        //    if (Regex.IsMatch(input, @"^([A-Za-z]+(-[A-Za-z]+)*\s*)+$"))
        //    {
        //        return input;
        //    }

        //    // Otherwise, return the first number or word
        //    Match match = Regex.Match(input, @"(-?\d*\.?\d+|[A-Za-z]+(-[A-Za-z]+)*)");
        //    return match.Success ? match.Value : "";
        //}

        private static string ExtractFirstWordOrNumber(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "";

            input = input.Trim();

            // Remove trailing punctuation like period, comma, etc.
            input = Regex.Replace(input, @"[\p{P}]+$", "");

            // Match only letters and hyphens in valid word structure
            if (Regex.IsMatch(input, @"^([A-Za-z]+(-[A-Za-z]+)*\s*)+$"))
            {
                return input;
            }

            // Otherwise, return the first number or word
            Match match = Regex.Match(input, @"(-?\d*\.?\d+|[A-Za-z]+(-[A-Za-z]+)*)");
            return match.Success ? match.Value : "";
        }


        private bool IsSimilar(string text1, string text2)
        {
            if (string.IsNullOrEmpty(text1) || string.IsNullOrEmpty(text2))
                return false;


            string normalized1 = text1.ToLower().Replace(" ", "");
            string normalized2 = text2.ToLower().Replace(" ", "");

            if (normalized1.Length >= 20 && normalized2.Length >= 20 &&
                normalized1.Substring(0, 20) == normalized2.Substring(0, 20))
            {
                return true;
            }

            if (normalized1.Length >= 15 && normalized2.Length >= 15 &&
               normalized1.Substring(0, 15) == normalized2.Substring(0, 15))
            {
                return true;
            }

            if (normalized1.Length >= 10 && normalized2.Length >= 10 &&
                normalized1.Substring(0, 10) == normalized2.Substring(0, 10))
            {
                return true;
            }

            if (normalized1.Length >= 5 && normalized2.Length >= 5 &&
                normalized1.Substring(0, 5) == normalized2.Substring(0, 5))
            {
                return true;
            }

            if (normalized1.Length >= 2 && normalized2.Length >= 2 &&
                normalized1.Substring(0, 2) == normalized2.Substring(0, 2))
            {
                return true;
            }

            return false;
        }

        public KeyInfoData GetKeyInfoMatch(string specName, List<KeyInfoData> keyInfoList)
        {
            // Step 1: First check for an exact match
            KeyInfoData exactMatch = keyInfoList
                .FirstOrDefault(k => string.Equals(Normalize(k.property), Normalize(specName), StringComparison.OrdinalIgnoreCase));

            if (exactMatch != null)
            {
                return exactMatch;
            }

            // Step 2: If no exact match, check using the similarity method
            return keyInfoList
                .FirstOrDefault(k => IsSimilar(Normalize(k.property), Normalize(specName)));
        }

        public AppConfig GetAppConfig()
        {
            try
            {
                string isTest = TPM_QAS.DAL.Database.GetAppSettingStatic("isTest"];

                return new AppConfig
                {
                    ProxyAddress = TPM_QAS.DAL.Database.GetAppSettingStatic("ProxyAddress"] ?? throw new Exception("ProxyAddress is missing"),
                    ProxyEmail = TPM_QAS.DAL.Database.GetAppSettingStatic("ProxyEmail"] ?? throw new Exception("ProxyEmail is missing"),
                    ProxyPassword = TPM_QAS.DAL.Database.GetAppSettingStatic("ProxyPassword"] ?? throw new Exception("ProxyPassword is missing"),
                    OCREndpoint = string.Equals(isTest, "TRUE", StringComparison.OrdinalIgnoreCase)
                        ? TPM_QAS.DAL.Database.GetAppSettingStatic("OCR_API_Test"] ?? throw new Exception("OCR_API_Test is missing")
                        : TPM_QAS.DAL.Database.GetAppSettingStatic("OCR_API_Live"] ?? throw new Exception("OCR_API_Live is missing"),
                    TMSUserAPIKey = TPM_QAS.DAL.Database.GetAppSettingStatic("TMSUserAPIKey"] ?? throw new Exception("TMSUserAPIKey is missing"),
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAppConfig: {ex.Message}");
                return null;
            }
        }

        public void DeleteOldFiles()
        {
            string firstPath = Server.MapPath("~/COA_DE_OCR/FirstOCRFiles/");
            string secondPath = Server.MapPath("~/Splitpath/");
            DateTime forteenDaysAgo = DateTime.Today.AddDays(-14);

            if (Directory.Exists(firstPath))
            {
                string[] files = Directory.GetFiles(firstPath);              

                foreach (string file in files)
                {
                    DateTime lastModified = System.IO.File.GetLastWriteTime(file);

                    if (lastModified < forteenDaysAgo)
                    {
                        System.IO.File.Delete(file);
                    }
                }
            }

            if (Directory.Exists(secondPath))
            {
                string[] files = Directory.GetFiles(secondPath);

                foreach (string file in files)
                {
                    DateTime lastModified = System.IO.File.GetLastWriteTime(file);

                    if (lastModified < forteenDaysAgo)
                    {
                        System.IO.File.Delete(file);
                    }
                }
            }
        }

        [HttpPost]
        public async Task<JsonResult> CheckFileExists(string fileName)
        {

            if(fileName != null && fileName != "")
            {
                string path = await blob.SetBlobUrlToken("COA Document/" + fileName);

                if(path != null)
                {
                    return Json(new { exists = true, path = path });
                }
                else
                {
                    return Json(new { exists = false });
                }
                
            }
            else
            {
                return Json(new { exists = false });
            }

        }


        #endregion

        #region DELETE

        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> deleteCOAOCR(List<string> lstid)
        {
            bool success = true;
            string message = "";

            COAOCRModel modelh = new COAOCRModel();

            if (lstid.Count < 1)
            {
                success = false;
                message += "Please select a record first to delete. ";
            }

            if (success && lstid.Count > 0)
            {
                foreach (var item in lstid)
                {
                    modelh.DE_OCR_H_ID = Convert.ToInt32(item);
                    modelh.RECORD_TYP = "5";

                    modelh.FINAL_FILE_NAME = "";
                    modelh.FINAL_RESULT = "";
                    modelh.ENTRY_TYPE = "";
                    modelh.OCR_INDICATOR = "";

                    //delete h
                    string result1 = await dbdal.DE_OCR_H_Maint(modelh);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                    else
                    {
                        //delete d1
                        string result9 = await dbdal.DE_OCR_D_Maint(Convert.ToInt32(item), 0, 0, "", 0, "", 0, "", 0, "", "", "2");
                        if (!(int.TryParse(result9, out int num2)))
                        {
                            success = false;
                            message += result9;
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
        public async Task<ActionResult> COA_DE_OCR_AUDIT(string id)
        {
            AuditTrailModels AuditTrailModels = new AuditTrailModels();
            List<SqlParameter> _pMssql = new List<SqlParameter>();
            _pMssql.Add(new SqlParameter("@TABLE", "PVIEW_DE_OCR_A"));
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
            //var param = new { pTable = "PVIEW_DE_OCR_A", pKeyValue = id };
            //List<AuditTrailModel> AuditTrailModel = await common.PSP_COMMON_DAPPER<AuditTrailModel>("PSP_COMMON_AUDIT", CommandType.StoredProcedure, param) ?? new List<AuditTrailModel>();

            //return View(AuditTrailModel);
        }

        #endregion

        #region DDL

        public async Task<ActionResult> fillSupplier()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items = await LoadInnerDllData(0, "", "COA_SUPPLIER");

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> fillMaterial(string supp)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items = await LoadInnerDllData(0, supp, "COA_SUPP_MAT");

            return Json(items, JsonRequestBehavior.AllowGet);
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