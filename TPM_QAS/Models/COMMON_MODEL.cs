using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TPM_QAS.Models
{
    public class ReturnData
    {
        public string err { get; set; }
        public string redirectURL { get; set; }
        public string status { get; set; }
        public string validation { get; set; }
        public string msg { get; set; }

    }

    public class AuditTrailModel
    {
        public string ID { get; set; }
        public string ACTION { get; set; }
        public string TABLE_COLUMN { get; set; }
        public DateTime CREATION_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public string OLD_VALUE { get; set; }
        public string NEW_VALUE { get; set; }
        public string PRIMARY_KEY { get; set; }
        public string H_ID { get; set; }
    }
}