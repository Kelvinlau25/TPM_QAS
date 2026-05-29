using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPM_QAS.Interface
{
    interface ISetting
    {
        Task<DataTable> PSP_LOAD_COMMON_SETTING(string Val, string Query = "PSP_GET_MASTER_SETTING_VALUE");
    }
}
