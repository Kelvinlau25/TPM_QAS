using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TPM_QAS.Models
{
    public class CheckRoutineModel
    {
        public int MM_CHECKROUTINE_H_ID { get; set; }
        public int MM_PRODGROUP_H_ID { get; set; }
        public string PROD_GROUP { get; set; }
        public string PATTERN { get; set; }
        public string PATTERN_DESC { get; set; }
        public string FULL_PRODLINE { get; set; }
        public string FULL_PRODLINENO { get; set; }
        public string WEEKORDAY { get; set; }
        public string WHOLEWEEK { get; set; }
        public string MONDAY { get; set; }
        public string TUESDAY { get; set; }
        public string WEDNESDAY { get; set; }
        public string THURSDAY { get; set; }
        public string FRIDAY { get; set; }
        public string SATURDAY { get; set; }
        public string SUNDAY { get; set; }
        public string PRODLINE_NO { get; set; }
        public string PRODLINE { get; set; }
        public string SEQUENCE { get; set; }
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

        public List<CheckRoutineProdLstModel> CheckRoutineProdLstModel { get; set; }
        public List<ProductLineModel> ProductLineModel { get; set; }
    }

    public class CheckRoutineProdLstModel
    {
        public int MM_CHECKROUTINE_D_ID { get; set; }
        public int MM_CHECKROUTINE_H_ID { get; set; }
        public int PRODLINENO_ID { get; set; }
        public string PRODLINE { get; set; }
        public string PRODLINENO { get; set; }
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

    public class ProductLineModel
    {
        public int PRODLINENO_ID { get; set; }
        public string PRODLINE { get; set; }
        public string PRODLINENO { get; set; }
        public string RECORD_TYP { get; set; }
        public string REC_TYPE_DESC { get; set; }
        public string SEARCH_VALUE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
        public string IS_EXIST { get; set; }
    }

}