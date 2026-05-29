using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TPM_QAS.Models
{
    public class PropertiesModel
    {
        public int MM_PROPERTIES_H_ID { get; set; }
        public string PROPERTIES { get; set; }
        public string PROP_ITEM { get; set; }
        public string REF_METHOD { get; set; }
        public string REMARKS { get; set; }
        public string MACHINE_NAME { get; set; }
        public string RECORD_TYP { get; set; }
        public string REC_TYPE_DESC { get; set; }
        public string SEARCH_VALUE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
        public int SEQUENCE { get; set; }

        public string DROPDOWN { get; set; }

        public List<PropertiesMachineLstModel> PropertiesMachineLstModel { get; set; }
        public List<SelectListItem> DropdownColumn { get; set; }

        public List<CSVFields> CSVFields { get; set; }

        public List<TPMSeqLstModel> TPMSeqLstModel { get; set; }
        public List<CompSeqLstModel> CompSeqLstModel { get; set; }

        public string IS_EXIST { get; set; }        

        public int ID_MM_PRODTYPE_D { get; set; }

        public List<SelectListItem> DropdownOraColumn { get; set; }
        public string OraColumn { get; set; }



    }
    public class PropertiesMachineLstModel
    {
        public int MM_PROPERTIES_D_ID { get; set; }
        public int MM_PROPERTIES_H_ID { get; set; }
        public string MACHINE_NAME { get; set; }
        public string MACHINE_PATH { get; set; }
        public string TESTING_TIME { get; set; }
        public string LOT_NO { get; set; }
        public string OPERATOR { get; set; }
        public string READING_1 { get; set; }
        public string READING_2 { get; set; }
        public string READING_3 { get; set; }
        public string READING_4 { get; set; }
        public string READING_5 { get; set; }
        public string READING_6 { get; set; }
        public decimal TTL_READING { get; set; }
        public string AVERAGE { get; set; }
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

    public class CSVFields
    {
        public string COLUMNS { get; set; }
        public string VALUE { get; set; }
        
    }

    public class TPMSeqLstModel
    {
        public int MM_PROPERTIES_H_ID { get; set; }
        public string PROPERTIES { get; set; }
        public string PROP_ITEM { get; set; }
        public int TPM_SEQUENCE { get; set; }
        public string RECORD_TYP { get; set; }
    }

    public class CompSeqLstModel
    {
        public int MM_PROPERTIES_H_ID { get; set; }
        public string PROPERTIES { get; set; }
        public string PROP_ITEM { get; set; }
        public int COMP_SEQUENCE { get; set; }
        public string RECORD_TYP { get; set; }
    }
}