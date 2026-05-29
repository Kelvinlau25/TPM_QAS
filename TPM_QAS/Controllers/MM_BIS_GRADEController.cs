using DBModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using TPM_QAS.Filters;
using TPM_QAS.Helpers;
using TPM_QAS.DAL;
using TPM_QAS.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace qas.Controllers
{
    public class MM_BIS_GRADEController : Controller
    {
        DB dbmain = new DB();
        MM_BIS_GRADE_DAL dbdal = new MM_BIS_GRADE_DAL();

        // GET: MM_BISGRADE_LST
        //[SessionExpire]
        //public async Task<ActionResult> MM_BIS_GRADE_LST()
        //{
        //    MM_BIS_GRADE_VIEWMODEL model = new MM_BIS_GRADE_VIEWMODEL();

        //    DataTable dt;
        //    dt = await dbmain.List("PVIEW_MM_BIS_GRADE", "", "", "", "ID_MM_BIS_GRADE_H", "1", "1", "10", "0");

        //    List<MM_BIS_GRADE> mlist = new List<MM_BIS_GRADE>();
        //    if (dt != null)
        //    {
        //        foreach (DataRow item in dt.Rows)
        //        {
        //            MM_BIS_GRADE itemM = new MM_BIS_GRADE();
        //            itemM.ID_MM_BIS_GRADE_D = System.DBNull.Value == item["ID_MM_BIS_GRADE_D"] ? 0 : Convert.ToInt16(item["ID_MM_BIS_GRADE_D"]);
        //            itemM.ID_MM_BIS_GRADE_H = System.DBNull.Value == item["ID_MM_BIS_GRADE_H"] ? 0 : Convert.ToInt16(item["ID_MM_BIS_GRADE_H"]);
        //            itemM.BIS_GRADE_CODE = System.DBNull.Value == item["BIS_GRADE_CODE"] ? "" : item["BIS_GRADE_CODE"].ToString();
        //            itemM.BIS_GRADE_PROD_CODE = System.DBNull.Value == item["BIS_GRADE_PROD_CODE"] ? "" : item["BIS_GRADE_PROD_CODE"].ToString();
        //            itemM.BIS_SINGLE_DESIGN_CODE = System.DBNull.Value == item["BIS_SINGLE_DESIGN_CODE"] ? "" : item["BIS_SINGLE_DESIGN_CODE"].ToString();
        //            itemM.RECORD_TYP = System.DBNull.Value == item["RECORD_TYP"] ? "" : item["RECORD_TYP"].ToString();
        //            itemM.CREATED_BY = System.DBNull.Value == item["CREATED_BY"] ? "" : item["CREATED_BY"].ToString();
        //            itemM.CREATED_DATE = System.DBNull.Value == item["CREATED_DATE"] ? DateTime.MinValue : Convert.ToDateTime(item["CREATED_DATE"]);
        //            itemM.CREATED_LOC = System.DBNull.Value == item["CREATED_LOC"] ? "" : item["CREATED_LOC"].ToString();
        //            itemM.UPDATED_BY = System.DBNull.Value == item["UPDATED_BY"] ? "" : item["UPDATED_BY"].ToString();
        //            itemM.UPDATED_DATE = System.DBNull.Value == item["UPDATED_DATE"] ? DateTime.MinValue : Convert.ToDateTime(item["UPDATED_DATE"]);
        //            itemM.UPDATED_LOC = System.DBNull.Value == item["UPDATED_LOC"] ? "" : item["UPDATED_LOC"].ToString();
        //            mlist.Add(itemM);
        //        }
        //    }

        //    model.ModelList = mlist;
        //    return View(model);
        //}

        #region LIST

        [SessionExpire]
        public async Task<ActionResult> MM_BIS_GRADE_LST(string Deleted = "0")
        {
            TempData["STATUS_DDL"] = await dbmain.PSP_COMMON_DDL("CMN_LST_STATUS", "", "", "", "", "", "");

            CommonFunction common = new CommonFunction();
            var param = new { Table = "PVIEW_MM_BIS_GRADE", TableID = "", Search = "", Value = "", SortField = "ID_MM_BIS_GRADE_H", Direction = "0", FrmRowno = "1", ToRowno = "500000", Deleted = Deleted };
            List<MM_BIS_GRADE_VIEWMODEL> model = await common.PSP_COMMON_DAPPER<MM_BIS_GRADE_VIEWMODEL>("PSP_COMMON_LIST", CommandType.StoredProcedure, param) ?? new List<MM_BIS_GRADE_VIEWMODEL>();

            ViewBag.Deleted = Deleted;

            return View(model);
        }

        #endregion

        // GET: MM_BISGRADE_ADD
        [SessionExpire]
        public ActionResult MM_BIS_GRADE_ADD()
        {
            MM_BIS_GRADE_VIEWMODEL model = new MM_BIS_GRADE_VIEWMODEL();

            return View(model);
        }

        // POST: MM_BISGRADE_ADD
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> MM_BIS_GRADE_ADD(string ds_code, List<Dictionary<string, string>> item_dtl)
        {
            bool success = true;
            string message = "";

            //Create Header
            string headerID = "";
            string resulth = await dbdal.BisGradeHeaderMaint(0, ds_code, "1");
            if (!(int.TryParse(resulth, out int num1) && resulth != "0")) //not number or 0
            {
                success = false;
                message = resulth;
            }
            else
            {
                headerID = resulth;
            }

            if (success && (item_dtl != null && item_dtl.Count != 0))
            {
                foreach (var item in item_dtl)
                {
                    string ID_MM_BIS_GRADE_D = item["ID_MM_BIS_GRADE_D"].ToString();
                    string BIS_GRADE_PROD_CODE = item["BIS_GRADE_PROD_CODE"].ToString();
                    string RECORD_TYP = item["RECORD_TYP"].ToString();
                    string SINGLE_CODE = item["BIS_SINGLE_DESIGN_CODE"].ToString();
                    if(string.IsNullOrEmpty(SINGLE_CODE))
                    {
                        SINGLE_CODE = "";
                    }

                    if (RECORD_TYP == "1") //insert
                    {
                        string addresult = await dbdal.BisGradeDetailMaint(Convert.ToInt32(ID_MM_BIS_GRADE_D), Convert.ToInt32(headerID), BIS_GRADE_PROD_CODE.ToString(), SINGLE_CODE.ToString(), "1");
                        if (!(int.TryParse(addresult, out int num2) && addresult != "0"))
                        {
                            message += addresult;
                        }
                    }
                }
            }

            var data = new { success = success, message = message };

            return Json(data);
        }

        // GET: MM_BISGRADE_VIEW
        [SessionExpire]
        public async Task<ActionResult> MM_BIS_GRADE_DETAIL(string id)
        {
            MM_BIS_GRADE_VIEWMODEL model = new MM_BIS_GRADE_VIEWMODEL();
            try
            {
                model = await dbdal.GetBisGradeHeaderData(id);
                model.ModelList = await dbdal.GetBisGradeDetailData(id);
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
            return View(model);
        }


        // POST: MM_BISGRADE_EDIT
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> MM_BIS_GRADE_DETAIL(string pID, string ds_code, List<Dictionary<string, string>> item_dtl)
        {
            bool success = true;
            string message = "";

            //Create Header
            string headerID = pID;
            string resulth = await dbdal.BisGradeHeaderMaint(Convert.ToInt32(headerID), ds_code, "3");
            if (!(int.TryParse(resulth, out int num1) && resulth != "0")) //not number or 0
            {
                success = false;
                message = resulth;
            }

            if (success && (item_dtl != null && item_dtl.Count != 0))
            {
                foreach (var item in item_dtl)
                {
                    string ID_MM_BIS_GRADE_D = item["ID_MM_BIS_GRADE_D"].ToString();
                    string BIS_GRADE_PROD_CODE = item["BIS_GRADE_PROD_CODE"].ToString();
                    string RECORD_TYP = item["RECORD_TYP"].ToString();
                    string SINGLE_CODE = item["BIS_SINGLE_DESIGN_CODE"].ToString();
                    if (string.IsNullOrEmpty(SINGLE_CODE))
                    {
                        SINGLE_CODE = "";
                    }

                    if (RECORD_TYP == "1") //insert
                    {
                        string addresult = await dbdal.BisGradeDetailMaint(Convert.ToInt32(ID_MM_BIS_GRADE_D), Convert.ToInt32(headerID), BIS_GRADE_PROD_CODE.ToString(), SINGLE_CODE.ToString(), "1");
                        if (!(int.TryParse(addresult, out int num2) && addresult != "0"))
                        {
                            message += addresult;
                        }
                    }
                    if (RECORD_TYP == "3") //update
                    {
                        string updresult = await dbdal.BisGradeDetailMaint(Convert.ToInt32(ID_MM_BIS_GRADE_D), Convert.ToInt32(headerID), BIS_GRADE_PROD_CODE.ToString(), SINGLE_CODE.ToString(), "3");
                        if (!(int.TryParse(updresult, out int num2) && updresult != "0"))
                        {
                            message += updresult;
                        }
                    }
                    if (RECORD_TYP == "5") //delete
                    {
                        string delresult = await dbdal.BisGradeDetailMaint(Convert.ToInt32(ID_MM_BIS_GRADE_D), Convert.ToInt32(headerID), BIS_GRADE_PROD_CODE.ToString(), SINGLE_CODE.ToString(), "5");
                        if (!(int.TryParse(delresult, out int num2) && delresult != "0"))
                        {
                            message += delresult;
                        }
                    }
                }
            }

            var data = new { success = success, message = message };

            return Json(data);
        }

        // POST: MM_BISGRADE_H_DEL
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> MM_BIS_GRADE_H_DEL(List<string> pID)
        {
            bool success = true;
            string message = "";

            for (int i = 0; i < pID.Count; i++)
            {
                string headerID = pID[i];
                string delresult = await dbdal.BisGradeHeaderMaint(Convert.ToInt32(headerID), "", "5");
                if (!(int.TryParse(delresult, out int num2) && delresult != "0"))
                {
                    success = false;
                    message += delresult;
                }
            }

            var data = new { success = success, message = message };

            return Json(data);
        }
        // POST: MM_BISGRADE_D_DEL
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> MM_BIS_GRADE_D_DEL(List<string> pID)
        {
            bool success = true;
            string message = "";

            for (int i = 0; i < pID.Count; i++)
            {
                string detailID = pID[i];
                string delresult = await dbdal.BisGradeDetailMaint(Convert.ToInt32(detailID), 0, "", "", "7");
                if (!(int.TryParse(delresult, out int num2) && delresult != "0"))
                {
                    success = false;
                    message += delresult;
                }
            }

            var data = new { success = success, message = message };

            return Json(data);
        }

        // GET: MM_BISGRADE_AUDIT
        [SessionExpire]
        public async Task<ActionResult> MM_BIS_GRADE_A(string id)
        {
            MM_BIS_GRADE_A_VIEWMODEL model = new MM_BIS_GRADE_A_VIEWMODEL();
            model.SQ_ID = id.ToString();

            string tmp_stmt = "KEY_VALUE = " + id;

            List<MM_BIS_GRADE_A> mlist = new List<MM_BIS_GRADE_A>();
            DataTable dt = await dbmain.List("PVIEW_MM_BIS_GRADE_D_A", "", "", tmp_stmt, "UPDATED_DATE", "0", "1", "50000", "0");
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                {
                    MM_BIS_GRADE_A itemM = new MM_BIS_GRADE_A();
                    itemM.SQ_ID = System.DBNull.Value == item["SQ_ID"] ? "" : item["SQ_ID"].ToString();
                    itemM.IDDESC = System.DBNull.Value == item["IDDESC"] ? "" : item["IDDESC"].ToString();
                    itemM.KEY_FIELD = System.DBNull.Value == item["KEY_FIELD"] ? "" : item["KEY_FIELD"].ToString();
                    itemM.KEY_VALUE = System.DBNull.Value == item["KEY_VALUE"] ? "" : item["KEY_VALUE"].ToString();
                    itemM.FIELD_NAME = System.DBNull.Value == item["FIELD_NAME"] ? "" : item["FIELD_NAME"].ToString();
                    itemM.B4_UPDATE = System.DBNull.Value == item["B4_UPDATE"] ? "" : item["B4_UPDATE"].ToString();
                    itemM.AF_UPDATE = System.DBNull.Value == item["AF_UPDATE"] ? "" : item["AF_UPDATE"].ToString();
                    itemM.UPDATED_BY = System.DBNull.Value == item["UPDATED_BY"] ? "" : item["UPDATED_BY"].ToString();
                    itemM.UPDATED_DATE = System.DBNull.Value == item["UPDATED_DATE"] ? "" : item["UPDATED_DATE"].ToString();
                    itemM.UPDATED_LOC = System.DBNull.Value == item["UPDATED_LOC"] ? "" : item["UPDATED_LOC"].ToString();
                    mlist.Add(itemM);
                }
            }
            model.ModelList = mlist;

            return View(model);
        }

        #region AUDIT

        [SessionExpire]
        public async Task<ActionResult> MM_BIS_GRADE_AUDIT(string id)
        {
            TempData["SQ_ID"] = id;
            CommonFunction common = new CommonFunction();
            var param = new { pTable = "PVIEW_MM_BIS_GRADE_D_A", pKeyValue = id };
            List<AuditTrailModel> AuditTrailModel = await common.PSP_COMMON_DAPPER<AuditTrailModel>("PSP_COMMON_AUDIT", CommandType.StoredProcedure, param) ?? new List<AuditTrailModel>();

            return View(AuditTrailModel);
        }

        #endregion
    }
}