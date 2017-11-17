using System;

namespace QuartzScheduler.Web.Models
{
    public class JobTriggerViewModel
    {
        public string JobName { get; set; }
        public string JobGroup { get; set; }
        public string TriggerName { get; set; }
        public string TriggerGroup { get; set; }
        public string State { get; set; }
        public string CronExpression { get; set; }
        public DateTime? PreviousFireTime { get; set; }
        public DateTime? NextFireTime { get; set; }
        public string Properties { get; set; }
    }
}