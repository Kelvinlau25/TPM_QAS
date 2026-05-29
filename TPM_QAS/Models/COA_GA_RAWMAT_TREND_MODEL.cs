using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TPM_QAS.Models
{
    public class RawMatTrendModel
    {
        public string DATE_FROM { get; set; }
        public string DATE_TO { get; set; }

        public int MM_MATERIAL_H_ID { get; set; }
        public string MM_SUPPLIER_H_ID_STRING { get; set; }
        public List<String> MM_SUPPLIER_H_ID { get; set; }
        public string MM_MATERIAL_D2_ID_STRING { get; set; }
        public List<String> MM_MATERIAL_D2_ID { get; set; }

        public List<SelectListItem> DropdownMaterial { get; set; }
        public List<SelectListItem> DropdownSupplier { get; set; }
        public List<SelectListItem> DropdownItem { get; set; }

    }
}