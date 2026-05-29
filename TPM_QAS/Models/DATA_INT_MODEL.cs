using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TPM_QAS.Models
{

    public class DataIntModel
    {
        public List<DataIntLogModel> DataIntLogModel { get; set; }
        public List<DataIntLstModel> DataIntLstModel { get; set; }
    }
    public class DataIntLogModel
    {
        public int ID_INT_H { get; set; }
        public string YEAR_MONTH { get; set; }
        public string INT_TYPE { get; set; }
        public string STATUS { get; set; }
        public string RECORD_TYP { get; set; }
        public string REC_TYPE_DESC { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
    }

    public class DataIntLstModel
    {
        public int ID_INTEGRATION { get; set; }
        public string DISPLAY_NAME { get; set; }
        public string INT_TYPE { get; set; }
        
    }


    #region raw data
    public class GradeDataModel
    {
        public string PROD_CODE { get; set; }
        public string PROD_DESC { get; set; }
        public string GRADE { get; set; }
        public string LOT_NO { get; set; }
        public string PACK_DATE { get; set; }
        public string PACK_QTY { get; set; }
        public string TAG_NO { get; set; }
    }
    #endregion
}