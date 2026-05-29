using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace TPM_QAS.Interface
{
    interface IDAPPER_DAL
    {
        Task<List<T>> PSP_COMMON_DAPPER<T>(string Query);
        Task<List<T>> PSP_COMMON_DAPPER<T>(string Query, string ConnectionString);
        Task<List<T>> PSP_COMMON_DAPPER<T>(string Query, CommandType Commandtype);
        Task<List<T>> PSP_COMMON_DAPPER<T>(string Query, CommandType Commandtype, object ListofParam);
    }
}
