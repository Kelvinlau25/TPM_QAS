using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TPM_QAS.Models
{
    public class MM_BIS_GRADE
    {
        public int ID_MM_BIS_GRADE_D { get; set; }
        public int ID_MM_BIS_GRADE_H { get; set; }
        public string BIS_GRADE_CODE { get; set; }
        public string BIS_GRADE_PROD_CODE { get; set; }
        public string BIS_SINGLE_DESIGN_CODE { get; set; }

        public string RECORD_TYP { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }
    }

    public class MM_BIS_GRADE_A
    {
        public string SQ_ID { get; set; }
        public string IDDESC { get; set; }
        public string KEY_FIELD { get; set; }
        public string KEY_VALUE { get; set; }
        public string FIELD_NAME { get; set; }
        public string B4_UPDATE { get; set; }
        public string AF_UPDATE { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }

    }

    public class MM_BIS_GRADE_VIEWMODEL
    {
        public int ID_MM_BIS_GRADE_D { get; set; }
        public int ID_MM_BIS_GRADE_H { get; set; }
        public string BIS_GRADE_CODE { get; set; }
        public string BIS_GRADE_PROD_CODE { get; set; }
        public string BIS_SINGLE_DESIGN_CODE { get; set; }

        public string RECORD_TYP { get; set; }
        public string REC_TYPE_DESC { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATED_LOC { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string UPDATED_LOC { get; set; }

        public List<MM_BIS_GRADE> ModelList { get; set; }
    }

    public class MM_BIS_GRADE_A_VIEWMODEL
    {
        public string SQ_ID { get; set; }
        public List<MM_BIS_GRADE_A> ModelList { get; set; }

    }
}