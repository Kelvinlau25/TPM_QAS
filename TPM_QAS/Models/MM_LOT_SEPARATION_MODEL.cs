using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TPM_QAS.Models
{
    public class LotSeparationModel
    {
        public int MM_LOTNO_H_ID { get; set; }
        public string SECTION_NAME { get; set; }
        public List<String> PRODLINENO_ID { get; set; }
        public string PRODLINE_ID_STRING { get; set; }
        public string PRODLINE { get; set; }
        public string PRODLINENO { get; set; }
        public int PACKINGTYPE_ID { get; set; }
        public string PACKINGTYPE { get; set; }
        public int CHECK_LAST_TAG { get; set; }
        public string CHECK_LAST_TAG_DESC { get; set; }
        public int MIN_INTERVAL { get; set; }
        public int MAX_INTERVAL { get; set; }
        public int FIRSTTAG { get; set; }
        public int INTERVAL { get; set; }
        public string LOTNO { get; set; }
        public string RECORD_TYP { get; set; }
        public string REC_TYPE_DESC { get; set; }
        public string SEARCH_VALUE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
        public List<SelectListItem> DropdownSection { get; set; }
        public List<SelectListItem> DropdownProdLine { get; set; }
        public List<SelectListItem> DropdownProdLineNo { get; set; }
        public List<SelectListItem> DropdownPackType { get; set; }

        public List<IntervalLstModel> IntervalLstModel { get; set; }
    }

    public class IntervalLstModel
    {
        public int MM_LOTNO_D_ID { get; set; }
        public int MM_LOTNO_H_ID { get; set; }
        public int LOT_NO { get; set; }
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
