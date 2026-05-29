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

namespace TPM_QAS.Controllers
{
    public class MM_LOT_SEPARATIONController : Controller
    {
        DB dbmain = new DB();
        MM_LOT_SEPARATION_DAL dbdal = new MM_LOT_SEPARATION_DAL();

        #region LIST

        [SessionExpire]
        public async Task<ActionResult> MM_LOT_SEPARATION_LST(string Deleted = "0")
        {
            TempData["STATUS_DDL"] = await dbmain.PSP_COMMON_DDL("CMN_LST_STATUS", "", "", "", "", "", "");

            CommonFunction common = new CommonFunction();
            var param = new { Table = "PVIEW_MM_LOTNO", TableID = "", Search = "", Value = "", SortField = "MM_LOTNO_H_ID", Direction = "0", FrmRowno = "1", ToRowno = "500000", Deleted = Deleted };
            List<LotSeparationModel> model = await common.PSP_COMMON_DAPPER<LotSeparationModel>("PSP_COMMON_LIST", System.Data.CommandType.StoredProcedure, param) ?? new List<LotSeparationModel>();

            ViewBag.Deleted = Deleted;
            return View(model);
        }

        #endregion

        #region VIEW/EDIT

        [SessionExpire]
        public async Task<ActionResult> MM_LOT_SEPARATION_DETAIL(string id)
        {
            ViewBag.Tittle = "Lot Separation";
            var model = new LotSeparationModel();
            
            DataTable dt = await dbdal.getLotSeparation_Data(id, "H"); //get header data

            if (dt != null && dt.Rows.Count > 0)
            {
                model.MM_LOTNO_H_ID = Convert.ToInt32(dt.Rows[0]["MM_LOTNO_H_ID"]);
                model.SECTION_NAME = dt.Rows[0]["SECTION_NAME"] != DBNull.Value ? dt.Rows[0]["SECTION_NAME"].ToString() : "";
                model.PRODLINE_ID_STRING = dt.Rows[0]["PRODLINE_ID_STRING"] != DBNull.Value ? dt.Rows[0]["PRODLINE_ID_STRING"].ToString() : "";
                model.PRODLINE = dt.Rows[0]["PRODLINE"] != DBNull.Value ? dt.Rows[0]["PRODLINE"].ToString() : "";
                model.PRODLINENO = dt.Rows[0]["PRODLINENO"] != DBNull.Value ? dt.Rows[0]["PRODLINENO"].ToString() : "";
                model.PACKINGTYPE_ID = Convert.ToInt32(dt.Rows[0]["PACKINGTYPE_ID"]);
                model.PACKINGTYPE = dt.Rows[0]["PACKINGTYPE"] != DBNull.Value ? dt.Rows[0]["PACKINGTYPE"].ToString() : "";
                model.CHECK_LAST_TAG = Convert.ToInt32(dt.Rows[0]["CHECK_LAST_TAG"]);
                model.CHECK_LAST_TAG_DESC = dt.Rows[0]["CHECK_LAST_TAG_DESC"] != DBNull.Value ? dt.Rows[0]["CHECK_LAST_TAG_DESC"].ToString() : "";
                model.LOTNO = dt.Rows[0]["LOTNO"] != DBNull.Value ? dt.Rows[0]["LOTNO"].ToString() : "";
                model.MIN_INTERVAL = Convert.ToInt32(dt.Rows[0]["MIN_INTERVAL"]);
                model.MAX_INTERVAL = Convert.ToInt32(dt.Rows[0]["MAX_INTERVAL"]);
                model.INTERVAL = Convert.ToInt32(dt.Rows[0]["INTERVAL"]);
                model.FIRSTTAG = Convert.ToInt32(dt.Rows[0]["FIRST_TAG"]);


                model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";
            }

            DataTable dtl = await dbdal.getLotSeparation_Data(id, "D"); //get d data
            List<IntervalLstModel> listItemsAdd = new List<IntervalLstModel>();
            if (dtl != null && dtl.Rows.Count > 0)
            {
                for (int i = 0; i < dtl.Rows.Count; i++)
                {
                    IntervalLstModel infoObjAdd = new IntervalLstModel();
                    infoObjAdd.MM_LOTNO_D_ID = Convert.ToInt32(dtl.Rows[i]["MM_LOTNO_D_ID"]);
                    infoObjAdd.MM_LOTNO_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_LOTNO_H_ID"]);
                    infoObjAdd.LOT_NO = Convert.ToInt32(dtl.Rows[i]["LOT_NO"]);

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

            model.IntervalLstModel = listItemsAdd;

            model.PRODLINENO_ID = model.PRODLINE_ID_STRING.Split(',').ToList();

            model.DropdownSection = await LoadDllData(0, "", "SECTION");
            model.DropdownProdLine = await LoadDllData(0, "", "PROD_LINE");
            model.DropdownProdLineNo = await LoadDllData(0, model.PRODLINE, "PROD_LINE_NO");
            model.DropdownPackType = await LoadDllData(0, "", "PACKING_TYPE");
            ViewBag.FROM = "PRE";
            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MM_LOT_SEPARATION_DETAIL(string ActionType, LotSeparationModel model)
        {
            ViewBag.Tittle = "Lot Separation";
            try
            {
                DataTable dt = await dbdal.getLotSeparation_Data(model.MM_LOTNO_H_ID.ToString(), "H"); //get header only
                if (dt != null && dt.Rows.Count > 0)
                {
                    model.MM_LOTNO_H_ID = Convert.ToInt32(dt.Rows[0]["MM_LOTNO_H_ID"]);
                    model.SECTION_NAME = dt.Rows[0]["SECTION_NAME"] != DBNull.Value ? dt.Rows[0]["SECTION_NAME"].ToString() : "";
                    model.PRODLINE_ID_STRING = dt.Rows[0]["PRODLINENO_ID"] != DBNull.Value ? dt.Rows[0]["PRODLINENO_ID"].ToString() : "";
                    //model.PRODLINE = dt.Rows[0]["PRODLINE"] != DBNull.Value ? dt.Rows[0]["PRODLINE"].ToString() : "";
                    model.PRODLINENO = dt.Rows[0]["PRODLINENO"] != DBNull.Value ? dt.Rows[0]["PRODLINENO"].ToString() : "";
                    model.PACKINGTYPE_ID = Convert.ToInt32(dt.Rows[0]["PACKINGTYPE_ID"]);
                    model.PACKINGTYPE = dt.Rows[0]["PACKINGTYPE"] != DBNull.Value ? dt.Rows[0]["PACKINGTYPE"].ToString() : "";
                    model.CHECK_LAST_TAG = Convert.ToInt32(dt.Rows[0]["CHECK_LAST_TAG"]);
                    model.CHECK_LAST_TAG_DESC = dt.Rows[0]["CHECK_LAST_TAG_DESC"] != DBNull.Value ? dt.Rows[0]["CHECK_LAST_TAG_DESC"].ToString() : "";
                    model.LOTNO = dt.Rows[0]["LOTNO"] != DBNull.Value ? dt.Rows[0]["LOTNO"].ToString() : "";
                    model.MIN_INTERVAL = Convert.ToInt32(dt.Rows[0]["MIN_INTERVAL"]);
                    model.MAX_INTERVAL = Convert.ToInt32(dt.Rows[0]["MAX_INTERVAL"]);
                    model.INTERVAL = Convert.ToInt32(dt.Rows[0]["INTERVAL"]);
                    model.FIRSTTAG = Convert.ToInt32(dt.Rows[0]["FIRST_TAG"]);


                    model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                    model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                    model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                    model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                    model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                    model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                    model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";
                }

                DataTable dtl = await dbdal.getLotSeparation_Data(model.MM_LOTNO_H_ID.ToString(), "D"); //get d data
                List<IntervalLstModel> listItemsAdd = new List<IntervalLstModel>();
                if (dtl != null && dtl.Rows.Count > 0)
                {
                    for (int i = 0; i < dtl.Rows.Count; i++)
                    {
                        IntervalLstModel infoObjAdd = new IntervalLstModel();
                        infoObjAdd.MM_LOTNO_D_ID = Convert.ToInt32(dtl.Rows[i]["MM_LOTNO_D_ID"]);
                        infoObjAdd.MM_LOTNO_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_LOTNO_H_ID"]);
                        infoObjAdd.LOT_NO = Convert.ToInt32(dtl.Rows[i]["LOT_NO"]);

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

                model.IntervalLstModel = listItemsAdd;
                model.PRODLINENO_ID = model.PRODLINE_ID_STRING.Split(',').ToList();

                model.DropdownSection = await LoadDllData(0, "", "SECTION");
                model.DropdownProdLine = await LoadDllData(0, "", "PROD_LINE");
                model.DropdownPackType = await LoadDllData(0, "", "PACKING_TYPE");


                if (model.PRODLINE != "" && model.PRODLINE != null && !model.PRODLINE.Contains("Select an option"))
                {
                    model.DropdownProdLineNo = await LoadDllData(0, model.PRODLINE, "PROD_LINE_NO");
                }
                else
                {
                    model.DropdownProdLineNo = new List<SelectListItem>();
                }

                if (ActionType == "postback")
                {
                    ModelState.Clear();
                }
                if (ActionType == "Back")
                {
                    ModelState.Clear();
                    return RedirectToAction("MM_LOT_SEPARATION_LST", "MM_LOT_SEPARATION");
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
        public async Task<ActionResult> InsertUpdateLotSeparation(LotSeparationModel model)
        {
            bool success = true;
            string message = "";


            if (ModelState.IsValid)
            {
                if (model.SECTION_NAME == null || model.SECTION_NAME == "" ||
                    model.PRODLINE == null || model.PRODLINE == "" ||
                    model.PRODLINE_ID_STRING == null || model.PACKINGTYPE_ID == 0 ||
                    model.IntervalLstModel == null || model.IntervalLstModel.Count < 1)
                {
                    success = false;
                    message += "Required field cannot empty.";
                }

                if (success)
                {
                    // insert h
                    string result1 = await dbdal.LotSeparation_H_Maint(model);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                    else
                    {
                        //delete existing d first
                        string result3 = await dbdal.LotSeparation_D_Maint(0, Convert.ToInt32(result1), 0, "2");
                        if (!(int.TryParse(result3, out int num3)))
                        {
                            success = false;
                            message += result3;
                        }
                        else
                        {
                            // insert d
                            foreach (var item in model.IntervalLstModel)
                            {
                                int lotno = Convert.ToInt32(item.LOT_NO);
                                string rectype = item.RECORD_TYP != null ? item.RECORD_TYP.ToString() : "";

                                string result2 = await dbdal.LotSeparation_D_Maint(item.MM_LOTNO_D_ID, Convert.ToInt32(result1), lotno, rectype);
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
                    message += "Error in saving data. ";
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
        public async Task<ActionResult> MM_LOT_SEPARATION_ADD(string id = "")
        {
            ViewBag.Tittle = "Lot Separation";
            var model = new LotSeparationModel();
            List<IntervalLstModel> listItemsAdd = new List<IntervalLstModel>();

            model.DropdownSection = await LoadDllData(0, "", "SECTION");
            model.DropdownProdLine = await LoadDllData(0, "", "PROD_LINE");
            model.DropdownProdLineNo = new List<SelectListItem>();
            model.DropdownPackType = await LoadDllData(0, "", "PACKING_TYPE");

            if (!string.IsNullOrWhiteSpace(id) && CommonMethod.isNumeric(id))
            {
                DataTable dt = await dbdal.getLotSeparation_Data(id, "H"); //get header data

                if (dt != null && dt.Rows.Count > 0)
                {
                    model.MM_LOTNO_H_ID = Convert.ToInt32(dt.Rows[0]["MM_LOTNO_H_ID"]);
                    model.SECTION_NAME = dt.Rows[0]["SECTION_NAME"] != DBNull.Value ? dt.Rows[0]["SECTION_NAME"].ToString() : "";
                    model.PRODLINE_ID_STRING = dt.Rows[0]["PRODLINE_ID_STRING"] != DBNull.Value ? dt.Rows[0]["PRODLINE_ID_STRING"].ToString() : "";
                    model.PRODLINE = dt.Rows[0]["PRODLINE"] != DBNull.Value ? dt.Rows[0]["PRODLINE"].ToString() : "";
                    model.PRODLINENO = dt.Rows[0]["PRODLINENO"] != DBNull.Value ? dt.Rows[0]["PRODLINENO"].ToString() : "";
                    model.PACKINGTYPE_ID = Convert.ToInt32(dt.Rows[0]["PACKINGTYPE_ID"]);
                    model.PACKINGTYPE = dt.Rows[0]["PACKINGTYPE"] != DBNull.Value ? dt.Rows[0]["PACKINGTYPE"].ToString() : "";
                    model.CHECK_LAST_TAG = Convert.ToInt32(dt.Rows[0]["CHECK_LAST_TAG"]);
                    model.CHECK_LAST_TAG_DESC = dt.Rows[0]["CHECK_LAST_TAG_DESC"] != DBNull.Value ? dt.Rows[0]["CHECK_LAST_TAG_DESC"].ToString() : "";
                    model.LOTNO = dt.Rows[0]["LOTNO"] != DBNull.Value ? dt.Rows[0]["LOTNO"].ToString() : "";
                    model.MIN_INTERVAL = Convert.ToInt32(dt.Rows[0]["MIN_INTERVAL"]);
                    model.MAX_INTERVAL = Convert.ToInt32(dt.Rows[0]["MAX_INTERVAL"]);
                    model.INTERVAL = Convert.ToInt32(dt.Rows[0]["INTERVAL"]);
                    model.FIRSTTAG = Convert.ToInt32(dt.Rows[0]["FIRST_TAG"]);


                    model.RECORD_TYP = dt.Rows[0]["RECORD_TYP"] != DBNull.Value ? dt.Rows[0]["RECORD_TYP"].ToString() : "";
                    model.CREATED_BY = dt.Rows[0]["CREATED_BY"] != DBNull.Value ? dt.Rows[0]["CREATED_BY"].ToString() : "";
                    model.CREATED_DATE = dt.Rows[0]["CREATED_DATE"] != DBNull.Value ? dt.Rows[0]["CREATED_DATE"].ToString() : "";
                    model.CREATED_LOC = dt.Rows[0]["CREATED_LOC"] != DBNull.Value ? dt.Rows[0]["CREATED_LOC"].ToString() : "";
                    model.UPDATED_BY = dt.Rows[0]["UPDATED_BY"] != DBNull.Value ? dt.Rows[0]["UPDATED_BY"].ToString() : "";
                    model.UPDATED_DATE = dt.Rows[0]["UPDATED_DATE"] != DBNull.Value ? dt.Rows[0]["UPDATED_DATE"].ToString() : "";
                    model.UPDATED_LOC = dt.Rows[0]["UPDATED_LOC"] != DBNull.Value ? dt.Rows[0]["UPDATED_LOC"].ToString() : "";

                    model.DropdownProdLineNo = await LoadDllData(0, model.PRODLINE, "PROD_LINE_NO");
                    model.PRODLINENO_ID = model.PRODLINE_ID_STRING.Split(',').ToList();
                }

                DataTable dtl = await dbdal.getLotSeparation_Data(id, "D"); //get d data
                if (dtl != null && dtl.Rows.Count > 0)
                {
                    for (int i = 0; i < dtl.Rows.Count; i++)
                    {
                        IntervalLstModel infoObjAdd = new IntervalLstModel();
                        infoObjAdd.MM_LOTNO_D_ID = Convert.ToInt32(dtl.Rows[i]["MM_LOTNO_D_ID"]);
                        infoObjAdd.MM_LOTNO_H_ID = Convert.ToInt32(dtl.Rows[i]["MM_LOTNO_H_ID"]);
                        infoObjAdd.LOT_NO = Convert.ToInt32(dtl.Rows[i]["LOT_NO"]);

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


            }
            else
            {
                model = new LotSeparationModel();
                //model.PRODLINENO_ID = model.PRODLINE_ID_STRING.Split(',').ToList();
                model.DropdownSection = await LoadDllData(0, "", "SECTION");
                model.DropdownProdLine = await LoadDllData(0, "", "PROD_LINE");
                model.DropdownProdLineNo = new List<SelectListItem>();
                model.DropdownPackType = await LoadDllData(0, "", "PACKING_TYPE");
            }


            model.IntervalLstModel = listItemsAdd;

            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MM_LOT_SEPARATION_ADD(string ActionType, LotSeparationModel model)
        {
            ViewBag.Tittle = "Lot Separation";
            try
            {
                model.PRODLINENO_ID = model.PRODLINE_ID_STRING.Split(',').ToList();
                model.DropdownSection = await LoadDllData(0, "", "SECTION");
                model.DropdownProdLine = await LoadDllData(0, "", "PROD_LINE");
                model.DropdownPackType = await LoadDllData(0, "", "PACKING_TYPE");

                if (model.PRODLINE != "" && model.PRODLINE != null && !model.PRODLINE.Contains("Select an option"))
                {
                    model.DropdownProdLineNo = await LoadDllData(0, model.PRODLINE, "PROD_LINE_NO");
                }
                else
                {
                    model.DropdownProdLineNo = new List<SelectListItem>();
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
        public async Task<ActionResult> DraftLotSeparation(LotSeparationModel model)
        {
            bool success = true;
            string message = "";

            //model.PRODLINE = model.PRODLINE != null ? model.PRODLINE.ToString() : "";
            //model.PRODLINENO = model.PRODLINENO != null ? model.PRODLINENO.ToString() : "";
            //model.PACKINGTYPE = model.PACKINGTYPE != null ? model.PACKINGTYPE.ToString() : "";
            //model.CHECK_LAST_TAG_DESC = model.CHECK_LAST_TAG_DESC != null ? model.CHECK_LAST_TAG_DESC.ToString() : "";
            //model.LOTNO = model.LOTNO != null ? model.LOTNO.ToString() : "";


            //if (ModelState.IsValid)
            //{
            if (model.SECTION_NAME == null || model.SECTION_NAME == "" || model.PACKINGTYPE_ID == 0)
            {
                success = false;
                message += "Required field cannot empty.";
            }

            if (success)
            {
                // insert h
                string result1 = await dbdal.LotSeparation_H_Maint(model);
                if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                {
                    success = false;
                    message += result1;
                }
                else
                {
                    if (model.IntervalLstModel != null && model.IntervalLstModel.Count > 0)
                    {
                        //delete existing d first
                        string result3 = await dbdal.LotSeparation_D_Maint(0, Convert.ToInt32(result1), 0, "2");
                        if (!(int.TryParse(result3, out int num3)))
                        {
                            success = false;
                            message += result3;
                        }
                        else
                        {
                            // insert d
                            foreach (var item in model.IntervalLstModel)
                            {
                                int lotno = Convert.ToInt32(item.LOT_NO);
                                string rectype = item.RECORD_TYP != null ? item.RECORD_TYP.ToString() : "";

                                string result2 = await dbdal.LotSeparation_D_Maint(item.MM_LOTNO_D_ID, Convert.ToInt32(result1), lotno, rectype);
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
            //}
            //else
            //{
            //    success = false;
            //    message += "Error in saving data : Modal State not valid.";
            //}

            var data = new { success = success, message = message };

            return Json(data);
        }


        #endregion

        #region DELETE

        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> deleteLotSeparation(List<string> lstid)
        {
            bool success = true;
            string message = "";

            LotSeparationModel modelh = new LotSeparationModel();

            if (lstid.Count < 1)
            {
                success = false;
                message = "Please select a record first to delete.";
            }

            if (success && lstid.Count > 0)
            {
                foreach (var item in lstid)
                {
                    modelh.MM_LOTNO_H_ID = Convert.ToInt32(item);
                    modelh.RECORD_TYP = "5";

                    modelh.SECTION_NAME = "";
                    modelh.PRODLINE_ID_STRING = "";
                    modelh.CHECK_LAST_TAG = 0;
                    modelh.MIN_INTERVAL = 0;
                    modelh.MAX_INTERVAL = 0;
                    modelh.INTERVAL = 0;
                    modelh.FIRSTTAG = 0;

                    //delete h
                    string result1 = await dbdal.LotSeparation_H_Maint(modelh);
                    if (!(int.TryParse(result1, out int num1) && result1 != "0"))
                    {
                        success = false;
                        message += result1;
                    }
                    else
                    {
                        //delete d
                        string result2 = await dbdal.LotSeparation_D_Maint(0, Convert.ToInt32(item), 0, "2");
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
        public async Task<ActionResult> MM_LOT_SEPARATION_AUDIT(string id)
        {
            TempData["SQ_ID"] = id;
            CommonFunction common = new CommonFunction();
            var param = new { pTable = "PVIEW_MM_LOTNO_A", pKeyValue = id };
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

        public async Task<ActionResult> fillLineNo(string lineno)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items = await LoadDllData(0, lineno, "PROD_LINE_NO");

            return Json(items);
        }


        #endregion
    }
}
