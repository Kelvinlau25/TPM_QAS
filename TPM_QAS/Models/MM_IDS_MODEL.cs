using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TPM_QAS.Models
{
    public class IDSModel
    {
        public string IDH { get; set; }
        public int IDH_NUMERIC { get; set; }
        public string PROD_GROUP { get; set; }
        public string PROPERTIES { get; set; }
        public string PROP_ITEM { get; set; }
        public string SAMPLING_TYPE { get; set; }
        public string COMP_GROUP { get; set; }
        public string SAMPLING_TYPE_DESC { get; set; }
        public string UNIT { get; set; }
        public string INDICATOR { get; set; }
        public string PROD_LINE { get; set; }
        public int MM_IDS_MAIN_LST_ID { get; set; }

        public string RECORD_TYP { get; set; }
        public string REC_TYPE_DESC { get; set; }
        public string SEARCH_VALUE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }

        //Mix SAMPLE
        public IDSMixModel IDSMixModel { get; set; }

        public List<SelectListItem> DropdownProdGroup { get; set; }
        public List<SelectListItem> DropdownProperty { get; set; }
        public List<SelectListItem> DropdownPropItem { get; set; }
        public List<SelectListItem> DropdownProdLine { get; set; }
        public List<SelectListItem> DropdownUnit { get; set; }
        public List<SelectListItem> DropdownIndicator { get; set; }

        public List<IDSMachinePopupModel> IDSMachinePopupModel { get; set; }

        // Direct Sample
        public IDSDirectModel IDSDirectModel { get; set; }
        public List<SelectListItem> DropdownSectionName { get; set; }
        public List<IDSPropertiesPopupModel> IDSPropertiesPopupModel { get; set; }

    }

    public class IDSMixModel // MM_IDS_H
    {
        public int MM_IDS_H_ID { get; set; }
        public string SAMPLING_TYPE { get; set; }
        public string COMP_GROUP { get; set; }
        public int MM_PRODGROUP_ID_H { get; set; }
        public string PROD_GROUP { get; set; }
        public int MM_PROPERTIES_H_ID { get; set; }
        public string PROPERTIES { get; set; }
        public string PROP_ITEM { get; set; }
        public string PROD_LINE { get; set; }
        public string UNIT { get; set; } //MPa
        public string INDICATOR { get; set; } //Calculation
        public string SHRINK_DOWN { get; set; }
        public string READING_TYPE { get; set; }
        public int READING_VALUE { get; set; } 
        public int REPEAT_VALUE { get; set; }
        public decimal HORIZONTAL { get; set; }
        public decimal VERTICAL { get; set; }
        public string SEGREGATION { get; set; } //1
        public string SEGREGATION_DESC { get; set; } //No
        public int BEFORESEGSET { get; set; }
        public int AFTERSEGSET { get; set; }
        public string COQ { get; set; } //1
        public string COQ_DESC { get; set; } //No
        public int AFTERCOQSET { get; set; }

        public string RECORD_TYP { get; set; }
        public string REC_TYPE_DESC { get; set; }
        public string SEARCH_VALUE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }

        public List<IDSMixLstModel> IDSMixLstModel { get; set; }

    }

    public class IDSMixLstModel
    {
        public int MM_IDS_D_ID { get; set; }
        public int MM_IDS_H_ID { get; set; }
        public int MACHINE_ID { get; set; }
        public string MACHINE_NAME { get; set; }
        public string MACHINE_PATH { get; set; }

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

    public class IDSDirectModel // MM_IDSSECTION_H
    {
        public int MM_IDSSECTION_H_ID { get; set; }
        public string SAMPLING_TYPE { get; set; }
        public string COMP_GROUP { get; set; }
        public string SECTION { get; set; } //BLACK SPECK
        public string FIELD_NAME { get; set; }

        public string RECORD_TYP { get; set; }
        public string REC_TYPE_DESC { get; set; }
        public string SEARCH_VALUE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }

        public List<IDSDirectLstModel> IDSDirectLstModel { get; set; }

    }

    public class IDSDirectLstModel
    {
        public int MM_IDSSECTION_D_ID { get; set; }
        public int MM_IDSSECTION_H_ID { get; set; }
        public string FIELD_NAME { get; set; }
        public string PROPERTIES { get; set; }
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

    public class IDSMachinePopupModel
    {
        public int MACHINE_ID { get; set; }
        public string MACHINE_NAME { get; set; }
        public string MACHINE_FIELD { get; set; }
        public string PROPERTIES { get; set; }
        public string PROP_ITEM { get; set; }
        public string IS_EXIST { get; set; }

    }

    public class IDSPropertiesPopupModel
    {
        public string PROPERTIES { get; set; }
    }


}