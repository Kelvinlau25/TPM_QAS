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
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace qas.Controllers
{
    public class MM_BIS_DESG_CODEController : Controller
    {
        DB dbmain = new DB();
        MM_BIS_DESG_CODE_DAL dbdal = new MM_BIS_DESG_CODE_DAL();

        // GET: MM_BIS_DESG_CODE_LST

        //public async Task<ActionResult>  MM_BIS_DESG_CODE_LST(string id, string password_cur)
        //{

        //    //if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(password_cur))
        //    //{
        //    //    string systemName = TPM_QAS.DAL.Database.GetAppSettingStatic("SystemName_ACL"];
        //    //    string companyCode = TPM_QAS.DAL.Database.GetAppSettingStatic("CompanyCode_ACL"];
        //    //    string DatabaseType = TPM_QAS.DAL.Database.GetAppSettingStatic("DBTYPE"];

        //    //    LoginRequest loginmodel = new LoginRequest();

        //    //    loginmodel.LoginID = id;
        //    //    loginmodel.LoginPass = password_cur;


        //    //    loginmodel.LoginID = id;
        //    //    loginmodel.LoginPass = password_cur;
        //    //    loginmodel.SystemName = systemName;
        //    //    loginmodel.CompanyCode = companyCode;
        //    //    loginmodel.DatabaseType = DatabaseType;

        //    //    ByteArrayContent clientbodystr = new StringContent(JsonConvert.SerializeObject(loginmodel), Encoding.UTF8, "application/json");

        //    //    HttpClient client = new HttpClient();
        //    //    HttpResponseMessage response = await client.PostAsync(TPM_QAS.DAL.Database.GetAppSettingStatic("ACL_API"] + "/api/v1/login/login", clientbodystr);
        //    //    if (response.IsSuccessStatusCode == true)
        //    //    {
        //    //        var responseBody = await response.Content.ReadAsStringAsync();
        //    //        var result = JsonConvert.DeserializeObject<LoginResponse>(responseBody);

        //    //        Response.Cookies["JWTToken"].Value = result.JWTToken;
        //    //        Response.Cookies["JWTToken"].Expires = DateTime.Now.AddMinutes(15);
        //    //        Response.Cookies["JWTRefreshToken"].Value = result.JWTRefreshToken;
        //    //        Response.Cookies["JWTRefreshToken"].Expires = DateTime.Now.AddMinutes(60);
        //    //        HttpContext.Session["AclUser"] = ACLHelper.JWTTokenToACLObj(result.JWTToken);


        //    //    }
        //    //}



        //    MM_BIS_DESG_CODE_VIEWMODEL model = new MM_BIS_DESG_CODE_VIEWMODEL();

        //    DataTable dt;
        //    dt = await dbmain.List("PVIEW_MM_BIS_DESG_CODE", "", "", "", "ID_MM_BIS_DESG_CODE_H", "1", "1", "10", "0");

        //    List<MM_BIS_DESG_CODE> mlist = new List<MM_BIS_DESG_CODE>();
        //    if (dt != null)
        //    {
        //        foreach (DataRow item in dt.Rows)
        //        {
        //            MM_BIS_DESG_CODE itemM = new MM_BIS_DESG_CODE();
        //            itemM.ID_MM_BIS_DESG_CODE_D = System.DBNull.Value == item["ID_MM_BIS_DESG_CODE_D"] ? 0 : Convert.ToInt16(item["ID_MM_BIS_DESG_CODE_D"]);
        //            itemM.ID_MM_BIS_DESG_CODE_H = System.DBNull.Value == item["ID_MM_BIS_DESG_CODE_H"] ? 0 : Convert.ToInt16(item["ID_MM_BIS_DESG_CODE_H"]);
        //            itemM.BIS_DESG_CAT = System.DBNull.Value == item["BIS_DESG_CAT"] ? "" : item["BIS_DESG_CAT"].ToString();
        //            itemM.BIS_DESG_CODE = System.DBNull.Value == item["BIS_DESG_CODE"] ? "" : item["BIS_DESG_CODE"].ToString();
        //            itemM.BIS_MIN_RANGE = System.DBNull.Value == item["BIS_MIN_RANGE"] ? 0 : Convert.ToDecimal(item["BIS_MIN_RANGE"]);
        //            itemM.BIS_MAX_RANGE = System.DBNull.Value == item["BIS_MAX_RANGE"] ? 0 : Convert.ToDecimal(item["BIS_MAX_RANGE"]);

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
        public async Task<ActionResult> MM_BIS_DESG_CODE_LST(string Deleted = "0")
        {
            TempData["STATUS_DDL"] = await dbmain.PSP_COMMON_DDL("CMN_LST_STATUS", "", "", "", "", "", "");

            CommonFunction common = new CommonFunction();
            var param = new { Table = "PVIEW_MM_BIS_DESG_CODE", TableID = "", Search = "", Value = "", SortField = "ID_MM_BIS_DESG_CODE_H", Direction = "0", FrmRowno = "1", ToRowno = "500000", Deleted = Deleted };
            List<MM_BIS_DESG_CODE_VIEWMODEL> model = await common.PSP_COMMON_DAPPER<MM_BIS_DESG_CODE_VIEWMODEL>("PSP_COMMON_LIST", CommandType.StoredProcedure, param) ?? new List<MM_BIS_DESG_CODE_VIEWMODEL>();

            ViewBag.Deleted = Deleted;
            return View(model);
        }

        #endregion

        // GET: MM_BIS_DESG_CODE_ADD

        public ActionResult MM_BIS_DESG_CODE_ADD()
        {
            MM_BIS_DESG_CODE_VIEWMODEL model = new MM_BIS_DESG_CODE_VIEWMODEL();

            return View(model);
        }

        // POST: MM_BIS_DESG_CODE_ADD
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> MM_BIS_DESG_CODE_ADD(string category, List<Dictionary<string, string>> item_dtl)
        {
            bool success = true;
            string message = "";

            //Create Header
            string headerID = "";
            string resulth = await dbdal.BisDesgCodeHeaderMaint(0, category, "1");
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
                    string ID_MM_BIS_DESG_CODE_D = item["ID_MM_BIS_DESG_CODE_D"].ToString();
                    string BIS_DESG_CODE = item["BIS_DESG_CODE"].ToString();
                    string BIS_MIN_RANGE = item["BIS_MIN_RANGE"].ToString();
                    string BIS_MAX_RANGE = item["BIS_MAX_RANGE"].ToString();
                    string RECORD_TYP = item["RECORD_TYP"].ToString();

                    if (RECORD_TYP == "1") //insert
                    {
                        string addresult = await dbdal.BisDesgCodeDetailMaint(Convert.ToInt32(ID_MM_BIS_DESG_CODE_D), Convert.ToInt32(headerID), BIS_DESG_CODE.ToString(), Convert.ToDecimal(BIS_MIN_RANGE), Convert.ToDecimal(BIS_MAX_RANGE), "1");
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

        // GET: MM_BIS_DESG_CODE_VIEW
        [SessionExpire]
        public async Task<ActionResult> MM_BIS_DESG_CODE_VIEW(string id)
        {
            MM_BIS_DESG_CODE_VIEWMODEL model = new MM_BIS_DESG_CODE_VIEWMODEL();
            try
            {
                model = await dbdal.GetBisDesgCodeHeaderData(id);
                model.ModelList = await dbdal.GetBisDesgCodeDetailData(id);
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
            return View(model);
        }

        // GET: MM_BIS_DESG_CODE_EDIT
        public async Task<ActionResult> MM_BIS_DESG_CODE_EDIT(string id)
        {
            MM_BIS_DESG_CODE_VIEWMODEL model = new MM_BIS_DESG_CODE_VIEWMODEL();
            try
            {
                model = await dbdal.GetBisDesgCodeHeaderData(id);
                model.ModelList = await dbdal.GetBisDesgCodeDetailData(id);
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
            return View(model);
        }

        // POST: MM_BIS_DESG_CODE
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> MM_BIS_DESG_CODE_EDIT(string pID, string category, List<Dictionary<string, string>> item_dtl)
        {
            bool success = true;
            string message = "";

            //Create Header
            string headerID = pID;
            string resulth = await dbdal.BisDesgCodeHeaderMaint(Convert.ToInt32(headerID), category, "3");
            if (!(int.TryParse(resulth, out int num1) && resulth != "0")) //not number or 0
            {
                success = false;
                message = resulth;
            }

            if (success && (item_dtl != null && item_dtl.Count != 0))
            {
                foreach (var item in item_dtl)
                {
                    string ID_MM_BIS_DESG_CODE_D = item["ID_MM_BIS_DESG_CODE_D"].ToString();
                    string BIS_DESG_CODE = item["BIS_DESG_CODE"].ToString();
                    string BIS_MIN_RANGE = item["BIS_MIN_RANGE"].ToString();
                    string BIS_MAX_RANGE = item["BIS_MAX_RANGE"].ToString();
                    string RECORD_TYP = item["RECORD_TYP"].ToString();

                    if (RECORD_TYP == "1") //insert
                    {
                        string addresult = await dbdal.BisDesgCodeDetailMaint(Convert.ToInt32(ID_MM_BIS_DESG_CODE_D), Convert.ToInt32(headerID), BIS_DESG_CODE.ToString(), Convert.ToDecimal(BIS_MIN_RANGE), Convert.ToDecimal(BIS_MAX_RANGE), "1");
                        if (!(int.TryParse(addresult, out int num2) && addresult != "0"))
                        {
                            message += addresult;
                        }
                    }
                    if (RECORD_TYP == "3") //update
                    {
                        string updresult = await dbdal.BisDesgCodeDetailMaint(Convert.ToInt32(ID_MM_BIS_DESG_CODE_D), Convert.ToInt32(headerID), BIS_DESG_CODE.ToString(), Convert.ToDecimal(BIS_MIN_RANGE), Convert.ToDecimal(BIS_MAX_RANGE), "3");
                        if (!(int.TryParse(updresult, out int num2) && updresult != "0"))
                        {
                            message += updresult;
                        }
                    }
                    if (RECORD_TYP == "5") //delete
                    {
                        string delresult = await dbdal.BisDesgCodeDetailMaint(Convert.ToInt32(ID_MM_BIS_DESG_CODE_D), Convert.ToInt32(headerID), BIS_DESG_CODE.ToString(), Convert.ToDecimal(BIS_MIN_RANGE), Convert.ToDecimal(BIS_MAX_RANGE), "5");
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

        // POST: MM_BIS_DESG_CODE_H_DEL
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> MM_BIS_DESG_CODE_H_DEL(List<string> pID)
        {
            bool success = true;
            string message = "";

            for (int i = 0; i < pID.Count; i++)
            {
                string headerID = pID[i];
                string delresult = await dbdal.BisDesgCodeHeaderMaint(Convert.ToInt32(headerID), "", "5");
                if (!(int.TryParse(delresult, out int num2) && delresult != "0"))
                {
                    success = false;
                    message += delresult;
                }
            }

            var data = new { success = success, message = message };

            return Json(data);
        }
        // POST: MM_BIS_DESG_CODE_D_DEL
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> MM_BIS_DESG_CODE_D_DEL(List<string> pID)
        {
            bool success = true;
            string message = "";

            for (int i = 0; i < pID.Count; i++)
            {
                string detailID = pID[i];
                string delresult = await dbdal.BisDesgCodeDetailMaint(Convert.ToInt32(detailID), 0, "", 0, 0, "7");
                if (!(int.TryParse(delresult, out int num2) && delresult != "0"))
                {
                    success = false;
                    message += delresult;
                }
            }

            var data = new { success = success, message = message };

            return Json(data);
        }


        [SessionExpire]
        public async Task<ActionResult> MM_BIS_DESG_CODE_DETAIL(string id)
        {
            MM_BIS_DESG_CODE_VIEWMODEL model = new MM_BIS_DESG_CODE_VIEWMODEL();
            try
            {
                model = await dbdal.GetBisDesgCodeHeaderData(id);
                model.ModelList = await dbdal.GetBisDesgCodeDetailData(id);
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
            return View(model);
        }


        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> MM_BIS_DESG_CODE_DETAIL(string pID, string category, List<Dictionary<string, string>> item_dtl)
        {
            bool success = true;
            string message = "";

            //Create Header
            string headerID = pID;
            string resulth = await dbdal.BisDesgCodeHeaderMaint(Convert.ToInt32(headerID), category, "3");
            if (!(int.TryParse(resulth, out int num1) && resulth != "0")) //not number or 0
            {
                success = false;
                message = resulth;
            }

            if (success && (item_dtl != null && item_dtl.Count != 0))
            {
                foreach (var item in item_dtl)
                {
                    string ID_MM_BIS_DESG_CODE_D = item["ID_MM_BIS_DESG_CODE_D"].ToString();
                    string BIS_DESG_CODE = item["BIS_DESG_CODE"].ToString();
                    string BIS_MIN_RANGE = item["BIS_MIN_RANGE"].ToString();
                    string BIS_MAX_RANGE = item["BIS_MAX_RANGE"].ToString();
                    string RECORD_TYP = item["RECORD_TYP"].ToString();

                    if (RECORD_TYP == "1") //insert
                    {
                        string addresult = await dbdal.BisDesgCodeDetailMaint(Convert.ToInt32(ID_MM_BIS_DESG_CODE_D), Convert.ToInt32(headerID), BIS_DESG_CODE.ToString(), Convert.ToDecimal(BIS_MIN_RANGE), Convert.ToDecimal(BIS_MAX_RANGE), "1");
                        if (!(int.TryParse(addresult, out int num2) && addresult != "0"))
                        {
                            message += addresult;
                        }
                    }
                    if (RECORD_TYP == "3") //update
                    {
                        string updresult = await dbdal.BisDesgCodeDetailMaint(Convert.ToInt32(ID_MM_BIS_DESG_CODE_D), Convert.ToInt32(headerID), BIS_DESG_CODE.ToString(), Convert.ToDecimal(BIS_MIN_RANGE), Convert.ToDecimal(BIS_MAX_RANGE), "3");
                        if (!(int.TryParse(updresult, out int num2) && updresult != "0"))
                        {
                            message += updresult;
                        }
                    }
                    if (RECORD_TYP == "5") //delete
                    {
                        string delresult = await dbdal.BisDesgCodeDetailMaint(Convert.ToInt32(ID_MM_BIS_DESG_CODE_D), Convert.ToInt32(headerID), BIS_DESG_CODE.ToString(), Convert.ToDecimal(BIS_MIN_RANGE), Convert.ToDecimal(BIS_MAX_RANGE), "5");
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
        #region AUDIT

        [SessionExpire]
        public async Task<ActionResult> MM_BIS_DESG_CODE_AUDIT(string id)
        {
            TempData["SQ_ID"] = id;
            CommonFunction common = new CommonFunction();
            var param = new { pTable = "PVIEW_MM_BIS_DESG_CODE_D_A", pKeyValue = id };
            List<AuditTrailModel> AuditTrailModel = await common.PSP_COMMON_DAPPER<AuditTrailModel>("PSP_COMMON_AUDIT", CommandType.StoredProcedure, param) ?? new List<AuditTrailModel>();

            return View(AuditTrailModel);
        }

        #endregion

    }
}