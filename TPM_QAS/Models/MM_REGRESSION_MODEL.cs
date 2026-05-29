using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TPM_QAS.Models
{
    public class RegressionModel
    {
        public int MM_REGRESSION_H_ID { get; set; }
        public string COMP_GROUP { get; set; }
        public string COMP_GROUP_DESC { get; set; }
        public string COMPPRODLINE { get; set; }
        public string COMPOUNDER { get; set; }
        public string PROPERTIES { get; set; }
        public string PROPERTIES_TPM { get; set; }
        public string PROD_LINE { get; set; }
        public string PROPERTIES_COMP { get; set; }
        public string PROP_ITEM { get; set; }
        public int PROP_ITEM_ID { get; set; }
        public string FORMULA { get; set; }
        public string REGRESSIND { get; set; }

        public string RECORD_TYP { get; set; }
        public string REC_TYPE_DESC { get; set; }
        public string SEARCH_VALUE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
        public List<SelectListItem> DropdownProperty { get; set; }
        public List<SelectListItem> DropdownPropItem { get; set; }
        public List<SelectListItem> DropdownCompounder { get; set; }

        public List<RegressionLstModel> RegressionLstModel { get; set; }

    }

    public class RegressionLstModel
    {
        public int MM_REGRESSION_D_ID { get; set; }
        public int MM_REGRESSION_H_ID { get; set; }
        public string PROD_GROUP_ID_STRING { get; set; }
        public int MM_PRODGROUP_H_ID_TPM { get; set; }
        public string MM_PRODGROUP_H_ID_TPM_STRING { get; set; }
        public string PROD_GROUP_TPM { get; set; }
        public int MM_PRODGROUP_H_ID_COMP { get; set; }
        public string MM_PRODGROUP_H_ID_COMP_STRING { get; set; }
        public string PROD_GROUP_COMP { get; set; }
        public int MACHINE_NAME_ID { get; set; }
        public string MACHINE_NAME { get; set; }
        public string FORMULA_TPM { get; set; }
        public string FORMULA_COMP { get; set; }
        public string REGRESSIND { get; set; }

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