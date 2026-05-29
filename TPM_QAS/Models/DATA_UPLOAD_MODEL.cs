using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TPM_QAS.Models
{

    public class DataUploadModel
    {
        public List<SupplierList> SupplierList { get; set; }
        public List<MaterialList> MaterialList { get; set; }
        public List<OraMaterialList> OraMaterialList2 { get; set; }
    }

    public class OraMaterialList2
    {
        public string MATERIAL_NAME { get; set; }
        public string MATERIAL_ABBR { get; set; }
        public string MATERIAL_CODE { get; set; }
        public string TPM_CODE { get; set; }
    }
    public class SupplierList
    {
        public int ID { get; set; }
        public string SUPPLIER_NAME { get; set; }
        public string FILENAME { get; set; }
    }

    public class MaterialList
    {
        public int EXCEL_ID { get; set; }
        public string EXCEL_MATERIAL_NAME { get; set; }
        public string EXCEL_MATERIAL_ABBR { get; set; }
        public string FILENAME { get; set; }
        public string ORA_MATERIAL_NAME { get; set; }
        public string ORA_MATERIAL_ABBR { get; set; }
        public string ORA_MATERIAL_CODE { get; set; }
        public string ORA_TPM_CODE { get; set; }
    }

}