using Quartz;
using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using TPM_QAS.DAL;

namespace TPM_QAS.Models
{
    public class ExecuteTaskServiceCallJob : IJob
    {
        public static readonly string SchedulingStatus = Database.GetAppSettingStatic("ExecuteTaskServiceCallSchedulingStatus") ?? "OFF";

        public Task Execute(IJobExecutionContext context)
        {
            var task = Task.Run(() =>
            {
                if (SchedulingStatus.Equals("ON"))
                {
                    try
                    {
                        string f = "";
                    }
                    catch (Exception ex)
                    {
                    }
                }
            });
            return task;
        }
    }
}
