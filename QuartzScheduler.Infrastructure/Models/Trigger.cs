using System;
using System.Collections.Generic;

namespace QuartzScheduler.Infrastructure.Models
{
    [Serializable]
    public class Trigger
    {
        public string Name { get; set; }
        public string GroupName { get; set; }
        public string Type { get; set; }
        public string State { get; set; }
        public string CronExpression { get; set; }
        public DateTime? NextFireTime { get; set; }
        public DateTime? PreviousFireTime { get; set; }
        public IDictionary<string, object> DataMap { get; set; }
    }
}
