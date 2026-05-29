using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;



namespace TPM_QAS.Models
{

    public class DailyIDSTransVM
    {

        public int ID_IDS_H { get; set; }
        public int IDS_D_ID { get; set; }
        [Display(Name = "Product Type")]
        public string PRODTYPE { get; set; }
        [Display(Name = "Inspection Date")]
        public string PACKEDDATE { get; set; }
        [Display(Name = "Packed Date")]
        public string PACKEDDATE2 { get; set; }
        [Display(Name = "Lot No")]
        public string LOTNO { get; set; }
        [Display(Name = "Quantity (kgs)")]
        public int QUANTITY { get; set; }
        [Display(Name = "Tested By")]
        public string TESTEDBY { get; set; }
        [Display(Name = "Updated By")]
        public string UPDATEDBY { get; set; }
        [Display(Name = "Status")]
        public string STATUS { get; set; }
        [Display(Name = "Grade")]
        public string GRADE { get; set; }
        [Display(Name = "Fully NG if more than")]
        public string FullNG { get; set; }
        [Display(Name = "Overwrite")]
        public string OVERWRITE { get; set; }
        [Display(Name = "Inspection")]
        public string INSPECTION { get; set; }

        public int INT_STATUS { get; set; }
        public bool PRODTYPCHGIND { get; set; }
        public int AFTERPRODTYPCHG { get; set; }
        public bool FLOT { get; set; }
        public bool MABS_P { get; set; }
        public bool CROSS_LINE { get; set; }
        public bool REJECT { get; set; }
        public string CIND { get; set; }
        public string REPACKLOT { get; set; }


        public string RECORD_TYP { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        [Display(Name = "Updated By")]
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
        public string SEARCH_VALUE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_LOC { get; set; }

        
               
        public Nullable<System.DateTime> PACKED_DATE { get; set; }
        public string action { get; set; }
        public string ranges { get; set; }
        

        //molding section
        public string mc { get; set; }
        public string RegressionResult { get; set; }
        public decimal? machineresult { get; set; }
        public decimal? averageMold { get; set; }
        public decimal? averageApp { get; set; }
        public decimal? avgData { get; set; }

        public string mainreading { get; set; }
        public int maxcount { get; set; }
        public int id_ids_d { get; set; }
        public int id_ids_sl { get; set; }
        public Nullable<System.DateTime> testeddatetimes { get; set; }
        public string updatedbys { get; set; }
        public string typeinds { get; set; }
        public string PropItem { get; set; }

        public string Reading1 { get; set; }
        public string Reading2 { get; set; }
        public string Reading3 { get; set; }
        public string Reading4 { get; set; }
        public string Reading5 { get; set; }
        public string Reading6 { get; set; }
        public string YIFORMULA { get; set; }

        public List<MoldingModel> MoldingList { get; set; }
        public List<MoldingModel> newHistoryList { get; set; }
        public List<descriptionmodel> descriptionList { get; set; }
        public List<TransactionHistModel> HistoryList { get; set; }
        public List<AppearanceTableModel> AppearanceTableList { get; set; }

        public List<DataTable> apptable { get; set; }
        public List<List<ratioModel>> ratioList { get; set; }

        public List<FirstBagModel> FirstBagList { get; set; }
        public List<PropGradeSpecModel> PropGradeSpecList { get; set; }

        public DateTime PACKEDDATEDATE { get; set; }


        //pp_passfail
        public string passfail { get; set; }

        //for mandatory
        public string prodline { get; set; }
        public string prodgroup { get; set; }
        public string chkType { get; set; }

        //report
        public int ID_IDS_D_APP { get; set; }
        public string REPORT_BY { get; set; }
        public string APPR_MOLD_NAME { get; set; }
        public string APPR_APPEA_NAME { get; set; }
        public string APPR_CHKBY_NAME { get; set; }
        public string APPR_VERFBY_NAME { get; set; }
        public string APPR_KEYBY_NAME { get; set; }
        public string MOLD_DATE { get; set; }
        public string APPEARANCE_DATE { get; set; }
        public string CHECKED_BY_DATE { get; set; }
        public string VERIFIED_BY_DATE { get; set; }
        public string KEYED_BY_DATE { get; set; }

        public string LOT_STATUS { get; set; }
        public string SILO_NO { get; set; }
        public string ONLINE_MFR { get; set; }
        public string ONLINE_YI { get; set; }
        public string ONLINE_YPBP { get; set; }
        public string TWIN_PELLET { get; set; }
        public string FISHEYE_LEVEL { get; set; }
        public string BS_SPOT_1 { get; set; }
        public string BS_SPOT_2 { get; set; }
        public string BS_SPOT_3 { get; set; }
        public string PRODUCTION_REMARK { get; set; }
        public string QA_REMARK { get; set; }
        public string OVERWRITE_GRADE { get; set; }
        public string BS_MACH { get; set; }
        public string COQ_ADJUST_RMK { get; set; }

        public List<SelectListItem> DropdownApprMold { get; set; }
        public List<SelectListItem> DropdownApprProdMold { get; set; }
        public List<SelectListItem> DropdownOwGrade { get; set; }
        public List<SelectListItem> DropdownBSMach { get; set; }

        //blank black speck & YI
        public string BBS { get; set; }
        public string BYI { get; set; }
        public string newcoq { get; set; }

        public bool IS_ARCHIVE { get; set; }

        public List<MoldingModel> APP_PCL_SPEC { get; set; }
        public List<MoldingModel> APP_PCL_REMARK { get; set; }
    }

    public class MoldingModel
    {
        //public string PropTypeMain { get; set; }
        public int Seq { get; set; }
        public string Properties { get; set; }
        public string PropItem { get; set; }
        //public string ProdLine { get; set; }
        public string Unit { get; set; }
        public string TypeInd { get; set; }
        public string RegressionResult { get; set; }
        public string COQAdj { get; set; }
        //public string Grade { get; set; }
        public string MachineName { get; set; }
        public string DataChecking { get; set; }
        public string COQInd { get; set; }
        public string RegressInd { get; set; }
        public string RegressFormula { get; set; }
        public int ID_IDS_D { get; set; }
        public int ID_IDS_H { get; set; }
        public string Average { get; set; }
        public string ProdGroup { get; set; }
        public string CntInd { get; set; }
        public string StatusInd { get; set; }
        public string MainReading { get; set; }
        public string Specification { get; set; }
        public string PCL_Spec { get; set; }

        public int Count { get; set; }
        public string lotno { get; set; }

        public int rounding { get; set; }
        public string mandatory { get; set; }
        public bool seg { get; set; }
        public bool extra { get; set; }        


        /// <summary>
        /// nabila 30-09-2021
        /// </summary>
        public string prodtype { get; set; }
        public decimal? averageMold { get; set; }
        public string Grade { get; set; }
        public string PCL_Result { get; set; }
        public string Reading1 { get; set; }
        public string Reading2 { get; set; }
        public string Reading3 { get; set; }
        public string Reading4 { get; set; }
        public string Reading5 { get; set; }
        public string Reading6 { get; set; }
        public string Grade1 { get; set; }
        public string Grade2 { get; set; }
        public string Grade3 { get; set; }
        public string Grade4 { get; set; }
        public string Grade5 { get; set; }
        public string Grade6 { get; set; }

    }

    public class descriptionmodel
    {
        public string description { get; set; }
        public List<List<fieldnamemodel>> fieldnameList { get; set; }
        public List<List<tagnomodel>> tagnoList { get; set; }
        public List<descmodel> descList { get; set; }
        //public List<ratioModel> ratioList { get; set; }
    }

    public class tagnomodel
    {
        public string tagno { get; set; }
        public string avg { get; set; }
        public string grd { get; set; }

        public string res5 { get; set; }
        public string res4 { get; set; }
        public string res2 { get; set; }
        public string res7 { get; set; }
        public string res3 { get; set; }
        public string tonnage { get; set; }
        public int seq { get; set; }
        
        

        public tagnomodel Clone()
        {
            return (tagnomodel)this.MemberwiseClone();
        }
    }

    public class fieldnamemodel
    {
        public string fieldname { get; set; }
        public string calcratio { get; set; }
    }

    public class descmodel
    {
        public string desc { get; set; }
        public string ratio { get; set; }
        public int id_ids_d { get; set; }
        public string descGrade { get; set; }
        public string rounding { get; set; }
    }

    public class TransactionHistModel
    {
        public string PropItem { get; set; }
        public string Reading1 { get; set; }
        public string Reading2 { get; set; }
        public string Reading3 { get; set; }
        public string Reading4 { get; set; }
        public string Reading5 { get; set; }
        public string Reading6 { get; set; }
        public string Average { get; set; }
        public string Rresult { get; set; }
        public string Grade { get; set; }
        public string TestedDateTime { get; set; }
        public string Properties { get; set; }
        public string id_ids_d { get; set; }
    }

    
    public class AppearanceTableModel
    {
        public string Properties { get; set; }
        public string PropertyItem { get; set; }
        public string Mgrade { get; set; }
    }

    public class FirstBagModel
    {
        public int ID { get; set; }
        public string PROPERTIES { get; set; }
        public string TAGNO { get; set; }
        public string TONNAGE { get; set; }
        public string GRADE { get; set; }
        public string READING { get; set; }

    }

    public class PropGradeSpecModel
    {
        public string PROPERTIES { get; set; }
        public string PROP_ITEM { get; set; }
        public string GRADE { get; set; }
        public string PRIORITY { get; set; }
        public string FINAL_PRIORITY { get; set; }
        public string L_SPEC { get; set; }
        public string U_SPEC { get; set; }
        public string ROUNDING { get; set; }
        public string GRADINGIND { get; set; }

    }

    public class MachineDataModel
    {
        public string lotNo { get; set; }
        public double machineData { get; set; }
        public string tonnage { get; set; }
        public string properties { get; set; }
        public string createddate { get; set; }
        public string yiformula { get; set; }
    }

    public class MachineNameModel
    {
        public string MachineName { get; set; }
        public string Result { get; set; }
        public string UpdateDate { get; set; }
        public string PropItem { get; set; }
        public string RegressFormula { get; set; }
        public string RegressInd { get; set; }
        public string Grade { get; set; }
        public string RegressionResult { get; set; }
        public string ProdGroup { get; set; }
        public string Average { get; set; }

    }

    public class SegregationBSModel
    {
        public string tagno { get; set; }
        public string reading { get; set; }
        public string grade { get; set; }
        public string id { get; set; }
        public string gradeInd { get; set; }
        public string prodtype { get; set; }
        public string id_ids_d { get; set; }
        public string tonnage { get; set; }
    }

    public class ratioModel
    {
        public decimal ratio { get; set; }
        public string name { get; set; }
    }

    public class SegregationSegLModel
    {
        public string tagno { get; set; }
        public string reading { get; set; }
        public string grade { get; set; }
        public string id_ids_segl { get; set; }
        public string gradeInd { get; set; }
        public string compare { get; set; }
        public string prodtype { get; set; }
        public string propitem { get; set; }
        public string id_ids_d { get; set; }
        public string ptype { get; set; }
        public bool subNG { get; set; }
        public bool ddlpassfail { get; set; }
        public bool btnSave { get; set; }
        public string tonnage { get; set; }

    }

    public class SubSegregationSegLModel
    {
        public string tagno { get; set; }
        public string reading { get; set; }
        public string gradeInd { get; set; }
        public string id_ids_segl { get; set; }
        public string grade { get; set; }
    }

    public class SummaryModel
    {

        [Column("ID_IDS_SUM")]
        public int ID_IDS_SUM { get; set; }

        [Column("ID_IDS_H")]
        public int ID_IDS_H { get; set; }

        [Column("GRADE")]
        public string GRADE { get; set; }

        [Column("TAGNOFROM")]
        public string TAGNOFROM { get; set; }

        [Column("TAGNOTO")]
        public string TAGNOTO { get; set; }

        [Column("TONNAGEFROM")]
        public string TONNAGEFROM { get; set; }

        [Column("TONNAGETO")]
        public string TONNAGETO { get; set; }

        [Column("RECORD_TYP")]
        public string RECORD_TYP { get; set; }

        [Column("ABNORMALITIES")]
        public string ABNORMALITIES { get; set; }

    }
}