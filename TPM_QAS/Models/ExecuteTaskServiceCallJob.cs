using Quartz;
using System;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace TPM_QAS.Models
{
    public class ExecuteTaskServiceCallJob : IJob
    {
        DatabaseModel.Database db = new DatabaseModel.Database();

     
        public static readonly string SchedulingStatus = ConfigurationManager.AppSettings["ExecuteTaskServiceCallSchedulingStatus"];
        public Task Execute(IJobExecutionContext context)
        {
            var task = Task.Run(() =>
            {
                if (SchedulingStatus.Equals("ON"))
                {
                    try
                    {
                        //Do whatever stuff you want
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