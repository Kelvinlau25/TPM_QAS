using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TPM_QAS.Models
{
    public class AppRoleModel
    {
        public int MM_APPROLE_H_ID { get; set; }
        public int ROLE_NAME_ID { get; set; }
        public string ROLE_NAME { get; set; }
        public string REMARKS { get; set; }
        public string EMP_NAME { get; set; }
        public int EMP_NAME_COUNT { get; set; }
        public string EMP_ID { get; set; }
       
        public string RECORD_TYP { get; set; }
        public string REC_TYPE_DESC { get; set; }
        public string SEARCH_VALUE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }

        public List<SelectListItem> DropdownApprovalName { get; set; }
        public List<AppRoleEmpLstModel> AppRoleEmpLstModel { get; set; }
    }

    public class AppRoleEmpLstModel
    {
        public int MM_APPROLE_D_ID { get; set; }
        public int MM_APPROLE_H_ID { get; set; }
        public string EMP_NAME { get; set; }
        public int EMP_ID { get; set; }
        public string EMP_EMAIL { get; set; }
        public string EMP_DEPT { get; set; }
        public string EMP_SECTION { get; set; }
        
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
    }
}