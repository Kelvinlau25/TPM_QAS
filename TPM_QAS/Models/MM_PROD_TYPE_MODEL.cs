using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TPM_QAS.Models
{

    public class ProdTypeModel
    {
        public int MM_PRODTYPE_H_ID { get; set; }
        public string PROD_TYPE { get; set; }
        public string COMP_GROUP { get; set; }
        public string PROPERTIES { get; set; }
        public string PRODITEM { get; set; }
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
        public int MM_PRODGROUP_D_ID { get; set; }
        public int MM_PRODGROUP_H_ID { get; set; }

        public List<ProdTypeMainPropLstModel> ProdTypeMainPropLstModel { get; set; }
        public List<ProdTypeProdItemLstModel> ProdTypeProdItemLstModel { get; set; }
        public List<ProdTypeDuplLstModel> ProdTypeDuplLstModel { get; set; }

        public List<PropertiesModel> PropertiesModel { get; set; }
        public List<ProdTypePropTypeLstModel> ProdTypePropTypeLstModel { get; set; }
    }

    public class ProdTypeMainPropLstModel
    {
        public int MM_PRODTYPE_D_ID { get; set; }
        public int MM_PRODTYPE_H_ID { get; set; }
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

    public class ProdTypeProdItemLstModel
    {
        public int MM_PRODTYPE_D1_ID { get; set; }
        public int MM_PRODTYPE_H_ID { get; set; }
        public string PROP_TYPE_MAIN { get; set; }
        public string PROP_TYPE_SUB { get; set; }
        public string PROP_CLASS { get; set; }
        public string PROP_COLOUR { get; set; }
        public string PROP_PACK { get; set; }
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

    public class ProdTypePropTypeLstModel //PVIEW_TPM_FLOT_SPLT
    {
        public string PROPTYPEMAIN { get; set; }
        public string PROPTYPESUB { get; set; }
        public string PROPCLS { get; set; }
        public string PROPCOLOR { get; set; }
        public string PROPPACK { get; set; }
        public string RECORD_TYP { get; set; }
        public string IS_EXIST { get; set; }
    }

    public class ProdTypeDuplLstModel
    {
        public int MM_PRODTYPE_H_ID { get; set; }
        public string PROD_TYPE { get; set; }
        public string COMP_GROUP { get; set; }

    }
}