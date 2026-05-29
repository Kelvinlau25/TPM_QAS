using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TPM_QAS.Models
{
    public class ProdPackModel
    {
        public int MM_PRODPACK_ID { get; set; }
        public int PACKING_TYPE_ID { get; set; }
        public string PACKING_TYPE { get; set; }

        public List<String> PROD_PACK_NAME { get; set; }
        public string PROD_PACK_NAME_STRING { get; set; }

        public string RECORD_TYP { get; set; }
        public string REC_TYPE_DESC { get; set; }
        public string SEARCH_VALUE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
        public List<SelectListItem> DropdownPackType { get; set; }
        public List<SelectListItem> DropdownPackName { get; set; }

    }
}