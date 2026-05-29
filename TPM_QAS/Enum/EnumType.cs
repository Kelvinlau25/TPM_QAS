using System;

namespace TPM_QAS.Enum
{
    public class EnumType
    {
        public enum PDF_TYPE
        {
            PORTRAIT = 0,
            LANSCPAPE = 1
        }
        public enum ExecutionType
        {
            ExecuteReader = 0,
            ExecuteNonQuery = 1
        }
        public enum StatusType
        {
            View = 1,
            Edit = 3,
            Deleted = 5,
            New = 0,
        }
    }
}
