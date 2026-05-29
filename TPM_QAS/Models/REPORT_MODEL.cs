using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TPM_QAS.Models
{

    public class SummaryVM
    {
        [DisplayName("Product Type")]
        public string PRODTYPE { get; set; }

        [DisplayName("Lot No")]
        public string LOTNO { get; set; }

        [DisplayName("Fully NG if more than")]
        public string FullNG { get; set; }

        [DisplayName("Inspection Date")]
        public string PACKEDDATE { get; set; }

        [DisplayName("Packed Date")]
        public string PACKEDDATE2 { get; set; }

        [DisplayName("Quantity (KGs)")]
        public string QUANTITY { get; set; }

        [DisplayName("Tested by")]
        public string TESTEDBY { get; set; }

        [DisplayName("Updated by")]
        public string UPDATEDBY { get; set; }

        [DisplayName("Status")]
        public string STATUS { get; set; }

        [DisplayName("Grade")]
        public string GRADE { get; set; }

        [DisplayName("Machine Name")]
        public string BS_MACH { get; set; }
        public string COQ_ADJ_RMK { get; set; }
        public string BBS { get; set; }
        public string BYI { get; set; }

    }

    public class MoldAndPropVM
    {
        [DisplayName("Property")]
        public string PROPITEM { get; set; }
        [DisplayName("Main Property")]
        public string MAIN_PROPERTIES { get; set; }

        [DisplayName("Unit")]
        public string UNIT { get; set; }

        [DisplayName("m/c")]
        public string MACHINENAME { get; set; }

        [DisplayName("Machine Result")]
        public string AVERAGE { get; set; }

        [DisplayName("Regression Result")]
        public string REGRESSIONRESULT { get; set; }

        [DisplayName("COQ Adjust")]
        public string COQADJ { get; set; }

        [DisplayName("Specification")]
        public string SPECIFICATION { get; set; }

        [DisplayName("Grade")]
        public string GRADE { get; set; }

        [DisplayName("Status")]
        public string STATUSIND { get; set; }

        [DisplayName("SEQUENCE")]
        public string SEQUENCE { get; set; }
    }

    public class HistTransVM
    {
        [DisplayName("Property")]
        public string PROPITEM { get; set; }
    }


    public class ItemIDS
    {
        public int SEQUENCE { get; set; }
        public string ITEM { get; set; }
        public string DBCOLNAME { get; set; }
        public string ITEMVALUE { get; set; }
    }

    public class ReportVM
    {
    }

    public class HistoryTransactionTableVM
    {
        [Display(Name = "Product Type")]
        public string PRODTYPE { get; set; }
    }

    public class ReportYiVM
    {
        public string datefrom { get; set; }
        public string dateto { get; set; }
        public string prodtype { get; set; }
        public string prodlinefrom { get; set; }
        public string prodlineto { get; set; }
       
        public string xaxis { get; set; }

        public string properties { get; set; }
        public string listproperties { get; set; }
        public int MM_PROPERTIES_H_ID { get; set; }
        public List<SelectListItem> DropdownPropItem { get; set; }
        public List<SelectListItem> DropdownProdType { get; set; }
    }

    public class ReportNGVM
    {
        public string datefrom { get; set; }
        public string dateto { get; set; }

        //for cap/ce prodline
        public int id_ids_h { get; set; }
        public string prodtype { get; set; }
        public string lotno { get; set; }
        public string packkeddate { get; set; }
        public int quantity { get; set; }
        public string grade { get; set; }
        public string type { get; set; }
        public string prodline { get; set; }
        public string tagnofrom { get; set; }
        public string tagnoto { get; set; }
        public string abnormality { get; set; }
        public int ng_qty { get; set; }
        public int rowspan { get; set; }
        public int rownumber { get; set; }

    }

    public class ReportSummary
    {
        public string datefrom { get; set; }
        public string dateto { get; set; }
        public string process { get; set; }
        public int inspect_lot { get; set; }
        public int inspect_qty { get; set; }
        public int ng_lot { get; set; }
        public int ng_qty { get; set; }

    }

    public class COAGRN
    {
        public List<COAGRNDETAIL> COAGRNDETAILUN { get; set; }
        public List<COAGRNDETAIL> COAGRNDETAILGRADED { get; set; }
    }
    public class COAGRNDETAIL
    {
        public DateTime GRN_DATE { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_DESC { get; set; }
        public string VENDOR_LOTNO { get; set; }
        public string LOTQTY { get; set; }
        public string ORGANIZATION { get; set; }
        public string UOM { get; set; }
        public string last { get; set; }

        public DateTime GRADING_DATE { get; set; }
        public string GRADING_STATUS { get; set; }

    }


    public class BULKEXPORTIDS
    {
        public int ID_IDS_H { get; set; }
        public string LOTNO { get; set; }
        public string PRODUCT_CODE { get; set; }

    }
}