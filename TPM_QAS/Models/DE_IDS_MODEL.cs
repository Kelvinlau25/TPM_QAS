using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TPM_QAS.Models
{
    public class W_GRADE_PL
    {
        public List<W_GRADE_PL_DETAIL> ListW_GRADE_PL_DETAIL { get; set; }
    }
    public class W_GRADE_PL_DETAIL
    {
        public string ITEM_CODE { get; set; }
        public string ITEM_DESC { get; set; }
        public string LOT_NUMBER { get; set; }
        public string PACK_QTY { get; set; }
        public string PACKED_DATE { get; set; }

        public string ORGANIZATION { get; set; }
        public string UOM { get; set; }
        public string last { get; set; }

        public DateTime PACKING_DATE { get; set; }
    }


    public class REPORT_IDS_VM
    {
        public string DATE_FROM { get; set; }
        public string DATE_TO { get; set; }
        public string PROD_TYPE { get; set; }
        public string PROD_LINE_FROM { get; set; }
        public string PROD_LINE_TO { get; set; }
        public bool INC_EXTERNAL { get; set; }

        public string xaxis { get; set; }
        public string properties { get; set; }
        public string listproperties { get; set; }
        public int MM_PROPERTIES_H_ID { get; set; }

        public List<string> SELECTED_PROP { get; set; }
        public List<SelectListItem> DropdownPropItem { get; set; }
        public List<SelectListItem> DropdownProdType { get; set; }
    }

    public class DE_IDS_REPORT
	{
        public string IDS_GROUP { get; set; }
        public string PACKING_DATE { get; set; }
        public string PROD_LINE_NAME { get; set; }
        public string PROD_LINE { get; set; }
        public string PROD_TYPE { get; set; }
        public string LOT_NO { get; set; }
        public string QTY_MT { get; set; }
        public string GRADE { get; set; }

        public string PROP_YI_PELLET { get; set; }
        public string PROP_YI_PLATE { get; set; }
        public string PROP_MFR { get; set; }
        public string PROP_CHARPY { get; set; }
        public string PROP_DTL { get; set; }
        public string PROP_VST { get; set; }
        public string PROP_TMODULUS { get; set; }
        public string PROP_HAZE { get; set; }
        public string PROP_FISHEYE_FILM { get; set; }
        public string PROP_FISHEYE_PLATE { get; set; }
        public string PROP_TWINS { get; set; }
        public string PROP_POWDER { get; set; }
        public string PROP_DENSITY { get; set; }
        public string PROP_GLASS { get; set; }

        public string PCL_RESULT { get; set; }
        public string REMARKS { get; set; }
    }


    public class DE_IDS_ReturnModel
    {
        public string Status { get; set; }
        public int ID { get; set; }
        public string Message { get; set; }

    }

}