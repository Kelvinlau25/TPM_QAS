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
using System.IO.Compression;
using System.IO;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Globalization;

namespace TPM_QAS.Controllers
{
    public class FILE_ATTACHMENTController : Controller
    {
        DB dbmain = new DB();
        FILE_ATTACHMENT_DAL dbdal = new FILE_ATTACHMENT_DAL();
        AzureBlobHelper blob = new AzureBlobHelper();
        CommonFunction common = new CommonFunction();

        #region LIST

        [SessionExpire]
        public async Task<ActionResult> FILE_ATTACHMENT_LST(string Deleted = "0")
        {
            TempData["STATUS_DDL"] = await dbmain.PSP_COMMON_DDL("CMN_LST_STATUS", "", "", "", "", "", "");

            CommonFunction common = new CommonFunction();
            var param = new { Table = "PVIEW_FILE_ATT", TableID = "", Search = "", Value = "", SortField = "MODULE_ID", Direction = "1", FrmRowno = "1", ToRowno = "500000", Deleted = Deleted };
            List<FileAttModel> model = await common.PSP_COMMON_DAPPER<FileAttModel>("PSP_COMMON_LIST", CommandType.StoredProcedure, param) ?? new List<FileAttModel>();

            ViewBag.Deleted = Deleted;
            return View(model);
        }

        [SessionExpire]
        public async Task<ActionResult> FILE_ATTACHMENT_LST2(string id, string Deleted = "0")
        {
            if (Deleted == "1")
            {
                TempData["STATUS_DDL"] = await dbmain.PSP_COMMON_DDL("CMN_LST_STATUS", "", "", "", "", "", "");
            }
            else
            {
                TempData["STATUS_DDL"] = await dbmain.PSP_COMMON_DDL("CMN_LST_STATUS2", "", "", "", "", "", "");
            }
            var model = new List<FileAttLstModel>();
            List<FileAttLstModel> listItemsAdd = new List<FileAttLstModel>();

            string folder = id;
            if (id == "INT_IDS" || id == "EXT_IDS")
            {
                if(id == "INT_IDS")
                {
                    folder = "Internal IDS";
                }
                else if(id == "EXT_IDS")
                {
                    folder = "External IDS";
                }

                var groupdate = await blob.GetFolderAndFiles(folder);
                if (groupdate != null && groupdate.Count > 0)
                {
                    foreach (var file in groupdate)
                    {
                        FileAttLstModel infoObjAdd = new FileAttLstModel();

                        infoObjAdd.MODULE_TYPE = id;
                        infoObjAdd.GROUP_DATE = file.Key.ToString();

                        var lotNos = file.Value
                                .Select(f =>
                                {
                                    string nameWithoutExt = Path.GetFileNameWithoutExtension(f);
                                    string[] parts = nameWithoutExt.Split('_');
                                    return parts.Length > 1 ? parts[0] : string.Empty;
                                })
                                .Where(x => !string.IsNullOrEmpty(x))
                                .Distinct();

                        infoObjAdd.LOT_NO = string.Join(", ", lotNos);

                        infoObjAdd.REC_TYPE_DESC = "New";

                        if (infoObjAdd.GROUP_DATE != null)
                        {
                            // only show file that exist
                            listItemsAdd.Add(infoObjAdd);
                        }
                    }
                }
            }
            else if (id == "COA")
            {
                folder = "COA Document";
                var groupdate = await blob.GetCOAFilesGroupedByDate(folder);

                if (groupdate != null && groupdate.Count > 0)
                {
                    foreach (var file in groupdate)
                    {
                        FileAttLstModel infoObjAdd = new FileAttLstModel();

                        infoObjAdd.MODULE_TYPE = id;
                        infoObjAdd.GROUP_DATE = file.Key.ToString();

                        var lotNos = file.Value
                                .Select(f =>
                                {
                                    string nameWithoutExt = Path.GetFileNameWithoutExtension(f.FileName);
                                    string[] parts = nameWithoutExt.Split('_');
                                    return parts.Length > 1 ? parts[1] : string.Empty;
                                })
                                .Where(x => !string.IsNullOrEmpty(x))
                                .Distinct();

                        infoObjAdd.LOT_NO = string.Join(", ", lotNos);

                        infoObjAdd.REC_TYPE_DESC = "New";

                        if (infoObjAdd.GROUP_DATE != null)
                        {
                            // only show file that exist
                            listItemsAdd.Add(infoObjAdd);
                        }
                    }
                }
            }

            model = listItemsAdd;
            ViewBag.ModType = id;
            ViewBag.Folder = folder;
            ViewBag.Deleted = Deleted;
            return View(model);
        }

        [SessionExpire]
        public async Task<ActionResult> FILE_ATTACHMENT_LST3(string id, string seldate, string Deleted = "0")
        {
            if (Deleted == "1")
            {
                TempData["STATUS_DDL"] = await dbmain.PSP_COMMON_DDL("CMN_LST_STATUS", "", "", "", "", "", "");
            }
            else
            {
                TempData["STATUS_DDL"] = await dbmain.PSP_COMMON_DDL("CMN_LST_STATUS2", "", "", "", "", "", "");
            }
            var model = new List<FileAttLstModel>();
            List<FileAttLstModel> listItemsAdd = new List<FileAttLstModel>();

            List<(string FileName, string FileUrl)> folderFiles = new List<(string FileName, string FileUrl)>();

            string folder = id;
            if(id == "INT_IDS" || id == "EXT_IDS")
            {
                if (id == "INT_IDS")
                {
                    folder = "Internal IDS";
                }
                else if (id == "EXT_IDS")
                {
                    folder = "External IDS";
                }

                folderFiles = await blob.GetFilesInSubfolder(seldate, folder);
            }
            else if(id == "COA")
            {
                folder = "COA Document";
                folderFiles = await blob.GetCOAFileUrlsByDate(seldate, folder);
            }
             
            if (folderFiles != null && folderFiles.Count > 0)
            {
                foreach (var file in folderFiles)
                {
                    FileAttLstModel infoObjAdd = new FileAttLstModel();

                    infoObjAdd.MODULE_TYPE = id;
                    infoObjAdd.GROUP_DATE = seldate;
                    infoObjAdd.FILE_ATTACHMENT = file.FileName.ToString();
                    infoObjAdd.FILE_ATTACHMENT_PATH = file.FileUrl.ToString();
                    infoObjAdd.REC_TYPE_DESC = "New";

                    if (id == "INT_IDS" || id == "EXT_IDS")
                    {
                        infoObjAdd.LOT_NO = Path.GetFileNameWithoutExtension(infoObjAdd.FILE_ATTACHMENT).Split('_')[0].ToString();
                    }
                    else if (id == "COA")
                    {
                        infoObjAdd.LOT_NO = Path.GetFileNameWithoutExtension(infoObjAdd.FILE_ATTACHMENT).Split('_')[1].ToString();
                    }

                    if (infoObjAdd.FILE_ATTACHMENT_PATH != null && infoObjAdd.FILE_ATTACHMENT_PATH != "")
                    {
                        // only show file that exist
                        listItemsAdd.Add(infoObjAdd);
                    }
                }
            }

            if (Deleted == "1")
            {
                DataTable dtldel = await dbdal.getFileAttDel_Sel(id);
                if (dtldel != null && dtldel.Rows.Count > 0)
                {
                    for (int i = 0; i < dtldel.Rows.Count; i++)
                    {
                        FileAttLstModel infoObjAdd = new FileAttLstModel();

                        infoObjAdd.MODULE_TYPE = id;
                        infoObjAdd.FILE_ATTACHMENT = dtldel.Rows[i]["FILE_ATTACHMENT"] != DBNull.Value ? dtldel.Rows[i]["FILE_ATTACHMENT"].ToString() : "";
                        infoObjAdd.LOT_NO = dtldel.Rows[i]["LOT_NO"] != DBNull.Value ? dtldel.Rows[i]["LOT_NO"].ToString() : "";
                        infoObjAdd.FILE_ATTACHMENT_PATH = "DELETE_FILE";
                        infoObjAdd.RECORD_TYP = dtldel.Rows[i]["RECORD_TYP"] != DBNull.Value ? dtldel.Rows[i]["RECORD_TYP"].ToString() : "";
                        infoObjAdd.REC_TYPE_DESC = dtldel.Rows[i]["REC_TYPE_DESC"] != DBNull.Value ? dtldel.Rows[i]["REC_TYPE_DESC"].ToString() : "";

                        listItemsAdd.Add(infoObjAdd);

                    }
                }
            }

            listItemsAdd = listItemsAdd.OrderBy(x => x.LOT_NO).ToList();

            model = listItemsAdd;
            ViewBag.ModType = id;
            ViewBag.Folder = folder;
            ViewBag.SelDate = seldate;
            ViewBag.Deleted = Deleted;
            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> deleteFileAtt(List<FileAttLstModel> lstid)
        {
            bool success = true;
            string message = "";

            string modtype = "INT_IDS";
            string seldate = "";

            if (lstid.Count < 1)
            {
                success = false;
                message = "Please select a record first to delete.";
            }

            if (success && lstid.Count > 0)
            {
                modtype = lstid[0].MODULE_TYPE != null ? lstid[0].MODULE_TYPE : "";
                seldate = lstid[0].GROUP_DATE != null ? lstid[0].GROUP_DATE : "";

                string folder = modtype;
                if (modtype == "INT_IDS")
                {
                    folder = "Internal IDS/";
                }
                else if (modtype == "EXT_IDS")
                {
                    folder = "External IDS/";
                }
                else if (modtype == "COA")
                {
                    folder = "COA Document/";
                }

                foreach (var item in lstid)
                {
                    string lotno = item.LOT_NO != null ? item.LOT_NO.ToString() : "";
                    string filename = item.FILE_ATTACHMENT != null ? item.FILE_ATTACHMENT.ToString() : "";
                    string path = filename;
                    // delete attachment
                    if (filename != null && filename != "")
                    {
                        if (modtype == "INT_IDS" || modtype == "EXT_IDS")
                        {
                            path = folder + seldate + "/" + filename;
                        }
                        else if (modtype == "COA")
                        {
                            path = folder + filename;
                        }

                        string res = await blob.DeleteBlobAsync(path);

                        if (res != "Delete Success")
                        {
                            success = false;
                            message += res;
                        }
                        else
                        {
                            string result2 = await dbdal.FileTrans_Maint(lotno, modtype, filename);
                            if (!(int.TryParse(result2, out int num2)))
                            {
                                success = false;
                                message += result2;
                            }
                        }
                    }
                }
            }
            var data = new { success = success, message = message, type = modtype, seldate = seldate };

            return Json(data);
        }

        [HttpPost]
        public async Task<ActionResult> FILE_ATTACHMENT_DL(List<FileAttLstModel> lstid)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var file in lstid)
                    {
                        var (stream, fileName) = await blob.GetBlobFileSel(file.FILE_ATTACHMENT_PATH);

                        var entry = archive.CreateEntry(fileName, CompressionLevel.Fastest);
                        using (var entryStream = entry.Open())
                        {
                            await stream.CopyToAsync(entryStream);
                        }

                        stream.Dispose();

                    }
                }

                string timestamp = DateTime.Now.ToString("dd-MM-yyyy");

                Response.Headers.Add("Content-Disposition", "attachment; filename=File Attachment - " + timestamp + ".zip");
                return File(memoryStream.ToArray(), "application/zip");
            }
        }

        [HttpPost]
        public async Task<ActionResult> FILE_ATTACHMENT_DL_BULK(List<FileAttLstModel> lstid)
        {
            try
            {
                if (lstid == null || lstid.Count == 0)
                    return Json(new { success = false, message = "No dates selected." });

                string modtype = lstid[0].MODULE_TYPE != null ? lstid[0].MODULE_TYPE : "";

                string folder = modtype;
                string zipFileName = $"File Attachment_{DateTime.Now:dd-MM-yyyy}.zip";
                if (modtype == "INT_IDS")
                {
                    folder = "Internal IDS/";
                    zipFileName = $"Internal IDS_File Attachment_{DateTime.Now:dd-MM-yyyy}.zip";
                }
                else if (modtype == "EXT_IDS")
                {
                    folder = "External IDS/";
                    zipFileName = $"External IDS_File Attachment_{DateTime.Now:dd-MM-yyyy}.zip";
                }
                else if (modtype == "COA")
                {
                    folder = "COA Document/";
                    zipFileName = $"COA_File Attachment_{DateTime.Now:dd-MM-yyyy}.zip";
                }

                using (var zipStream = new MemoryStream())
                {
                    using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var item in lstid)
                        {
                            List<(string FileName, byte[] FileBytes)> files = new List<(string FileName, byte[] FileBytes)>();

                            string date = item.GROUP_DATE;
                            if (modtype == "INT_IDS" || modtype == "EXT_IDS")
                            {
                                files = await blob.GetBlobFilesBySubfolder(date, folder);
                            }
                            else if (modtype == "COA")
                            {
                                files = await blob.GetCOABlobFileByDate(date, folder);
                            }

                            if (files.Count == 0)
                                continue;

                            // ✅ Create a folder inside the ZIP for each date
                            foreach (var file in files)
                            {
                                string entryName = $"{date}/{file.FileName}";
                                var zipEntry = archive.CreateEntry(entryName, CompressionLevel.Fastest);

                                using (var entryStream = zipEntry.Open())
                                {
                                    await entryStream.WriteAsync(file.FileBytes, 0, file.FileBytes.Length);
                                }
                            }
                        }
                    }

                    zipStream.Position = 0;

                    
                    return File(zipStream.ToArray(), "application/zip", zipFileName);
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, "Error generating ZIP: " + ex.Message);
            }
        }

        #endregion


    }
}