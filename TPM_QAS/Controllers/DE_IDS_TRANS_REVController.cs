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

namespace TPM_QAS.Controllers
{
    public class DE_IDS_TRANS_REVController : Controller
    {
        DB dbmain = new DB();


        #region Initialization
        public CommonModel<DailyIDSTransVM, DailyIDSTransVM> DEIDSTCM { get; set; }

        DE_IDS_TRANS_DAL dbdal = new DE_IDS_TRANS_DAL();
        DataTable dtPermission = new DataTable();
        //AclContext aclContext = new AclContext();

        string user_id = "";
        string actionName = "";
        string controllerName = "DE_IDS_TRANS_REV";

        public DE_IDS_TRANS_REVController()
        {
            DEIDSTCM = new CommonModel<DailyIDSTransVM, DailyIDSTransVM>(this, "Daily Property Inspection Result");
        }
        #endregion

        #region LIST

        [SessionExpire]
        public async Task<ActionResult> DE_IDS_TRANS_REV_LST(string Deleted = "0")
        {
            TempData["STATUS_DDL"] = await dbmain.PSP_COMMON_DDL("IDS_ENRTY", "", "", "", "", "", "");

            CommonFunction common = new CommonFunction();
            //var dt = db.List("PVIEW_IDS_H", "", "", searchval, "ID_IDS_H", "0", "1", "10", "0");
            var param = new { Table = "PVIEW_IDS_H", TableID = "", Search = "", Value = "", SortField = "ID_IDS_H", Direction = "0", FrmRowno = "1", ToRowno = "500000", Deleted = Deleted };
            List<DailyIDSTransVM> model = await common.PSP_COMMON_DAPPER<DailyIDSTransVM>("PSP_COMMON_LIST", CommandType.StoredProcedure, param) ?? new List<DailyIDSTransVM>();
            ViewBag.SerializedData = JsonConvert.SerializeObject(model);

            string emp_no = (HttpContext.Session.GetObject<ACL_UserObj>("AclUser")).EMP_NO.ToString();

            DataTable chkApproval = await dbdal.CheckUserAppRole(emp_no);
            if (chkApproval != null && chkApproval.Rows.Count > 0)
            {
                ViewBag.isManager = "Y";
            }

            //for KS
            if ((HttpContext.Session.GetObject<ACL_UserObj>("AclUser")).USER_ID.ToString() == "800133")
            {
                ViewBag.isManager = "Y";
            }

            return View(model);
        }

        #endregion

        #region ADD
        [SessionExpire]
        public async Task<ActionResult> DE_IDS_TRANS_REV_ADD()
        {
            List<DailyIDSTransVM> listProdType = await dbdal.getProdTypeList();

            ViewBag.SerializedProdType = JsonConvert.SerializeObject(listProdType);
            return View(DEIDSTCM);
        }

        //[HttpPost]
        //[SessionExpire]
        //public async Task<ActionResult> DE_IDS_TRANS_REV_ADD(DailyIDSTransVM m, List<List<fieldnamemodel>> field, List<List<List<tagnomodel>>> tag, List<descmodel> description, string action)
        //{
        //    string result = "";
        //    string NG = "";
        //    //ActionType = "Draft";	
        //    string loc = HttpContext.Connection.RemoteIpAddress?.ToString();
        //    var aclUser = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");

        //    m.CREATED_BY = aclUser.USER_ID;
        //    m.CREATED_LOC = loc;
        //    m.GRADE = "";
        //    if (action == "btnDraft")
        //    {
        //        m.STATUS = "Draft";
        //    }
        //    else
        //    {
        //        m.STATUS = "";
        //    }
        //    try
        //    {
        //        string tmp = await dbdal.lockCheck(m, "1", "");
        //        string[] tmp2 = tmp.Split('-');
        //        if (tmp2[0] == "0")
        //        {
        //            result = await dbdal.IDS_H_Maint(m, "1");
        //            //result = "4157";	
        //            await IDS_D_Maint(m, result, "1");
        //            await saveapptab(tag, description, result, NG, "1", m, field);
        //            if (m.STATUS != "Draft")
        //            {
        //                await chkData(m, result);
        //            }
        //            result = "Data save successfully";
        //            await dbdal.lockCheck(m, "3", tmp2[1]);
        //            return Json(result);
        //        }
        //        else
        //        {
        //            result = tmp2[0];
        //            return Json(result);
        //        }

        //        //return Json("FOR TEST PURPOSE");

        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("", "Error in saving data");
        //        return View(m);
        //    }
        //}

        public async Task<string> SetAfterSelProdType(string prodtype, string lotno, string packeddate)
        {
            try
            {

                DailyIDSTransVM data = await dbdal.dataByProdtype(prodtype, lotno);

                var aclUser = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
                data.UPDATEDBY = aclUser.USER_ID;
                data.TESTEDBY = aclUser.USER_ID;
                data.FullNG = "50";
                data.PACKEDDATE = DateTime.Today.ToString("dd-MM-yyyy");

                data.MoldingList = await dbdal.GetDataMolding(prodtype, packeddate, lotno);
                //DataTable cal = new DataTable();
                //int count = 0;
                //if (data.MoldingList != null)
                //{
                //    count = data.MoldingList.Count;

                //    foreach (var item in data.MoldingList)
                //    {
                //        if (item.RegressionResult != null && item.RegressionResult != "")
                //        {
                //            var X = cal.Compute(item.RegressionResult.ToString(), "");
                //            item.RegressionResult = Math.Round(Convert.ToDouble(X), 2).ToString();
                //        }
                //        await GetGrade(count, item);
                //    }
                //}
                //var app = await apptab(prodtype, lotno, packeddate, data);
                //if (app != null)
                //{
                //    data.descriptionList = app.descriptionList;
                //    data.ratioList = app.ratioList;
                //}

                //HttpContext.Session.SetString("lotno", lotno?.ToString() ?? "");

                //var _datatable = await dbdal.GetData1(prodtype, "1", lotno, packeddate);
                //var prodgroup = "";

                //if (_datatable != null && _datatable.Rows.Count > 0)
                //{
                //    if (_datatable.Rows.Count > 0)
                //    {
                //        prodgroup = _datatable.Rows[0]["PRODGROUP"].ToString();
                //    }
                //}

                //var _datatable2 = await dbdal.DataChk56(packeddate, prodgroup, prodtype, data.prodline);

                //if (_datatable2 != null)
                //{
                //    if (_datatable2 != null && _datatable2.Rows.Count > 0)
                //    {
                //        HttpContext.Session.SetString("_datatable2", _datatable2.ToString() ?? "");

                //        for (int x = 0; x < _datatable2.Rows.Count; x++)
                //        {
                //            for (int k = 0; k < data.MoldingList.Count; k++)
                //            {
                //                if (data.MoldingList[k].PropItem == _datatable2.Rows[x]["PROPITEM"].ToString())
                //                {
                //                    data.MoldingList[k].mandatory = "1";
                //                }
                //            }
                //        }
                //    }
                //}

                //data.prodgroup = prodgroup;

                //ViewBag.prodgroup = prodgroup;
                //ViewBag.yiformula = await GetYIFormula(lotno);
                //data.YIFORMULA = await GetYIFormula(lotno);

                string result = JsonConvert.SerializeObject(data).ToString();

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //public async Task chkData(DailyIDSTransVM model, string idH)
        //{
        //    DataTable datachkdt = new DataTable();
        //    int ttlcnt = 0;
        //    int nong = 0;
        //    double passperc = 0.0;
        //    var grade = "NG";
        //    bool chkind = false;
        //    var status = "Incomplete";
        //    bool chkind2 = false;
        //    string statusind = "";

        //    for (int i = 0; i < model.MoldingList.Count; i++)
        //    {
        //        datachkdt = await GetDataChk(datachkdt, model.MoldingList[i].Properties, model.MoldingList[i].PropItem, model.LOTNO, model.PRODTYPE);
        //        if (datachkdt != null && datachkdt.Rows.Count > 0)
        //        {
        //            ttlcnt = Convert.ToInt32(datachkdt.Rows[0]["TTLCNT"]);
        //            nong = Convert.ToInt32(datachkdt.Rows[0]["NONGCNT"]);
        //            passperc = Convert.ToDouble(datachkdt.Rows[0]["PASSPREC"]);

        //            if (passperc >= ((nong / ttlcnt) * 100))
        //            {
        //                grade = "OK";
        //            }
        //            else
        //            {
        //                chkind = true;
        //            }

        //            if (datachkdt.Rows[0]["Count"].ToString() == datachkdt.Rows[0]["TTLCNT"].ToString())
        //            {
        //                status = "Completed";
        //            }
        //            else
        //            {
        //                chkind2 = true;
        //            }

        //        }
        //        statusind = model.MoldingList[i].StatusInd ?? "";
        //    }

        //    if (model.STATUS != "Rejected")
        //    {
        //        bool chkind3 = true;
        //        bool chkind4 = true;
        //        bool chkind5 = true;

        //        DataTable _datatable = new DataTable();
        //        _datatable = await dbdal.GetDataTable(idH, "2", "", "", "");

        //        if (grade != "")
        //        {
        //            grade = grade + ",";
        //        }

        //        if (_datatable != null && _datatable.Rows.Count > 0)
        //        {
        //            for (int a = 0; a < _datatable.Rows.Count; a++)
        //            {
        //                if (_datatable.Rows[a]["STATUSIND"].ToString() == "Incomplete")
        //                {
        //                    chkind2 = true;
        //                    chkind3 = true;
        //                    chkind5 = false;
        //                }
        //                else if (_datatable.Rows[a]["STATUSIND"].ToString() == "Completed")
        //                {
        //                    if (chkind5)
        //                    {
        //                        status = "Completed";
        //                        chkind3 = false;
        //                    }
        //                }

        //                grade = grade + _datatable.Rows[a]["GRADE"].ToString() + ",";
        //            }
        //        }

        //        var split = grade.Split(',');
        //        if (split.Contains("NG"))
        //        {
        //            grade = "NG";
        //        }
        //        else if (split.Contains("B"))
        //        {
        //            grade = "B";
        //        }
        //        else if (split.Contains("I"))
        //        {
        //            grade = "I";
        //        }
        //        else if (split.Contains("AT"))
        //        {
        //            grade = "AT";
        //        }
        //        else if (split.Contains("AA"))
        //        {
        //            grade = "AA";
        //        }
        //        else if (split.Contains("A"))
        //        {
        //            grade = "A";
        //        }

        //        var main_mold = false;
        //        var main_bsng = false;
        //        var main_ying = false;

        //        if (grade == "NG")
        //        {
        //            main_mold = true;
        //        }

        //        if (chkind2 && chkind3)
        //        {
        //            status = "Incomplete";
        //        }

        //        var _datatableApp = await dbdal.GetDataTable(idH, "5", "", "", "");

        //        if (_datatable != null && _datatable.Rows.Count > 0)
        //        {
        //            for (int b = 0; b < _datatableApp.Rows.Count; b++)
        //            {
        //                if ((_datatableApp.Rows[b]["MGRADE"].ToString() == "NG") && (_datatableApp.Rows[b]["PROPERTIES"].ToString() == "BLACK SPECK"))
        //                {
        //                    main_bsng = true;
        //                }
        //                else if (_datatableApp.Rows[b]["MGRADE"].ToString() == "NG" && _datatableApp.Rows[b]["PROPERTIES"].ToString() == "YI")
        //                {
        //                    main_ying = true;
        //                }
        //            }
        //        }

        //        if (grade != "NG")
        //        {
        //            var grade_app = "";
        //            var bsng = false;
        //            var ying = false;

        //            if(_datatableApp != null && _datatableApp.Rows.Count > 0)
        //            {
        //                for (int c = 0; c < _datatableApp.Rows.Count; c++)
        //                {
        //                    grade_app = grade_app + _datatableApp.Rows[c]["MGRADE"].ToString() + ",";

        //                    if ((_datatableApp.Rows[c]["MGRADE"].ToString() == "NG") && (_datatableApp.Rows[c]["PROPERTIES"].ToString() == "BLACK SPECK"))
        //                    {
        //                        bsng = true;
        //                    }
        //                    else if (_datatableApp.Rows[c]["MGRADE"].ToString() == "NG" && _datatableApp.Rows[c]["PROPERTIES"].ToString() == "YI")
        //                    {
        //                        ying = true;
        //                    }
        //                }
        //            }

        //            var split2 = grade_app.Split(',');

        //            if (split2.Contains("NG"))
        //            {
        //                grade = "NG";
        //            }
        //            else if (split2.Contains("B"))
        //            {
        //                grade = "B";
        //            }
        //            else if (split2.Contains("I"))
        //            {
        //                grade = "I";
        //            }
        //            else if (split2.Contains("AT"))
        //            {
        //                if (grade != "B" && grade != "I")
        //                {
        //                    grade = "AT";
        //                }

        //            }
        //            else if (split2.Contains("AA"))
        //            {
        //                if (grade != "B" && grade != "I" && grade != "AT")
        //                {
        //                    grade = "AA";
        //                }

        //            }
        //            else if (split.Contains("A"))
        //            {
        //                if (grade != "B" && grade != "I" && grade != "AT" && grade != "AA")
        //                {
        //                    grade = "A";
        //                }
        //            }


        //            // Added condition will only run for below:
        //            //bs got ng and yi got ng
        //            //yi got ng and not only 1st data ng
        //            //bs ng and prodline is ce
        //            if (bsng == true && ying == true)
        //            {
        //                status = "Incomplete";
        //            }
        //            else if (bsng == false && ying == true)
        //            {
        //                var temp = await dbdal.Get1stYI(idH);

        //                if (temp != null && temp.Rows.Count > 0)
        //                {
        //                    if (temp.Rows.Count > 0)
        //                    {
        //                        for (int d = 0; d < temp.Rows.Count; d++)
        //                        {
        //                            if (d != 0 && temp.Rows[d]["GRADE"].ToString() == "NG")
        //                            {
        //                                status = "Incomplete";
        //                            }
        //                        }
        //                    }
        //                }

        //            }
        //            else if (bsng == true && ying == false)
        //            {
        //                var prodline = model.LOTNO.Substring(1, 2);
        //                var temp = await dbdal.GetCAPCE(prodline);

        //                if (temp != null && temp.Rows.Count > 0)
        //                {
        //                    if (temp.Rows.Count > 0)
        //                    {
        //                        if (temp.Rows[0][0].ToString() == "CE")
        //                        {
        //                            status = "Incomplete";
        //                        }
        //                    }
        //                }
        //            }

        //            if (main_bsng && main_ying)
        //            {
        //                status = "Completed";
        //                grade = "NG";
        //            }
        //        }
        //        else
        //        {
        //            if (main_bsng && main_ying)
        //            {
        //                if (chkind2 && chkind3)
        //                {
        //                    status = "Incomplete";
        //                }
        //                else
        //                {
        //                    status = "Completed";
        //                    grade = "NG";
        //                }
        //            }
        //        }



        //        //added condition : status = completed if all data shrink down saved even still NG
        //        if (main_bsng || main_ying)
        //        {
        //            var prodline1 = model.LOTNO.Substring(1, 2);
        //            var temp1 = await dbdal.GetCAPCE(prodline1);

        //            if (temp1 != null && temp1.Rows.Count > 0)
        //            {
        //                if (temp1.Rows.Count > 0)
        //                {
        //                    DataTable _chkShrinkDown = new DataTable();

        //                    if (temp1.Rows[0][0].ToString() == "CE")
        //                    {
        //                        _chkShrinkDown = await dbdal.GetDataDLLALL("ChkAppShrink", idH, "1", "", "", "", "");

        //                        if (_chkShrinkDown != null && _chkShrinkDown.Rows.Count > 0)
        //                        {
        //                            status = "Incomplete";
        //                        }
        //                        else
        //                        {
        //                            if (chkind2 && chkind3)
        //                            {
        //                                status = "Incomplete";
        //                            }
        //                            else
        //                            {
        //                                status = "Completed";
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        _chkShrinkDown = await dbdal.GetDataDLLALL("ChkAppShrink", idH, "5", "", "", "", "");

        //                        if (_chkShrinkDown != null && _chkShrinkDown.Rows.Count > 0)
        //                        {
        //                            status = "Incomplete";
        //                        }
        //                        else
        //                        {

        //                            if (chkind2 && chkind3)
        //                            {
        //                                status = "Incomplete";
        //                            }
        //                            else
        //                            {
        //                                status = "Completed";
        //                            }
        //                        }
        //                    }
        //                }
        //            }


        //        }
        //        model.ID_IDS_H = Convert.ToInt32(idH);
        //        //if (model.STATUS == "" || model.STATUS == null)
        //        //{
        //        //    model.STATUS = status;
        //        //}
        //        model.STATUS = status;
        //        model.GRADE = grade;
        //    }

        //    await dbdal.IDS_H_Maint(model, "3");
        //    if (model.STATUS == "Completed")
        //    {
        //        DataTable dt = await GetSumGrd(model.ID_IDS_H.ToString(), model);
        //        if (dt != null && dt.Rows.Count > 0)
        //        {
        //            foreach (DataRow row in dt.Rows)
        //            {
        //                SummaryModel sum = new SummaryModel();

        //                sum.id_ids_h = model.ID_IDS_H.ToString();
        //                sum.grade = row["GRD"].ToString();
        //                sum.tagnofrom = row["MIN"].ToString();
        //                sum.tagnoto = row["MAX"].ToString();
        //                sum.tonform = row["TonFrom"].ToString();
        //                sum.tonto = row["TonTo"].ToString();

        //                await dbdal.IDS_SUM_Maint(sum);
        //            }
        //        }

        //    }
        //}

        //public async Task<DataTable>  GetSumGrd(string idh, DailyIDSTransVM model)
        //{
        //    bool main_moldng = false;
        //    bool main_bsng = false;
        //    bool main_ying = false;
        //    bool skpchk = false;
        //    bool fullyng = false;

        //    DataTable _datatable = await dbdal.GetDataTable(idh, "2", "", "", "");
        //    DataTable _datatableApp = await dbdal.GetDataTable(idh, "5", "", "", "");

        //    if (_datatable != null && _datatable.Rows.Count > 0)
        //    {
        //        for (int ii = 0; ii < _datatable.Rows.Count; ii++)
        //        {
        //            if (_datatable.Rows[ii]["GRADE"].ToString() == "NG")
        //            {
        //                main_moldng = true;
        //            }
        //        }
        //    }

        //    if (_datatableApp != null && _datatableApp.Rows.Count > 0)
        //    {
        //        for (int v = 0; v < _datatableApp.Rows.Count; v++)
        //        {
        //            if (_datatableApp.Rows[v]["MGRADE"].ToString() == "NG" && _datatableApp.Rows[v]["PROPERTIES"].ToString() == "BLACK SPECK")
        //            {
        //                main_bsng = true;
        //            }
        //            else if (_datatableApp.Rows[v]["MGRADE"].ToString() == "NG" && _datatableApp.Rows[v]["PROPERTIES"].ToString() == "YI")
        //            {
        //                main_ying = true;
        //            }
        //        }
        //    }

        //    DataTable _datatableSumGrd = await dbdal.GetDataTable(idh, "8", "", "", "");
        //    DataTable _datatableSumGrdOA = await dbdal.GetDataTable(idh + "-1-3", "8", "", "", "");
        //    DataTable _dtNOMinus10Plus5 = await dbdal.GetDataTable(idh, "9", "", "", "");

        //    DataTable _ChkMold = await dbdal.GetDataDLLALL("ChkEntry", "ChkMold_NG", idh, model.PRODTYPE, "", "", "");

        //    if (_ChkMold != null && _ChkMold.Rows.Count > 0)
        //    {
        //        if (_ChkMold.Rows[0]["res1"].ToString() == "0")
        //        {
        //            _datatableSumGrd = await dbdal.GetDataTable(idh + "-1", "8", "", "", ""); //fully ng
        //            _datatableSumGrdOA = await dbdal.GetDataTable(idh + "-1", "8", "", "", ""); //fully ng
        //            fullyng = true;
        //            skpchk = true;
        //        }
        //        else if (_ChkMold.Rows[0]["res1"].ToString() == "N")
        //        {
        //            //no NG data in molding
        //            //check BSYI
        //            //status = incomplete
        //        }
        //        else
        //        {
        //            string props = "";
        //            if (_ChkMold != null && _ChkMold.Rows.Count > 0)
        //            {
        //                for (int i = 0; i < _ChkMold.Rows.Count; i++)
        //                {
        //                    props += _ChkMold.Rows[0]["res2"].ToString() + ",";
        //                }

        //            }

        //            DataTable _ChkMoldShrink = await dbdal.GetDataDLLALL("ChkEntry", "ChkMold_SDown", idh, props, "", "", "");
        //            if (_ChkMoldShrink != null && _ChkMoldShrink.Rows.Count > 0 && _ChkMoldShrink.Rows[0]["res1"].ToString() == "0")
        //            {
        //                //if already shrink down
        //                //then => check segl all NG then fully NG else check BSYI
        //                if (_ChkMoldShrink.Rows[0]["res1"].ToString() != _ChkMoldShrink.Rows[0]["res2"].ToString())
        //                {
        //                    //not all NG, check BSYI
        //                }
        //                else
        //                {
        //                    //all NG => fully NG
        //                    _datatableSumGrd = await dbdal.GetDataTable(idh + "-1", "8", "", "", ""); //fully ng
        //                    _datatableSumGrdOA = await dbdal.GetDataTable(idh + "-1", "8", "", "", "");//'fully ng
        //                    skpchk = true;
        //                }
        //            }
        //            else
        //            {
        //                //if no yet shrink down then check testtype status = incomplete
        //            }

        //        }
        //    }

        //    if (_datatableSumGrd != null && _datatableSumGrd.Rows.Count > 0)
        //    {
        //        //condition where 50% or more is NG then fully NG
        //        //if (skpchk == false)
        //        //{
        //        //    int ngfrm = 0;
        //        //    int ngto = 0;
        //        //    int hightag = 0;
        //        //    var _fullng = "";
        //        //    bool ngind = false;

        //        //    for (int x = 0; x < _dtNOMinus10Plus5.Rows.Count; x++)
        //        //    {
        //        //        if (_dtNOMinus10Plus5.Rows[x]["GRADE"].ToString().Equals("NG"))
        //        //        {
        //        //            if (ngfrm == 0 && ngto == 0)
        //        //            {

        //        //                ngfrm = Convert.ToInt32(_dtNOMinus10Plus5.Rows[x]["MINTAG"].ToString());
        //        //                ngto = Convert.ToInt32(_dtNOMinus10Plus5.Rows[x]["MAXTAG"].ToString());
        //        //                hightag = Convert.ToInt32(_dtNOMinus10Plus5.Rows[x]["HIGHTAG"].ToString());
        //        //                _fullng = _dtNOMinus10Plus5.Rows[x]["FULLNG"].ToString();
        //        //            }
        //        //            else if (ngfrm > Convert.ToInt32(_dtNOMinus10Plus5.Rows[x]["MINTAG"].ToString()))
        //        //            {
        //        //                ngfrm = Convert.ToInt32(_dtNOMinus10Plus5.Rows[x]["MINTAG"].ToString());
        //        //            }
        //        //            else if (ngto < Convert.ToInt32(_dtNOMinus10Plus5.Rows[x]["MAXTAG"].ToString()))
        //        //            {
        //        //                ngto = Convert.ToInt32(_dtNOMinus10Plus5.Rows[x]["MAXTAG"].ToString());
        //        //            }
        //        //            ngind = true;
        //        //        }
        //        //    }

        //        //    if (ngind)
        //        //    {
        //        //        int ng = ngto - ngfrm;
        //        //        double cpr = ((ng + 1) / (hightag + 1)) * 100;

        //        //        var fullng = _fullng;
        //        //        if (fullng == "")
        //        //        {
        //        //            fullng = "100";
        //        //        }

        //        //        if (cpr > Convert.ToDouble(fullng))
        //        //        {
        //        //            _datatableSumGrd = db2.GetDataTable(idh + "-1", "8", "", "", ""); //'fully ng
        //        //            _datatableSumGrdOA = db2.GetDataTable(idh + "-1", "8", "", "", ""); //'fully ng
        //        //        }
        //        //    }
        //        //}
        //    }

        //    DataTable dt3 = new DataTable();
        //    dt3 = await SumGrdBind(_datatableSumGrdOA, fullyng);
        //    return dt3;
        //}

        //public async Task<DataTable>  SumGrdBind(DataTable _datatableSumGrd, bool fullyng)
        //{
        //    DataTable dt = new DataTable();
        //    DataTable dt2 = new DataTable();
        //    DataRow dr = null;
        //    List<string> fngrow = new List<string>();
        //    List<string> sngrow = new List<string>();
        //    List<string> tngrow = new List<string>();
        //    bool fsegfng = false, ssegfng = false, tsegfng = false, insdone = false, insdone2 = false, ngind = false;
        //    string grdstr = "", nggrd = "NG", notnggrd = "", fmt = "", fntchar = "", tmpmin2 = "", grdcurr = "", grdnex = "", grdprev = "";
        //    int minprev = 0, mincurr = 0, maxprev = 0, maxcurr = 0, minnex = 0, maxnex = 0;
        //    int mintag = 0, maxtag = 0, tonfrom = 0, tonto = 0;
        //    int tmpmin = 0, tmpmax = 0, ngsrt = 0, notngsrt = 0, hightag = 0;
        //    int fngtmpmin = 0, fngtmpmax = 0, sngtmpmin = 0, sngtmpmax = 0, tngtmpmin = 0, tngtmpmax = 0;
        //    int stmpmin = 0, stmpmax = 0, ttmpmin = 0, ttmpmax = 0;

        //    dt.Columns.Add(new DataColumn("GRADE", typeof(string)));
        //    dt.Columns.Add(new DataColumn("GRD", typeof(string)));
        //    dt.Columns.Add(new DataColumn("MIN", typeof(string)));
        //    dt.Columns.Add(new DataColumn("MAX", typeof(string)));
        //    dt.Columns.Add(new DataColumn("SRT", typeof(int)));
        //    dt.Columns.Add(new DataColumn("TonFrom", typeof(string)));
        //    dt.Columns.Add(new DataColumn("TonTo", typeof(string)));

        //    dt2.Columns.Add(new DataColumn("GRADE", typeof(string)));
        //    dt2.Columns.Add(new DataColumn("GRD", typeof(string)));
        //    dt2.Columns.Add(new DataColumn("MIN", typeof(string)));
        //    dt2.Columns.Add(new DataColumn("MAX", typeof(string)));
        //    dt2.Columns.Add(new DataColumn("SRT", typeof(int)));
        //    dt2.Columns.Add(new DataColumn("TonFrom", typeof(string)));
        //    dt2.Columns.Add(new DataColumn("TonTo", typeof(string)));

        //    if (fullyng)
        //    {

        //        dr = dt.NewRow();
        //        dr["GRD"] = _datatableSumGrd.Rows[0]["grade"].ToString();
        //        dr["MIN"] = _datatableSumGrd.Rows[0]["TAGNOCHAR"].ToString() + _datatableSumGrd.Rows[0]["mintag"].ToString();
        //        dr["MAX"] = _datatableSumGrd.Rows[0]["TAGNOCHAR"].ToString() + _datatableSumGrd.Rows[0]["maxtag"].ToString();
        //        dr["SRT"] = _datatableSumGrd.Rows[0]["srt"].ToString();
        //        dr["TonFrom"] = _datatableSumGrd.Rows[0]["tonfrom"].ToString();
        //        dr["TonTo"] = _datatableSumGrd.Rows[0]["tonto"].ToString();
        //        dt.Rows.Add(dr);

        //        var dv = dt.DefaultView;
        //        dv.Sort = "MIN";
        //        dt2 = dv.ToTable();

        //        return dt2;
        //    }
        //    else
        //    {
        //        try
        //        {
        //            hightag = Convert.ToInt32(_datatableSumGrd.Rows[0]["hightag"].ToString()) + 1;

        //            if (_datatableSumGrd != null && _datatableSumGrd.Rows.Count > 0)
        //            {
        //                for (var v = 0; v <= _datatableSumGrd.Rows.Count - 1; v++) // record first and last row in each segment
        //                {

        //                    if (_datatableSumGrd.Rows[v]["GRADE"].ToString() == "NG")
        //                    {
        //                        ngind = true;
        //                        ngsrt = Convert.ToInt32(_datatableSumGrd.Rows[v]["SRT"].ToString());
        //                    }
        //                    else
        //                    {
        //                        notngsrt = Convert.ToInt32(_datatableSumGrd.Rows[v]["SRT"].ToString());
        //                        notnggrd = _datatableSumGrd.Rows[v]["GRADE"].ToString();
        //                    }

        //                    if (_datatableSumGrd.Rows[v]["grade"].ToString() == "NG")
        //                    {
        //                        if (Convert.ToInt32(_datatableSumGrd.Rows[v]["tonfrom"].ToString()) >= 1 && fngtmpmin == 0)
        //                        {
        //                            fngrow.Add(v.ToString());
        //                            fngtmpmin = Convert.ToInt32(_datatableSumGrd.Rows[v]["tonfrom"].ToString());
        //                            fngtmpmax = Convert.ToInt32(_datatableSumGrd.Rows[v]["tonto"].ToString());
        //                            if (Convert.ToInt32(fngtmpmin) != 1)
        //                                fngtmpmin += 5;
        //                            else if (Convert.ToInt32(fngtmpmin) == 1 && Convert.ToInt32(fngtmpmax) == 15)
        //                                fngtmpmin += 5;
        //                        }
        //                        if (Convert.ToInt32(_datatableSumGrd.Rows[v]["tonto"].ToString()) <= 45)
        //                        {
        //                            fngrow.Add(v.ToString());
        //                            fngtmpmax = Convert.ToInt32(_datatableSumGrd.Rows[v]["tonto"].ToString());
        //                            if (Convert.ToInt32(fngtmpmax) != Convert.ToInt32(hightag))
        //                                fngtmpmax = fngtmpmax - 5;
        //                        }
        //                        if (Convert.ToInt32(_datatableSumGrd.Rows[v]["tonfrom"].ToString()) >= 36 && Convert.ToInt32(_datatableSumGrd.Rows[v]["tonfrom"].ToString()) < 76 && sngtmpmin == 0)
        //                            // if in second segment full ng range then min + 5
        //                            sngtmpmin = Convert.ToInt32(_datatableSumGrd.Rows[v]["tonfrom"].ToString()) + 5;
        //                        if (Convert.ToInt32(_datatableSumGrd.Rows[v]["tonto"].ToString()) > 45 && Convert.ToInt32(_datatableSumGrd.Rows[v]["tonto"].ToString()) <= 85)
        //                        {
        //                            sngrow.Add(v.ToString());
        //                            sngtmpmax = Convert.ToInt32(_datatableSumGrd.Rows[v]["tonto"].ToString());
        //                            if (Convert.ToInt32(sngtmpmax) != Convert.ToInt32(hightag) || Convert.ToInt32(_datatableSumGrd.Rows[v]["tonto"].ToString()) - Convert.ToInt32(_datatableSumGrd.Rows[v]["tonfrom"].ToString()) > 10)
        //                                sngtmpmax = sngtmpmax - 5;
        //                        }
        //                        if (Convert.ToInt32(_datatableSumGrd.Rows[v]["tonfrom"].ToString()) >= 76 && tngtmpmin == 0)
        //                            tngtmpmin = Convert.ToInt32(_datatableSumGrd.Rows[v]["tonfrom"].ToString()) + 5;
        //                        if (Convert.ToInt32(_datatableSumGrd.Rows[v]["tonto"].ToString()) > 85 && Convert.ToInt32(_datatableSumGrd.Rows[v]["tonto"].ToString()) <= 120)
        //                        {
        //                            tngrow.Add(v.ToString());
        //                            tngtmpmax = Convert.ToInt32(_datatableSumGrd.Rows[v]["tonto"].ToString());
        //                            if (tngtmpmax != hightag || Convert.ToInt32(_datatableSumGrd.Rows[v]["tonto"].ToString()) - Convert.ToInt32(_datatableSumGrd.Rows[v]["tonfrom"].ToString()) > 10)
        //                                tngtmpmax = tngtmpmax - 5;
        //                        }
        //                    }
        //                }
        //            }


        //            if (ngind)
        //            {
        //                if (hightag <= 40)
        //                {
        //                    if (hightag % 40 != 0)
        //                    {
        //                        if (fngtmpmax != 0 && fngtmpmin != 0)
        //                        {
        //                            if (fngtmpmax - fngtmpmin + 1 > (hightag % 40) / 2)
        //                                fsegfng = true;
        //                        }
        //                    }
        //                    else if (fngtmpmax != 0 && fngtmpmin != 0)
        //                    {
        //                        if (Convert.ToInt32(fngtmpmax) - Convert.ToInt32(fngtmpmin) + 1 > 20)
        //                            fsegfng = true;
        //                    }
        //                }
        //                else if (hightag > 40 && hightag <= 80)
        //                {
        //                    if (hightag % 40 != 0)
        //                    {
        //                        if (fngtmpmax != 0 && fngtmpmin != 0)
        //                        {
        //                            if (fngtmpmax - fngtmpmin + 1 > 20)
        //                                fsegfng = true;
        //                        }
        //                        if (sngtmpmax != 0 && sngtmpmin != 0)
        //                        {
        //                            if (sngtmpmax - sngtmpmin + 1 > (hightag % 40) / 2)
        //                                ssegfng = true;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (fngtmpmax != 0 && fngtmpmin != 0)
        //                        {
        //                            if (fngtmpmax - fngtmpmin + 1 > 20)
        //                                fsegfng = true;
        //                        }
        //                        if (sngtmpmax != 0 && sngtmpmin != 0)
        //                        {
        //                            if (sngtmpmax - sngtmpmin + 1 > 20)
        //                                ssegfng = true;
        //                        }
        //                    }
        //                }
        //                else if (hightag > 80)
        //                {
        //                    if (hightag % 40 != 0)
        //                    {
        //                        if (fngtmpmax != 0 && fngtmpmin != 0)
        //                        {
        //                            if (fngtmpmax - fngtmpmin + 1 > 20)
        //                                fsegfng = true;
        //                        }
        //                        if (sngtmpmax != 0 && sngtmpmin != 0)
        //                        {
        //                            if (sngtmpmax - sngtmpmin + 1 > 20)
        //                                ssegfng = true;
        //                        }
        //                        if (tngtmpmax != 0 && tngtmpmin != 0)
        //                        {
        //                            if (tngtmpmax - tngtmpmin + 1 > (hightag % 40) / 2)
        //                                tsegfng = true;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (fngtmpmax != 0 && fngtmpmin != 0)
        //                        {
        //                            if (fngtmpmax - fngtmpmin + 1 > 20)
        //                                fsegfng = true;
        //                        }
        //                        if (sngtmpmax != 0 && sngtmpmin != 0)
        //                        {
        //                            if (sngtmpmax - sngtmpmin + 1 > 20)
        //                                ssegfng = true;
        //                        }
        //                        if (tngtmpmax != 0 && tngtmpmin != 0)
        //                        {
        //                            if (tngtmpmax - tngtmpmin + 1 > 20)
        //                                tsegfng = true;
        //                        }
        //                    }
        //                }
        //            }

        //            mintag = Convert.ToInt32(_datatableSumGrd.Rows[0]["MINITAG"].ToString());
        //            maxtag = Convert.ToInt32(_datatableSumGrd.Rows[0]["MAXITAG"].ToString());

        //            if (fsegfng && ssegfng && tsegfng || (hightag <= 40 && fsegfng) || (hightag > 40 && hightag <= 80 && fsegfng && ssegfng)) //all segment full ng
        //            {
        //                fntchar = _datatableSumGrd.Rows[0]["TAGNOCHAR"].ToString();
        //                for (var fmti = 1; fmti <= Convert.ToInt32(_datatableSumGrd.Rows[0]["LENTAGNO"].ToString()); fmti++) fmt = fmt + "0";
        //                tonfrom = mintag - mintag + 1;
        //                tonto = maxtag - mintag + 1;

        //                dr = dt.NewRow();
        //                //grdstr = "NG" + " = " + fntchar + mintag.ToString(fmt) + " - " + fntchar + maxtag.ToString(fmt) + " " + tonfrom + " " + tonto;
        //                //dr["GRADE"] = grdstr;
        //                dr["GRD"] = "NG";
        //                dr["MIN"] = fntchar + mintag.ToString(fmt);
        //                dr["MAX"] = fntchar + maxtag.ToString(fmt);
        //                dr["SRT"] = ngsrt;
        //                dr["TonFrom"] = tonfrom;
        //                dr["TonTo"] = tonto;
        //                dt.Rows.Add(dr);
        //            }
        //            else if (fsegfng || ssegfng || tsegfng) //any segment full ng
        //            {
        //                if (fsegfng)
        //                {
        //                    tmpmin = mintag;

        //                    if (Convert.ToInt32(_datatableSumGrd.Rows[Convert.ToInt32(fngrow[fngrow.Count - 1])]["tonto"].ToString()) > 40)
        //                        tmpmax = tmpmin + 44;
        //                    else if (hightag == 40)
        //                        tmpmax = tmpmin + 39;
        //                    //else if (hightag < 40)
        //                    //    tmpmax = tmpmin + hightag - 1;
        //                    else
        //                        tmpmax = tmpmin + 39;
        //                }
        //                if (ssegfng)
        //                {
        //                    if (Convert.ToInt32(_datatableSumGrd.Rows[Convert.ToInt32(sngrow[0])]["tonfrom"].ToString()) < 41)
        //                        stmpmin = Convert.ToInt32(mintag) + 35;
        //                    else
        //                        stmpmin = Convert.ToInt32(mintag) + 40;

        //                    if (Convert.ToInt32(_datatableSumGrd.Rows[Convert.ToInt32(sngrow[sngrow.Count - 1])]["tonto"].ToString()) > 80)
        //                        stmpmax = Convert.ToInt32(mintag) + 84;
        //                    else if (hightag == 80)
        //                        stmpmax = mintag + 79;
        //                    else
        //                        //stmpmax = mintag + hightag - 1;
        //                        stmpmax = mintag + 79;
        //                }
        //                if (tsegfng)
        //                {
        //                    if (Convert.ToInt32(_datatableSumGrd.Rows[Convert.ToInt32(tngrow[0])]["tonfrom"].ToString()) < 81)
        //                        ttmpmin = Convert.ToInt32(mintag) + 75;
        //                    else
        //                    {
        //                        ttmpmin = Convert.ToInt32(mintag) + 80;
        //                        ttmpmax = Convert.ToInt32(_datatableSumGrd.Rows[Convert.ToInt32(tngrow[tngrow.Count - 1])]["MAXITAG"].ToString());
        //                    }
        //                }

        //                if (_datatableSumGrd != null && _datatableSumGrd.Rows.Count > 0)
        //                {
        //                    for (var v = 0; v <= _datatableSumGrd.Rows.Count - 1; v++)
        //                    {
        //                        if (v == 0)
        //                        {
        //                            fntchar = _datatableSumGrd.Rows[0]["TAGNOCHAR"].ToString();
        //                            for (var fmti = 1; fmti <= Convert.ToInt32(_datatableSumGrd.Rows[0]["LENTAGNO"].ToString()); fmti++)
        //                                fmt = fmt + "0";
        //                        }
        //                        else if (v != 0)
        //                        {
        //                            minprev = Convert.ToInt32(_datatableSumGrd.Rows[v - 1]["MINTAG"].ToString());
        //                            maxprev = Convert.ToInt32(_datatableSumGrd.Rows[v - 1]["MAXTAG"].ToString());
        //                            grdprev = _datatableSumGrd.Rows[v - 1]["GRADE"].ToString();
        //                        }

        //                        mincurr = Convert.ToInt32(_datatableSumGrd.Rows[v]["MINTAG"].ToString());
        //                        maxcurr = Convert.ToInt32(_datatableSumGrd.Rows[v]["MAXTAG"].ToString());
        //                        grdcurr = _datatableSumGrd.Rows[v]["GRADE"].ToString();

        //                        if (_datatableSumGrd != null && _datatableSumGrd.Rows.Count > 0)
        //                        {
        //                            if (v < _datatableSumGrd.Rows.Count - 1)
        //                            {
        //                                minnex = Convert.ToInt32(_datatableSumGrd.Rows[v + 1]["MINTAG"].ToString());
        //                                maxnex = Convert.ToInt32(_datatableSumGrd.Rows[v + 1]["MAXTAG"].ToString());
        //                                grdnex = _datatableSumGrd.Rows[v + 1]["GRADE"].ToString();
        //                            }
        //                        }


        //                        if (fsegfng && insdone == false) //11
        //                        {
        //                            if (v < Convert.ToInt32(fngrow[fngrow.Count - 1])) continue;

        //                            string[] tmpstr;
        //                            if (v != _datatableSumGrd.Rows.Count - 1 && (minnex < tmpmax || minnex - tmpmax == 1)) //if next row can be combine record mincurr and skip
        //                            {
        //                                tmpmin2 = tmpmin2 + "," + tmpmin; // tmpmin stored first ton
        //                                continue;
        //                            }
        //                            tmpstr = tmpmin2.Split(',');
        //                            int min = 0, max = 0;
        //                            dr = dt.NewRow();
        //                            if (tmpstr.Length == 1) //if any skip occur befor
        //                            {
        //                                grdstr = nggrd + " = " + fntchar + tmpmin.ToString(fmt) + " - " + fntchar + tmpmax.ToString(fmt) + " " + (tmpmin - tmpmin + 1).ToString() + " " + (tmpmax - tmpmin + 1).ToString();
        //                                min = tmpmin;
        //                                max = tmpmax;
        //                                tonfrom = tmpmin - tmpmin + 1;
        //                                tonto = tmpmax - tmpmin + 1;
        //                                tmpmin = tmpmax + 1;
        //                            }
        //                            else //no skip occur before
        //                            {
        //                                grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(tmpstr[1]).ToString(fmt) + " - " + fntchar + maxcurr.ToString(fmt) + " " + (Convert.ToInt32(tmpstr[1]) - mintag + 1).ToString() + " " + (maxcurr - mintag + 1).ToString();
        //                                min = Convert.ToInt32(tmpstr[1]);
        //                                max = maxcurr;
        //                                tonfrom = Convert.ToInt32(tmpstr[1]) - mintag + 1;
        //                                tonto = maxcurr - mintag + 1;
        //                                tmpmin = maxcurr + 1;
        //                                tmpmin2 = "";
        //                            }
        //                            //dr["GRADE"] = grdstr;
        //                            dr["GRD"] = nggrd;
        //                            dr["MIN"] = fntchar + Convert.ToInt32(min).ToString(fmt);
        //                            dr["MAX"] = fntchar + Convert.ToInt32(max).ToString(fmt);
        //                            dr["SRT"] = ngsrt;
        //                            dr["TonFrom"] = tonfrom;
        //                            dr["TonTo"] = tonto;
        //                            dt.Rows.Add(dr);

        //                            if (v == Convert.ToInt32(fngrow[fngrow.Count - 1]) && v == _datatableSumGrd.Rows.Count - 1) //if only first segment full ng and the only ng
        //                            {
        //                                if (dt.Rows[dt.Rows.Count - 1]["MAX"].ToString() == tmpmax.ToString())
        //                                {
        //                                    dr = dt.NewRow();
        //                                    //grdstr = notnggrd + " = " + fntchar + Convert.ToInt32(tmpmin).ToString(fmt) + " - " + fntchar + maxtag.ToString(fmt) + " " + (Convert.ToInt32(tmpmin) - mintag + 1).ToString() + " " + (maxtag - mintag + 1).ToString();
        //                                    //dr["GRADE"] = grdstr;
        //                                    dr["GRD"] = notnggrd;
        //                                    dr["MIN"] = fntchar + (Convert.ToInt32(dt.Rows[dt.Rows.Count - 1]["MAX"].ToString()) + 1).ToString(fmt);
        //                                    dr["MAX"] = fntchar + Convert.ToInt32(maxtag).ToString(fmt);
        //                                    dr["SRT"] = notngsrt;
        //                                    dr["TonFrom"] = (Convert.ToInt32(dt.Rows[dt.Rows.Count - 1]["MAX"].ToString()) + 1) - mintag + 1;
        //                                    dr["TonTo"] = maxtag - mintag + 1;
        //                                    dt.Rows.Add(dr);
        //                                    continue;
        //                                }
        //                                dr = dt.NewRow();
        //                                //grdstr = notnggrd + " = " + fntchar + Convert.ToInt32(tmpmin).ToString(fmt) + " - " + fntchar + maxtag.ToString(fmt) + " " + (Convert.ToInt32(tmpmin) - mintag + 1).ToString() + " " + (maxtag - mintag + 1).ToString();
        //                                //dr["GRADE"] = grdstr;
        //                                dr["GRD"] = notnggrd;
        //                                dr["MIN"] = fntchar + Convert.ToInt32(tmpmin).ToString(fmt);
        //                                dr["MAX"] = fntchar + Convert.ToInt32(maxtag).ToString(fmt);
        //                                dr["SRT"] = notngsrt;
        //                                dr["TonFrom"] = tmpmin - mintag + 1;
        //                                dr["TonTo"] = maxtag - mintag + 1;
        //                                dt.Rows.Add(dr);
        //                                continue;
        //                            }
        //                            else if (v == _datatableSumGrd.Rows.Count - 1) //first segment not full ng but ng only in first segment
        //                            {
        //                                dr = dt.NewRow();
        //                                //grdstr = notnggrd + " = " + fntchar + Convert.ToInt32(max + 1).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxtag).ToString(fmt) + " " + (max + 1 - mintag + 1).ToString() + " " + (maxtag - mintag + 1).ToString();
        //                                //dr["GRADE"] = grdstr;
        //                                dr["GRD"] = notnggrd;
        //                                dr["MIN"] = fntchar + Convert.ToInt32(max + 1).ToString(fmt);
        //                                dr["MAX"] = fntchar + Convert.ToInt32(maxtag).ToString(fmt);
        //                                dr["SRT"] = notngsrt;
        //                                dr["TonFrom"] = max + 1 - mintag + 1;
        //                                dr["TonTo"] = maxtag - mintag + 1;
        //                                dt.Rows.Add(dr);
        //                                continue;
        //                            }

        //                            if ((minnex > tmpmax && minnex - tmpmax > 1)) //next row cannot combine
        //                            {
        //                                tmpmax = Convert.ToInt32(minnex) - 1;
        //                                dr = dt.NewRow();
        //                                //grdstr = notnggrd + " = " + fntchar + Convert.ToInt32(tmpmin).ToString(fmt) + " - " + fntchar + Convert.ToInt32(tmpmax).ToString(fmt) + " " + (tmpmin - mintag + 1).ToString() + " " + (tmpmax - mintag + 1).ToString();
        //                                //dr["GRADE"] = grdstr;
        //                                dr["GRD"] = notnggrd;
        //                                dr["MIN"] = fntchar + Convert.ToInt32(tmpmin).ToString(fmt);
        //                                dr["MAX"] = fntchar + Convert.ToInt32(tmpmax).ToString(fmt);
        //                                dr["SRT"] = notngsrt;
        //                                dr["TonFrom"] = tmpmin - mintag + 1;
        //                                dr["TonTo"] = tmpmax - mintag + 1;
        //                                dt.Rows.Add(dr);
        //                            }

        //                            if (minnex - mintag + 1 >= 76) //next ng is in third segment
        //                            {
        //                                insdone = true;
        //                                insdone2 = true;
        //                            }
        //                            else if (minnex - mintag + 1 >= 36) //next ng is in second segment
        //                                insdone = true;
        //                            continue;
        //                        }
        //                        else if (fsegfng == false && insdone == false) //10
        //                        {
        //                            if (hightag <= 40)
        //                            {
        //                                if (v == 0)
        //                                {
        //                                    if (grdcurr != nggrd)
        //                                    {
        //                                        tmpmin = mincurr;
        //                                        string[] tmpstr;
        //                                        tmpstr = tmpmin2.Split(',');
        //                                        if (tmpstr.Count() == 1) continue;
        //                                    }
        //                                    else if (grdcurr == nggrd)
        //                                    {
        //                                        if (grdnex == nggrd && (minnex < maxcurr || minnex - maxcurr == 1)) //next row can be combine
        //                                        {
        //                                            tmpmin2 = tmpmin2 + "," + mincurr;
        //                                            if (minnex - mintag + 1 < 36) continue;
        //                                        }
        //                                        tmpmin = maxcurr + 1;
        //                                        dr = dt.NewRow();
        //                                        //grdstr = _datatableSumGrd.Rows[v]["GRADE"].ToString() + " = " + fntchar + Convert.ToInt32(mincurr).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + (mincurr - mintag + 1).ToString() + " " + (maxcurr - mintag + 1).ToString();
        //                                        //dr["GRADE"] = grdstr;
        //                                        dr["GRD"] = _datatableSumGrd.Rows[v]["GRADE"].ToString();
        //                                        dr["MIN"] = fntchar + Convert.ToInt32(mincurr).ToString(fmt);
        //                                        dr["MAX"] = fntchar + Convert.ToInt32(maxcurr).ToString(fmt);
        //                                        dr["SRT"] = ngsrt;
        //                                        dr["TonFrom"] = mincurr - mintag + 1;
        //                                        dr["TonTo"] = maxcurr - mintag + 1;
        //                                        dt.Rows.Add(dr);
        //                                    }
        //                                }
        //                                else if (v != _datatableSumGrd.Rows.Count - 1)
        //                                {
        //                                    if (grdnex != nggrd)
        //                                    {
        //                                        string[] tmpstr;
        //                                        tmpstr = tmpmin2.Split(',');
        //                                        if (tmpstr.Count() == 1) continue;
        //                                    }
        //                                    else if (grdcurr != nggrd && grdnex == nggrd)
        //                                    {
        //                                        tmpmax = minnex - 1;
        //                                        dr = dt.NewRow();
        //                                        if (minnex > tmpmin - 1 && minnex - tmpmin - 1 > 1)
        //                                        {
        //                                            //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(tmpmin).ToString(fmt) + " - " + fntchar + Convert.ToInt32(tmpmax).ToString(fmt) + " " + (tmpmin - mintag + 1).ToString() + " " + (tmpmax - mintag + 1).ToString();
        //                                            //dr["GRADE"] = grdstr;
        //                                            dr["GRD"] = grdcurr;
        //                                            dr["MIN"] = fntchar + Convert.ToInt32(tmpmin).ToString(fmt);
        //                                            dr["MAX"] = fntchar + Convert.ToInt32(tmpmax).ToString(fmt);
        //                                            dr["SRT"] = notngsrt;
        //                                            dr["TonFrom"] = tmpmin - mintag + 1;
        //                                            dr["TonTo"] = tmpmax - mintag + 1;
        //                                            dt.Rows.Add(dr);
        //                                        }
        //                                        else
        //                                        {
        //                                            var r = dt.Rows.Count - 1;
        //                                            //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["TonTo"].ToString(), (maxnex - mintag + 1).ToString());
        //                                            //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["Max"].ToString(), maxnex.ToString());
        //                                            dt.Rows[r]["MAX"] = maxnex;
        //                                            dt.Rows[r]["TonTo"] = maxnex - mintag + 1;
        //                                        }
        //                                    }
        //                                    else if (grdcurr == nggrd && grdnex == nggrd)
        //                                    {
        //                                        if (grdnex == nggrd && (minnex < maxcurr || minnex - maxcurr == 1))
        //                                        {
        //                                            tmpmin2 = tmpmin2 + "," + mincurr;
        //                                            if (minnex - mintag + 1 < 36) continue;
        //                                        }
        //                                        tmpmax = minnex - 1;
        //                                        tmpmin = maxcurr + 1;
        //                                        string[] tmpstr;
        //                                        if (minnex < maxcurr || (minnex - maxcurr == 1 && minnex - maxcurr > 0) && minnex - mintag + 1 < 76)
        //                                        {
        //                                            tmpmin2 = tmpmin2 + "," + mincurr;
        //                                            continue;
        //                                        }
        //                                        tmpstr = tmpmin2.Split(',');
        //                                        var r = dt.Rows.Count - 1;
        //                                        if (Convert.ToInt32(dt.Rows[r]["MAX"]) != maxcurr && mincurr > Convert.ToInt32(dt.Rows[r]["MAX"])) //last table row cannot combine
        //                                        {
        //                                            var min = 0;
        //                                            dr = dt.NewRow();
        //                                            if (minnex > maxcurr && tmpstr.Length == 1)
        //                                            {
        //                                                //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(mincurr).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + (mincurr - mintag + 1).ToString() + " " + (maxcurr - mintag + 1).ToString();
        //                                                min = mincurr;
        //                                                tonfrom = mincurr - mintag + 1;
        //                                                tonto = maxcurr - mintag + 1;
        //                                            }
        //                                            else
        //                                            {
        //                                                //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(tmpstr[1]).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + (Convert.ToInt32(tmpstr[1]) - mintag + 1).ToString() + " " + (maxcurr - mintag + 1).ToString();
        //                                                min = Convert.ToInt32(tmpstr[1]);
        //                                                tonfrom = Convert.ToInt32(tmpstr[1]) - mintag + 1;
        //                                                tonto = maxcurr - mintag + 1;
        //                                                tmpmin2 = "";
        //                                            }
        //                                            //dr["GRADE"] = grdstr;
        //                                            dr["GRD"] = grdcurr;
        //                                            dr["MIN"] = min;
        //                                            dr["MAX"] = fntchar + Convert.ToInt32(maxcurr).ToString(fmt);
        //                                            dr["SRT"] = _datatableSumGrd.Rows[v]["SRT"].ToString();
        //                                            dr["TonFrom"] = tonfrom;
        //                                            dr["TonTo"] = tonto;
        //                                            dt.Rows.Add(dr);
        //                                            tmpmin2 = "";
        //                                        }
        //                                        else if (Convert.ToInt32(dt.Rows[r]["MAX"]) != maxcurr && mincurr < Convert.ToInt32(dt.Rows[r]["MAX"])) //last table row can combine
        //                                        {
        //                                            //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["TonTo"].ToString(), (maxcurr - mintag + 1).ToString());
        //                                            //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["Max"].ToString(), maxcurr.ToString());
        //                                            dt.Rows[r]["MAX"] = maxcurr;
        //                                            dt.Rows[r]["TonTo"] = maxcurr - mintag + 1;
        //                                            tmpmin2 = "";
        //                                        }
        //                                        if (minnex - maxcurr > 1)
        //                                        {
        //                                            dr = dt.NewRow();
        //                                            //grdstr = notnggrd + " = " + fntchar + Convert.ToInt32(tmpmin).ToString(fmt) + " - " + fntchar + Convert.ToInt32(tmpmax).ToString(fmt) + " " + (tmpmin - mintag + 1).ToString() + " " + (tmpmax - mintag + 1).ToString();
        //                                            //dr["GRADE"] = grdstr;
        //                                            dr["GRD"] = notnggrd;
        //                                            dr["MIN"] = fntchar + Convert.ToInt32(tmpmin).ToString(fmt);
        //                                            dr["MAX"] = fntchar + Convert.ToInt32(tmpmax).ToString(fmt);
        //                                            dr["SRT"] = notngsrt;
        //                                            dr["TonFrom"] = tmpmin - mintag + 1;
        //                                            dr["TonTo"] = tmpmax - mintag + 1;
        //                                            dt.Rows.Add(dr);
        //                                        }
        //                                    }
        //                                    if (grdcurr == nggrd && grdnex != nggrd)
        //                                    {
        //                                        string[] tmpstr;

        //                                        tmpstr = tmpmin2.Split(',');
        //                                        dr = dt.NewRow();
        //                                        //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(tmpstr[1]).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + (Convert.ToInt32(tmpstr[1]) - mintag + 1).ToString() + " " + (maxcurr - mintag + 1).ToString();
        //                                        tonfrom = Convert.ToInt32(tmpstr[1]) - mintag + 1;
        //                                        tonto = maxcurr - mintag + 1;
        //                                        //dr["GRADE"] = grdstr;
        //                                        dr["GRD"] = grdcurr;
        //                                        dr["MIN"] = fntchar + Convert.ToInt32(tmpstr[1]).ToString(fmt);
        //                                        dr["MAX"] = fntchar + Convert.ToInt32(maxcurr).ToString(fmt);
        //                                        dr["SRT"] = _datatableSumGrd.Rows[v]["SRT"].ToString();
        //                                        dr["TonFrom"] = tonfrom;
        //                                        dr["TonTo"] = tonto;
        //                                        dt.Rows.Add(dr);
        //                                        tmpmin = maxcurr + 1;
        //                                        tmpmin2 = "";
        //                                    }
        //                                }
        //                                else if (v == _datatableSumGrd.Rows.Count - 1 && maxcurr != maxtag) //last row not maxtag
        //                                {
        //                                    dr = dt.NewRow();
        //                                    //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(mincurr).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + (mincurr - mintag + 1).ToString() + " " + (maxcurr - mintag + 1).ToString();
        //                                    //dr["GRADE"] = grdstr;
        //                                    dr["GRD"] = grdcurr;
        //                                    dr["MIN"] = fntchar + Convert.ToInt32(mincurr).ToString(fmt);
        //                                    dr["MAX"] = fntchar + Convert.ToInt32(maxcurr).ToString(fmt);
        //                                    dr["SRT"] = _datatableSumGrd.Rows[v]["SRT"].ToString();
        //                                    dr["TonFrom"] = mincurr - mintag + 1;
        //                                    dr["TonTo"] = maxcurr - mintag + 1;
        //                                    dt.Rows.Add(dr);
        //                                    dr = dt.NewRow();
        //                                    //grdstr = notnggrd + " = " + fntchar + Convert.ToInt32(maxcurr + 1).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxtag).ToString(fmt) + " " + (maxcurr + 1 - mintag + 1).ToString() + " " + (maxtag - mintag + 1).ToString();
        //                                    //dr["GRADE"] = grdstr;
        //                                    dr["GRD"] = _datatableSumGrd.Rows[v - 1]["GRADE"].ToString();
        //                                    dr["MIN"] = fntchar + Convert.ToInt32(maxcurr + 1).ToString(fmt);
        //                                    dr["MAX"] = fntchar + Convert.ToInt32(maxtag).ToString(fmt);
        //                                    dr["SRT"] = notngsrt;
        //                                    dr["TonFrom"] = maxcurr - mintag + 1;
        //                                    dr["TonTo"] = maxtag - mintag + 1;
        //                                    dt.Rows.Add(dr);
        //                                }
        //                                else if (v == _datatableSumGrd.Rows.Count - 1 && maxcurr == maxtag) //last row max tag
        //                                {
        //                                    var r = dt.Rows.Count - 1;
        //                                    if (dt.Rows[dt.Rows.Count - 1]["GRD"].ToString() == notnggrd) //last table row is cannot combine
        //                                    {
        //                                        dr = dt.NewRow();
        //                                        //grdstr = _datatableSumGrd.Rows[v]["GRADE"].ToString() + " = " + fntchar + Convert.ToInt32(ttmpmin).ToString(fmt) + " - " + fntchar + Convert.ToInt32(ttmpmax).ToString(fmt) + " " + (ttmpmin - mintag + 1).ToString() + " " + (ttmpmax - mintag + 1).ToString();
        //                                        //dr["GRADE"] = grdstr;
        //                                        dr["GRD"] = _datatableSumGrd.Rows[v]["GRADE"].ToString();
        //                                        dr["MIN"] = fntchar + Convert.ToInt32(ttmpmin).ToString(fmt);
        //                                        dr["MAX"] = fntchar + Convert.ToInt32(ttmpmax).ToString(fmt);
        //                                        dr["SRT"] = ngsrt;
        //                                        dr["TonFrom"] = ttmpmin - mintag + 1;
        //                                        dr["TonTo"] = ttmpmax - mintag + 1;
        //                                        dt.Rows.Add(dr);
        //                                    }
        //                                    else //last table row can combine
        //                                    {
        //                                        //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["TonTo"].ToString(), (ttmpmin - 1 - mintag + 1).ToString());
        //                                        //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["Max"].ToString(), (ttmpmin - 1).ToString());
        //                                        dt.Rows[r]["MAX"] = ttmpmin - 1;
        //                                        dt.Rows[r]["TonTo"] = ttmpmin - 1 - mintag + 1;
        //                                    }
        //                                }
        //                            }
        //                            else
        //                                insdone = true;
        //                        }

        //                        if (hightag > 40)
        //                        {
        //                            if (ssegfng == false && fsegfng && insdone2 == false) //1120
        //                            {
        //                                if (hightag > 40 && hightag <= 80 || tsegfng)
        //                                {
        //                                    if (grdcurr == nggrd && grdnex == nggrd)
        //                                    {
        //                                        tmpmax = minnex - 1;
        //                                        tmpmin = maxcurr + 1;
        //                                        string[] tmpstr;
        //                                        if (v != _datatableSumGrd.Rows.Count - 1 && minnex - mintag + 1 < 76 && (minnex < maxcurr || minnex - maxcurr == 1))
        //                                        {
        //                                            tmpmin2 = tmpmin2 + "," + mincurr;
        //                                            continue;
        //                                        }
        //                                        else if (minnex - mintag + 1 >= 76 && (minnex < maxcurr || minnex - maxcurr == 1)) tmpmin2 = tmpmin2 + "," + mincurr;

        //                                        tmpstr = tmpmin2.Split(',');
        //                                        var r = dt.Rows.Count - 1;
        //                                        int min = 0;
        //                                        if (dt.Rows[r]["GRD"].ToString() == notnggrd && Convert.ToInt32(dt.Rows[r]["MIN"].ToString()) >= mincurr)
        //                                        {
        //                                            dt.Rows[r].Delete();
        //                                            dt.AcceptChanges();
        //                                            r = dt.Rows.Count - 1;
        //                                        }
        //                                        else if (dt.Rows[r]["GRD"].ToString() == notnggrd && (Convert.ToInt32(dt.Rows[r]["MIN"]) < mincurr && Convert.ToInt32(dt.Rows[r]["MAX"]) > mincurr || mincurr - Convert.ToInt32(dt.Rows[r]["MAX"].ToString()) == 1))
        //                                        {
        //                                            //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["TonTo"].ToString(), (mincurr - 1 - mintag + 1).ToString());
        //                                            //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["Max"].ToString(), (mincurr - 1).ToString());
        //                                            dt.Rows[r]["MAX"] = mincurr - 1;
        //                                            dt.Rows[r]["TonTo"] = mincurr - 1 - mintag + 1;
        //                                        }
        //                                        // if last row can combine
        //                                        if (dt.Rows[r]["GRD"].ToString() == nggrd && (Convert.ToInt32(dt.Rows[r]["MAX"].ToString()) > mincurr || mincurr - Convert.ToInt32(dt.Rows[r]["MAX"].ToString()) == 1))
        //                                        {
        //                                            //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["TonTo"].ToString(), (maxcurr - mintag + 1).ToString());
        //                                            //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["Max"].ToString(), maxcurr.ToString());
        //                                            dt.Rows[r]["MAX"] = maxcurr;
        //                                            dt.Rows[r]["TonTo"] = maxcurr - mintag + 1;
        //                                            if (maxcurr == maxtag)
        //                                                continue;
        //                                        }

        //                                        if (v == _datatableSumGrd.Rows.Count - 1 && maxcurr != maxtag)
        //                                        {
        //                                            dr = dt.NewRow();
        //                                            //grdstr = notnggrd + " = " + fntchar + Convert.ToInt32(maxcurr + 1).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxtag).ToString(fmt) + " " + (maxcurr + 1 - mintag + 1).ToString() + " " + (maxtag - mintag + 1).ToString();
        //                                            //dr["GRADE"] = grdstr;
        //                                            dr["GRD"] = notnggrd;
        //                                            dr["MIN"] = fntchar + Convert.ToInt32(maxcurr + 1).ToString(fmt);
        //                                            dr["MAX"] = fntchar + Convert.ToInt32(maxtag).ToString(fmt);
        //                                            dr["SRT"] = notngsrt;
        //                                            dr["TonFrom"] = Convert.ToInt32(maxcurr + 1) - Convert.ToInt32(mintag) + 1;
        //                                            dr["TonTo"] = Convert.ToInt32(maxtag) - Convert.ToInt32(mintag) + 1;
        //                                            dt.Rows.Add(dr);
        //                                            continue;
        //                                        }

        //                                        dr = dt.NewRow();
        //                                        if (tmpstr.Length == 1)
        //                                        {
        //                                            //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(mincurr).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + (mincurr - mintag + 1).ToString() + " " + (maxcurr - mintag + 1).ToString();
        //                                            min = mincurr;
        //                                            tonfrom = mincurr - mintag + 1;
        //                                            tonto = maxcurr - mintag + 1;
        //                                        }
        //                                        else
        //                                        {
        //                                            //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(tmpstr[1]).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + (Convert.ToInt32(tmpstr[1]) - mintag + 1).ToString() + " " + (maxcurr - mintag + 1).ToString();
        //                                            min = Convert.ToInt32(tmpstr[1]);
        //                                            tonfrom = Convert.ToInt32(tmpstr[1]) - mintag + 1;
        //                                            tonto = maxcurr - mintag + 1;
        //                                            tmpmin2 = "";
        //                                        }
        //                                        //dr["GRADE"] = grdstr;
        //                                        dr["GRD"] = grdcurr;
        //                                        dr["MIN"] = fntchar + Convert.ToInt32(min).ToString(fmt);
        //                                        dr["MAX"] = fntchar + Convert.ToInt32(maxcurr).ToString(fmt);
        //                                        dr["SRT"] = _datatableSumGrd.Rows[v]["SRT"].ToString();
        //                                        dr["TonFrom"] = tonfrom;
        //                                        dr["TonTo"] = tonto;
        //                                        dt.Rows.Add(dr);

        //                                        if (maxcurr - mintag + 1 <= 85 && tsegfng)
        //                                        {
        //                                            tmpmin = maxcurr + 1;
        //                                            tmpmax = maxnex - 1;

        //                                            dr = dt.NewRow();
        //                                            //grdstr = notnggrd + " = " + fntchar + Convert.ToInt32(tmpmin).ToString(fmt) + " - " + fntchar + Convert.ToInt32(tmpmax).ToString(fmt) + " " + Convert.ToInt32(tmpmin) - Convert.ToInt32(mintag) + 1 + " " + Convert.ToInt32(tmpmax) - Convert.ToInt32(mintag) + 1;
        //                                            //dr["GRADE"] = grdstr;
        //                                            dr["GRD"] = notnggrd;
        //                                            dr["MIN"] = fntchar + Convert.ToInt32(tmpmin).ToString(fmt);
        //                                            dr["MAX"] = fntchar + Convert.ToInt32(tmpmax).ToString(fmt);
        //                                            dr["SRT"] = notngsrt;
        //                                            dr["TonFrom"] = tmpmin - mintag + 1;
        //                                            dr["TonTo"] = tmpmax - mintag + 1;
        //                                            dt.Rows.Add(dr);
        //                                        }
        //                                        else if (maxcurr - mintag + 1 >= 85 && tsegfng == false)
        //                                        {
        //                                            tmpmin = maxcurr + 1;
        //                                            tmpmax = minnex - 1;

        //                                            dr = dt.NewRow();
        //                                            //grdstr = notnggrd + " = " + fntchar + Convert.ToInt32(tmpmin).ToString(fmt) + " - " + fntchar + Convert.ToInt32(tmpmax).ToString(fmt) + " " + Convert.ToInt32(tmpmin) - Convert.ToInt32(mintag) + 1 + " " + Convert.ToInt32(tmpmax) - Convert.ToInt32(mintag) + 1;
        //                                            //dr["GRADE"] = grdstr;
        //                                            dr["GRD"] = notnggrd;
        //                                            dr["MIN"] = fntchar + Convert.ToInt32(tmpmin).ToString(fmt);
        //                                            dr["MAX"] = fntchar + Convert.ToInt32(tmpmax).ToString(fmt);
        //                                            dr["SRT"] = notngsrt;
        //                                            dr["TonFrom"] = tmpmin - mintag + 1;
        //                                            dr["TonTo"] = tmpmax - mintag + 1;
        //                                            dt.Rows.Add(dr);
        //                                        }
        //                                    }
        //                                    if (tsegfng && v + 1 == Convert.ToInt32(tngrow[0]))
        //                                    {
        //                                        insdone2 = true;
        //                                        continue;
        //                                    }
        //                                    else if (tmpmax + 1 - mintag + 1 > 80)
        //                                    {
        //                                        insdone2 = true;
        //                                        continue;
        //                                    }
        //                                }
        //                                else if (tsegfng == false)
        //                                    insdone2 = true;
        //                            }
        //                            else if (ssegfng && fsegfng && insdone2 == false) //1121
        //                            {
        //                                if (v < Convert.ToInt32(sngrow[sngrow.Count - 1]))
        //                                    continue;
        //                                if (dt.Rows[dt.Rows.Count - 1]["GRD"].ToString() == notnggrd && Convert.ToInt32(dt.Rows[dt.Rows.Count - 1]["TonTo"].ToString()) <= stmpmax - mintag + 1)
        //                                {
        //                                    dt.Rows[dt.Rows.Count - 1].Delete();
        //                                    dt.AcceptChanges();
        //                                }
        //                                var r = dt.Rows.Count - 1;
        //                                //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["TonTo"].ToString(), stmpmax - mintag + 1);
        //                                //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["Max"].ToString(), stmpmax);
        //                                dt.Rows[r]["MAX"] = stmpmax;
        //                                dt.Rows[r]["TonTo"] = stmpmax - mintag + 1;
        //                                insdone2 = true;

        //                                if (v == Convert.ToInt32(sngrow[sngrow.Count - 1]) && v == _datatableSumGrd.Rows.Count - 1)
        //                                {
        //                                    dr = dt.NewRow();
        //                                    //grdstr = notnggrd + " = " + fntchar + Convert.ToInt32(stmpmax + 1).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxtag).ToString(fmt) + " " + Convert.ToInt32(stmpmax + 1) - Convert.ToInt32(mintag) + 1 + " " + Convert.ToInt32(maxtag) - Convert.ToInt32(mintag) + 1;
        //                                    //dr["GRADE"] = grdstr;
        //                                    dr["GRD"] = notnggrd;
        //                                    dr["MIN"] = fntchar + Convert.ToInt32(stmpmax + 1).ToString(fmt);
        //                                    dr["MAX"] = fntchar + Convert.ToInt32(maxtag).ToString(fmt);
        //                                    dr["SRT"] = notngsrt;
        //                                    dr["TonFrom"] = stmpmax + 1 - mintag + 1;
        //                                    dr["TonTo"] = maxtag - mintag + 1;
        //                                    dt.Rows.Add(dr);
        //                                    continue;
        //                                }

        //                                if (minnex - maxcurr > 1)
        //                                {
        //                                    dr = dt.NewRow();
        //                                    //grdstr = notnggrd + " = " + fntchar + Convert.ToInt32(stmpmax + 1).ToString(fmt) + " - " + fntchar + Convert.ToInt32(minnex - 1).ToString(fmt) + " " + stmpmax + 1 - mintag + 1 + " " + minnex - 1 - mintag + 1;
        //                                    //dr["GRADE"] = grdstr;
        //                                    dr["GRD"] = notnggrd;
        //                                    dr["MIN"] = fntchar + Convert.ToInt32(stmpmax + 1).ToString(fmt);
        //                                    dr["MAX"] = fntchar + Convert.ToInt32(minnex - 1).ToString(fmt);
        //                                    dr["SRT"] = notngsrt;
        //                                    dr["TonFrom"] = stmpmax + 1 - mintag + 1;
        //                                    dr["TonTo"] = minnex - 1 - mintag + 1;
        //                                    dt.Rows.Add(dr);
        //                                }
        //                                continue;
        //                            }
        //                            else if (ssegfng && fsegfng == false && insdone2 == false) //1021
        //                            {
        //                                if (v >= Convert.ToInt32(sngrow[0]) && v < Convert.ToInt32(sngrow[sngrow.Count - 1]))
        //                                    continue;
        //                                if (v == Convert.ToInt32(sngrow[sngrow.Count - 1]))
        //                                {
        //                                    var r = dt.Rows.Count - 1;
        //                                    if (dt.Rows[r]["GRD"].ToString() == notnggrd && Convert.ToInt32(dt.Rows[r]["MIN"].ToString()) >= stmpmin) //last row is notng and cannot combine
        //                                    {
        //                                        dt.Rows[r].Delete();
        //                                        dt.AcceptChanges();
        //                                        r = dt.Rows.Count - 1;
        //                                    }
        //                                    //last row is notng and need to replace max to min ng
        //                                    else if (dt.Rows[r]["GRD"].ToString() == notnggrd && (Convert.ToInt32(dt.Rows[r]["MIN"].ToString()) < stmpmin && Convert.ToInt32(dt.Rows[r]["MAX"].ToString()) > stmpmin || stmpmin - Convert.ToInt32(dt.Rows[r]["MAX"].ToString()) == 1))
        //                                    {
        //                                        //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["Max"].ToString(), stmpmin - 1);
        //                                        //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["TonTo"].ToString(), stmpmin - 1 - mintag + 1);
        //                                        dt.Rows[r]["MAX"] = stmpmin - 1;
        //                                        dt.Rows[r]["TonTo"] = stmpmin - 1 - mintag + 1;
        //                                    }
        //                                    // if last row can combine
        //                                    if (dt.Rows[r]["GRD"].ToString() == nggrd && (Convert.ToInt32(dt.Rows[r]["MAX"].ToString()) > stmpmin || stmpmin - Convert.ToInt32(dt.Rows[r]["MAX"].ToString()) == 1))
        //                                    {
        //                                        //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["MAX"].ToString(), stmpmax); // need to replace only last occurance
        //                                        //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["TonTo"].ToString(), (stmpmax - mintag + 1));
        //                                        dt.Rows[r]["MAX"] = stmpmax;
        //                                        dt.Rows[r]["TonTo"] = stmpmax - mintag + 1;
        //                                        insdone2 = true;
        //                                        if (hightag > 80 && v != _datatableSumGrd.Rows.Count - 1)
        //                                            continue;
        //                                        else if (hightag > 80 && v == _datatableSumGrd.Rows.Count - 1)
        //                                        {
        //                                            dr = dt.NewRow();
        //                                            //grdstr = notnggrd + " = " + fntchar + Convert.ToInt32(stmpmax + 1).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxtag).ToString(fmt) + " " + Convert.ToInt32(stmpmax + 1) - Convert.ToInt32(mintag) + 1 + " " + Convert.ToInt32(maxtag) - Convert.ToInt32(mintag) + 1;
        //                                            //dr["GRADE"] = grdstr;
        //                                            dr["GRD"] = notnggrd;
        //                                            dr["MIN"] = fntchar + Convert.ToInt32(stmpmax + 1).ToString(fmt);
        //                                            dr["MAX"] = fntchar + Convert.ToInt32(maxtag).ToString(fmt);
        //                                            dr["SRT"] = notngsrt;
        //                                            dr["TonFrom"] = stmpmax + 1 - mintag + 1;
        //                                            dr["TonTo"] = maxtag - mintag + 1;
        //                                            dt.Rows.Add(dr);
        //                                            continue;
        //                                        }
        //                                        else if (hightag <= 80 && maxcurr != maxtag)
        //                                        {
        //                                            dr = dt.NewRow();
        //                                            //grdstr = notnggrd + " = " + fntchar + Convert.ToInt32(stmpmax + 1).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxtag).ToString(fmt) + " " + Convert.ToInt32(stmpmax + 1) - Convert.ToInt32(mintag) + 1 + " " + Convert.ToInt32(maxtag) - Convert.ToInt32(mintag) + 1;
        //                                            //dr["GRADE"] = grdstr;
        //                                            dr["GRD"] = notnggrd;
        //                                            dr["MIN"] = fntchar + Convert.ToInt32(stmpmax + 1).ToString(fmt);
        //                                            dr["MAX"] = fntchar + Convert.ToInt32(maxtag).ToString(fmt);
        //                                            dr["SRT"] = notngsrt;
        //                                            dr["TonFrom"] = stmpmax + 1 - mintag + 1;
        //                                            dr["TonTo"] = maxtag - mintag + 1;
        //                                            dt.Rows.Add(dr);
        //                                            continue;
        //                                        }
        //                                        else if (hightag <= 80 && maxcurr == maxtag)
        //                                            continue;
        //                                    }
        //                                    dr = dt.NewRow();
        //                                    //grdstr = nggrd + " = " + fntchar + Convert.ToInt32(stmpmin).ToString(fmt) + " - " + fntchar + Convert.ToInt32(stmpmax).ToString(fmt) + " " + Convert.ToInt32(stmpmin) - Convert.ToInt32(mintag) + 1 + " " + Convert.ToInt32(stmpmax) - Convert.ToInt32(mintag) + 1;
        //                                    //dr["GRADE"] = grdstr;
        //                                    dr["GRD"] = nggrd;
        //                                    dr["MIN"] = fntchar + Convert.ToInt32(stmpmin).ToString(fmt);
        //                                    dr["MAX"] = fntchar + Convert.ToInt32(stmpmax).ToString(fmt);
        //                                    dr["SRT"] = ngsrt;
        //                                    dr["TonFrom"] = stmpmin - mintag + 1;
        //                                    dr["TonTo"] = stmpmax - mintag + 1;
        //                                    dt.Rows.Add(dr);

        //                                    if (minnex - maxcurr > 1)
        //                                    {
        //                                        dr = dt.NewRow();
        //                                        //grdstr = notnggrd + " = " + fntchar + Convert.ToInt32(stmpmax + 1).ToString(fmt) + " - " + fntchar + Convert.ToInt32(minnex - 1).ToString(fmt) + " " + stmpmax + 1 - mintag + 1 + " " + minnex - 1 - mintag + 1;
        //                                        //dr["GRADE"] = grdstr;
        //                                        dr["GRD"] = notnggrd;
        //                                        dr["MIN"] = fntchar + Convert.ToInt32(mincurr).ToString(fmt);
        //                                        dr["MAX"] = fntchar + Convert.ToInt32(maxcurr).ToString(fmt);
        //                                        dr["SRT"] = notngsrt;
        //                                        dr["TonFrom"] = stmpmax + 1 - mintag + 1;
        //                                        dr["TonTo"] = minnex - 1 - mintag + 1;
        //                                        dt.Rows.Add(dr);
        //                                    }

        //                                    insdone2 = true;
        //                                    continue;
        //                                }

        //                                if (v == 0)
        //                                {
        //                                    if (grdcurr != nggrd)
        //                                    {
        //                                        tmpmin = mincurr;
        //                                        continue;
        //                                    }
        //                                    else if (grdcurr == nggrd)
        //                                    {
        //                                        if (grdnex == nggrd && (minnex < maxcurr || minnex - maxcurr == 1))
        //                                        {
        //                                            tmpmin2 = tmpmin2 + "," + mincurr;
        //                                            if (minnex - mintag + 1 < 36)
        //                                                continue;
        //                                        }
        //                                        tmpmin = maxcurr + 1;
        //                                        dr = dt.NewRow();
        //                                        //grdstr = _datatableSumGrd.Rows[v]["GRADE"].ToString() + " = " + fntchar + Convert.ToInt32(mincurr).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + mincurr - mintag + 1 + " " + maxcurr - mintag + 1;
        //                                        //dr["GRADE"] = grdstr;
        //                                        dr["GRD"] = _datatableSumGrd.Rows[v]["GRADE"].ToString();
        //                                        dr["MIN"] = fntchar + Convert.ToInt32(mincurr).ToString(fmt);
        //                                        dr["MAX"] = fntchar + Convert.ToInt32(maxcurr).ToString(fmt);
        //                                        dr["SRT"] = ngsrt;
        //                                        dr["TonFrom"] = mincurr - mintag + 1;
        //                                        dr["TonTo"] = maxcurr - mintag + 1;
        //                                        dt.Rows.Add(dr);
        //                                    }
        //                                }
        //                                else if (v != _datatableSumGrd.Rows.Count - 1)
        //                                {
        //                                    if (grdcurr == nggrd && grdnex != nggrd)
        //                                    {
        //                                        string[] tmpstr;
        //                                        int min = 0;
        //                                        tmpstr = tmpmin2.Split(',');
        //                                        var r = dt.Rows.Count - 1;

        //                                        dr = dt.NewRow();
        //                                        if (minnex > maxcurr && tmpstr.Length == 1)
        //                                        {
        //                                            //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(mincurr).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + mincurr - mintag + 1 + " " + maxcurr - mintag + 1;
        //                                            min = mincurr;
        //                                            tonfrom = mincurr - mintag + 1;
        //                                            tonto = maxcurr - mintag + 1;
        //                                        }
        //                                        else
        //                                        {
        //                                            //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(tmpstr[1]).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + Convert.ToInt32(tmpstr[1]) - Convert.ToInt32(mintag) + 1 + " " + maxcurr - mintag + 1;
        //                                            min = Convert.ToInt32(tmpstr[1]);
        //                                            tonfrom = Convert.ToInt32(tmpstr[1]) - mintag + 1;
        //                                            tonto = maxcurr - mintag + 1;
        //                                            tmpmin2 = "";
        //                                        }
        //                                        dr["GRADE"] = grdstr;
        //                                        dr["GRD"] = grdcurr;
        //                                        dr["MIN"] = fntchar + Convert.ToInt32(min).ToString(fmt);
        //                                        dr["MAX"] = fntchar + Convert.ToInt32(maxcurr).ToString(fmt);
        //                                        dr["SRT"] = _datatableSumGrd.Rows[v]["SRT"].ToString();
        //                                        dr["TonFrom"] = tonfrom;
        //                                        dr["TonTo"] = tonto;
        //                                        dt.Rows.Add(dr);
        //                                        tmpmin = maxcurr + 1;
        //                                        tmpmin2 = "";
        //                                    }
        //                                    if (grdnex != nggrd)
        //                                    {
        //                                        string[] tmpstr;
        //                                        tmpstr = tmpmin2.Split(',');
        //                                        if (tmpstr.Count() == 1)
        //                                            continue;
        //                                    }
        //                                    else if (grdcurr != nggrd && grdnex == nggrd)
        //                                    {
        //                                        tmpmax = minnex - 1;
        //                                        if (minnex > tmpmin - 1 && minnex - tmpmin > 1)
        //                                        {
        //                                            dr = dt.NewRow();
        //                                            //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(tmpmin).ToString(fmt) + " - " + fntchar + Convert.ToInt32(tmpmax).ToString(fmt) + " " + Convert.ToInt32(tmpmin) - Convert.ToInt32(mintag) + 1 + " " + Convert.ToInt32(tmpmax) - Convert.ToInt32(mintag) + 1;
        //                                            //dr["GRADE"] = grdstr;
        //                                            dr["GRD"] = grdcurr;
        //                                            dr["MIN"] = fntchar + Convert.ToInt32(tmpmin).ToString(fmt);
        //                                            dr["MAX"] = fntchar + Convert.ToInt32(tmpmax).ToString(fmt);
        //                                            dr["SRT"] = notngsrt;
        //                                            dr["TonFrom"] = tmpmin - mintag + 1;
        //                                            dr["TonTo"] = tmpmax - mintag + 1;
        //                                            dt.Rows.Add(dr);
        //                                        }
        //                                        else
        //                                        {
        //                                            var r = dt.Rows.Count - 1;
        //                                            //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["TonTo"].ToString(), maxnex - mintag + 1);
        //                                            //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["Max"].ToString(), maxnex);
        //                                            dt.Rows[r]["MAX"] = maxnex;
        //                                            dt.Rows[r]["TonTo"] = maxnex - mintag + 1;
        //                                        }
        //                                    }
        //                                    else if (grdcurr == nggrd && grdnex == nggrd)
        //                                    {
        //                                        tmpmax = minnex - 1;
        //                                        tmpmin = maxcurr + 1;
        //                                        string[] tmpstr;

        //                                        if (minnex < maxcurr || minnex - maxcurr == 1)
        //                                        {
        //                                            tmpmin2 = tmpmin2 + "," + mincurr;
        //                                            if (minnex - mintag + 1 < 36)
        //                                                continue;
        //                                        }
        //                                        tmpstr = tmpmin2.Split(',');
        //                                        var r = dt.Rows.Count - 1;
        //                                        if (Convert.ToInt32(dt.Rows[r]["MAX"].ToString()) != maxcurr && mincurr > Convert.ToInt32(dt.Rows[r]["MAX"].ToString()))
        //                                        {
        //                                            int min = 0;
        //                                            dr = dt.NewRow();
        //                                            if (minnex > maxcurr && tmpstr.Length == 1)
        //                                            {
        //                                                //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(mincurr).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + mincurr - mintag + 1 + " " + maxcurr - mintag + 1;
        //                                                min = mincurr;
        //                                                tonfrom = mincurr - mintag + 1;
        //                                                tonto = maxcurr - mintag + 1;
        //                                            }
        //                                            else
        //                                            {
        //                                                //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(tmpstr[1]).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + Convert.ToInt32(tmpstr[1]) - Convert.ToInt32(mintag) + 1 + " " + maxcurr - mintag + 1;
        //                                                min = Convert.ToInt32(tmpstr[1]);
        //                                                tonfrom = Convert.ToInt32(tmpstr[1]) - mintag + 1;
        //                                                tonto = maxcurr - mintag + 1;
        //                                                tmpmin2 = "";
        //                                            }
        //                                            dr["GRADE"] = grdstr;
        //                                            dr["GRD"] = grdcurr;
        //                                            dr["MIN"] = fntchar + Convert.ToInt32(min).ToString(fmt);
        //                                            dr["MAX"] = fntchar + Convert.ToInt32(maxcurr).ToString(fmt);
        //                                            dr["SRT"] = _datatableSumGrd.Rows[v]["SRT"].ToString();
        //                                            dr["TonFrom"] = tonfrom;
        //                                            dr["TonTo"] = tonto;
        //                                            dt.Rows.Add(dr);
        //                                            tmpmin2 = "";
        //                                        }
        //                                        else if (Convert.ToInt32(dt.Rows[r]["MAX"].ToString()) != maxcurr && mincurr < Convert.ToInt32(dt.Rows[r]["MAX"].ToString()))
        //                                        {
        //                                            //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["TonTo"].ToString(), maxcurr - mintag + 1);
        //                                            //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["Max"].ToString(), maxcurr);
        //                                            dt.Rows[r]["MAX"] = maxcurr;
        //                                            dt.Rows[r]["TonTo"] = maxcurr - mintag + 1;
        //                                            tmpmin2 = "";
        //                                        }
        //                                        if (minnex - maxcurr > 1)
        //                                        {
        //                                            dr = dt.NewRow();
        //                                            //grdstr = notnggrd + " = " + fntchar + Convert.ToInt32(tmpmin).ToString(fmt) + " - " + fntchar + Convert.ToInt32(tmpmax).ToString(fmt) + " " + Convert.ToInt32(tmpmin) - Convert.ToInt32(mintag) + 1 + " " + Convert.ToInt32(tmpmax) - Convert.ToInt32(mintag) + 1;
        //                                            //dr["GRADE"] = grdstr;
        //                                            dr["GRD"] = notnggrd;
        //                                            dr["MIN"] = fntchar + Convert.ToInt32(tmpmin).ToString(fmt);
        //                                            dr["MAX"] = fntchar + Convert.ToInt32(tmpmax).ToString(fmt);
        //                                            dr["SRT"] = notngsrt;
        //                                            dr["TonFrom"] = tmpmin - mintag + 1;
        //                                            dr["TonTo"] = tmpmax - mintag + 1;
        //                                            dt.Rows.Add(dr);
        //                                        }
        //                                    }
        //                                }

        //                                if (tsegfng && v + 1 == Convert.ToInt32(tngrow[0]))
        //                                {
        //                                    insdone2 = true;
        //                                    continue;
        //                                }
        //                                else if (minnex - mintag + 1 > 76)
        //                                {
        //                                    insdone2 = true;
        //                                    continue;
        //                                }
        //                            }
        //                            else if (ssegfng == false && fsegfng == false && insdone2 == false) //1020
        //                            {
        //                                if (hightag <= 80)
        //                                {
        //                                    if (v == 0)
        //                                    {
        //                                        if (grdcurr != nggrd)
        //                                        {
        //                                            tmpmin = mincurr;
        //                                            string[] tmpstr;
        //                                            tmpstr = tmpmin2.Split(',');
        //                                            if (tmpstr.Count() == 1)
        //                                                continue;
        //                                        }
        //                                        else if (grdcurr == nggrd)
        //                                        {
        //                                            if (grdnex == nggrd && (minnex < maxcurr || minnex - maxcurr == 1))
        //                                            {
        //                                                tmpmin2 = tmpmin2 + "," + mincurr;
        //                                                if (minnex - mintag + 1 < 36)
        //                                                    continue;
        //                                            }
        //                                            tmpmin = maxcurr + 1;
        //                                            dr = dt.NewRow();
        //                                            //grdstr = _datatableSumGrd.Rows[v]["GRADE"].ToString() + " = " + fntchar + Convert.ToInt32(mincurr).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + mincurr - mintag + 1 + " " + maxcurr - mintag + 1;
        //                                            //dr["GRADE"] = grdstr;
        //                                            dr["GRD"] = _datatableSumGrd.Rows[v]["GRADE"].ToString();
        //                                            dr["MIN"] = fntchar + Convert.ToInt32(mincurr).ToString(fmt);
        //                                            dr["MAX"] = fntchar + Convert.ToInt32(maxcurr).ToString(fmt);
        //                                            dr["SRT"] = ngsrt;
        //                                            dr["TonFrom"] = mincurr - mintag + 1;
        //                                            dr["TonTo"] = maxcurr - mintag + 1;
        //                                            dt.Rows.Add(dr);
        //                                        }
        //                                    }
        //                                    else if (v != _datatableSumGrd.Rows.Count - 1)
        //                                    {
        //                                        if (grdnex != nggrd)
        //                                        {
        //                                            string[] tmpstr;
        //                                            tmpstr = tmpmin2.Split(',');
        //                                            if (tmpstr.Count() == 1)
        //                                                continue;
        //                                        }
        //                                        else if (grdcurr != nggrd && grdnex == nggrd)
        //                                        {
        //                                            tmpmax = minnex - 1;
        //                                            dr = dt.NewRow();
        //                                            if (minnex > tmpmin - 1 && minnex - tmpmin - 1 != 1 && minnex - tmpmin - 1 > 0)
        //                                            {
        //                                                //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(tmpmin).ToString(fmt) + " - " + fntchar + Convert.ToInt32(tmpmax).ToString(fmt) + " " + Convert.ToInt32(tmpmin) - Convert.ToInt32(mintag) + 1 + " " + Convert.ToInt32(tmpmax) - Convert.ToInt32(mintag) + 1;
        //                                                //dr["GRADE"] = grdstr;
        //                                                dr["GRD"] = grdcurr;
        //                                                dr["MIN"] = fntchar + Convert.ToInt32(tmpmin).ToString(fmt);
        //                                                dr["MAX"] = fntchar + Convert.ToInt32(tmpmax).ToString(fmt);
        //                                                dr["SRT"] = notngsrt;
        //                                                dr["TonFrom"] = tmpmin - mintag + 1;
        //                                                dr["TonTo"] = tmpmax - mintag + 1;
        //                                                dt.Rows.Add(dr);
        //                                            }
        //                                            else
        //                                            {
        //                                                var r = dt.Rows.Count - 1;
        //                                                //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["TonTo"].ToString(), maxnex - mintag + 1);
        //                                                //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["Max"].ToString(), maxnex);
        //                                                dt.Rows[r]["MAX"] = maxnex;
        //                                                dt.Rows[r]["TonTo"] = maxnex - mintag + 1;
        //                                            }
        //                                        }
        //                                        else if (grdcurr == nggrd && grdnex == nggrd)
        //                                        {
        //                                            if (grdnex == nggrd && (minnex < maxcurr || minnex - maxcurr == 1))
        //                                            {
        //                                                tmpmin2 = tmpmin2 + "," + mincurr;
        //                                                if (minnex - mintag + 1 < 36)
        //                                                    continue;
        //                                            }
        //                                            tmpmax = minnex - 1;
        //                                            tmpmin = maxcurr + 1;
        //                                            string[] tmpstr;
        //                                            if (minnex < maxcurr || (minnex - maxcurr == 1 && minnex - maxcurr > 0) && minnex - mintag + 1 < 76)
        //                                            {
        //                                                tmpmin2 = tmpmin2 + "," + mincurr;
        //                                                continue;
        //                                            }
        //                                            tmpstr = tmpmin2.Split(',');
        //                                            var r = dt.Rows.Count - 1;
        //                                            if (Convert.ToInt32(dt.Rows[r]["MAX"].ToString()) != maxcurr && mincurr > Convert.ToInt32(dt.Rows[r]["MAX"].ToString()))
        //                                            {
        //                                                int min = 0;
        //                                                dr = dt.NewRow();
        //                                                if (minnex > maxcurr && tmpstr.Length == 1)
        //                                                {
        //                                                    //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(mincurr).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + mincurr - mintag + 1 + " " + maxcurr - mintag + 1;
        //                                                    min = mincurr;
        //                                                    tonfrom = mincurr - mintag + 1;
        //                                                    tonto = maxcurr - mintag + 1;
        //                                                }
        //                                                else
        //                                                {
        //                                                    //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(tmpstr[1]).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + Convert.ToInt32(tmpstr[1]) - Convert.ToInt32(mintag) + 1 + " " + maxcurr - mintag + 1;
        //                                                    min = Convert.ToInt32(tmpstr[1]);
        //                                                    tonfrom = Convert.ToInt32(tmpstr[1]) - mintag + 1;
        //                                                    tonto = maxcurr - mintag + 1;
        //                                                    tmpmin2 = "";
        //                                                }
        //                                                //dr["GRADE"] = grdstr;
        //                                                dr["GRD"] = grdcurr;
        //                                                dr["MIN"] = fntchar + Convert.ToInt32(min).ToString(fmt);
        //                                                dr["MAX"] = fntchar + Convert.ToInt32(maxcurr).ToString(fmt);
        //                                                dr["SRT"] = _datatableSumGrd.Rows[v]["SRT"].ToString();
        //                                                dr["TonFrom"] = tonfrom;
        //                                                dr["TonTo"] = tonto;
        //                                                dt.Rows.Add(dr);
        //                                                tmpmin2 = "";
        //                                            }
        //                                            else if (Convert.ToInt32(dt.Rows[r]["MAX"].ToString()) != maxcurr && mincurr < Convert.ToInt32(dt.Rows[r]["MAX"].ToString()))
        //                                            {
        //                                                //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["TonTo"].ToString(), maxcurr - mintag + 1);
        //                                                //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["Max"].ToString(), maxcurr);
        //                                                dt.Rows[r]["MAX"] = maxcurr;
        //                                                dt.Rows[r]["TonTo"] = maxcurr - mintag + 1;
        //                                                tmpmin2 = "";
        //                                            }
        //                                            if (minnex - maxcurr > 1)
        //                                            {
        //                                                dr = dt.NewRow();
        //                                                //grdstr = notnggrd + " = " + fntchar + Convert.ToInt32(tmpmin).ToString(fmt) + " - " + fntchar + Convert.ToInt32(tmpmax).ToString(fmt) + " " + Convert.ToInt32(tmpmin) - Convert.ToInt32(mintag) + 1 + " " + Convert.ToInt32(tmpmax) - Convert.ToInt32(mintag) + 1;
        //                                                //dr["GRADE"] = grdstr;
        //                                                dr["GRD"] = notnggrd;
        //                                                dr["MIN"] = fntchar + Convert.ToInt32(tmpmin).ToString(fmt);
        //                                                dr["MAX"] = fntchar + Convert.ToInt32(tmpmax).ToString(fmt);
        //                                                dr["SRT"] = notngsrt;
        //                                                dr["TonFrom"] = tmpmin - mintag + 1;
        //                                                dr["TonTo"] = tmpmax - mintag + 1;
        //                                                dt.Rows.Add(dr);
        //                                            }
        //                                        }
        //                                        if (grdcurr == nggrd && grdnex != nggrd)
        //                                        {
        //                                            string[] tmpstr;

        //                                            tmpstr = tmpmin2.Split(',');
        //                                            dr = dt.NewRow();
        //                                            //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(tmpstr[1]).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + Convert.ToInt32(tmpstr[1]) - Convert.ToInt32(mintag) + 1 + " " + maxcurr - mintag + 1;
        //                                            tonfrom = Convert.ToInt32(tmpstr[1]) - mintag + 1;
        //                                            tonto = maxcurr - mintag + 1;
        //                                            dr["GRADE"] = grdstr;
        //                                            dr["GRD"] = grdcurr;
        //                                            dr["MIN"] = fntchar + Convert.ToInt32(tmpstr[1]).ToString(fmt);
        //                                            dr["MAX"] = fntchar + Convert.ToInt32(maxcurr).ToString(fmt);
        //                                            dr["SRT"] = _datatableSumGrd.Rows[v]["SRT"].ToString();
        //                                            dr["TonFrom"] = tonfrom;
        //                                            dr["TonTo"] = tonto;
        //                                            dt.Rows.Add(dr);
        //                                            tmpmin = maxcurr + 1;
        //                                            tmpmin2 = "";
        //                                        }
        //                                    }
        //                                    else if (v == _datatableSumGrd.Rows.Count - 1 && maxcurr != maxtag)
        //                                    {
        //                                        dr = dt.NewRow();
        //                                        //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(mincurr).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + mincurr - mintag + 1 + " " + maxcurr - mintag + 1;
        //                                        //dr["GRADE"] = grdstr;
        //                                        dr["GRD"] = grdcurr;
        //                                        dr["MIN"] = fntchar + Convert.ToInt32(mincurr).ToString(fmt);
        //                                        dr["MAX"] = fntchar + Convert.ToInt32(maxcurr).ToString(fmt);
        //                                        dr["SRT"] = _datatableSumGrd.Rows[v]["SRT"].ToString();
        //                                        dr["TonFrom"] = mincurr - mintag + 1;
        //                                        dr["TonTo"] = maxcurr - mintag + 1;
        //                                        dt.Rows.Add(dr);
        //                                        dr = dt.NewRow();
        //                                        //grdstr = notnggrd + " = " + fntchar + Convert.ToInt32(maxcurr + 1).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxtag).ToString(fmt) + " " + Convert.ToInt32(maxcurr + 1) - Convert.ToInt32(mintag) + 1 + " " + Convert.ToInt32(maxtag) - Convert.ToInt32(mintag) + 1;
        //                                        //dr["GRADE"] = grdstr;
        //                                        dr["GRD"] = notnggrd;
        //                                        dr["MIN"] = fntchar + Convert.ToInt32(maxcurr + 1).ToString(fmt);
        //                                        dr["MAX"] = fntchar + Convert.ToInt32(maxtag).ToString(fmt);
        //                                        dr["SRT"] = notngsrt;
        //                                        dr["TonFrom"] = maxcurr - mintag + 1;
        //                                        dr["TonTo"] = maxtag - mintag + 1;
        //                                        dt.Rows.Add(dr);
        //                                    }
        //                                    else if (v == _datatableSumGrd.Rows.Count - 1 && maxcurr == maxtag)
        //                                    {
        //                                        var r = dt.Rows.Count - 1;
        //                                        if (dt.Rows[dt.Rows.Count - 1]["GRD"].ToString() == notnggrd)
        //                                        {
        //                                            dr = dt.NewRow();
        //                                            //grdstr = _datatableSumGrd.Rows[v]["GRADE"].ToString() + " = " + fntchar + Convert.ToInt32(ttmpmin).ToString(fmt) + " - " + fntchar + Convert.ToInt32(ttmpmax).ToString(fmt) + " " + ttmpmin - mintag + 1 + " " + ttmpmax - mintag + 1;
        //                                            //dr["GRADE"] = grdstr;
        //                                            dr["GRD"] = _datatableSumGrd.Rows[v]["GRADE"].ToString();
        //                                            dr["MIN"] = fntchar + Convert.ToInt32(ttmpmin).ToString(fmt);
        //                                            dr["MAX"] = fntchar + Convert.ToInt32(ttmpmax).ToString(fmt);
        //                                            dr["SRT"] = ngsrt;
        //                                            dr["TonFrom"] = ttmpmin - mintag + 1;
        //                                            dr["TonTo"] = ttmpmax - mintag + 1;
        //                                            dt.Rows.Add(dr);
        //                                        }
        //                                        else
        //                                        {
        //                                            //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["TonTo"].ToString(), ttmpmin - 1 - mintag + 1);
        //                                            //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["Max"].ToString(), ttmpmin - 1);
        //                                            dt.Rows[r]["MAX"] = ttmpmin - 1;
        //                                            dt.Rows[r]["TonTo"] = ttmpmin - 1 - mintag + 1;
        //                                        }
        //                                    }
        //                                }
        //                                else
        //                                    insdone2 = true;
        //                            }

        //                            if (hightag > 80)
        //                            {
        //                                if (tsegfng == false && fsegfng && ssegfng && insdone2) //112130
        //                                {
        //                                    if (v != _datatableSumGrd.Rows.Count - 1)
        //                                    {
        //                                        if (grdcurr == nggrd && grdnex == nggrd)
        //                                        {
        //                                            tmpmax = minnex - 1;
        //                                            tmpmin = maxcurr + 1;
        //                                            string[] tmpstr;
        //                                            if (minnex < maxcurr || minnex - maxcurr == 1)
        //                                            {
        //                                                tmpmin2 = tmpmin2 + "," + mincurr;
        //                                                continue;
        //                                            }
        //                                            tmpstr = tmpmin2.Split(',');
        //                                            int min = 0;
        //                                            dr = dt.NewRow(); // continue insert row
        //                                            if (minnex > maxcurr && tmpstr.Length == 1)
        //                                            {
        //                                                //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(mincurr).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + mincurr - mintag + 1 + " " + maxcurr - mintag + 1;
        //                                                tonfrom = mincurr - mintag + 1;
        //                                                tonto = maxcurr - mintag + 1;
        //                                                min = mincurr;
        //                                            }
        //                                            else
        //                                            {
        //                                                //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(tmpstr[1]).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + Convert.ToInt32(tmpstr[1]) - Convert.ToInt32(mintag) + 1 + " " + tonto;
        //                                                tonfrom = Convert.ToInt32(tmpstr[1]) - mintag + 1;
        //                                                tonto = maxcurr - mintag + 1;
        //                                                min = Convert.ToInt32(tmpstr[1]);
        //                                                tmpmin2 = "";
        //                                            }
        //                                            //dr["GRADE"] = grdstr;
        //                                            dr["GRD"] = grdcurr;
        //                                            dr["MIN"] = fntchar + Convert.ToInt32(min).ToString(fmt);
        //                                            dr["MAX"] = fntchar + Convert.ToInt32(maxcurr).ToString(fmt);
        //                                            dr["SRT"] = _datatableSumGrd.Rows[v]["SRT"].ToString();
        //                                            dr["TonFrom"] = tonfrom;
        //                                            dr["TonTo"] = tonto;
        //                                            dt.Rows.Add(dr);
        //                                            dr = dt.NewRow();
        //                                            //grdstr = notnggrd + " = " + fntchar + Convert.ToInt32(tmpmin).ToString(fmt) + " - " + fntchar + Convert.ToInt32(tmpmax).ToString(fmt) + " " + Convert.ToInt32(tmpmin) - Convert.ToInt32(mintag) + 1 + " " + Convert.ToInt32(tmpmax) - Convert.ToInt32(mintag) + 1;
        //                                            //dr["GRADE"] = grdstr;
        //                                            dr["GRD"] = notnggrd;
        //                                            dr["MIN"] = fntchar + Convert.ToInt32(tmpmin).ToString(fmt);
        //                                            dr["MAX"] = fntchar + Convert.ToInt32(tmpmax).ToString(fmt);
        //                                            dr["SRT"] = notngsrt;
        //                                            dr["TonFrom"] = tmpmin - mintag + 1;
        //                                            dr["TonTo"] = tmpmax - mintag + 1;
        //                                            dt.Rows.Add(dr);
        //                                        }
        //                                    }
        //                                    else if (v == _datatableSumGrd.Rows.Count - 1 && maxcurr != maxtag)
        //                                    {
        //                                        string[] tmpstr;
        //                                        tmpstr = tmpmin2.Split(',');
        //                                        int min = 0;
        //                                        dr = dt.NewRow();
        //                                        if (mincurr - stmpmax > 1)
        //                                        {
        //                                            dr = dt.NewRow();
        //                                            //grdstr = notnggrd + " = " + fntchar + Convert.ToInt32(stmpmax + 1).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr - 1).ToString(fmt) + " " + Convert.ToInt32(stmpmax + 1) - Convert.ToInt32(mintag) + 1 + " " + mincurr - 1 - mintag + 1;
        //                                            tonfrom = stmpmax + 1 - mintag + 1;
        //                                            tonto = maxcurr - mintag + 1;
        //                                            //dr["GRADE"] = grdstr;
        //                                            dr["GRD"] = notnggrd;
        //                                            dr["MIN"] = fntchar + Convert.ToInt32(stmpmax + 1).ToString(fmt);
        //                                            dr["MAX"] = fntchar + Convert.ToInt32(mincurr - 1).ToString(fmt);
        //                                            dr["SRT"] = _datatableSumGrd.Rows[v]["SRT"].ToString();
        //                                            dr["TonFrom"] = stmpmax + 1 - mintag + 1;
        //                                            dr["TonTo"] = mincurr - 1 - mintag + 1;
        //                                            dt.Rows.Add(dr);
        //                                            dr = dt.NewRow();
        //                                            tmpstr = tmpmin2.Split(',');
        //                                            if (tmpstr.Length == 1)
        //                                            {
        //                                                //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(mincurr).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + mincurr - mintag + 1 + " " + maxcurr - mintag + 1;
        //                                                min = mincurr;
        //                                                tonfrom = mincurr - mintag + 1;
        //                                                tonto = maxcurr - mintag + 1;
        //                                            }
        //                                            else
        //                                            {
        //                                                //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(tmpstr[1]).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + Convert.ToInt32(tmpstr[1]) - Convert.ToInt32(mintag) + 1 + " " + maxcurr - mintag + 1;
        //                                                min = Convert.ToInt32(tmpstr[1]);
        //                                                tonfrom = Convert.ToInt32(tmpstr[1]) - mintag + 1;
        //                                                tonto = maxcurr - mintag + 1;
        //                                                tmpmin2 = "";
        //                                            }
        //                                            //dr["GRADE"] = grdstr;
        //                                            dr["GRD"] = grdcurr;
        //                                            dr["MIN"] = fntchar + Convert.ToInt32(min).ToString(fmt);
        //                                            dr["MAX"] = fntchar + Convert.ToInt32(maxcurr).ToString(fmt);
        //                                            dr["SRT"] = _datatableSumGrd.Rows[v]["SRT"].ToString();
        //                                            dr["TonFrom"] = tonfrom;
        //                                            dr["TonTo"] = tonto;
        //                                            dt.Rows.Add(dr);
        //                                        }
        //                                        else if (mincurr < stmpmax || mincurr - stmpmax == 1)
        //                                        {
        //                                            var r = dt.Rows.Count - 1;
        //                                            //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["TonTo"].ToString(), maxcurr - mintag + 1);
        //                                            //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["Max"].ToString(), maxcurr);
        //                                            dt.Rows[r]["MAX"] = maxcurr;
        //                                            dt.Rows[r]["TonTo"] = maxcurr - mintag + 1;
        //                                            if (v == _datatableSumGrd.Rows.Count - 1 && maxcurr != maxtag)
        //                                            {
        //                                                dr = dt.NewRow();
        //                                                //grdstr = notnggrd + " = " + fntchar + Convert.ToInt32(maxcurr + 1).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxtag).ToString(fmt) + " " + (maxcurr + 1 - mintag + 1).ToString() + " " + (maxtag - mintag + 1).ToString();
        //                                                //dr["GRADE"] = grdstr;
        //                                                dr["GRD"] = notnggrd;
        //                                                dr["MIN"] = fntchar + Convert.ToInt32(maxcurr + 1).ToString(fmt);
        //                                                dr["MAX"] = fntchar + Convert.ToInt32(maxtag).ToString(fmt);
        //                                                dr["SRT"] = notngsrt;
        //                                                dr["TonFrom"] = Convert.ToInt32(maxcurr + 1) - Convert.ToInt32(mintag) + 1;
        //                                                dr["TonTo"] = Convert.ToInt32(maxtag) - Convert.ToInt32(mintag) + 1;
        //                                                dt.Rows.Add(dr);
        //                                                continue;
        //                                            }
        //                                        }
        //                                        dr = dt.NewRow();
        //                                        //grdstr = notnggrd + " = " + fntchar + Convert.ToInt32(maxcurr + 1).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxtag).ToString(fmt) + " " + Convert.ToInt32(maxcurr + 1) - Convert.ToInt32(mintag) + 1 + " " + Convert.ToInt32(maxtag) - Convert.ToInt32(mintag) + 1;
        //                                        //dr["GRADE"] = grdstr;
        //                                        dr["GRD"] = notnggrd;
        //                                        dr["MIN"] = fntchar + Convert.ToInt32(maxcurr + 1).ToString(fmt);
        //                                        dr["MAX"] = fntchar + Convert.ToInt32(maxtag).ToString(fmt);
        //                                        dr["SRT"] = notngsrt;
        //                                        dr["TonFrom"] = maxcurr + 1 - mintag + 1;
        //                                        dr["TonTo"] = maxtag - mintag + 1;
        //                                        dt.Rows.Add(dr);
        //                                    }
        //                                    else if (v == _datatableSumGrd.Rows.Count - 1 && maxcurr == maxtag)
        //                                    {
        //                                        string[] tmpstr;
        //                                        dr = dt.NewRow();
        //                                        tmpstr = tmpmin2.Split(',');
        //                                        int min = 0;
        //                                        if (tmpstr.Length != 1)
        //                                        {
        //                                            //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(tmpstr[1]).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + tmpstr[1] - mintag + 1 + " " + maxcurr - mintag + 1;
        //                                            min = Convert.ToInt32(tmpstr[1]);
        //                                            tonfrom = Convert.ToInt32(tmpstr[1]) - mintag + 1;
        //                                            tonto = maxcurr - mintag + 1;
        //                                            tmpmin2 = "";
        //                                        }
        //                                        else
        //                                        {
        //                                            //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(mincurr).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + mincurr - mintag + 1 + " " + maxcurr - mintag + 1;
        //                                            min = mincurr;
        //                                            tonfrom = mincurr - mintag + 1;
        //                                            tonto = maxcurr - mintag + 1;
        //                                        }
        //                                        //dr["GRADE"] = grdstr;
        //                                        dr["GRD"] = grdcurr;
        //                                        dr["MIN"] = fntchar + Convert.ToInt32(min).ToString(fmt);
        //                                        dr["MAX"] = fntchar + Convert.ToInt32(maxcurr).ToString(fmt);
        //                                        dr["SRT"] = _datatableSumGrd.Rows[v]["SRT"].ToString();
        //                                        dr["TonFrom"] = min - mintag + 1;
        //                                        dr["TonTo"] = maxcurr - mintag + 1;
        //                                        dt.Rows.Add(dr);
        //                                    }
        //                                }
        //                                else if (tsegfng == false && fsegfng && ssegfng == false && insdone2) //112030
        //                                {
        //                                    string[] tmpstr;
        //                                    var r = dt.Rows.Count - 1;
        //                                    if (v != _datatableSumGrd.Rows.Count - 1)
        //                                    {
        //                                        if (grdcurr == nggrd && grdnex == nggrd)
        //                                        {
        //                                            tmpmax = minnex - 1;
        //                                            tmpmin = maxcurr + 1;
        //                                            if (minnex < maxcurr || minnex - maxcurr == 1)
        //                                            {
        //                                                tmpmin2 = tmpmin2 + "," + mincurr;
        //                                                continue;
        //                                            }
        //                                            tmpstr = tmpmin2.Split(',');
        //                                            dr = dt.NewRow();
        //                                            int min = 0;
        //                                            if (minnex > maxcurr && tmpstr.Length == 1)
        //                                            {
        //                                                //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(mincurr).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + mincurr - mintag + 1 + " " + maxcurr - mintag + 1;
        //                                                min = mincurr;
        //                                                tonfrom = mincurr - mintag + 1;
        //                                                tonto = maxcurr - mintag + 1;
        //                                            }
        //                                            else
        //                                            {
        //                                                //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(tmpstr[1]).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + Convert.ToInt32(tmpstr[1]) - Convert.ToInt32(mintag) + 1 + " " + maxcurr - mintag + 1;
        //                                                min = Convert.ToInt32(tmpstr[1]);
        //                                                tonfrom = Convert.ToInt32(tmpstr[1]) - mintag + 1;
        //                                                tonto = maxcurr - mintag + 1;
        //                                                tmpmin2 = "";
        //                                            }
        //                                            //dr["GRADE"] = grdstr;
        //                                            dr["GRD"] = grdcurr;
        //                                            dr["MIN"] = fntchar + Convert.ToInt32(min).ToString(fmt);
        //                                            dr["MAX"] = fntchar + Convert.ToInt32(maxcurr).ToString(fmt);
        //                                            dr["SRT"] = _datatableSumGrd.Rows[v]["SRT"].ToString();
        //                                            dr["TonFrom"] = tonfrom;
        //                                            dr["TonTo"] = tonto;
        //                                            dt.Rows.Add(dr);
        //                                            dr = dt.NewRow();
        //                                            //grdstr = notnggrd + " = " + fntchar + Convert.ToInt32(tmpmin).ToString(fmt) + " - " + fntchar + Convert.ToInt32(tmpmax).ToString(fmt) + " " + Convert.ToInt32(tmpmin) - Convert.ToInt32(mintag) + 1 + " " + Convert.ToInt32(tmpmax) - Convert.ToInt32(mintag) + 1;
        //                                            //dr["GRADE"] = grdstr;
        //                                            dr["GRD"] = notnggrd;
        //                                            dr["MIN"] = fntchar + Convert.ToInt32(tmpmin).ToString(fmt);
        //                                            dr["MAX"] = fntchar + Convert.ToInt32(tmpmax).ToString(fmt);
        //                                            dr["SRT"] = notngsrt;
        //                                            dr["TonFrom"] = tmpmin - mintag + 1;
        //                                            dr["TonTo"] = tmpmax - mintag + 1;
        //                                            dt.Rows.Add(dr);
        //                                        }
        //                                    }
        //                                    else if (v == _datatableSumGrd.Rows.Count - 1 && maxcurr != maxtag)
        //                                    {
        //                                        dr = dt.NewRow();
        //                                        tmpstr = tmpmin2.Split(',');
        //                                        int min = 0;
        //                                        if (tmpstr.Count() > 1)
        //                                        {
        //                                            //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(tmpstr[1]).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + tmpstr[1] - mintag + 1 + " " + maxcurr - mintag + 1;
        //                                            min = Convert.ToInt32(tmpstr[1]);
        //                                            tonfrom = Convert.ToInt32(tmpstr[1]) - mintag + 1;
        //                                            tonto = maxcurr - mintag + 1;
        //                                            tmpmin2 = "";
        //                                        }
        //                                        else if (tmpstr.Count() == 1)
        //                                        {
        //                                            //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(mincurr).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + mincurr - mintag + 1 + " " + maxcurr - mintag + 1;
        //                                            min = mincurr;
        //                                            tonfrom = mincurr - mintag + 1;
        //                                            tonto = maxcurr - mintag + 1;
        //                                        }
        //                                        //dr["GRADE"] = grdstr;
        //                                        dr["GRD"] = grdcurr;
        //                                        dr["MIN"] = min;
        //                                        dr["MAX"] = fntchar + Convert.ToInt32(maxcurr).ToString(fmt);
        //                                        dr["SRT"] = _datatableSumGrd.Rows[v]["SRT"].ToString();
        //                                        dr["TonFrom"] = tonfrom;
        //                                        dr["TonTo"] = tonto;
        //                                        dt.Rows.Add(dr);
        //                                        dr = dt.NewRow();
        //                                        //grdstr = notnggrd + " = " + fntchar + Convert.ToInt32(maxcurr + 1).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxtag).ToString(fmt) + " " + Convert.ToInt32(maxcurr + 1) - Convert.ToInt32(mintag) + 1 + " " + Convert.ToInt32(maxtag) - Convert.ToInt32(mintag) + 1;
        //                                        //dr["GRADE"] = grdstr;
        //                                        dr["GRD"] = _datatableSumGrd.Rows[v - 1]["GRADE"].ToString();
        //                                        dr["MIN"] = fntchar + Convert.ToInt32(maxcurr + 1).ToString(fmt);
        //                                        dr["MAX"] = fntchar + Convert.ToInt32(maxtag).ToString(fmt);
        //                                        dr["SRT"] = notngsrt;
        //                                        dr["TonFrom"] = maxcurr + 1 - mintag + 1;
        //                                        dr["TonTo"] = maxtag - mintag + 1;
        //                                        dt.Rows.Add(dr);
        //                                    }
        //                                    else if (v == _datatableSumGrd.Rows.Count - 1 && maxcurr == maxtag)
        //                                    {
        //                                        dr = dt.NewRow();
        //                                        int min = 0;
        //                                        tmpstr = tmpmin2.Split(',');
        //                                        if (tmpstr.Length != 1)
        //                                        {
        //                                            //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(tmpstr[1]).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + tmpstr[1] - mintag + 1 + " " + maxcurr - mintag + 1;
        //                                            min = Convert.ToInt32(tmpstr[1]);
        //                                            tonfrom = Convert.ToInt32(tmpstr[1]) - mintag + 1;
        //                                            tonto = maxcurr - mintag + 1;
        //                                            tmpmin2 = "";
        //                                        }
        //                                        else
        //                                        {
        //                                            //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(mincurr).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + mincurr - mintag + 1 + " " + maxcurr - mintag + 1;
        //                                            min = mincurr;
        //                                            tonfrom = mincurr - mintag + 1;
        //                                            tonto = maxcurr - mintag + 1;
        //                                        }
        //                                        //dr["GRADE"] = grdstr;
        //                                        dr["GRD"] = grdcurr;
        //                                        dr["MIN"] = fntchar + Convert.ToInt32(min).ToString(fmt);
        //                                        dr["MAX"] = fntchar + Convert.ToInt32(maxcurr).ToString(fmt);
        //                                        dr["SRT"] = _datatableSumGrd.Rows[v]["SRT"].ToString();
        //                                        dr["TonFrom"] = tonfrom;
        //                                        dr["TonTo"] = tonto;
        //                                        dt.Rows.Add(dr);
        //                                    }
        //                                }
        //                                else if (tsegfng == false && fsegfng == false && ssegfng && insdone2) //102130
        //                                {
        //                                    // set tmpmax a grade

        //                                    string[] tmpstr;

        //                                    if (v != _datatableSumGrd.Rows.Count - 1)
        //                                    {
        //                                        if (grdcurr == nggrd && grdnex == nggrd)
        //                                        {
        //                                            tmpmax = minnex - 1;
        //                                            tmpmin = maxcurr + 1;
        //                                            if (minnex - maxcurr <= 1)
        //                                            {
        //                                                tmpmin2 = tmpmin2 + "," + mincurr;
        //                                                continue;
        //                                            }
        //                                            tmpstr = tmpmin2.Split(',');

        //                                            if (mincurr - stmpmax > 1) //cannot combine
        //                                            {
        //                                                int min = 0;
        //                                                if (minnex > maxcurr && tmpstr.Length == 1)
        //                                                {
        //                                                    //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(mincurr).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + tonfrom + " " + tonto;
        //                                                    min = mincurr;
        //                                                    tonfrom = mincurr - mintag + 1;
        //                                                    tonto = maxcurr - mintag + 1;
        //                                                }
        //                                                else
        //                                                {
        //                                                    //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(tmpstr[1]).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + Convert.ToInt32(tmpstr[1]) - Convert.ToInt32(mintag) + 1 + " " + tonto;
        //                                                    min = Convert.ToInt32(tmpstr[1]);
        //                                                    tonfrom = Convert.ToInt32(tmpstr[1]) - mintag + 1;
        //                                                    tonto = maxcurr - mintag + 1;
        //                                                    tmpmin2 = "";
        //                                                }
        //                                                //dr["GRADE"] = grdstr;
        //                                                dr["GRD"] = grdcurr;
        //                                                dr["MIN"] = fntchar + Convert.ToInt32(mincurr).ToString(fmt);
        //                                                dr["MAX"] = fntchar + Convert.ToInt32(maxcurr).ToString(fmt);
        //                                                dr["SRT"] = _datatableSumGrd.Rows[v]["SRT"].ToString();
        //                                                dr["TonFrom"] = tonfrom;
        //                                                dr["TonTo"] = tonto;
        //                                                dt.Rows.Add(dr);
        //                                            }
        //                                            else if (mincurr < stmpmax || mincurr - stmpmax == 1) //can combine with second segment
        //                                            {
        //                                                var r = dt.Rows.Count - 1;
        //                                                //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["TonTo"].ToString(), maxcurr - mintag + 1);
        //                                                //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["Max"].ToString(), maxcurr);
        //                                                dt.Rows[r]["MAX"] = maxcurr;
        //                                                dt.Rows[r]["TonTo"] = maxcurr - mintag + 1;
        //                                                if (v == _datatableSumGrd.Rows.Count - 1 && maxcurr == maxtag)
        //                                                {
        //                                                    continue;
        //                                                }
        //                                                else if (v == _datatableSumGrd.Rows.Count - 1 && maxcurr != maxtag)
        //                                                {
        //                                                    dr = dt.NewRow();
        //                                                    //grdstr = notnggrd + " = " + fntchar + Convert.ToInt32(maxcurr + 1).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxtag).ToString(fmt) + " " + (maxcurr + 1 - mintag + 1).ToString() + " " + (maxtag - mintag + 1).ToString();
        //                                                    //dr["GRADE"] = grdstr;
        //                                                    dr["GRD"] = notnggrd;
        //                                                    dr["MIN"] = fntchar + Convert.ToInt32(maxcurr + 1).ToString(fmt);
        //                                                    dr["MAX"] = fntchar + Convert.ToInt32(maxtag).ToString(fmt);
        //                                                    dr["SRT"] = notngsrt;
        //                                                    dr["TonFrom"] = Convert.ToInt32(maxcurr + 1) - Convert.ToInt32(mintag) + 1;
        //                                                    dr["TonTo"] = Convert.ToInt32(maxtag) - Convert.ToInt32(mintag) + 1;
        //                                                    dt.Rows.Add(dr);
        //                                                    continue;
        //                                                }
        //                                            }
        //                                            dr = dt.NewRow();
        //                                            //grdstr = notnggrd + " = " + fntchar + Convert.ToInt32(tmpmin).ToString(fmt) + " - " + fntchar + Convert.ToInt32(tmpmax).ToString(fmt) + " " + Convert.ToInt32(tmpmin) - Convert.ToInt32(mintag) + 1 + " " + Convert.ToInt32(tmpmax) - Convert.ToInt32(mintag) + 1;
        //                                            dr["GRADE"] = grdstr;
        //                                            dr["GRD"] = notnggrd;
        //                                            dr["MIN"] = fntchar + Convert.ToInt32(tmpmin).ToString(fmt);
        //                                            dr["MAX"] = fntchar + Convert.ToInt32(tmpmax).ToString(fmt);
        //                                            dr["SRT"] = notngsrt;
        //                                            dr["TonFrom"] = tmpmin - mintag + 1;
        //                                            dr["TonTo"] = tmpmax - mintag + 1;
        //                                            dt.Rows.Add(dr);
        //                                        }
        //                                    }
        //                                    else if (v == _datatableSumGrd.Rows.Count - 1 && maxcurr != maxtag)
        //                                    {
        //                                        int min = 0;
        //                                        if (dt.Rows[dt.Rows.Count - 1]["GRD"].ToString() == notnggrd && Convert.ToInt32(dt.Rows[dt.Rows.Count - 1]["TonTo"].ToString()) <= stmpmax - mintag + 1)
        //                                        {
        //                                            dt.Rows[dt.Rows.Count - 1].Delete();
        //                                            dt.AcceptChanges();
        //                                        }
        //                                        if (mincurr - stmpmax > 1) //cannot combine with last segment
        //                                        {
        //                                            dr = dt.NewRow();
        //                                            //grdstr = notnggrd + " = " + fntchar + Convert.ToInt32(stmpmax + 1).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr - 1).ToString(fmt) + " " + Convert.ToInt32(stmpmax + 1) - Convert.ToInt32(mintag) + 1 + " " + mincurr - 1 - mintag + 1;
        //                                            tonfrom = stmpmax + 1 - mintag + 1;
        //                                            tonto = maxcurr - mintag + 1;
        //                                            //dr["GRADE"] = grdstr;
        //                                            dr["GRD"] = notnggrd;
        //                                            dr["MIN"] = fntchar + Convert.ToInt32(stmpmax + 1).ToString(fmt);
        //                                            dr["MAX"] = fntchar + Convert.ToInt32(mincurr - 1).ToString(fmt);
        //                                            dr["SRT"] = _datatableSumGrd.Rows[v]["SRT"].ToString();
        //                                            dr["TonFrom"] = stmpmax + 1 - mintag + 1;
        //                                            dr["TonTo"] = mincurr - 1 - mintag + 1;
        //                                            dt.Rows.Add(dr);
        //                                            dr = dt.NewRow();
        //                                            tmpstr = tmpmin2.Split(',');
        //                                            if (tmpstr.Length == 1)
        //                                            {
        //                                                //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(mincurr).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + mincurr - mintag + 1 + " " + maxcurr - mintag + 1;
        //                                                min = mincurr;
        //                                                tonfrom = mincurr - mintag + 1;
        //                                                tonto = maxcurr - mintag + 1;
        //                                            }
        //                                            else
        //                                            {
        //                                                //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(tmpstr[1]).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + Convert.ToInt32(tmpstr[1]) - Convert.ToInt32(mintag) + 1 + " " + maxcurr - mintag + 1;
        //                                                min = Convert.ToInt32(tmpstr[1]);
        //                                                tonfrom = Convert.ToInt32(tmpstr[1]) - mintag + 1;
        //                                                tonto = maxcurr - mintag + 1;
        //                                            }
        //                                            //dr["GRADE"] = grdstr;
        //                                            dr["GRD"] = grdcurr;
        //                                            dr["MIN"] = fntchar + Convert.ToInt32(min).ToString(fmt);
        //                                            dr["MAX"] = fntchar + Convert.ToInt32(maxcurr).ToString(fmt);
        //                                            dr["SRT"] = _datatableSumGrd.Rows[v]["SRT"].ToString();
        //                                            dr["TonFrom"] = tonfrom;
        //                                            dr["TonTo"] = tonto;
        //                                            dt.Rows.Add(dr);
        //                                        }
        //                                        else if (mincurr < stmpmax || mincurr - stmpmax == 1) //can combine
        //                                        {
        //                                            var r = dt.Rows.Count - 1;
        //                                            //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["TonTo"].ToString(), maxcurr - mintag + 1);
        //                                            //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["Max"].ToString(), maxcurr);
        //                                            dt.Rows[r]["MAX"] = maxcurr;
        //                                            dt.Rows[r]["TonTo"] = maxcurr - mintag + 1;
        //                                            dr = dt.NewRow();
        //                                            //grdstr = notnggrd + " = " + fntchar + Convert.ToInt32(maxcurr + 1).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxtag).ToString(fmt) + " " + (maxcurr + 1 - mintag + 1).ToString() + " " + (maxtag - mintag + 1).ToString();
        //                                            //dr["GRADE"] = grdstr;
        //                                            dr["GRD"] = notnggrd;
        //                                            dr["MIN"] = fntchar + Convert.ToInt32(maxcurr + 1).ToString(fmt);
        //                                            dr["MAX"] = fntchar + Convert.ToInt32(maxtag).ToString(fmt);
        //                                            dr["SRT"] = notngsrt;
        //                                            dr["TonFrom"] = Convert.ToInt32(maxcurr + 1) - Convert.ToInt32(mintag) + 1;
        //                                            dr["TonTo"] = Convert.ToInt32(maxtag) - Convert.ToInt32(mintag) + 1;
        //                                            dt.Rows.Add(dr);
        //                                            continue;
        //                                        }
        //                                        dr = dt.NewRow();
        //                                        //grdstr = notnggrd + " = " + fntchar + Convert.ToInt32(maxcurr + 1).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxtag).ToString(fmt) + " " + Convert.ToInt32(maxcurr + 1) - Convert.ToInt32(mintag) + 1 + " " + Convert.ToInt32(maxtag) - Convert.ToInt32(mintag) + 1;
        //                                        //dr["GRADE"] = grdstr;
        //                                        dr["GRD"] = _datatableSumGrd.Rows[v - 1]["GRADE"].ToString();
        //                                        dr["MIN"] = fntchar + Convert.ToInt32(maxcurr + 1).ToString(fmt);
        //                                        dr["MAX"] = fntchar + Convert.ToInt32(maxtag).ToString(fmt);
        //                                        dr["SRT"] = notngsrt;
        //                                        dr["TonFrom"] = maxcurr + 1 - mintag - mintag + 1;
        //                                        dr["TonTo"] = maxtag - mintag + 1;
        //                                        dt.Rows.Add(dr);
        //                                    }
        //                                    else if (v == _datatableSumGrd.Rows.Count - 1 && maxcurr == maxtag)
        //                                    {
        //                                        int min = 0;
        //                                        dr = dt.NewRow();
        //                                        tmpstr = tmpmin2.Split(',');
        //                                        if (tmpstr.Length == 1)
        //                                        {
        //                                            //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(mincurr).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + mincurr - mintag + 1 + " " + maxcurr - mintag + 1;
        //                                            min = mincurr;
        //                                            tonfrom = mincurr - mintag + 1;
        //                                            tonto = maxcurr - mintag + 1;
        //                                        }
        //                                        else
        //                                        {
        //                                            //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(tmpstr[1]).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + Convert.ToInt32(tmpstr[1]) - Convert.ToInt32(mintag) + 1 + " " + maxcurr - mintag + 1;
        //                                            min = Convert.ToInt32(tmpstr[1]);
        //                                            tonfrom = Convert.ToInt32(tmpstr[1]) - mintag + 1;
        //                                            tonto = maxcurr - mintag + 1;
        //                                        }
        //                                        //dr["GRADE"] = grdstr;
        //                                        dr["GRD"] = grdcurr;
        //                                        dr["MIN"] = fntchar + Convert.ToInt32(mincurr).ToString(fmt);
        //                                        dr["MAX"] = fntchar + Convert.ToInt32(maxcurr).ToString(fmt);
        //                                        dr["SRT"] = _datatableSumGrd.Rows[v]["SRT"].ToString();
        //                                        dr["TonFrom"] = tonfrom;
        //                                        dr["TonTo"] = tonto;
        //                                        dt.Rows.Add(dr);
        //                                    }
        //                                }
        //                                else if (tsegfng && fsegfng && ssegfng == false && insdone2) //112031
        //                                {
        //                                    if (v == Convert.ToInt32(tngrow[0]))
        //                                    {
        //                                        if (dt.Rows[dt.Rows.Count - 1]["GRD"].ToString() == notnggrd && Convert.ToInt32(dt.Rows[dt.Rows.Count - 1]["TonFrom"].ToString()) >= ttmpmin - mintag + 1)
        //                                        {
        //                                            dt.Rows[dt.Rows.Count - 1].Delete();
        //                                            dt.AcceptChanges();
        //                                        }
        //                                        var r = dt.Rows.Count - 1;
        //                                        if (dt.Rows[r]["GRD"].ToString() == notnggrd && (Convert.ToInt32(dt.Rows[r]["MAX"].ToString()) != ttmpmin || ttmpmin - Convert.ToInt32(dt.Rows[r]["MAX"].ToString()) == 1))
        //                                        {
        //                                            //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["TonTo"].ToString(), ttmpmin - 1 - mintag + 1);
        //                                            //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["Max"].ToString(), ttmpmin - 1);
        //                                            dt.Rows[r]["MAX"] = ttmpmin - 1;
        //                                            dt.Rows[r]["TonTo"] = ttmpmin - 1 - mintag + 1;
        //                                        }
        //                                        else if (dt.Rows[r]["GRD"].ToString() == nggrd && (Convert.ToInt32(dt.Rows[r]["MAX"].ToString()) > ttmpmin || ttmpmin - Convert.ToInt32(dt.Rows[r]["MAX"].ToString()) == 1))
        //                                        {
        //                                            //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["TonTo"].ToString(), ttmpmax - mintag + 1);
        //                                            //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["Max"].ToString(), ttmpmax);
        //                                            dt.Rows[r]["MAX"] = ttmpmax;
        //                                            dt.Rows[r]["TonTo"] = ttmpmax - mintag + 1;
        //                                        }
        //                                        continue;
        //                                    }
        //                                    else if (v < Convert.ToInt32(tngrow[tngrow.Count - 1]))
        //                                        continue;
        //                                    else if (v == Convert.ToInt32(tngrow[tngrow.Count - 1]))
        //                                    {
        //                                        if (dt.Rows[dt.Rows.Count - 1]["GRD"].ToString() == notnggrd)
        //                                        {
        //                                            dr = dt.NewRow();
        //                                            //grdstr = _datatableSumGrd.Rows[v]["GRADE"].ToString() + " = " + fntchar + Convert.ToInt32(ttmpmin).ToString(fmt) + " - " + fntchar + Convert.ToInt32(ttmpmax).ToString(fmt) + " " + ttmpmin - mintag + 1 + " " + ttmpmax - mintag + 1;
        //                                            //dr["GRADE"] = grdstr;
        //                                            dr["GRD"] = _datatableSumGrd.Rows[v]["GRADE"].ToString();
        //                                            dr["MIN"] = fntchar + Convert.ToInt32(ttmpmin).ToString(fmt);
        //                                            dr["MAX"] = fntchar + Convert.ToInt32(ttmpmax).ToString(fmt);
        //                                            dr["SRT"] = ngsrt;
        //                                            dr["TonFrom"] = ttmpmin - mintag + 1;
        //                                            dr["TonTo"] = ttmpmax - mintag + 1;
        //                                            dt.Rows.Add(dr);
        //                                        }
        //                                    }
        //                                }
        //                                else if (tsegfng && ssegfng && fsegfng == false && insdone2) //102131
        //                                {
        //                                    if (dt.Rows[dt.Rows.Count - 1]["GRD"].ToString() == notnggrd && Convert.ToInt32(dt.Rows[dt.Rows.Count - 1]["TonTo"].ToString()) <= stmpmax - mintag + 1)
        //                                    {
        //                                        dt.Rows[dt.Rows.Count - 1].Delete();
        //                                        dt.AcceptChanges();
        //                                    }
        //                                    var r = dt.Rows.Count - 1;
        //                                    //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["TonTo"].ToString(), maxtag - mintag + 1);
        //                                    //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["Max"].ToString(), maxtag);
        //                                    dt.Rows[r]["MAX"] = maxtag;
        //                                    dt.Rows[r]["TonTo"] = maxtag - mintag + 1;
        //                                    insdone2 = true;
        //                                }
        //                                else if (tsegfng && fsegfng == false && ssegfng == false)
        //                                {
        //                                    if (v >= Convert.ToInt32(tngrow[0]) && v < Convert.ToInt32(tngrow[tngrow.Count - 1]))
        //                                        continue;
        //                                    if (v == Convert.ToInt32(tngrow[tngrow.Count - 1]))
        //                                    {
        //                                        var r = dt.Rows.Count - 1;
        //                                        if (dt.Rows[r]["GRD"].ToString() == notnggrd && Convert.ToInt32(dt.Rows[r]["MIN"].ToString()) >= ttmpmin)
        //                                        {
        //                                            dt.Rows[r].Delete();
        //                                            dt.AcceptChanges();
        //                                            r = dt.Rows.Count - 1;
        //                                        }
        //                                        if (dt.Rows[r]["GRD"].ToString() == notnggrd && (Convert.ToInt32(dt.Rows[r]["MIN"].ToString()) < ttmpmin && Convert.ToInt32(dt.Rows[r]["MAX"].ToString()) > ttmpmin || ttmpmin - Convert.ToInt32(dt.Rows[r]["MAX"].ToString()) == 1))
        //                                        {
        //                                            //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["TonTo"].ToString(), ttmpmin - 1 - mintag + 1);
        //                                            //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["Max"].ToString(), ttmpmin - 1);
        //                                            dt.Rows[r]["MAX"] = ttmpmin - 1;
        //                                            dt.Rows[r]["TonTo"] = ttmpmin - 1 - mintag + 1;
        //                                            dr = dt.NewRow();
        //                                            //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(ttmpmin).ToString(fmt) + " - " + fntchar + Convert.ToInt32(ttmpmax).ToString(fmt) + " " + Convert.ToInt32(ttmpmin) - Convert.ToInt32(mintag) + 1 + " " + Convert.ToInt32(ttmpmax) - Convert.ToInt32(mintag) + 1;
        //                                            //dr["GRADE"] = grdstr;
        //                                            dr["GRD"] = grdcurr;
        //                                            dr["MIN"] = fntchar + Convert.ToInt32(tmpmin).ToString(fmt);
        //                                            dr["MAX"] = fntchar + Convert.ToInt32(tmpmax).ToString(fmt);
        //                                            dr["SRT"] = ngsrt;
        //                                            dr["TonFrom"] = ttmpmin - mintag + 1;
        //                                            dr["TonTo"] = ttmpmax - mintag + 1;
        //                                            dt.Rows.Add(dr);
        //                                        }
        //                                        else if (dt.Rows[r]["GRD"].ToString() == nggrd && (Convert.ToInt32(dt.Rows[r]["MAX"].ToString()) > ttmpmin || ttmpmin - Convert.ToInt32(dt.Rows[r]["MAX"].ToString()) == 1))
        //                                        {
        //                                            //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["TonTo"].ToString(), ttmpmax - mintag + 1);
        //                                            //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["Max"].ToString(), ttmpmax);
        //                                            dt.Rows[r]["MAX"] = ttmpmax;
        //                                            dt.Rows[r]["TonTo"] = ttmpmax - mintag + 1;
        //                                        }
        //                                        continue;
        //                                    }

        //                                    if (v == 0)
        //                                    {
        //                                        if (grdcurr != nggrd)
        //                                        {
        //                                            tmpmin = mincurr;
        //                                            string[] tmpstr;
        //                                            tmpstr = tmpmin2.Split(',');
        //                                            if (tmpstr.Count() == 1)
        //                                                continue;
        //                                        }
        //                                        else if (grdcurr == nggrd)
        //                                        {
        //                                            if (grdnex == nggrd && (minnex < maxcurr || minnex - maxcurr == 1))
        //                                            {
        //                                                tmpmin2 = tmpmin2 + "," + mincurr;
        //                                                if (minnex - mintag + 1 < 36)
        //                                                    continue;
        //                                            }
        //                                            tmpmin = maxcurr + 1;
        //                                            dr = dt.NewRow();
        //                                            //grdstr = _datatableSumGrd.Rows[v]["GRADE"].ToString() + " = " + fntchar + Convert.ToInt32(mincurr).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + mincurr - mintag + 1 + " " + maxcurr - mintag + 1;
        //                                            //dr["GRADE"] = grdstr;
        //                                            dr["GRD"] = _datatableSumGrd.Rows[v]["GRADE"].ToString();
        //                                            dr["MIN"] = fntchar + Convert.ToInt32(mincurr).ToString(fmt);
        //                                            dr["MAX"] = fntchar + Convert.ToInt32(maxcurr).ToString(fmt);
        //                                            dr["SRT"] = ngsrt;
        //                                            dr["TonFrom"] = mincurr - mintag + 1;
        //                                            dr["TonTo"] = maxcurr - mintag + 1;
        //                                            dt.Rows.Add(dr);
        //                                        }
        //                                    }
        //                                    else if (v != _datatableSumGrd.Rows.Count - 1)
        //                                    {
        //                                        if (grdnex != nggrd)
        //                                        {
        //                                            string[] tmpstr;
        //                                            tmpstr = tmpmin2.Split(',');
        //                                            if (tmpstr.Count() == 1)
        //                                                continue;
        //                                        }
        //                                        else if (grdcurr != nggrd && grdnex == nggrd)
        //                                        {
        //                                            tmpmax = minnex - 1;
        //                                            dr = dt.NewRow();
        //                                            if (minnex > tmpmin - 1 && minnex - tmpmin - 1 != 1 && minnex - tmpmin - 1 > 0)
        //                                            {
        //                                                //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(tmpmin).ToString(fmt) + " - " + fntchar + Convert.ToInt32(tmpmax).ToString(fmt) + " " + Convert.ToInt32(tmpmin) - Convert.ToInt32(mintag) + 1 + " " + Convert.ToInt32(tmpmax) - Convert.ToInt32(mintag) + 1;
        //                                                //dr["GRADE"] = grdstr;
        //                                                dr["GRD"] = grdcurr;
        //                                                dr["MIN"] = fntchar + Convert.ToInt32(tmpmin).ToString(fmt);
        //                                                dr["MAX"] = fntchar + Convert.ToInt32(tmpmax).ToString(fmt);
        //                                                dr["SRT"] = notngsrt;
        //                                                dr["TonFrom"] = tmpmin - mintag + 1;
        //                                                dr["TonTo"] = tmpmax - mintag + 1;
        //                                                dt.Rows.Add(dr);
        //                                            }
        //                                            else
        //                                            {
        //                                                var r = dt.Rows.Count - 1;
        //                                                //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["TonTo"].ToString(), maxnex - mintag + 1);
        //                                                //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["Max"].ToString(), maxnex);
        //                                                dt.Rows[r]["MAX"] = maxnex;
        //                                                dt.Rows[r]["TonTo"] = maxnex - mintag + 1;
        //                                            }
        //                                        }
        //                                        else if (grdcurr == nggrd && grdnex == nggrd)
        //                                        {
        //                                            if (grdnex == nggrd && (minnex < maxcurr || minnex - maxcurr == 1))
        //                                            {
        //                                                tmpmin2 = tmpmin2 + "," + mincurr;
        //                                                if (minnex - mintag + 1 < 36)
        //                                                    continue;
        //                                            }
        //                                            tmpmax = minnex - 1;
        //                                            tmpmin = maxcurr + 1;
        //                                            string[] tmpstr;
        //                                            if (minnex < maxcurr || (minnex - maxcurr == 1 && minnex - maxcurr > 0) && minnex - mintag + 1 < 76)
        //                                            {
        //                                                tmpmin2 = tmpmin2 + "," + mincurr;
        //                                                continue;
        //                                            }
        //                                            tmpstr = tmpmin2.Split(',');
        //                                            var r = dt.Rows.Count - 1;
        //                                            if (Convert.ToInt32(dt.Rows[r]["MAX"].ToString()) != maxcurr && mincurr > Convert.ToInt32(dt.Rows[r]["MAX"].ToString()))
        //                                            {
        //                                                int min = 0;
        //                                                dr = dt.NewRow();
        //                                                if (minnex > maxcurr && tmpstr.Length == 1)
        //                                                {
        //                                                    //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(mincurr).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + mincurr - mintag + 1 + " " + maxcurr - mintag + 1;
        //                                                    min = mincurr;
        //                                                    tonfrom = mincurr - mintag + 1;
        //                                                    tonto = maxcurr - mintag + 1;
        //                                                }
        //                                                else
        //                                                {
        //                                                    //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(tmpstr[1]).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + Convert.ToInt32(tmpstr[1]) - Convert.ToInt32(mintag) + 1 + " " + maxcurr - mintag + 1;
        //                                                    min = Convert.ToInt32(tmpstr[1]);
        //                                                    tonfrom = Convert.ToInt32(tmpstr[1]) - mintag + 1;
        //                                                    tonto = maxcurr - mintag + 1;
        //                                                    tmpmin2 = "";
        //                                                }
        //                                                //dr["GRADE"] = grdstr;
        //                                                dr["GRD"] = grdcurr;
        //                                                dr["MIN"] = min;
        //                                                dr["MAX"] = fntchar + Convert.ToInt32(maxcurr).ToString(fmt);
        //                                                dr["SRT"] = _datatableSumGrd.Rows[v]["SRT"].ToString();
        //                                                dr["TonFrom"] = tonfrom;
        //                                                dr["TonTo"] = tonto;
        //                                                dt.Rows.Add(dr);
        //                                                tmpmin2 = "";
        //                                            }
        //                                            else if (Convert.ToInt32(dt.Rows[r]["MAX"].ToString()) != maxcurr && mincurr < Convert.ToInt32(dt.Rows[r]["MAX"].ToString()))
        //                                            {
        //                                                //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["TonTo"].ToString(), maxcurr - mintag + 1);
        //                                                //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["Max"].ToString(), maxcurr);
        //                                                dt.Rows[r]["MAX"] = maxcurr;
        //                                                dt.Rows[r]["TonTo"] = maxcurr - mintag + 1;
        //                                                tmpmin2 = "";
        //                                            }
        //                                            if (minnex - maxcurr > 1)
        //                                            {
        //                                                dr = dt.NewRow();
        //                                                //grdstr = notnggrd + " = " + fntchar + Convert.ToInt32(tmpmin).ToString(fmt) + " - " + fntchar + Convert.ToInt32(tmpmax).ToString(fmt) + " " + Convert.ToInt32(tmpmin) - Convert.ToInt32(mintag) + 1 + " " + Convert.ToInt32(tmpmax) - Convert.ToInt32(mintag) + 1;
        //                                                //dr["GRADE"] = grdstr;
        //                                                dr["GRD"] = notnggrd;
        //                                                dr["MIN"] = fntchar + Convert.ToInt32(tmpmin).ToString(fmt);
        //                                                dr["MAX"] = fntchar + Convert.ToInt32(tmpmax).ToString(fmt);
        //                                                dr["SRT"] = notngsrt;
        //                                                dr["TonFrom"] = tmpmin - mintag + 1;
        //                                                dr["TonTo"] = tmpmax - mintag + 1;
        //                                                dt.Rows.Add(dr);
        //                                            }
        //                                        }
        //                                        if (grdcurr == nggrd && grdnex != nggrd)
        //                                        {
        //                                            string[] tmpstr;

        //                                            tmpstr = tmpmin2.Split(',');
        //                                            dr = dt.NewRow();
        //                                            //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(tmpstr[1]).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + Convert.ToInt32(tmpstr[1]) - Convert.ToInt32(mintag) + 1 + " " + maxcurr - mintag + 1;
        //                                            tonfrom = Convert.ToInt32(tmpstr[1]) - mintag + 1;
        //                                            tonto = maxcurr - mintag + 1;
        //                                            dr["GRADE"] = grdstr;
        //                                            dr["GRD"] = grdcurr;
        //                                            dr["MIN"] = fntchar + Convert.ToInt32(tmpstr[1]).ToString(fmt);
        //                                            dr["MAX"] = fntchar + Convert.ToInt32(maxcurr).ToString(fmt);
        //                                            dr["SRT"] = _datatableSumGrd.Rows[v]["SRT"].ToString();
        //                                            dr["TonFrom"] = tonfrom;
        //                                            dr["TonTo"] = tonto;
        //                                            dt.Rows.Add(dr);
        //                                            tmpmin = maxcurr + 1;
        //                                            tmpmin2 = "";
        //                                        }
        //                                    }
        //                                    else if (v == _datatableSumGrd.Rows.Count - 1 && maxcurr != maxtag)
        //                                    {
        //                                        dr = dt.NewRow();
        //                                        //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(mincurr).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + mincurr - mintag + 1 + " " + maxcurr - mintag + 1;
        //                                        //dr["GRADE"] = grdstr;
        //                                        dr["GRD"] = grdcurr;
        //                                        dr["MIN"] = fntchar + Convert.ToInt32(mincurr).ToString(fmt);
        //                                        dr["MAX"] = fntchar + Convert.ToInt32(maxcurr).ToString(fmt);
        //                                        dr["SRT"] = _datatableSumGrd.Rows[v]["SRT"].ToString();
        //                                        dr["TonFrom"] = mincurr - mintag + 1;
        //                                        dr["TonTo"] = maxcurr - mintag + 1;
        //                                        dt.Rows.Add(dr);
        //                                        dr = dt.NewRow();
        //                                        //grdstr = notnggrd + " = " + fntchar + Convert.ToInt32(maxcurr + 1).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxtag).ToString(fmt) + " " + Convert.ToInt32(maxcurr + 1) - Convert.ToInt32(mintag) + 1 + " " + Convert.ToInt32(maxtag) - Convert.ToInt32(mintag) + 1;
        //                                        //dr["GRADE"] = grdstr;
        //                                        dr["GRD"] = notnggrd;
        //                                        dr["MIN"] = fntchar + Convert.ToInt32(maxcurr + 1).ToString(fmt);
        //                                        dr["MAX"] = fntchar + Convert.ToInt32(maxtag).ToString(fmt);
        //                                        dr["SRT"] = notngsrt;
        //                                        dr["TonFrom"] = maxcurr - mintag + 1;
        //                                        dr["TonTo"] = maxtag - mintag + 1;
        //                                        dt.Rows.Add(dr);
        //                                    }
        //                                    else if (v == _datatableSumGrd.Rows.Count - 1 && maxcurr == maxtag)
        //                                    {
        //                                        var r = dt.Rows.Count - 1;
        //                                        if (dt.Rows[dt.Rows.Count - 1]["GRD"].ToString() == notnggrd)
        //                                        {
        //                                            dr = dt.NewRow();
        //                                            //grdstr = _datatableSumGrd.Rows[v]["GRADE"].ToString() + " = " + fntchar + Convert.ToInt32(ttmpmin).ToString(fmt) + " - " + fntchar + Convert.ToInt32(ttmpmax).ToString(fmt) + " " + ttmpmin - mintag + 1 + " " + ttmpmax - mintag + 1;
        //                                            //dr["GRADE"] = grdstr;
        //                                            dr["GRD"] = _datatableSumGrd.Rows[v]["GRADE"].ToString();
        //                                            dr["MIN"] = fntchar + Convert.ToInt32(ttmpmin).ToString(fmt);
        //                                            dr["MAX"] = fntchar + Convert.ToInt32(ttmpmax).ToString(fmt);
        //                                            dr["SRT"] = ngsrt;
        //                                            dr["TonFrom"] = ttmpmin - mintag + 1;
        //                                            dr["TonTo"] = ttmpmax - mintag + 1;
        //                                            dt.Rows.Add(dr);
        //                                        }
        //                                        else
        //                                        {
        //                                            //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["TonTo"].ToString(), ttmpmin - 1 - mintag + 1);
        //                                            //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["Max"].ToString(), ttmpmin - 1);
        //                                            dt.Rows[r]["MAX"] = ttmpmin - 1;
        //                                            dt.Rows[r]["TonTo"] = ttmpmin - 1 - mintag + 1;
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }


        //            }
        //            else //no full ng
        //            {
        //                if (_datatableSumGrd != null && _datatableSumGrd.Rows.Count > 0)
        //                {
        //                    for (var v = 0; v <= _datatableSumGrd.Rows.Count - 1; v++)
        //                    {
        //                        if (v == 0)
        //                        {
        //                            fntchar = _datatableSumGrd.Rows[0]["TAGNOCHAR"].ToString();
        //                            for (var fmti = 1; fmti <= Convert.ToInt32(_datatableSumGrd.Rows[0]["LENTAGNO"].ToString()); fmti++)
        //                                fmt = fmt + "0";
        //                        }

        //                        mincurr = Convert.ToInt32(_datatableSumGrd.Rows[v]["MINTAG"].ToString());
        //                        maxcurr = Convert.ToInt32(_datatableSumGrd.Rows[v]["MAXTAG"].ToString());
        //                        grdcurr = _datatableSumGrd.Rows[v]["GRADE"].ToString();

        //                        if (v < _datatableSumGrd.Rows.Count - 1)
        //                        {
        //                            minnex = Convert.ToInt32(_datatableSumGrd.Rows[v + 1]["MINTAG"].ToString());
        //                            maxnex = Convert.ToInt32(_datatableSumGrd.Rows[v + 1]["MAXTAG"].ToString());
        //                            grdnex = _datatableSumGrd.Rows[v + 1]["GRADE"].ToString();
        //                        }

        //                        if (v == 0)
        //                        {
        //                            if (grdcurr != nggrd)
        //                            {
        //                                tmpmin = mincurr;
        //                                continue;
        //                            }
        //                            else if (grdcurr == nggrd)
        //                            {
        //                                if (grdnex == nggrd && (minnex < maxcurr || minnex - maxcurr == 1))
        //                                {
        //                                    tmpmin2 = tmpmin2 + "," + mincurr;
        //                                    if (minnex - mintag + 1 < 36)
        //                                        continue;
        //                                }
        //                                tmpmin = maxcurr + 1;
        //                                dr = dt.NewRow();
        //                                //grdstr = _datatableSumGrd.Rows[v]["GRADE"].ToString() + " = " + fntchar + Convert.ToInt32(mincurr).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + mincurr - mintag + 1 + " " + maxcurr - mintag + 1;
        //                                //dr["GRADE"] = grdstr;
        //                                dr["GRD"] = _datatableSumGrd.Rows[v]["GRADE"].ToString();
        //                                dr["MIN"] = fntchar + Convert.ToInt32(mincurr).ToString(fmt);
        //                                dr["MAX"] = fntchar + Convert.ToInt32(maxcurr).ToString(fmt);
        //                                dr["SRT"] = ngsrt;
        //                                dr["TonFrom"] = mincurr - mintag + 1;
        //                                dr["TonTo"] = maxcurr - mintag + 1;
        //                                dt.Rows.Add(dr);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            minprev = Convert.ToInt32(_datatableSumGrd.Rows[v - 1]["MINTAG"].ToString());
        //                            maxprev = Convert.ToInt32(_datatableSumGrd.Rows[v - 1]["MAXTAG"].ToString());
        //                            grdprev = _datatableSumGrd.Rows[v - 1]["GRADE"].ToString();
        //                            if (v != _datatableSumGrd.Rows.Count - 1)
        //                            {
        //                                if (grdcurr == nggrd && grdnex != nggrd)
        //                                {
        //                                    string[] tmpstr;
        //                                    int min = 0;
        //                                    tmpstr = tmpmin2.Split(',');
        //                                    var r = dt.Rows.Count - 1;

        //                                    dr = dt.NewRow();
        //                                    if (minnex > maxcurr && tmpstr.Length == 1)
        //                                    {
        //                                        //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(mincurr).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + mincurr - mintag + 1 + " " + maxcurr - mintag + 1;
        //                                        min = mincurr;
        //                                        tonfrom = mincurr - mintag + 1;
        //                                        tonto = maxcurr - mintag + 1;
        //                                    }
        //                                    else
        //                                    {
        //                                        //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(tmpstr[1]).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + Convert.ToInt32(tmpstr[1]) - Convert.ToInt32(mintag) + 1 + " " + maxcurr - mintag + 1;
        //                                        min = Convert.ToInt32(tmpstr[1]);
        //                                        tonfrom = Convert.ToInt32(tmpstr[1]) - mintag + 1;
        //                                        tonto = maxcurr - mintag + 1;
        //                                        tmpmin2 = "";
        //                                    }
        //                                    //dr["GRADE"] = grdstr;
        //                                    dr["GRD"] = grdcurr;
        //                                    dr["MIN"] = fntchar + Convert.ToInt32(min).ToString(fmt);
        //                                    dr["MAX"] = fntchar + Convert.ToInt32(maxcurr).ToString(fmt);
        //                                    dr["SRT"] = _datatableSumGrd.Rows[v]["SRT"].ToString();
        //                                    dr["TonFrom"] = tonfrom;
        //                                    dr["TonTo"] = tonto;
        //                                    dt.Rows.Add(dr);
        //                                    tmpmin = maxcurr + 1;
        //                                    tmpmin2 = "";
        //                                }
        //                                if (grdnex != nggrd)
        //                                    continue;
        //                                else if (grdcurr != nggrd && grdnex == nggrd)
        //                                {
        //                                    tmpmax = minnex - 1;
        //                                    if (minnex > tmpmin - 1 && minnex - tmpmin - 1 != 1 && minnex - tmpmin - 1 > 0)
        //                                    {
        //                                        dr = dt.NewRow();
        //                                        //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(tmpmin).ToString(fmt) + " - " + fntchar + Convert.ToInt32(tmpmax).ToString(fmt) + " " + Convert.ToInt32(tmpmin) - Convert.ToInt32(mintag) + 1 + " " + Convert.ToInt32(tmpmax) - Convert.ToInt32(mintag) + 1;
        //                                        //dr["GRADE"] = grdstr;
        //                                        dr["GRD"] = grdcurr;
        //                                        dr["MIN"] = fntchar + Convert.ToInt32(tmpmin).ToString(fmt);
        //                                        dr["MAX"] = fntchar + Convert.ToInt32(tmpmax).ToString(fmt);
        //                                        dr["SRT"] = notngsrt;
        //                                        dr["TonFrom"] = tmpmin - mintag + 1;
        //                                        dr["TonTo"] = tmpmax - mintag + 1;
        //                                        dt.Rows.Add(dr);
        //                                    }
        //                                    else
        //                                    {
        //                                        var r = dt.Rows.Count - 1;
        //                                        //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["TonTo"].ToString(), maxnex - mintag + 1);
        //                                        //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["Max"].ToString(), maxnex);
        //                                        dt.Rows[r]["MAX"] = maxnex;
        //                                        dt.Rows[r]["TonTo"] = maxnex - mintag + 1;
        //                                    }
        //                                }
        //                                else if (grdcurr == nggrd && grdnex == nggrd)
        //                                {
        //                                    if (grdnex == nggrd && (minnex < maxcurr || minnex - maxcurr == 1))
        //                                    {
        //                                        tmpmin2 = tmpmin2 + "," + mincurr;
        //                                        if (minnex - mintag + 1 < 36)
        //                                            continue;
        //                                    }
        //                                    tmpmax = minnex - 1;
        //                                    tmpmin = maxcurr + 1;
        //                                    string[] tmpstr;
        //                                    if (minnex < maxcurr || minnex - maxcurr == 1)
        //                                    {
        //                                        tmpmin2 = tmpmin2 + "," + mincurr;
        //                                        continue;
        //                                    }
        //                                    tmpstr = tmpmin2.Split(',');
        //                                    var r = dt.Rows.Count - 1;
        //                                    if (Convert.ToInt32(dt.Rows[r]["MAX"].ToString()) != maxcurr && mincurr > Convert.ToInt32(dt.Rows[r]["MAX"].ToString()))
        //                                    {
        //                                        int min = 0;
        //                                        dr = dt.NewRow();
        //                                        if (minnex > maxcurr && tmpstr.Length == 1)
        //                                        {
        //                                            //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(mincurr).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + mincurr - mintag + 1 + " " + maxcurr - mintag + 1;
        //                                            min = mincurr;
        //                                            tonfrom = mincurr - mintag + 1;
        //                                            tonto = maxcurr - mintag + 1;
        //                                        }
        //                                        else
        //                                        {
        //                                            //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(tmpstr[1]).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + Convert.ToInt32(tmpstr[1]) - Convert.ToInt32(mintag) + 1 + " " + maxcurr - mintag + 1;
        //                                            min = Convert.ToInt32(tmpstr[1]);
        //                                            tonfrom = Convert.ToInt32(tmpstr[1]) - mintag + 1;
        //                                            tonto = maxcurr - mintag + 1;
        //                                        }
        //                                        //dr["GRADE"] = grdstr;
        //                                        dr["GRD"] = grdcurr;
        //                                        dr["MIN"] = fntchar + Convert.ToInt32(min).ToString(fmt);
        //                                        dr["MAX"] = fntchar + Convert.ToInt32(maxcurr).ToString(fmt);
        //                                        dr["SRT"] = _datatableSumGrd.Rows[v]["SRT"].ToString();
        //                                        dr["TonFrom"] = tonfrom;
        //                                        dr["TonTo"] = tonto;
        //                                        dt.Rows.Add(dr);
        //                                        tmpmin2 = "";
        //                                    }
        //                                    else if (Convert.ToInt32(dt.Rows[r]["MAX"].ToString()) != maxcurr && mincurr < Convert.ToInt32(dt.Rows[r]["MAX"].ToString()))
        //                                    {
        //                                        //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["TonTo"].ToString(), maxcurr - mintag + 1);
        //                                        //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["Max"].ToString(), maxcurr);
        //                                        dt.Rows[r]["MAX"] = maxcurr;
        //                                        dt.Rows[r]["TonTo"] = maxcurr - mintag + 1;
        //                                        tmpmin2 = "";
        //                                    }
        //                                    dr = dt.NewRow();
        //                                    //grdstr = notnggrd + " = " + fntchar + Convert.ToInt32(tmpmin).ToString(fmt) + " - " + fntchar + Convert.ToInt32(tmpmax).ToString(fmt) + " " + Convert.ToInt32(tmpmin) - Convert.ToInt32(mintag) + 1 + " " + Convert.ToInt32(tmpmax) - Convert.ToInt32(mintag) + 1;
        //                                    //dr["GRADE"] = grdstr;
        //                                    dr["GRD"] = notnggrd;
        //                                    dr["MIN"] = fntchar + Convert.ToInt32(tmpmin).ToString(fmt);
        //                                    dr["MAX"] = fntchar + Convert.ToInt32(tmpmax).ToString(fmt);
        //                                    dr["SRT"] = notngsrt;
        //                                    dr["TonFrom"] = tonfrom;
        //                                    dr["TonTo"] = tonto;
        //                                    dt.Rows.Add(dr);
        //                                }
        //                            }
        //                            else if (v == _datatableSumGrd.Rows.Count - 1 && maxcurr != maxtag && grdcurr == nggrd)
        //                            {
        //                                var r = dt.Rows.Count - 1;
        //                                if (Convert.ToInt32(dt.Rows[r]["MAX"].ToString()) != maxcurr && mincurr > Convert.ToInt32(dt.Rows[r]["MAX"].ToString()))
        //                                {
        //                                    string[] tmpstr;
        //                                    int min = 0;
        //                                    tmpstr = tmpmin2.Split(',');
        //                                    dr = dt.NewRow();
        //                                    if (tmpstr.Length == 1)
        //                                    {
        //                                        //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(mincurr).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + mincurr - mintag + 1 + " " + maxcurr - mintag + 1;
        //                                        min = mincurr;
        //                                        tonfrom = mincurr - mintag + 1;
        //                                        tonto = maxcurr - mintag + 1;
        //                                    }
        //                                    else
        //                                    {
        //                                        //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(tmpstr[1]).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + Convert.ToInt32(tmpstr[1]) - Convert.ToInt32(mintag) + 1 + " " + maxcurr - mintag + 1;
        //                                        min = Convert.ToInt32(tmpstr[1]);
        //                                        tonfrom = Convert.ToInt32(tmpstr[1]) - mintag + 1;
        //                                        tonto = maxcurr - mintag + 1;
        //                                        tmpmin2 = "";
        //                                    }
        //                                    //dr["GRADE"] = grdstr;
        //                                    dr["GRD"] = grdcurr;
        //                                    dr["MIN"] = fntchar + Convert.ToInt32(min).ToString(fmt);
        //                                    dr["MAX"] = fntchar + Convert.ToInt32(maxcurr).ToString(fmt);
        //                                    dr["SRT"] = _datatableSumGrd.Rows[v]["SRT"].ToString();
        //                                    dr["TonFrom"] = tonfrom;
        //                                    dr["TonTo"] = tonto;
        //                                    dt.Rows.Add(dr);
        //                                }
        //                                else if (Convert.ToInt32(dt.Rows[r]["MAX"].ToString()) != maxcurr && mincurr < Convert.ToInt32(dt.Rows[r]["MAX"].ToString()))
        //                                {
        //                                    //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["TonTo"].ToString(), maxcurr - mintag + 1);
        //                                    //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["Max"].ToString(), maxcurr);
        //                                    dt.Rows[r]["MAX"] = maxcurr;
        //                                    dt.Rows[r]["TonTo"] = maxcurr - mintag + 1;
        //                                    tmpmin2 = "";
        //                                }
        //                                dr = dt.NewRow();
        //                                //grdstr = notnggrd + " = " + fntchar + Convert.ToInt32(maxcurr + 1).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxtag).ToString(fmt) + " " + Convert.ToInt32(maxcurr + 1) - Convert.ToInt32(mintag) + 1 + " " + Convert.ToInt32(maxtag) - Convert.ToInt32(mintag) + 1;
        //                                //dr["GRADE"] = grdstr;
        //                                dr["GRD"] = _datatableSumGrd.Rows[v - 1]["GRADE"].ToString();
        //                                dr["MIN"] = fntchar + Convert.ToInt32(maxcurr + 1).ToString(fmt);
        //                                dr["MAX"] = fntchar + Convert.ToInt32(maxtag).ToString(fmt);
        //                                dr["SRT"] = notngsrt;
        //                                dr["TonFrom"] = Convert.ToInt32(maxcurr + 1) - Convert.ToInt32(mintag) + 1;
        //                                dr["TonTo"] = maxtag - mintag + 1;
        //                                dt.Rows.Add(dr);
        //                            }
        //                            else if (v == _datatableSumGrd.Rows.Count - 1 && grdcurr == notnggrd)
        //                            {
        //                                dr = dt.NewRow();
        //                                //grdstr = grdcurr + " = " + fntchar + Convert.ToInt32(tmpmin).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + tmpmin - mintag + 1 + " " + maxcurr - mintag + 1;
        //                                //dr["GRADE"] = grdstr;
        //                                dr["GRD"] = grdcurr;
        //                                dr["MIN"] = fntchar + Convert.ToInt32(tmpmin).ToString(fmt);
        //                                dr["MAX"] = fntchar + Convert.ToInt32(maxtag).ToString(fmt);
        //                                dr["SRT"] = _datatableSumGrd.Rows[v]["SRT"].ToString();
        //                                dr["TonFrom"] = tmpmin - mintag + 1;
        //                                dr["TonTo"] = maxtag - mintag + 1;
        //                                dt.Rows.Add(dr);
        //                                dr = dt.NewRow();
        //                            }
        //                            else if (v == _datatableSumGrd.Rows.Count - 1 && maxcurr == maxtag)
        //                            {
        //                                var r = dt.Rows.Count - 1;
        //                                if (dt.Rows[dt.Rows.Count - 1]["GRD"].ToString() == notnggrd)
        //                                {
        //                                    dr = dt.NewRow();
        //                                    //grdstr = _datatableSumGrd.Rows[v]["GRADE"].ToString() + " = " + fntchar + Convert.ToInt32(mincurr).ToString(fmt) + " - " + fntchar + Convert.ToInt32(maxcurr).ToString(fmt) + " " + mincurr - mintag + 1 + " " + maxcurr - mintag + 1;
        //                                    //dr["GRADE"] = grdstr;
        //                                    dr["GRD"] = _datatableSumGrd.Rows[v]["GRADE"].ToString();
        //                                    dr["MIN"] = fntchar + Convert.ToInt32(mincurr).ToString(fmt);
        //                                    dr["MAX"] = fntchar + Convert.ToInt32(maxcurr).ToString(fmt);
        //                                    dr["SRT"] = ngsrt;
        //                                    dr["TonFrom"] = mincurr - mintag + 1;
        //                                    dr["TonTo"] = maxcurr - mintag + 1;
        //                                    dt.Rows.Add(dr);
        //                                }
        //                                else
        //                                {
        //                                    //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["TonTo"].ToString(), ttmpmin - 1 - mintag + 1);
        //                                    //dt.Rows[r]["GRADE"] = dt.Rows[r]["GRADE"].ToString().Replace(dt.Rows[r]["Max"].ToString(), ttmpmin - 1);
        //                                    dt.Rows[r]["MAX"] = ttmpmin - 1;
        //                                    dt.Rows[r]["TonTo"] = ttmpmin - 1 - mintag + 1;
        //                                }
        //                            }
        //                        }
        //                    }
        //                }

        //            }
        //            var dv = dt.DefaultView;
        //            dv.Sort = "MIN";
        //            dt2 = dv.ToTable();

        //            return dt2;
        //        }
        //        catch (Exception ex)
        //        {
        //            string result = ex.Message;
        //            return null;
        //        }
        //    }

        //}
        //public async Task IDS_D_Maint(DailyIDSTransVM m, string idH, string rectype)
        //{
        //    for (int i = 0; i < m.MoldingList.Count; i++)
        //    {


        //        string loc = HttpContext.Connection.RemoteIpAddress?.ToString();
        //        var aclUser = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");

        //        m.CREATED_BY = aclUser.USER_ID;
        //        m.CREATED_LOC = loc;

        //        var mach = m.MoldingList[i].MachineName == null ? "" : m.MoldingList[i].MachineName;
        //        var machres = m.MoldingList[i].Average == null ? "" : m.MoldingList[i].Average;
        //        var reg = m.MoldingList[i].RegressionResult == null ? "" : m.MoldingList[i].RegressionResult;
        //        var mainreading = m.MoldingList[i].MainReading;
        //        var typeind = m.MoldingList[i].TypeInd;
        //        var regressind = m.MoldingList[i].RegressInd == null ? "" : m.MoldingList[i].RegressInd;
        //        var regressformula = m.MoldingList[i].RegressFormula == null ? "" : m.MoldingList[i].RegressFormula;
        //        var coqind = m.MoldingList[i].COQInd == null ? "" : m.MoldingList[i].COQInd;
        //        var reading1 = m.MoldingList[i].Reading1 == null ? "" : m.MoldingList[i].Reading1;
        //        var reading2 = m.MoldingList[i].Reading2 == null ? "" : m.MoldingList[i].Reading2;
        //        var reading3 = m.MoldingList[i].Reading3 == null ? "" : m.MoldingList[i].Reading3;
        //        var reading4 = m.MoldingList[i].Reading4 == null ? "" : m.MoldingList[i].Reading4;
        //        var reading5 = m.MoldingList[i].Reading5 == null ? "" : m.MoldingList[i].Reading5;
        //        var reading6 = m.MoldingList[i].Reading6 == null ? "" : m.MoldingList[i].Reading6;
        //        var regressionresult = m.MoldingList[i].RegressionResult == null ? "" : m.MoldingList[i].RegressionResult; //10/2/2023 LiewKarWei change 0.00 to ""
        //        var coqadj = m.MoldingList[i].COQAdj == null ? "" : m.MoldingList[i].COQAdj;
        //        var grade = m.MoldingList[i].Grade == null ? "" : m.MoldingList[i].Grade;
        //        var grade1 = m.MoldingList[i].Grade1 == null ? "" : m.MoldingList[i].Grade1;
        //        var grade2 = m.MoldingList[i].Grade2 == null ? "" : m.MoldingList[i].Grade2;
        //        var grade3 = m.MoldingList[i].Grade3 == null ? "" : m.MoldingList[i].Grade3;
        //        var grade4 = m.MoldingList[i].Grade4 == null ? "" : m.MoldingList[i].Grade4;
        //        var grade5 = m.MoldingList[i].Grade5 == null ? "" : m.MoldingList[i].Grade5;
        //        var grade6 = m.MoldingList[i].Grade6 == null ? "" : m.MoldingList[i].Grade6;


        //        if (mainreading == "MAIN READING" && typeind == "E")
        //        {
        //            if (i == 0)
        //            {
        //                var err = await dbdal.IDS_D_MAINT(0, idH, m.MoldingList[i].Properties, m.MoldingList[i].PropItem, m.MoldingList[i].Unit, typeind, m.MoldingList[i].DataChecking, mach,
        //                regressind, regressformula, coqind, "", "", reading1, "", "", "", "",
        //                "", reading1, regressionresult, coqadj, "", "", "", grade1, rectype, m.CREATED_BY, m.CREATED_LOC);
        //            }
        //            else if (i == 1)
        //            {
        //                if (reading2 != "")
        //                {
        //                    await dbdal.IDS_SL_MAINT("0", idH, "", reading2, "", "", "", "", reading2, grade2, "1", m.CREATED_BY, m.CREATED_LOC);
        //                }

        //            }
        //            else if (i == 2)
        //            {
        //                if (reading3 != "")
        //                {
        //                    await dbdal.IDS_SL_MAINT("0", idH, "", "", reading3, "", "", "", reading3, grade3, "1", m.CREATED_BY, m.CREATED_LOC);
        //                }

        //            }
        //            else if (i == 3)
        //            {
        //                if (reading4 != "")
        //                {
        //                    await dbdal.IDS_SL_MAINT("0", idH, "", "", "", reading4, "", "", reading4, grade4, "1", m.CREATED_BY, m.CREATED_LOC);
        //                }

        //            }
        //            else if (i == 4)
        //            {
        //                if (reading5 != "")
        //                {
        //                    await dbdal.IDS_SL_MAINT("0", idH, "", "", "", "", reading5, "", reading5, grade5, "1", m.CREATED_BY, m.CREATED_LOC);
        //                }

        //            }
        //            else if (i == 5)
        //            {
        //                if (reading6 != "")
        //                {
        //                    await dbdal.IDS_SL_MAINT("0", idH, "", "", "", "", "", reading6, reading6, grade6, "1", m.CREATED_BY, m.CREATED_LOC);
        //                }

        //            }
        //        }
        //        else if (m.MoldingList[i].TypeInd == "P") // pending => check grade either pass A fail NG (4/1/2022)
        //        {
        //            var err2 = await dbdal.IDS_D_MAINT(0, idH, m.MoldingList[i].Properties, m.MoldingList[i].PropItem, m.MoldingList[i].Unit, typeind, m.MoldingList[i].DataChecking, mach,
        //               regressind, regressformula, coqind, "", "", "", "", "", "", "",
        //               "", machres, regressionresult, coqadj, "", "", "", "", rectype, m.CREATED_BY, m.CREATED_LOC);
        //        }
        //        else if (m.MoldingList[i].TypeInd == "M")
        //        {
        //            var err = await dbdal.IDS_D_MAINT(0, idH, m.MoldingList[i].Properties, m.MoldingList[i].PropItem, m.MoldingList[i].Unit, typeind, m.MoldingList[i].DataChecking, mach,
        //               regressind, regressformula, coqind, "", "", machres, "", "", "", "",
        //               "", machres, regressionresult, coqadj, "", "", "", grade, rectype, m.CREATED_BY, m.CREATED_LOC);

        //            //var err2 = db2.IDS_SL_MAINT(err, machres, "", "", "", "", "", machres, grade, rectype, m.CREATED_BY, m.CREATED_LOC); //temp remarks by KL Ong as double entry in IDS_SL
        //        }
        //        else
        //        {
        //            DataTable datachkdt = new DataTable();
        //            int ttlcnt = 0;
        //            int ttlcntnotng = 0;
        //            double ttlval = 0.0;
        //            double ttlvalng = 0.0;
        //            var grd = "";

        //            if (grade1 != "NG" && grade1 != "")
        //            {
        //                ttlcntnotng += 1;
        //                ttlval += Convert.ToDouble(reading1);
        //            }
        //            else if (grade1 == "NG")
        //            {
        //                ttlvalng += Convert.ToDouble(reading1 == "" ? "0" : reading1);
        //            }

        //            if (grade2 != "NG" && grade2 != "")
        //            {
        //                ttlcntnotng += 1;
        //                ttlval += Convert.ToDouble(reading2);
        //            }
        //            else if (grade2 == "NG")
        //            {
        //                ttlvalng += Convert.ToDouble(reading2 == "" ? "0" : reading2);
        //            }

        //            if (grade3 != "NG" && grade3 != "")
        //            {
        //                ttlcntnotng += 1;
        //                ttlval += Convert.ToDouble(reading3);
        //            }
        //            else if (grade3 == "NG")
        //            {
        //                ttlvalng += Convert.ToDouble(reading3 == "" ? "0" : reading3);
        //            }

        //            if (grade4 != "NG" && grade4 != "")
        //            {
        //                ttlcntnotng += 1;
        //                ttlval += Convert.ToDouble(reading4);
        //            }
        //            else if (grade4 == "NG")
        //            {
        //                ttlvalng += Convert.ToDouble(reading4 == "" ? "0" : reading4);
        //            }

        //            if (grade5 != "NG" && grade5 != "")
        //            {
        //                ttlcntnotng += 1;
        //                ttlval += Convert.ToDouble(reading5);
        //            }
        //            else if (grade5 == "NG")
        //            {
        //                ttlvalng += Convert.ToDouble(reading5 == "" ? "0" : reading5);
        //            }

        //            if (grade6 != "NG" && grade6 != "")
        //            {
        //                ttlcntnotng += 1;
        //                ttlval += Convert.ToDouble(reading6);
        //            }
        //            else if (grade6 == "NG")
        //            {
        //                ttlvalng += Convert.ToDouble(reading6 == "" ? "0" : reading6);
        //            }

        //            if (grade1 != "") { ttlcnt += 1; }
        //            if (grade2 != "") { ttlcnt += 1; }
        //            if (grade3 != "") { ttlcnt += 1; }
        //            if (grade4 != "") { ttlcnt += 1; }
        //            if (grade5 != "") { ttlcnt += 1; }
        //            if (grade6 != "") { ttlcnt += 1; }

        //            if (ttlcnt != 0 && ttlval != 0 && ttlcnt != 1)
        //            {
        //                DataTable dtchk = new DataTable();


        //                dtchk = await GetDataChk(dtchk, m.MoldingList[i].Properties, m.MoldingList[i].PropItem, m.LOTNO, m.PRODTYPE);

        //                if (dtchk != null && dtchk.Rows.Count > 0)
        //                {
        //                    var passperc = Convert.ToDecimal(dtchk.Rows[0]["PASSPREC"].ToString());
        //                    decimal ttlcntnotng2 = Convert.ToDecimal(ttlcntnotng);
        //                    if (Math.Round(((ttlcntnotng2 / ttlcnt) * 100), 2) >= passperc)
        //                    {
        //                        machres = (Math.Round((Convert.ToDecimal(ttlval) / ttlcntnotng2), 2)).ToString();
        //                    }
        //                    else
        //                    {
        //                        if (ttlcntnotng == (ttlcnt - ttlcntnotng))
        //                        {
        //                            machres = (Math.Round(((ttlvalng + ttlval) / (ttlcnt)), 2)).ToString();
        //                        }
        //                        else
        //                        {
        //                            machres = (Math.Round((ttlvalng / (ttlcnt - ttlcntnotng)), 2)).ToString();
        //                            grd = "NGNG";
        //                        }
        //                    }

        //                    var temp = await dbdal.DataChk(m.PRODTYPE, machres, "1", m.MoldingList[i].PropItem);

        //                    if (temp != null && temp.Rows.Count > 0)
        //                    {
        //                        for (int ii = 0; ii < temp.Rows.Count; ii++)
        //                        {
        //                            if (temp.Rows[ii]["GRADE"].ToString() != "NGNG")
        //                            {
        //                                grd = temp.Rows[ii]["GRADE"].ToString();
        //                            }
        //                        }

        //                        if (grd == "")
        //                        {
        //                            grd = "NGNG";
        //                        }


        //                    }
        //                    int len = grd.Length - 2;
        //                    if (grd.Substring(0, 1) == "OK")
        //                    {
        //                        grd = grd.Substring(2, len);
        //                    }
        //                    else
        //                    {
        //                        grd = grd.Substring(2, len);
        //                    }
        //                }

        //            }
        //            else
        //            {
        //                grd = grade;
        //            }

        //            var err2 = await dbdal.IDS_D_MAINT(0, idH, m.MoldingList[i].Properties, m.MoldingList[i].PropItem, m.MoldingList[i].Unit, typeind, m.MoldingList[i].DataChecking, mach,
        //                regressind, regressformula, coqind, "", "", reading1, reading2, reading3, reading4, reading5,
        //                reading6, machres, regressionresult, coqadj, "", "", "", grd, rectype, m.CREATED_BY, m.CREATED_LOC);
        //        }
        //    }
        //}

        //public async Task<DataTable>  GetDataChk(DataTable dt, string properties, string propitem, string lotno, string prodtype)
        //{
        //    var prodline = lotno.Substring(1, 2);
        //    dt = await dbdal.HowManyDataChk(properties, propitem, prodtype, prodline);
        //    return dt;
        //}

        ////for hiding + button
        //public async Task<ActionResult> GetDataChk2(string properties, string propitem, string lotno, string prodtype, string id_ids_h)
        //{
        //    var prodline = lotno.Substring(1, 2);
        //    DataTable dt = new DataTable();
        //    dt = await dbdal.HowManyDataChk(properties, propitem, prodtype, prodline);
        //    if (dt != null && dt.Rows.Count > 0)
        //    {
        //        foreach (DataRow row in dt.Rows)
        //        {
        //            if (row["ID_IDS_H"].ToString() != id_ids_h)
        //            {
        //                row.Delete();
        //            }
        //        }

        //        dt.AcceptChanges();
        //    }


        //    string JSONString = string.Empty;
        //    JSONString = JsonConvert.SerializeObject(dt);
        //    return Json(JSONString);
        //}

        //public async Task saveapptab(List<List<List<tagnomodel>>> tagList, List<descmodel> description, string idH, string NG, string rectype, DailyIDSTransVM m, List<List<fieldnamemodel>> field)
        //{
        //    var grade = "";
        //    var tempDL = "";
        //    string result = "";
        //    string loc = HttpContext.Connection.RemoteIpAddress?.ToString();
        //    var aclUser = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
        //    m.CREATED_BY = aclUser.USER_ID;
        //    m.CREATED_LOC = loc;
        //    var tempD = "";
        //    try
        //    {
        //        for (int a = 0; a < tagList.Count; a++)
        //        {
        //            tempD = await dbdal.IDS_D_MAINT(0, idH, description[a].desc, "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", rectype, m.CREATED_BY, m.CREATED_LOC);
        //            for (int c = 0; c < tagList[a].Count - 1; c++) // -1 because the the average button	
        //            {
        //                for (int d = 0; d < tagList[a][c].Count; d++) //3	
        //                {
        //                    if (d == 0)
        //                    {
        //                        if (tagList[a][c][d].tagno != null || tagList[a][c][d].tagno != "Average")
        //                        {
        //                            var tagno = tagList[a][c][d].tagno.ToString(); //TAGNO	
        //                            var tag = tagno.Split('-');
        //                            var tagfrom = tag[0].ToString().Trim();
        //                            var tagto = tag[1].ToString().Trim();
        //                            var taggrade = tagList[a][c][d].grd == null ? "" : tagList[a][c][d].grd.ToString();
        //                            tempDL = await dbdal.IDS_DL_MAINT("0", tempD, tagfrom, tagto, taggrade, rectype, m.CREATED_BY, m.CREATED_LOC);
        //                            int x = 2;
        //                            for (int e = 0; e < field[a].Count - 2; e++)
        //                            {
        //                                if (field[a][e + 2].fieldname.ToString() != null)
        //                                {
        //                                    var txt = field[a][e + 2].fieldname.ToString();
        //                                    var rat = "1.00";
        //                                    var col = tagList[a][c][d + x].avg == null ? "" : tagList[a][c][d + x].avg.ToString();
        //                                    //var col = tagList[a][c][d + x].avg ;
        //                                    var tempDLL = await dbdal.IDS_DLL_MAINT("0", tempDL, txt, col, m.PRODTYPE, description[a].desc, rat, rectype, m.CREATED_BY, m.CREATED_LOC);
        //                                }
        //                                x = x + 1;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            //do nothing	
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result = ex.Message;
        //    }
        //}

        //[HttpPost]
        //[SessionExpire]
        //public async Task<ActionResult> SearchProdType(DailyIDSTransVM model)
        //{
        //    try
        //    {
        //        DailyIDSTransVM data = await dbdal.dataByProdtype(model.PRODTYPE, model.LOTNO);
        //        string loc = HttpContext.Connection.RemoteIpAddress?.ToString();
        //        var aclUser = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");
        //        data.UPDATEDBY = aclUser.USER_ID;
        //        data.TESTEDBY = aclUser.USER_ID;
        //        data.FullNG = "50";
        //        data.PACKEDDATE = DateTime.Today.ToString();
        //        ViewBag.prodtype = await dbdal.getProdTypeList();
        //        data.MoldingList = await dbdal.GetDataMolding(model.PRODTYPE, data.PACKEDDATE, model.LOTNO);
        //        int count = data.MoldingList.Count;

        //        foreach (var item in data.MoldingList)
        //        {
        //            await GetGrade(count, item);
        //        }

        //        //data.MoldingList = db2.GetData(model.PRODTYPE, "1", model.LOTNO, model.PACKEDDATE2, "");	
        //        var app = await apptab(model.PRODTYPE, model.LOTNO, model.PACKEDDATE2, model);
        //        data.descriptionList = app.descriptionList;
        //        data.ratioList = app.ratioList;
        //        TempData["prodtype"] = model.PRODTYPE;
        //        HttpContext.Session.SetString("lotno", model.LOTNO?.ToString() ?? "");
        //        HttpContext.Session.SetString("pack", model.PACKEDDATE2.ToString() ?? "");

        //        var _datatable = await dbdal.GetData1(model.PRODTYPE, "1", model.LOTNO, model.PACKEDDATE2);
        //        var prodgroup = "";

        //        if (_datatable != null && _datatable.Rows.Count > 0)
        //        {
        //            prodgroup = _datatable.Rows[0]["PRODGROUP"].ToString();
        //        }

        //        var _datatable2 = await dbdal.DataChk56(model.PACKEDDATE2, prodgroup, model.PRODTYPE, data.prodline);

        //        if (_datatable2 != null && _datatable2.Rows.Count > 0)
        //        {
        //            HttpContext.Session.SetString("_datatable2", _datatable2.ToString() ?? "");

        //            for (int x = 0; x < _datatable2.Rows.Count; x++)
        //            {
        //                for (int k = 0; k < data.MoldingList.Count; k++)
        //                {
        //                    if (data.MoldingList[k].PropItem == _datatable2.Rows[x]["PROPITEM"].ToString())
        //                    {
        //                        data.MoldingList[k].mandatory = "1";
        //                    }
        //                }
        //            }
        //        }

        //        data.prodgroup = prodgroup;

        //        //ViewBag.prodgroup = prodgroup;
        //        //ViewBag.yiformula = GetYIFormula(model.LOTNO);
        //        DEIDSTCM = new CommonModel<DailyIDSTransVM, DailyIDSTransVM>(this, "Daily Property Inspection Result");
        //        DEIDSTCM.ModelObj = data;
        //        return View("DE_IDS_TRANS_REV_ADD", DEIDSTCM);
        //    }
        //    catch (Exception EX)
        //    {
        //        ViewBag.Error = EX.Message;
        //        return View();
        //    }
        //}


        //public async Task<MoldingModel> GetGrade(int countMold, MoldingModel model)
        //{

        //    var prodtype = model.prodtype;
        //    var propItem = model.PropItem;
        //    var typeind = model.TypeInd;

        //    await Reading1to6Grade(typeind, prodtype, propItem, model);

        //    var idmachres = model.Average;
        //    var idreg = model.RegressionResult;
        //    var result = "";

        //    if (idmachres != "0" && idmachres != null)
        //    {
        //        result = idmachres.ToString();

        //        if (idreg != "0.00" && idreg != "" && idreg != null)
        //        {
        //            result = idreg;
        //        }

        //        var pind = typeind == "P" ? 0 : 1;
        //        var temp = await dbdal.DataChk(prodtype, result, pind.ToString(), propItem);

        //        if (temp != null && temp.Rows.Count >= 0)
        //        {
        //            var grade = "";

        //            for (int ii = 0; ii <= (temp.Rows.Count - 1); ii++)
        //            {
        //                if (temp.Rows[ii]["GRADE"].ToString() != "NGNG")
        //                {
        //                    grade = temp.Rows[ii]["GRADE"].ToString();
        //                }
        //            }

        //            if (grade == "")
        //            {
        //                grade = "NGNG";
        //            }
        //            int len = grade.Length - 2;
        //            if (grade.Substring(0, 1) == "ok")
        //            {
        //                grade = grade.Substring(2, len);
        //            }
        //            else
        //            {
        //                grade = grade.Substring(2, len);
        //            }
        //            model.Grade = grade;
        //        }
        //    }
        //    return model;
        //}

        //public async Task<MoldingModel> Reading1to6Grade(string typeind, string prodtype, string propitem, MoldingModel model)
        //{
        //    string result = "";

        //    for (int iii = 1; iii <= 6; iii++)
        //    {
        //        string txtR = "";

        //        if (iii == 1)
        //        {
        //            txtR = model.Reading1;
        //        }
        //        else if (iii == 2)
        //        {

        //            txtR = model.Reading2;
        //        }
        //        else if (iii == 3)
        //        {
        //            txtR = model.Reading3;
        //        }
        //        else if (iii == 4)
        //        {
        //            txtR = model.Reading4;
        //        }
        //        else if (iii == 5)
        //        {
        //            txtR = model.Reading5;
        //        }
        //        else if (iii == 6)
        //        {
        //            txtR = model.Reading6;// LEHA CHANGE FROM READING2 TO READING6
        //        }

        //        if (txtR != "0.00" && txtR != "")
        //        {
        //            result = txtR;
        //        }

        //        var pind = typeind == "P" ? 0 : 1;

        //        if (txtR != null)
        //        {
        //            var temp = await dbdal.DataChk(prodtype, result, pind.ToString(), propitem);

        //            if (temp != null && temp.Rows.Count > 0)
        //            {
        //                var grade = "";

        //                for (int ii = 0; ii < (temp.Rows.Count); ii++)
        //                {
        //                    if (temp.Rows[ii]["GRADE"].ToString() != "NGNG")
        //                    {
        //                        grade = temp.Rows[ii]["GRADE"].ToString();
        //                    }
        //                }

        //                if (grade == "")
        //                {
        //                    grade = "NGNG";
        //                }

        //                int len = grade.Length - 2;
        //                if (grade.Substring(0, 1) == "OK")
        //                {
        //                    grade = grade.Substring(2, len);
        //                }
        //                else
        //                {
        //                    grade = grade.Substring(2, len);
        //                }

        //                if (iii == 1)
        //                {

        //                    model.Grade1 = grade;
        //                }
        //                else if (iii == 2)
        //                {
        //                    model.Grade2 = grade;
        //                }
        //                else if (iii == 3)
        //                {
        //                    model.Grade3 = grade;
        //                }
        //                else if (iii == 4)
        //                {
        //                    model.Grade4 = grade;
        //                }
        //                else if (iii == 5)
        //                {
        //                    model.Grade5 = grade;
        //                }
        //                else if (iii == 6)
        //                {
        //                    model.Grade6 = grade;
        //                }
        //            }
        //        }
        //    }
        //    return model;
        //}

        //public async Task<DailyIDSTransVM>apptab(string prod, string lot, string packed, DailyIDSTransVM m)
        //{
        //    try
        //    {
        //        List<descriptionmodel> s = new List<descriptionmodel>();
        //        List<tagnomodel> t = new List<tagnomodel>();
        //        List<fieldnamemodel> f = new List<fieldnamemodel>();
        //        List<descmodel> d = new List<descmodel>();

        //        descriptionmodel mdl = new descriptionmodel();

        //        List<List<fieldnamemodel>> fn = new List<List<fieldnamemodel>>();
        //        List<List<tagnomodel>> tg = new List<List<tagnomodel>>();

        //        List<fieldnamemodel> fieldratio = new List<fieldnamemodel>();
        //        List<List<ratioModel>> Finalratio = new List<List<ratioModel>>();


        //        if (lot != "")
        //        {
        //            DataTable datatable3 = new DataTable();

        //            datatable3 = await dbdal.GetDataDLLALL("E_AppBorder", "", prod, "", "", "", "");

        //            if (datatable3 != null && datatable3.Rows.Count > 0)
        //            {
        //                int data3Count = datatable3.Rows.Count;
        //                HttpContext.Session.SetString("hid", data3Count?.ToString() ?? "");


        //                for (int i = 0; i < data3Count; i++)
        //                {
        //                    List<ratioModel> ratio = new List<ratioModel>();

        //                    f = await dbdal.getfielname("E_AppColumn", datatable3.Rows[i]["description"].ToString(), prod, "", "", "", "");
        //                    t = await dbdal.gettagno("NewEntry", "E_AppRow", lot, datatable3.Rows[i]["description"].ToString(), prod, packed, "");

        //                    ViewBag.cnt = t.Count;
        //                    var desc = datatable3.Rows[i]["description"].ToString();

        //                    foreach (var item in f)
        //                    {
        //                        ratio.Add(new ratioModel { name = item.fieldname, ratio = Convert.ToDecimal(item.calcratio) });
        //                    }
        //                    Finalratio.Add(ratio);

        //                    d.Add(new descmodel { desc = desc });
        //                    // mdl.description = desc;

        //                    if (t.Count > 0)
        //                    {

        //                        int word0 = 0;
        //                        int word1 = 0;
        //                        int word2 = 0;

        //                        string wordChar = t[0].res3.ToString();
        //                        var fmt = "";

        //                        for (int fmtii = 1; fmtii <= Convert.ToInt32(t[0].res5); fmtii++)
        //                        {
        //                            fmt = fmt + "0";
        //                        }

        //                        word0 = Convert.ToInt32(t[0].res4.ToString());
        //                        word1 = Convert.ToInt32(t[0].res4.ToString());
        //                        word2 = Convert.ToInt32(t[0].res2.ToString());

        //                        int word3 = 0;
        //                        int Cv = 0;
        //                        string item = "";

        //                        for (int v = 0; v < t.Count; v++)
        //                        {
        //                            int setdata = Convert.ToInt32(t[v].res7.ToString());

        //                            word3 = word0 + setdata - 1;

        //                            for (int iv = 0; iv < f.Count; iv++)
        //                            {
        //                                if (iv == 0)
        //                                {
        //                                    if (word3 > word2)
        //                                    {
        //                                        if (wordChar == "")
        //                                        {
        //                                            item = wordChar + " " + word1.ToString(fmt) + "-" + wordChar + " " + word2.ToString(fmt);
        //                                            t[v].tagno = item;  //satu
        //                                            t[v].res7 = setdata.ToString();  //satu
        //                                        }
        //                                        else
        //                                        {
        //                                            item = wordChar + " " + word1.ToString(fmt) + "-" + wordChar + " " + word2.ToString(fmt);
        //                                            t[v].tagno = item;
        //                                            t[v].res7 = setdata.ToString();//dua
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        if (wordChar == "")
        //                                        {
        //                                            item = wordChar + " " + word1.ToString(fmt) + "-" + wordChar + " " + word3.ToString(fmt);
        //                                            t[v].tagno = item;
        //                                            t[v].res7 = setdata.ToString();//tiga
        //                                        }
        //                                        else
        //                                        {
        //                                            item = wordChar + " " + word1.ToString(fmt) + "-" + wordChar + " " + word3.ToString(fmt);
        //                                            t[v].tagno = item;
        //                                            t[v].res7 = setdata.ToString();//empat
        //                                        }
        //                                    }
        //                                    word1 = word3 + 1;
        //                                }
        //                            }
        //                            Cv = v + 1;
        //                            if (word3 >= word2)
        //                            {
        //                                v = t.Count();
        //                            }

        //                        }

        //                        if (word2 > word3)
        //                        {
        //                            word3 = word1 + 1;

        //                            for (int iv = 0; iv < f.Count; iv++)
        //                            {
        //                                int setdata = Convert.ToInt32(t[iv].res7.ToString());
        //                                if (iv == 0)
        //                                {
        //                                    if (wordChar == "")
        //                                    {
        //                                        item = wordChar + " " + word1.ToString(fmt) + "-" + wordChar + " " + word2.ToString(fmt);
        //                                        t[iv].tagno = item;
        //                                        t[iv].res7 = setdata.ToString();// lima
        //                                    }
        //                                    else
        //                                    {
        //                                        item = wordChar + " " + word1.ToString(fmt) + "-" + wordChar + " " + word2.ToString(fmt);
        //                                        t[iv].tagno = item;
        //                                        t[iv].res7 = setdata.ToString();//enam
        //                                    }

        //                                    word1 = word3 + 1;
        //                                }
        //                            }
        //                        }
        //                    }

        //                    string temp = t[0].res3.ToString();
        //                    for (int ii = 0; ii < t.Count(); ii++)
        //                    {
        //                        if (temp == "")
        //                        {
        //                            if (t[ii].tagno != null)
        //                            {
        //                                string[] a = null;
        //                                a = t[ii].tagno.Split('-');
        //                                t[ii].res7 = (Convert.ToInt32(a[1].Trim()) - Convert.ToInt32(t[ii].res4) + 1).ToString();
        //                            }
        //                        }
        //                        else
        //                        {
        //                            if (t[ii].tagno != null)
        //                            {
        //                                string[] a = null;
        //                                a = t[ii].tagno.Split('-');
        //                                string b = a[1].Trim().Substring(1);
        //                                t[ii].res7 = (Convert.ToInt32(b) - Convert.ToInt32(t[ii].res4) + 1).ToString();
        //                            }
        //                        }
        //                    }

        //                    fn.Add(f);
        //                    tg.Add(t);
        //                    mdl.fieldnameList = fn;
        //                    mdl.tagnoList = tg;
        //                    mdl.descList = d;
        //                    s.Add(mdl);
        //                    m.descriptionList = s;
        //                    m.ratioList = Finalratio;
        //                }
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var msg = ex.Message;
        //    }
        //    return m;
        //}

        //public async Task<string> GetYIFormula(string lotno)
        //{
        //    var dt = await dbdal.getYiFormula(lotno);
        //    var formula = "";
        //    if (dt != null && dt.Rows.Count > 0)
        //    {
        //        formula = dt.Rows[0]["REGRESSFORMULA"].ToString().Trim();
        //    }

        //    //formula = "0.837* var1 +5.821"; //for test purpose
        //    return formula == null || formula == "" ? "" : formula;
        //}

        //public async Task<JsonResult> UpdateMandatory(DailyIDSTransVM model)
        //{
        //    var _datatable2 = new DataTable();
        //    if (model.chkType == "Full")
        //    {
        //        _datatable2 = await dbdal.DataChk56(model.PACKEDDATE2 + "?FULL", model.prodgroup, model.PRODTYPE, model.prodline);
        //    }
        //    else if (model.chkType == "Part")
        //    {
        //        _datatable2 = await dbdal.DataChk56(model.PACKEDDATE2 + "?PART", model.prodgroup, model.PRODTYPE, model.prodline);
        //    }
        //    else if (model.chkType == "Uncheck")
        //    {
        //        _datatable2 = await dbdal.DataChk56(model.PACKEDDATE2, model.prodgroup, model.PRODTYPE, model.prodline);
        //    }

        //    if (_datatable2 != null)
        //    {
        //        if (_datatable2.Rows.Count > 0)
        //        {
        //            for (int x = 0; x < _datatable2.Rows.Count; x++)
        //            {
        //                for (int k = 0; k < model.MoldingList.Count; k++)
        //                {
        //                    var p = _datatable2.Rows[x]["PROPITEM"].ToString().Trim();
        //                    if (model.MoldingList[k].PropItem == p)
        //                    {
        //                        model.MoldingList[k].mandatory = "1";
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return Json(model);
        //}

        //public async Task<JsonResult> GetMachineName(string propitem, string lotno, string prodgroup)
        //{
        //    //propitem = "Melt Flow Rate (MFR)"; //for test purpose
        //    //lotno = "301-11-507"; //for test purpose
        //    //prodgroup = "ASA"; //for test purpose		
        //    var vm = await dbdal.getMachineName(lotno, propitem, prodgroup);
        //    return Json(vm);

        //}

        //public async Task<JsonResult> GetFormula(string MachineName, string PropItem, string ProdGroup)
        //{
        //    //MachineName = "TM"; //for test purpose
        //    //PropItem = "Modulus (TM)"; //for test purpose
        //    //ProdGroup = "General Purposes(GP)"; //for test purpose
        //    var dt = await dbdal.GetReg(MachineName, PropItem, ProdGroup);
        //    var formula = "";

        //    if (dt != null && dt.Rows.Count > 0)
        //    {
        //        formula = dt.Rows[0]["REGRESSFORMULA"].ToString();
        //    }

        //    return Json(formula);
        //}

        //public async Task<ActionResult> GetMachineData(string lotNo)
        //{
        //    DataTable machineDataTable = new DataTable();
        //    machineDataTable = await dbdal.GetMachineData(lotNo);

        //    List<MachineDataModel> machineDataList = new List<MachineDataModel>();
        //    List<List<MachineDataModel>> machineDataList2 = new List<List<MachineDataModel>>();

        //    string cdate = "";
        //    int i = 1;

        //    if (machineDataTable != null && machineDataTable.Rows.Count > 0)
        //    {
        //        foreach (DataRow row in machineDataTable.Rows)
        //        {
        //            i++;
        //            MachineDataModel m = new MachineDataModel();
        //            m.lotNo = row["LOTNO"].ToString();
        //            m.machineData = row["READING1"].ToString();
        //            m.tonnage = row["READING20"].ToString();
        //            m.properties = row["PROPERTIES"].ToString();
        //            m.createddate = row["CREATED_DATE"].ToString();
        //            if (m.properties != "YI") continue;
        //            if (cdate != "" && cdate != m.createddate)
        //            {
        //                machineDataList2.Add(machineDataList.ToList());
        //                machineDataList.Clear();
        //            }

        //            cdate = row["CREATED_DATE"].ToString();
        //            if (m.properties == "YI")
        //            {
        //                machineDataList.Add(m);
        //            }
        //            if (i == machineDataTable.Rows.Count) machineDataList2.Add(machineDataList);
        //        }
        //    }



        //    return Json(machineDataList2);
        //}

        //public async Task<JsonResult> oldCalculateResult(List<MachineNameModel> machinename, DailyIDSTransVM mod)
        //{
        //    var vm = "";
        //    var result = "";
        //    // DataTable dt = new DataTable();
        //    DataTable cal = new DataTable();
        //    List<MachineNameModel> m = new List<MachineNameModel>();
        //    foreach (var item in machinename)
        //    {
        //        result = item.Result;
        //        var dt = await dbdal.GetReg(item.MachineName, item.PropItem, item.ProdGroup);
        //        var reg = "0.00";
        //        var formula = "";
        //        var ind = "";
        //        var regressresult = "";

        //        if (dt != null && dt.Rows.Count > 0)
        //        {
        //            formula = dt.Rows[0]["REGRESSFORMULA"].ToString();
        //            formula = formula.Replace(" ", "");
        //            //ind = dt.Rows[0]["REGRESSIND"].ToString(); // KL Ong 20220425 not return from SP
        //            var rep = formula.Replace("var1", item.Result);
        //            var com = cal.Compute(rep, ""); // KL Ong 20220425 reg to rep
        //            reg = Math.Round(Convert.ToDouble(com), 2).ToString();
        //            //reg = Math.Round(cal.Compute(dt.Rows[0]["REGRESSFORMULA"].toString().Replace("var1", item.Result), ""), 2);
        //        }

        //        if (reg != "0.00")
        //        {
        //            regressresult = reg;
        //        }
        //        var grd = await dbdal.DataChk(mod.PRODTYPE, regressresult != "" ? regressresult : result, "1", item.PropItem);
        //        var grd2 = grd.Rows[0]["GRADE"].ToString().Substring(2);
        //        m.Add(new MachineNameModel { MachineName = item.MachineName, Result = result, PropItem = item.PropItem, RegressFormula = formula, RegressInd = ind, RegressionResult = regressresult, Grade = grd2 });

        //    }

        //    return Json(m);
        //}

        //public async Task<JsonResult> CalculateResult(List<MachineNameModel> machinename, DailyIDSTransVM mod)
        //{
        //    var vm = "";
        //    var result = "";
        //    // DataTable dt = new DataTable();
        //    DataTable cal = new DataTable();
        //    List<MachineNameModel> m = new List<MachineNameModel>();
        //    foreach (var item in machinename)
        //    {
        //        result = item.Result;
        //        // ng average - phase 3  nisa

        //        DataTable dthistory = await dbdal.getIDSDTransHistData(mod.IDS_D_ID.ToString(), mod.PRODTYPE, mod.LOTNO);

        //        int row = 1;
        //        decimal totalavg = Convert.ToDecimal(item.Result);
        //        decimal avgavg = 0;
        //        if (dthistory != null && dthistory.Rows.Count > 0)
        //        {
        //            for (int i = 0; i < dthistory.Rows.Count; i++)
        //            {
        //                row++;
        //                totalavg = Convert.ToDecimal(dthistory.Rows[i]["AVGRESULT"]) + totalavg;
        //            }

        //            avgavg = Math.Round((totalavg / row), 2);
        //            result = avgavg.ToString();
        //        }


        //        var dt = await dbdal.GetReg(item.MachineName, item.PropItem, item.ProdGroup);
        //        var reg = "0.00";
        //        var formula = "";
        //        var ind = "";
        //        var regressresult = "";

        //        if (dt != null && dt.Rows.Count > 0)
        //        {
        //            formula = dt.Rows[0]["REGRESSFORMULA"].ToString();
        //            formula = formula.Replace(" ", "");
        //            //ind = dt.Rows[0]["REGRESSIND"].ToString(); // KL Ong 20220425 not return from SP
        //            var rep = formula.Replace("var1", item.Result);
        //            var com = cal.Compute(rep, ""); // KL Ong 20220425 reg to rep
        //            reg = Math.Round(Convert.ToDouble(com), 2).ToString();
        //            //reg = Math.Round(cal.Compute(dt.Rows[0]["REGRESSFORMULA"].toString().Replace("var1", item.Result), ""), 2);
        //        }

        //        if (reg != "0.00")
        //        {
        //            regressresult = reg;
        //        }
        //        var grd = await dbdal.DataChk(mod.PRODTYPE, regressresult != "" ? regressresult : result, "1", item.PropItem);
        //        var grd2 = grd.Rows[0]["GRADE"].ToString().Substring(2);
        //        m.Add(new MachineNameModel { MachineName = item.MachineName, Result = result, PropItem = item.PropItem, RegressFormula = formula, RegressInd = ind, RegressionResult = regressresult, Grade = grd2 });

        //    }

        //    return Json(m);
        //}

        //public async Task<ActionResult> MoldingTab(MoldingModel model)  // check grading for each reading
        //{
        //    DailyIDSTransVM data = new DailyIDSTransVM();

        //    var lotno = HttpContext.Session.GetString("lotno").ToString();
        //    //var pack = HttpContext.Session.GetString("pack").ToString();
        //    //var moldRead = await dbdal.getMoldDetail(lotno, model.PropItem, model.prodtype);
        //var moldRead = await dbdal.getPropItemDet(lotno, model.PropItem, model.prodtype); //slow 22082024

        //    var moldinglist = await dbdal.GetData(model.prodtype, "1", lotno);

        //    int countMold = moldinglist.Count();
        //    model.Average = model.averageMold.ToString();
        //    await GetGrade(countMold, model);

        //    var txtMr = moldRead.mainreading == null ? "" : moldRead.mainreading;
        //    var txtCnt = moldRead.maxcount;
        //    var txtR1 = model.Reading1;
        //    var txtR2 = model.Reading2;
        //    var txtR3 = model.Reading3;
        //    var txtR4 = model.Reading4;
        //    var txtR5 = model.Reading5;
        //    var txtR6 = model.Reading6;

        //    int curcnt = 0;
        //    var mainavg = model.averageMold;


        //    if (txtR1 != null)
        //    {
        //        curcnt += 1;
        //        mainavg = Convert.ToDecimal(txtR1);
        //    }
        //    if (txtR2 != null)
        //    {
        //        curcnt += 1;
        //        mainavg = Convert.ToDecimal(txtR2);
        //    }
        //    if (txtR3 != null)
        //    {
        //        curcnt += 1;
        //        mainavg = Convert.ToDecimal(txtR3);
        //    }
        //    if (txtR4 != null)
        //    {
        //        curcnt += 1;
        //        mainavg = Convert.ToDecimal(txtR4);
        //    }
        //    if (txtR5 != null)
        //    {
        //        curcnt += 1;
        //        mainavg = Convert.ToDecimal(txtR5);
        //    }
        //    if (txtR6 != null)
        //    {
        //        curcnt += 1;
        //        mainavg = Convert.ToDecimal(txtR6);
        //    }

        //    // ng average - phase 3  nisa

        //    DataTable dthistory = await dbdal.getIDSDTransHistData(model.ID_IDS_D.ToString(), model.prodtype, model.lotno);

        //    int row = 1;
        //    decimal totalavg = Convert.ToDecimal(model.averageMold);
        //    decimal avgavg = 0;
        //    if (dthistory != null && dthistory.Rows.Count > 0)
        //    {
        //        for (int i = 0; i < dthistory.Rows.Count; i++)
        //        {
        //            row++;
        //            totalavg = Convert.ToDecimal(dthistory.Rows[i]["AVGRESULT"]) + totalavg;
        //        }

        //        avgavg = totalavg / row;
        //        model.averageMold = avgavg;
        //    }

        //    if (txtMr == "MAIN READING")
        //    {
        //        if (txtCnt == 0)
        //        {
        //            txtCnt = 1;
        //        }
        //        if (curcnt > txtCnt)
        //        {
        //            return Content("<script>alert('Error: Cannot Enter More Than Max Count Set In IDS Maintenance')</script>", "text/html");
        //        }
        //        else
        //        {
        //            data.averageMold = mainavg;
        //        }
        //    }

        //    return Json(model);
        //}

        //public async Task<ActionResult> GetCalcFormula(string propitem)
        //{
        //    DataTable dt = new DataTable();
        //    dt = await dbdal.GetDataDLLALL("Formula", propitem, "", "", "", "", "");
        //    string JSONString = string.Empty;
        //    JSONString = JsonConvert.SerializeObject(dt);
        //    return Json(JSONString);
        //}
        //#endregion

        //#region VIEW/EDIT
        //[SessionExpire]
        //public async Task<ActionResult> DE_IDS_TRANS_REV_DETAIL(string id)
        //{
        //    DailyIDSTransVM m = new DailyIDSTransVM();
        //    List<MoldingModel> mold = new List<MoldingModel>();

        //    var summary = await dbdal.getSummaryData(id);
        //    m.PRODTYPE = summary.PRODTYPE;
        //    m.LOTNO = summary.LOTNO;
        //    m.FullNG = summary.FullNG;
        //    m.PACKEDDATE = summary.PACKEDDATE;
        //    m.PACKEDDATE2 = summary.PACKEDDATE2;
        //    m.QUANTITY = summary.QUANTITY;
        //    m.TESTEDBY = summary.TESTEDBY;
        //    m.UPDATEDBY = summary.UPDATEDBY;
        //    m.STATUS = summary.STATUS;
        //    m.ID_IDS_H = summary.ID_IDS_H;
        //    m.MABS_P = summary.MABS_P;
        //    m.FLOT = summary.FLOT;
        //    m.GRADE = summary.GRADE;

        //    var moulding = await dbdal.getMouldingData(summary.PRODTYPE, "2", summary.LOTNO);

        //    m.MoldingList = moulding;

        //    string idhis = "";
        //    for (int i = 0; i < moulding.Count(); i++)
        //    {
        //        idhis += moulding[i].ID_IDS_D.ToString() + ",";
        //    }
        //    if (idhis != "")
        //    {
        //        idhis = idhis.Remove(idhis.Length - 1, 1);
        //    }

        //    //leha add mandatory
        //    var _datatable2 = await dbdal.DataChk56(m.PACKEDDATE, summary.prodgroup, m.PRODTYPE, summary.prodline);

        //    if (_datatable2 != null)
        //    {
        //        if (_datatable2 != null && _datatable2.Rows.Count > 0)
        //        {
        //            HttpContext.Session.SetString("_datatable2", _datatable2.ToString() ?? "");

        //            for (int x = 0; x < _datatable2.Rows.Count; x++)
        //            {
        //                for (int k = 0; k < m.MoldingList.Count; k++)
        //                {
        //                    if (m.MoldingList[k].PropItem == _datatable2.Rows[x]["PROPITEM"].ToString())
        //                    {
        //                        m.MoldingList[k].mandatory = "1";

        //                        if (m.MoldingList[k].Grade == "")
        //                        {
        //                            m.MoldingList[k].StatusInd = "Incomplete";
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    // checking with statusind

        //    // leha add mandatory end

        //    HttpContext.Session.SetString("idhis", idhis?.ToString() ?? "");

        //    var history = await dbdal.getTransHistData(idhis, m.PRODTYPE, m.LOTNO);
        //    var reg = "";
        //    var FinalR1 = "";
        //    var FinalR2 = "";
        //    var FinalR3 = "";
        //    var FinalR4 = "";
        //    var FinalR5 = "";
        //    var FinalR6 = "";
        //    DataTable cal = new DataTable();
        //    List<TransactionHistModel> list = new List<TransactionHistModel>();

        //    if (history != null && history.Rows.Count > 0)
        //    {
        //        for (int k = 0; k < history.Rows.Count; k++)
        //        {
        //            if (history.Rows[k]["REGRESSIONIND"].ToString() == "Y")
        //            {
        //                foreach (DataColumn column in history.Columns)
        //                {
        //                    if (column.ColumnName == "READING1")
        //                    {
        //                        var Reading1 = history.Rows[k]["READING1"].ToString();
        //                        if (Reading1 != null && Reading1 != "" && Reading1 != "NULL")
        //                        {
        //                            var formula = history.Rows[k]["REGRESSIONFORMULA"].ToString();
        //                            formula = formula.Replace(" ", "");
        //                            reg = formula.Replace("var1", Reading1);
        //                            var com = cal.Compute(reg, "");
        //                            FinalR1 = Math.Round(Convert.ToDouble(com), 2).ToString();
        //                        }
        //                        else
        //                        {
        //                            FinalR1 = "";
        //                        }

        //                    }
        //                    else if (column.ColumnName == "READING2")
        //                    {
        //                        var Reading2 = history.Rows[k]["READING2"].ToString();
        //                        if (Reading2 != null && Reading2 != "" && Reading2 != "NULL")
        //                        {
        //                            var formula = history.Rows[k]["REGRESSIONFORMULA"].ToString();
        //                            formula = formula.Replace(" ", "");
        //                            reg = formula.Replace("var1", Reading2);
        //                            var com = cal.Compute(reg, "");
        //                            FinalR2 = Math.Round(Convert.ToDouble(com), 2).ToString();
        //                        }
        //                        else
        //                        {
        //                            FinalR2 = "";
        //                        }
        //                    }
        //                    else if (column.ColumnName == "READING3")
        //                    {
        //                        var Reading3 = history.Rows[k]["READING3"].ToString();
        //                        if (Reading3 != null && Reading3 != "" && Reading3 != "NULL")
        //                        {
        //                            var formula = history.Rows[k]["REGRESSIONFORMULA"].ToString();
        //                            formula = formula.Replace(" ", "");
        //                            reg = formula.Replace("var1", Reading3);
        //                            var com = cal.Compute(reg, "");
        //                            FinalR3 = Math.Round(Convert.ToDouble(com), 2).ToString();
        //                        }
        //                        else
        //                        {
        //                            FinalR3 = "";
        //                        }
        //                    }
        //                    else if (column.ColumnName == "READING4")
        //                    {
        //                        var Reading4 = history.Rows[k]["READING4"].ToString();
        //                        if (Reading4 != null && Reading4 != "" && Reading4 != "NULL")
        //                        {
        //                            var formula = history.Rows[k]["REGRESSIONFORMULA"].ToString();
        //                            formula = formula.Replace(" ", "");
        //                            reg = formula.Replace("var1", Reading4);
        //                            var com = cal.Compute(reg, "");
        //                            FinalR4 = Math.Round(Convert.ToDouble(com), 2).ToString();
        //                        }
        //                        else
        //                        {
        //                            FinalR4 = "";
        //                        }
        //                    }
        //                    else if (column.ColumnName == "READING5")
        //                    {
        //                        var Reading5 = history.Rows[k]["READING5"].ToString();
        //                        if (Reading5 != null && Reading5 != "" && Reading5 != "NULL")
        //                        {
        //                            var formula = history.Rows[k]["REGRESSIONFORMULA"].ToString();
        //                            formula = formula.Replace(" ", "");
        //                            reg = formula.Replace("var1", Reading5);
        //                            var com = cal.Compute(reg, "");
        //                            FinalR5 = Math.Round(Convert.ToDouble(com), 2).ToString();
        //                        }
        //                        else
        //                        {
        //                            FinalR5 = "";
        //                        }
        //                    }
        //                    else if (column.ColumnName == "READING6")
        //                    {
        //                        var Reading6 = history.Rows[k]["READING6"].ToString();
        //                        if (Reading6 != null && Reading6 != "" && Reading6 != "NULL")
        //                        {
        //                            var formula = history.Rows[k]["REGRESSIONFORMULA"].ToString();
        //                            formula = formula.Replace(" ", "");
        //                            reg = formula.Replace("var1", Reading6);
        //                            var com = cal.Compute(reg, "");
        //                            FinalR6 = Math.Round(Convert.ToDouble(com), 2).ToString();
        //                        }
        //                        else
        //                        {
        //                            FinalR6 = "";
        //                        }
        //                    }
        //                }

        //            }

        //            list.Add(new TransactionHistModel
        //            {
        //                PropItem = history.Rows[k]["PROPITEM"].ToString(),
        //                Reading1 = history.Rows[k]["REGRESSIONIND"].ToString() == "Y" ? FinalR1.ToString() : history.Rows[k]["READING1"].ToString() == "NULL" ? "" : history.Rows[k]["READING1"].ToString(),
        //                Reading2 = history.Rows[k]["REGRESSIONIND"].ToString() == "Y" ? FinalR2.ToString() : history.Rows[k]["READING2"].ToString() == "NULL" ? "" : history.Rows[k]["READING2"].ToString(),
        //                Reading3 = history.Rows[k]["REGRESSIONIND"].ToString() == "Y" ? FinalR3.ToString() : history.Rows[k]["READING3"].ToString() == "NULL" ? "" : history.Rows[k]["READING3"].ToString(),
        //                Reading4 = history.Rows[k]["REGRESSIONIND"].ToString() == "Y" ? FinalR4.ToString() : history.Rows[k]["READING4"].ToString() == "NULL" ? "" : history.Rows[k]["READING4"].ToString(),
        //                Reading5 = history.Rows[k]["REGRESSIONIND"].ToString() == "Y" ? FinalR5.ToString() : history.Rows[k]["READING5"].ToString() == "NULL" ? "" : history.Rows[k]["READING5"].ToString(),
        //                Reading6 = history.Rows[k]["REGRESSIONIND"].ToString() == "Y" ? FinalR6.ToString() : history.Rows[k]["READING6"].ToString() == "NULL" ? "" : history.Rows[k]["READING6"].ToString(),
        //                Average = history.Rows[k]["AVERAGE"].ToString(),
        //                Rresult = history.Rows[k]["Rresult"].ToString(),
        //                Grade = history.Rows[k]["GRADE"].ToString(),
        //                TestedDateTime = history.Rows[k]["TESTEDDATETIME"].ToString(),

        //            });
        //        }
        //    }



        //    m.HistoryList = list;

        //    if (m.PRODTYPE != null && m.LOTNO != null)
        //    {
        //        var appearance = await apptabE(id, m);
        //        m.descriptionList = appearance.descriptionList;
        //    }

        //    var appearanceTable = await dbdal.getAppearanceList(id);
        //    m.AppearanceTableList = appearanceTable;

        //    //----- molding segregation
        //    for (int a = 0; a < moulding.Count; a++)
        //    {
        //        if (moulding[a].Grade == "NG")
        //        {
        //            var properties = moulding[a].Properties;
        //            var propitem = moulding[a].PropItem;
        //            var lotno = summary.LOTNO;
        //            var prodtype = summary.PRODTYPE;
        //            var dtchk = new DataTable();
        //            dtchk = await GetDataChk(dtchk, properties, propitem, lotno, prodtype);

        //            moulding[a].seg = false;
        //            moulding[a].extra = true;

        //            if (dtchk != null && dtchk.Rows.Count > 0)
        //            {
        //                for (int b = 0; b < dtchk.Rows.Count; b++)
        //                {
        //                    if (dtchk.Rows[b]["ID_IDS_D"].ToString() == moulding[a].ID_IDS_D.ToString())
        //                    {
        //                        if (dtchk.Rows[b]["TestType"].ToString() == "Y")
        //                        {
        //                            if (dtchk.Rows[b]["SegregationInd"].ToString() == "Y")
        //                            {
        //                                if (Convert.ToInt32(dtchk.Rows[b]["TTLCNT"]) >= Convert.ToInt32(dtchk.Rows[b]["avgMainReadingCount"]))
        //                                {
        //                                    if (moulding[a].TypeInd == "E")
        //                                    {
        //                                        moulding[a].seg = true;
        //                                        moulding[a].extra = false;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    HttpContext.Session.SetString("lotno", summary.LOTNO?.ToString() ?? "");


        //    DEIDSTCM.ModelObj = m;

        //    string emp_no = (HttpContext.Session.GetObject<ACL_UserObj>("AclUser")).EMP_NO.ToString();

        //    DataTable chkApproval = await dbdal.CheckUserAppRole(emp_no);
        //    if (chkApproval != null && chkApproval.Rows.Count > 0)
        //    {
        //        ViewBag.isManager = "Y";
        //    }

        //    //for KS
        //    if ((HttpContext.Session.GetObject<ACL_UserObj>("AclUser")).USER_ID.ToString() == "800133")
        //    {
        //        ViewBag.isManager = "Y";
        //    }

        //    ViewBag.IDH = id;
        //    return View(DEIDSTCM);
        //}

        //[HttpPost]
        //public async Task<ActionResult> DE_IDS_TRANS_REV_DETAIL(DailyIDSTransVM m, List<List<fieldnamemodel>> field, List<List<List<tagnomodel>>> tag, List<descmodel> description, string rectype)
        //{
        //    string loc = HttpContext.Connection.RemoteIpAddress?.ToString();
        //    var aclUser = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");

        //    m.CREATED_BY = aclUser.USER_ID;
        //    m.CREATED_LOC = loc;

        //    string emp_no = (HttpContext.Session.GetObject<ACL_UserObj>("AclUser")).EMP_NO.ToString();

        //    DataTable chkApproval = await dbdal.CheckUserAppRole(emp_no);
        //    if (chkApproval != null && chkApproval.Rows.Count > 0)
        //    {
        //        ViewBag.isManager = "Y";
        //    }

        //    //for KS
        //    if ((HttpContext.Session.GetObject<ACL_UserObj>("AclUser")).USER_ID.ToString() == "800133")
        //    {
        //        ViewBag.isManager = "Y";
        //    }


        //    var result = "";
        //    string tmp = await dbdal.lockCheck(m, "1", "");
        //    string[] tmp2 = tmp.Split('-');
        //    if (rectype == "1")
        //    {
        //        if (tmp2[0] == "0")
        //        {
        //            // add history list for plus button

        //            if (m.newHistoryList != null && m.newHistoryList.Count() > 0)
        //            {
        //                await IDS_SL_MAINT(m);
        //            }

        //            string NG = "";
        //            result = await dbdal.IDS_H_Maint(m, "1");
        //            await IDS_D_Maint(m, result, "1");
        //            await saveapptab(tag, description, result, NG, "1", m, field);
        //            await chkData(m, result);
        //            result = "Data save successfully";
        //            await dbdal.lockCheck(m, "3", tmp2[1]);
        //            return Json(result);
        //        }
        //        else
        //        {
        //            result = tmp2[0];
        //            return Json(result);
        //        }
        //    }
        //    else
        //    {
        //        if (tmp2[0] == "0")
        //        {
        //            // add history list for plus button

        //            if (m.newHistoryList != null && m.newHistoryList.Count() > 0)
        //            {
        //                await IDS_SL_MAINT(m);
        //            }

        //            result = m.ID_IDS_H.ToString();

        //            await IDS_D_Maint(m, result, rectype);
        //            await saveapptab(tag, description, result, "", rectype, m, field);

        //            await VerticalPerCen(m);
        //            await chkData(m, result);

        //            result = "Data save successfully";

        //            await dbdal.lockCheck(m, "3", tmp2[1]);
        //        }
        //        else
        //        {
        //            result = tmp2[0];
        //            return Json(result);
        //        }

        //    }

        //    return Json(result);
        //}

        //public async Task<JsonResult> SaveComplete(DailyIDSTransVM model)
        //{
        //    string loc = HttpContext.Connection.RemoteIpAddress?.ToString();
        //    var aclUser = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");

        //    model.CREATED_BY = aclUser.USER_ID;
        //    model.CREATED_LOC = loc;
        //    model.STATUS = "Completed";

        //    var vm = await dbdal.IDS_H_Maint(model, "6");
        //    await chkData(model, model.ID_IDS_H.ToString());
        //    vm = "Data save successfully";
        //    return Json(vm);
        //}

        //public async Task<JsonResult> DeleteDraft(string id_ids_h)
        //{
        //    var result = "";
        //    result = await dbdal.DeleteDraft(id_ids_h);

        //    return Json(result);
        //}

        //public async Task<DailyIDSTransVM>apptabE(string id, DailyIDSTransVM vm)
        //{
        //    List<fieldnamemodel> field = new List<fieldnamemodel>();
        //    List<tagnomodel> tag = new List<tagnomodel>();
        //    List<descmodel> d = new List<descmodel>();
        //    List<List<fieldnamemodel>> fieldList = new List<List<fieldnamemodel>>();
        //    List<List<tagnomodel>> tagList = new List<List<tagnomodel>>();

        //    descriptionmodel mdl = new descriptionmodel();
        //    List<descriptionmodel> mdlList = new List<descriptionmodel>();

        //    DataTable datatable3 = new DataTable();
        //    DataTable datatable5 = new DataTable();
        //    datatable3 = await dbdal.GetDataDLLALL("E_AppBorder", "E", id, "", "", "", "");

        //    //set for description
        //    if (datatable3 != null && datatable3.Rows.Count > 0)
        //    {
        //        for (int i = 0; i < datatable3.Rows.Count; i++)
        //        {
        //            var description = datatable3.Rows[i]["description"].ToString();
        //            var id_ids_d = Convert.ToInt32(datatable3.Rows[i]["id_ids_d"]);//ch1461 - 1472
        //            var ratio = datatable3.Rows[i]["calcratio"].ToString();
        //            var rounding = datatable3.Rows[i]["ROUNDING"].ToString();
        //            d.Add(new descmodel
        //            {
        //                desc = description,
        //                id_ids_d = id_ids_d,
        //                ratio = ratio,
        //                rounding = rounding
        //            });
        //            //datatable5 = db2.GetDataDLLALL("E_AppTbl", id_ids_d.ToString(), "", "", "", "", "");	
        //            //for(int j = 0; j < datatable5.Rows.Count; j++)	
        //            //{	
        //            //}
        //        }
        //    }

        //    List<fieldnamemodel> fieldratio = new List<fieldnamemodel>();
        //    List<List<ratioModel>> Finalratio = new List<List<ratioModel>>();

        //    string values = "";
        //    for (int type = 1; type <= 2; type++)
        //    {
        //        List<ratioModel> ratio = new List<ratioModel>();

        //        DataTable appearanceDataTable = new DataTable();
        //        appearanceDataTable = await dbdal.GetAppearanceTableList(vm.PRODTYPE, vm.LOTNO, type);
        //        List<string> ColumnHeaderList = new List<string>();

        //        if (type == 1)
        //        {
        //            fieldratio = await dbdal.getfielname("E_AppColumn", "BLACK SPECK", vm.PRODTYPE, "", "", "", "");
        //        }
        //        else
        //        {
        //            fieldratio = await dbdal.getfielname("E_AppColumn", "YI", vm.PRODTYPE, "", "", "", "");
        //        }

        //        foreach (var item in fieldratio)
        //        {
        //            ratio.Add(new ratioModel { name = item.fieldname, ratio = Convert.ToDecimal(item.calcratio) });
        //        }

        //        if (appearanceDataTable != null && appearanceDataTable.Rows.Count > 0)
        //        {
        //            foreach (DataColumn dataColumn in appearanceDataTable.Columns)
        //            {
        //                field.Add(new fieldnamemodel { fieldname = dataColumn.ColumnName });
        //                ColumnHeaderList.Add(dataColumn.ColumnName);
        //            }
        //        }

        //        //set for column name

        //        fieldList.Add(field);
        //        field = new List<fieldnamemodel>();

        //        //set for value
        //        if (appearanceDataTable != null && appearanceDataTable.Rows.Count > 0)
        //        {
        //            foreach (DataRow dataRow in appearanceDataTable.Rows)
        //            {
        //                foreach (string columnName in ColumnHeaderList)
        //                {
        //                    values += DBNull.Value.Equals(dataRow[columnName]) ? "" + "," : dataRow[columnName].ToString() + ",";

        //                }
        //                values.Remove(values.Length - 1, 1);
        //                tag.Add(new tagnomodel { tagno = values });
        //                values = "";

        //            }
        //        }

        //        tagList.Add(tag);
        //        tag = new List<tagnomodel>();

        //        Finalratio.Add(ratio);

        //    }

        //    mdl.fieldnameList = fieldList;
        //    mdl.tagnoList = tagList;
        //    mdl.descList = d;

        //    mdlList.Add(mdl);
        //    vm.descriptionList = mdlList;
        //    vm.ratioList = Finalratio;

        //    return vm;
        //}

        //public async Task<ActionResult> GetIdsSummary(string idsh)
        //{
        //    DataTable dt = new DataTable();
        //    dt = await dbdal.IDS_SUM_SEL(idsh);
        //    string JSONString = string.Empty;
        //    JSONString = JsonConvert.SerializeObject(dt);
        //    return Json(JSONString);
        //}

        //public async Task<JsonResult> GetSegregationL(string part, string id_ids_d, string prodtype, string propitem, string packeddate2, string lotno)
        //{
        //    var temp = await dbdal.GetDataDLLALL("E_Chk1", id_ids_d, "", "", "", "", "");
        //    var pddlVal = "";
        //    var ddlpassfail = false;
        //    var btnSave = false;
        //    var _from = "";
        //    var _to = "";


        //    if (temp != null && temp.Rows.Count > 0)
        //    {
        //        pddlVal = temp.Rows[0]["ptype"].ToString();
        //        ddlpassfail = false;
        //        btnSave = false;

        //    }
        //    else
        //    {
        //        var temp2 = await dbdal.GetDataDLLALL("NewEntry", "E_Chk2", lotno, prodtype, packeddate2, "", "");
        //        if (temp2 != null)
        //        {
        //            _from = temp2.Rows[0]["res1"].ToString();
        //            _to = temp2.Rows[0]["res2"].ToString();
        //        }
        //        ddlpassfail = true;
        //    }

        //    var dt = await calltable1(part, id_ids_d, prodtype, propitem, packeddate2, lotno, _from, _to);
        //    //LoadSubSeg(part, _from, _to, reading, gradeInd, lotno, ids_d);
        //    List<SegregationSegLModel> segL = new List<SegregationSegLModel>();

        //    if (dt != null && dt.Rows.Count > 0)
        //    {
        //        foreach (DataRow row in dt.Rows)
        //        {
        //            SegregationSegLModel m = new SegregationSegLModel();
        //            m.id_ids_segl = row["ID_IDS_SegL"].ToString();
        //            m.tagno = row["TagNo"].ToString();
        //            m.reading = row["Reading"].ToString();
        //            m.grade = row["Grade"].ToString();
        //            m.gradeInd = row["Gradeind"].ToString();
        //            m.compare = row["compare"].ToString();
        //            m.tonnage = row["tonnage"].ToString();

        //            var checkSeg = await dbdal.CheckSegL(id_ids_d);

        //            if (checkSeg != null && checkSeg.Rows.Count > 0)
        //            {
        //                var words = m.tagno.Split('-');
        //                int tagfrom = Convert.ToInt32(words[0]);
        //                var tagto = Convert.ToInt32(words[1]);
        //                var diff = tagto - tagfrom;
        //                int diffgap = 0;

        //                if (m.compare == "0")
        //                {
        //                    if (pddlVal != "Part 2" && m.grade != "")
        //                    {
        //                        m.subNG = true;
        //                    }
        //                }

        //                if (pddlVal == "Part 1")
        //                {
        //                    diffgap = 5;
        //                }
        //                else if (pddlVal == "Part 3")
        //                {
        //                    diffgap = 40;
        //                }

        //                if (diff < diffgap)
        //                {
        //                    m.subNG = false;
        //                }
        //            }
        //            else
        //            {
        //                m.subNG = false;
        //            }


        //            m.ptype = part;
        //            m.ddlpassfail = ddlpassfail;
        //            m.btnSave = btnSave;
        //            segL.Add(m);

        //        }
        //    }


        //    TempData["from"] = _from;
        //    TempData["to"] = _to;
        //    TempData["packdate"] = packeddate2;
        //    TempData["lotno"] = lotno;
        //    TempData["prodtypeSub"] = prodtype;
        //    TempData["propitemSub"] = propitem;
        //    TempData["id_ids_d"] = id_ids_d;
        //    TempData["pype"] = pddlVal;

        //    return Json(segL);
        //}

        //public async Task<DataTable>  calltable1(string part, string id_ids_d, string prodtype, string propitem, string packeddate2, string lotno, string _from, string _to)
        //{
        //    var temp = new DataTable();

        //    if (part == "Part 2")
        //    {
        //        temp = await dbdal.GetDataDLLALL("E_BSSeg_P2", _from, _to, "5", id_ids_d, "", "");
        //    } else if (part == "Part 3")
        //    {
        //        temp = await dbdal.GetDataDLLALL("E_BSSeg_P2", _from, _to, "40", id_ids_d, "", "");
        //    }
        //    else
        //    {
        //        temp = await dbdal.GetDataDLLALL("E_BSSeg_P2", _from, _to, "1", id_ids_d, "", "");
        //    }

        //    //if (part == "Part 2")
        //    //{
        //    //    temp = await dbdal.GetDataDLLALL("E_BSSeg_P2", _from, _to, "1", id_ids_d, "", "");
        //    //}
        //    //else if (part == "Part 3")
        //    //{
        //    //    temp = await dbdal.GetDataDLLALL("E_MinAvgMax", lotno, id_ids_d, "", "", "", "");
        //    //}
        //    //else
        //    //{
        //    //    temp = await dbdal.GetDataDLLALL("E_BSSeg_P1", _from, _to, lotno, id_ids_d, prodtype, packeddate2);
        //    //}

        //    return temp;
        //}

        //public async Task<JsonResult> SaveSegregationL(List<SegregationSegLModel> seglist)
        //{
        //    var vm = "";
        //    foreach (var item in seglist)
        //    {
        //        var words = item.tagno.Split('-');
        //        var tagfrom = words[0].ToString();
        //        var tagto = words[1].ToString();
        //        DataTable temp;
        //        var paction = "";
        //        var grade = "";

        //        if (item.id_ids_segl == "0")
        //        {
        //            paction = "1";
        //        }
        //        else
        //        {
        //            paction = "3";
        //        }

        //        if (item.reading == "" || item.reading == "0")
        //        {
        //            item.grade = "A";
        //        }
        //        else
        //        {
        //            temp = await dbdal.DataChk(item.prodtype, item.reading, "1", item.propitem);
        //            item.grade = temp.Rows[0]["GRADE"].ToString().Remove(0, 2);
        //        }

        //        if (item.grade == "NG")
        //        {
        //            grade += item.tagno + " : " + item.grade + " \n ";
        //        }

        //        string loc = HttpContext.Connection.RemoteIpAddress?.ToString();
        //        var aclUser = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");

        //        var created_by = aclUser.USER_ID;
        //        var location = loc;

        //        var _tempD = await dbdal.IDS_Seg_Maint(item.id_ids_segl, item.id_ids_d, tagfrom, tagto, item.reading, item.grade, item.gradeInd, paction, item.ptype, created_by, location);

        //        var _from = TempData["from"].ToString();
        //        var _to = TempData["to"].ToString();
        //        var packdate = TempData["packdate"].ToString();
        //        var lotno = TempData["lotno"].ToString();

        //        if (grade != "" && item.ptype != "Part 2")
        //        {
        //            var dt = await calltable1(item.ptype, item.id_ids_d, item.prodtype, item.propitem, packdate, lotno, _from, _to);
        //            List<SegregationSegLModel> segL = new List<SegregationSegLModel>();
        //            if (dt != null && dt.Rows.Count > 0)
        //            {
        //                foreach (DataRow row in dt.Rows)
        //                {
        //                    SegregationSegLModel m = new SegregationSegLModel();
        //                    m.id_ids_segl = row["ID_IDS_SegL"].ToString();
        //                    m.tagno = row["TagNo"].ToString();
        //                    m.reading = row["Reading"].ToString();
        //                    m.grade = row["Grade"].ToString();
        //                    m.gradeInd = row["Gradeind"].ToString();
        //                    m.compare = row["compare"].ToString();

        //                    var checkSeg = await dbdal.CheckSegL(item.id_ids_d);

        //                    if (checkSeg != null && checkSeg.Rows.Count > 0)
        //                    {
        //                        if ((row["Grade"].ToString() != "" || row["Grade"].ToString() != null) && item.ptype != "Part 2")
        //                        {
        //                            m.subNG = true;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        m.subNG = false;
        //                    }

        //                    segL.Add(m);

        //                }
        //            }

        //        }
        //        vm = "ok";
        //    }

        //    return Json(vm);
        //}

        //public async Task<JsonResult> LoadSubSeg(string part, string tagnofrom, string tagnoto, string reading, string gradeInd, string lotno, string ids_d)
        //{
        //    var dt = new DataTable();
        //    var vm = "No Product Type Found";
        //    if (part == "Part 1")
        //    {
        //        dt = await calltableYI(tagnofrom, tagnoto, gradeInd, lotno, ids_d);

        //    }
        //    else if (part == "Part 3")
        //    {
        //        dt = await calltable(tagnofrom, tagnoto, gradeInd);
        //    }

        //    if (dt == null || dt.Rows.Count <= 0)
        //    {
        //        return Json(vm);
        //    }
        //    else
        //    {
        //        List<SubSegregationSegLModel> sub = new List<SubSegregationSegLModel>();

        //        if (dt != null && dt.Rows.Count > 0)
        //        {
        //            foreach (DataRow row in dt.Rows)
        //            {
        //                SubSegregationSegLModel seg = new SubSegregationSegLModel();
        //                seg.id_ids_segl = row["ID_IDS_SegL"].ToString();
        //                seg.tagno = row["TagNo"].ToString();
        //                seg.grade = row["Grade"].ToString();
        //                seg.gradeInd = row["GradeInd"].ToString();

        //                if (row == dt.Rows[dt.Rows.Count - 1])
        //                {
        //                    seg.reading = reading;
        //                }
        //                else
        //                {
        //                    seg.reading = row["Reading"].ToString();
        //                }


        //                sub.Add(seg);
        //            }
        //        }

        //        return Json(sub);
        //    }
        //}

        //public async Task<DataTable>  calltableYI(string tagnofrom, string tagnoto, string gradeInd, string lotno, string ids_d)
        //{
        //    DataTable temp = new DataTable();

        //    var words = lotno.Split('-');
        //    var word = words[0].Remove(0, 1);
        //    var gradeind = Convert.ToInt32(gradeInd) + 1;

        //    DataTable chk = await dbdal.GetDataDLLALL("E_ChkLine", word, "", "", "", "", "");

        //    if (chk != null && chk.Rows.Count > 0)
        //    {
        //        temp = await dbdal.GetDataDLLALL("NewEntry", "E_BSSeg2", tagnofrom, tagnoto, chk.Rows[0]["PRODLINE"].ToString(), ids_d, gradeInd);

        //    }
        //    return temp;
        //}

        //public async Task<DataTable>  calltable(string tagnofrom, string tagnoto, string gradeInd)
        //{
        //    var temp = await dbdal.GetDataDLLALL("E_MinAvgMax_SubSeg", tagnofrom, tagnoto, gradeInd, "", "", "");
        //    return temp;
        //}

        //public async Task<ActionResult> GetSegreationBS(string lotno, string from, string to, string id_ids_d, string ftag)
        //{
        //    var vm = "";
        //    var words = lotno.Split('-');
        //    var word = words[0].Remove(0, 1);
        //    var chk = await dbdal.GetDataDLLALL("E_ChkLine", word, "", "", "", "", "");

        //    List<SegregationBSModel> segBS = new List<SegregationBSModel>();
        //    if (chk != null && chk.Rows.Count != 0)
        //    {
        //        var temp = await dbdal.GetDataDLLALL("NewEntry", "E_BSSeg", from, to, chk.Rows[0]["PRODLINE"].ToString(), id_ids_d, "");
        //        if (temp != null && temp.Rows.Count != 0)
        //        {
        //            foreach (DataRow row in temp.Rows)
        //            {
        //                SegregationBSModel m = new SegregationBSModel();
        //                m.id = row["res2"].ToString();
        //                m.tagno = row["res3"].ToString();
        //                m.reading = row["res4"].ToString();
        //                m.grade = row["res5"].ToString();
        //                m.gradeInd = row["res7"].ToString();
        //                m.id_ids_d = id_ids_d;
        //                m.grade = row["Grade"].ToString();
        //                var tmp = m.tagno.Split('-');
        //                m.tonnage = (Convert.ToInt32(tmp[1]) - Convert.ToInt32(ftag) + 1).ToString();
        //                segBS.Add(m);

        //            }
        //        }
        //    }
        //    return Json(segBS);
        //}

        //public async Task<JsonResult> SaveSegregationBS(List<SegregationBSModel> seglist)
        //{
        //    var vm = "";
        //    foreach (var item in seglist)
        //    {
        //        var words = item.tagno.Split('-');
        //        var tagfrom = words[0].ToString();
        //        var tagto = words[1].ToString();
        //        DataTable temp;

        //        if (item.grade == "" || item.grade == null)
        //        {
        //            item.grade = "A";
        //        }
        //        else
        //        {
        //            temp = await dbdal.DataChk(item.prodtype, item.reading, "3", "");
        //            item.grade = temp.Rows[0]["GRADE"].ToString().Remove(0, 2);
        //        }

        //        var GradeInd = item.gradeInd;

        //        string loc = HttpContext.Connection.RemoteIpAddress?.ToString();
        //        var aclUser = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");

        //        var updatedby = aclUser.USER_ID;

        //        var tempD = await dbdal.IDS_Seg_Maint(item.id, item.id_ids_d, tagfrom, tagto, item.reading, item.grade, GradeInd, "1", "", updatedby, loc);

        //        vm = "ok";
        //    }

        //    return Json(vm);
        //}

        //public async Task VerticalPerCen(DailyIDSTransVM m)
        //{
        //    var passper = 0.0;
        //    var finalresult = 0.0;
        //    DataTable datachkdt = new DataTable();
        //    DataTable temp = new DataTable();
        //    var id_ids_d = 0;
        //    var prevprop = "";
        //    var qty = 0.0;
        //    var ngqty = 0.0;
        //    var ttlcnt = 0;
        //    var grade = "";
        //    var cntng = 0;

        //    for (int i = 0; i < m.HistoryList.Count; i++)
        //    {
        //        grade = m.HistoryList[i].Grade;
        //        var average = m.HistoryList[i].Average;

        //        if (average != "Pass" || average != "Fail")
        //        {
        //            // 1st row
        //            if (i == 0)
        //            {
        //                ttlcnt = 1;
        //                prevprop = m.HistoryList[i].PropItem;

        //                if (grade == "NG")
        //                {
        //                    cntng += 1;
        //                    ngqty += Convert.ToDouble(average);
        //                }
        //                else
        //                {
        //                    qty += Convert.ToDouble(average);
        //                }

        //                if (prevprop == m.HistoryList[i + 1].PropItem)
        //                {
        //                    //do nothing
        //                }
        //            }
        //            else
        //            {
        //                //if not 1st row
        //                // same propitem from prev rows
        //                if (prevprop == m.HistoryList[i].PropItem)
        //                {
        //                    ttlcnt += 1;
        //                    if (grade == "NG")
        //                    {
        //                        cntng += 1;
        //                        ngqty += Convert.ToDouble(average);
        //                    }
        //                    else
        //                    {
        //                        qty += Convert.ToDouble(average);
        //                    }

        //                    if (i < m.HistoryList.Count - 1)
        //                    {
        //                        if (prevprop != m.HistoryList[i + 1].PropItem)
        //                        {
        //                            datachkdt = await GetDataChk(datachkdt, m.HistoryList[i].Properties, m.HistoryList[i].PropItem, m.LOTNO, m.PRODTYPE);
        //                            if (datachkdt != null && datachkdt.Rows.Count > 0)
        //                            {
        //                                passper = Convert.ToDouble(datachkdt.Rows[0]["PASSVERT"]);
        //                            }

        //                            double ttlcnt2 = ttlcnt;
        //                            double cntng2 = cntng;
        //                            double qty2 = qty;
        //                            double ngqty2 = ngqty;

        //                            if (Math.Round(((ttlcnt2 - cntng2) / ttlcnt2) * 100, 2) >= Math.Round(Convert.ToDouble(passper), 2))
        //                            {
        //                                finalresult = Math.Round((qty2 / (ttlcnt2 - cntng2)), 2);
        //                            }
        //                            else
        //                            {
        //                                finalresult = Math.Round((ngqty2 / (cntng2)), 2);
        //                                grade = "NGNG";
        //                            }

        //                            temp = await dbdal.DataChk(m.PRODTYPE, finalresult.ToString(), "1", prevprop);
        //                            if (temp != null && temp.Rows.Count > 0)
        //                            {
        //                                grade = "";
        //                                for (int ii = 0; ii < temp.Rows.Count; ii++)
        //                                {
        //                                    if (temp.Rows[ii]["GRADE"].ToString() != "NGNG")
        //                                    {
        //                                        grade = temp.Rows[ii]["GRADE"].ToString();
        //                                    }

        //                                    if (grade == "")
        //                                    {
        //                                        grade = "NGNG";
        //                                    }

        //                                    int len = grade.Length - 2;
        //                                    grade = grade.Substring(2, len);

        //                                    await dbdal.IDS_D_UPD(m.HistoryList[i].id_ids_d, finalresult.ToString(), grade);
        //                                }
        //                            }
        //                        }
        //                    }
        //                    else if (i == m.HistoryList.Count - 1)
        //                    {
        //                        await GetDataChk(datachkdt, m.HistoryList[i].Properties, m.HistoryList[i].PropItem, "", "");
        //                        if (datachkdt != null && datachkdt.Rows.Count > 0)
        //                        {
        //                            passper = Convert.ToDouble(datachkdt.Rows[0]["PASSVERT"]);
        //                        }


        //                        double ttlcnt2 = ttlcnt;
        //                        double cntng2 = cntng;
        //                        double qty2 = qty;
        //                        double ngqty2 = ngqty;

        //                        if (Math.Round(((ttlcnt2 - cntng2) / ttlcnt2) * 100, 2) >= Math.Round(Convert.ToDouble(passper), 2))
        //                        {
        //                            finalresult = Math.Round((qty2 / (ttlcnt2 - cntng2)), 2);
        //                        }
        //                        else
        //                        {
        //                            finalresult = Math.Round((ngqty2 / (cntng2)), 2);
        //                            grade = "NGNG";
        //                        }

        //                        temp = await dbdal.DataChk(m.PRODTYPE, finalresult.ToString(), "1", prevprop);
        //                        if (temp != null && temp.Rows.Count > 0)
        //                        {
        //                            grade = "";
        //                            for (int ii = 0; ii < temp.Rows.Count; ii++)
        //                            {
        //                                if (temp.Rows[ii]["GRADE"].ToString() != "NGNG")
        //                                {
        //                                    grade = temp.Rows[ii]["GRADE"].ToString();
        //                                }

        //                                if (grade == "")
        //                                {
        //                                    grade = "NGNG";
        //                                }

        //                                int len = grade.Length - 2;
        //                                grade = grade.Substring(2, len);

        //                                await dbdal.IDS_D_UPD(m.HistoryList[i].id_ids_d, finalresult.ToString(), grade);
        //                            }
        //                        }
        //                    }
        //                    id_ids_d = Convert.ToInt32(m.HistoryList[i].id_ids_d);
        //                }
        //                else
        //                {
        //                    // not same propiyem from prev rows
        //                    ttlcnt = 1;
        //                    cntng = 0;
        //                    ngqty = 0;
        //                    qty = 0;
        //                    prevprop = m.HistoryList[i].PropItem;

        //                    if (grade == "NG")
        //                    {
        //                        cntng = 1;
        //                        ngqty += Convert.ToDouble(average);
        //                    }
        //                    else
        //                    {
        //                        if (average == "")
        //                        {
        //                            average = "0";
        //                        }
        //                        qty += Convert.ToDouble(average);
        //                    }

        //                    id_ids_d = Convert.ToInt32(m.HistoryList[i].id_ids_d);
        //                }
        //            }
        //        }
        //    }
        //}

        //public async Task getcoqcopye(DailyIDSTransVM m)
        //{
        //    var getCOQCopyE = await dbdal.COQCopy(m.LOTNO, m.PRODTYPE, "3");
        //}

        //public async Task<JsonResult> IDS_SL_MAINT(DailyIDSTransVM m)
        //{
        //    string loc = HttpContext.Connection.RemoteIpAddress?.ToString();
        //    var aclUser = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");

        //    m.CREATED_BY = aclUser.USER_ID;
        //    m.CREATED_LOC = loc;
        //    List<TransactionHistModel> list = new List<TransactionHistModel>();

        //    if (m.newHistoryList != null && m.newHistoryList.Count() > 0)
        //    {
        //        for(int i=0; i < m.newHistoryList.Count(); i++)
        //        {
        //            var prodgroup = m.newHistoryList[i].ProdGroup == null ? "" : m.newHistoryList[i].ProdGroup;
        //            int id_ids_d = m.newHistoryList[i].ID_IDS_D == 0 ? 0 : m.newHistoryList[i].ID_IDS_D;

        //            var reading1 = m.newHistoryList[i].Reading1 == null ? "" : m.newHistoryList[i].Reading1;
        //            var reading2 = m.newHistoryList[i].Reading2 == null ? "" : m.newHistoryList[i].Reading2;
        //            var reading3 = m.newHistoryList[i].Reading3 == null ? "" : m.newHistoryList[i].Reading3;
        //            var reading4 = m.newHistoryList[i].Reading4 == null ? "" : m.newHistoryList[i].Reading4;
        //            var reading5 = m.newHistoryList[i].Reading5 == null ? "" : m.newHistoryList[i].Reading5;
        //            var reading6 = m.newHistoryList[i].Reading6 == null ? "" : m.newHistoryList[i].Reading6;

        //            //prodgroup save in id_ids_sl
        //            var sl = await dbdal.IDS_SL_MAINT(prodgroup, id_ids_d.ToString(), reading1, reading2, reading3, reading4, reading5, reading6,
        //                m.newHistoryList[i].Average, m.newHistoryList[i].Grade, "1", m.CREATED_BY, m.CREATED_LOC);

        //            var idshis = HttpContext.Session.GetString("idhis").ToString();
        //            var history = await dbdal.getTransHistData(idshis, m.PRODTYPE, m.LOTNO);
        //            var reg = "";
        //            var FinalR1 = "";
        //            var FinalR2 = "";
        //            var FinalR3 = "";
        //            var FinalR4 = "";
        //            var FinalR5 = "";
        //            var FinalR6 = "";
        //            DataTable cal = new DataTable();

        //            if (history != null && history.Rows.Count > 0)
        //            {
        //                for (int k = 0; k < history.Rows.Count; k++)
        //                {
        //                    if (history.Rows[k]["REGRESSIONIND"].ToString() == "Y")
        //                    {
        //                        foreach (DataColumn column in history.Columns)
        //                        {
        //                            if (column.ColumnName == "READING1")
        //                            {
        //                                var Reading1 = history.Rows[k]["READING1"].ToString();
        //                                if (Reading1 != null && Reading1 != "" && Reading1 != "NULL")
        //                                {
        //                                    var formula = history.Rows[k]["REGRESSIONFORMULA"].ToString();
        //                                    formula = formula.Replace(" ", "");
        //                                    reg = formula.Replace("var1", Reading1);
        //                                    var com = cal.Compute(reg, "");
        //                                    FinalR1 = Math.Round(Convert.ToDouble(com), 2).ToString();
        //                                }
        //                                else
        //                                {
        //                                    FinalR1 = "";
        //                                }

        //                            }
        //                            else if (column.ColumnName == "READING2")
        //                            {
        //                                var Reading2 = history.Rows[k]["READING2"].ToString();
        //                                if (Reading2 != null && Reading2 != "" && Reading2 != "NULL")
        //                                {
        //                                    var formula = history.Rows[k]["REGRESSIONFORMULA"].ToString();
        //                                    formula = formula.Replace(" ", "");
        //                                    reg = formula.Replace("var1", Reading2);
        //                                    var com = cal.Compute(reg, "");
        //                                    FinalR2 = Math.Round(Convert.ToDouble(com), 2).ToString();
        //                                }
        //                                else
        //                                {
        //                                    FinalR2 = "";
        //                                }
        //                            }
        //                            else if (column.ColumnName == "READING3")
        //                            {
        //                                var Reading3 = history.Rows[k]["READING3"].ToString();
        //                                if (Reading3 != null && Reading3 != "" && Reading3 != "NULL")
        //                                {
        //                                    var formula = history.Rows[k]["REGRESSIONFORMULA"].ToString();
        //                                    formula = formula.Replace(" ", "");
        //                                    reg = formula.Replace("var1", Reading3);
        //                                    var com = cal.Compute(reg, "");
        //                                    FinalR3 = Math.Round(Convert.ToDouble(com), 2).ToString();
        //                                }
        //                                else
        //                                {
        //                                    FinalR3 = "";
        //                                }
        //                            }
        //                            else if (column.ColumnName == "READING4")
        //                            {
        //                                var Reading4 = history.Rows[k]["READING4"].ToString();
        //                                if (Reading4 != null && Reading4 != "" && Reading4 != "NULL")
        //                                {
        //                                    var formula = history.Rows[k]["REGRESSIONFORMULA"].ToString();
        //                                    formula = formula.Replace(" ", "");
        //                                    reg = formula.Replace("var1", Reading4);
        //                                    var com = cal.Compute(reg, "");
        //                                    FinalR4 = Math.Round(Convert.ToDouble(com), 2).ToString();
        //                                }
        //                                else
        //                                {
        //                                    FinalR4 = "";
        //                                }
        //                            }
        //                            else if (column.ColumnName == "READING5")
        //                            {
        //                                var Reading5 = history.Rows[k]["READING5"].ToString();
        //                                if (Reading5 != null && Reading5 != "" && Reading5 != "NULL")
        //                                {
        //                                    var formula = history.Rows[k]["REGRESSIONFORMULA"].ToString();
        //                                    formula = formula.Replace(" ", "");
        //                                    reg = formula.Replace("var1", Reading5);
        //                                    var com = cal.Compute(reg, "");
        //                                    FinalR5 = Math.Round(Convert.ToDouble(com), 2).ToString();
        //                                }
        //                                else
        //                                {
        //                                    FinalR5 = "";
        //                                }
        //                            }
        //                            else if (column.ColumnName == "READING6")
        //                            {
        //                                var Reading6 = history.Rows[k]["READING6"].ToString();
        //                                if (Reading6 != null && Reading6 != "" && Reading6 != "NULL")
        //                                {
        //                                    var formula = history.Rows[k]["REGRESSIONFORMULA"].ToString();
        //                                    formula = formula.Replace(" ", "");
        //                                    reg = formula.Replace("var1", Reading6);
        //                                    var com = cal.Compute(reg, "");
        //                                    FinalR6 = Math.Round(Convert.ToDouble(com), 2).ToString();
        //                                }
        //                                else
        //                                {
        //                                    FinalR6 = "";
        //                                }
        //                            }
        //                        }
        //                    }

        //                    list.Add(new TransactionHistModel
        //                    {
        //                        PropItem = history.Rows[k]["PROPITEM"].ToString(),
        //                        Reading1 = history.Rows[k]["REGRESSIONIND"].ToString() == "Y" ? FinalR1.ToString() : history.Rows[k]["READING1"].ToString(),
        //                        Reading2 = history.Rows[k]["REGRESSIONIND"].ToString() == "Y" ? FinalR2.ToString() : history.Rows[k]["READING2"].ToString(),
        //                        Reading3 = history.Rows[k]["REGRESSIONIND"].ToString() == "Y" ? FinalR3.ToString() : history.Rows[k]["READING3"].ToString(),
        //                        Reading4 = history.Rows[k]["REGRESSIONIND"].ToString() == "Y" ? FinalR4.ToString() : history.Rows[k]["READING4"].ToString(),
        //                        Reading5 = history.Rows[k]["REGRESSIONIND"].ToString() == "Y" ? FinalR5.ToString() : history.Rows[k]["READING5"].ToString(),
        //                        Reading6 = history.Rows[k]["REGRESSIONIND"].ToString() == "Y" ? FinalR6.ToString() : history.Rows[k]["READING6"].ToString(),
        //                        Average = history.Rows[k]["AVERAGE"].ToString(),
        //                        Grade = history.Rows[k]["GRADE"].ToString(),
        //                        TestedDateTime = history.Rows[k]["TESTEDDATETIME"].ToString(),
        //                        Properties = history.Rows[k]["PROPERTIES"].ToString(),
        //                        id_ids_d = history.Rows[k]["ID_IDS_D"].ToString(),
        //                        Rresult = history.Rows[k]["AVGRESULT"].ToString(),

        //                    });
        //                }
        //            }

        //        }
        //    }

        //    return Json(list);
        //}

        //public async Task<JsonResult> SaveSubSegregationL(List<SubSegregationSegLModel> seglist)
        //{
        //    var vm = "";
        //    var prodtype = TempData["prodtypeSub"].ToString();
        //    var propitem = TempData["propitemSub"].ToString();
        //    var id_ids_d = TempData["id_ids_d"].ToString();
        //    var pype = TempData["pype"].ToString();

        //    foreach (var item in seglist)
        //    {
        //        var words = item.tagno.Split('-');
        //        var tagfrom = words[0].ToString();
        //        var tagto = words[1].ToString();
        //        DataTable temp;
        //        var paction = "";
        //        var grade = "";

        //        var gradeInd = Convert.ToInt32(item.gradeInd) + 1;

        //        if (item.reading == "" || item.reading == "0")
        //        {
        //            item.grade = "A";
        //        }
        //        else
        //        {
        //            temp = await dbdal.DataChk(prodtype, item.reading, "1", propitem);
        //            item.grade = temp.Rows[0]["GRADE"].ToString().Remove(0, 2);
        //        }


        //        string loc = HttpContext.Connection.RemoteIpAddress?.ToString();
        //        var aclUser = HttpContext.Session.GetObject<ACL_UserObj>("AclUser");

        //        var created_by = aclUser.USER_ID;
        //        var location = loc;

        //        var _tempD = await dbdal.IDS_Seg_Maint("0", id_ids_d, tagfrom, tagto, item.reading, item.grade, gradeInd.ToString(), "3", pype, created_by, location);

        //        vm = "ok";
        //    }

        //    return Json(vm);
        //}

        //private async Task<List<SelectListItem>> LoadDllData(string ID)
        //{
        //    List<SelectListItem> listItems = new List<SelectListItem>();

        //    DataTable dt = await dbdal.GetDataDLLALL("DDLOWGrade", "", "", "", "", "", "");
        //    if (dt != null && dt.Rows.Count > 0)
        //    {
        //        for (int i = 0; i < dt.Rows.Count; i++)
        //        {
        //            SelectListItem infoObj = new SelectListItem();
        //            infoObj.Text = dt.Rows[i]["NAME_TEXT"].ToString();
        //            infoObj.Value = dt.Rows[i]["ID_VALUE"].ToString();

        //            listItems.Add(infoObj);
        //        }
        //    }
        //    return listItems;
        //}

        //#endregion

        //#region DELETE

        //[HttpPost]
        //[SessionExpire]
        //public async Task<ActionResult> deleteIDSTrans(List<string> lstid)
        //{
        //    bool success = true;
        //    string message = "";

        //    if (lstid.Count < 1)
        //    {
        //        success = false;
        //        message += "Please select a record first to delete. ";
        //    }

        //    if (success && lstid.Count > 0)
        //    {
        //        foreach (var item in lstid)
        //        {
        //            int idh = Convert.ToInt32(item);

        //            //delete h
        //            string result1 = await dbdal.deleteIDSTrans(idh);
        //            if (!(int.TryParse(result1, out int num1) && result1 != "0"))
        //            {
        //                success = false;
        //                message += result1;
        //            }

        //        }
        //    }

        //    var data = new { success = success, message = message };

        //    return Json(data);
        //}

        //#endregion

        //#region AUDIT

        //[SessionExpire]
        //public async Task<ActionResult> DE_IDS_TRANS_REV_AUDIT(string id)
        //{
        //    TempData["SQ_ID"] = id;
        //    CommonFunction common = new CommonFunction();
        //    var param = new { pTable = "IDS_H_A", pKeyValue = id };
        //    List<AuditTrailModel> AuditTrailModel = await common.PSP_COMMON_DAPPER<AuditTrailModel>("PSP_COMMON_AUDIT", CommandType.StoredProcedure, param) ?? new List<AuditTrailModel>();

        //    return View(AuditTrailModel);
        //}

        #endregion

    }
    public class DEIDSTREVCM
    {
    }
}