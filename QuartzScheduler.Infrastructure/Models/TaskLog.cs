using System;
using System.Collections.Generic;
using System.Text;

namespace QuartzScheduler.Model
{
    public class TaskLog
    {

        /// <summary>
        /// auto_increment
        /// </summary>		
        public long Id { get; set; }
        /// <summary>
        /// task_id
        /// </summary>		
        public int TaskId { get; set; }
        /// <summary>
        /// name
        /// </summary>		
        public string Name { get; set; }
        /// <summary>
        /// command
        /// </summary>		
        public string Command { get; set; }
        /// <summary>
        /// start_time
        /// </summary>		
        public DateTime StartTime { get; set; }
        /// <summary>
        /// end_time
        /// </summary>		
        public DateTime EndTime { get; set; }
        /// <summary>
        /// status
        /// </summary>		
        public int Status { get; set; }
        /// <summary>
        /// result
        /// </summary>		
        public string Result { get; set; }

    }
}
