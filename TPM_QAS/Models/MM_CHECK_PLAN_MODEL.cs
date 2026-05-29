using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TPM_QAS.Models
{
    public class CheckPlanModel
    {
        public int MM_CHECKPLAN_H_ID { get; set; }
        public int MM_PRODGROUP_H_ID { get; set; }
        public string PROD_GROUP { get; set; }
        public string PATTERN_TYPE { get; set; }
        public string PATTERN_DESC { get; set; }
        public string COMP_GROUP { get; set; }
        public string COMPANY_DESC { get; set; }
        public string PROPERTIES { get; set; }
        public string PROP_ITEM { get; set; }
        public string RECORD_TYP { get; set; }
        public string REC_TYPE_DESC { get; set; }
        public string SEARCH_VALUE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }

        public List<SelectListItem> DropdownProdGroup { get; set; }
        public List<SelectListItem> DropdownPattern { get; set; }

        public List<CheckPlanPropLstModel> CheckPlanPropLstModel { get; set; }
        public List<PropertiesModel> PropertiesModel { get; set; }
    }

    public class CheckPlanPropLstModel
    {
        public int MM_CHECKPLAN_D_ID { get; set; }
        public int MM_CHECKPLAN_H_ID { get; set; }
        public int MM_PROPERTIES_H_ID { get; set; }
        public string PROPERTIES { get; set; }
        public string PROP_ITEM { get; set; }
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