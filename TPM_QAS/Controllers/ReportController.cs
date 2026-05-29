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
using System.Web.UI.WebControls;
using TPM_QAS.Controllers;
using Image = iTextSharp.text.Image;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using static Azure.Core.HttpHeader;

namespace qas.Models
{

    public class ReportController : Controller
    {
        DB dbmain = new DB(); 
        REPORT_DAL db = new REPORT_DAL();
        DE_IDS_TRANS_DAL dbdal = new DE_IDS_TRANS_DAL();
        AzureBlobHelper blob = new AzureBlobHelper();
        CommonFunction common = new CommonFunction();
        Ora o = new Ora();

        [SessionExpire]
        public ActionResult COA_GRN()
        {
            
            COAGRN model = new COAGRN();
            ViewBag.ShowReport = "FALSE";
            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> COA_GRN(string id)
        {
            try
            {
                DataTable dtungraded = await db.getdtcoa("N"); //db.getUngradedGRN();
                DataTable dtgraded = await db.getdtcoa("Y");
                List<COAGRNDETAIL> coagrngraded = new List<COAGRNDETAIL>();
                List<COAGRNDETAIL> coagrnungraded = new List<COAGRNDETAIL>();

                if (dtungraded != null)
                {
                    for (int i = 0; i < dtungraded.Rows.Count; i++)
                    {

                        coagrnungraded.Add(new COAGRNDETAIL
                        {
                            GRN_DATE = Convert.ToDateTime(dtungraded.Rows[i]["GRN_DATE"].ToString()),
                            ITEM_CODE = dtungraded.Rows[i]["ITEM_CODE"].ToString().Trim(),
                            ITEM_DESC = dtungraded.Rows[i]["ITEM_DESCRIPTION"].ToString().Trim(),
                            VENDOR_LOTNO = dtungraded.Rows[i]["VENDOR_LOT_NO"].ToString().Trim(),
                            LOTQTY = dtungraded.Rows[i]["QUANTITY"].ToString().Trim(),
                            UOM = dtungraded.Rows[i]["UOM"].ToString().Trim(),
                            ORGANIZATION = dtungraded.Rows[i]["ORGANIZATION"].ToString().Trim(),
                        });
                    }
                }

                if (dtgraded != null)
                {
                    for (int i = 0; i < dtgraded.Rows.Count; i++)
                    {
                        coagrngraded.Add(new COAGRNDETAIL
                        {
                            GRN_DATE = Convert.ToDateTime(dtgraded.Rows[i]["GRN_DATE"].ToString()),
                            ITEM_CODE = dtgraded.Rows[i]["ITEM_CODE"].ToString().Trim(),
                            ITEM_DESC = dtgraded.Rows[i]["ITEM_DESCRIPTION"].ToString().Trim(),
                            VENDOR_LOTNO = dtgraded.Rows[i]["VENDOR_LOT_NO"].ToString().Trim(),
                            LOTQTY = dtgraded.Rows[i]["QUANTITY"].ToString().Trim(),
                            UOM = dtgraded.Rows[i]["UOM"].ToString().Trim(),
                            ORGANIZATION = dtgraded.Rows[i]["ORGANIZATION"].ToString().Trim(),
                            GRADING_DATE = Convert.ToDateTime(dtgraded.Rows[i]["GRADING_DATE"].ToString()),
                            GRADING_STATUS = dtgraded.Rows[i]["GRADING_STATUS"].ToString().Trim(),
                        });
                    }
                }

                COAGRN model = new COAGRN();
                model.COAGRNDETAILGRADED = coagrngraded;
                model.COAGRNDETAILUN = coagrnungraded;
                ViewBag.ShowReport = "TRUE";
                return View(model);
            }
            catch (Exception ex)
            {
                return View();

            }

        }

        [HttpGet]
        public async Task<JsonResult> CheckGRNExcel(string type = "Y")
        {
            try
            {
                DataTable dt = await db.getdtcoaExcel(type);

                if (dt != null && dt.Rows.Count > 0)
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "No data available to export." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLogSys err = new ErrorLogSys();
                await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, (Session["AclUser"] as ACL_UserObj).USER_ID.ToString());
                err = null;
                return Json(new { success = false, message = "Error occurred while checking data." }, JsonRequestBehavior.AllowGet);
            }
        }


        public async Task<string> GRNExcel(string type = "Y")
        {
            string ok = "";

            if (type != "")
            {
                try {
                    DataTable dt = await db.getdtcoaExcel(type);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        string db = "";
                        if (type == "Y")
                        {
                            db = "Graded GRN Raw Material";
                        }
                        else
                        {
                            db = "Ungraded GRN Raw Material";
                        }

                        DateTime currentDateTime = DateTime.Now;
                        string filename = db + " " + currentDateTime.ToString("dd-MM-yyyy");

                        IXLWorkbook workbook = new XLWorkbook();
                        IXLWorksheet ws = workbook.Worksheets.Add("sheet1");

                        ws.Range(1, 1, 1, dt.Columns.Count).Merge();
                        ws.Cell(1, 1).Value = db;
                        ws.Cell(1, 1).Style.Font.Bold = true;

                        int colno = 0;
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            colno++;
                            ws.Cell(3, colno).Value = dt.Columns[i].ColumnName.ToString();

                        }

                        ws.Range(3, 1, 3, colno).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Range(3, 1, 3, colno).Style.Font.Bold = true;

                        ws.Range(3, 1, 3, colno).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        ws.Range(3, 1, 3, colno).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        int idatarow = 4;
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            colno = 1;
                            for (int c = 0; c < dt.Columns.Count; c++)
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
                        httpResponse.AddHeader("content-disposition", "attachment;filename=" + filename + ".xlsx");

                        using (MemoryStream tmpMemoryStream = new MemoryStream())
                        {
                            workbook.SaveAs(tmpMemoryStream);
                            tmpMemoryStream.WriteTo(httpResponse.OutputStream);
                            tmpMemoryStream.Close();
                        }
                        httpResponse.End();
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogSys err = new ErrorLogSys();
                    await err.ErrorLog_Add_V2(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, (Session["AclUser"] as ACL_UserObj).USER_ID.ToString());
                    err = null;
                    //return null;
                }
                
            }


            return ok;
        }

        public async Task<ActionResult> Report()
        {
            return View();
        }

        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> Report(string id)
        {
            return File(await Test(id), "application/pdf");
        }


        public async Task<ActionResult> ReportInt(string id)
        {
            return File(await IntReport(id), "application/pdf");
        }

        public async Task<ActionResult> GenerateMultipleReports(List<BULKEXPORTIDS> ids)
        {
            try
            {
                string today = DateTime.Now.ToString("yyyyMMdd");
                for (int i = 0; i < ids.Count; i++)
                {
                    string currids = ids[i].ID_IDS_H.ToString();
                    byte[] pdfBytes = await IntReport(currids, false);

                    if (pdfBytes != null)
                    {

                        //BLOB STORAGE//
                        Dictionary<string, string> fileUpload = new Dictionary<string, string>();
                        string partfilename = "Internal IDS/"+ today +"/"+ ids[i].LOTNO.ToString() + "_" + ids[i].PRODUCT_CODE.ToString() +".pdf";

                        fileUpload = await blob.uploadBlobpdf(partfilename, common.getContainerName(), pdfBytes);
                    }
                }
                return Json(new { message = "PDFs generated" }, JsonRequestBehavior.AllowGet);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { message = ex.Message }, JsonRequestBehavior.AllowGet);

            }
        }

        [SessionExpire]
        public async Task<byte[]> IntReport(string id, bool handleHttpResponse = true)
        {
            // nisa
            // Create path to pdf file
            string filePath = Server.MapPath("\\") + "IDS_INT_REPORT" + ".pdf";

            string username = (Session["AclUser"] as ACL_UserObj).USER_ID.ToString();

            Document doc = new Document();
            doc.SetPageSize(PageSize.A4);
            doc.SetMargins(22, 22, 17, 22); // left, right, top, bottom
            doc.AddAuthor(username);

            FileStream fs = new FileStream(filePath, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            writer.PageEvent = new ITextEvents(username);

            doc.Open();

            try
            {
                BaseColor altRowColor = new BaseColor(227, 227, 227);

                Font normalTextFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);
                Font tablesubTitleFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);
                Font tableTitleFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                Font tableTitleFont11 = FontFactory.GetFont(FontFactory.HELVETICA, 11);
                Font colHeaderFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9);
                Font subTitleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 15);

                PdfPTable dataPdfTable4 = CreateTable(new float[] { 130f, 145f, 145f, 130f }, 550f); // Inspection Details
                PdfPTable dataPdfTable5 = CreateTable(new float[] { 54.6f, 97.29f, 73.47f, 50.63f, 59.57f, 59.57f, 65.52f, 33.75f, 55.6f }, 550f); //Property Test Data

                PdfPTable dataPdfTable6 = CreateTable(new float[] { 226.27f, 25.85f, 117.16f, 25.85f, 154.87f }, 550f); //Yellowness Index
                PdfPTable dataPdfTable6A = CreateTable(new float[] { 51.64f, 61.56f, 61.56f, 51.63f }, 226.27f); //Tonnage
                PdfPTable dataPdfTable6B = CreateTable(new float[] { 58.58f, 58.58f }, 117.16f); //First Bag
                PdfPTable dataPdfTable6C = CreateTable(new float[] { 154.87f }, 154.87f); //QA Remarks

                PdfPTable dataPdfTable3 = CreateTable(new float[] { 550f }, 550f); //Black Speck
                PdfPTable dataPdfTable2 = CreateTable(new float[] { 115f, 115f, 90f, 80f, 150f }, 550f); //Lot Grading Details
                PdfPTable dataPdfTable1 = CreateTable(new float[] { 310f, 15f, 225f }, 550f); ; //Container Table
                PdfPTable dataPdfTable1A = CreateTable(new float[] { 80f, 93f, 69f, 68f }, 310f); //Approval Details
                PdfPTable dataPdfTable1B = CreateTable(new float[] { 55f, 55f, 30f, 35f, 50f }, 220f); //Production Lot Close


                PdfPCell dataCellR1C1 = CreateCell(1, 1);
                PdfPCell dataCellR1C1BC = CreateCell(1, 1);
                PdfPCell dataCellRC = CreateCell(1, 1);
                PdfPCell dataCellR1C1H13 = CreateCell(1, 1, 13f);
                PdfPCell dataCellR1C1H16 = CreateCell(1, 1, 16f);
                PdfPCell dataCellR1C1H18 = CreateCell(1, 1, 18f);
                PdfPCell dataCellR1C1H18T = CreateCell(1, 1, 20f);
                PdfPCell dataCellR1C1H23 = CreateCell(1, 1, 23f);
                PdfPCell dataCellR1C1H24 = CreateCell(1, 1, 24f);
                PdfPCell dataCellR1C1H25 = CreateCell(1, 1, 25f);
                PdfPCell dataCellR1C1H96 = CreateCell(1, 1, 96f);
                PdfPCell dataCellR1C1H165 = CreateCell(1, 1, 165f);

                PdfPCell dataCellR1C1NF = CreateCellNotFixedHeight(1, 1);
                PdfPCell dataCellR1C2 = CreateCell(1, 2);
                PdfPCell dataCellR1C2H13 = CreateCell(1, 2, 13f);
                PdfPCell dataCellR1C2H18 = CreateCell(1, 2, 18f);
                PdfPCell dataCellR1C3 = CreateCell(1, 3);
                PdfPCell dataCellR1C3H12 = CreateCell(1, 3, 12f);
                PdfPCell dataCellR1C4 = CreateCell(1, 4);
                PdfPCell dataCellR1C5 = CreateCell(1, 5);
                PdfPCell dataCellR1C5H12 = CreateCell(1, 5, 12f);
                PdfPCell dataCellR1C5H13 = CreateCell(1, 5, 13f);
                PdfPCell dataCellR1C5H40 = CreateCell(1, 5, 40f);
                PdfPCell dataCellR1C9 = CreateCell(1, 9);
                PdfPCell dataCellR2C1H12 = CreateCell(2, 1, 12);
                PdfPCell dataCellR2C1 = CreateCell(2, 1);
                PdfPCell dataCellR2C2 = CreateCell(2, 2);
                PdfPCell dataCellR3C1 = CreateCell(3, 1);

                #region HEADER
                ////////////////////////////////////////////////// HEADER //////////////////////////////////////////////////////////////////////////
                PdfPTable headerPdfTable = new PdfPTable(new float[] { 82f, 336f, 174f });
                headerPdfTable.TotalWidth = 592f;
                headerPdfTable.LockedWidth = true;
                headerPdfTable.HorizontalAlignment = Element.ALIGN_JUSTIFIED;

                PdfPCell TitleCell = new PdfPCell()
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    Border = Rectangle.NO_BORDER,

                };

                string imagePath = Path.Combine(Server.MapPath("\\Images\\"), "torayv3.png");

                if (System.IO.File.Exists(imagePath))
                {
                    Image img = Image.GetInstance(imagePath);

                    img.ScaleToFit(78f, 70f);
                    PdfPCell imageCell = new PdfPCell(img);
                    imageCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    imageCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    imageCell.Border = Rectangle.NO_BORDER;
                    headerPdfTable.AddCell(imageCell);

                }
                else
                {
                    // If the image doesn't exist, add an empty cell as a placeholder
                    PdfPCell emptyCell = new PdfPCell();
                    emptyCell.Border = Rectangle.NO_BORDER;
                    headerPdfTable.AddCell(emptyCell);
                }

                TitleCell.Phrase = new Phrase("DAILY PROPERTY INSPECTION RESULT (Datalink)", titleFont);
                TitleCell.HorizontalAlignment = Element.ALIGN_CENTER;
                TitleCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                TitleCell.Rowspan = 3;
                TitleCell.Colspan = 1;
                headerPdfTable.AddCell(TitleCell);

                PdfPTable nestedHeaderTable = new PdfPTable(1);
                nestedHeaderTable.WidthPercentage = 100;

                DataTable dt = await dbmain.getDllData(0, "", "IDS_ENTRY_REPORT");
                string doc_name = "";
                string issue_no = "";
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["SUB_CATEGORY"].ToString() == "DOCUMENT NAME")
                        {
                            doc_name = dt.Rows[i]["ITEMS"].ToString();
                        }
                        if (dt.Rows[i]["SUB_CATEGORY"].ToString() == "ISSUE NO")
                        {
                            issue_no = dt.Rows[i]["ITEMS"].ToString();
                        }
                    }
                }

                PdfPCell docNameCell = new PdfPCell(new Phrase("Document Name : " + doc_name, normalTextFont));
                docNameCell.Border = Rectangle.NO_BORDER;
                docNameCell.HorizontalAlignment = Element.ALIGN_LEFT;
                docNameCell.VerticalAlignment = Element.ALIGN_TOP;
                nestedHeaderTable.AddCell(docNameCell);

                PdfPCell issueNoCell = new PdfPCell(new Phrase("Issue No. : " + issue_no, normalTextFont));
                issueNoCell.Border = Rectangle.NO_BORDER;
                issueNoCell.HorizontalAlignment = Element.ALIGN_LEFT;
                issueNoCell.VerticalAlignment = Element.ALIGN_TOP;
                nestedHeaderTable.AddCell(issueNoCell);

                string todayDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                PdfPCell printedDateCell = new PdfPCell(new Phrase("Printed Date. : " + todayDate, normalTextFont));
                printedDateCell.Border = Rectangle.NO_BORDER;
                printedDateCell.HorizontalAlignment = Element.ALIGN_LEFT;
                printedDateCell.VerticalAlignment = Element.ALIGN_TOP;
                nestedHeaderTable.AddCell(printedDateCell);

                // Add the nested table to the main table
                PdfPCell nestedHeaderTableCell = new PdfPCell(nestedHeaderTable);
                nestedHeaderTableCell.Border = Rectangle.NO_BORDER;
                nestedHeaderTableCell.VerticalAlignment = Element.ALIGN_TOP;
                headerPdfTable.AddCell(nestedHeaderTableCell);

                doc.Add(headerPdfTable);

                #endregion HEADER

                #region INSPECTION DETAILS
                ////////////////////////////////////////////////// INSPECTION DETAILS //////////////////////////////////////////////////////////////////////////
                SummaryVM summaryVM = await db.GetSummaryData(id);

                dataPdfTable4.SpacingBefore = 3f;

                dataCellR1C4.Phrase = new Phrase("Inspection Details", subTitleFont);
                dataCellR1C4.PaddingLeft = 6f;
                dataCellR1C4.HorizontalAlignment = Element.ALIGN_LEFT;
                dataCellR1C4.BackgroundColor = altRowColor;
                dataPdfTable4.AddCell(dataCellR1C4);

                dataCellR1C1.Phrase = new Phrase("Product Type", tableTitleFont11);
                dataCellR1C1.PaddingLeft = 6f;
                dataCellR1C1.HorizontalAlignment = Element.ALIGN_LEFT;
                dataPdfTable4.AddCell(dataCellR1C1);
                dataCellR1C1.Phrase = new Phrase(summaryVM.PRODTYPE.ToString(), tableTitleFont11);
                dataCellR1C1.PaddingLeft = 0f;
                dataCellR1C1.HorizontalAlignment = Element.ALIGN_CENTER;
                dataPdfTable4.AddCell(dataCellR1C1);

                dataCellR1C1.Phrase = new Phrase("Packed Date", tableTitleFont11);
                dataCellR1C1.PaddingLeft = 6f;
                dataCellR1C1.HorizontalAlignment = Element.ALIGN_LEFT;
                dataPdfTable4.AddCell(dataCellR1C1);
                DateTime packedDate = DateTime.ParseExact(summaryVM.PACKEDDATE2, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                dataCellR1C1.Phrase = new Phrase(packedDate.ToString("dd/MM/yyyy"), tableTitleFont11);
                dataCellR1C1.PaddingLeft = 0f;
                dataCellR1C1.HorizontalAlignment = Element.ALIGN_CENTER;
                dataPdfTable4.AddCell(dataCellR1C1);

                dataCellR1C1.Phrase = new Phrase("Lot No.", tableTitleFont11);
                dataCellR1C1.PaddingLeft = 6f;
                dataCellR1C1.HorizontalAlignment = Element.ALIGN_LEFT;
                dataPdfTable4.AddCell(dataCellR1C1);
                dataCellR1C1.Phrase = new Phrase(summaryVM.LOTNO.ToString(), tableTitleFont11);
                dataCellR1C1.PaddingLeft = 0f;
                dataCellR1C1.HorizontalAlignment = Element.ALIGN_CENTER;
                dataPdfTable4.AddCell(dataCellR1C1);

                dataCellR1C1.Phrase = new Phrase("Quantity (kgs)", tableTitleFont11);
                dataCellR1C1.PaddingLeft = 6f;
                dataCellR1C1.HorizontalAlignment = Element.ALIGN_LEFT;
                dataPdfTable4.AddCell(dataCellR1C1);
                //dataCellR1C1.Phrase = new Phrase(summaryVM.QUANTITY.ToString(), tableTitleFont);
                int quantity = int.Parse(summaryVM.QUANTITY);
                dataCellR1C1.Phrase = new Phrase(quantity.ToString("N0"), tableTitleFont11);
                dataCellR1C1.PaddingLeft = 0f;
                dataCellR1C1.HorizontalAlignment = Element.ALIGN_CENTER;
                dataPdfTable4.AddCell(dataCellR1C1);

                doc.Add(dataPdfTable4);

                #endregion INSPECTION DETAILS

                #region PROPERTY

                ////////////////////////////////////////////////// Property Test Data //////////////////////////////////////////////////////////////////////////
                dataPdfTable5.SpacingBefore = 9f;

                dataCellR1C9.Phrase = new Phrase("Property Test Data", subTitleFont);
                dataCellR1C9.PaddingLeft = 6f;
                dataCellR1C9.HorizontalAlignment = Element.ALIGN_LEFT;
                dataCellR1C9.BackgroundColor = altRowColor;
                dataPdfTable5.AddCell(dataCellR1C9);

                dataCellR2C2.Phrase = new Phrase("Property", tableTitleFont11);
                dataCellR2C2.HorizontalAlignment = Element.ALIGN_CENTER;
                dataPdfTable5.AddCell(dataCellR2C2);

                dataCellR2C1.Phrase = new Phrase("Machine", tableTitleFont11);
                dataCellR2C1.HorizontalAlignment = Element.ALIGN_CENTER;
                dataPdfTable5.AddCell(dataCellR2C1);

                dataCellR2C1.Phrase = new Phrase("Unit", tableTitleFont11);
                dataCellR2C1.HorizontalAlignment = Element.ALIGN_CENTER;
                dataPdfTable5.AddCell(dataCellR2C1);

                dataCellR1C2.Phrase = new Phrase("Result", tableTitleFont11);
                dataCellR1C2.HorizontalAlignment = Element.ALIGN_CENTER;
                dataPdfTable5.AddCell(dataCellR1C2);

                dataCellR2C1.Phrase = new Phrase("Specification", tableTitleFont);
                dataCellR2C1.HorizontalAlignment = Element.ALIGN_CENTER;
                dataPdfTable5.AddCell(dataCellR2C1);

                dataCellR2C1.Phrase = new Phrase("Grade", tableTitleFont);
                dataCellR2C1.HorizontalAlignment = Element.ALIGN_CENTER;
                dataPdfTable5.AddCell(dataCellR2C1);

                dataCellR2C1.Phrase = new Phrase("COQ. Adj.", tableTitleFont11);
                dataCellR2C1.HorizontalAlignment = Element.ALIGN_CENTER;
                dataPdfTable5.AddCell(dataCellR2C1);

                dataCellR1C1.Phrase = new Phrase("Machine", tableTitleFont);
                dataCellR1C1.HorizontalAlignment = Element.ALIGN_CENTER;
                dataPdfTable5.AddCell(dataCellR1C1);

                dataCellR1C1.Phrase = new Phrase("Regression", tableTitleFont);
                dataCellR1C1.HorizontalAlignment = Element.ALIGN_CENTER;
                dataPdfTable5.AddCell(dataCellR1C1);

                List<MoldAndPropVM> MoldingAndPropertyDataTable = await db.GetMoldingPropertyData(summaryVM.PRODTYPE, summaryVM.LOTNO, "99");

                //MoldingAndPropertyDataTable = MoldingAndPropertyDataTable
                //  .OrderBy(x => x.SEQUENCE)  // Order by sequence first
                //    .GroupBy(x => x.MAIN_PROPERTIES)  // Group by A_PROPERTIES
                //    .SelectMany(group => group.OrderBy(x => x.SEQUENCE))
                //  .ToList();

                //Select distinct MAIN_PROPERTIES and count their occurrences:
                var distinctMainPropertiesWithCounts = MoldingAndPropertyDataTable
                    .GroupBy(item => item.MAIN_PROPERTIES)
                    .Select(group => new
                    {
                        MAIN_PROPERTIES = group.Key,
                        Count = group.Count(),
                        ShowMain = group.Count() > 1 ? "Y" : "N"
                    })
                    .ToList();

                string prevmainprop = "";
                foreach (MoldAndPropVM row in MoldingAndPropertyDataTable)
                {
                    var itemsWithCountGreaterThan1 = distinctMainPropertiesWithCounts.Where(item => item.Count > 1 && item.MAIN_PROPERTIES == row.MAIN_PROPERTIES);
                    if (itemsWithCountGreaterThan1.Any())
                    {
                        foreach (var item in itemsWithCountGreaterThan1)
                        {
                            if (prevmainprop == "" || prevmainprop != item.MAIN_PROPERTIES)
                            {
                                dataCellRC.Phrase = new Phrase(item.MAIN_PROPERTIES, tablesubTitleFont);
                                dataCellRC.Rowspan = item.Count;
                                dataCellRC.Colspan = 1;
                                dataCellRC.HorizontalAlignment = Element.ALIGN_LEFT;
                                dataCellRC.PaddingLeft = 6f;
                                dataPdfTable5.AddCell(dataCellRC);
                                prevmainprop = item.MAIN_PROPERTIES;
                            }
                        }

                        dataCellR1C1.Phrase = new Phrase(row.PROPITEM, tablesubTitleFont);
                        dataCellR1C1.PaddingLeft = 0f;
                        dataCellR1C1.HorizontalAlignment = Element.ALIGN_CENTER;
                        dataPdfTable5.AddCell(dataCellR1C1);
                    }
                    else
                    {
                        if (row.MAIN_PROPERTIES == row.PROPITEM)
                        {
                            dataCellR1C2.Phrase = new Phrase(row.MAIN_PROPERTIES, tablesubTitleFont);
                            dataCellR1C2.PaddingLeft = 6f;
                            dataCellR1C2.HorizontalAlignment = Element.ALIGN_LEFT;
                            dataPdfTable5.AddCell(dataCellR1C2);
                        }
                        else
                        {
                            dataCellR1C1.Phrase = new Phrase(row.MAIN_PROPERTIES, tablesubTitleFont);
                            dataCellR1C1.PaddingLeft = 6f;
                            dataCellR1C1.HorizontalAlignment = Element.ALIGN_LEFT;
                            dataPdfTable5.AddCell(dataCellR1C1);

                            dataCellR1C1.Phrase = new Phrase(row.PROPITEM, tablesubTitleFont);
                            dataCellR1C1.PaddingLeft = 0f;
                            dataCellR1C1.HorizontalAlignment = Element.ALIGN_CENTER;
                            dataPdfTable5.AddCell(dataCellR1C1);
                        }

                    }

                    dataCellRC.PaddingLeft = 0f;
                    dataCellR1C2.PaddingLeft = 0f;
                    dataCellR1C1.PaddingLeft = 0f;

                    dataCellR1C1.Phrase = new Phrase(row.MACHINENAME, tableTitleFont);
                    dataCellR1C1.HorizontalAlignment = Element.ALIGN_CENTER;
                    dataPdfTable5.AddCell(dataCellR1C1);

                    dataCellR1C1.Phrase = new Phrase(row.UNIT, tableTitleFont);
                    dataPdfTable5.AddCell(dataCellR1C1);

                    dataCellR1C1.Phrase = new Phrase(row.AVERAGE, tableTitleFont);
                    dataPdfTable5.AddCell(dataCellR1C1);

                    dataCellR1C1.Phrase = new Phrase(row.REGRESSIONRESULT, tableTitleFont);
                    dataPdfTable5.AddCell(dataCellR1C1);

                    dataCellR1C1NF.Phrase = new Phrase(row.SPECIFICATION, tableTitleFont);
                    dataPdfTable5.AddCell(dataCellR1C1NF);

                    dataCellR1C1.Phrase = new Phrase(row.GRADE, tableTitleFont);
                    dataPdfTable5.AddCell(dataCellR1C1);

                    dataCellR1C1.Phrase = new Phrase(row.COQADJ, tableTitleFont);
                    dataPdfTable5.AddCell(dataCellR1C1);
                }


                doc.Add(dataPdfTable5);
                #endregion PROPERTY

                #region YELLOWNESS INDEX

                ///////////////////////////////////////////////////// YELLOWNESS INDEX //////////////////////////////////////////////////////////////////////////

                // Get Appearance Table

                //GET RATIO
                List<fieldnamemodel> dBSratio = new List<fieldnamemodel>();
                List<fieldnamemodel> dYIratio = new List<fieldnamemodel>();
                List<AppearanceTableModel> appearanceTable = await dbdal.getAppearanceList(id);
                dBSratio = await dbdal.getfielname("E_AppColumn", "BLACK SPECK", summaryVM.PRODTYPE, summaryVM.LOTNO, "", "", "");
                dYIratio = await dbdal.getfielname("E_AppColumn", "YI", summaryVM.PRODTYPE, summaryVM.LOTNO, "", "", "");

                decimal yiratio = Convert.ToDecimal(dYIratio.Where(x => x.fieldname == "YI")
                                      .Select(x => x.calcratio)
                                      .FirstOrDefault());

                List<PropGradeSpecModel> PropGradeSpecList = new List<PropGradeSpecModel>();
                PropGradeSpecList = await dbdal.GetPropGradeSpec(summaryVM.LOTNO, "1");

                string YIGrade = appearanceTable.Where(x => x.Properties == "YI")
                                      .Select(x => x.Mgrade)
                                      .FirstOrDefault();

                DataTable dtYI = await db.GetAppearanceTable(summaryVM.PRODTYPE, summaryVM.LOTNO, 2);
                if (summaryVM.BYI == "Y")
                {
                    dtYI = null;
                }
                DataTable dtDirect = await db.getReportData("", summaryVM.PRODTYPE, summaryVM.LOTNO, "DIRECT");

                DataTable dtApp = await db.getReportData(id, "", "", "APP");

                dataPdfTable3.DeleteBodyRows();
                dataPdfTable3.SpacingBefore = 9f;

                dataCellR1C1.Phrase = new Phrase("Yellowness Index", subTitleFont);
                dataCellR1C1.PaddingLeft = 6f;
                dataCellR1C1.HorizontalAlignment = Element.ALIGN_LEFT;
                dataCellR1C1.BackgroundColor = altRowColor;
                dataCellR1C1.Border = Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                dataPdfTable3.AddCell(dataCellR1C1);

                doc.Add(dataPdfTable3);

                dataCellR1C1.BackgroundColor = null;

                dataCellR2C1H12.Phrase = new Phrase("Tonnage", tableTitleFont11);
                dataCellR2C1H12.HorizontalAlignment = Element.ALIGN_CENTER;
                dataCellR2C1H12.VerticalAlignment = Element.ALIGN_MIDDLE;
                dataPdfTable6A.AddCell(dataCellR2C1H12);

                dataCellR1C2H13.Phrase = new Phrase("Result", tablesubTitleFont);
                dataCellR1C2H13.HorizontalAlignment = Element.ALIGN_CENTER;
                dataCellR1C2H13.VerticalAlignment = Element.ALIGN_MIDDLE;
                dataPdfTable6A.AddCell(dataCellR1C2H13);

                dataCellR2C1H12.Phrase = new Phrase("Grade", tableTitleFont11);
                dataPdfTable6A.AddCell(dataCellR2C1H12);

                dataCellR1C1H13.Phrase = new Phrase("m/c (7)", tablesubTitleFont);
                dataCellR1C1H13.HorizontalAlignment = Element.ALIGN_CENTER;
                dataCellR1C1H13.VerticalAlignment = Element.ALIGN_MIDDLE;
                dataPdfTable6A.AddCell(dataCellR1C1H13);

                dataCellR1C1H13.Phrase = new Phrase("Regression", tablesubTitleFont);
                dataPdfTable6A.AddCell(dataCellR1C1H13);

                if (dtYI != null)
                {
                    for (int i = 0; i < dtYI.Rows.Count; i++)
                    {
                        dataCellR1C1H13.Phrase = new Phrase(dtYI.Rows[i]["TONNAGE"].ToString(), tablesubTitleFont);
                        dataCellR1C1H13.HorizontalAlignment = Element.ALIGN_CENTER;
                        dataCellR1C1H13.VerticalAlignment = Element.ALIGN_MIDDLE;
                        dataPdfTable6A.AddCell(dataCellR1C1H13);

                        decimal yiround;
                        if (dtYI.Rows[i]["GRADE"].ToString() != "NG")
                        {
                            yiround = Convert.ToDecimal(PropGradeSpecList.Where(x => x.PROPERTIES == "YI" && x.GRADE == dtYI.Rows[i]["GRADE"].ToString()
                                                                                    && x.GRADINGIND == "A")
                                      .Select(x => x.ROUNDING)
                                      .FirstOrDefault());
                        }
                        else
                        {
                            yiround = Convert.ToDecimal(PropGradeSpecList.Where(x => x.PROPERTIES == "YI" && x.GRADINGIND == "A")
                                      .Select(x => x.ROUNDING)
                                      .FirstOrDefault());
                        }

                        string formatString = "0." + new string('0', (int)yiround);

                        string formattedYI = dtYI.Rows[i]["YI"] == DBNull.Value || string.IsNullOrWhiteSpace(dtYI.Rows[i]["YI"].ToString())
                        ? ""
                        : Convert.ToDecimal(dtYI.Rows[i]["YI"]).ToString(formatString);

                        dataCellR1C1H13.Phrase = new Phrase(formattedYI, tablesubTitleFont);
                        dataPdfTable6A.AddCell(dataCellR1C1H13);

                        string formattedReg = dtYI.Rows[i]["REGRESSION"] == DBNull.Value || string.IsNullOrWhiteSpace(dtYI.Rows[i]["REGRESSION"].ToString())
                        ? ""
                        : Convert.ToDecimal(dtYI.Rows[i]["REGRESSION"]).ToString(formatString);
                        dataCellR1C1H13.Phrase = new Phrase(formattedReg, tablesubTitleFont);
                        dataPdfTable6A.AddCell(dataCellR1C1H13);

                        dataCellR1C1H13.Phrase = new Phrase(dtYI.Rows[i]["GRADE"].ToString(), tablesubTitleFont);
                        dataPdfTable6A.AddCell(dataCellR1C1H13);
                    }
                }

                for (int i = 0; i < 6; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        dataCellR1C1H13.Phrase = new Phrase(" ", tablesubTitleFont);
                        dataPdfTable6A.AddCell(dataCellR1C1H13);
                    }
                }

                object sumyist = dtYI != null ? dtYI.Compute("SUM([YI])", "") : 0;
                decimal sumYI = decimal.TryParse(sumyist.ToString(), out decimal b) ? Convert.ToDecimal(sumyist) : 0;

                decimal YIAvg = dtYI != null ? (sumYI * yiratio) / (dtYI.Rows.Count) : 0;

                YIAvg = Math.Truncate(YIAvg * 100) / 100;

                object sumREGst = dtYI != null ? dtYI.Compute("SUM([REGRESSION])", "") : 0;
                string sumREGvc = decimal.TryParse(sumREGst.ToString(), out decimal Ct) ? sumREGst.ToString() : "";
                bool regyi = sumREGvc == "" ? false : true;

                decimal sumREG = decimal.TryParse(sumREGst.ToString(), out decimal C) ? Convert.ToDecimal(sumREGst) : 0;

                decimal REGAvg = dtYI != null ? (sumREG) / (dtYI.Rows.Count) : 0;

                REGAvg = Math.Round(REGAvg, 2, MidpointRounding.AwayFromZero);

                decimal yiroundsum = Convert.ToDecimal(PropGradeSpecList.Where(x => x.PROPERTIES == "YI" && x.GRADE == YIGrade
                                                                                && x.GRADINGIND == "A")
                          .Select(x => x.ROUNDING)
                          .FirstOrDefault());

                string formatStringsum = "#,##0." + new string('0', (int)yiroundsum);

                dataCellR1C1H13.Phrase = new Phrase("Average", normalTextFont);
                dataPdfTable6A.AddCell(dataCellR1C1H13);

                if (dtYI != null)
                {
                    dataCellR1C1H13.Phrase = new Phrase(YIAvg.ToString(formatStringsum), tablesubTitleFont);
                }
                else
                {
                    dataCellR1C1H13.Phrase = new Phrase("", tablesubTitleFont);
                }
                dataCellR1C1H13.HorizontalAlignment = Element.ALIGN_CENTER;
                dataCellR1C1H13.VerticalAlignment = Element.ALIGN_MIDDLE;
                dataPdfTable6A.AddCell(dataCellR1C1H13);

                if (regyi && dtYI != null)
                {
                    dataCellR1C1H13.Phrase = new Phrase(REGAvg.ToString(formatStringsum), tablesubTitleFont);
                }
                else
                {
                    dataCellR1C1H13.Phrase = new Phrase("", tablesubTitleFont);
                }
                dataCellR1C1H13.HorizontalAlignment = Element.ALIGN_CENTER;
                dataCellR1C1H13.VerticalAlignment = Element.ALIGN_MIDDLE;
                dataPdfTable6A.AddCell(dataCellR1C1H13);

                dataCellR1C1H13.Phrase = new Phrase(" ", tablesubTitleFont);
                dataPdfTable6A.AddCell(dataCellR1C1H13);


                string yispec = "";

                if (dtYI != null)
                {
                    for (int i = 0; i < dtDirect.Rows.Count; i++)
                    {
                        if (dtDirect.Rows[i]["PROP_ITEM"].ToString() == "YI")
                        {
                            if (yispec == "")
                            {
                                yispec = dtDirect.Rows[i]["SPECIFICATION"] != DBNull.Value ? (dtDirect.Rows[i]["GRADE"].ToString() + " " + dtDirect.Rows[i]["SPECIFICATION"].ToString()) : "";
                            }
                            else
                            {
                                yispec = dtDirect.Rows[i]["SPECIFICATION"] != DBNull.Value ? (yispec + ", " + dtDirect.Rows[i]["GRADE"].ToString() + " " + dtDirect.Rows[i]["SPECIFICATION"].ToString()) : yispec + "";
                            }
                        }
                    }
                }
                else
                {
                    YIGrade = "";
                }

                dataCellR1C1H13.Phrase = new Phrase("Specification", normalTextFont);
                dataPdfTable6A.AddCell(dataCellR1C1H13);
                dataCellR1C3H12.Phrase = new Phrase(yispec, tablesubTitleFont);
                dataCellR1C3H12.HorizontalAlignment = Element.ALIGN_CENTER;
                dataCellR1C3H12.VerticalAlignment = Element.ALIGN_MIDDLE;
                dataPdfTable6A.AddCell(dataCellR1C3H12);

                dataCellR1C1H13.Phrase = new Phrase("Grade", normalTextFont);
                dataPdfTable6A.AddCell(dataCellR1C1H13);
                dataCellR1C3H12.Phrase = new Phrase(YIGrade, tablesubTitleFont);
                dataCellR1C3H12.HorizontalAlignment = Element.ALIGN_CENTER;
                dataCellR1C3H12.VerticalAlignment = Element.ALIGN_MIDDLE;
                dataPdfTable6A.AddCell(dataCellR1C3H12);

                dataCellR1C1H13.Phrase = new Phrase("COQ Adjust", normalTextFont);
                dataPdfTable6A.AddCell(dataCellR1C1H13);
                dataCellR1C3H12.Phrase = new Phrase(summaryVM.COQ_ADJ_RMK.ToString(), tablesubTitleFont);
                dataCellR1C3H12.HorizontalAlignment = Element.ALIGN_LEFT;
                dataCellR1C3H12.VerticalAlignment = Element.ALIGN_MIDDLE;
                dataPdfTable6A.AddCell(dataCellR1C3H12);

                PdfPCell cell6a = new PdfPCell(dataPdfTable6A);
                cell6a.HorizontalAlignment = Element.ALIGN_LEFT;
                cell6a.Border = Rectangle.NO_BORDER;
                dataPdfTable6.AddCell(cell6a);

                dataCellR1C1.Phrase = new Phrase("", tablesubTitleFont);
                dataCellR1C1.BackgroundColor = null;
                dataCellR1C1.Border = Rectangle.NO_BORDER;
                dataPdfTable6.AddCell(dataCellR1C1);

                dataCellR1C1.Border = Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;

                int totalrow = dtYI != null ? dtYI.Rows.Count + 10 : 10;

                DataTable dtFB = await db.getFirstBag(id);

                dataCellR1C1H24.Phrase = new Phrase("First Bag", tableTitleFont);
                dataCellR1C1H24.HorizontalAlignment = Element.ALIGN_CENTER;
                dataCellR1C1H24.VerticalAlignment = Element.ALIGN_MIDDLE;
                dataPdfTable6B.AddCell(dataCellR1C1H24);

                dataCellR1C1H24.Phrase = new Phrase(" ", tableTitleFont);
                dataCellR1C1H24.HorizontalAlignment = Element.ALIGN_CENTER;
                dataCellR1C1H24.VerticalAlignment = Element.ALIGN_MIDDLE;
                dataPdfTable6B.AddCell(dataCellR1C1H24);

                if (dtFB != null)
                {
                    for (int i = 0; i < dtFB.Rows.Count; i++)
                    {
                        dataCellR1C1H24.Phrase = new Phrase(dtFB.Rows[i]["PROPITEM"].ToString(), tableTitleFont);
                        dataPdfTable6B.AddCell(dataCellR1C1H24);
                        dataCellR1C1H24.Phrase = new Phrase(dtFB.Rows[i]["READING"].ToString(), tableTitleFont);
                        dataPdfTable6B.AddCell(dataCellR1C1H24);
                    }

                    totalrow = totalrow - dtFB.Rows.Count;
                }


                for (int i = 0; i < totalrow; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        dataCellR1C1H13.Phrase = new Phrase(" ", tableTitleFont);
                        dataPdfTable6B.AddCell(dataCellR1C1H13);
                    }
                }

                PdfPCell cell6b = new PdfPCell(dataPdfTable6B);
                cell6b.HorizontalAlignment = Element.ALIGN_LEFT;
                cell6b.Border = Rectangle.NO_BORDER;
                dataPdfTable6.AddCell(cell6b);

                dataCellR1C1.Phrase = new Phrase("", tableTitleFont);
                dataCellR1C1.BackgroundColor = null;
                dataCellR1C1.Border = Rectangle.NO_BORDER;
                dataPdfTable6.AddCell(dataCellR1C1);

                dataCellR1C1H24.Phrase = new Phrase("QA Remarks:", tableTitleFont11);
                dataCellR1C1H24.HorizontalAlignment = Element.ALIGN_CENTER;
                dataCellR1C1H24.VerticalAlignment = Element.ALIGN_MIDDLE;
                dataPdfTable6C.AddCell(dataCellR1C1H24);


                dataCellR1C1H165.Phrase = new Phrase(dtApp.Rows[0]["QA_REMARK"].ToString(), tableTitleFont);
                dataCellR1C1H165.VerticalAlignment = Element.ALIGN_TOP;
                dataPdfTable6C.AddCell(dataCellR1C1H165);


                dataCellR1C1.VerticalAlignment = Element.ALIGN_MIDDLE;
                dataCellR1C1.Border = Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;

                PdfPCell cell6c = new PdfPCell(dataPdfTable6C);
                cell6c.HorizontalAlignment = Element.ALIGN_LEFT;
                cell6c.Border = Rectangle.NO_BORDER;
                dataPdfTable6.AddCell(cell6c);

                doc.Add(dataPdfTable6);

                #endregion YELLOW INDEX

                #region BLACKSPECK
                ///////////////////////////////////////////////////// Black Speck //////////////////////////////////////////////////////////////////////////

                doc.SetMargins(22, 22, 22, 22);
                doc.NewPage();

                int[] coltoremove = new int[] { 0 };
                int ij = 1;
                DataTable appearanceDataTable = await db.GetAppearanceTable(summaryVM.PRODTYPE, summaryVM.LOTNO, 1);
                if (appearanceDataTable != null)
                {
                    for (int i = 3; i < appearanceDataTable.Columns.Count - 1; i++)
                    {
                        string columnname = appearanceDataTable.Columns[i].ColumnName;

                        if ((columnname.ToUpper().Contains("ID_IDS_DL")))
                        {
                            DataRow[] haveinfo = appearanceDataTable.Select("ISNULL([" + columnname + "],9999) <> 9999");
                            //DataRow[] haveinfo = appearanceDataTable.Select(exp);
                            if (haveinfo.Length == 0)
                            {
                                Array.Resize(ref coltoremove, ij + 1);
                                coltoremove[ij] = i;
                                ij = ij + 1;
                            }
                        }
                    }


                    Array.Sort(coltoremove);  // Sort in ascending order first
                    Array.Reverse(coltoremove);

                    for (int i = 0; i < coltoremove.Length; i++)
                    {
                        appearanceDataTable.Columns.RemoveAt(coltoremove[i]);
                    }

                    appearanceDataTable.AcceptChanges();
                }
                //550f
                float d = appearanceDataTable != null ? (296 / (appearanceDataTable.Columns.Count - 3)) : (296 / 3);
                float[] arraywidth = new float[appearanceDataTable != null ? appearanceDataTable.Columns.Count : 3];
                arraywidth[0] = 140f;
                arraywidth[1] = 57f;
                if (appearanceDataTable != null)
                {
                    arraywidth[appearanceDataTable.Columns.Count - 1] = 57f;

                    for (int i = 2; i < appearanceDataTable.Columns.Count - 1; i++)
                    {
                        arraywidth[i] = d;
                    }
                }

                PdfPTable dataPdfTblapp = new PdfPTable(arraywidth);
                dataPdfTblapp.TotalWidth = 550f;
                dataPdfTblapp.LockedWidth = true;

                dataCellR1C1.HorizontalAlignment = Element.ALIGN_CENTER;
                dataCellR1C1H23.HorizontalAlignment = Element.ALIGN_CENTER;

                PdfPCell dataCellR1BS = CreateCell(1, appearanceDataTable != null ? appearanceDataTable.Columns.Count : 3);

                dataCellR1BS.Phrase = new Phrase("Black Speck (m/c " + summaryVM.BS_MACH.ToString() + ")", subTitleFont);
                dataCellR1BS.PaddingLeft = 6f;
                dataCellR1BS.HorizontalAlignment = Element.ALIGN_LEFT;
                dataCellR1BS.BackgroundColor = altRowColor;
                dataPdfTblapp.AddCell(dataCellR1BS);
                dataCellR1C1H18.HorizontalAlignment = Element.ALIGN_CENTER;
                dataCellR1C1H18.VerticalAlignment = Element.ALIGN_MIDDLE;
                dataCellR1C1NF.HorizontalAlignment = Element.ALIGN_CENTER;
                dataCellR1C1NF.VerticalAlignment = Element.ALIGN_MIDDLE;
                dataCellR1C1H18T.HorizontalAlignment = Element.ALIGN_CENTER;
                dataCellR1C1H18T.VerticalAlignment = Element.ALIGN_MIDDLE;
                if (appearanceDataTable != null)
                {
                    if (summaryVM.BBS == "Y")
                    {
                        appearanceDataTable.Columns["GRADE"].ReadOnly = false;
                        for (int i = 0; i < appearanceDataTable.Rows.Count; i++)
                        {
                            appearanceDataTable.Rows[i]["GRADE"] = "";
                        }
                        appearanceDataTable.Columns["GRADE"].ReadOnly = true;
                    }

                    for (int i = 0; i < appearanceDataTable.Columns.Count; i++)
                    {
                        dataCellR1C1NF.Phrase = new Phrase(appearanceDataTable.Columns[i].ColumnName, tableTitleFont);
                        dataPdfTblapp.AddCell(dataCellR1C1NF);
                    }

                    string bstext = "";
                    for (int i = 0; i < appearanceDataTable.Rows.Count; i++)
                    {
                        for (int j = 0; j < appearanceDataTable.Columns.Count; j++)
                        {
                            if (j != 0 && j != 1 && j != appearanceDataTable.Columns.Count - 1)
                            {
                                decimal currration = Convert.ToDecimal(dBSratio.Where(x => x.fieldname == appearanceDataTable.Columns[j].ColumnName.ToString())
                                              .Select(x => x.calcratio)
                                              .FirstOrDefault());

                                bstext = appearanceDataTable.Rows[i][j].ToString();

                                if (bstext != null && bstext != "")
                                {
                                    appearanceDataTable.Columns[j].ReadOnly = false;
                                    decimal bscurr = Convert.ToDecimal(appearanceDataTable.Rows[i][j]) * currration;
                                    appearanceDataTable.Rows[i][j] = bscurr.ToString();
                                    bstext = bscurr.ToString();
                                    appearanceDataTable.Columns[j].ReadOnly = true;
                                }

                            }
                            else
                            {
                                bstext = appearanceDataTable.Rows[i][j].ToString();
                            }
                            dataCellR1C1.Phrase = new Phrase(bstext.ToString(), tableTitleFont);
                            dataPdfTblapp.AddCell(dataCellR1C1);
                        }
                    }

                    //Total
                    for (int i = 0; i < appearanceDataTable.Columns.Count; i++)
                    {
                        if (i == 0)
                        {
                            dataCellR1C2H18.Phrase = new Phrase("Total", tableTitleFont11);
                            dataCellR1C2H18.PaddingLeft = 6f;
                            dataCellR1C2H18.HorizontalAlignment = Element.ALIGN_LEFT;
                            dataPdfTblapp.AddCell(dataCellR1C2H18);
                        }
                        else if (i == appearanceDataTable.Columns.Count - 1)
                        {
                            dataCellR1C1BC.Phrase = new Phrase(" ", normalTextFont);
                            dataPdfTblapp.AddCell(dataCellR1C1BC);
                        }
                        else
                        {
                            if (i != 1)
                            {
                                decimal bsroundTTL = Convert.ToDecimal(PropGradeSpecList.Where(x => x.PROPERTIES == "BLACK SPECK" && x.PROP_ITEM == appearanceDataTable.Columns[i].ColumnName.ToString()
                                                                                                && x.GRADINGIND == "A")
                                  .Select(x => x.ROUNDING)
                                  .FirstOrDefault());

                                string formatStringbsTTL = "#,##0." + new string('0', 2);//(int)bsroundTTL);
                                //exclude top, middle, last row
                                string filter = "[Tag No] NOT LIKE '%First bag%'";
                                string formula = "SUM([" + appearanceDataTable.Columns[i].ColumnName.ToString() + "])";
                                object sumcolst = appearanceDataTable.Compute(formula, filter);
                                decimal sumcol = decimal.TryParse(sumcolst.ToString(), out decimal a) ? Convert.ToDecimal(sumcolst) : 0;
                                dataCellR1C1H18.Phrase = new Phrase(sumcol.ToString(formatStringbsTTL), tableTitleFont);
                                dataPdfTblapp.AddCell(dataCellR1C1H18);
                            }
                        }
                    }

                    //Average
                    for (int i = 0; i < appearanceDataTable.Columns.Count; i++)
                    {
                        if (i == 0)
                        {
                            dataCellR1C2H18.Phrase = new Phrase("Average", tableTitleFont11);
                            dataCellR1C2H18.PaddingLeft = 6f;
                            dataCellR1C2H18.HorizontalAlignment = Element.ALIGN_LEFT;
                            dataPdfTblapp.AddCell(dataCellR1C2H18);
                        }
                        else if (i == appearanceDataTable.Columns.Count - 1)
                        {
                            dataCellR1C1BC.Phrase = new Phrase(" ", normalTextFont);
                            dataPdfTblapp.AddCell(dataCellR1C1BC);
                        }
                        else
                        {
                            if (i != 1)
                            {

                                decimal bsroundavg = Convert.ToDecimal(PropGradeSpecList.Where(x => x.PROPERTIES == "BLACK SPECK" && x.PROP_ITEM == appearanceDataTable.Columns[i].ColumnName.ToString()
                                                                                            && x.GRADINGIND == "A")
                                  .Select(x => x.ROUNDING)
                                  .FirstOrDefault());

                                string formatStringbsavg = "#,##0." + new string('0', 2);//(int)bsroundavg);
                                //exclude top, middle, last row
                                string filter = "[Tag No] NOT LIKE '%First bag%'";
                                string formula = "SUM([" + appearanceDataTable.Columns[i].ColumnName.ToString() + "])";
                                object sumcolst = appearanceDataTable.Compute(formula, filter);
                                decimal sumcol = decimal.TryParse(sumcolst.ToString(), out decimal a) ? Convert.ToDecimal(sumcolst) : 0;
                                decimal currration = Convert.ToDecimal(dBSratio.Where(x => x.fieldname == appearanceDataTable.Columns[i].ColumnName.ToString())
                                              .Select(x => x.calcratio)
                                              .FirstOrDefault());

                                int filteredRowCount = appearanceDataTable.Select("[Tag No] NOT LIKE '%First bag%'").Length;

                                //decimal curravg = (sumcol * currration) / (filteredRowCount);
                                decimal curravg = sumcol / filteredRowCount;
                                dataCellR1C1H18.Phrase = new Phrase(curravg.ToString(formatStringbsavg), tableTitleFont);
                                dataPdfTblapp.AddCell(dataCellR1C1H18);
                            }
                        }
                    }


                    //Specification
                    appearanceDataTable.Columns.Remove("Tonnage");
                    appearanceDataTable.Columns.Remove("Tag No");
                    appearanceDataTable.Columns.Remove("Grade");


                    dataCellR1C2H18.Phrase = new Phrase("Specification", tableTitleFont11);
                    dataCellR1C2H18.PaddingLeft = 6f;
                    dataCellR1C2H18.HorizontalAlignment = Element.ALIGN_LEFT;
                    dataPdfTblapp.AddCell(dataCellR1C2H18);

                    for (int i = 0; i < appearanceDataTable.Columns.Count; i++)
                    {
                        string bsclmnname = appearanceDataTable.Columns[i].ColumnName.ToString();
                        string bsscpec = "";

                        for (int k = 0; k < dtDirect.Rows.Count; k++)
                        {
                            if (dtDirect.Rows[k]["PROP_ITEM"].ToString() == bsclmnname)
                            {
                                bsscpec = dtDirect.Rows[k]["SPECIFICATION"] != DBNull.Value ? dtDirect.Rows[k]["SPECIFICATION"].ToString() : "";
                                dataCellR1C1NF.Phrase = new Phrase(bsscpec, tablesubTitleFont);
                                dataPdfTblapp.AddCell(dataCellR1C1NF);
                                break;
                            }
                        }
                    }

                    dataCellR1C1BC.Phrase = new Phrase(" ", normalTextFont);
                    dataPdfTblapp.AddCell(dataCellR1C1BC);
                }
                //for (int i = 0; i < appearanceDataTable.Columns.Count; i++)
                //{
                //    if (i == 0)
                //    {
                //        dataCellR1C2H18.Phrase = new Phrase("Specification", tableTitleFont);
                //        dataCellR1C2H18.PaddingLeft = 6f;
                //        dataCellR1C2H18.HorizontalAlignment = Element.ALIGN_LEFT;
                //        dataPdfTblapp.AddCell(dataCellR1C2H18);
                //    }
                //    else if (i == appearanceDataTable.Columns.Count - 1)
                //    {
                //        dataCellR1C1BC.Phrase = new Phrase(" ", normalTextFont);
                //        dataPdfTblapp.AddCell(dataCellR1C1BC);
                //    }
                //    else
                //    {
                //        string bsclmnname = appearanceDataTable.Columns[i].ColumnName.ToString();
                //        string bsscpec = "";

                //        for (int k = 0; k < dtDirect.Rows.Count; k++)
                //        {
                //            if (dtDirect.Rows[k]["PROP_ITEM"].ToString() == bsclmnname)
                //            {
                //                bsscpec = dtDirect.Rows[k]["SPECIFICATION"] != DBNull.Value ? dtDirect.Rows[k]["SPECIFICATION"].ToString() : "";
                //                dataCellR1C1NF.Phrase = new Phrase(bsscpec, normalTextFont);
                //                dataPdfTblapp.AddCell(dataCellR1C1NF);
                //                break;
                //            }
                //        }
                //    }
                //}

                dataPdfTable3.DeleteBodyRows();

                dataCellR1C1 = new PdfPCell(dataPdfTblapp);
                dataCellR1C1.Border = Rectangle.TOP_BORDER;
                dataCellR1C1.HorizontalAlignment = Element.ALIGN_LEFT;
                dataPdfTable3.AddCell(dataCellR1C1);

                doc.Add(dataPdfTable3);

                #endregion BLACKSPEC

                #region LOT GRADING DETAILS

                //LOT GRADING DETAILS
                dataCellR1C1.Border = Rectangle.RECTANGLE;
                dataCellR1C1.BorderWidth = 1f;
                dataCellR1C1.VerticalAlignment = Element.ALIGN_MIDDLE;
                dataCellR1C1.HorizontalAlignment = Element.ALIGN_LEFT;

                dataPdfTable2.SpacingBefore = 12f;

                DataTable dth = await dbdal.IDS_SUM_SEL_RPT(id);

                dataCellR1C5.Phrase = new Phrase("Lot Grading Details", subTitleFont);
                dataCellR1C5.PaddingLeft = 6f;
                dataCellR1C5.HorizontalAlignment = Element.ALIGN_LEFT;
                dataCellR1C5.BackgroundColor = altRowColor;
                dataPdfTable2.AddCell(dataCellR1C5);
                if (dth != null)
                {
                    dataCellR1C1H18.Phrase = new Phrase("Tag Number", tableTitleFont11);
                    dataCellR1C1H18.HorizontalAlignment = Element.ALIGN_CENTER;
                    dataPdfTable2.AddCell(dataCellR1C1H18);

                    dataCellR1C1H18.Phrase = new Phrase("Tonnage", tableTitleFont11);
                    dataCellR1C1H18.HorizontalAlignment = Element.ALIGN_CENTER;
                    dataPdfTable2.AddCell(dataCellR1C1H18);

                    dataCellR1C1H18.Phrase = new Phrase("Quantity (Ton)", tableTitleFont11);
                    dataCellR1C1H18.HorizontalAlignment = Element.ALIGN_CENTER;
                    dataPdfTable2.AddCell(dataCellR1C1H18);

                    dataCellR1C1H18.Phrase = new Phrase("Grade", tableTitleFont11);
                    dataCellR1C1H18.HorizontalAlignment = Element.ALIGN_CENTER;
                    dataPdfTable2.AddCell(dataCellR1C1H18);

                    dataCellR1C1H18.Phrase = new Phrase("Abnormalities", tableTitleFont11);
                    dataCellR1C1H18.HorizontalAlignment = Element.ALIGN_CENTER;
                    dataPdfTable2.AddCell(dataCellR1C1H18);

                    for (int i = 0; i < dth.Rows.Count; i++)
                    {
                        dataCellR1C1H18T.Phrase = new Phrase(dth.Rows[i]["TAGNOFROMTO"].ToString(), tableTitleFont);
                        dataPdfTable2.AddCell(dataCellR1C1H18T);

                        dataCellR1C1H18T.Phrase = new Phrase(dth.Rows[i]["TONNAGEFROMTO"].ToString(), tableTitleFont);
                        dataPdfTable2.AddCell(dataCellR1C1H18T);

                        dataCellR1C1H18T.Phrase = new Phrase(dth.Rows[i]["QUANTITY"].ToString(), tableTitleFont);
                        dataPdfTable2.AddCell(dataCellR1C1H18T);

                        dataCellR1C1H18T.Phrase = new Phrase(dth.Rows[i]["GRADE"].ToString(), titleFont);
                        dataPdfTable2.AddCell(dataCellR1C1H18T);

                        dataCellR1C1H18T.Phrase = new Phrase(dth.Rows[i]["ABNORMALITIES"].ToString(), normalTextFont);
                        dataPdfTable2.AddCell(dataCellR1C1H18T);
                    }
                }
                else
                {
                    dataCellR1C1H18.Phrase = new Phrase("Status", tableTitleFont11);
                    dataCellR1C1H18.HorizontalAlignment = Element.ALIGN_CENTER;
                    dataPdfTable2.AddCell(dataCellR1C1H18);

                    dataCellR1C1H18.Phrase = new Phrase(summaryVM.STATUS.ToString(), tableTitleFont11);
                    dataCellR1C1H18.HorizontalAlignment = Element.ALIGN_CENTER;
                    dataPdfTable2.AddCell(dataCellR1C1H18);

                    dataCellR1C1H18.Phrase = new Phrase(" ", tableTitleFont11);
                    dataCellR1C1H18.HorizontalAlignment = Element.ALIGN_CENTER;
                    dataPdfTable2.AddCell(dataCellR1C1H18);

                    dataCellR1C1H18.Phrase = new Phrase(" ", tableTitleFont11);
                    dataCellR1C1H18.HorizontalAlignment = Element.ALIGN_CENTER;
                    dataPdfTable2.AddCell(dataCellR1C1H18);

                    dataCellR1C1H18.Phrase = new Phrase(" ", tableTitleFont11);
                    dataCellR1C1H18.HorizontalAlignment = Element.ALIGN_CENTER;
                    dataPdfTable2.AddCell(dataCellR1C1H18);
                }

                doc.Add(dataPdfTable2);

                #endregion LOT GRADING DETAILS

                #region APPROVAL DETAILS

                dataPdfTable3.DeleteBodyRows();
                dataPdfTable3.SpacingBefore = 13f;
                dataCellR1C1.FixedHeight = 15f;
                dataCellR1C1.Phrase = new Phrase("Approval Details", subTitleFont);
                dataCellR1C1.PaddingLeft = 6f;
                dataCellR1C1.HorizontalAlignment = Element.ALIGN_LEFT;
                dataCellR1C1.BackgroundColor = altRowColor;
                dataCellR1C1.Border = Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                dataPdfTable3.AddCell(dataCellR1C1);

                doc.Add(dataPdfTable3);

                dataCellR1C1H25.Phrase = new Phrase("Details", tableTitleFont11);
                dataCellR1C1H25.PaddingLeft = 6f;
                dataCellR1C1H25.HorizontalAlignment = Element.ALIGN_CENTER;
                dataPdfTable1A.AddCell(dataCellR1C1H25);

                dataCellR1C1H25.Phrase = new Phrase("Name", tableTitleFont11);
                dataPdfTable1A.AddCell(dataCellR1C1H25);

                dataCellR1C1H25.Phrase = new Phrase("Date", tableTitleFont11);
                dataPdfTable1A.AddCell(dataCellR1C1H25);

                dataCellR1C1H25.Phrase = new Phrase("Signature", tableTitleFont11);
                dataPdfTable1A.AddCell(dataCellR1C1H25);

                dataCellR1C1H25.Phrase = new Phrase("Moulding & Property", tableTitleFont);
                dataCellR1C1H25.PaddingLeft = 6f;
                dataCellR1C1H25.HorizontalAlignment = Element.ALIGN_CENTER;
                dataPdfTable1A.AddCell(dataCellR1C1H25);

                dataCellR1C1H25.Phrase = new Phrase(dtApp.Rows[0]["APPR_MOLD_NAME"].ToString(), normalTextFont);
                dataPdfTable1A.AddCell(dataCellR1C1H25);

                dataCellR1C1H25.Phrase = new Phrase(dtApp.Rows[0]["MOLD_DATE"].ToString(), tablesubTitleFont);
                dataPdfTable1A.AddCell(dataCellR1C1H25);

                dataCellR1C1H25.Phrase = new Phrase("", tablesubTitleFont);
                dataPdfTable1A.AddCell(dataCellR1C1H25);

                dataCellR1C1H25.Phrase = new Phrase("Appearance", tableTitleFont);
                dataCellR1C1H25.PaddingLeft = 6f;
                dataPdfTable1A.AddCell(dataCellR1C1H25);

                dataCellR1C1H25.Phrase = new Phrase(dtApp.Rows[0]["APPR_APPEA_NAME"].ToString(), normalTextFont);
                dataPdfTable1A.AddCell(dataCellR1C1H25);

                dataCellR1C1H25.Phrase = new Phrase(dtApp.Rows[0]["APPEARANCE_DATE"].ToString(), tablesubTitleFont);
                dataPdfTable1A.AddCell(dataCellR1C1H25);

                dataCellR1C1H25.Phrase = new Phrase("", tablesubTitleFont);
                dataPdfTable1A.AddCell(dataCellR1C1H25);

                dataCellR1C1H25.Phrase = new Phrase("Checked by", tableTitleFont);
                dataCellR1C1H25.PaddingLeft = 6f;
                dataPdfTable1A.AddCell(dataCellR1C1H25);

                dataCellR1C1H25.Phrase = new Phrase(dtApp.Rows[0]["APPR_CHKBY_NAME"].ToString(), normalTextFont);
                dataPdfTable1A.AddCell(dataCellR1C1H25);

                dataCellR1C1H25.Phrase = new Phrase(dtApp.Rows[0]["CHECKED_BY_DATE"].ToString(), tablesubTitleFont);
                dataPdfTable1A.AddCell(dataCellR1C1H25);

                dataCellR1C1H25.Phrase = new Phrase("", tablesubTitleFont);
                dataPdfTable1A.AddCell(dataCellR1C1H25);

                dataCellR1C1H25.Phrase = new Phrase("Verified by", tableTitleFont);
                dataCellR1C1H25.PaddingLeft = 6f;
                dataPdfTable1A.AddCell(dataCellR1C1H25);

                dataCellR1C1H25.Phrase = new Phrase(dtApp.Rows[0]["APPR_VERFBY_NAME"].ToString(), normalTextFont);
                dataPdfTable1A.AddCell(dataCellR1C1H25);

                dataCellR1C1H25.Phrase = new Phrase(dtApp.Rows[0]["VERIFIED_BY_DATE"].ToString(), tablesubTitleFont);
                dataPdfTable1A.AddCell(dataCellR1C1H25);

                dataCellR1C1H25.Phrase = new Phrase("", tablesubTitleFont);
                dataPdfTable1A.AddCell(dataCellR1C1H25);

                dataCellR1C1H25.Phrase = new Phrase("Keyed-in by", tableTitleFont);
                dataCellR1C1H25.PaddingLeft = 6f;
                dataPdfTable1A.AddCell(dataCellR1C1H25);

                dataCellR1C1H25.Phrase = new Phrase(dtApp.Rows[0]["APPR_KEYBY_NAME"].ToString(), normalTextFont);
                dataPdfTable1A.AddCell(dataCellR1C1H25);

                dataCellR1C1H25.Phrase = new Phrase(dtApp.Rows[0]["KEYED_BY_DATE"].ToString(), tablesubTitleFont);
                dataPdfTable1A.AddCell(dataCellR1C1H25);

                dataCellR1C1H25.Phrase = new Phrase("", tablesubTitleFont);
                dataPdfTable1A.AddCell(dataCellR1C1H25);

                PdfPCell cell1 = new PdfPCell(dataPdfTable1A);
                cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                cell1.Border = Rectangle.NO_BORDER;
                dataPdfTable1.AddCell(cell1);

                dataCellR1C1.Phrase = new Phrase("", tableTitleFont);
                dataCellR1C1.BackgroundColor = null;
                dataCellR1C1.Border = Rectangle.NO_BORDER;
                dataPdfTable1.AddCell(dataCellR1C1);

                dataCellR1C5H13.Phrase = new Phrase("Production Lot Close Report by:", tablesubTitleFont);
                dataCellR1C5H13.PaddingLeft = 6f;
                dataCellR1C5H13.HorizontalAlignment = Element.ALIGN_LEFT;
                dataCellR1C5H13.Border = Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                dataPdfTable1B.AddCell(dataCellR1C5H13);

                dataCellR1C5H13.Phrase = new Phrase(dtApp.Rows[0]["REPORT_BY"].ToString(), tablesubTitleFont);
                dataCellR1C5H13.PaddingLeft = 6f;
                dataCellR1C5H13.HorizontalAlignment = Element.ALIGN_LEFT;
                dataCellR1C5H13.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                dataPdfTable1B.AddCell(dataCellR1C5H13);

                dataCellR1C1H16.HorizontalAlignment = Element.ALIGN_CENTER;
                dataCellR1C2H13.HorizontalAlignment = Element.ALIGN_CENTER;
                dataCellR3C1.HorizontalAlignment = Element.ALIGN_CENTER;
                dataCellR1C5H12.HorizontalAlignment = Element.ALIGN_LEFT;
                dataCellR1C5H12.PaddingLeft = 5f;

                dataCellR1C1H16.Phrase = new Phrase("Lot Status", normalTextFont);
                dataPdfTable1B.AddCell(dataCellR1C1H16);
                dataCellR1C1H16.Phrase = new Phrase(dtApp.Rows[0]["LOT_STATUS"].ToString(), tablesubTitleFont);
                dataPdfTable1B.AddCell(dataCellR1C1H16);
                dataCellR1C2H13.Phrase = new Phrase("Fisheye Level", normalTextFont);
                dataPdfTable1B.AddCell(dataCellR1C2H13);
                dataCellR1C1H16.Phrase = new Phrase(dtApp.Rows[0]["FISHEYE_LEVEL"].ToString(), tablesubTitleFont);
                dataPdfTable1B.AddCell(dataCellR1C1H16);

                dataCellR1C1H16.Phrase = new Phrase("Silo No.", normalTextFont);
                dataPdfTable1B.AddCell(dataCellR1C1H16);
                dataCellR1C1H16.Phrase = new Phrase(dtApp.Rows[0]["SILO_NO"].ToString(), tablesubTitleFont);
                dataPdfTable1B.AddCell(dataCellR1C1H16);
                dataCellR3C1.Phrase = new Phrase("Black Speck.", normalTextFont);
                dataPdfTable1B.AddCell(dataCellR3C1);
                dataCellR1C1H16.Phrase = new Phrase("Spot-1", normalTextFont);
                dataPdfTable1B.AddCell(dataCellR1C1H16);
                dataCellR1C1H16.Phrase = new Phrase(dtApp.Rows[0]["BS_SPOT_1"].ToString(), tablesubTitleFont);
                dataPdfTable1B.AddCell(dataCellR1C1H16);

                dataCellR1C1H16.Phrase = new Phrase("On-line MFR.", normalTextFont);
                dataPdfTable1B.AddCell(dataCellR1C1H16);
                dataCellR1C1H16.Phrase = new Phrase(dtApp.Rows[0]["ONLINE_MFR"].ToString(), tablesubTitleFont);
                dataPdfTable1B.AddCell(dataCellR1C1H16);
                dataCellR1C1H16.Phrase = new Phrase("Spot-2", normalTextFont);
                dataPdfTable1B.AddCell(dataCellR1C1H16);
                dataCellR1C1H16.Phrase = new Phrase(dtApp.Rows[0]["BS_SPOT_2"].ToString(), tablesubTitleFont);
                dataPdfTable1B.AddCell(dataCellR1C1H16);

                dataCellR1C1H16.Phrase = new Phrase("On-line YI.", normalTextFont);
                dataPdfTable1B.AddCell(dataCellR1C1H16);
                dataCellR1C1H16.Phrase = new Phrase(dtApp.Rows[0]["ONLINE_YI"].ToString(), tablesubTitleFont);
                dataPdfTable1B.AddCell(dataCellR1C1H16);
                dataCellR1C1H16.Phrase = new Phrase("Spot-3", normalTextFont);
                dataPdfTable1B.AddCell(dataCellR1C1H16);
                dataCellR1C1H16.Phrase = new Phrase(dtApp.Rows[0]["BS_SPOT_3"].ToString(), tablesubTitleFont);
                dataPdfTable1B.AddCell(dataCellR1C1H16);

                dataCellR1C1H16.Phrase = new Phrase("Twin Pellet", normalTextFont);
                dataPdfTable1B.AddCell(dataCellR1C1H16);
                dataCellR1C1H16.Phrase = new Phrase(dtApp.Rows[0]["TWIN_PELLET"].ToString(), tablesubTitleFont);
                dataPdfTable1B.AddCell(dataCellR1C1H16);
                dataCellR1C2H13.Phrase = new Phrase("Online YP/BP", normalTextFont);
                dataPdfTable1B.AddCell(dataCellR1C2H13);
                dataCellR1C1H16.Phrase = new Phrase(dtApp.Rows[0]["ONLINE_YPBP"].ToString(), tablesubTitleFont);
                dataPdfTable1B.AddCell(dataCellR1C1H16);

                dataCellR1C5H40.Phrase = new Phrase("Production Remarks: " + dtApp.Rows[0]["PRODUCTION_REMARK"].ToString(), tablesubTitleFont);
                dataCellR1C5H40.VerticalAlignment = Element.ALIGN_TOP;
                dataCellR1C5H40.PaddingTop = 4f;
                dataPdfTable1B.AddCell(dataCellR1C5H40);

                PdfPCell cell3 = new PdfPCell(dataPdfTable1B);
                cell3.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell3.Border = Rectangle.NO_BORDER;
                dataPdfTable1.AddCell(cell3);

                doc.Add(dataPdfTable1);

                #endregion APPROVAL DETAILS

                #region HISTORY
                ////////////////////////////////////////////////// HISTORY TRANSACTION TABLE //////////////////////////////////////////////////////////////////////////
                doc.NewPage();
                int counter = 0;

                // Get ID_IDS_D joined as string
                string idForHistory = await db.GetID_IDS_D(summaryVM.PRODTYPE, summaryVM.LOTNO);

                // Title
                Paragraph histTransTitle = new Paragraph("History Transaction", subTitleFont)
                {
                    Alignment = Element.ALIGN_LEFT,
                    SpacingAfter = 10f
                };
                doc.Add(histTransTitle);

                // string of column header
                string[] histTransColHeader = { "Property", "Unit", "Reading#1", "Reading#2", "Reading#3", "Reading#4", "Reading#5", "Reading#6", "Average", "Grade", "Tested Date Time" };

                PdfPTable histTransTable = new PdfPTable(new float[] { 15f, 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f });
                histTransTable.WidthPercentage = 100;
                histTransTable.SpacingAfter = 5f;
                histTransTable.SpacingBefore = 5f;

                // Header
                foreach (string colHeaderStr in histTransColHeader)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(colHeaderStr, colHeaderFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    histTransTable.AddCell(cell);
                }

                // Data Area
                DataTable HistTransDataTable = await db.GetHistoryTransactionData(idForHistory, summaryVM.PRODTYPE, summaryVM.LOTNO);
                foreach (DataRow row in HistTransDataTable.Rows)
                {
                    counter++;
                    PdfPCell cell = new PdfPCell()
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        PaddingTop = 3,
                        PaddingBottom = 3,
                        BackgroundColor = counter % 2 == 0 ? BaseColor.WHITE : altRowColor

                    };

                    cell.Phrase = new Phrase(row["PROPITEM"].ToString(), tableTitleFont);
                    histTransTable.AddCell(cell);

                    cell.Phrase = new Phrase(row["UNIT"].ToString(), tableTitleFont);
                    histTransTable.AddCell(cell);

                    cell.Phrase = new Phrase(row["READING1"].ToString(), tableTitleFont);
                    histTransTable.AddCell(cell);

                    cell.Phrase = new Phrase(row["READING2"].ToString(), tableTitleFont);
                    histTransTable.AddCell(cell);

                    cell.Phrase = new Phrase(row["READING3"].ToString(), tableTitleFont);
                    histTransTable.AddCell(cell);

                    cell.Phrase = new Phrase(row["READING4"].ToString(), tableTitleFont);
                    histTransTable.AddCell(cell);

                    cell.Phrase = new Phrase(row["READING5"].ToString(), tableTitleFont);
                    histTransTable.AddCell(cell);

                    cell.Phrase = new Phrase(row["READING6"].ToString(), tableTitleFont);
                    histTransTable.AddCell(cell);

                    cell.Phrase = new Phrase(row["AVERAGE"].ToString(), tableTitleFont);
                    histTransTable.AddCell(cell);

                    cell.Phrase = new Phrase(row["GRADE"].ToString(), tableTitleFont);
                    histTransTable.AddCell(cell);

                    cell.Phrase = new Phrase(row["TESTEDDATETIME"].ToString(), normalTextFont);
                    histTransTable.AddCell(cell);
                }

                // Add table
                doc.Add(histTransTable);

                #endregion HISTORY

                // close document, stream and writer
                string filename = summaryVM.LOTNO.ToString() + "_" + summaryVM.PRODTYPE.ToString();
                doc.Close();
                doc.CloseDocument();
                doc.Dispose();
                writer.Close();
                writer.Dispose();
                fs.Close();
                fs.Dispose();
                // Return file bytes
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                if (handleHttpResponse)
                {
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + ".pdf");
                    Response.TransmitFile(filePath);
                    Response.End();
                }
                System.IO.File.Delete(filePath);
                return fileBytes;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            finally
            {
                // close document, stream and writer
                doc.Close();
                doc.CloseDocument();
                doc.Dispose();
                writer.Close();
                writer.Dispose();
                fs.Close();
                fs.Dispose();
            }

        }

        //public async Task<ActionResult> Report2(string id)
        //{
        //    return File(await Test(id), "application/pdf");
        //}

        //list of item in ids
        private static DataTable GetItemIDS()
        {
            // Create a list of items.
            List<ItemIDS> items = new List<ItemIDS>()
            {
                new ItemIDS() { SEQUENCE = 1, ITEM = "Product Type", DBCOLNAME = "", ITEMVALUE = "PX04-X50" },
                new ItemIDS() { SEQUENCE = 2, ITEM = "Colour Code", DBCOLNAME = "", ITEMVALUE = "B1" },
                new ItemIDS() { SEQUENCE = 3, ITEM = "Lot No.", DBCOLNAME = "", ITEMVALUE = "218-6-230" },
                new ItemIDS() { SEQUENCE = 4, ITEM = "Quantity", DBCOLNAME = "", ITEMVALUE = "" },
                new ItemIDS() { SEQUENCE = 5, ITEM = "Production Date", DBCOLNAME = "", ITEMVALUE = "" },
                new ItemIDS() { SEQUENCE = 6, ITEM = "Production Line/Lot No", DBCOLNAME = "", ITEMVALUE = "" },
                new ItemIDS() { SEQUENCE = 7, ITEM = "Previous Production Product Type", DBCOLNAME = "", ITEMVALUE = "" },
                new ItemIDS() { SEQUENCE = 8, ITEM = "Natural Resin Grade", DBCOLNAME = "", ITEMVALUE = "" },
                new ItemIDS() { SEQUENCE = 9, ITEM = "Natural Resin Lot No", DBCOLNAME = "", ITEMVALUE = "" },
                new ItemIDS() { SEQUENCE = 10, ITEM = " ", DBCOLNAME = "", ITEMVALUE = "" },
                new ItemIDS() { SEQUENCE = 11, ITEM = "Natural Resin Grade", DBCOLNAME = "", ITEMVALUE = "" },
                new ItemIDS() { SEQUENCE = 12, ITEM = "Natural Resin Lot No", DBCOLNAME = "", ITEMVALUE = "" },
                new ItemIDS() { SEQUENCE = 13, ITEM = " ", DBCOLNAME = "", ITEMVALUE = "" },
                new ItemIDS() { SEQUENCE = 14, ITEM = "Natural Resin Grade", DBCOLNAME = "", ITEMVALUE = "" },
                new ItemIDS() { SEQUENCE = 15, ITEM = "Natural Resin Lot No", DBCOLNAME = "", ITEMVALUE = "" },


            };

            DataTable table = new DataTable();

            // Define the table columns.
            table.Columns.Add("SEQUENCE", typeof(int));
            table.Columns.Add("ITEM", typeof(string));
            table.Columns.Add("DBCOLNAME", typeof(string));
            table.Columns.Add("ITEMVALUE", typeof(string));

            foreach (ItemIDS item in items)
            {
                DataRow row = table.NewRow();
                row["SEQUENCE"] = item.SEQUENCE;
                row["ITEM"] = item.ITEM;

                table.Rows.Add(row);
            }
            return table;
        }

        public PdfPTable CreateTable(float[] columnWidths, float width = 450f)
        {
            PdfPTable table = new PdfPTable(columnWidths);
            table.TotalWidth = width;
            table.LockedWidth = true;
            table.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            return table;
        }

        public PdfPCell CreateCell(int rowspan, int colspan, float height = 15f)
        {
            PdfPCell datacell = new PdfPCell()
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Rowspan = rowspan,
                Colspan = colspan,
                BorderWidth = 1f,
                FixedHeight = height
            };
            return datacell;
        }

        public PdfPCell CreateCellNotFixedHeight(int rowspan, int colspan)
        {
            PdfPCell datacell = new PdfPCell()
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Rowspan = rowspan,
                Colspan = colspan,
                BorderWidth = 1f,
            };
            return datacell;
        }

        //public async Task<byte[]> Test2(string id)
        //{
        //    // Create path to pdf file
        //    string filePath = Server.MapPath("\\") + "IDS_REPORT" + ".pdf";
        //    string username = (Session["AclUser"] as ACL_UserObj).USER_ID.ToString();
        //    // Create document and its writing process
        //    Document doc = new Document(new Rectangle(288f, 144f), 70, 50, 1, 1);
        //    doc.SetPageSize(PageSize.A4);
        //    doc.AddAuthor(username);

        //    FileStream fs = new FileStream(filePath, FileMode.Create);
        //    PdfWriter writer = PdfWriter.GetInstance(doc, fs);
        //    writer.PageEvent = new ITextEvents(username);

        //    doc.Open();

        //    try
        //    {
        //        BaseColor altRowColor = new BaseColor(227, 227, 227);

        //        Font titleFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 12);
        //        Font subTitleFont = new Font(Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD, BaseColor.BLACK);
        //        Font tableTitleFont = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 8, Font.UNDERLINE | Font.BOLD, BaseColor.BLACK);
        //        Font normalTextFont = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 8);

        //        Font colHeaderFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8);
        //        Font colNormalFont = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 8);
        //        // to from date
        //        //13 cell
        //        PdfPTable tofromdatePdfTable = new PdfPTable(new float[] { 40f, 5f, 235f, 90f, 80f });
        //        tofromdatePdfTable.TotalWidth = 450f;
        //        tofromdatePdfTable.LockedWidth = true;
        //        tofromdatePdfTable.HorizontalAlignment = Element.ALIGN_JUSTIFIED;

        //        // Create cells to use
        //        PdfPCell TitleCell = new PdfPCell()
        //        {
        //            VerticalAlignment = Element.ALIGN_MIDDLE,
        //            HorizontalAlignment = Element.ALIGN_LEFT,
        //            Border = Rectangle.NO_BORDER
        //        };

        //        TitleCell.Phrase = new Phrase("To", normalTextFont);
        //        TitleCell.HorizontalAlignment = Element.ALIGN_LEFT;
        //        tofromdatePdfTable.AddCell(TitleCell);
        //        TitleCell.Phrase = new Phrase(":", normalTextFont);
        //        tofromdatePdfTable.AddCell(TitleCell);
        //        TitleCell.Phrase = new Phrase("TORAY PLASTICS (M) SDN. BERHAD (46619-P)", normalTextFont);
        //        tofromdatePdfTable.AddCell(TitleCell);
        //        TitleCell.Phrase = new Phrase("TPM/QA/QAS1/015", normalTextFont);
        //        TitleCell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //        tofromdatePdfTable.AddCell(TitleCell);
        //        TitleCell.Phrase = new Phrase("", normalTextFont);
        //        tofromdatePdfTable.AddCell(TitleCell);

        //        TitleCell.Phrase = new Phrase("From", normalTextFont);
        //        TitleCell.HorizontalAlignment = Element.ALIGN_LEFT;
        //        tofromdatePdfTable.AddCell(TitleCell);
        //        TitleCell.Phrase = new Phrase(":", normalTextFont);
        //        tofromdatePdfTable.AddCell(TitleCell);
        //        TitleCell.Phrase = new Phrase("SIK COLOR (M) SDN.BHD ", normalTextFont);
        //        tofromdatePdfTable.AddCell(TitleCell);
        //        TitleCell.Phrase = new Phrase("Issue No. : 2.3", normalTextFont);
        //        TitleCell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //        tofromdatePdfTable.AddCell(TitleCell);
        //        TitleCell.Phrase = new Phrase("", normalTextFont);
        //        tofromdatePdfTable.AddCell(TitleCell);

        //        TitleCell.Phrase = new Phrase("Date", normalTextFont);
        //        TitleCell.HorizontalAlignment = Element.ALIGN_LEFT;
        //        tofromdatePdfTable.AddCell(TitleCell);
        //        TitleCell.Phrase = new Phrase(":", normalTextFont);
        //        tofromdatePdfTable.AddCell(TitleCell);
        //        TitleCell.Phrase = new Phrase("20.07.22", normalTextFont);
        //        tofromdatePdfTable.AddCell(TitleCell);
        //        TitleCell.Phrase = new Phrase("Ref. No. : __________", normalTextFont);
        //        TitleCell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //        tofromdatePdfTable.AddCell(TitleCell);
        //        TitleCell.Phrase = new Phrase("", normalTextFont);
        //        tofromdatePdfTable.AddCell(TitleCell);

        //        doc.Add(tofromdatePdfTable);

        //        Paragraph title = new Paragraph("INSPECTION DATA SHEET", titleFont)
        //        {
        //            Alignment = Element.ALIGN_CENTER,
        //            SpacingAfter = 5
        //        };
        //        doc.Add(title);

        //        /////////////////////////////////////START TABLE////////////////

        //        PdfPTable dataPdfTable4 = CreateTable(new float[] { 150f, 100f, 100f, 100f });
        //        PdfPTable dataPdfTable5 = CreateTable(new float[] { 110f, 40f, 100f, 100f, 100f });
        //        PdfPTable dataPdfTable5p = CreateTable(new float[] { 80f, 70f, 100f, 100f, 100f });
        //        PdfPTable dataPdfTable9 = CreateTable(new float[] { 80f, 30f, 40f, 50f, 50f, 50f, 50f, 50f, 50f });
        //        PdfPTable dataPdfTable8 = CreateTable(new float[] { 110f, 40f, 50f, 50f, 50f, 50f, 50f, 50f });
        //        PdfPTable dataPdfTable9p = CreateTable(new float[] { 31f, 79f, 40f, 50f, 50f, 50f, 50f, 50f, 50f });
        //        PdfPTable dataPdfTable12 = CreateTable(new float[] { 80f, 30f, 40f, 33f, 33f, 34f, 33f, 33f, 34f, 33f, 33f, 34f });
        //        PdfPTable dataPdfTable11 = CreateTable(new float[] { 110f, 40f, 33f, 33f, 34f, 33f, 33f, 34f, 33f, 33f, 34f });

        //        PdfPCell dataCell1 = CreateCell(1, 1);
        //        PdfPCell dataCellR1C1 = CreateCell(1, 1);
        //        PdfPCell dataCellR4C1 = CreateCell(4, 1);
        //        PdfPCell dataCellR1C2 = CreateCell(1, 2);
        //        dataCellR1C2.HorizontalAlignment = Element.ALIGN_CENTER;
        //        PdfPCell dataCellR3C1 = CreateCell(3, 1);

        //        DataTable dtdata = GetItemIDS();

        //        foreach (DataRow item in dtdata.Rows)
        //        {
        //            dataCellR1C1.Phrase = new Phrase(item["ITEM"].ToString(), normalTextFont);
        //            dataPdfTable4.AddCell(dataCellR1C1);
        //            dataCellR1C1.Phrase = new Phrase("", normalTextFont);
        //            for (int i = 1; i <= 3; i++)
        //            {
        //                dataPdfTable4.AddCell(dataCellR1C1);
        //            }
        //        }

        //        doc.Add(dataPdfTable4);
        //        string[] itemdatacell = new string[3] { "", "Colour", "Dispersion" };
        //        for (int j = 1; j <= 2; j++)
        //        {
        //            dataCellR1C1.Phrase = new Phrase(itemdatacell[j], normalTextFont);
        //            dataCellR1C1.HorizontalAlignment = Element.ALIGN_LEFT;
        //            dataPdfTable5.AddCell(dataCellR1C1);
        //            dataCellR1C1.Phrase = new Phrase("-", normalTextFont);
        //            dataCellR1C1.HorizontalAlignment = Element.ALIGN_CENTER;
        //            dataPdfTable5.AddCell(dataCellR1C1);
        //            dataCellR1C1.Phrase = new Phrase("", normalTextFont);
        //            for (int i = 1; i <= 3; i++)
        //            {
        //                dataPdfTable5.AddCell(dataCellR1C1);
        //            }
        //        }

        //        doc.Add(dataPdfTable5);
        //        //---------------------------------

        //        dataCell1.Phrase = new Phrase("Test Item", normalTextFont);
        //        dataCell1.HorizontalAlignment = Element.ALIGN_CENTER;
        //        dataCell1.Rowspan = 2;
        //        dataCell1.Colspan = 2;
        //        dataPdfTable9.AddCell(dataCell1);
        //        dataCell1.Phrase = new Phrase("Unit", normalTextFont);
        //        dataCell1.Rowspan = 2;
        //        dataCell1.Colspan = 1;
        //        dataPdfTable9.AddCell(dataCell1);
        //        dataCellR1C2.Phrase = new Phrase("Results", normalTextFont);
        //        for (int i = 1; i <= 3; i++)
        //        {
        //            dataPdfTable9.AddCell(dataCellR1C2);
        //        }

        //        for (int i = 1; i <= 3; i++)
        //        {
        //            dataCellR1C1.Phrase = new Phrase("Machine", normalTextFont);
        //            dataPdfTable9.AddCell(dataCellR1C1);
        //            dataCellR1C1.Phrase = new Phrase("Regressed", normalTextFont);
        //            dataPdfTable9.AddCell(dataCellR1C1);
        //        }

        //        dataCellR4C1.Phrase = new Phrase("Yellowness Index", normalTextFont);
        //        dataPdfTable9.AddCell(dataCellR4C1);
        //        dataCellR1C1.Phrase = new Phrase("First", normalTextFont);
        //        dataCellR1C1.HorizontalAlignment = Element.ALIGN_CENTER;
        //        dataPdfTable9.AddCell(dataCellR1C1);
        //        dataCellR4C1.Phrase = new Phrase("-", normalTextFont);
        //        dataCellR4C1.HorizontalAlignment = Element.ALIGN_CENTER;
        //        dataPdfTable9.AddCell(dataCellR4C1);
        //        dataCellR1C1.Phrase = new Phrase("", normalTextFont);
        //        for (int i = 1; i <= 6; i++)
        //        {
        //            dataPdfTable9.AddCell(dataCellR1C1);
        //        }

        //        string[] itemdatafml = new string[4] { "First", "Middle", "Last", "Ave" };
        //        for (int j = 1; j <= 3; j++)
        //        {
        //            dataCellR1C1.Phrase = new Phrase(itemdatafml[j], normalTextFont);
        //            dataPdfTable9.AddCell(dataCellR1C1);
        //            dataCellR1C1.Phrase = new Phrase("", normalTextFont);
        //            for (int i = 1; i <= 6; i++)
        //            {
        //                dataPdfTable9.AddCell(dataCellR1C1);
        //            }
        //        }

        //        doc.Add(dataPdfTable9);

        //        //---------------------------------
        //        string[] itemdataspot = new string[4] { "", "Spot-1", "Spot-2", "Spot-3" };
        //        dataCellR1C2.Phrase = new Phrase("Test Item", normalTextFont);
        //        dataPdfTable12.AddCell(dataCellR1C2);
        //        dataCellR1C1.Phrase = new Phrase("Unit", normalTextFont);
        //        dataPdfTable12.AddCell(dataCellR1C1);

        //        for (int j = 1; j <= 3; j++)
        //        {
        //            for (int i = 1; i <= 3; i++)
        //            {
        //                dataCellR1C1.Phrase = new Phrase(itemdataspot[i], normalTextFont);
        //                dataPdfTable12.AddCell(dataCellR1C1);
        //            }
        //        }

        //        dataCellR4C1.Phrase = new Phrase("Black Speck", normalTextFont);
        //        dataCellR4C1.HorizontalAlignment = Element.ALIGN_LEFT;
        //        dataPdfTable12.AddCell(dataCellR4C1);
        //        dataCellR1C1.Phrase = new Phrase("First", normalTextFont);
        //        dataCellR1C1.HorizontalAlignment = Element.ALIGN_CENTER;
        //        dataPdfTable12.AddCell(dataCellR1C1);
        //        dataCellR4C1.Phrase = new Phrase("Spot/ 100cm", normalTextFont);
        //        dataCellR4C1.HorizontalAlignment = Element.ALIGN_CENTER;
        //        dataPdfTable12.AddCell(dataCellR4C1);
        //        dataCellR1C1.Phrase = new Phrase("", normalTextFont);
        //        for (int i = 1; i <= 9; i++)
        //        {
        //            dataPdfTable12.AddCell(dataCellR1C1);
        //        }
        //        for (int j = 1; j <= 3; j++)
        //        {
        //            dataCellR1C1.Phrase = new Phrase(itemdatafml[j], normalTextFont);
        //            dataPdfTable12.AddCell(dataCellR1C1);
        //            dataCellR1C1.Phrase = new Phrase("", normalTextFont);
        //            for (int i = 1; i <= 9; i++)
        //            {
        //                dataPdfTable12.AddCell(dataCellR1C1);
        //            }
        //        }

        //        doc.Add(dataPdfTable12);

        //        //---------------------------------

        //        dataCell1.Phrase = new Phrase("Test Item", normalTextFont);
        //        dataCell1.Rowspan = 2;
        //        dataCell1.Colspan = 2;
        //        dataPdfTable9p.AddCell(dataCell1);
        //        dataCell1.Phrase = new Phrase("Unit", normalTextFont);
        //        dataCell1.Rowspan = 2;
        //        dataCell1.Colspan = 1;
        //        dataPdfTable9p.AddCell(dataCell1);
        //        dataCellR1C2.Phrase = new Phrase("Results", normalTextFont);

        //        for (int i = 1; i <= 3; i++)
        //        {
        //            dataPdfTable9p.AddCell(dataCellR1C2);
        //        }

        //        for (int i = 1; i <= 3; i++)
        //        {
        //            dataCellR1C1.Phrase = new Phrase("Machine", normalTextFont);
        //            dataPdfTable9p.AddCell(dataCellR1C1);
        //            dataCellR1C1.Phrase = new Phrase("Regressed", normalTextFont);
        //            dataPdfTable9p.AddCell(dataCellR1C1);
        //        }

        //        string[] itemdata3 = new string[4] { "", "Melt Flow Rate (MFR)", "Charpy Impact", "Deflection Temp. Load" };
        //        string[] itemunit3 = new string[4] { "", "g/10 min", "kj/m2", "C" };

        //        for (int j = 1; j <= 3; j++)
        //        {
        //            dataCellR1C2.Phrase = new Phrase(itemdata3[j], normalTextFont);
        //            dataCellR1C2.HorizontalAlignment = Element.ALIGN_LEFT;
        //            dataPdfTable9p.AddCell(dataCellR1C2);

        //            dataCellR1C1.Phrase = new Phrase(itemunit3[j], normalTextFont);
        //            dataCellR1C1.HorizontalAlignment = Element.ALIGN_CENTER;
        //            dataPdfTable9p.AddCell(dataCellR1C1);

        //            dataCellR1C1.Phrase = new Phrase("", normalTextFont);
        //            for (int i = 1; i <= 6; i++)
        //            {
        //                dataPdfTable9p.AddCell(dataCellR1C1);
        //            }
        //        }

        //        string[] itemtensil = new string[5] { "", "Strength at Yield", "Break Stress", "Elongation", "Modulus Of Elastic" };
        //        string[] itemunittensil = new string[5] { "", "MPa", "MPa", "%", "MPa" };

        //        dataCellR4C1.Phrase = new Phrase("Tensile", normalTextFont);
        //        dataCellR4C1.HorizontalAlignment = Element.ALIGN_LEFT;
        //        dataPdfTable9p.AddCell(dataCellR4C1);

        //        for (int j = 1; j <= 4; j++)
        //        {
        //            dataCellR1C1.Phrase = new Phrase(itemtensil[j], normalTextFont);
        //            dataCellR1C1.HorizontalAlignment = Element.ALIGN_LEFT;
        //            dataPdfTable9p.AddCell(dataCellR1C1);
        //            dataCellR1C1.Phrase = new Phrase(itemunittensil[j], normalTextFont);
        //            dataCellR1C1.HorizontalAlignment = Element.ALIGN_CENTER;
        //            dataPdfTable9p.AddCell(dataCellR1C1);
        //            dataCellR1C1.Phrase = new Phrase("", normalTextFont);

        //            for (int i = 1; i <= 6; i++)
        //            {
        //                dataPdfTable9p.AddCell(dataCellR1C1);
        //            }
        //        }

        //        string[] itemflexural = new string[3] { "", "Stress", "Modulus" };
        //        string[] itemunitflexural = new string[3] { "", "MPa", "MPa" };

        //        dataCell1.Phrase = new Phrase("Flexural", normalTextFont);
        //        dataCell1.HorizontalAlignment = Element.ALIGN_LEFT;
        //        dataCell1.Rowspan = 2;
        //        dataCell1.Colspan = 1;
        //        dataPdfTable9p.AddCell(dataCell1);

        //        for (int j = 1; j <= 2; j++)
        //        {
        //            dataCellR1C1.Phrase = new Phrase(itemflexural[j], normalTextFont);
        //            dataCellR1C1.HorizontalAlignment = Element.ALIGN_LEFT;
        //            dataPdfTable9p.AddCell(dataCellR1C1);
        //            dataCellR1C1.Phrase = new Phrase(itemunitflexural[j], normalTextFont);
        //            dataCellR1C1.HorizontalAlignment = Element.ALIGN_CENTER;
        //            dataPdfTable9p.AddCell(dataCellR1C1);
        //            dataCellR1C1.Phrase = new Phrase("", normalTextFont);

        //            for (int i = 1; i <= 6; i++)
        //            {
        //                dataPdfTable9p.AddCell(dataCellR1C1);
        //            }
        //        }

        //        string[] itemfp = new string[3] { "", "Fisheye", "Pellet Weight" };
        //        string[] itemfpunit = new string[3] { "", "level", "g/50pcs." };

        //        for (int j = 1; j <= 2; j++)
        //        {
        //            dataCellR1C2.Phrase = new Phrase(itemfp[j], normalTextFont);
        //            dataCellR1C2.HorizontalAlignment = Element.ALIGN_LEFT;
        //            dataPdfTable9p.AddCell(dataCellR1C2);
        //            dataCellR1C1.Phrase = new Phrase(itemfpunit[j], normalTextFont);
        //            dataCellR1C1.HorizontalAlignment = Element.ALIGN_CENTER;
        //            dataPdfTable9p.AddCell(dataCellR1C1);
        //            dataCellR1C1.Phrase = new Phrase("", normalTextFont);
        //            for (int i = 1; i <= 6; i++)
        //            {
        //                dataPdfTable9p.AddCell(dataCellR1C1);
        //            }
        //        }

        //        doc.Add(dataPdfTable9p);

        //        dataCellR3C1.Phrase = new Phrase("Macaroni", normalTextFont);
        //        dataPdfTable11.AddCell(dataCellR3C1);
        //        dataCellR3C1.Phrase = new Phrase("%", normalTextFont);
        //        dataCellR3C1.HorizontalAlignment = Element.ALIGN_CENTER;
        //        dataPdfTable11.AddCell(dataCellR3C1);

        //        for (int j = 1; j <= 3; j++)
        //        {
        //            for (int i = 0; i < 3; i++)
        //            {
        //                dataCellR1C1.Phrase = new Phrase(itemdatafml[i], normalTextFont);
        //                dataPdfTable11.AddCell(dataCellR1C1);
        //            }
        //        }

        //        dataCellR1C1.Phrase = new Phrase(" ", normalTextFont);

        //        for (int i = 1; i <= 9; i++)
        //        {
        //            dataPdfTable11.AddCell(dataCellR1C1);
        //        }

        //        dataCell1.Phrase = new Phrase("T.Mix = ", normalTextFont);
        //        dataCell1.HorizontalAlignment = Element.ALIGN_LEFT;
        //        dataCell1.Rowspan = 1;
        //        dataCell1.Colspan = 3;
        //        dataPdfTable11.AddCell(dataCell1);
        //        dataCell1.Phrase = new Phrase("ave. = ", normalTextFont);
        //        dataPdfTable11.AddCell(dataCell1);
        //        dataPdfTable11.AddCell(dataCell1);

        //        doc.Add(dataPdfTable11);

        //        //------------------------
        //        string[] itemaftmac = new string[10] { "", "Haze", "Gloss", "Density", "Powder Content", "Glass / Ash Content", "Bromine Content", "Flammability (UL 94)", "Surface Resistivity", "Volatile Content" };
        //        string[] itemaftmacunit = new string[10] { "", "%", "-", "kg/m3", "% wt.", "% wt.", "ppm", "HB", "ohm", "%" };
        //        for (int j = 1; j <= 9; j++)
        //        {
        //            dataCellR1C1.Phrase = new Phrase(itemaftmac[j], normalTextFont);
        //            dataCellR1C1.HorizontalAlignment = Element.ALIGN_LEFT;
        //            dataPdfTable8.AddCell(dataCellR1C1);
        //            dataCellR1C1.Phrase = new Phrase(itemaftmacunit[j], normalTextFont);
        //            dataCellR1C1.HorizontalAlignment = Element.ALIGN_CENTER;
        //            dataPdfTable8.AddCell(dataCellR1C1);
        //            dataCellR1C1.Phrase = new Phrase("", normalTextFont);
        //            for (int i = 1; i <= 6; i++)
        //            {
        //                dataPdfTable8.AddCell(dataCellR1C1);
        //            }
        //        }
        //        //--702
        //        doc.Add(dataPdfTable8);

        //        string[] itemappr = new string[4] { "", "Status", "Checked By", "Approved By" };
        //        string[] itemcnfrm = new string[4] { "", "Grade", "Confirmed By", "Keyed-in By" };

        //        dataCellR3C1.Phrase = new Phrase("CONTRACT COMPOUNDER/ TPM", normalTextFont);
        //        dataPdfTable5p.AddCell(dataCellR3C1);

        //        for (int j = 1; j <= 3; j++)
        //        {
        //            dataCellR1C1.Phrase = new Phrase(itemappr[j], normalTextFont);
        //            dataPdfTable5p.AddCell(dataCellR1C1);
        //            dataCellR1C1.Phrase = new Phrase("", normalTextFont);
        //            for (int i = 1; i <= 3; i++)
        //            {
        //                dataPdfTable5p.AddCell(dataCellR1C1);
        //            }
        //        }

        //        dataCellR3C1.Phrase = new Phrase("TPM", normalTextFont);
        //        dataPdfTable5p.AddCell(dataCellR3C1);

        //        for (int j = 1; j <= 3; j++)
        //        {
        //            dataCellR1C1.Phrase = new Phrase(itemcnfrm[j], normalTextFont);
        //            dataPdfTable5p.AddCell(dataCellR1C1);
        //            dataCellR1C1.Phrase = new Phrase("", normalTextFont);
        //            for (int i = 1; i <= 3; i++)
        //            {
        //                dataPdfTable5p.AddCell(dataCellR1C1);
        //            }
        //        }

        //        doc.Add(dataPdfTable5p);

        //        dataPdfTable5p.DeleteBodyRows();

        //        Image imgUnchecked = Image.GetInstance(Server.MapPath("\\") + "\\images\\Checkbox-Unchecked.png");
        //        Chunk chunkCheckBox = new Chunk(imgUnchecked, 2, 0, false);

        //        dataCellR1C1.Border = Rectangle.NO_BORDER;
        //        dataCellR1C1.Phrase = new Phrase("Pellet sample (pls.tick):", normalTextFont);
        //        dataPdfTable5p.AddCell(dataCellR1C1);

        //        string[] itemchk = new string[5] { "", "YI test", "Fisheye test & Film", "Bromine Content", "Properties test" };
        //        for (int j = 1; j <= 4; j++)
        //        {
        //            Paragraph para = new Paragraph()
        //            {
        //                Leading = 7
        //            };
        //            para.Add(chunkCheckBox);
        //            para.Add(new Chunk("  ", normalTextFont));
        //            para.Add(new Chunk(itemchk[j], normalTextFont));

        //            para.Alignment = Element.ALIGN_TOP;

        //            //Add the paragrph to it
        //            dataCellR1C1.AddElement(para);
        //            dataPdfTable5p.AddCell(dataCellR1C1);
        //            dataCellR1C1.Phrase = null;
        //        }

        //        dataCellR1C1.Phrase = new Phrase("Remarks:-", normalTextFont);
        //        dataCellR1C1.HorizontalAlignment = Element.ALIGN_LEFT;
        //        dataPdfTable5p.AddCell(dataCellR1C1);

        //        for (int i = 1; i <= 4; i++)
        //        {
        //            dataCellR1C1.Phrase = new Phrase("", normalTextFont);
        //            dataPdfTable5p.AddCell(dataCellR1C1);
        //        }

        //        dataCellR1C1.AddElement(dataPdfTable5p);
        //        dataCellR1C1.Border = Rectangle.RECTANGLE;

        //        PdfPTable dataPdfTable1 = CreateTable(new float[] { 450f });//new PdfPTable(new float[] { 450f });
        //        dataPdfTable1.AddCell(dataCellR1C1);

        //        doc.Add(dataPdfTable1);
        //        // close document, stream and writer
        //        doc.Close();
        //        doc.CloseDocument();
        //        doc.Dispose();
        //        writer.Close();
        //        writer.Dispose();
        //        fs.Close();
        //        fs.Dispose();
        //        // Return file bytes
        //        byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
        //        Response.Clear();
        //        Response.ContentType = "application/pdf";
        //        Response.AppendHeader("Content-Disposition", "attachment; filename=IDS_REPORT.pdf");
        //        Response.TransmitFile(filePath);
        //        Response.End();
        //        System.IO.File.Delete(filePath);
        //        return fileBytes;

        //        //746
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        return null;
        //    }
        //    finally
        //    {
        //        // close document, stream and writer
        //        doc.Close();
        //        doc.CloseDocument();
        //        doc.Dispose();
        //        writer.Close();
        //        writer.Dispose();
        //        fs.Close();
        //        fs.Dispose();
        //    }
        //}

        public async Task<byte[]> Test(string id)
        {
            // Create path to pdf file
            string filePath = Server.MapPath("\\") + "IDS_REPORT" + ".pdf";
            string username = (Session["AclUser"] as ACL_UserObj).USER_ID.ToString();
            // Create document and its writing process
            Document document = new Document(new Rectangle(288f, 144f), 15, 15, 20, 10);
            document.SetPageSize(PageSize.A4);
            document.AddAuthor(username);

            FileStream fs = new FileStream(filePath, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            writer.PageEvent = new ITextEvents(username);

            document.Open();

            try
            {
                // Header values
                int counter = 0;
                BaseColor altRowColor = new BaseColor(227, 227, 227);

                Font titleFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 14);
                Font subTitleFont = new Font(Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD, BaseColor.BLACK);
                Font tableTitleFont = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 8, Font.UNDERLINE | Font.BOLD, BaseColor.BLACK);
                Font normalTextFont = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 8);

                Font colHeaderFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8);
                Font colNormalFont = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 8);

                // Title of report
                Paragraph title = new Paragraph("IDS Report", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 5
                };
                document.Add(title);

                /////////////////////////////////////////////// Get Summary of Report /////////////////////////////////////////////////////////

                Paragraph summaryTitle = new Paragraph("Summary", subTitleFont);
                summaryTitle.Alignment = Element.ALIGN_LEFT;
                document.Add(summaryTitle);

                // Get Data
                SummaryVM summaryVM = await db.GetSummaryData(id);

                PdfPTable summaryPdfTable = new PdfPTable(new float[] { 10f, 1f, 10f, 10f, 1f, 10f });
                //summaryPdfTable.TotalWidth = 750f;
                //summaryPdfTable.LockedWidth = true;
                summaryPdfTable.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                summaryPdfTable.SpacingAfter = 10f;
                summaryPdfTable.SpacingBefore = 10f;

                // Create cells to use
                PdfPCell summaryTitleCell = new PdfPCell()
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    Border = Rectangle.NO_BORDER
                };

                PdfPCell summaryValueCell = new PdfPCell()
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    Border = Rectangle.NO_BORDER
                };

                PdfPCell summaryEmptyCell = new PdfPCell(new Phrase(":", colNormalFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = Rectangle.NO_BORDER
                };

                // Loop summary vm properties - get the value
                List<PropertyInfo> summaryProperties = summaryVM.GetType().GetProperties().ToList();
                string[] propertyValuesArray = new string[summaryProperties.Count];
                for (int i = 0; i < summaryProperties.Count; i++)
                {
                    PropertyInfo property = summaryProperties[i];

                    // Get the value of the property as a string
                    string value = property.GetValue(summaryVM)?.ToString();

                    // Assign the value to the array at the corresponding index
                    propertyValuesArray[i] = value;
                }

                // This is used to create the title -- A very stupid ways, if have any prefer method, can replace my code or take reference with original code below
                // For the data, need to modified the sequence in function above
                // The arrangement change need to change the data sequence by modal [SummaryVM] directly 
                string[] titleArray = typeof(SummaryVM)
            .GetProperties()
            .Select(property => property.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName)
            .Where(displayName => displayName != null)
            .ToArray();

                int loopIterations = 6;
                double nextLoopIterations = loopIterations / 2;
                int roundedLoopIterations = (int)Math.Ceiling(nextLoopIterations);// Total number of output data
                // This code only can doing the code which is same row of 2 vertical. For example:
                // Title 1 : Data 1                Title 3 : Data 3
                // Title 2 : Data 2                Title 4 : Data 4
                // This is because the limitation of this condition [i <= loopIterations && j <= loopIterations], which was bring a issues which can't get the remain row of data in first vertical
                // The odd number of the loopIterations will cause 2 vertical length is different, and the first vertical will be always more then second vertical (not sure this logic is correct or not)
                // Therefore, need 1 fomula to get the remain data to show out [I'm sorry about my broken English if you still can't understand my code logic...] 
                for (int i = 0, j = roundedLoopIterations + 1; i <= roundedLoopIterations; i++, j++)
                {
                    if (i <= roundedLoopIterations)
                    {
                        string displayName = titleArray[i];
                        string value = propertyValuesArray[i];

                        summaryTitleCell.Phrase = new Phrase(displayName, normalTextFont);
                        summaryValueCell.Phrase = new Phrase(value, normalTextFont);

                        summaryPdfTable.AddCell(summaryTitleCell);
                        summaryPdfTable.AddCell(summaryEmptyCell);
                        summaryPdfTable.AddCell(summaryValueCell);
                    }
                    else
                    {
                        // Empty if no more data
                        summaryTitleCell.Phrase = new Phrase("", normalTextFont);
                        summaryValueCell.Phrase = new Phrase("", normalTextFont);

                        summaryPdfTable.AddCell(summaryTitleCell);
                        summaryPdfTable.AddCell(summaryValueCell);
                        summaryPdfTable.AddCell(summaryValueCell);
                    }

                    if (j <= loopIterations)
                    {
                        string secondDisplayName = titleArray[j];
                        string secondValue = propertyValuesArray[j];

                        summaryTitleCell.Phrase = new Phrase(secondDisplayName, normalTextFont);
                        summaryValueCell.Phrase = new Phrase(secondValue, normalTextFont);

                        summaryPdfTable.AddCell(summaryTitleCell);
                        summaryPdfTable.AddCell(summaryEmptyCell);
                        summaryPdfTable.AddCell(summaryValueCell);
                    }
                    else
                    {
                        // Empty if no more data
                        summaryTitleCell.Phrase = new Phrase("", normalTextFont);
                        summaryValueCell.Phrase = new Phrase("", normalTextFont);
                        summaryPdfTable.AddCell(summaryTitleCell);
                        summaryPdfTable.AddCell(summaryValueCell);
                        summaryPdfTable.AddCell(summaryValueCell);
                    }
                }

                document.Add(summaryPdfTable);
                document.Add(new LineSeparator());

                //////////////////////////////////////////////// MOLDING AND PROPERTY /////////////////////////////////////////////////////////////////////////////// 
                // Reset Counter
                counter = 0;

                Paragraph moldAndPropTitle = new Paragraph("Molding and Property", subTitleFont)
                {
                    Alignment = Element.ALIGN_LEFT,
                    SpacingAfter = 10f
                };
                document.Add(moldAndPropTitle);

                // string of column header                
                string[] colHeader = { "Property", "Unit", "m/c", "Machine Result", "Regression Result", "COQ Adjust", "Specification", "Grade", "Status" };
                PdfPTable moldAndPropPdfTable = new PdfPTable(new float[] { 15f, 5f, 10f, 10f, 10f, 10f, 15f, 5f, 10f });
                moldAndPropPdfTable.WidthPercentage = 100;
                moldAndPropPdfTable.SpacingAfter = 5f;
                moldAndPropPdfTable.SpacingBefore = 5f;

                // Header
                foreach (string colHeaderStr in colHeader)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(colHeaderStr, colHeaderFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    moldAndPropPdfTable.AddCell(cell);
                }

                // Data Area
                List<MoldAndPropVM> MoldingAndPropertyDataTable = await db.GetMoldingPropertyData(summaryVM.PRODTYPE, summaryVM.LOTNO, "2");
                foreach (MoldAndPropVM row in MoldingAndPropertyDataTable)
                {
                    // Create cell with different colo
                    counter++;
                    PdfPCell cell = new PdfPCell()
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        PaddingTop = 3,
                        PaddingBottom = 3,
                        BackgroundColor = counter % 2 == 0 ? BaseColor.WHITE : altRowColor
                    };

                    // Loop property and get values
                    List<PropertyInfo> moldAndPropProperties = row.GetType().GetProperties().ToList();
                    foreach (PropertyInfo property in moldAndPropProperties)
                    {
                        string value = row.GetType().GetProperty(property.Name).GetValue(row).ToString() ?? "";
                        cell.Phrase = new Phrase(value, colNormalFont);
                        moldAndPropPdfTable.AddCell(cell);
                    }
                }

                // Add table
                document.Add(moldAndPropPdfTable);

                //////// Singnature ///////
                PdfPTable signTable = new PdfPTable(new float[] { 8f, 8f, 8f, 8f });
                signTable.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                signTable.SpacingAfter = 10f;
                signTable.SpacingBefore = 10f;

                // Create cells to use
                PdfPCell signTitleCell = new PdfPCell()
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    Border = Rectangle.NO_BORDER
                };

                PdfPCell signLineLineCell = new PdfPCell()
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    Border = Rectangle.ALIGN_BOTTOM
                };

                PdfPCell signEmptyCell = new PdfPCell(new Phrase("", colNormalFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = Rectangle.NO_BORDER
                };

                PdfPCell paddingCell = new PdfPCell(new Phrase(" ", colNormalFont))
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 20,
                    PaddingBottom = 20,
                    Border = Rectangle.NO_BORDER
                };

                PdfPCell signTitleCellBorder = new PdfPCell()
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };

                PdfPCell signRowCellBorder = new PdfPCell()
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };

                signTitleCellBorder.Phrase = new Phrase(" ", normalTextFont);
                signTable.AddCell(signTitleCellBorder);
                signTitleCellBorder.Phrase = new Phrase("Signature", normalTextFont);
                signTable.AddCell(signTitleCellBorder);
                signTitleCellBorder.Phrase = new Phrase("Name", normalTextFont);
                signTable.AddCell(signTitleCellBorder);
                signTitleCellBorder.Phrase = new Phrase("Date", normalTextFont);
                signTable.AddCell(signTitleCellBorder);

                signRowCellBorder.Phrase = new Phrase("Molding & Property", normalTextFont);
                signTable.AddCell(signRowCellBorder);
                signTitleCellBorder.Phrase = new Phrase(" ", normalTextFont);
                signTable.AddCell(signTitleCellBorder);
                signTitleCellBorder.Phrase = new Phrase(" ", normalTextFont);
                signTable.AddCell(signTitleCellBorder);
                signTitleCellBorder.Phrase = new Phrase(" ", normalTextFont);
                signTable.AddCell(signTitleCellBorder);

                signRowCellBorder.Phrase = new Phrase("Appearance", normalTextFont);
                signTable.AddCell(signRowCellBorder);
                signTitleCellBorder.Phrase = new Phrase(" ", normalTextFont);
                signTable.AddCell(signTitleCellBorder);
                signTitleCellBorder.Phrase = new Phrase(" ", normalTextFont);
                signTable.AddCell(signTitleCellBorder);
                signTitleCellBorder.Phrase = new Phrase(" ", normalTextFont);
                signTable.AddCell(signTitleCellBorder);

                signRowCellBorder.Phrase = new Phrase("Checked by", normalTextFont);
                signTable.AddCell(signRowCellBorder);
                signTitleCellBorder.Phrase = new Phrase(" ", normalTextFont);
                signTable.AddCell(signTitleCellBorder);
                signTitleCellBorder.Phrase = new Phrase(" ", normalTextFont);
                signTable.AddCell(signTitleCellBorder);
                signTitleCellBorder.Phrase = new Phrase(" ", normalTextFont);
                signTable.AddCell(signTitleCellBorder);

                signRowCellBorder.Phrase = new Phrase("Verified by", normalTextFont);
                signTable.AddCell(signRowCellBorder);
                signTitleCellBorder.Phrase = new Phrase(" ", normalTextFont);
                signTable.AddCell(signTitleCellBorder);
                signTitleCellBorder.Phrase = new Phrase(" ", normalTextFont);
                signTable.AddCell(signTitleCellBorder);
                signTitleCellBorder.Phrase = new Phrase(" ", normalTextFont);
                signTable.AddCell(signTitleCellBorder);

                signRowCellBorder.Phrase = new Phrase("Keyed-in by", normalTextFont);
                signTable.AddCell(signRowCellBorder);
                signTitleCellBorder.Phrase = new Phrase(" ", normalTextFont);
                signTable.AddCell(signTitleCellBorder);
                signTitleCellBorder.Phrase = new Phrase(" ", normalTextFont);
                signTable.AddCell(signTitleCellBorder);
                signTitleCellBorder.Phrase = new Phrase(" ", normalTextFont);
                signTable.AddCell(signTitleCellBorder);
                // Add table
                document.Add(signTable);

                ////////////////////////////////////////////////// HISTORY TRANSACTION TABLE //////////////////////////////////////////////////////////////////////////
                document.NewPage();
                counter = 0;

                // Get ID_IDS_D joined as string
                string idForHistory = await db.GetID_IDS_D(summaryVM.PRODTYPE, summaryVM.LOTNO);

                // Title
                Paragraph histTransTitle = new Paragraph("History Transaction", subTitleFont)
                {
                    Alignment = Element.ALIGN_LEFT,
                    SpacingAfter = 10f
                };
                document.Add(histTransTitle);

                // string of column header
                string[] histTransColHeader = { "Property", "Reading#1", "Reading#2", "Reading#3", "Reading#4", "Reading#5", "Reading#6", "Average", "Grade", "Tested Date Time" };

                PdfPTable histTransTable = new PdfPTable(new float[] { 15f, 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f });
                histTransTable.WidthPercentage = 100;
                histTransTable.SpacingAfter = 5f;
                histTransTable.SpacingBefore = 5f;

                // Header
                foreach (string colHeaderStr in histTransColHeader)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(colHeaderStr, colHeaderFont))
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    histTransTable.AddCell(cell);
                }

                // Data Area
                DataTable HistTransDataTable = await db.GetHistoryTransactionData(idForHistory, summaryVM.PRODTYPE, summaryVM.LOTNO);
                foreach (DataRow row in HistTransDataTable.Rows)
                {
                    counter++;
                    PdfPCell cell = new PdfPCell()
                    {
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        PaddingTop = 3,
                        PaddingBottom = 3,
                        BackgroundColor = counter % 2 == 0 ? BaseColor.WHITE : altRowColor

                    };

                    cell.Phrase = new Phrase(row["PROPITEM"].ToString(), colNormalFont);
                    histTransTable.AddCell(cell);

                    cell.Phrase = new Phrase(row["READING1"].ToString(), colNormalFont);
                    histTransTable.AddCell(cell);

                    cell.Phrase = new Phrase(row["READING2"].ToString(), colNormalFont);
                    histTransTable.AddCell(cell);

                    cell.Phrase = new Phrase(row["READING3"].ToString(), colNormalFont);
                    histTransTable.AddCell(cell);

                    cell.Phrase = new Phrase(row["READING4"].ToString(), colNormalFont);
                    histTransTable.AddCell(cell);

                    cell.Phrase = new Phrase(row["READING5"].ToString(), colNormalFont);
                    histTransTable.AddCell(cell);

                    cell.Phrase = new Phrase(row["READING6"].ToString(), colNormalFont);
                    histTransTable.AddCell(cell);

                    cell.Phrase = new Phrase(row["AVERAGE"].ToString(), colNormalFont);
                    histTransTable.AddCell(cell);

                    cell.Phrase = new Phrase(row["GRADE"].ToString(), colNormalFont);
                    histTransTable.AddCell(cell);

                    cell.Phrase = new Phrase(row["TESTEDDATETIME"].ToString(), colNormalFont);
                    histTransTable.AddCell(cell);
                }

                // Add table
                document.Add(histTransTable);

                ////////////////////////////////////////////////// APPEARANCE ///////////////////////////////////////////////////////////////////////////////
                document.NewPage();

                // Title
                Paragraph appearanceTitle = new Paragraph("Appearance", subTitleFont)
                {
                    Alignment = Element.ALIGN_LEFT,
                    SpacingAfter = 10f
                };
                document.Add(appearanceTitle);

                // Loop twice
                for (int type = 1; type <= 2; type++)
                {
                    //Add Table title
                    Paragraph appearanceTableTitle = new Paragraph(type == 1 ? "Black Speck" : "YI", tableTitleFont)
                    {
                        Alignment = Element.ALIGN_JUSTIFIED,
                        SpacingAfter = 5f
                    };
                    document.Add(appearanceTableTitle);

                    // Get Appearance Table
                    DataTable appearanceDataTable = await db.GetAppearanceTable(summaryVM.PRODTYPE, summaryVM.LOTNO, type);
                    List<string> columnHeaderList = new List<string>();


                    // Create PDF Table
                    PdfPTable appearancePdfTable = new PdfPTable(appearanceDataTable.Columns.Count)
                    {
                        //TotalWidth = 50f * (appearanceDataTable.Columns.Count),
                        //LockedWidth = true,
                        WidthPercentage = 100,
                        SpacingAfter = 10f,
                        SpacingBefore = 5f,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };

                    // Set Column Header
                    foreach (DataColumn dataColumn in appearanceDataTable.Columns)
                    {
                        columnHeaderList.Add(dataColumn.ColumnName);
                        PdfPCell cell = new PdfPCell(new Phrase(dataColumn.ColumnName, colHeaderFont))
                        {
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            Padding = 3f,
                        };
                        appearancePdfTable.AddCell(cell);
                    }

                    // Set Values
                    foreach (DataRow row in appearanceDataTable.Rows)
                    {
                        foreach (string columnName in columnHeaderList)
                        {
                            PdfPCell cell = new PdfPCell(new Phrase(row[columnName].ToString(), colNormalFont))
                            {
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_CENTER,
                                Padding = 3f,
                            };

                            appearancePdfTable.AddCell(cell);
                        }
                    }

                    // Add Document                
                    document.Add(appearancePdfTable);
                }

                // close document, stream and writer
                document.Close();
                document.CloseDocument();
                document.Dispose();
                writer.Close();
                writer.Dispose();
                fs.Close();
                fs.Dispose();
                // Return file bytes
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AppendHeader("Content-Disposition", "attachment; filename=IDS_REPORT.pdf");
                Response.TransmitFile(filePath);
                Response.End();
                System.IO.File.Delete(filePath);
                return fileBytes;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            finally
            {
                // close document, stream and writer
                document.Close();
                document.CloseDocument();
                document.Dispose();
                writer.Close();
                writer.Dispose();
                fs.Close();
                fs.Dispose();
            }
        }

        [SessionExpire]
        public async Task<ActionResult> TREND()
        {
            ReportYiVM model = new ReportYiVM();
            model.DropdownPropItem = await LoadDllData(0, "", "PROP_ITEM_ALL");
            model.DropdownProdType = await db.GetProdTypeList();
            //ViewBag.prodtype = db.GetProdTypeList();
            Session["refreshtrend"] = 1;
            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> TREND(ReportYiVM m)
        {
            m.DropdownPropItem = await LoadDllData(0, "", "PROP_ITEM_ALL");
            //ViewBag.prodtype = db.GetProdTypeList();
            m.DropdownProdType = await db.GetProdTypeList();
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            ExcelPackage pck = new ExcelPackage();
            pck.Workbook.Worksheets.Add("Trend Chart");
            ExcelWorksheet ws = pck.Workbook.Worksheets[0];
            // ExcelWorksheet w1 = pck.Workbook.Worksheets[1];

            ws.PrinterSettings.Orientation = eOrientation.Landscape;
            ws.Cells.AutoFitColumns();

            List<DataSet> ds = new List<DataSet>();
            List<DataTable> dt = new List<DataTable>();
            DataTable lhspec = new DataTable();
            bool proceed = true;
            lhspec = await db.getLHSpec(m.prodtype, m.listproperties, m.prodlinefrom, m.prodlineto);
            string addtable = "";
            string listProp = "";
            int coldatastart = 4;
            int coldataend = coldatastart;
            if (lhspec == null)
            {
                proceed = false;
            }
            else
            {
                if (lhspec.Rows.Count <= 0)
                {
                    proceed = false;
                }
            }

            for (int i = 0; i < lhspec.Rows.Count; i++)
            {
                if (lhspec.Rows[i]["ORA_COLUMN"].ToString() != "")
                {
                    addtable = addtable + ", TO_NUMBER(r." + lhspec.Rows[i]["ORA_COLUMN"].ToString() + ") as \"" + lhspec.Rows[i]["PROP_ITEM"].ToString() + "\" ";
                    if (listProp == "") {
                        listProp = lhspec.Rows[i]["PROPERTIES"].ToString() + "-" + lhspec.Rows[i]["PROP_ITEM"].ToString();
                    }
                    else
                    {
                        listProp = listProp + ", " + lhspec.Rows[i]["PROPERTIES"].ToString() + "-" + lhspec.Rows[i]["PROP_ITEM"].ToString();
                    }
                    coldataend = coldataend + 1;
                }

            }

            ds = await o.YI(m, addtable);

            if (ds == null)
            {
                proceed = false;
            }
            foreach (var i in ds)
            {
                dt.Add(i.Tables[0]);
                if (i.Tables[0] == null)
                {
                    proceed = false;
                } else
                {
                    if(i.Tables[0].Rows.Count <= 0)
                    {
                        proceed = false;
                    }
                }
            }

            if (proceed == false)
            {
                ViewBag.Error = "No data to export.";
            }
            else
            {
                if (lhspec.Rows.Count > 0)
                {
                    //da.Fill(dt);
                    ws.Cells["A1:H2"].Merge = true;
                    ws.Cells["A1:H2"].Style.Font.Size = 14.3f;
                    ws.Cells["A1:H2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Cells["A1:H2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    ws.Cells["A1:H2"].Value = m.prodtype + " Trend Chart (" + m.datefrom + " to " + m.dateto + ")";
                    ws.Cells[3, 1].Value = "Production Line: ";
                    ws.Cells[3, 2].Value = m.prodlinefrom + " to " + m.prodlineto;
                    ws.Cells[4, 1].Value = "Property(ies): ";
                    ws.Cells[4, 2].Value = listProp;
                    ws.Cells[2, 14].Value = "Report Generate Date: " + DateTime.Now.ToString("dd-MM-yyyy hh:mm tt");
                    int rowdatastrt = 7;
                    int rowdataend = rowdatastrt;

                    for (int i = 0; i < dt.Count; i++)
                    {
                        dt[i].TableName = dt[i].TableName + i;
                        if (i == 0)
                        {
                            ws.Cells[6, 1].LoadFromDataTable(dt[i], true, OfficeOpenXml.Table.TableStyles.Light11);
                            ws.Cells[7, 3, dt[i].Rows.Count + 7, 3].Style.Numberformat.Format = "dd/mm/yyyy";
                            ws.Cells[6, 1, (dt[i].Rows.Count + 6 + 8), 5 + lhspec.Rows.Count].AutoFitColumns();
                        }
                        else
                        {
                            ws.Cells[6, 3].LoadFromDataTable(dt[i], true, OfficeOpenXml.Table.TableStyles.Light11);
                        }
                        rowdataend = rowdatastrt + dt[i].Rows.Count;
                    }
                    ws.Cells[rowdatastrt, coldatastart, rowdataend - 1, coldataend - 1].Style.Numberformat.Format = "#,###,##0.00";
                    ws.Cells[rowdatastrt, coldataend, rowdataend - 1, coldataend].Style.Numberformat.Format = "#,###,##0";

                    for (int i = 1; i < 10; i++)
                    {
                        ws.Cells[(rowdataend + i), 1, (rowdataend + i), 2].Merge = true;
                    }

                    ws.Cells[(rowdataend + 1), 1].Value = "Average";
                    ws.Cells[(rowdataend + 2), 1].Value = "Min";
                    ws.Cells[(rowdataend + 3), 1].Value = "Max";
                    ws.Cells[(rowdataend + 4), 1].Value = "Std Dev";
                    ws.Cells[(rowdataend + 5), 1].Value = "CPL";
                    ws.Cells[(rowdataend + 6), 1].Value = "CPU";
                    ws.Cells[(rowdataend + 7), 1].Value = "CPK";
                    ws.Cells[(rowdataend + 8), 1].Value = "Lower Limit";
                    ws.Cells[(rowdataend + 9), 1].Value = "Upper Limit";

                    int cols = 4;
                    for (int i = 0; i < lhspec.Rows.Count; i++)
                    {
                        if (lhspec.Rows[i]["ORA_COLUMN"].ToString() != "" && lhspec.Rows[i]["ORA_COLUMN"].ToString() != null)
                        {
                            string colsalp = GetExcelColumnName(cols);
                            ws.Cells[(rowdataend + 1), cols].Formula = "AVERAGE(" + colsalp + rowdatastrt + ":" + colsalp + (rowdataend - 1) + ")";
                            ws.Cells[(rowdataend + 2), cols].Formula = "MIN(" + colsalp + rowdatastrt + ":" + colsalp + (rowdataend - 1) + ")";
                            ws.Cells[(rowdataend + 3), cols].Formula = "MAX(" + colsalp + rowdatastrt + ":" + colsalp + (rowdataend - 1) + ")";
                            ws.Cells[(rowdataend + 4), cols].Formula = "_xlfn.STDEV.S(" + colsalp + rowdatastrt + ":" + colsalp + (rowdataend - 1) + ")";
                            ws.Cells[(rowdataend + 5), cols].Formula = "IF((3*_xlfn.STDEV.S(" + colsalp + rowdatastrt + ":" + colsalp + (rowdataend - 1) + ")) = 0, 0, " + "(" + colsalp + (rowdataend + 1) + " - " + colsalp + (rowdataend + 8) + ")/(3*_xlfn.STDEV.S(" + colsalp + rowdatastrt + ":" + colsalp + (rowdataend - 1) + ")))";
                            ws.Cells[(rowdataend + 6), cols].Formula = "IF((3*_xlfn.STDEV.S(" + colsalp + rowdatastrt + ":" + colsalp + (rowdataend - 1) + ")) = 0, 0, " + "(" + colsalp + (rowdataend + 9) + " - " + colsalp + (rowdataend + 1) + ")/(3*_xlfn.STDEV.S(" + colsalp + rowdatastrt + ":" + colsalp + (rowdataend - 1) + ")))";
                            ws.Cells[(rowdataend + 7), cols].Formula = "MIN(" + colsalp + (rowdataend + 5) + ":" + colsalp + (rowdataend + 6) + ")";
                            ws.Cells[(rowdataend + 8), cols].Value = Convert.ToDecimal(lhspec.Rows[i]["LOWERSPEC"].ToString());
                            ws.Cells[(rowdataend + 9), cols].Value = Convert.ToDecimal(lhspec.Rows[i]["HIGHERSPEC"].ToString());

                            cols = cols + 1;
                        }
                    }

                    ws.Cells[(rowdataend + 1), 4, (rowdataend + 9), (cols - 1)].Style.Numberformat.Format = "#,##0.00";

                    ws.Cells[(rowdataend + 1), 1, (rowdataend + 9), (cols - 1)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells[(rowdataend + 1), 1, (rowdataend + 9), (cols - 1)].Style.Border.Top.Color.SetColor(System.Drawing.Color.DarkGray);
                    ws.Cells[(rowdataend + 1), 1, (rowdataend + 9), (cols - 1)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells[(rowdataend + 1), 1, (rowdataend + 9), (cols - 1)].Style.Border.Right.Color.SetColor(System.Drawing.Color.DarkGray);
                    ws.Cells[(rowdataend + 1), 1, (rowdataend + 9), (cols - 1)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[(rowdataend + 1), 1, (rowdataend + 9), (cols - 1)].Style.Border.Left.Color.SetColor(System.Drawing.Color.DarkGray);
                    ws.Cells[(rowdataend + 1), 1, (rowdataend + 9), (cols - 1)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    ws.Cells[(rowdataend + 1), 1, (rowdataend + 9), (cols - 1)].Style.Border.Bottom.Color.SetColor(System.Drawing.Color.DarkGray);

                    cols = 4;
                    int chartcol = coldataend + 2;
                    int chartrow = 4;
                    int cntchrt = 0;
                    for (int i = 0; i < lhspec.Rows.Count; i++)
                    {
                        if (lhspec.Rows[i]["ORA_COLUMN"].ToString() != "" && lhspec.Rows[i]["ORA_COLUMN"].ToString() != null)
                        {
                            string colsalp = GetExcelColumnName(cols);
                            var chart = ws.Drawings.AddChart(lhspec.Rows[i]["PROP_ITEM"].ToString() + " vs " + m.xaxis + "_" + i.ToString(), eChartType.LineMarkers);
                            if (dt.Count != 0)
                            {

                                chart.Series.Add(colsalp + rowdatastrt + ":" + colsalp + (rowdataend - 1), "B" + rowdatastrt + ":B" + (rowdataend - 1));
                                string upperlist = "{";
                                string lowerlist = "{";
                                for (int cc = 0; cc < rowdataend - rowdatastrt; cc++)
                                {
                                    upperlist = upperlist + lhspec.Rows[i]["HIGHERSPEC"].ToString();
                                    lowerlist = lowerlist + lhspec.Rows[i]["LOWERSPEC"].ToString();
                                    if (cc != rowdataend - rowdatastrt - 1)
                                    {
                                        upperlist = upperlist + ",";
                                        lowerlist = lowerlist + ",";
                                    }

                                }
                                upperlist = upperlist + "}";
                                lowerlist = lowerlist + "}";
                                //chart.Series.Add(colsalp + (rowdataend + 9) + ":" + colsalp + (rowdataend + 9), "B" + rowdatastrt + ":B" + (rowdataend - 1));
                                chart.Series.Add(upperlist, "B" + rowdatastrt + ":B" + (rowdataend - 1));
                                chart.Series.Add(lowerlist, "B" + rowdatastrt + ":B" + (rowdataend - 1));
                                chart.SetPosition(chartrow, chartrow, chartcol, chartcol);
                                chart.SetSize(420, 320);

                                chart.XAxis.Title.Text = "Lot No";
                                chart.YAxis.Title.Text = lhspec.Rows[i]["PROP_ITEM"].ToString();

                                chart.YAxis.Title.TextBody.Rotation = 270;


                                chart.Title.Text = lhspec.Rows[i]["PROP_ITEM"].ToString() + " vs " + m.xaxis;
                                chart.ShowDataLabelsOverMaximum = true;
                            }
                            cols = cols + 1;
                            chartcol = chartcol + 9;

                            if ((cntchrt + 1) % 2 == 0)
                            {
                                chartcol = coldataend + 2;
                                chartrow = chartrow + 20;
                            }
                            cntchrt = cntchrt + 1;
                        }

                    }

                    Response.Clear();
                    Response.Charset = System.Text.UTF8Encoding.UTF8.WebName;
                    Response.ContentEncoding = System.Text.UTF8Encoding.UTF8;
                    Response.AddHeader("content-disposition", "attachment;  filename=TrendReport.xlsx");
                    Response.ContentType = "application/text";
                    Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
                    Response.BinaryWrite(pck.GetAsByteArray());
                    Response.End();
                }
            }
            return View(m);
        }

        private string GetExcelColumnName(int columnNumber)
        {
            string columnName = "";

            while (columnNumber > 0)
            {
                int modulo = (columnNumber - 1) % 26;
                columnName = Convert.ToChar('A' + modulo) + columnName;
                columnNumber = (columnNumber - modulo) / 26;
            }

            return columnName;
        }

        public async Task<ActionResult> MTHLY_NG()
        {
            ViewBag.msg = "";
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> MTHLY_NG(ReportNGVM model)
        {

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            ExcelPackage pck = new ExcelPackage();

            //'2024-02-01' > '01/02/2024' 
            string[] dtfr = model.datefrom.Split('-');
            model.datefrom = dtfr[2] + "/" + dtfr[1] + "/" + dtfr[0];
            string[] dtto = model.dateto.Split('-');
            model.dateto = dtto[2] + "/" + dtto[1] + "/" + dtto[0];
            List<ReportNGVM> data = await db.getNGReport(model);
            List<ReportNGVM> data07 = new List<ReportNGVM>();
            List<ReportNGVM> datace = new List<ReportNGVM>();
            var selected = data.Where(item => item.prodline == "07").ToList();
            selected.ForEach(item => data.Remove(item));
            data07.AddRange(selected);
            var selected2 = data.Where(item => item.prodline != "" && Convert.ToInt32(item.prodline) > 19).ToList();
            selected2.ForEach(item => data.Remove(item));
            datace.AddRange(selected2);
            DataTable dt = await o.MonthlyNG(model);
            DataTable dt2 = await o.MonthlyNGCap7(model);


            ReportSummary sum = new ReportSummary();
            sum.datefrom = model.datefrom;
            sum.dateto = model.dateto;

            List<ReportSummary> rptSUm = await db.getSummaryNG(sum);

            int row = dt.Rows.Count;
            int rr = 0;
            for (int i = 0; i < 4; i++) //number of sheets
            {

                if (i == 0) // sheet 1 = cap
                {
                    pck.Workbook.Worksheets.Add("CAP (NG)");
                    ExcelWorksheet ws = pck.Workbook.Worksheets[i];

                    ws.Cells["B2"].Value = "QAS MONTHLY NG & R GRADE REPORT";
                    ws.Cells["B2"].Style.Font.Bold = true;
                    ws.Cells["B4"].Value = "OVERALL SUMMARY";
                    ws.Cells["B4"].Style.Font.Bold = true;

                    //============================================== header table 1 ====================================================
                    ws.Cells["C6:D6"].Merge = true;
                    ws.Cells["C6:D6"].Value = "Inspected Lot";
                    ws.Cells["C6:D6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    ws.Cells["E6:F6"].Merge = true;
                    ws.Cells["E6:F6"].Value = "NG Lot";
                    ws.Cells["E6:F6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    ws.Cells["B7:F7"].Value = "Process";
                    ws.Cells["B6,B7:F7,C6:D6,E6:F6"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["B6,B7:F7,C6:D6,E6:F6"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["B6,B7:F7,C6:D6,E6:F6"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["B6,B7:F7,C6:D6,E6:F6"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["C7"].Value = "Lot";
                    ws.Cells["D7"].Value = "Qty (mt)";
                    ws.Cells["E7"].Value = "Lot";
                    ws.Cells["F7"].Value = "Qty (mt)";

                    //======================================== data row table 1 ============================================================================

                    for (int j = 0; j < rptSUm.Count; j++)
                    {
                        ws.Cells[(7 + j + 1), 2].Value = rptSUm[j].process;
                        ws.Cells[(7 + j + 1), 3].Value = rptSUm[j].inspect_lot;
                        ws.Cells[(7 + j + 1), 4].Value = rptSUm[j].inspect_qty;
                        ws.Cells[(7 + j + 1), 5].Value = rptSUm[j].ng_lot;
                        ws.Cells[(7 + j + 1), 6].Value = rptSUm[j].ng_qty;

                        ws.Cells[(7 + j + 1), 2, (7 + j + 1), 6].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[(7 + j + 1), 2, (7 + j + 1), 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[(7 + j + 1), 2, (7 + j + 1), 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[(7 + j + 1), 2, (7 + j + 1), 6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }

                    ws.Cells["B13"].Value = "NG GRADE";

                    //=========================================================== NG Grade table (header) =======================================================================================

                    ws.Cells["B16"].Value = "Abnormality";
                    ws.Cells["C16"].Value = "Type";
                    ws.Cells["D16"].Value = "Lot No";
                    ws.Cells["E16"].Value = "Pack Date";
                    ws.Cells["F16"].Value = "Lot Qty (Kgs)";
                    ws.Cells["G16"].Value = "NG Qty (Kgs)";

                    ws.Cells["B16:G16"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["B16:G16"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["B16:G16"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["B16:G16"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["B16:G16"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    int gap = 0;
                    int TotalRow = 0;
                    int perItemRow = 0; // KL Ong 20221228
                    int gtotal = 0, gabs = 0, gmabs = 0, gtotal07 = 0, gngtotal = 0, gngabs = 0, gngmabs = 0, gtotalng07 = 0;
                    for (int k = 0; k < data.Count; k++)
                    {
                        var temp = "";
                        temp = data[k].abnormality.ToString();


                        // Added by KL Ong 20221228
                        if (k == 0)
                        {
                            ws.Cells[17, 2, 17, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells[17, 2, 17, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            ws.Cells[17, 2, 17, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells[17, 2, 17, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            ws.Cells[17, 2].Value = data[k].abnormality;
                            ws.Cells[17, 3].Value = data[k].type;
                            ws.Cells[17, 4].Value = data[k].lotno;
                            ws.Cells[17, 5].Value = data[k].packkeddate;
                            ws.Cells[17, 6].Value = data[k].quantity;
                            ws.Cells[17, 7].Value = data[k].ng_qty;

                            TotalRow += 1;
                            perItemRow += 1;
                            ws.Cells[17 + TotalRow, 5].Value = "Total";
                            ws.Cells[17 + TotalRow, 6].Value = data[k].quantity;
                            ws.Cells[17 + TotalRow, 7].Value = data[k].ng_qty;

                            if (perItemRow == data[k].rowspan)
                            {
                                ws.Cells[17 + TotalRow + gap, 5, 17 + TotalRow + gap, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                ws.Cells[17 + TotalRow + gap, 5, 17 + TotalRow + gap, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                ws.Cells[17 + TotalRow + gap, 5, 17 + TotalRow + gap, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                ws.Cells[17 + TotalRow + gap, 5, 17 + TotalRow + gap, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                ws.Cells[17 + TotalRow + gap, 5].Value = "Total";                           
                                ws.Cells[17 + TotalRow + gap, 6].Value = data[k].quantity;
                                ws.Cells[17 + TotalRow + gap, 7].Value = data[k].ng_qty;

                                gtotal += Convert.ToInt32(ws.Cells[17 + TotalRow + gap, 6].Value);
                                gngtotal += Convert.ToInt32(ws.Cells[17 + TotalRow + gap, 8].Value) + Convert.ToInt32(ws.Cells[17 + TotalRow + gap, 9].Value) + Convert.ToInt32(ws.Cells[17 + TotalRow + gap, 10].Value) + Convert.ToInt32(ws.Cells[17 + TotalRow + gap, 11].Value) + Convert.ToInt32(ws.Cells[17 + TotalRow + gap, 12].Value) + Convert.ToInt32(ws.Cells[17 + TotalRow + gap, 14].Value);

                            }
                        }
                        else
                        {
                            if (temp != data[k - 1].abnormality)
                            {
                                perItemRow = 0;
                                gap += 2;

                                ws.Cells[(17 + TotalRow + gap), 2, (17 + TotalRow + gap), 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                ws.Cells[(17 + TotalRow + gap), 2, (17 + TotalRow + gap), 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                ws.Cells[(17 + TotalRow + gap), 2, (17 + TotalRow + gap), 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                ws.Cells[(17 + TotalRow + gap), 2, (17 + TotalRow + gap), 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                ws.Cells[(17 + TotalRow + gap), 2].Value = data[k].abnormality;
                                ws.Cells[(17 + TotalRow + gap), 3].Value = data[k].type;
                                ws.Cells[(17 + TotalRow + gap), 4].Value = data[k].lotno;
                                ws.Cells[(17 + TotalRow + gap), 5].Value = data[k].packkeddate;
                                ws.Cells[(17 + TotalRow + gap), 6].Value = data[k].quantity;
                                ws.Cells[(17 + TotalRow + gap), 7].Value = data[k].ng_qty;

                                TotalRow += 1;
                                perItemRow += 1;

                                if (perItemRow == data[k].rowspan)
                                {
                                    ws.Cells[(17 + TotalRow + gap), 5, (17 + TotalRow + gap), 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[(17 + TotalRow + gap), 5, (17 + TotalRow + gap), 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[(17 + TotalRow + gap), 5, (17 + TotalRow + gap), 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[(17 + TotalRow + gap), 5, (17 + TotalRow + gap), 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[17 + TotalRow + gap, 5].Value = "Total";
                                    //ws.Cells[17 + TotalRow + gap, 6].Value = "HIIIII2";
                                    ws.Cells[17 + TotalRow + gap, 6].Formula = "SUM(F" + (17 + TotalRow + gap - data[k].rowspan) + ":F" + (17 + TotalRow + gap - 1) + ")";
                                    ws.Cells[17 + TotalRow + gap, 7].Formula = "SUM(G" + (17 + TotalRow + gap - data[k].rowspan) + ":G" + (17 + TotalRow + gap - 1) + ")";
                                    ws.Cells[17 + TotalRow + gap, 6].Calculate();

                                    gtotal += Convert.ToInt32(ws.Cells[17 + TotalRow + gap, 6].Value);
                                    gngtotal += Convert.ToInt32(ws.Cells[17 + TotalRow + gap, 8].Value) + Convert.ToInt32(ws.Cells[17 + TotalRow + gap, 9].Value) + Convert.ToInt32(ws.Cells[17 + TotalRow + gap, 10].Value) + Convert.ToInt32(ws.Cells[17 + TotalRow + gap, 11].Value) + Convert.ToInt32(ws.Cells[17 + TotalRow + gap, 12].Value) + Convert.ToInt32(ws.Cells[17 + TotalRow + gap, 14].Value);
                                    gmabs += Convert.ToInt32(ws.Cells[17 + TotalRow + gap, 13].Value);

                                }
                            }
                            else
                            {
                                ws.Cells[(17 + TotalRow + gap), 2, (17 + TotalRow + gap), 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                ws.Cells[(17 + TotalRow + gap), 2, (17 + TotalRow + gap), 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                ws.Cells[(17 + TotalRow + gap), 2, (17 + TotalRow + gap), 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                ws.Cells[(17 + TotalRow + gap), 2, (17 + TotalRow + gap), 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                ws.Cells[(17 + TotalRow + gap), 2].Value = data[k].abnormality;
                                ws.Cells[(17 + TotalRow + gap), 3].Value = data[k].type;
                                ws.Cells[(17 + TotalRow + gap), 4].Value = data[k].lotno;
                                ws.Cells[(17 + TotalRow + gap), 5].Value = data[k].packkeddate;
                                ws.Cells[(17 + TotalRow + gap), 6].Value = data[k].quantity;
                                ws.Cells[(17 + TotalRow + gap), 7].Value = data[k].ng_qty;

                                TotalRow += 1;
                                perItemRow += 1;

                                if (perItemRow == data[k].rowspan)
                                {
                                    ws.Cells[(17 + TotalRow + gap), 5, (17 + TotalRow + gap), 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[(17 + TotalRow + gap), 5, (17 + TotalRow + gap), 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[(17 + TotalRow + gap), 5, (17 + TotalRow + gap), 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[(17 + TotalRow + gap), 5, (17 + TotalRow + gap), 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[17 + TotalRow + gap, 5].Value = "Total";
                                    ws.Cells[17 + TotalRow + gap, 6].Formula = "SUM(F" + (17 + TotalRow + gap - data[k].rowspan) + ":F" + (17 + TotalRow + gap - 1) + ")";
                                    ws.Cells[17 + TotalRow + gap, 7].Formula = "SUM(G" + (17 + TotalRow + gap - data[k].rowspan) + ":G" + (17 + TotalRow + gap - 1) + ")";
                                    ws.Cells[17 + TotalRow + gap, 6].Calculate();

                                    gtotal += Convert.ToInt32(ws.Cells[17 + TotalRow + gap, 6].Value);
                                    gngtotal += Convert.ToInt32(ws.Cells[17 + TotalRow + gap, 8].Value) + Convert.ToInt32(ws.Cells[17 + TotalRow + gap, 9].Value) + Convert.ToInt32(ws.Cells[17 + TotalRow + gap, 10].Value) + Convert.ToInt32(ws.Cells[17 + TotalRow + gap, 11].Value) + Convert.ToInt32(ws.Cells[17 + TotalRow + gap, 12].Value) + Convert.ToInt32(ws.Cells[17 + TotalRow + gap, 14].Value);
                                }
                            }
                        }
                        //END-Added by KL Ong 20221228
                    }

                    ws.Cells[17 + TotalRow + gap + 2, 5, 17 + TotalRow + gap + 2, 6].Style.Font.Bold = true;
                    ws.Cells[17 + TotalRow + gap + 2, 5, 17 + TotalRow + gap + 2 + 2, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells[17 + TotalRow + gap + 2, 6, 17 + TotalRow + gap + 2 + 2, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[17 + TotalRow + gap + 2, 5, 17 + TotalRow + gap + 2 + 2, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells[17 + TotalRow + gap + 2, 5, 17 + TotalRow + gap + 2 + 2, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[17 + TotalRow + gap + 2, 5, 17 + TotalRow + gap + 2 + 2, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells[17 + TotalRow + gap + 2, 5, 17 + TotalRow + gap + 2 + 2, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells[17 + TotalRow + gap + 2, 5].Value = "Grand Total";
                    ws.Cells[17 + TotalRow + gap + 2, 6].Value = gtotal;
                    ws.Cells[17 + TotalRow + gap + 2, 7].Value = gngtotal;
                    ws.Cells[17 + TotalRow + gap + 2 + 1, 5].Value = "ABS (CAP 0,1,2,3,4 & 6)";
                    ws.Cells[17 + TotalRow + gap + 2 + 1, 6].Value = gabs;
                    ws.Cells[17 + TotalRow + gap + 2 + 1, 7].Value = gngabs;
                    ws.Cells[17 + TotalRow + gap + 2 + 2, 5].Value = "MABS (CAP 5)";
                    ws.Cells[17 + TotalRow + gap + 2 + 2, 6].Value = gmabs;
                    ws.Cells[17 + TotalRow + gap + 2 + 2, 7].Value = gngmabs;

                    ws.Cells[17 + TotalRow + gap + 2 + 4, 2].Value = "CAP-7";
                    ws.Cells[17 + TotalRow + gap + 2 + 4, 2].Style.Font.Bold = true;
                    ws.Cells[17 + TotalRow + gap + 2 + 4, 2].Style.Font.UnderLine = true;

                    //ws.Cells[17 + TotalRow + gap + 2 + 8, 2, 17 + TotalRow + gap + 2 + 8, 6].Merge = true;
                    ws.Cells[17 + TotalRow + gap + 2 + 6, 2, 17 + TotalRow + gap + 2 + 6, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[17 + TotalRow + gap + 2 + 6, 2, 17 + TotalRow + gap + 2 + 6, 7].Style.Font.Bold = true;
                    ws.Cells[17 + TotalRow + gap + 2 + 6, 2, 17 + TotalRow + gap + 2 + 6, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells[17 + TotalRow + gap + 2 + 6, 2, 17 + TotalRow + gap + 2 + 6, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[17 + TotalRow + gap + 2 + 6, 2, 17 + TotalRow + gap + 2 + 6, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells[17 + TotalRow + gap + 2 + 6, 2, 17 + TotalRow + gap + 2 + 6, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    ws.Cells[17 + TotalRow + gap + 2 + 6, 2].Value = "Abnormality";
                    ws.Cells[17 + TotalRow + gap + 2 + 6, 3].Value = "Type";
                    ws.Cells[17 + TotalRow + gap + 2 + 6, 4].Value = "Lot No";
                    ws.Cells[17 + TotalRow + gap + 2 + 6, 5].Value = "Pack Date";
                    ws.Cells[17 + TotalRow + gap + 2 + 6, 6].Value = "Lot Qty (Kgs)";
                    ws.Cells[17 + TotalRow + gap + 2 + 6, 7].Value = "NG Qty (Kgs)";

                    for (int k = 0; k < data07.Count; k++)
                    {
                        var temp = "";
                        temp = data07[k].abnormality.ToString();

                        if (k == 0)
                        {
                            ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 2, 17 + TotalRow + gap + 2 + 6 + 1 + k, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 2, 17 + TotalRow + gap + 2 + 6 + 1 + k, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 2, 17 + TotalRow + gap + 2 + 6 + 1 + k, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 2, 17 + TotalRow + gap + 2 + 6 + 1 + k, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 2].Value = data07[k].abnormality;
                            ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 3].Value = data07[k].type;
                            ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 4].Value = data07[k].lotno;
                            ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 5].Value = data07[k].packkeddate;
                            ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 6].Value = data07[k].quantity;
                            ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 7].Value = data07[k].ng_qty;

                            TotalRow += 1;
                            perItemRow = 1;

                            if (perItemRow == data07[k].rowspan)
                            {
                                ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 5].Value = "Total";
                                //ws.Cells[17 + TotalRow + gap, 6].Value = "HIIIII3";
                                ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 6].Formula = "SUM(F" + (17 + TotalRow + gap + 2 + 6 + 1 + k - data07[k].rowspan) + ":F" + (17 + TotalRow + gap + 2 + 6 + 1 + k - 1) + ")";
                                ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 6].Calculate();
                                ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 7].Formula = "SUM(G" + (17 + TotalRow + gap + 2 + 6 + 1 + k - data07[k].rowspan) + ":G" + (17 + TotalRow + gap + 2 + 6 + 1 + k - 1) + ")";
                                ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 7].Calculate();
                                ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 5, 17 + TotalRow + gap + 2 + 6 + 1 + k, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 5, 17 + TotalRow + gap + 2 + 6 + 1 + k, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 5, 17 + TotalRow + gap + 2 + 6 + 1 + k, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 5, 17 + TotalRow + gap + 2 + 6 + 1 + k, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                gtotal07 += Convert.ToInt32(ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 6].Value);
                            }
                        }
                        else
                        {
                            if (temp != data07[k - 1].abnormality)
                            {
                                perItemRow = 0;
                                gap += 2;

                                ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 2, 17 + TotalRow + gap + 2 + 6 + 1 + k, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 2, 17 + TotalRow + gap + 2 + 6 + 1 + k, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 2, 17 + TotalRow + gap + 2 + 6 + 1 + k, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 2, 17 + TotalRow + gap + 2 + 6 + 1 + k, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 2].Value = data07[k].abnormality;
                                ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 3].Value = data07[k].type;
                                ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 4].Value = data07[k].lotno;
                                ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 5].Value = data07[k].packkeddate;
                                ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 6].Value = data07[k].quantity;
                                ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 7].Value = data07[k].ng_qty;

                                TotalRow += 1;
                                perItemRow += 1;

                                if (perItemRow == data07[k].rowspan)
                                {
                                    ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 5].Value = "Total";
                                    //ws.Cells[17 + TotalRow + gap, 6].Value = "HIIIII2";
                                    ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 6].Formula = "SUM(F" + (17 + TotalRow + gap + 2 + 6 + 1 + k - data07[k].rowspan) + ":F" + (17 + TotalRow + gap + 2 + 6 + 1 + k - 1) + ")";
                                    ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 6].Calculate();
                                    ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 7].Formula = "SUM(G" + (17 + TotalRow + gap + 2 + 6 + 1 + k - data07[k].rowspan) + ":G" + (17 + TotalRow + gap + 2 + 6 + 1 + k - 1) + ")";
                                    ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 7].Calculate();
                                    ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 5, 17 + TotalRow + gap + 2 + 6 + 1 + k, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 5, 17 + TotalRow + gap + 2 + 6 + 1 + k, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 5, 17 + TotalRow + gap + 2 + 6 + 1 + k, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 5, 17 + TotalRow + gap + 2 + 6 + 1 + k, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    gtotal07 += Convert.ToInt32(ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 6].Value);
                                }
                            }
                            else
                            {

                                ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 2, 17 + TotalRow + gap + 2 + 6 + 1 + k, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 2, 17 + TotalRow + gap + 2 + 6 + 1 + k, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 2, 17 + TotalRow + gap + 2 + 6 + 1 + k, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 2, 17 + TotalRow + gap + 2 + 6 + 1 + k, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 2].Value = data07[k].abnormality;
                                ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 3].Value = data07[k].type;
                                ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 4].Value = data07[k].lotno;
                                ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 5].Value = data07[k].packkeddate;
                                ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 6].Value = data07[k].quantity;
                                ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 7].Value = data07[k].ng_qty;

                                TotalRow += 1;
                                perItemRow += 1;

                                if (perItemRow == data07[k].rowspan)
                                {
                                    ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 5].Value = "Total";
                                    //ws.Cells[17 + TotalRow + gap, 6].Value = "HIIIII3";
                                    ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 6].Formula = "SUM(F" + (17 + TotalRow + gap + 2 + 6 + 1 + k - data07[k].rowspan) + ":F" + (17 + TotalRow + gap + 2 + 6 + 1 + k - 1) + ")";
                                    ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 6].Calculate();
                                    ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 7].Formula = "SUM(G" + (17 + TotalRow + gap + 2 + 6 + 1 + k - data07[k].rowspan) + ":G" + (17 + TotalRow + gap + 2 + 6 + 1 + k - 1) + ")";
                                    ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 7].Calculate();
                                    ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 5, 17 + TotalRow + gap + 2 + 6 + 1 + k, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 5, 17 + TotalRow + gap + 2 + 6 + 1 + k, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 5, 17 + TotalRow + gap + 2 + 6 + 1 + k, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 5, 17 + TotalRow + gap + 2 + 6 + 1 + k, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    gtotal07 += Convert.ToInt32(ws.Cells[17 + TotalRow + gap + 2 + 6 + 1 + k, 6].Value);
                                }
                            }
                        }
                        //END-Added by KL Ong 20221228
                    }

                    ws.Cells[17 + TotalRow + gap + 2 + 6 + 3, 5, 17 + TotalRow + gap + 2 + 6 + 3, 6].Style.Font.Bold = true;
                    ws.Cells[17 + TotalRow + gap + 2 + 6 + 3, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells[17 + TotalRow + gap + 2 + 6 + 3, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[17 + TotalRow + gap + 2 + 6 + 3, 5, 17 + TotalRow + gap + 2 + 6 + 3, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells[17 + TotalRow + gap + 2 + 6 + 3, 5, 17 + TotalRow + gap + 2 + 6 + 3, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[17 + TotalRow + gap + 2 + 6 + 3, 5, 17 + TotalRow + gap + 2 + 6 + 3, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells[17 + TotalRow + gap + 2 + 6 + 3, 5, 17 + TotalRow + gap + 2 + 6 + 3, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells[17 + TotalRow + gap + 2 + 6 + 3, 5].Value = "Grand Total";
                    ws.Cells[17 + TotalRow + gap + 2 + 6 + 3, 6].Value = gtotal07;

                    ws.Cells[ws.Dimension.Address].AutoFitColumns();
                }
                else if (i == 1)
                {
                    pck.Workbook.Worksheets.Add("CE (NG)");
                    ExcelWorksheet ws = pck.Workbook.Worksheets[i];

                    ws.Cells["B2:E2"].Merge = true;
                    ws.Cells["B2"].Value = "QAS MONTHLY NG & R GRADE REPORT";
                    ws.Cells["B2"].Style.Font.Bold = true;

                    //=========================================================== NG Grade table (header) =======================================================================================
                    ws.Cells["H15:P15"].Merge = true;
                    ws.Cells["H15:P15"].Value = "CE Line";
                    ws.Cells["H15:P15, B16:P16"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["H15:P15, B16:P16"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H15:P15, B16:P16"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H15:P15, B16:P16"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H15:P15, B16:P16"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["B16"].Value = "Abnormality";

                    ws.Cells["C16"].Value = "Type";
                    ws.Cells["D16"].Value = "Lot No";
                    ws.Cells["E16"].Value = "Pack Date";
                    ws.Cells["F16"].Value = "Lot Qty (Kgs)";
                    ws.Cells["G16"].Value = "NG Qty (Kgs)";
                    ws.Cells["H16"].Value = "20";
                    ws.Cells["I16"].Value = "21";
                    ws.Cells["J16"].Value = "22";
                    ws.Cells["K16"].Value = "23";
                    ws.Cells["L16"].Value = "24";
                    ws.Cells["M16"].Value = "25";
                    ws.Cells["N16"].Value = "26";
                    ws.Cells["O16"].Value = "27";
                    ws.Cells["P16"].Value = "28";

                    int TotalRow = 0;
                    int perItemRow = 0; // KL Ong 20221228
                    for (int k = 0; k < datace.Count; k++)
                    {
                        var temp = "";
                        temp = datace[k].abnormality.ToString();
                        if (k == 0)
                        {
                            ws.Cells[17, 2, 17, 16].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells[17, 2, 17, 16].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            ws.Cells[17, 2, 17, 16].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells[17, 2, 17, 16].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            ws.Cells[17, 2].Value = datace[k].abnormality;
                            ws.Cells[17, 3].Value = datace[k].type;
                            ws.Cells[17, 4].Value = datace[k].lotno;
                            ws.Cells[17, 5].Value = datace[k].packkeddate;
                            ws.Cells[17, 6].Value = datace[k].quantity;
                            ws.Cells[17, 7].Value = datace[k].ng_qty;

                            if (datace[k].prodline == "20")
                            {
                                ws.Cells[17, 8].Value = datace[k].ng_qty;
                            }
                            else if (datace[k].prodline == "21")
                            {
                                ws.Cells[17, 9].Value = datace[k].ng_qty;
                            }
                            else if (datace[k].prodline == "22")
                            {
                                ws.Cells[17, 10].Value = datace[k].ng_qty;
                            }
                            else if (datace[k].prodline == "23")
                            {
                                ws.Cells[17, 11].Value = datace[k].ng_qty;
                            }
                            else if (datace[k].prodline == "24")
                            {
                                ws.Cells[17, 12].Value = datace[k].ng_qty;
                            }
                            else if (datace[k].prodline == "25")
                            {
                                ws.Cells[17, 13].Value = datace[k].ng_qty;
                            }
                            else if (datace[k].prodline == "26")
                            {
                                ws.Cells[17, 14].Value = datace[k].ng_qty;
                            }
                            else if (datace[k].prodline == "27")
                            {
                                ws.Cells[17, 15].Value = datace[k].ng_qty;
                            }
                            else if (datace[k].prodline == "28")
                            {
                                ws.Cells[17, 16].Value = datace[k].ng_qty;
                            }

                            TotalRow += 1;
                            perItemRow += 1;

                            if (perItemRow == datace[k].rowspan)
                            {
                                perItemRow = 1;
                                ws.Cells[17 + TotalRow, 5].Value = "Total";
                                ws.Cells[17 + TotalRow, 6].Formula = "SUM(F" + (17 + TotalRow - datace.Count) + ":F" + (17 + TotalRow - 1) + ")";
                                ws.Cells[17 + TotalRow, 7].Formula = "SUM(G" + (17 + TotalRow - datace.Count) + ":G" + (17 + TotalRow - 1) + ")";
                                ws.Cells[(17 + TotalRow), 5, (17 + TotalRow), 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                ws.Cells[(17 + TotalRow), 5, (17 + TotalRow), 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                ws.Cells[(17 + TotalRow), 5, (17 + TotalRow), 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                ws.Cells[(17 + TotalRow), 5, (17 + TotalRow), 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            }
                            for (int cc = 8; cc <= 16; cc++)
                            {
                                if (Convert.ToInt32(ws.Cells[17 + TotalRow, cc].Value) == 0)
                                {
                                    ws.Cells[17 + TotalRow, cc].Value = "";
                                }
                            }
                        }
                        else
                        {
                            if (temp != datace[k - 1].abnormality)
                            {
                                perItemRow = 0;

                                ws.Cells[(17 + TotalRow), 2, (17 + TotalRow), 16].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                ws.Cells[(17 + TotalRow), 2, (17 + TotalRow), 16].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                ws.Cells[(17 + TotalRow), 2, (17 + TotalRow), 16].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                ws.Cells[(17 + TotalRow), 2, (17 + TotalRow), 16].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                ws.Cells[(17 + TotalRow), 2].Value = datace[k].abnormality;
                                ws.Cells[(17 + TotalRow), 3].Value = datace[k].type;
                                ws.Cells[(17 + TotalRow), 4].Value = datace[k].lotno;
                                ws.Cells[(17 + TotalRow), 5].Value = datace[k].packkeddate;
                                ws.Cells[(17 + TotalRow), 6].Value = datace[k].quantity;
                                ws.Cells[(17 + TotalRow), 7].Value = datace[k].ng_qty;

                                if (datace[k].prodline == "20")
                                {
                                    ws.Cells[17 + TotalRow, 8].Value = datace[k].ng_qty;
                                }
                                else if (datace[k].prodline == "21")
                                {
                                    ws.Cells[17 + TotalRow, 9].Value = datace[k].ng_qty;
                                }
                                else if (datace[k].prodline == "22")
                                {
                                    ws.Cells[17 + TotalRow, 10].Value = datace[k].ng_qty;
                                }
                                else if (datace[k].prodline == "23")
                                {
                                    ws.Cells[17 + TotalRow, 11].Value = datace[k].ng_qty;
                                }
                                else if (datace[k].prodline == "24")
                                {
                                    ws.Cells[17 + TotalRow, 12].Value = datace[k].ng_qty;
                                }
                                else if (datace[k].prodline == "25")
                                {
                                    ws.Cells[17 + TotalRow, 13].Value = datace[k].ng_qty;
                                }
                                else if (datace[k].prodline == "26")
                                {
                                    ws.Cells[17 + TotalRow, 14].Value = datace[k].ng_qty;
                                }
                                else if (datace[k].prodline == "27")
                                {
                                    ws.Cells[17 + TotalRow, 15].Value = datace[k].ng_qty;
                                }
                                else if (datace[k].prodline == "28")
                                {
                                    ws.Cells[17 + TotalRow, 16].Value = datace[k].ng_qty;
                                }

                                TotalRow += 1;
                                perItemRow += 1;

                                if (perItemRow == datace[k].rowspan)
                                {
                                    perItemRow = 1;
                                    ws.Cells[17 + TotalRow, 5].Value = "Total";
                                    ws.Cells[17 + TotalRow, 6].Formula = "SUM(F" + (17 + TotalRow - datace.Count) + ":F" + (17 + TotalRow - 1) + ")";
                                    ws.Cells[17 + TotalRow, 7].Formula = "SUM(G" + (17 + TotalRow - datace.Count) + ":G" + (17 + TotalRow - 1) + ")";
                                    ws.Cells[(17 + TotalRow), 5, (17 + TotalRow), 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[(17 + TotalRow), 5, (17 + TotalRow), 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[(17 + TotalRow), 5, (17 + TotalRow), 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[(17 + TotalRow), 5, (17 + TotalRow), 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                }
                                for (int cc = 8; cc <= 16; cc++)
                                {
                                    if (Convert.ToInt32(ws.Cells[17 + TotalRow, cc].Value) == 0)
                                    {
                                        ws.Cells[17 + TotalRow, cc].Value = "";
                                    }
                                }
                            }
                            else
                            {
                                ws.Cells[(17 + TotalRow), 2, (17 + TotalRow), 16].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                ws.Cells[(17 + TotalRow), 2, (17 + TotalRow), 16].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                ws.Cells[(17 + TotalRow), 2, (17 + TotalRow), 16].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                ws.Cells[(17 + TotalRow), 2, (17 + TotalRow), 16].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                ws.Cells[(17 + TotalRow), 2].Value = datace[k].abnormality;
                                ws.Cells[(17 + TotalRow), 3].Value = datace[k].type;
                                ws.Cells[(17 + TotalRow), 4].Value = datace[k].lotno;
                                ws.Cells[(17 + TotalRow), 5].Value = datace[k].packkeddate;
                                ws.Cells[(17 + TotalRow), 6].Value = datace[k].quantity;
                                ws.Cells[(17 + TotalRow), 7].Value = datace[k].ng_qty;

                                if (datace[k].prodline == "20")
                                {
                                    ws.Cells[17 + TotalRow, 8].Value = datace[k].ng_qty;
                                }
                                else if (datace[k].prodline == "21")
                                {
                                    ws.Cells[17 + TotalRow, 9].Value = datace[k].ng_qty;
                                }
                                else if (datace[k].prodline == "22")
                                {
                                    ws.Cells[17 + TotalRow, 10].Value = datace[k].ng_qty;
                                }
                                else if (datace[k].prodline == "23")
                                {
                                    ws.Cells[17 + TotalRow, 11].Value = datace[k].ng_qty;
                                }
                                else if (datace[k].prodline == "24")
                                {
                                    ws.Cells[17 + TotalRow, 12].Value = datace[k].ng_qty;
                                }
                                else if (datace[k].prodline == "25")
                                {
                                    ws.Cells[17 + TotalRow, 13].Value = datace[k].ng_qty;
                                }
                                else if (datace[k].prodline == "26")
                                {
                                    ws.Cells[17 + TotalRow, 14].Value = datace[k].ng_qty;
                                }
                                else if (datace[k].prodline == "27")
                                {
                                    ws.Cells[17 + TotalRow, 15].Value = datace[k].ng_qty;
                                }
                                else if (datace[k].prodline == "28")
                                {
                                    ws.Cells[17 + TotalRow, 16].Value = datace[k].ng_qty;
                                }

                                TotalRow += 1;
                                perItemRow += 1;

                                if (perItemRow == datace[k].rowspan)
                                {
                                    perItemRow = 1;
                                    ws.Cells[17 + TotalRow, 5].Value = "Total";
                                    ws.Cells[17 + TotalRow, 6].Formula = "SUM(F" + (17 + TotalRow - datace.Count) + ":F" + (17 + TotalRow - 1) + ")";
                                    ws.Cells[17 + TotalRow, 7].Formula = "SUM(G" + (17 + TotalRow - datace.Count) + ":G" + (17 + TotalRow - 1) + ")";
                                    ws.Cells[(17 + TotalRow), 5, (17 + TotalRow), 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[(17 + TotalRow), 5, (17 + TotalRow), 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[(17 + TotalRow), 5, (17 + TotalRow), 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[(17 + TotalRow), 5, (17 + TotalRow), 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                }
                            }
                        }
                        //END-Added by KL Ong 20221228
                    }

                    ws.Cells[ws.Dimension.Address].AutoFitColumns();

                    for (int cc = 13; cc >= 4; cc--)
                    {
                        ws.DeleteRow(cc);
                    }
                }
                else if (i == 2)
                {
                    pck.Workbook.Worksheets.Add("R GRADE");
                    ExcelWorksheet ws = pck.Workbook.Worksheets[i];
                    int gtrow = 1;
                    ws.Cells["B2"].Value = "QAS MONTHLY NG & R GRADE REPORT";
                    ws.Cells["B2"].Style.Font.Bold = true;
                    ws.Cells[5, 4, gtrow + 4, 4].Style.Numberformat.Format = "dd/mm/yyyy";
                    dt.TableName = "dt-" + rr.ToString();
                    rr++;
                    ws.Cells["B4"].LoadFromDataTable(dt, true, OfficeOpenXml.Table.TableStyles.Light11);

                    if (dt.Rows.Count > 0) //Added KL Ong on 20221228: add IF ELSE incase no data
                    {
                        gtrow = dt.Rows.Count;
                        ws.Cells[gtrow + 5, 4].Value = "Grand Total";
                        ws.Cells[gtrow + 5, 5].Formula = "SUM(E5:E" + (gtrow + 4) + ")";
                    }
                    else
                    {
                        ws.Cells["B4:G4"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells["B4:G4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkGray);
                        ws.Cells["B4:G4"].Style.Font.Color.SetColor(System.Drawing.Color.White);
                        ws.Cells["B4"].Value = "Product Type";
                        ws.Cells["C4"].Value = "Lot No";
                        ws.Cells["D4"].Value = "Date";
                        ws.Cells["E4"].Value = "Qty (KG)";
                        ws.Cells["F4"].Value = "Grade";
                        ws.Cells["G4"].Value = "Remark";

                        ws.Cells["D6"].Value = "Grand Total";
                        ws.Cells["E6"].Value = "0";
                    }
                    ws.Cells[ws.Dimension.Address].AutoFitColumns();
                }
                else
                {
                    pck.Workbook.Worksheets.Add("R GRADE (CAP 7)");
                    ExcelWorksheet ws = pck.Workbook.Worksheets[i];
                    int gtrow = 1;
                    ws.Cells["B2"].Value = "QAS MONTHLY NG & R GRADE REPORT";
                    ws.Cells["B2"].Style.Font.Bold = true;
                    ws.Cells[5, 4, gtrow + 4, 4].Style.Numberformat.Format = "dd/mm/yyyy";
                    //ws.Cells[4, row + 4].Formula = "=DATE(5,10,2021)";pl

                    if (dt2 != null)
                    {
                        if (dt2.Rows.Count > 0)
                        {
                            gtrow = dt2.Rows.Count;
                            dt2.TableName = "dt2-" + rr.ToString();
                            rr++;
                            ws.Cells["B4"].LoadFromDataTable(dt2, true, OfficeOpenXml.Table.TableStyles.Light11);
                            ws.Cells[gtrow + 5, 4].Value = "Grand Total";
                            ws.Cells[gtrow + 5, 5].Formula = "SUM(E5:E" + (gtrow + 4) + ")";
                        }
                        else
                        {
                            ws.Cells["B4:G4"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            ws.Cells["B4:G4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkGray);
                            ws.Cells["B4:G4"].Style.Font.Color.SetColor(System.Drawing.Color.White);
                            ws.Cells["B4:G4"].AutoFilter = true;
                            ws.Cells["B4"].Value = "Product Type";
                            ws.Cells["C4"].Value = "Lot No";
                            ws.Cells["D4"].Value = "Date";
                            ws.Cells["E4"].Value = "Qty (KG)";
                            ws.Cells["F4"].Value = "Grade";
                            ws.Cells["G4"].Value = "Remark";

                            ws.Cells["D6"].Value = "Grand Total";
                            ws.Cells["E6"].Value = "0";

                        }
                    }

                    ws.Cells[ws.Dimension.Address].AutoFitColumns();
                }

            }
            Response.Clear();
            Response.Charset = System.Text.UTF8Encoding.UTF8.WebName;
            Response.ContentEncoding = System.Text.UTF8Encoding.UTF8;
            Response.AddHeader("content-disposition", "attachment;  filename=MonthlyNG" + DateTime.Now + ".xlsx");
            Response.ContentType = "application/text";
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
            
            return View(model);

        }

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

    public class ITextEvents : PdfPageEventHelper
    {
        // This is the contentbyte object of the writer
        PdfContentByte cb;

        // this is the BaseFont we are going to use for the header / footer
        BaseFont bf = null;
        float fontSize;
        float rightSpacing = 10;

        // This keeps track of the creation time
        string author;
        DateTime PrintTime = DateTime.Now;

        public ITextEvents(string author)
        {
            this.author = author;
        }

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            base.OnOpenDocument(writer, document);

            // Set up details
            PrintTime = DateTime.Now;
            bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            fontSize = 8.0f;
            cb = writer.DirectContent;
        }

        public override void OnEndPage(PdfWriter writer, Document doc)
        {
            PdfContentByte content = writer.DirectContent;
            Rectangle rectangle = new Rectangle(doc.PageSize);
            rectangle.Left += 10; 
            rectangle.Right -= 10;
            rectangle.Top -= 10;
            rectangle.Bottom += 10; 
            content.SetColorStroke(BaseColor.BLACK);
            content.Rectangle(rectangle.Left, rectangle.Bottom, rectangle.Width, rectangle.Height);
            content.Stroke();

            BaseFont footer = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            content.SetFontAndSize(footer, 8);

            string footerText = "";// "This is a computer generated report and no signature is required";

            float textWidth = footer.GetWidthPoint(footerText, 8);
            float xPosition = (doc.PageSize.Width - textWidth) / 2;
            float yPosition = doc.BottomMargin - 8; 

            content.BeginText();
            content.ShowTextAligned(Element.ALIGN_LEFT, footerText, xPosition, yPosition, 0);
            content.EndText();

            BaseFont pagenum = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            content.SetFontAndSize(pagenum, 10);

            int currentPageNumber = writer.PageNumber;
            string pageNumberText = $"Page: {currentPageNumber}";

            float pageNumberWidth = pagenum.GetWidthPoint(pageNumberText, 10);
            float pageNumberX = doc.PageSize.Width - doc.RightMargin - pageNumberWidth;
            float pageNumberY = doc.BottomMargin - 8;

            content.BeginText();
            content.ShowTextAligned(Element.ALIGN_LEFT, pageNumberText, pageNumberX, pageNumberY, 0);
            content.EndText();
        }


        //public override void OnEndPage(PdfWriter writer, Document document)
        //{
        //    base.OnEndPage(writer, document);

        //    cb.BeginText();
        //    cb.SetFontAndSize(bf, fontSize);
        //    cb.SetTextMatrix(document.PageSize.Width - rightSpacing, document.PageSize.GetTop(15));
        //    cb.ShowText("Printed by :" + author);
        //    cb.EndText();

        //    cb.BeginText();
        //    cb.SetFontAndSize(bf, fontSize);
        //    cb.SetTextMatrix(document.PageSize.Width - rightSpacing, document.PageSize.GetTop(30));
        //    cb.ShowText("Print Date :" + PrintTime.ToString("yyyy/MM/dd"));
        //    cb.EndText();
        //}

    }
}