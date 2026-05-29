using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TPM_QAS.Models
{
    public class ProdGroupModel
    {
        public int MM_PRODGROUP_H_ID { get; set; }
        public string PROD_GROUP { get; set; }
        public string PROD_TYPE { get; set; }
        public string RECORD_TYP { get; set; }
        public string REC_TYPE_DESC { get; set; }
        public string SEARCH_VALUE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }

        public List<ProdTypeModel> ProdTypeModel { get; set; }
        //public List<ProdTypeProdItemLstModel> ProdTypeProdItemLstModel { get; set; }

        //public List<PropertiesModel> PropertiesModel { get; set; }
        //public List<ProdTypePropTypeLstModel> ProdTypePropTypeLstModel { get; set; }
    }
}