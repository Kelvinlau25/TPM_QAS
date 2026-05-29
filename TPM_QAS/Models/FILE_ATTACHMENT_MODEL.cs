using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TPM_QAS.Models
{
    public class FileAttModel
    {
        public string MODULE_TYPE { get; set; }
        public string MODULE_NAME { get; set; }
        
        public string RECORD_TYP { get; set; }
        public string REC_TYPE_DESC { get; set; }
        public string Created_By { get; set; }
        public string Created_Date { get; set; }
        public string CREATED_DATE2 { get; set; }
        public string Created_Loc { get; set; }
        public string Updated_By { get; set; }
        public string Updated_Date { get; set; }
        public string Updated_Loc { get; set; }

        public List<FileAttLstModel> FileAttLstModel { get; set; }

    }

    public class FileAttLstModel
    {
        public string LOT_NO { get; set; }
        public string MODULE_TYPE { get; set; }
        public string FILE_ATTACHMENT { get; set; }
        public string FILE_ATTACHMENT_PATH { get; set; }
        public string GROUP_DATE { get; set; }

        public string RECORD_TYP { get; set; }
        public string REC_TYPE_DESC { get; set; }
       

    }

}