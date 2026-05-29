using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TPM_QAS.Models;
using TPM_QAS.DAL;

namespace TPM_QAS.Helpers
{
    public class AuditTrailHelper
    {
        public static async Task<AuditTrailModels> AuditTrailStoreProcedureSqlAsync(string sp, CommandType type, List<SqlParameter> ListofParam, string DBName)
        {
            CommonFunction common = new CommonFunction();
            DataTable result = await common.PSP_COMMON_SQL(sp, type, ListofParam, "", DBName);

            List<string> keyNames = result.Columns.Cast<DataColumn>()
            .Select(column => column.ColumnName)
            .ToList();

            string jsonResult = JsonConvert.SerializeObject(result);

            AuditTrailModels dataModel = new AuditTrailModels
            {
                JsonData = jsonResult,
                ListData = keyNames
            };

            return dataModel;
        }
    }
}
