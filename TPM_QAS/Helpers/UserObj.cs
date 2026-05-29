using System;

namespace TPM_QAS.Helpers
{
    public class UserObj
    {
        public int ID_ACL_USER { get; set; }
        public string EMP_NAME { get; set; }
    }

    public class AdUserObj
    {
        public string AD_User_ID { get; set; }
    }

    public class ACL_UserObj
    {
        public int ID_ACL_USER { get; set; }
        public int ID_ACL_ROLE { get; set; }
        public int ID_ACL_RESOURCE { get; set; }
        public string USER_ID { get; set; }
        public string USR_EMAIL { get; set; }
        public string COMPANY { get; set; }
        public string EMP_NO { get; set; }
        public string EMP_NAME { get; set; }
        public string ROLE_NAME { get; set; }
        public string ROLE_DESC { get; set; }
        public string RESOURCE_NAME { get; set; }
        public string RESOURCE_DESC { get; set; }
    }
}
