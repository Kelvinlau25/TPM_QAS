using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using TPM_QAS.Filters;
using TPM_QAS.Helpers;
using TPM_QAS.Models;
using TPM_QAS.DAL;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using Font = iTextSharp.text.Font;
using OfficeOpenXml.Style;
using System.Threading.Tasks;
using System.Globalization;
using DBModel;
using TPM_QAS.Controllers;
using Image = iTextSharp.text.Image;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Vml.Office;

namespace qas.Models
{
    public class ReportIDSController : Controller
    {
        DB dbmain = new DB();
        REPORT_DAL db = new REPORT_DAL();
        DE_IDS_TRANS_DAL dbdal = new DE_IDS_TRANS_DAL();
        Ora o = new Ora();


        #region W Grade Production Listing
        [SessionExpire]
        public ActionResult W_GRADE_PL()
        {
            W_GRADE_PL model = new W_GRADE_PL();
            ViewBag.ShowReport = "FALSE";
            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> W_GRADE_PL(string id)
        {
            W_GRADE_PL model = new W_GRADE_PL();
            try
            {
                DataTable dtwgradepl = await db.getdtwgradepl();
                List<W_GRADE_PL_DETAIL> ListW_GRADE_PL_DETAIL = new List<W_GRADE_PL_DETAIL>();

                if (dtwgradepl != null)
                {
                    for (int i = 0; i < dtwgradepl.Rows.Count; i++)
                    {
                        string packedDateRaw = dtwgradepl.Rows[i]["PACKED_DATE"].ToString().Trim();
                        string packedDateFormatted;

                        DateTime tempDate;
                        if (DateTime.TryParse(packedDateRaw, out tempDate))
                            packedDateFormatted = tempDate.ToString("dd/MM/yy");
                        else
                            packedDateFormatted = packedDateRaw;

                        ListW_GRADE_PL_DETAIL.Add(new W_GRADE_PL_DETAIL
                        {
                            ITEM_CODE = dtwgradepl.Rows[i]["ITEM_NAME"].ToString().Trim(),
                            ITEM_DESC = dtwgradepl.Rows[i]["ITEM_DECSRIPTION"].ToString().Trim(),
                            LOT_NUMBER = dtwgradepl.Rows[i]["LOT_NUMBER"].ToString().Trim(),
                            PACK_QTY = dtwgradepl.Rows[i]["PACK_QTY"].ToString().Trim(),
                            //PACKED_DATE = dtwgradepl.Rows[i]["PACKED_DATE"].ToString()
                            PACKED_DATE = packedDateFormatted
                        });
                    }
                }

                model.ListW_GRADE_PL_DETAIL = ListW_GRADE_PL_DETAIL;

                ViewBag.ShowReport = "TRUE";
                return View(model);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        #region JEFFREY TESTING
        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> W_GRADE_PL_EXCEL()
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME;

            try
            {
                DataTable dt = await db.getdtwgradepl_Excel();

                if (dt == null || dt.Rows.Count == 0)
                {
                    return Json(new { success = false, message = "No data available to export." });
                }

                byte[] fileBytes;
                using (var ms = new MemoryStream())
                {
                    IXLWorkbook workbook = new XLWorkbook();
                    IXLWorksheet ws = workbook.Worksheets.Add("sheet1");

                    // Add printed date/time at top right
                    string printedOn = "Printed On: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    ws.Range(1, 8, 1, 11).Merge();
                    var cellPrinted = ws.Cell(1, 8);
                    cellPrinted.Value = printedOn;

                    // Styling
                    cellPrinted.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    cellPrinted.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    cellPrinted.Style.Font.Bold = true;
                    cellPrinted.Style.Font.Italic = true;
                    cellPrinted.Style.Font.FontSize = 12;
                    cellPrinted.Style.Font.FontColor = XLColor.Black;

                    ws.Row(1).Height = 22;

                    int colno = 1; // start from column A
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        ws.Cell(3, colno).Value = dt.Columns[i].ColumnName.ToString();
                        colno++;
                    }

                    ws.Range(3, 1, 3, colno - 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range(3, 1, 3, colno - 1).Style.Font.Bold = true;
                    ws.Range(3, 1, 3, colno - 1).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    ws.Range(3, 1, 3, colno - 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    int idatarow = 4;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        colno = 1;
                        for (int c = 0; c < dt.Columns.Count; c++)
                        {
                            if (colno == 6) // Pack Qty
                            {
                                if (double.TryParse(dt.Rows[i][c].ToString(), out double num))
                                    ws.Cell(idatarow, colno).Value = num;
                                else
                                    ws.Cell(idatarow, colno).Value = 0;
                            }
                            else if (colno == 5) // ✅ PACKED_DATE column — format date nicely
                            {
                                if (DateTime.TryParse(dt.Rows[i][c].ToString(), out DateTime dtValue))
                                    ws.Cell(idatarow, colno).Value = dtValue.ToString("yyyy-MM-dd");
                                else
                                    ws.Cell(idatarow, colno).Value = dt.Rows[i][c]?.ToString();
                            }
                            else
                            {
                                ws.Cell(idatarow, colno).Value = dt.Rows[i][c]?.ToString();
                            }

                            colno++;
                        }

                        if (i % 2 == 1)
                            ws.Range(idatarow, 1, idatarow, colno - 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#EEECE1");

                        idatarow++;
                    }

                    // Increase row height for readability
                    ws.Rows(3, idatarow + 3).Height = 22;

                    // Set column widths (manual fine-tuning for A4 fit)
                    ws.Column(1).Width = 8;     // Fisheye
                    ws.Column(2).Width = 13;    // Testing Type
                    ws.Column(3).Width = 38;    // Product Code
                    ws.Column(4).Width = 17;    // Lot No
                    ws.Column(5).Width = 12;    // Packed Date
                    ws.Column(6).Width = 17;    // Pack Qty
                    ws.Column(6).Style.NumberFormat.Format = "#,##0";
                    ws.Column(7).Width = 32;    // Remarks
                    ws.Column(8).Width = 8;     // Draft
                    ws.Column(9).Width = 8;    // Complete
                    ws.Column(10).Width = 8;    // Print
                    ws.Column(11).Width = 8;    // Split

                    // Borders
                    ws.Cells().Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cells().Style.Border.TopBorderColor = XLColor.White;
                    ws.Cells().Style.Border.BottomBorderColor = XLColor.White;
                    ws.Cells().Style.Border.LeftBorderColor = XLColor.White;
                    ws.Cells().Style.Border.RightBorderColor = XLColor.White;

                    ws.Range(3, 1, idatarow - 1, colno - 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Range(3, 1, idatarow - 1, colno - 1).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    ws.Range(3, 1, idatarow - 1, colno - 1).Style.Border.OutsideBorderColor = XLColor.Black;
                    ws.Range(3, 1, idatarow - 1, colno - 1).Style.Border.InsideBorderColor = XLColor.Black;

                    // ✅ Add 3 blank rows after data (follow color pattern)
                    for (int extra = 0; extra < 3; extra++)
                    {
                        int blankRow = idatarow + extra;
                        bool isAlternate = ((blankRow - 4) % 2 == 1);

                        var range = ws.Range(blankRow, 1, blankRow, dt.Columns.Count);
                        range.Style.Fill.BackgroundColor = isAlternate ? XLColor.FromHtml("#EEECE1") : XLColor.White;
                        range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        range.Style.Border.OutsideBorderColor = XLColor.Black;
                        range.Style.Border.InsideBorderColor = XLColor.Black;
                    }

                    // ✅ Print Setup for A4 Landscape
                    ws.PageSetup.PageOrientation = XLPageOrientation.Landscape;
                    ws.PageSetup.PaperSize = XLPaperSize.A4Paper;
                    ws.PageSetup.FitToPages(1, 0); // Fit all columns in one page wide
                    ws.PageSetup.Margins.Top = 0.3;
                    ws.PageSetup.Margins.Bottom = 0.3;
                    ws.PageSetup.Margins.Left = 0.3;
                    ws.PageSetup.Margins.Right = 0.3;
                    ws.PageSetup.CenterHorizontally = true;
                    ws.PageSetup.CenterVertically = false;

                    // 🔁 Repeat header row on every printed page
                    ws.PageSetup.SetRowsToRepeatAtTop(1, 3);

                    workbook.SaveAs(ms);
                    ms.Position = 0;
                    fileBytes = ms.ToArray();
                }

                string fileKey = Guid.NewGuid().ToString();
                TempData[fileKey] = fileBytes;

                return Json(new { success = true, fileKey });
            }
            catch (Exception ex)
            {
                var err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(
                    System.Reflection.MethodBase.GetCurrentMethod().Name,
                    ex,
                    userobj.USER_ID
                );

                return Json(new { success = false, message = "An unexpected error occurred." });
            }
        }
        #endregion

        #region ORIGINAL CODE - JENN HONRG
        //[HttpPost]
        //[SessionExpire]
        //public async Task<ActionResult> W_GRADE_PL_EXCEL()
        //{
        //    ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
        //    string USERID = userobj.EMP_NAME;

        //    try
        //    {
        //        DataTable dt = await db.getdtwgradepl_Excel();

        //        if (dt == null || (dt != null && dt.Rows.Count == 0))
        //        {
        //            return Json(new { success = false, message = "No data available to export." });
        //        }

        //        // Generate Excel bytes (TODO: write actual Excel)
        //        byte[] fileBytes;
        //        using (var ms = new MemoryStream())
        //        {
        //            IXLWorkbook workbook = new XLWorkbook();
        //            IXLWorksheet ws = workbook.Worksheets.Add("sheet1");

        //            int colno = 2; //Let start with B
        //            for (int i = 0; i < dt.Columns.Count; i++)
        //            {
        //                ws.Cell(3, colno).Value = dt.Columns[i].ColumnName.ToString();
        //                colno++;
        //            }

        //            ws.Range(3, 2, 3, colno - 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        //            ws.Range(3, 2, 3, colno - 1).Style.Font.Bold = true;

        //            ws.Range(3, 2, 3, colno - 1).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        //            ws.Range(3, 2, 3, colno - 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        //            int idatarow = 4;
        //            for (int i = 0; i < dt.Rows.Count; i++)
        //            {
        //                colno = 2; //Let start with B
        //                for (int c = 0; c < dt.Columns.Count; c++)
        //                {
        //                    //ws.Cell(idatarow, colno).Value = dt.Rows[i][c].ToString();
        //                    if (colno == 7) //Pack Qty
        //                    {
        //                        if (double.TryParse(dt.Rows[i][c].ToString(), out double num))
        //                            ws.Cell(idatarow, colno).Value = num;
        //                        else
        //                            ws.Cell(idatarow, colno).Value = 0; // fallback
        //                    }
        //                    else
        //                    {
        //                        ws.Cell(idatarow, colno).Value = dt.Rows[i][c]?.ToString();
        //                    }
        //                    colno++;
        //                }
        //                if (i % 2 == 1)
        //                {
        //                    ws.Range(idatarow, 2, idatarow, colno - 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#EEECE1");
        //                }

        //                idatarow++;
        //            }

        //            //ws.Columns().AdjustToContents();
        //            ws.Column(2).Width = 8;     //Fisheye
        //            ws.Column(3).Width = 13;    //Testing Type
        //            ws.Column(4).Width = 38;    //Product Code
        //            ws.Column(5).Width = 17;    //Lot No
        //            ws.Column(6).Width = 12;    //Packed Date
        //            ws.Column(7).Width = 17;    //Pack Qty
        //            ws.Column(7).Style.NumberFormat.Format = "#,##0";
        //            ws.Column(8).Width = 32;    //Remarks
        //            ws.Column(9).Width = 8;     //Draft
        //            ws.Column(10).Width = 8;    //Complete
        //            ws.Column(11).Width = 8;    //Print
        //            ws.Column(12).Width = 8;    //Split

        //            // Make all borders white initially
        //            ws.Cells().Style.Border.TopBorder = XLBorderStyleValues.Thin;
        //            ws.Cells().Style.Border.TopBorderColor = XLColor.White;
        //            ws.Cells().Style.Border.BottomBorderColor = XLColor.White;
        //            ws.Cells().Style.Border.LeftBorderColor = XLColor.White;
        //            ws.Cells().Style.Border.RightBorderColor = XLColor.White;

        //            ws.Range(3, 2, idatarow - 1, colno - 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //            ws.Range(3, 2, idatarow - 1, colno - 1).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        //            ws.Range(3, 2, idatarow - 1, colno - 1).Style.Border.OutsideBorderColor = XLColor.Black;
        //            ws.Range(3, 2, idatarow - 1, colno - 1).Style.Border.InsideBorderColor = XLColor.Black;

        //            // ✅ Add 3 blank rows after the last data row (follow alternating color)
        //            for (int extra = 0; extra < 3; extra++)
        //            {
        //                int blankRow = idatarow + extra;
        //                colno = 2; // start from column B

        //                // determine color pattern same as data rows (odd rows shaded)
        //                bool isAlternate = ((blankRow - 4) % 2 == 1); // since data starts at row 4

        //                if (isAlternate)
        //                    ws.Range(blankRow, 2, blankRow, colno - 1 + dt.Columns.Count).Style.Fill.BackgroundColor = XLColor.FromHtml("#EEECE1");
        //                else
        //                    ws.Range(blankRow, 2, blankRow, colno - 1 + dt.Columns.Count).Style.Fill.BackgroundColor = XLColor.White;

        //                // Apply border styling (same as data rows)
        //                ws.Range(blankRow, 2, blankRow, colno - 1 + dt.Columns.Count).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //                ws.Range(blankRow, 2, blankRow, colno - 1 + dt.Columns.Count).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        //                ws.Range(blankRow, 2, blankRow, colno - 1 + dt.Columns.Count).Style.Border.OutsideBorderColor = XLColor.Black;
        //                ws.Range(blankRow, 2, blankRow, colno - 1 + dt.Columns.Count).Style.Border.InsideBorderColor = XLColor.Black;
        //            }

        //            workbook.SaveAs(ms);
        //            ms.Position = 0;
        //            // Write file into ms here
        //            fileBytes = ms.ToArray();
        //        }

        //        // Store in TempData or cache for download
        //        string fileKey = Guid.NewGuid().ToString();
        //        TempData[fileKey] = fileBytes;

        //        return Json(new { success = true, fileKey });
        //    }
        //    catch (Exception ex)
        //    {
        //        var err = new ErrorLogSys();
        //        await err.ErrorLog_Add_V2(
        //            System.Reflection.MethodBase.GetCurrentMethod().Name,
        //            ex,
        //            userobj.USER_ID
        //        );

        //        return Json(new { success = false, message = "An unexpected error occurred." });
        //    }
        //}
        #endregion

        [HttpGet]
        public ActionResult DownloadWGradeReport(string fileKey)
        {
            if (TempData[fileKey] is byte[] fileBytes)
            {
                DateTime currentDateTime = DateTime.Now;
                string filename = "W Grade Production Listing " + currentDateTime.ToString("dd-MM-yyyy") + ".xlsx";
                return File(
                    fileBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    filename
                );
            }

            return Content("File not found or expired.");
        }
        #endregion


        #region IDS Report
        [SessionExpire]
        public async Task<ActionResult> IDS_REPORT()
        {
            REPORT_IDS_VM model = new REPORT_IDS_VM();
            model.DropdownPropItem = await LoadDllData(0, "", "DE_IDS_REPORT_PROP");
            model.DropdownProdType = await db.GetProdTypeList();
            //ViewBag.prodtype = db.GetProdTypeList();
            HttpContext.Session.SetString("refreshids", 1.ToString() ?? "");
            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> IDS_REPORT(REPORT_IDS_VM m)
        {
            ACL_UserObj userobj = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
            string USERID = userobj.EMP_NAME;
            CommonFunction common = new CommonFunction();

            // Convert to DateTime
            DateTime dateFrom = DateTime.ParseExact(m.DATE_FROM, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime dateTo = DateTime.ParseExact(m.DATE_TO, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            // Create list to hold all dates
            List<string> dateList = new List<string>();
            for (DateTime date = dateFrom; date <= dateTo; date = date.AddDays(1))
            {
                dateList.Add(date.ToString("yyyy-MM-dd"));
            }

            try
            {
                if (dateList.Count > 7)
                {
                    return Json(new { success = false, message = "Date range must within 1 week." });
                }

                bool hasData = false;
                // Generate Excel bytes (TODO: write actual Excel)
                byte[] fileBytes;
                using (var ms = new MemoryStream())
                {
                    IXLWorkbook workbook = new XLWorkbook();

                    foreach (var d in dateList)
                    {
                        var param = new
                        {
                            USER_ID = USERID,
                            DATE_FROM = d,
                            DATE_TO = d,
                            PROD_TYPE = m.PROD_TYPE,
                            PROD_LINE_FROM = m.PROD_LINE_FROM,
                            PROD_LINE_TO = m.PROD_LINE_TO,
                            INC_EXTERNAL = m.INC_EXTERNAL
                        };

                        var reportData = await common.PSP_COMMON_DAPPER<DE_IDS_REPORT>(
                            "PSP_DE_IDS_REPORT",
                            System.Data.CommandType.StoredProcedure,
                            param
                        ) ?? new List<DE_IDS_REPORT>();

                        IXLWorksheet ws = workbook.Worksheets.Add(d);

                        int row = 1;
                        int col = 1;

                        int last_col = 21;
                        if (m.SELECTED_PROP != null && m.SELECTED_PROP.Count > 0)
                        {
                            last_col = 7 + m.SELECTED_PROP.Count;
                        }

                        ws.Range(row, col, row, 3).Merge();
                        ws.Cell(row, col).Value = "TPM QAS Daily Inspection Report";
                        ws.Cell(row, col).Style.Font.Bold = true;
                        ws.Cell(row, col).Style.Font.FontSize = 12;
                        ws.Cell(row, last_col).Value = "Date :";
                        ws.Cell(row, last_col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell(row, last_col).Style.Font.Bold = true;
                        ws.Cell(row, last_col + 1).Value = DateTime.Parse(d).ToString("d-MMM");
                        ws.Cell(row, last_col + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell(row, last_col + 1).Style.Font.Bold = true;

                        List<string> ListPRODLINE = new List<string> { "CAP", "CE" };
                        foreach (string prodline in ListPRODLINE)
                        {
                            List<DE_IDS_REPORT> filteredData = reportData
                                    .Where(item => item.PROD_LINE_NAME == prodline)
                                    .ToList();

                            //filteredData.AddRange(filteredData.ToList());
                            //filteredData.AddRange(filteredData.ToList());
                            //filteredData.AddRange(filteredData.ToList());

                            if (filteredData != null && filteredData.Count > 0)
                            {
                                hasData = true;

                                row++;
                                col = 1;
                                int starting_row = row;
                                ws.Cell(row, col).Value = "Packing";
                                ws.Cell(row + 1, col).Value = "Date";
                                ws.Column(col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                col++;
                                ws.Cell(row, col).Value = "Line";
                                ws.Cell(row + 1, col).Value = prodline + "-";
                                ws.Column(col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                col++;
                                ws.Range(row, col, row + 1, col).Merge();
                                ws.Cell(row, col).Value = "Type";
                                col++;
                                ws.Range(row, col, row + 1, col).Merge();
                                ws.Cell(row, col).Value = "Lot No.";
                                ws.Column(col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                col++;
                                ws.Cell(row, col).Value = "Qty.";
                                ws.Cell(row + 1, col).Value = "mt.";
                                ws.Column(col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                col++;
                                ws.Range(row, col, row + 1, col).Merge();
                                ws.Cell(row, col).Value = "Grade";
                                ws.Column(col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                                if (m.SELECTED_PROP == null || (m.SELECTED_PROP != null && m.SELECTED_PROP.Contains("PROP_YI_PELLET")))
                                {
                                    col++;
                                    ws.Cell(row, col).Value = "YI Pellet";
                                    ws.Cell(row + 1, col).Value = "-";
                                    ws.Column(col).Width = 7.1;    //Prop - YI Pellet
                                }
                                if (m.SELECTED_PROP == null || (m.SELECTED_PROP != null && m.SELECTED_PROP.Contains("PROP_YI_PLATE")))
                                {
                                    col++;
                                    ws.Cell(row, col).Value = "YI Plate";
                                    ws.Cell(row + 1, col).Value = "-";
                                    ws.Column(col).Width = 7.1;    //Prop - YI Plate
                                }
                                if (m.SELECTED_PROP == null || (m.SELECTED_PROP != null && m.SELECTED_PROP.Contains("PROP_MFR")))
                                {
                                    col++;
                                    ws.Cell(row, col).Value = "MFR";
                                    ws.Cell(row + 1, col).Value = "g/10min";
                                    ws.Column(col).Width = 7.1;    //Prop - MFR
                                }
                                if (m.SELECTED_PROP == null || (m.SELECTED_PROP != null && m.SELECTED_PROP.Contains("PROP_CHARPY")))
                                {
                                    col++;
                                    ws.Cell(row, col).Value = "Charpy";
                                    ws.Cell(row + 1, col).Value = "kJ/m2";
                                    ws.Column(col).Width = 7.1;    //Prop - Charpy
                                }
                                if (m.SELECTED_PROP == null || (m.SELECTED_PROP != null && m.SELECTED_PROP.Contains("PROP_DTL")))
                                {
                                    col++;
                                    ws.Cell(row, col).Value = "DTL";
                                    ws.Cell(row + 1, col).Value = "°C";
                                    ws.Column(col).Width = 7.1;   //Prop - DTL
                                }
                                if (m.SELECTED_PROP == null || (m.SELECTED_PROP != null && m.SELECTED_PROP.Contains("PROP_TMODULUS")))
                                {
                                    col++;
                                    ws.Cell(row, col).Value = "T.Modulus";
                                    ws.Cell(row + 1, col).Value = "MPa";
                                    ws.Column(col).Width = 9.1;   //Prop - T.Modulus
                                }
                                if (m.SELECTED_PROP == null || (m.SELECTED_PROP != null && m.SELECTED_PROP.Contains("PROP_HAZE")))
                                {
                                    col++;
                                    ws.Cell(row, col).Value = "Haze";
                                    ws.Cell(row + 1, col).Value = "%";
                                    ws.Column(col).Width = 7.1;   //Prop - Haze
                                }
                                if (m.SELECTED_PROP == null || (m.SELECTED_PROP != null && m.SELECTED_PROP.Contains("PROP_FISHEYE_FILM") && m.SELECTED_PROP.Contains("PROP_FISHEYE_PLATE")))
                                {
                                    col++;
                                    ws.Range(row, col, row, col + 1).Merge();
                                    ws.Cell(row, col).Value = "Fisheye";
                                    ws.Cell(row + 1, col).Value = "Film";
                                    ws.Cell(row + 1, col + 1).Value = "Plate";
                                    col++;
                                    ws.Column(col).Width = 7.1;   //Prop - Fisheye Film
                                    ws.Column(col).Width = 7.1;   //Prop - Fisheye Plate
                                }
                                else
                                {
                                    if (m.SELECTED_PROP.Contains("PROP_FISHEYE_FILM"))
                                    {
                                        col++;
                                        ws.Cell(row, col).Value = "Fisheye";
                                        ws.Cell(row + 1, col).Value = "Film";
                                        ws.Column(col).Width = 7.1;   //Prop - Fisheye Film
                                    }
                                    if (m.SELECTED_PROP.Contains("PROP_FISHEYE_PLATE"))
                                    {
                                        col++;
                                        ws.Cell(row, col).Value = "Fisheye";
                                        ws.Cell(row + 1, col).Value = "Plate";
                                        ws.Column(col).Width = 7.1;   //Prop - Fisheye Plate
                                    }
                                }
                                if (m.SELECTED_PROP == null || (m.SELECTED_PROP != null && m.SELECTED_PROP.Contains("PROP_TWINS")))
                                {
                                    col++;
                                    ws.Cell(row, col).Value = "Twins";
                                    ws.Cell(row + 1, col).Value = "%";
                                    ws.Column(col).Width = 7.1;   //Prop - Twins
                                }
                                if (m.SELECTED_PROP == null || (m.SELECTED_PROP != null && m.SELECTED_PROP.Contains("PROP_POWDER")))
                                {
                                    col++;
                                    ws.Cell(row, col).Value = "Powder";
                                    ws.Cell(row + 1, col).Value = "% wt";
                                    ws.Column(col).Width = 7.1;   //Prop - Powder
                                }
                                if (m.SELECTED_PROP == null || (m.SELECTED_PROP != null && m.SELECTED_PROP.Contains("PROP_DENSITY")))
                                {
                                    col++;
                                    ws.Cell(row, col).Value = "Density";
                                    ws.Cell(row + 1, col).Value = "kg/m³";
                                    ws.Column(col).Width = 7.1;   //Prop - Desnsity
                                }
                                if (m.SELECTED_PROP == null || (m.SELECTED_PROP != null && m.SELECTED_PROP.Contains("PROP_GLASS")))
                                {
                                    col++;
                                    ws.Cell(row, col).Value = "Glass/F";
                                    ws.Cell(row + 1, col).Value = "% wt";
                                    ws.Column(col).Width = 7.1;   //Prop - Glass
                                }

                                col++;
                                ws.Range(row, col, row + 1, col).Merge();
                                ws.Cell(row, col).Value = "PCL Remark";
                                ws.Cell(row, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                ws.Column(col).Width = 20;

                                col++;
                                ws.Range(row, col, row, col + 1).Merge();
                                ws.Cell(row, col).Value = "Abnormality if any,";
                                ws.Range(row + 1, col, row + 1, col + 1).Merge();
                                ws.Cell(row + 1, col).Value = "Remarks";
                                ws.Column(col).Width = 18; // 13;    //Remarks
                                ws.Column(col + 1).Width = 18; // 13;    //Remarks


                                ws.Range(row, 1, row + 1, last_col + 1).Style.Font.Bold = true;
                                if (prodline == "CAP")
                                {
                                    ws.Range(row, 1, row + 1, last_col + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#000080");
                                }
                                else
                                {
                                    ws.Range(row, 1, row + 1, last_col + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#000000");
                                }
                                ws.Range(row, 1, row + 1, last_col + 1).Style.Font.FontColor = XLColor.White;
                                ws.Range(row, 1, row + 1, last_col + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                ws.Range(row, 1, row + 1, last_col + 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                ws.Range(row, 1, row + 1, last_col + 1).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                                ws.Range(row, 1, row + 1, last_col + 1).Style.Border.InsideBorderColor = XLColor.White;

                                ws.Range(row, 1, row + 1, 1).Style.Border.InsideBorder = XLBorderStyleValues.None; //Packing Date
                                ws.Range(row, last_col, row + 1, last_col + 1).Style.Border.InsideBorder = XLBorderStyleValues.None; //Remark

                                // 🔁 Repeat header row on every printed page
                                ws.PageSetup.SetRowsToRepeatAtTop(row, row + 1);

                                row++;
                                string currentProdLot = null;
                                string prevProdLot = null;
                                string nextProdLot = null;
                                string currentGrade = null;
                                string prevGrade = null;
                                string nextGrade = null;
                                string currentGroup = null;
                                string prevGroup = null;
                                string nextGroup = null;
                                if (filteredData != null && filteredData.Count > 0)
                                {
                                    for (int i = 0; i < filteredData.Count; i++)
                                    {
                                        DE_IDS_REPORT item = filteredData[i];

                                        currentProdLot = filteredData[i].PROD_TYPE + "|" + filteredData[i].LOT_NO;
                                        currentGrade = filteredData[i].GRADE;
                                        currentGroup = filteredData[i].IDS_GROUP;
                                        // prev exists only when i > 0
                                        prevProdLot = (i > 0)
                                            ? filteredData[i - 1].PROD_TYPE + "|" + filteredData[i - 1].LOT_NO
                                            : null;
                                        prevGrade = (i > 0)
                                            ? filteredData[i - 1].GRADE
                                            : null;
                                        prevGroup = (i > 0)
                                            ? filteredData[i - 1].IDS_GROUP
                                            : null;
                                        // next exists only when i < last index
                                        nextProdLot = (i < filteredData.Count - 1)
                                            ? filteredData[i + 1].PROD_TYPE + "|" + filteredData[i + 1].LOT_NO
                                            : null;
                                        nextGrade = (i < filteredData.Count - 1)
                                            ? filteredData[i + 1].GRADE
                                            : null;
                                        nextGroup = (i < filteredData.Count - 1)
                                            ? filteredData[i + 1].IDS_GROUP
                                            : null;

                                        bool showLabel = true;
                                        if (currentGrade == "NG" && (prevProdLot == currentProdLot))
                                        {
                                            showLabel = false;
                                        }

                                        row++;
                                        col = 1;
                                        ws.Cell(row, col).Value = (showLabel) ? item.PACKING_DATE : "";
                                        ws.Cell(row, col).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                        ws.Cell(row, col).Style.Border.LeftBorderColor = XLColor.Black;
                                        ws.Cell(row, col).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                        ws.Cell(row, col).Style.Border.RightBorderColor = XLColor.Black;
                                        col++;
                                        ws.Cell(row, col).Value = (showLabel) ? item.PROD_LINE : "";
                                        ws.Cell(row, col).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                        ws.Cell(row, col).Style.Border.LeftBorderColor = XLColor.Black;
                                        ws.Cell(row, col).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                        ws.Cell(row, col).Style.Border.RightBorderColor = XLColor.Black;
                                        col++;
                                        ws.Cell(row, col).Value = (showLabel) ? item.PROD_TYPE : "";
                                        ws.Cell(row, col).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                        ws.Cell(row, col).Style.Border.LeftBorderColor = XLColor.Black;
                                        ws.Cell(row, col).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                        ws.Cell(row, col).Style.Border.RightBorderColor = XLColor.Black;
                                        col++;
                                        ws.Cell(row, col).Value = (showLabel) ? item.LOT_NO : "";
                                        ws.Cell(row, col).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                        ws.Cell(row, col).Style.Border.LeftBorderColor = XLColor.Black;
                                        ws.Cell(row, col).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                        ws.Cell(row, col).Style.Border.RightBorderColor = XLColor.Black;
                                        col++;
                                        ws.Cell(row, col).Value = decimal.TryParse(item.QTY_MT, out var v) ? v : XLCellValue.FromObject(item.QTY_MT);
                                        ws.Cell(row, col).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                        ws.Cell(row, col).Style.Border.LeftBorderColor = XLColor.Black;
                                        ws.Cell(row, col).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                        ws.Cell(row, col).Style.Border.RightBorderColor = XLColor.Black;
                                        col++;
                                        ws.Cell(row, col).Value = item.GRADE;
                                        ws.Cell(row, col).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                        ws.Cell(row, col).Style.Border.LeftBorderColor = XLColor.Black;
                                        ws.Cell(row, col).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                        ws.Cell(row, col).Style.Border.RightBorderColor = XLColor.Black;

                                        if (item.IDS_GROUP == "FB")
                                        {
                                            ws.Row(row).Style.Font.FontColor = XLColor.Blue;
                                        }
                                        if (item.GRADE == "R" || item.GRADE == "NG")
                                        {
                                            ws.Row(row).Style.Font.FontColor = XLColor.Red;
                                        }
                                        
                                        if (m.SELECTED_PROP == null || (m.SELECTED_PROP != null && m.SELECTED_PROP.Contains("PROP_YI_PELLET")))
                                        {
                                            col++;
                                            if (item.PROP_YI_PELLET?.Contains(">") == true || item.PROP_YI_PELLET?.Contains("<") == true)
                                            {
                                                var richText = ws.Cell(row, col).GetRichText();
                                                if (item.PROP_YI_PELLET?.Contains(">") == true)
                                                {
                                                    richText.AddText("↑ ").SetFontColor(XLColor.Red);
                                                }
                                                if (item.PROP_YI_PELLET?.Contains("<") == true)
                                                {
                                                    richText.AddText("↓ ").SetFontColor(XLColor.Red);
                                                }
                                                richText.AddText(item.PROP_YI_PELLET?.Replace(">", "").Replace("<", "").Trim()).SetFontColor(XLColor.Black);
                                            }
                                            else
                                            {
                                                //ws.Cell(row, col).Value = decimal.TryParse(item.PROP_YI, out var v1) ? (v1 == 0 ? XLCellValue.FromObject("") : v1) : XLCellValue.FromObject(item.PROP_YI); // item.PROP_YI == 0 ? Blank.Value : XLCellValue.FromObject(item.PROP_YI);
                                                var propVal = item.PROP_YI_PELLET?.Trim();
                                                if (string.IsNullOrEmpty(propVal))
                                                {
                                                    ws.Cell(row, col).Value = ""; // empty → empty
                                                }
                                                else if (decimal.TryParse(propVal, out var v1))
                                                {
                                                    ws.Cell(row, col).Value = v1;

                                                    // short way to count decimals and build format
                                                    var decimals = propVal.Contains('.') ? propVal.Split('.')[1].Length : 0;
                                                    ws.Cell(row, col).Style.NumberFormat.Format = decimals > 0 ? "0." + new string('0', decimals) : "0";
                                                }
                                                else
                                                {
                                                    ws.Cell(row, col).Value = ""; // zero or invalid
                                                }
                                            }
                                            ws.Cell(row, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                            ws.Cell(row, col).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                            ws.Cell(row, col).Style.Border.LeftBorderColor = XLColor.Black;
                                            ws.Cell(row, col).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                            ws.Cell(row, col).Style.Border.RightBorderColor = XLColor.Black;
                                        }
                                        if (m.SELECTED_PROP == null || (m.SELECTED_PROP != null && m.SELECTED_PROP.Contains("PROP_YI_PLATE")))
                                        {
                                            col++;
                                            if (item.PROP_YI_PLATE?.Contains(">") == true || item.PROP_YI_PLATE?.Contains("<") == true)
                                            {
                                                var richText = ws.Cell(row, col).GetRichText();
                                                if (item.PROP_YI_PLATE?.Contains(">") == true)
                                                {
                                                    richText.AddText("↑ ").SetFontColor(XLColor.Red);
                                                }
                                                if (item.PROP_YI_PLATE?.Contains("<") == true)
                                                {
                                                    richText.AddText("↓ ").SetFontColor(XLColor.Red);
                                                }
                                                richText.AddText(item.PROP_YI_PLATE?.Replace(">", "").Replace("<", "").Trim()).SetFontColor(XLColor.Black);
                                            }
                                            else
                                            {
                                                //ws.Cell(row, col).Value = decimal.TryParse(item.PROP_YI, out var v1) ? (v1 == 0 ? XLCellValue.FromObject("") : v1) : XLCellValue.FromObject(item.PROP_YI); // item.PROP_YI == 0 ? Blank.Value : XLCellValue.FromObject(item.PROP_YI);
                                                var propVal = item.PROP_YI_PLATE?.Trim();
                                                if (string.IsNullOrEmpty(propVal))
                                                {
                                                    ws.Cell(row, col).Value = ""; // empty → empty
                                                }
                                                else if (decimal.TryParse(propVal, out var v1))
                                                {
                                                    ws.Cell(row, col).Value = v1;

                                                    // short way to count decimals and build format
                                                    var decimals = propVal.Contains('.') ? propVal.Split('.')[1].Length : 0;
                                                    ws.Cell(row, col).Style.NumberFormat.Format = decimals > 0 ? "0." + new string('0', decimals) : "0";
                                                }
                                                else
                                                {
                                                    ws.Cell(row, col).Value = ""; // zero or invalid
                                                }
                                            }
                                            ws.Cell(row, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                            ws.Cell(row, col).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                            ws.Cell(row, col).Style.Border.LeftBorderColor = XLColor.Black;
                                            ws.Cell(row, col).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                            ws.Cell(row, col).Style.Border.RightBorderColor = XLColor.Black;
                                        }
                                        if (m.SELECTED_PROP == null || (m.SELECTED_PROP != null && m.SELECTED_PROP.Contains("PROP_MFR")))
                                        {
                                            col++;
                                            if (item.PROP_MFR?.Contains(">") == true || item.PROP_MFR?.Contains("<") == true)
                                            {
                                                var richText = ws.Cell(row, col).GetRichText();
                                                if (item.PROP_MFR?.Contains(">") == true)
                                                {
                                                    richText.AddText("↑ ").SetFontColor(XLColor.Red);
                                                }
                                                if (item.PROP_MFR?.Contains("<") == true)
                                                {
                                                    richText.AddText("↓ ").SetFontColor(XLColor.Red);
                                                }
                                                richText.AddText(item.PROP_MFR?.Replace(">", "").Replace("<", "").Trim()).SetFontColor(XLColor.Black);
                                            }
                                            else
                                            {
                                                //ws.Cell(row, col).Value = decimal.TryParse(item.PROP_MFR, out var v2) ? (v2 == 0 ? XLCellValue.FromObject("") : v2) : XLCellValue.FromObject(item.PROP_MFR); // item.PROP_MFR == 0 ? Blank.Value : XLCellValue.FromObject(item.PROP_MFR);
                                                var propVal = item.PROP_MFR?.Trim();
                                                if (string.IsNullOrEmpty(propVal))
                                                {
                                                    ws.Cell(row, col).Value = ""; // empty → empty
                                                }
                                                else if (decimal.TryParse(propVal, out var v1))
                                                {
                                                    ws.Cell(row, col).Value = v1;

                                                    // short way to count decimals and build format
                                                    var decimals = propVal.Contains('.') ? propVal.Split('.')[1].Length : 0;
                                                    ws.Cell(row, col).Style.NumberFormat.Format = decimals > 0 ? "0." + new string('0', decimals) : "0";
                                                }
                                                else
                                                {
                                                    ws.Cell(row, col).Value = ""; // zero or invalid
                                                }
                                            }
                                            ws.Cell(row, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                            ws.Cell(row, col).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                            ws.Cell(row, col).Style.Border.LeftBorderColor = XLColor.Black;
                                            ws.Cell(row, col).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                            ws.Cell(row, col).Style.Border.RightBorderColor = XLColor.Black;
                                        }
                                        if (m.SELECTED_PROP == null || (m.SELECTED_PROP != null && m.SELECTED_PROP.Contains("PROP_CHARPY")))
                                        {
                                            col++;
                                            if (item.PROP_CHARPY?.Contains(">") == true || item.PROP_CHARPY?.Contains("<") == true)
                                            {
                                                var richText = ws.Cell(row, col).GetRichText();
                                                if (item.PROP_CHARPY?.Contains(">") == true)
                                                {
                                                    richText.AddText("↑ ").SetFontColor(XLColor.Red);
                                                }
                                                if (item.PROP_CHARPY?.Contains("<") == true)
                                                {
                                                    richText.AddText("↓ ").SetFontColor(XLColor.Red);
                                                }
                                                richText.AddText(item.PROP_CHARPY?.Replace(">", "").Replace("<", "").Trim()).SetFontColor(XLColor.Black);
                                            }
                                            else
                                            {
                                                //ws.Cell(row, col).Value = decimal.TryParse(item.PROP_CHARPY, out var v3) ? (v3 == 0 ? XLCellValue.FromObject("") : v3) : XLCellValue.FromObject(item.PROP_CHARPY); // item.PROP_CHARPY == 0 ? Blank.Value : XLCellValue.FromObject(item.PROP_CHARPY); ;
                                                var propVal = item.PROP_CHARPY?.Trim();
                                                if (string.IsNullOrEmpty(propVal))
                                                {
                                                    ws.Cell(row, col).Value = ""; // empty → empty
                                                }
                                                else if(decimal.TryParse(propVal, out var v1))
                                                {
                                                    ws.Cell(row, col).Value = v1;

                                                    // short way to count decimals and build format
                                                    var decimals = propVal.Contains('.') ? propVal.Split('.')[1].Length : 0;
                                                    ws.Cell(row, col).Style.NumberFormat.Format = decimals > 0 ? "0." + new string('0', decimals) : "0";
                                                }
                                                else
                                                {
                                                    ws.Cell(row, col).Value = ""; // zero or invalid
                                                }
                                            }
                                            ws.Cell(row, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                            ws.Cell(row, col).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                            ws.Cell(row, col).Style.Border.LeftBorderColor = XLColor.Black;
                                            ws.Cell(row, col).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                            ws.Cell(row, col).Style.Border.RightBorderColor = XLColor.Black;
                                        }
                                        if (m.SELECTED_PROP == null || (m.SELECTED_PROP != null && m.SELECTED_PROP.Contains("PROP_DTL")))
                                        {
                                            col++;
                                            if (item.PROP_DTL?.Contains(">") == true || item.PROP_DTL?.Contains("<") == true)
                                            {
                                                var richText = ws.Cell(row, col).GetRichText();
                                                if (item.PROP_DTL?.Contains(">") == true)
                                                {
                                                    richText.AddText("↑ ").SetFontColor(XLColor.Red);
                                                }
                                                if (item.PROP_DTL?.Contains("<") == true)
                                                {
                                                    richText.AddText("↓ ").SetFontColor(XLColor.Red);
                                                }
                                                richText.AddText(item.PROP_DTL?.Replace(">", "").Replace("<", "").Trim()).SetFontColor(XLColor.Black);
                                            }
                                            else
                                            {
                                                //ws.Cell(row, col).Value = decimal.TryParse(item.PROP_DTL, out var v4) ? (v4 == 0 ? XLCellValue.FromObject("") : v4) : XLCellValue.FromObject(item.PROP_DTL); // item.PROP_DTL == 0 ? Blank.Value : XLCellValue.FromObject(item.PROP_DTL);
                                                var propVal = item.PROP_DTL?.Trim();
                                                if (string.IsNullOrEmpty(propVal))
                                                {
                                                    ws.Cell(row, col).Value = ""; // empty → empty
                                                }
                                                else if(decimal.TryParse(propVal, out var v1))
                                                {
                                                    ws.Cell(row, col).Value = v1;

                                                    // short way to count decimals and build format
                                                    var decimals = propVal.Contains('.') ? propVal.Split('.')[1].Length : 0;
                                                    ws.Cell(row, col).Style.NumberFormat.Format = decimals > 0 ? "0." + new string('0', decimals) : "0";
                                                }
                                                else
                                                {
                                                    ws.Cell(row, col).Value = ""; // zero or invalid
                                                }
                                            }
                                            ws.Cell(row, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                            ws.Cell(row, col).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                            ws.Cell(row, col).Style.Border.LeftBorderColor = XLColor.Black;
                                            ws.Cell(row, col).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                            ws.Cell(row, col).Style.Border.RightBorderColor = XLColor.Black;
                                        }
                                        if (m.SELECTED_PROP == null || (m.SELECTED_PROP != null && m.SELECTED_PROP.Contains("PROP_TMODULUS")))
                                        {
                                            col++;
                                            if (item.PROP_TMODULUS?.Contains(">") == true || item.PROP_TMODULUS?.Contains("<") == true)
                                            {
                                                var richText = ws.Cell(row, col).GetRichText();
                                                if (item.PROP_TMODULUS?.Contains(">") == true)
                                                {
                                                    richText.AddText("↑ ").SetFontColor(XLColor.Red);
                                                }
                                                if (item.PROP_TMODULUS?.Contains("<") == true)
                                                {
                                                    richText.AddText("↓ ").SetFontColor(XLColor.Red);
                                                }
                                                richText.AddText(item.PROP_TMODULUS?.Replace(">", "").Replace("<", "").Trim()).SetFontColor(XLColor.Black);
                                            }
                                            else
                                            {
                                                //ws.Cell(row, col).Value = decimal.TryParse(item.PROP_TMODULUS, out var v6) ? (v6 == 0 ? XLCellValue.FromObject("") : v6) : XLCellValue.FromObject(item.PROP_TMODULUS); // item.PROP_TMODULUS == 0 ? Blank.Value : XLCellValue.FromObject(item.PROP_TMODULUS);
                                                var propVal = item.PROP_TMODULUS?.Trim();
                                                if (string.IsNullOrEmpty(propVal))
                                                {
                                                    ws.Cell(row, col).Value = ""; // empty → empty
                                                }
                                                else if(decimal.TryParse(propVal, out var v1))
                                                {
                                                    ws.Cell(row, col).Value = v1;

                                                    // short way to count decimals and build format
                                                    var decimals = propVal.Contains('.') ? propVal.Split('.')[1].Length : 0;
                                                    ws.Cell(row, col).Style.NumberFormat.Format = decimals > 0 ? "0." + new string('0', decimals) : "0";
                                                }
                                                else
                                                {
                                                    ws.Cell(row, col).Value = ""; // zero or invalid
                                                }
                                            }
                                            ws.Cell(row, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                            ws.Cell(row, col).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                            ws.Cell(row, col).Style.Border.LeftBorderColor = XLColor.Black;
                                            ws.Cell(row, col).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                            ws.Cell(row, col).Style.Border.RightBorderColor = XLColor.Black;
                                        }
                                        if (m.SELECTED_PROP == null || (m.SELECTED_PROP != null && m.SELECTED_PROP.Contains("PROP_HAZE")))
                                        {
                                            col++;
                                            if (item.PROP_HAZE?.Contains(">") == true || item.PROP_HAZE?.Contains("<") == true)
                                            {
                                                var richText = ws.Cell(row, col).GetRichText();
                                                if (item.PROP_HAZE?.Contains(">") == true)
                                                {
                                                    richText.AddText("↑ ").SetFontColor(XLColor.Red);
                                                }
                                                if (item.PROP_HAZE?.Contains("<") == true)
                                                {
                                                    richText.AddText("↓ ").SetFontColor(XLColor.Red);
                                                }
                                                richText.AddText(item.PROP_HAZE?.Replace(">", "").Replace("<", "").Trim()).SetFontColor(XLColor.Black);
                                            }
                                            else
                                            {
                                                //ws.Cell(row, col).Value = decimal.TryParse(item.PROP_HAZE, out var v7) ? (v7 == 0 ? XLCellValue.FromObject("") : v7) : XLCellValue.FromObject(item.PROP_HAZE); // item.PROP_HAZE == 0 ? Blank.Value : XLCellValue.FromObject(item.PROP_HAZE);
                                                var propVal = item.PROP_HAZE?.Trim();
                                                if (string.IsNullOrEmpty(propVal))
                                                {
                                                    ws.Cell(row, col).Value = ""; // empty → empty
                                                }
                                                else if(decimal.TryParse(propVal, out var v1))
                                                {
                                                    ws.Cell(row, col).Value = v1;

                                                    // short way to count decimals and build format
                                                    var decimals = propVal.Contains('.') ? propVal.Split('.')[1].Length : 0;
                                                    ws.Cell(row, col).Style.NumberFormat.Format = decimals > 0 ? "0." + new string('0', decimals) : "0";
                                                }
                                                else
                                                {
                                                    ws.Cell(row, col).Value = ""; // zero or invalid
                                                }
                                            }
                                            ws.Cell(row, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                            ws.Cell(row, col).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                            ws.Cell(row, col).Style.Border.LeftBorderColor = XLColor.Black;
                                            ws.Cell(row, col).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                            ws.Cell(row, col).Style.Border.RightBorderColor = XLColor.Black;
                                        }
                                        if (m.SELECTED_PROP == null || (m.SELECTED_PROP != null && m.SELECTED_PROP.Contains("PROP_FISHEYE_FILM")))
                                        {
                                            col++;
                                            if (item.PROP_FISHEYE_FILM?.Contains(">") == true || item.PROP_FISHEYE_FILM?.Contains("<") == true)
                                            {
                                                var richText = ws.Cell(row, col).GetRichText();
                                                if (item.PROP_FISHEYE_FILM?.Contains(">") == true)
                                                {
                                                    richText.AddText("↑ ").SetFontColor(XLColor.Red);
                                                }
                                                if (item.PROP_FISHEYE_FILM?.Contains("<") == true)
                                                {
                                                    richText.AddText("↓ ").SetFontColor(XLColor.Red);
                                                }
                                                richText.AddText(item.PROP_FISHEYE_FILM?.Replace(">", "").Replace("<", "").Trim()).SetFontColor(XLColor.Black);
                                            }
                                            else
                                            {
                                                //ws.Cell(row, col).Value = decimal.TryParse(item.PROP_FISHEYE_FILM, out var v8) ? (v8 == 0 ? XLCellValue.FromObject("") : v8) : XLCellValue.FromObject(item.PROP_FISHEYE_FILM); // item.PROP_FISHEYE_FILM == 0 ? Blank.Value : XLCellValue.FromObject(item.PROP_FISHEYE_FILM);
                                                var propVal = item.PROP_FISHEYE_FILM?.Trim();
                                                if (string.IsNullOrEmpty(propVal))
                                                {
                                                    ws.Cell(row, col).Value = ""; // empty → empty
                                                }
                                                else if(decimal.TryParse(propVal, out var v1))
                                                {
                                                    ws.Cell(row, col).Value = v1;

                                                    // short way to count decimals and build format
                                                    var decimals = propVal.Contains('.') ? propVal.Split('.')[1].Length : 0;
                                                    ws.Cell(row, col).Style.NumberFormat.Format = decimals > 0 ? "0." + new string('0', decimals) : "0";
                                                }
                                                else
                                                {
                                                    ws.Cell(row, col).Value = ""; // zero or invalid
                                                }
                                            }
                                            ws.Cell(row, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                            ws.Cell(row, col).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                            ws.Cell(row, col).Style.Border.LeftBorderColor = XLColor.Black;
                                            ws.Cell(row, col).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                            ws.Cell(row, col).Style.Border.RightBorderColor = XLColor.Black;
                                        }
                                        if (m.SELECTED_PROP == null || (m.SELECTED_PROP != null && m.SELECTED_PROP.Contains("PROP_FISHEYE_PLATE")))
                                        {
                                            col++;
                                            if (item.PROP_FISHEYE_PLATE?.Contains(">") == true || item.PROP_FISHEYE_PLATE?.Contains("<") == true)
                                            {
                                                var richText = ws.Cell(row, col).GetRichText();
                                                if (item.PROP_FISHEYE_PLATE?.Contains(">") == true)
                                                {
                                                    richText.AddText("↑ ").SetFontColor(XLColor.Red);
                                                }
                                                if (item.PROP_FISHEYE_PLATE?.Contains("<") == true)
                                                {
                                                    richText.AddText("↓ ").SetFontColor(XLColor.Red);
                                                }
                                                richText.AddText(item.PROP_FISHEYE_PLATE?.Replace(">", "").Replace("<", "").Trim()).SetFontColor(XLColor.Black);
                                            }
                                            else
                                            {
                                                //ws.Cell(row, col).Value = decimal.TryParse(item.PROP_FISHEYE_PLATE, out var v9) ? (v9 == 0 ? XLCellValue.FromObject("") : v9) : XLCellValue.FromObject(item.PROP_FISHEYE_PLATE); // item.PROP_FISHEYE_PLATE == 0 ? Blank.Value : XLCellValue.FromObject(item.PROP_FISHEYE_PLATE);
                                                var propVal = item.PROP_FISHEYE_PLATE?.Trim();
                                                if (string.IsNullOrEmpty(propVal))
                                                {
                                                    ws.Cell(row, col).Value = ""; // empty → empty
                                                }
                                                else if(decimal.TryParse(propVal, out var v1))
                                                {
                                                    ws.Cell(row, col).Value = v1;

                                                    // short way to count decimals and build format
                                                    var decimals = propVal.Contains('.') ? propVal.Split('.')[1].Length : 0;
                                                    ws.Cell(row, col).Style.NumberFormat.Format = decimals > 0 ? "0." + new string('0', decimals) : "0";
                                                }
                                                else
                                                {
                                                    ws.Cell(row, col).Value = ""; // zero or invalid
                                                }
                                            }
                                            ws.Cell(row, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                            ws.Cell(row, col).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                            ws.Cell(row, col).Style.Border.LeftBorderColor = XLColor.Black;
                                            ws.Cell(row, col).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                            ws.Cell(row, col).Style.Border.RightBorderColor = XLColor.Black;
                                        }
                                        if (m.SELECTED_PROP == null || (m.SELECTED_PROP != null && m.SELECTED_PROP.Contains("PROP_TWINS")))
                                        {
                                            col++;
                                            if (item.PROP_TWINS?.Contains(">") == true || item.PROP_TWINS?.Contains("<") == true)
                                            {
                                                var richText = ws.Cell(row, col).GetRichText();
                                                if (item.PROP_TWINS?.Contains(">") == true)
                                                {
                                                    richText.AddText("↑ ").SetFontColor(XLColor.Red);
                                                }
                                                if (item.PROP_TWINS?.Contains("<") == true)
                                                {
                                                    richText.AddText("↓ ").SetFontColor(XLColor.Red);
                                                }
                                                richText.AddText(item.PROP_TWINS?.Replace(">", "").Replace("<", "").Trim()).SetFontColor(XLColor.Black);
                                            }
                                            else
                                            {
                                                //ws.Cell(row, col).Value = decimal.TryParse(item.PROP_TWINS, out var v10) ? (v10 == 0 ? XLCellValue.FromObject("") : v10) : XLCellValue.FromObject(item.PROP_TWINS); // item.PROP_TWINS == 0 ? Blank.Value : XLCellValue.FromObject(item.PROP_TWINS);
                                                var propVal = item.PROP_TWINS?.Trim();
                                                if (string.IsNullOrEmpty(propVal))
                                                {
                                                    ws.Cell(row, col).Value = ""; // empty → empty
                                                }
                                                else if(decimal.TryParse(propVal, out var v1))
                                                {
                                                    ws.Cell(row, col).Value = v1;

                                                    // short way to count decimals and build format
                                                    var decimals = propVal.Contains('.') ? propVal.Split('.')[1].Length : 0;
                                                    ws.Cell(row, col).Style.NumberFormat.Format = decimals > 0 ? "0." + new string('0', decimals) : "0";
                                                }
                                                else
                                                {
                                                    ws.Cell(row, col).Value = ""; // zero or invalid
                                                }
                                            }
                                            ws.Cell(row, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                            ws.Cell(row, col).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                            ws.Cell(row, col).Style.Border.LeftBorderColor = XLColor.Black;
                                            ws.Cell(row, col).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                            ws.Cell(row, col).Style.Border.RightBorderColor = XLColor.Black;
                                        }
                                        if (m.SELECTED_PROP == null || (m.SELECTED_PROP != null && m.SELECTED_PROP.Contains("PROP_POWDER")))
                                        {
                                            col++;
                                            if (item.PROP_POWDER?.Contains(">") == true || item.PROP_POWDER?.Contains("<") == true)
                                            {
                                                var richText = ws.Cell(row, col).GetRichText();
                                                if (item.PROP_POWDER?.Contains(">") == true)
                                                {
                                                    richText.AddText("↑ ").SetFontColor(XLColor.Red);
                                                }
                                                if (item.PROP_POWDER?.Contains("<") == true)
                                                {
                                                    richText.AddText("↓ ").SetFontColor(XLColor.Red);
                                                }
                                                richText.AddText(item.PROP_POWDER?.Replace(">", "").Replace("<", "").Trim()).SetFontColor(XLColor.Black);
                                            }
                                            else
                                            {
                                                //ws.Cell(row, col).Value = decimal.TryParse(item.PROP_POWDER, out var v11) ? (v11 == 0 ? XLCellValue.FromObject("") : v11) : XLCellValue.FromObject(item.PROP_POWDER); // item.PROP_POWDER == 0 ? Blank.Value : XLCellValue.FromObject(item.PROP_POWDER);
                                                var propVal = item.PROP_POWDER?.Trim();
                                                if (string.IsNullOrEmpty(propVal))
                                                {
                                                    ws.Cell(row, col).Value = ""; // empty → empty
                                                }
                                                else if(decimal.TryParse(propVal, out var v1))
                                                {
                                                    ws.Cell(row, col).Value = v1;

                                                    // short way to count decimals and build format
                                                    var decimals = propVal.Contains('.') ? propVal.Split('.')[1].Length : 0;
                                                    ws.Cell(row, col).Style.NumberFormat.Format = decimals > 0 ? "0." + new string('0', decimals) : "0";
                                                }
                                                else
                                                {
                                                    ws.Cell(row, col).Value = ""; // zero or invalid
                                                }
                                            }
                                            ws.Cell(row, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                            ws.Cell(row, col).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                            ws.Cell(row, col).Style.Border.LeftBorderColor = XLColor.Black;
                                            ws.Cell(row, col).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                            ws.Cell(row, col).Style.Border.RightBorderColor = XLColor.Black;
                                        }
                                        if (m.SELECTED_PROP == null || (m.SELECTED_PROP != null && m.SELECTED_PROP.Contains("PROP_DENSITY")))
                                        {
                                            col++;
                                            if (item.PROP_DENSITY?.Contains(">") == true || item.PROP_DENSITY?.Contains("<") == true)
                                            {
                                                var richText = ws.Cell(row, col).GetRichText();
                                                if (item.PROP_DENSITY?.Contains(">") == true)
                                                {
                                                    richText.AddText("↑ ").SetFontColor(XLColor.Red);
                                                }
                                                if (item.PROP_DENSITY?.Contains("<") == true)
                                                {
                                                    richText.AddText("↓ ").SetFontColor(XLColor.Red);
                                                }
                                                richText.AddText(item.PROP_DENSITY?.Replace(">", "").Replace("<", "").Trim()).SetFontColor(XLColor.Black);
                                            }
                                            else
                                            {
                                                //ws.Cell(row, col).Value = decimal.TryParse(item.PROP_DENSITY, out var v12) ? (v12 == 0 ? XLCellValue.FromObject("") : v12) : XLCellValue.FromObject(item.PROP_DENSITY); // item.PROP_DENSITY == 0 ? Blank.Value : XLCellValue.FromObject(item.PROP_DENSITY);
                                                var propVal = item.PROP_DENSITY?.Trim();
                                                if (string.IsNullOrEmpty(propVal))
                                                {
                                                    ws.Cell(row, col).Value = ""; // empty → empty
                                                }
                                                else if(decimal.TryParse(propVal, out var v1))
                                                {
                                                    ws.Cell(row, col).Value = v1;

                                                    // short way to count decimals and build format
                                                    var decimals = propVal.Contains('.') ? propVal.Split('.')[1].Length : 0;
                                                    ws.Cell(row, col).Style.NumberFormat.Format = decimals > 0 ? "0." + new string('0', decimals) : "0";
                                                }
                                                else
                                                {
                                                    ws.Cell(row, col).Value = ""; // zero or invalid
                                                }
                                            }
                                            ws.Cell(row, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                            ws.Cell(row, col).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                            ws.Cell(row, col).Style.Border.LeftBorderColor = XLColor.Black;
                                            ws.Cell(row, col).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                            ws.Cell(row, col).Style.Border.RightBorderColor = XLColor.Black;
                                        }
                                        if (m.SELECTED_PROP == null || (m.SELECTED_PROP != null && m.SELECTED_PROP.Contains("PROP_GLASS")))
                                        {
                                            col++;
                                            if (item.PROP_GLASS?.Contains(">") == true || item.PROP_GLASS?.Contains("<") == true)
                                            {
                                                var richText = ws.Cell(row, col).GetRichText();
                                                if (item.PROP_GLASS?.Contains(">") == true)
                                                {
                                                    richText.AddText("↑ ").SetFontColor(XLColor.Red);
                                                }
                                                if (item.PROP_GLASS?.Contains("<") == true)
                                                {
                                                    richText.AddText("↓ ").SetFontColor(XLColor.Red);
                                                }
                                                richText.AddText(item.PROP_GLASS?.Replace(">", "").Replace("<", "").Trim()).SetFontColor(XLColor.Black);
                                            }
                                            else
                                            {
                                                //ws.Cell(row, col).Value = decimal.TryParse(item.PROP_GLASS, out var v13) ? (v13 == 0 ? XLCellValue.FromObject("") : v13) : XLCellValue.FromObject(item.PROP_GLASS); // item.PROP_GLASS == 0 ? Blank.Value : XLCellValue.FromObject(item.PROP_GLASS);
                                                var propVal = item.PROP_GLASS?.Trim();
                                                if (string.IsNullOrEmpty(propVal))
                                                {
                                                    ws.Cell(row, col).Value = ""; // empty → empty
                                                }
                                                else if(decimal.TryParse(propVal, out var v1))
                                                {
                                                    ws.Cell(row, col).Value = v1;

                                                    // short way to count decimals and build format
                                                    var decimals = propVal.Contains('.') ? propVal.Split('.')[1].Length : 0;
                                                    ws.Cell(row, col).Style.NumberFormat.Format = decimals > 0 ? "0." + new string('0', decimals) : "0";
                                                }
                                                else
                                                {
                                                    ws.Cell(row, col).Value = ""; // zero or invalid
                                                }
                                            }
                                            ws.Cell(row, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                            ws.Cell(row, col).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                            ws.Cell(row, col).Style.Border.LeftBorderColor = XLColor.Black;
                                            ws.Cell(row, col).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                            ws.Cell(row, col).Style.Border.RightBorderColor = XLColor.Black;
                                        }
                                        col++;
                                        ws.Cell(row, col).Value = item.PCL_RESULT;
                                        ws.Cell(row, col).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                        ws.Cell(row, col).Style.Border.LeftBorderColor = XLColor.Black;
                                        ws.Cell(row, col).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                        ws.Cell(row, col).Style.Border.RightBorderColor = XLColor.Black;

                                        col++;
                                        string remark1 = item.REMARKS;
                                        if (item.GRADE == "W")
                                        {
                                            remark1 = "";
                                        }
                                        ws.Cell(row, col).Value = remark1;
                                        ws.Cell(row, col).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                        ws.Cell(row, col).Style.Border.LeftBorderColor = XLColor.Black;
                                        string remark2 = "";
                                        if (item.GRADE == "W")
                                        {
                                            remark2 = item.REMARKS;
                                        }
                                        ws.Cell(row, col + 1).Value = remark2;
                                        ws.Cell(row, col + 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                        ws.Cell(row, col + 1).Style.Border.RightBorderColor = XLColor.Black;
                                        ws.Cell(row, col + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                        //ws.Range(row, col, row, col + 1).Merge();
                                        //ws.Cell(row, col).Value = item.REMARKS;
                                        //ws.Range(row, col, row, col + 1).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                        //ws.Range(row, col, row, col + 1).Style.Border.LeftBorderColor = XLColor.Black;
                                        //ws.Range(row, col, row, col + 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                        //ws.Range(row, col, row, col + 1).Style.Border.RightBorderColor = XLColor.Black;


                                        if (
                                            (currentGroup == "FB" || prevGroup == "FB" || nextGroup == "FB")
                                            && (prevProdLot == currentProdLot || nextProdLot == currentProdLot))
                                        {
                                            ws.Range(row, 1, row, last_col + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#D9D9D9");
                                        }
                                        if (
                                            (currentGrade == "R" || prevGrade == "R" || nextGrade == "R"
                                            || currentGrade == "NG" || prevGrade == "NG" || nextGrade == "NG")
                                            && (prevProdLot == currentProdLot || nextProdLot == currentProdLot))
                                        {
                                            ws.Range(row, 1, row, last_col + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFF66");
                                        }
                                    }

                                    ws.Range(starting_row, 1, row, last_col + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                    ws.Range(starting_row, 1, row, last_col + 1).Style.Border.OutsideBorderColor = XLColor.Black;

                                    row++;
                                }
                            }

                        }

                        ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Column(1).Width = 9.1;    //Packing Date
                        ws.Column(2).Width = 5.6;    //Line
                        ws.Column(3).Width = 17.6;   //Type
                        ws.Column(4).Width = 12.3;   //Lot No
                        ws.Column(5).Width = 7.8;    //Qty. mt.
                        ws.Column(6).Width = 6.1;    //Grade


                        // Enable wrap text for all cells in the worksheet
                        ws.Cells().Style.Alignment.WrapText = true;
                        ws.Cells().Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        // ✅ Print Setup for A4 Landscape
                        ws.PageSetup.PageOrientation = XLPageOrientation.Landscape;
                        ws.PageSetup.PaperSize = XLPaperSize.A4Paper;
                        ws.PageSetup.FitToPages(1, 0); // Fit all columns in one page wide
                        ws.PageSetup.Margins.Top = 0.3;
                        ws.PageSetup.Margins.Bottom = 0.3;
                        ws.PageSetup.Margins.Left = 0.3;
                        ws.PageSetup.Margins.Right = 0.3;
                        ws.PageSetup.CenterHorizontally = true;
                        ws.PageSetup.CenterVertically = false;
                    }

                    workbook.SaveAs(ms);
                    ms.Position = 0;
                    // Write file into ms here
                    fileBytes = ms.ToArray();
                }

                if (hasData)
                {
                    // Store in TempData or cache for download
                    string fileKey = Guid.NewGuid().ToString();
                    TempData[fileKey] = fileBytes;

                    return Json(new { success = true, fileKey });
                }
                else
                {
                    return Json(new { success = false, message = "No record found." });
                }
                   
            }
            catch (Exception ex)
            {
                var err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(
                    System.Reflection.MethodBase.GetCurrentMethod().Name,
                    ex,
                    userobj.USER_ID
                );

                return Json(new { success = false, message = "An unexpected error occurred." });
            }
        }

        [HttpGet]
        public ActionResult DownloadReport(string datefr, string dateto, string fileKey)
        {
            if (TempData[fileKey] is byte[] fileBytes)
            {
                string formattedDate = "";
                string formattedFrom = datefr;
                string formattedTo = dateto;

                if (DateTime.TryParse(datefr, out DateTime dateFrom))
                {
                    formattedFrom = dateFrom.ToString("yyMMdd");
                }

                if (DateTime.TryParse(dateto, out DateTime dateTo))
                {
                    formattedTo = dateTo.ToString("yyMMdd");
                }

                if (formattedFrom == formattedTo)
                {
                    formattedDate = formattedFrom;
                }
                else
                {
                    formattedDate = $"{formattedFrom}-{formattedTo}";
                }

                DateTime currentDateTime = DateTime.Now;
                //string filename = "TPM QAS Daily Inspection Report " + currentDateTime.ToString("dd-MM-yyyy") + ".xlsx";
                string filename = $"TPM QAS Daily Inspection Report {formattedDate}.xlsx";
                return File(
                    fileBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    filename
                );
            }

            return Content("File not found or expired.");
        }
        #endregion


        private async Task<List<SelectListItem>> LoadDllData(int ID, string act, string category)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();

            DataTable dt = await db.getDllData(ID, act, category);
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

    }
}