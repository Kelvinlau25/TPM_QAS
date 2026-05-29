using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TPM_QAS.Models
{
    public class COAMMMaterialModel
    {
        public int MM_MATERIAL_H_ID { get; set; }
        public string MATERIAL_NAME { get; set; }
        public string MATERIAL_ABBR { get; set; }
        public string MATERIAL_CODE { get; set; }
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

        //public List<SelectListItem> DropdownProdGroup { get; set; }
        //public List<SelectListItem> DropdownPattern { get; set; }

        //public List<CheckPlanPropLstModel> CheckPlanPropLstModel { get; set; }
        public List<OraMaterialList> OraMaterialList { get; set; }
        public List<TPMMatSpecList> TPMMatSpecList { get; set; }
        public List<SUPPMatSpecList> SUPPMatSpecList { get; set; }
    }

    public class OraMaterialList
    {
        public string MATERIAL_NAME { get; set; }
        public string MATERIAL_ABBR { get; set; }
        public string MATERIAL_CODE { get; set; }
        public string TPM_CODE { get; set; }        
    }

    public class TPMMatSpecList
    {
        public int MM_MATERIAL_H_ID { get; set; }
        public int MM_MATERIAL_D1_ID { get; set; }
        public string MATERIAL_NAME { get; set; }
        public string TPM_SPEC_NAME { get; set; }
        public string TPM_SPEC_VALUE { get; set; }
        public string TPM_SPEC_TYPE { get; set; }
        public string TPM_SPEC_UOM { get; set; }
        public string TPM_SPEC_UOM_ID { get; set; }
    }

    public class SUPPMatSpecList
    {
        public int MM_MATERIAL_H_ID { get; set; }
        public int MM_MATERIAL_D1_ID { get; set; }
        public int MM_MATERIAL_D2_ID { get; set; }
        public int MM_SUPPLIER_H_ID { get; set; }
        public string SUPPLIER_NAME { get; set; }
        public string SUPP_MATERIAL_NAME { get; set; }
        public string COUNT_SUPPLIER { get; set; }
        
        public string TPM_SPEC_NAME { get; set; }
        public string SUPP_SPEC_NAME { get; set; }
        public string SUPP_SPEC_VALUE { get; set; }
        public string SUPP_SPEC_TYPE { get; set; }
        public string SUPP_SPEC_UOM { get; set; }
    }


    //public class CheckPlanPropLstModel
    //{
    //    public int MM_CHECKPLAN_D_ID { get; set; }
    //    public int MM_CHECKPLAN_H_ID { get; set; }
    //    public int MM_PROPERTIES_H_ID { get; set; }
    //    public string PROPERTIES { get; set; }
    //    public string PROP_ITEM { get; set; }
    //    public string RECORD_TYP { get; set; }
    //    public string REC_TYPE_DESC { get; set; }
    //    public string SEARCH_VALUE { get; set; }
    //    public string CREATED_BY { get; set; }
    //    public string CREATED_DATE { get; set; }
    //    public string CREATED_LOC { get; set; }
    //    public string UPDATED_BY { get; set; }
    //    public string UPDATED_DATE { get; set; }
    //    public string UPDATED_LOC { get; set; }
    //}
}