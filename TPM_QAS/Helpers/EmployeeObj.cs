using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TPM_QAS.Helpers
{
    public class EmployeeObj
    {
        #region Properties

        /// <summary>
        /// Gets or sets User ID property.
        /// </summary>

        public int ID_MM_USER_H { get; set; }
        public string USER_NAME { get; set; }
        public string USER_EMAIL { get; set; }
        public string DEPARTMENT { get; set; }
        public string SHIFT { get; set; }
        public int EMP_NO { get; set; }
        public int APPROVAL_LEVEL { get; set; }


        #endregion
    }

    public class AdEmployeeObj
    {
        public string AD_User_ID { get; set; }
    }

    public class ACL_EmployeeObj
    {
        public int ID_MM_USER_H { get; set; }
        public string USER_NAME { get; set; }
        public string USER_EMAIL { get; set; }
        public string DEPARTMENT { get; set; }
        public string SHIFT { get; set; }
        public int EMP_NO { get; set; }
        public int APPROVAL_LEVEL { get; set; }
    }
}