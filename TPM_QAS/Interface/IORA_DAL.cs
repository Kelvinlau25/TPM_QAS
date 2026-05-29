using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using TPM_QAS.Enum;

namespace TPM_QAS.Interface
{
    interface IORA_DAL
    {
        Task<DataTable> PSP_COMMON_ORA(string Query);
        Task<DataTable> PSP_COMMON_ORA(string Query, EnumType.ExecutionType ExecutionType);
        Task<DataTable> PSP_COMMON_ORA(string Query, string ConnectionString);
        Task<DataTable> PSP_COMMON_ORA(string Query, CommandType type);
        Task<DataTable> PSP_COMMON_ORA(string Query, CommandType type, List<OracleParameter> ListofParam);
        Task<DataTable> PSP_COMMON_ORA(string Query, CommandType type, List<OracleParameter> ListofParam, string ConnectionString);
    }
}
