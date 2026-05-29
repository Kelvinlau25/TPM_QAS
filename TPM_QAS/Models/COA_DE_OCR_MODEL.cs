using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TPM_QAS.Models
{
    public class COAOCRModel
    {
        public int DE_OCR_H_ID { get; set; }
        public string ENTRY_TYPE { get; set; }
        public string OCR_INDICATOR { get; set; }
        public string TPM_CODE { get; set; }
        public string ALL_LOT_NO { get; set; }
        public string FINAL_RESULT { get; set; }
        public string INSPECTION_DATE { get; set; }
        public string FILE_NAME { get; set; }
        public string FINAL_FILE_NAME { get; set; }
        public string VIEW_FILE { get; set; }
        public HttpPostedFileBase DataUploadFile { get; set; }
        public string SUPPLIER_LANG { get; set; }

        public int MM_SUPPLIER_H_ID { get; set; }
        public int MM_MATERIAL_H_ID { get; set; }
        public string LOT_NO { get; set; }

        public string RECORD_TYP { get; set; }
        public string REC_TYPE_DESC { get; set; }
        public string SEARCH_VALUE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
        public string ocrindicator { get; set; }
        public List<OCRLstModel> OCRLstModel { get; set; }
        public List<FirstOCRLstModel> FirstOCRLstModel { get; set; }
        public List<List<MMOCRData>> MMOCRData { get; set; }
        public List<MMOCRData> MMOCRDataLst { get; set; }
        public List<List<MMSpecLstModel>> MMSpecLstModel { get; set; }
        public List<HeaderMMOCRData> HeaderMMOCRData { get; set; }
        public Dictionary<Tuple<string, int>, List<OCRLstModel>> OCRLstModelDetail { get; set; }
        public List<SelectListItem> DropdownSupplier { get; set; }
        public List<SelectListItem> DropdownMaterial { get; set; }

    }

    public class OCRLstModel
    {
        public int DE_OCR_D_ID { get; set; }
        public int DE_OCR_H_ID { get; set; }
        public int MM_MATERIAL_H_ID { get; set; }
        public int MM_MATERIAL_D2_ID { get; set; }
        public string MATERIAL_NAME { get; set; }
        public int MM_SUPPLIER_H_ID { get; set; }
        public string SUPPLIER_NAME { get; set; }
        public string FILE_NAME { get; set; }
        public string LOT_NO { get; set; }
        public string TPM_CODE { get; set; }
        public string SUPPLIER_SPEC_TYPE { get; set; }
        public string SUPPLIER_MATERIAL_NAME { get; set; }
        public string SUPPLIER_SPEC_NAME { get; set; }
        public string SUPPLIER_SPEC_VALUE { get; set; }
        public string OCR_INSPECTION_ITEM { get; set; }
        public string OCR_INSPECTION_ITEM_SCORE { get; set; }
        public string OCR_RESULT { get; set; }
        public string OCR_RESULT_SCORE { get; set; }
        public string COMPARISON_RESULT { get; set; }

        public string RECORD_TYP { get; set; }
        public string REC_TYPE_DESC { get; set; }
        public string SEARCH_VALUE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
    }

    public class AppConfig
    {
        public string ProxyAddress { get; set; }
        public string ProxyEmail { get; set; }
        public string ProxyPassword { get; set; }
        public string OCREndpoint { get; set; }
        public string TMSUserAPIKey { get; set; }

    }

    public class OcrResult
    {
        public List<key_info> key_info { get; set; }
    }

    public class key_info
    {
        public List<string> material_name { get; set; }
        public List<string> supplier_name_not_toray { get; set; }
        public List<string> lot_no { get; set; }
    }

    public class Indicator3lot
    {
        public string type  { get; set; }
        public string lot { get; set; }
        public string path { get; set; }

    }
    public class KeyInfoData
    {
        public string property { get; set; }
        public string cfs_property { get; set; }
        public string value { get; set; }
        public string cfs_value { get; set; }
        public string lot_no { get; set; }
        public string cfs_lot_no { get; set; }
    }

    public class MMOCRData
    {
        public int MATERIAL_ID { get; set; }
        public int MM_MATERIAL_D2_ID { get; set; }
        public int supplier_id { get; set; }
        public string mm_spec_type { get; set; }
        public string mat_name { get; set; }
        public string mm_spec { get; set; }
        public string mm_spec_val { get; set; }
        public string ocr_spec { get; set; }
        public decimal cfs_ocr_spec { get; set; }
        public string ocr_spec_value { get; set; }
        public decimal cfs_ocr_spec_value { get; set; }
        public string lot_no { get; set; }
        public decimal cfs_lot_no { get; set; }
        public string compare_result { get; set; }
        public string final_name { get; set; }
        public string RECORD_TYP { get; set; }
    }

    public class HeaderMMOCRData
    {
        public int supplier_id { get; set; }
        public string supplier_name { get; set; }
        public int material_id { get; set; }
        public string material_name { get; set; }
        public string tpm_code { get; set; }
        public string lot_no { get; set; }
        public string is_lot_exist { get; set; }
        public string has_rejectedID { get; set; }
        public string resultOCR { get; set; }
        public string tabname { get; set; }
        public string path { get; set; }

    }
    public class FirstOCRLstModel
    {
        public string material_id { get; set; }
        public string material_name { get; set; }
        public int supplier_id { get; set; }
        public string supplier_name { get; set; }
        public string lot_no { get; set; }
        public string record_typ { get; set; }
    }
    public class ProcessPageResult
    {
        public byte[] PdfPage { get; set; }
        public string isExist { get; set; }
        public string rejectedID { get; set; }
        public string resultfromOCR { get; set; }
        public string apprejlot { get; set; }
        public List<Indicator3lot> Indicator3lot { get; set; }
        public List<KeyInfoData> KeyInfoData { get; set; }
    }

    public class SupplierTab
    {
        public int MM_SUPPLIER_H_ID { get; set; }
        public string SUPPLIER_NAME { get; set; }
        public string LOT_NO { get; set; }
        public string TABNAME { get; set; }

    }


}