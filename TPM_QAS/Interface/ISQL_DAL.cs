using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPM_QAS.Enum;

namespace TPM_QAS.Interface
{
    interface ISQL_DAL
    {
        Task<DataTable> PSP_COMMON_SQL(string Query);
        Task<DataTable> PSP_COMMON_SQL(string Query, EnumType.ExecutionType ExecutionType);
        Task<DataTable> PSP_COMMON_SQL(string Query, string ConnectionString);
        Task<DataTable> PSP_COMMON_SQL(string Query, CommandType type);
        Task<DataTable> PSP_COMMON_SQL(string Query, CommandType type, List<SqlParameter> ListofParam);
        Task<DataTable> PSP_COMMON_SQL(string Query, EnumType.ExecutionType ExecutionType, CommandType type, List<SqlParameter> ListofParam);
        Task<DataTable> PSP_COMMON_SQL(string Query, CommandType type, List<SqlParameter> ListofParam, string ConnectionString);
        Task<DataTable> PSP_COMMON_SQL(string Query, EnumType.ExecutionType ExecutionType, CommandType type, List<SqlParameter> ListofParam, string ConnectionString);
    }
}
