using CargoSupport.Web.IJobs;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.Web.Helpers
{
    public class TaskHelper
    {
        private readonly IScheduler _scheduler;

        public TaskHelper(IScheduler factory)
        {
            _scheduler = factory;
        }

        public async Task CheckAvailability()
        {
            ITrigger trigger = TriggerBuilder.Create()
                 .WithIdentity($"BackupDatabase-{DateTime.Now}")
                 .StartAt(new DateTimeOffset(DateTime.Now.AddSeconds(5)))
                 .WithPriority(1)
                 .Build();

            //IDictionary<string, object> map = new Dictionary<string, object>()
            //{
            //    {"Current Date Time", $"{DateTime.Now}" },
            //    {"Tickets needed", $"5" },
            //    {"Concert Name", $"Rock" }
            //};

            IJobDetail job = JobBuilder.Create<BackupDatabase>()
                        .WithIdentity("BackupDatabase")
                        //.SetJobData(new JobDataMap(map))
                        .Build();

            await _scheduler.ScheduleJob(job, trigger);
        }
    }
}