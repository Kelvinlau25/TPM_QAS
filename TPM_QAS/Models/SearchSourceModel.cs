using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TPM_QAS.Models
{
    public class SearchSourceModel
    {
        // MM
        public static string[] TypeLst = { "Type/TYPE" };
        public static string[] CategoryLst = { "Category/CATEGORY", "Category_Desc/CATEGORY_DESC" };
        public static string[] AcctTypLst = { "Account_Typ/ACCT_TYP" };
        public static string[] ShiftLst = { "Shift/SHIFT_GROUP" };
        public static string[] TransformerLst = { "Transformer/TRANSFORMER", "Temperature/TEMPERATURE" };
        public static string[] JobStatLst = { "Job_Status/JOB_STATUS" };
        public static string[] AccidTypeLst = { "Accid_Type/ACCID_TYPE" };
        public static string[] RoutineLst = { "Routine_Code/ROUTINE_CODE", "Routine_Desc/ROUTINE_DESC" };
        public static string[] PriorityLst = { "Priority_Level/PRIORITY_LVL", "Priority_Desc/PRIORITY_DESC" };

        public static string[] AreaList = { "Area/AREA" };
        public static string[] MM_AREA_LST = { "Area/AREA" };
        public static string[] VectorList = { "Area/AREA", "Unit Process/PROCESS_NAME", "Process Cat/PROCESS_CAT", "Vector/VELOCITY" };
        public static string[] MM_VECTOR_LST = { "Area/AREA", "Unit Process/PROCESS_NAME", "Process Category/PROCESS_CAT", "Vector/VELOCITY" };

        public static string[] MM_EQUIP_LST = { "Area/AREA", "Unit/UNIT", "Equipment/EQUIP", "Equipment Description/EQUIP_DESC" };
        public static string[] PopoutApproleLst = { "Name/NAME" , "Company/COMPANY","Department/DEPARTMENT","Section/SECTION", "Email/COMPANYEMAIL" };
        public static string[] PopoutEquip = { "Equipment/EQUIP", "Equipment Description/EQUIP_DESC" };
        public static string[] MM_APP_ROLE_LST = { "Module Name/MODULE_NAME","Approval Name/APP_ROLE_NAME", "Action Type/APPR_TYPE", "Level/APPR_ORDER" };
        public static string[] MM_COMMONGRP_LST = { "Category/CATEGORY" };


        // Entry
        public static string[] EQUIP_SCH_ENTRY_LST = { "Area/AREA", "Category/CATEGORY", "PM/PM" };
        public static string[] DT_ENG_PROD_REQ_LST = { "Work Order No/WORK_ORDER_NO", "Module Name/MODULE", "Area/AREA", "Unit Process/UNIT_PROCESS", "Engineer PIC/CREATED_BY" , "Requested Date/REQ_DATE" };
        public static string[] DT_SHIFT_E_VIB_LST = {  "Area/AREA", "Unit Category/UNIT_CATEGORY", "Vector/VECTOR", "Month/VIB_MONTH" };
        public static string[] DT_SHIFT_TX_TEMP_LST = { "Area/AREA", "Month/YEAR_MONTH" };


        public static string[] ceeList = { "CE No/CE_NO", "Area/AREA", "CE Title/CE_TITLE", "Comp Date/COMP_DATE", "Progress/PROGRESS", "Remark/REMARK" };
        public static string[] CE_ENTRY_LST = {  "CE No/CE_NO", "Area/AREA", "CE Title/CE_TITLE", "Comp Date/COMP_DATE", "Progress/PROGRESS", "Remark/REMARK" };
        public static string[] PPPICList = { "EMP_NAME/EMP_NAME" };
        public static string[] cogenList = { "P Ref No/P_REF_NO", "Apply Date/APPLY_DATE", "P Work Desc/P_WORK_DESC", "App Vendor/APP_VENDOR", "App Pic/APPR_PIC", "Final App Pic/FINAL_APPR_PIC" };
        public static string[] COGEN_P_E_LST = { "P Ref No/P_REF_NO", "Apply Date/APPLY_DATE", "P Work Desc/P_WORK_DESC", "App Vendor/APP_VENDOR", "App Pic/APPR_PIC", "Final App Pic/FINAL_APPR_PIC" };
        public static string[] EquipList = { "Equipment/EQUIP", "Equipment Description/EQUIP_DESC", "Area ID/ID_MM_AREA_H", "Area/AREA" };
        public static string[] ShiftList = { "Job No/JOB_NO", "Area/AREA", "Equipment/EQUIP", "Created Date/CREATED_DATE2" };
        public static string[] SHIFT_E_LST = { "Job No/JOB_NO", "Area/AREA", "Equipment/EQUIP", "Created Date/CREATED_DATE2" };
        public static string[] DailyJobList = { "Job ID/DJ_NO","Area/AREA","Section/SECTION", "Created Date/CREATED_DATE2" };
        public static string[] DAILY_JE_LST = { "Job ID/DJ_NO","Area/AREA", "Section/SECTION", "Created Date/CREATED_DATE2" };
        public static string[] ENTRY_COGEN_LIST = { "Job No/JOB_NO", "Area/AREA" };
        //Entry

    }
}