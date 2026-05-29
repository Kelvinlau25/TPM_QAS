using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TPM_QAS.Models
{
    public class CommonGroupModel
    {
        public int MM_COMMONGROUP_H_ID { get; set; }
        public string CATEGORY { get; set; }
        public string SUB_CATEGORY { get; set; }
        public string RECORD_TYP { get; set; }
        public string REC_TYPE_DESC { get; set; }
        public string SEARCH_VALUE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
        public List<CommonGroupLstModel> CommonGroupLstModel { get; set; }
        public List<PropertyColorLstModel> PropertyColorLstModel { get; set; }

    }

    public class CommonGroupLstModel
    {
        public int MM_COMMONGROUP_D_ID { get; set; }
        public int MM_COMMONGROUP_H_ID { get; set; }
        public string SUB_CATEGORY { get; set; }
        public string ITEMS { get; set; }
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

    public class PropertyColorLstModel
    {
        public string PROPCOLOR { get; set; }
        
    }
}