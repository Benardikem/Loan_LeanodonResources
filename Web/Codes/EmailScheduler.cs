using Quartz;
using Quartz.Impl;
using System.Threading.Tasks;

namespace Web.Codes
{
    public class EmailScheduler
    {
        public async static void Start()
        {
            IScheduler scheduler = await Task.Run(() => StdSchedulerFactory.GetDefaultScheduler());
            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<EmailJob>().Build();

            ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity("trigger1", "group1")
            .StartNow()
            .WithSimpleSchedule(x => x
            .WithIntervalInSeconds(30)
            .RepeatForever())
            .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
    }
}