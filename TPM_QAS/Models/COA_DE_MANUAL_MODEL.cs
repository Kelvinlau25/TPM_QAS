using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TPM_QAS.Models
{
    public class COAManualModel
    {
        public int DE_OCR_H_ID { get; set; }
        public string ENTRY_TYPE { get; set; }
        public string OCR_INDICATOR { get; set; }
        public string TPM_CODE { get; set; }
        public string ALL_LOT_NO { get; set; }
        public string FINAL_RESULT { get; set; }
        public string INSPECTION_DATE { get; set; }
        public string FILE_NAME { get; set; }
        public string VIEW_FILE { get; set; }
        public HttpPostedFileBase DataUploadFile { get; set; }
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

        public List<SelectListItem> DropdownSupplier { get; set; }
        public List<SelectListItem> DropdownMaterial { get; set; }
        public List<MMSpecLstModel> MMSpecLstModel { get; set; }

    }
    public class MMSpecLstModel
    {
        public int MM_MATERIAL_D2_ID { get; set; }
        public string SUPPLIER_SPEC_NAME { get; set; }
        public string SUPPLIER_SPEC_VALUE { get; set; }
        public string SUPPLIER_SPEC_TYPE { get; set; }
        public string OCR_RESULT { get; set; }
        public string COMPARISON_RESULT { get; set; }
        public string TPM_CODE { get; set; }
    }
}