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
using System.IO;
using OfficeOpenXml;

namespace TPM_QAS.Controllers
{
    public class DATA_UploadController : Controller
    {
        DB dbmain = new DB();
        DATA_UPLOAD_DAL dbdal = new DATA_UPLOAD_DAL();
        COA_MM_MATERIAL_DAL dbmat = new COA_MM_MATERIAL_DAL();


        [SessionExpire]
        public ActionResult DATA_UPLOAD_GET_SUPPLIER_NAME(string Deleted = "0")
        {
            string data = "";
            string path = "C:\\Users\\solehah\\OneDrive - Max System\\Desktop\\TPM COA 2025\\lastestrawdata";
            // check if directory exist
            DataTable dtSupplier = new DataTable();
            DataRow dr;
            dtSupplier.Columns.Add("SUPPLIER_NAME");
            dtSupplier.Columns.Add("FILENAME");

            DataTable dtMaterial = new DataTable();

            if (Directory.Exists(path))
            {
                ViewBag.Pathexist = "Path exist : " + path;
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                foreach (FileInfo file in directoryInfo.GetFiles("*.xlsx"))
                {
                    string filename = file.Name;
                    string filenouse = filename.Substring(0, 1);

                    try
                    {
                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                        using (ExcelPackage package = new ExcelPackage(file))
                        {
                            foreach (ExcelWorksheet worksheet in package.Workbook.Worksheets)
                            {
                                data = data + "Worksheet: " + worksheet.Name;
                                if (worksheet != null)
                                {
                                    int rowCount = worksheet.Dimension.Rows;
                                    int columnCount = worksheet.Dimension.Columns;
                                    string curritem = "";
                                    for (int row = 1; row <= rowCount; row++) // Excel rows are 1-based
                                    {
                                        object newitem = worksheet.Cells[row, 1].Value;
                                        if (newitem != null)
                                        {
                                            if (newitem.ToString() != "")
                                            {
                                                curritem = newitem.ToString();
                                                //currmaterial = worksheet.Cells[row, 2].Value.ToString();

                                                for (int col = 5; col <= columnCount; col++) // Excel columns are 1-based
                                                {
                                                    if (worksheet.Cells[row + 1, col].Value != null)
                                                    {
                                                        string suppliername = worksheet.Cells[row + 1, col].Value.ToString().Trim();
                                                        if (worksheet.Cells[row + 2, col].Value != null)
                                                        {
                                                            suppliername = suppliername + " " + worksheet.Cells[row + 2, col].Value.ToString().Trim();
                                                        }
                                                        if (suppliername != "")
                                                        {
                                                            dr = dtSupplier.NewRow();
                                                            dr["SUPPLIER_NAME"] = suppliername;
                                                            dr["FILENAME"] = filename;
                                                            dtSupplier.Rows.Add(dr);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                        ViewBag.Error = ex.Message + " at " + filename;
                        break;
                    }
                }

            }

            DataUploadModel model = new DataUploadModel();

            List<SupplierList> supplierNames = dtSupplier.AsEnumerable()
                .GroupBy(row => new { SupplierName = row.Field<string>("SUPPLIER_NAME"), FileName = row.Field<string>("FILENAME") })
                .Select(group => group.First()) // Take the first row from each group (distinct combination)
                .OrderBy(row => row.Field<string>("SUPPLIER_NAME"))
                .Select(row => new SupplierList
                {
                    SUPPLIER_NAME = row.Field<string>("SUPPLIER_NAME"),
                    FILENAME = row.Field<string>("FILENAME")
                })
                .ToList();

            model.SupplierList = supplierNames;
            return View(model);
        }

        public async Task<ActionResult> DATA_UPLOAD()
        {

            string data = "";
            string path = "C:\\Users\\solehah\\OneDrive - Max System\\Desktop\\TPM COA 2025\\lastestrawdata";
            // check if directory exist

            DataTable dtSupplier = new DataTable();
            DataRow dr;
            dtSupplier.Columns.Add("SUPPLIER_NAME");
            dtSupplier.Columns.Add("FILENAME");

            DataTable dtMaterial = new DataTable();
            dtMaterial.Columns.Add("EXCEL_ID");
            dtMaterial.Columns.Add("EXCEL_MATERIAL_NAME");
            dtMaterial.Columns.Add("EXCEL_MATERIAL_ABBR");
            dtMaterial.Columns.Add("FILENAME");
            dtMaterial.Columns.Add("ORA_MATERIAL_NAME");
            dtMaterial.Columns.Add("ORA_MATERIAL_ABBR");
            dtMaterial.Columns.Add("ORA_TPM_CODE");
            dtMaterial.Columns.Add("ORA_MATERIAL_CODE");

            List<MaterialList> MaterialListExcel = new List<MaterialList>();

            DataTable dtora = await dbdal.getORAMaterialList();

            if (Directory.Exists(path))
            {
                ViewBag.Pathexist = "Path exist : " + path;
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                int excelno = 0;
                foreach (FileInfo file in directoryInfo.GetFiles("*.xlsx"))
                {
                    string filename = file.Name;
                    string filenouse = filename.Substring(0, 1);

                    try
                    {
                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                        using (ExcelPackage package = new ExcelPackage(file))
                        {
                            foreach (ExcelWorksheet worksheet in package.Workbook.Worksheets)
                            {
                                data = data + "Worksheet: " + worksheet.Name;
                                if (worksheet != null)
                                {
                                    int rowCount = worksheet.Dimension.Rows;
                                    int columnCount = worksheet.Dimension.Columns;
                                    string curritem = "";
                                    string currmaterial = "";
                                    string currmatabbr = "";
                                    for (int row = 1; row <= rowCount; row++) // Excel rows are 1-based
                                    {
                                        object newitem = worksheet.Cells[row, 1].Value;
                                        if (newitem != null)
                                        {
                                            if (newitem.ToString() != "")
                                            {
                                                curritem = newitem.ToString();
                                                //---------supplier part---
                                                for (int col = 5; col <= columnCount; col++) // Excel columns are 1-based
                                                {
                                                    if (worksheet.Cells[row + 1, col].Value != null)
                                                    {
                                                        string suppliername = worksheet.Cells[row + 1, col].Value.ToString().Trim();
                                                        if (worksheet.Cells[row + 2, col].Value != null)
                                                        {
                                                            suppliername = suppliername + " " + worksheet.Cells[row + 2, col].Value.ToString().Trim();
                                                        }
                                                        if (suppliername != "")
                                                        {
                                                            dr = dtSupplier.NewRow();
                                                            dr["SUPPLIER_NAME"] = suppliername;
                                                            dr["FILENAME"] = filename;
                                                            dtSupplier.Rows.Add(dr);
                                                        }
                                                    }
                                                }
                                                //supplier part end
                                                if (worksheet.Cells[row, 2].Value != null)
                                                {
                                                    currmaterial = worksheet.Cells[row, 2].Value.ToString();
                                                    if (currmaterial.Contains("Material Name"))
                                                    {
                                                        currmaterial = currmaterial.Replace("Material Name", "");
                                                        currmaterial = currmaterial.Replace(":", "").Trim();

                                                        if (currmaterial != "")
                                                        {
                                                            currmatabbr = worksheet.Cells[row + 1, 2].Value.ToString();
                                                            if (currmatabbr.Contains("Abbreviation"))
                                                            {
                                                                currmatabbr = currmatabbr.Replace("Abbreviation", "");
                                                                currmatabbr = currmatabbr.Replace(":", "").Trim();
                                                            }

                                                            //string oramatname = "";
                                                            //string oramatabbr = "";
                                                            //string oramatcode = "";
                                                            //string oratpmcode = "";
                                                            //DataRow[] foundabbr;
                                                            //foundabbr = dtora.Select("MATERIAL_ABBR = '"+ currmatabbr+"'");
                                                            ////exactly the abbreviaion found
                                                            //if (foundabbr.Length > 0)
                                                            //{
                                                            //    for (int x = 0; x < foundabbr.Length; x++) // Excel columns are 1-based
                                                            //    {
                                                            //        oramatname = oramatname + "," + foundabbr[x]["MATERIAL_NAME"].ToString();
                                                            //        oramatabbr = oramatabbr + "," + foundabbr[x]["MATERIAL_ABBR"].ToString();
                                                            //        oramatcode = oramatcode + "," + foundabbr[x]["MATERIAL_CODE"].ToString();
                                                            //        oratpmcode = oratpmcode + "," + foundabbr[x]["TPM_CODE"].ToString();                                                                }
                                                            //} else
                                                            //{
                                                            //    DataRow[] foundmat;
                                                            //    foundmat = dtora.Select("MATERIAL_NAME LIKE '%" + currmaterial + "%'");
                                                            //    //find if match like material name
                                                            //    if (foundmat.Length > 0)
                                                            //    {
                                                            //        for (int x = 0; x < foundmat.Length; x++) // Excel columns are 1-based
                                                            //        {
                                                            //            oramatname = oramatname + "," + foundmat[x]["MATERIAL_NAME"].ToString();
                                                            //            oramatabbr = oramatabbr + "," + foundmat[x]["MATERIAL_ABBR"].ToString();
                                                            //            oramatcode = oramatcode + "," + foundmat[x]["MATERIAL_CODE"].ToString();
                                                            //            oratpmcode = oratpmcode + "," + foundmat[x]["TPM_CODE"].ToString();
                                                            //        }
                                                            //    } else
                                                            //    { //if xmatxh lagsung, like dengan abbreviation
                                                            //        DataRow[] foundlikeabbr;
                                                            //        foundlikeabbr = dtora.Select("MATERIAL_ABBR like '%" + currmatabbr + "%'");
                                                            //        if (foundlikeabbr.Length > 0)
                                                            //        {
                                                            //            for (int x = 0; x < foundlikeabbr.Length; x++) // Excel columns are 1-based
                                                            //            {
                                                            //                oramatname = oramatname + "," + foundlikeabbr[x]["MATERIAL_NAME"].ToString();
                                                            //                oramatabbr = oramatabbr + "," + foundlikeabbr[x]["MATERIAL_ABBR"].ToString();
                                                            //                oramatcode = oramatcode + "," + foundlikeabbr[x]["MATERIAL_CODE"].ToString();
                                                            //                oratpmcode = oratpmcode + "," + foundlikeabbr[x]["TPM_CODE"].ToString();
                                                            //            }
                                                            //        }
                                                            //    }
                                                            //}
                                                            excelno = excelno + 1;
                                                            MaterialListExcel.Add(new MaterialList
                                                            {
                                                                EXCEL_ID = Convert.ToInt32(excelno), //old SP
                                                                EXCEL_MATERIAL_NAME = currmaterial,
                                                                EXCEL_MATERIAL_ABBR = currmatabbr,
                                                                FILENAME = filename,
                                                                //ORA_MATERIAL_NAME = oramatname,
                                                                //ORA_MATERIAL_ABBR = oramatabbr,
                                                                //ORA_MATERIAL_CODE = oramatcode,
                                                                //ORA_TPM_CODE = oratpmcode

                                                            });
                                                            //dr = dtMaterial.NewRow();
                                                            //dr["EXCEL_ID"] = excelno;
                                                            //dr["EXCEL_MATERIAL_NAME"] = currmaterial;
                                                            //dr["EXCEL_MATERIAL_ABBR"] = currmatabbr;
                                                            //dr["FILENAME"] = filename;
                                                            //dtMaterial.Rows.Add(dr);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                        ViewBag.Error = ex.Message + " at " + filename;
                        break;
                    }
                }

            }

            DataUploadModel model = new DataUploadModel();
            List<OraMaterialList> OraList = await dbmat.getMaterialListFrORAICCP();

            List<SupplierList> supplierNames = dtSupplier.AsEnumerable()
                .GroupBy(row => new { SupplierName = row.Field<string>("SUPPLIER_NAME"), FileName = row.Field<string>("FILENAME") })
                .Select(group => group.First()) // Take the first row from each group (distinct combination)
                .OrderBy(row => row.Field<string>("SUPPLIER_NAME"))
                .Select(row => new SupplierList
                {
                    SUPPLIER_NAME = row.Field<string>("SUPPLIER_NAME"),
                    FILENAME = row.Field<string>("FILENAME")
                })
                .ToList();

            model.SupplierList = supplierNames;
            model.MaterialList = MaterialListExcel;
            model.OraMaterialList2 = OraList;

            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        public ActionResult SaveMaterial(List<MaterialList> materialModel)
        {
            bool success = true;
            string message = "";

            if (ModelState.IsValid)
            {

                if (success)
                {
                    //// insert h
                    //string result1 = await dbdal.COAMM_MATERIAL_H_Maint(m);
                    //if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    //{
                    //    success = false;
                    //    message += "Error in saving data : " + result1;
                    //}
                    //else
                    //{
                    //    string resultd1 = "";
                    //    m.MM_MATERIAL_H_ID = Convert.ToInt32(result1);
                    //    await COA_MM_MATERIAL_D_MAINT(m, TPMModel, SUPPModel, resultd1);

                    //    if (resultd1 != "")
                    //    {
                    //        success = false;
                    //        message += "Error in saving data : " + resultd1;
                    //    }
                    //}
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

        /// <summary>
        /// Intialize directory
        /// </summary>
        /// <param name="path"></param>
        /// <param name="destination"></param>
        /// <param name="backup"></param>
        public void DirectoryInitializer(string path, out string destination, out string backup)
        {
            destination = "";
            backup = "";

            // check if directory exist
            if (Directory.Exists(path))
            {
                // Create Directory Info and path string
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                destination = path + "\\Run";
                backup = path + "\\Backup";

                // Check if Run and Backup directory exist. Create them if not exist
                if (Directory.Exists(destination) == false) { Directory.CreateDirectory(destination); }
                if (Directory.Exists(backup) == false) { Directory.CreateDirectory(backup); }

                try
                {
                    foreach (FileInfo file in directoryInfo.GetFiles("*.csv"))
                    {
                        file.MoveTo(Path.Combine(destination, file.Name));
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
            }
        }

        /// <summary>
        /// Finalized backup files
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="backup"></param>
        /// <param name="file"></param>
        public void DirectoryFinalizer(string destination, string backup, FileInfo file)
        {
            // Create Path
            string savePath = Path.Combine(backup, file.Name);
            string deletePath = Path.Combine(destination, file.Name);

            // Move to backup location if not exist, else delete current file
            if (System.IO.File.Exists(savePath) == false)
            {
                file.MoveTo(savePath);
            }
            else
            {
                file.Delete();
            }
        }

        /// <summary>
        /// Used to read excel file and get row and column data
        /// </summary>
        /// <param name="dataRow">Data Row from data table in monthly data</param>
        /// <param name="excelFile">The excel file to read</param>
        /// <param name="destination">Path Location of Excel File</param>
        /// <param name="rowData">Output data of rows (Values)</param>
        /// <param name="colData">Output data of columns (Column Names)</param>
        public void GetRowAndColumnData(DataRow dataRow, FileInfo excelFile, string destination, out List<string> rowData, out string colData)
        {
            // Set default value as empty
            string rowD = "";
            rowData = new List<string>();
            colData = "";
            string prop = dataRow["PROPERTIES"].ToString();

            try
            {
                // Intialize data
                string fileName = excelFile.Name;
                string cellRow = "";
                List<List<string>> stringlistlist = new List<List<string>>();
                StreamReader reader = new StreamReader(destination + "\\" + fileName);

                // Loop reader and add to text
                while (reader.Peek() != -1)
                {
                    List<string> cellList = new List<string>();
                    cellRow = reader.ReadLine();
                    //Split cell
                    cellList = cellRow.Split(',').ToList();
                    if (prop == "YI")
                    {
                        if (cellList[1] != "" && cellList[2] != "")
                        {
                            stringlistlist.Add(cellList);
                        }
                    }
                    else stringlistlist.Add(cellList);
                }
                reader.Close();

                // Check if more than 1 cell
                if (cellRow.Contains(",") == false)
                {
                    // If less than one cell, skip process
                    return;
                }

                for (int j = 0; j < stringlistlist.Count(); j++)
                {
                    // Loop columns in dataRow
                    for (int i = 0; i < dataRow.ItemArray.Length; i++)
                    {
                        // Check if column containts the word column
                        if (dataRow[i].ToString().Contains("Column"))
                        {
                            // Get column number - from reading
                            int colNum = int.Parse(dataRow[i].ToString().Substring(6));

                            //Nisa - minus 1 since system start from 0
                            colNum = colNum - 1;

                            // Loop cellList and add to data if number is same

                            for (int k = 0; k < stringlistlist[j].Count(); k++) //KL Ong 20220215 K change to j
                            {
                                if (colNum == k)
                                {
                                    if (j == 0)
                                    {
                                        colData += dataRow.Table.Columns[i].ColumnName + ",";
                                    }

                                    rowD += "'" + stringlistlist[j][k] + "',";
                                }
                            }

                        }
                    }
                    rowData.Add(rowD.Remove(rowD.Length - 1, 1));
                    rowD = "";
                }

                colData = colData.Remove(colData.Length - 1, 1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void GetRowAndColumnDataSch(DataRow dataRow, FileInfo excelFile, string destination, out List<string> rowData, out string colData)
        {
            // Set default value as empty
            string rowD = "";
            rowData = new List<string>();
            colData = "";
            string prop = dataRow["PROPERTIES"].ToString();

            try
            {
                // Intialize data
                string fileName = excelFile.Name;
                string cellRow = "";
                List<List<string>> stringlistlist = new List<List<string>>();
                StreamReader reader = new StreamReader(destination + "\\" + fileName);

                // Loop reader and add to text
                while (reader.Peek() != -1)
                {
                    List<string> cellList = new List<string>();
                    cellRow = reader.ReadLine();
                    //Split cell
                    cellList = cellRow.Split(',').ToList();
                    if (prop == "YI")
                    {
                        if(cellList.Count > 1)
                        {
                            if (cellList[1] != "" && cellList[2] != "")
                            {
                                stringlistlist.Add(cellList);
                            }
                        }
                        else
                        {
                            //nisa - add logic for exclude input file 
                            break;
                            //reader.Close();
                        }

                    }
                    else stringlistlist.Add(cellList);
                }
                reader.Close();

                // Check if more than 1 cell
                if (cellRow.Contains(",") == false)
                {
                    // If less than one cell, skip process
                    return;
                }

                for (int j = 0; j < stringlistlist.Count(); j++)
                {
                    // Loop columns in dataRow
                    for (int i = 0; i < dataRow.ItemArray.Length; i++)
                    {
                        // Check if column containts the word column
                        if (dataRow[i].ToString().Contains("Column"))
                        {
                            // Get column number - from reading
                            int colNum = int.Parse(dataRow[i].ToString().Substring(6));

                            // Loop cellList and add to data if number is same

                            for (int k = 0; k < stringlistlist[j].Count(); k++) //KL Ong 20220215 K change to j
                            {
                                if (colNum == k)
                                {
                                    if (j == 0)
                                    {
                                        colData += dataRow.Table.Columns[i].ColumnName + ",";
                                    }

                                    rowD += "'" + stringlistlist[j][k] + "',";
                                }
                            }

                        }
                    }
                    rowData.Add(rowD.Remove(rowD.Length - 1, 1));
                    rowD = "";
                }

                colData = colData.Remove(colData.Length - 1, 1);
            }
            catch (Exception ex)
            {
                if(ex.Message.Contains("StartIndex cannot be less than zero"))
                {
                    Console.WriteLine(ex.Message);
                }
                else
                {
                    Console.WriteLine(ex.Message);
                }
                
            }
        }

    }
}