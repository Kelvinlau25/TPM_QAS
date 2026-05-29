using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPM_QAS.Interface
{
    interface IDAPPER_DAL
    {
        Task<List<T>> PSP_COMMON_DAPPER<T>(string Query);
        Task<List<T>> PSP_COMMON_DAPPER<T>(string Query, string ConnectionString);
        Task<List<T>> PSP_COMMON_DAPPER<T>(string Query, System.Data.CommandType Commandtype);
        Task<List<T>> PSP_COMMON_DAPPER<T>(string Query, System.Data.CommandType Commandtype, object ListofParam);
    }
}
