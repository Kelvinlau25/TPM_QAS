using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TPM_QAS.Models
{
    public class GradingModel
    {
        public int MM_GRADING_H_ID { get; set; }
        public int MM_PRODTYPE_H_ID { get; set; }
        public string PROD_TYPE { get; set; }
        public string COMP_GROUP { get; set; }
        public string PROD_LINE { get; set; }
        public string COMP_GROUP_DESC { get; set; }

        public string GRADING_INDICATOR { get; set; }

        public string PROPERTIES { get; set; }
        public int MM_PROPERTIES_H_ID { get; set; }

        public string RECORD_TYP { get; set; }
        public string REC_TYPE_DESC { get; set; }
        public string SEARCH_VALUE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
        public List<GradingNormalLstModel> GradingNormalLstModel { get; set; }
        public List<GradingAppearanceLstModel> GradingAppearanceLstModel { get; set; }
        public List<GradingPassingLstModel> GradingPassingLstModel { get; set; }
        public List<GradingDuplLstModel> GradingDuplLstModel { get; set; }
        public List<SelectListItem> DropdownProdType { get; set; }

        public List<SelectListItem> DropdownProperty { get; set; }
        public List<SelectListItem> DropdownItem { get; set; }

    }

    public class GradingNormalLstModel
    {
        public int MM_GRADING_D1_ID { get; set; }
        public int MM_GRADING_H_ID { get; set; }
        public int SEQUENCE { get; set; }
        public string PROPERTY { get; set; }
        public int MM_PROPERTIES_H_ID { get; set; }
        public string PROP_ITEM { get; set; }
        public int PRIORITY { get; set; }
        public int FINAL_PRIORITY { get; set; }
        public decimal L_SPEC { get; set; }
        public decimal U_SPEC { get; set; }
        public decimal L_PCL { get; set; }
        public decimal U_PCL { get; set; }
        public decimal CENTRE_LINE { get; set; }
        public string GRADE { get; set; }
        public int ROUNDING { get; set; }

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

    public class GradingAppearanceLstModel
    {
        public int MM_GRADING_D2_ID { get; set; }
        public int MM_GRADING_H_ID { get; set; }
        public int SEQUENCE { get; set; }
        public string SECTION { get; set; }
        public string FIELD_NAME { get; set; }
        public int PRIORITY { get; set; }
        public int FINAL_PRIORITY { get; set; }
        public decimal L_SPEC { get; set; }
        public decimal U_SPEC { get; set; }
        public decimal L_PCL { get; set; }
        public decimal U_PCL { get; set; }
        public decimal CALCULATION { get; set; }
        public string GRADE { get; set; }
        public int ROUNDING { get; set; }

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

    public class GradingPassingLstModel
    {
        public int MM_GRADING_D3_ID { get; set; }
        public int MM_GRADING_H_ID { get; set; }
        public string PROPERTIES { get; set; }
        public string PROP_ITEM { get; set; }
        public string PASS_GRADE { get; set; }

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

    public class GradingDuplLstModel
    {
        public int MM_GRADING_H_ID { get; set; }
        public int MM_PRODTYPE_H_ID { get; set; }
        public string PROD_TYPE { get; set; }
        public string COMP_GROUP { get; set; }
        public string COMP_GROUP_DESC { get; set; }

        public string GRADING_INDICATOR { get; set; }

    }


}