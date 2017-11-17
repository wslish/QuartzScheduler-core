using System;

namespace QuartzScheduler.Infrastructure.Models
{
    public class SchedulerInformation
    {
        public string SchedulerName { get; set; }
        public string SchedulerInstanceId { get; set; }
        public Type SchedulerType { get; set; }
        public bool SchedulerRemote { get; set; }
        public bool Started { get; set; }
        public bool InStandbyMode { get; set; }
        public bool Shutdown { get; set; }
        public Type ThreadPoolType { get; set; }
        public int ThreadPoolSize { get; set; }
        public DateTimeOffset? RunningSince { get; set; }
        public int NumberOfJobsExecuted { get; set; }
        public bool JobStoreSupportsPersistence { get; set; }
    }
}
