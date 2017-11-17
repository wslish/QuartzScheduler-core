using System;
using System.Collections.Generic;

namespace QuartzScheduler.Infrastructure.Models
{
    [Serializable]
    public class Job
    {
        public string Name { get; set; }
        public string GroupName { get; set; }
        public IDictionary<string,object> DataMap { get; set; }
        public List<Trigger> Triggers { get; set; }
    }
}
