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
using Newtonsoft.Json;

namespace TPM_QAS.Controllers
{
    public class COA_GA_RAWMAT_ABNORMController : Controller
    {
        DB dbmain = new DB();
        COA_GA_RAWMAT_ABNORM_DAL dbdal = new COA_GA_RAWMAT_ABNORM_DAL();

        #region LIST

        [SessionExpire]
        public async Task<ActionResult> COA_GA_RAWMAT_ABNORM_LST(string Deleted = "0")
        {
            TempData["STATUS_DDL"] = await dbmain.PSP_COMMON_DDL("CMN_LST_STATUS", "", "", "", "", "", "");

            CommonFunction common = new CommonFunction();
            var param = new { Table = "PVIEW_GA_RAWMAT_ABNORM", TableID = "", Search = "", Value = "", SortField = "GA_ABNORMAL_H_ID", Direction = "0", FrmRowno = "1", ToRowno = "500000", Deleted = Deleted };
            List<COAGARAWMATABNORMModel> model = await common.PSP_COMMON_DAPPER<COAGARAWMATABNORMModel>("PSP_COMMON_LIST", CommandType.StoredProcedure, param) ?? new List<COAGARAWMATABNORMModel>();

            ViewBag.Deleted = Deleted;
            return View(model);
        }

        #endregion

        [SessionExpire]
        public async Task<ActionResult> COA_GA_RAWMAT_ABNORM_DET(string id)
        {
            ViewBag.Tittle = "Abnormality Detection Graph";
            var model = new COAGARAWMATABNORMModel();
            model.GA_ABNORMAL_H_ID = Convert.ToInt32(id);
            DataTable dtpie = await dbdal.getdataforabnorm(id, "", "");
            string UOM = "";

            string dataitemOcrResult = "[";

            if (dtpie != null) {
                
                for (int i = 0; i < dtpie.Rows.Count; i++)
                {
                    if (dtpie.Rows[i]["TPM_SPEC_UOM"].ToString() == null || dtpie.Rows[i]["TPM_SPEC_UOM"].ToString() == "") 
                    {
                        UOM = dtpie.Rows[i]["TPM_SPEC_NAME"].ToString();
                    }
                    else
                    {
                        UOM = dtpie.Rows[i]["TPM_SPEC_NAME"].ToString() + " (" + dtpie.Rows[i]["TPM_SPEC_UOM"].ToString() + ")";
                    }
                    ViewBag.Tittle = "Abnormality Detection Graph - " + dtpie.Rows[i]["SUPPLIER_NAME"].ToString() + " - " + dtpie.Rows[i]["MATERIAL_NAME"].ToString() + " - " + dtpie.Rows[i]["TPM_SPEC_NAME"].ToString();
                    ViewBag.Itemspec = dtpie.Rows[i]["TPM_SPEC_NAME"].ToString();
                    //string label = Convert.ToDateTime(dtpie.Rows[i]["CREATED_DATE"]).ToString("dd/dd");
                    string label2 = "";
                    string classtxt = "";
                    string mcolor = "";
                    if (dtpie.Rows[i]["ABNORM_STAT"].ToString() != "NORMAL")
                    {
                        label2 = "Sudden " + dtpie.Rows[i]["ABNORM_STAT"].ToString() + " Detected on:";
                        classtxt = "text-danger";
                        mcolor = ", markerColor: 'red'";
                    }

                    string itemspec = dtpie.Rows[i]["TPM_SPEC_NAME"].ToString();
                    string Lotno = dtpie.Rows[i]["LOT_NO"].ToString();
                    string Datefull = Convert.ToDateTime(dtpie.Rows[i]["CREATED_DATE"]).ToString("dd/MM/yyyy");
                    string datex = (i + 1).ToString();//Convert.ToDateTime(dtpie.Rows[i]["CREATED_DATE"]).ToString("dd/MM");
                    string OCR_RESULT = Convert.ToDecimal(dtpie.Rows[i]["OCR_RESULT"]).ToString();
                    dataitemOcrResult = dataitemOcrResult + "{ x: " + datex + ", y: " + OCR_RESULT + ", label: '" + Lotno + "', label2: '" + label2 + "', itemspec: '" + itemspec + "',  LOTNO: '" + Lotno + "',  DATEFULL: '" + Datefull + "', classtxt: '" + classtxt + "'" + mcolor + "}, ";
                }
            }

            dataitemOcrResult = dataitemOcrResult.Substring(0, dataitemOcrResult.Length - 2);
            dataitemOcrResult = dataitemOcrResult + "]";

            ViewBag.OcrResulData = dataitemOcrResult;

            // pie chart 

            List<DataPoint> dataPoints = new List<DataPoint>();

            for (int i = 0; i < dtpie.Rows.Count; i++)
            {
                DataPoint CG = new DataPoint();
                //dataPoints.Add(new DataPoint(Convert.ToDateTime(dtpie.Rows[i]["CREATED_DATE"]).ToString("dd/MM"), Convert.ToDouble(Convert.ToDouble(dtpie.Rows[i]["OCR_RESULT"]).ToString("###,###,##0.0"))));
                dataPoints.Add(new DataPoint(dtpie.Rows[i]["LOT_NO"].ToString(), Convert.ToDouble(dtpie.Rows[i]["OCR_RESULT"])));

            }


            ViewBag.Yaxis = JsonConvert.SerializeObject(UOM);
            ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);

            return View(model);
        }

        [HttpPost]
        [SessionExpire]
        public async Task<ActionResult> COA_GA_RAWMAT_ABNORM_DET(COAGARAWMATABNORMModel model)
        {
            ViewBag.Tittle = "Abnormality Detection Graph";
            DataTable dtpie = await dbdal.getdataforabnorm(model.GA_ABNORMAL_H_ID.ToString(), model.DATE_FROM, model.DATE_TO);
            ViewBag.OcrResulData2 = "";
            string dataitemOcrResult = "[";
            string UOM = "";

            if (dtpie != null)
            {
                for (int i = 0; i < dtpie.Rows.Count; i++)
                {
                    if (dtpie.Rows[i]["TPM_SPEC_UOM"].ToString() == null || dtpie.Rows[i]["TPM_SPEC_UOM"].ToString() == "")
                    {
                        UOM = dtpie.Rows[i]["TPM_SPEC_NAME"].ToString();
                    }
                    else
                    {
                        UOM = dtpie.Rows[i]["TPM_SPEC_NAME"].ToString() + " (" + dtpie.Rows[i]["TPM_SPEC_UOM"].ToString() + ")";
                    }
                    ViewBag.Tittle = "Abnormality Detection Graph - " + dtpie.Rows[i]["SUPPLIER_NAME"].ToString() + " - " + dtpie.Rows[i]["MATERIAL_NAME"].ToString() + " - " + dtpie.Rows[i]["TPM_SPEC_NAME"].ToString();
                    ViewBag.Itemspec = dtpie.Rows[i]["TPM_SPEC_NAME"].ToString();
                    //string label = Convert.ToDateTime(dtpie.Rows[i]["CREATED_DATE"]).ToString("dd/dd");
                    string label2 = "";
                    string classtxt = "";
                    string mcolor = "";
                    if (dtpie.Rows[i]["ABNORM_STAT"].ToString() != "NORMAL")
                    {
                        label2 = "Sudden " + dtpie.Rows[i]["ABNORM_STAT"].ToString() + " Detected on:";
                        classtxt = "text-danger";
                        mcolor = ", \"markerColor\": \"red\"";
                    }

                    string itemspec = dtpie.Rows[i]["TPM_SPEC_NAME"].ToString();
                    string Lotno = dtpie.Rows[i]["LOT_NO"].ToString();
                    string Datefull = Convert.ToDateTime(dtpie.Rows[i]["CREATED_DATE"]).ToString("dd/MM/yyyy");
                    string datex = (i + 1).ToString();//Convert.ToDateTime(dtpie.Rows[i]["CREATED_DATE"]).ToString("dd/MM");
                    string OCR_RESULT = Convert.ToDecimal(dtpie.Rows[i]["OCR_RESULT"]).ToString();
                    dataitemOcrResult = dataitemOcrResult + "{ \"x\": \"" + datex + "\", \"y\": \"" + OCR_RESULT + "\", \"label\": \"" + Lotno + "\", \"label2\": \"" + label2 + "\", \"itemspec\": \"" + itemspec + "\",  \"LOTNO\": \"" + Lotno + "\",  \"DATEFULL\": \"" + Datefull + "\", \"classtxt\": \"" + classtxt + "\"" + mcolor + "}, ";
                }
            }

            dataitemOcrResult = dataitemOcrResult.Substring(0, dataitemOcrResult.Length - 2);
            dataitemOcrResult = dataitemOcrResult + "]";

            ViewBag.OcrResulData2 = dataitemOcrResult;

            // pie chart 
            chartinfo ci = new chartinfo();
            List<DataPoint> dataPoints = new List<DataPoint>();

            for (int i = 0; i < dtpie.Rows.Count; i++)
            {
                DataPoint CG = new DataPoint();
                //dataPoints.Add(new DataPoint(Convert.ToDateTime(dtpie.Rows[i]["CREATED_DATE"]).ToString("dd/MM"), Convert.ToDouble(Convert.ToDouble(dtpie.Rows[i]["OCR_RESULT"]).ToString("###,###,##0.0"))));
                dataPoints.Add(new DataPoint(dtpie.Rows[i]["LOT_NO"].ToString(),Convert.ToDouble(dtpie.Rows[i]["OCR_RESULT"])));

            }


            //string rdatapoint = JsonConvert.SerializeObject(dataPoints).ToString();
            string rocrresult = dataitemOcrResult;

            //ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);


            ci.DataPoint = dataPoints;
            ci.OcrResult = rocrresult;
            ci.Yaxis = UOM;

            string result = JsonConvert.SerializeObject(ci).ToString();
            return Json(result, JsonRequestBehavior.AllowGet);

            //return View(model);
        }

    }

}