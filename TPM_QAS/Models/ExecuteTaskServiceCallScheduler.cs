using Quartz;
using Quartz.Impl;
using System;
using TPM_QAS.DAL;

namespace TPM_QAS.Models
{
    public class ExecuteTaskServiceCallScheduler
    {
        private static readonly string ScheduleCronExpression = Database.GetAppSettingStatic("ExecuteTaskScheduleCronExpression") ?? "0 0/1 * 1/1 * ? *";

        public static async System.Threading.Tasks.Task StartAsync()
        {
            try
            {
                var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                if (!scheduler.IsStarted)
                {
                    await scheduler.Start();
                }
                var job = JobBuilder.Create<ExecuteTaskServiceCallJob>()
                    .WithIdentity("ExecuteTaskServiceCallJob1", "group1")
                    .Build();
                var trigger = TriggerBuilder.Create()
                    .WithIdentity("ExecuteTaskServiceCallTrigger1", "group1")
                    .WithCronSchedule(ScheduleCronExpression)
                    .Build();
                await scheduler.ScheduleJob(job, trigger);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
