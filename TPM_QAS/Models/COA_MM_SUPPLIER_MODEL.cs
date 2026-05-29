using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TPM_QAS.Models
{
    public class COASupplierModel
    {
        public int MM_SUPPLIER_H_ID { get; set; }
        public string SUPPLIER_NAME { get; set; }
        public string TPM_CODE { get; set; }
        public string RECORD_TYP { get; set; }
        public string REC_TYPE_DESC { get; set; }
        public string SEARCH_VALUE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
        public List<SupplierLstModel> SupplierLstModel { get; set; }

    }

    public class SupplierLstModel
    {
        public int MM_SUPPLIER_D_ID { get; set; }
        public int MM_SUPPLIER_H_ID { get; set; }
        public string KEY_TYPE { get; set; }
        public string SUPPLIER_KEY { get; set; }
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
}