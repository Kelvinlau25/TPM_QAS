using ClosedXML.Excel;
using DBModel;
using DocumentFormat.OpenXml.Office2010.Excel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
    public class COA_GA_RAWMAT_TRENDController : Controller
    {
        DB dbmain = new DB();
        COA_GA_RAWMAT_TREND_DAL dbdal = new COA_GA_RAWMAT_TREND_DAL();

        #region GRAPH

        [SessionExpire]
        public async Task<ActionResult> COA_GA_RAWMAT_TREND_DET()
        {
            ViewBag.Tittle = "Raw Materials Trend Analysis";
            var model = new RawMatTrendModel();
            model.DropdownMaterial = await LoadDllData(0, "", "COA_MATERIAL");
            //model.DropdownSupplier = await LoadDllData(0, "", "COA_SUPPLIER");
            model.DropdownSupplier = new List<SelectListItem>();
            model.DropdownItem = new List<SelectListItem>();
            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> getTrendChartData(RawMatTrendModel model)
        {
            bool success = true;
            string message = "";
            string dtl = "";
            string ocrResulData = "";

            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.DATE_FROM) || string.IsNullOrEmpty(model.DATE_TO) ||
                    model.MM_MATERIAL_H_ID == 0 || string.IsNullOrEmpty(model.MM_SUPPLIER_H_ID_STRING) ||
                    string.IsNullOrEmpty(model.MM_MATERIAL_D2_ID_STRING))
                {
                    success = false;
                    message += "Required field cannot be empty.";
                }

                if (success)
                {
                    DataTable dt = await dbdal.getTrendAnalysis(model);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        // Dictionary to store data grouped by supplier
                        Dictionary<string, List<string>> supplierData = new Dictionary<string, List<string>>();

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            string supplierName = dt.Rows[i]["SUPPLIER_NAME"].ToString();
                            string itemspec = dt.Rows[i]["TPM_SPEC_NAME"].ToString();
                            string lotNo = dt.Rows[i]["LOT_NO"].ToString();
                            string dateFull = Convert.ToDateTime(dt.Rows[i]["CREATED_DATE"]).ToString("dd/MM/yyyy");
                            string dateX = Convert.ToDateTime(dt.Rows[i]["CREATED_DATE"]).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");

                            string ocrResult = dt.Rows[i]["OCR_RESULT"].ToString();
                            string unit = dt.Rows[i]["UNIT"].ToString();

                            var dataPointObj = new
                            {
                                x = dateX,
                                y = Convert.ToDouble(ocrResult),
                                label = lotNo,
                                itemspec = itemspec,
                                LOTNO = lotNo,
                                DATEFULL = dateFull,
                                UNIT = unit
                            };

                            string dataPoint = JsonConvert.SerializeObject(dataPointObj);

                            if (!supplierData.ContainsKey(supplierName))
                            {
                                supplierData[supplierName] = new List<string>();
                            }

                            supplierData[supplierName].Add(dataPoint);
                        }

                        Dictionary<string, string> jsonSupplierData = supplierData
                            .ToDictionary(k => k.Key, v => "[" + string.Join(",", v.Value) + "]");

                        ocrResulData = JsonConvert.SerializeObject(jsonSupplierData);
                        dtl = JsonConvert.SerializeObject(dt);
                    }
                    else
                    {
                        success = false;
                        message += "No data found for selected values.";
                    }
                }
                else
                {
                    success = false;
                    message += "Error in fetching data.";
                }
            }
            else
            {
                success = false;
                message += "Error: Model state is not valid.";
            }

            return Json(new { success = success, message = message, dtl = dtl, ocrResulData = ocrResulData });
        }

        [HttpPost]
        public async Task<JsonResult> CheckTrendExcel(RawMatTrendModel model)
        {

            if (string.IsNullOrEmpty(model.DATE_FROM) || string.IsNullOrEmpty(model.DATE_TO) ||
            model.MM_MATERIAL_H_ID == 0 || string.IsNullOrEmpty(model.MM_SUPPLIER_H_ID_STRING) ||
            string.IsNullOrEmpty(model.MM_MATERIAL_D2_ID_STRING))
            {
                return Json(new { success = false, message = "Required field cannot be empty." });
            }

            try
            {
                DataTable dt = await dbdal.getTrendAnalysisExcel(model);

                if (dt != null && dt.Rows.Count > 0)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "No data available to export." });
                }
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, (HttpContext.Session.GetObject<ACL_UserObj>("AclUser")).USER_ID.ToString());
                err = null;
                return Json(new { success = false, message = "Error occurred while checking data." });
            }
        }

        public async Task<string> TrendExcel(string DATE_FROM,string DATE_TO,string MM_MATERIAL_H_ID, string MM_SUPPLIER_H_ID_STRING, string MM_MATERIAL_D2_ID_STRING)
        {
            string ok = "";

            var model = new RawMatTrendModel
            {
                DATE_FROM = DATE_FROM,
                DATE_TO = DATE_TO,
                MM_MATERIAL_H_ID = Convert.ToInt32(MM_MATERIAL_H_ID),
                MM_SUPPLIER_H_ID_STRING = MM_SUPPLIER_H_ID_STRING,
                MM_MATERIAL_D2_ID_STRING = MM_MATERIAL_D2_ID_STRING
            };
            try
            {
                DataTable dt = await dbdal.getTrendAnalysisExcel(model);

                if (dt != null && dt.Rows.Count > 0)
                {

                    DateTime currentDateTime = DateTime.Now;
                    string filename = "COA Trend Analysis " + currentDateTime.ToString("dd-MM-yyyy");

                    IXLWorkbook workbook = new XLWorkbook();
                    IXLWorksheet ws = workbook.Worksheets.Add("sheet1");

                    ws.Range(1, 1, 1, dt.Columns.Count).Merge();
                    ws.Cell(1, 1).Value = "TPM COA - Raw Materials Trend Analysis";
                    ws.Cell(1, 1).Style.Font.Bold = true;

                    ws.Cell(3, 1).Value = "Date From : ";
                    ws.Cell(3, 1).Style.Font.Bold = true;
                    ws.Cell(3, 2).Value = model.DATE_FROM;

                    ws.Cell(3, 3).Value = "Date To : ";
                    ws.Cell(3, 3).Style.Font.Bold = true;
                    ws.Cell(3, 4).Value = model.DATE_TO;

                    ws.Cell(4, 1).Value = "Material Name : ";
                    ws.Cell(4, 1).Style.Font.Bold = true;
                    ws.Cell(4, 2).Value = dt.Rows[0]["MATERIAL_NAME"].ToString();

                    int colno = 0;
                    for (int i = 1; i < dt.Columns.Count; i++)
                    {
                        colno++;
                        ws.Cell(6, colno).Value = dt.Columns[i].ColumnName.ToString();

                    }

                    ws.Range(6, 1, 6, colno).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range(6, 1, 6, colno).Style.Font.Bold = true;

                    ws.Range(6, 1, 6, colno).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    ws.Range(6, 1, 6, colno).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    int idatarow = 7;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        colno = 1;
                        for (int c = 1; c < dt.Columns.Count; c++)
                        {
                            ws.Cell(idatarow, colno).Value = dt.Rows[i][c].ToString();
                            colno++;
                        }

                        idatarow++;
                    }
                    ws.Columns().AdjustToContents();

                    var httpResponse = Response;
                    httpResponse.Clear();
                    httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    httpResponse.Headers.Append("content-disposition", "attachment;filename=" + filename + ".xlsx");

                    using (MemoryStream tmpMemoryStream = new MemoryStream())
                    {
                        workbook.SaveAs(tmpMemoryStream);
                        tmpMemoryStream.WriteTo(httpResponse.Body);
                        tmpMemoryStream.Close();
                    }
                    // Response.End() removed - not available in .NET Core
                }
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, (HttpContext.Session.GetObject<ACL_UserObj>("AclUser")).USER_ID.ToString());
                err = null;
                //return null;
            }

            return ok;
        }


        #endregion

        #region DDL

        public async Task<JsonResult> GetInspItemDDL(string[] supp, string matid)
        {
            List<SelectListItem> supplier = await dbdal.getInspItem(string.Join(",", supp), matid);
            return Json(supplier);
        }

        [HttpPost]
        public async Task<JsonResult> GetSupplierDDL(string matid)
        {
            var sections = await LoadDllData(0, matid, "COA_SUPPLIER_BY_MAT");
            return Json(sections);
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