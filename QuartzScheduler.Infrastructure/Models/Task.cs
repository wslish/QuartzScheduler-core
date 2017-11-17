using System;
using System.Collections.Generic;
using System.Text;

namespace QuartzScheduler.Model
{
    public class Task
    {

        /// <summary>
        /// auto_increment
        /// </summary>		
        public int Id { get; set; }
        /// <summary>
        /// name
        /// </summary>		
        public string Name { get; set; }
        public string JobGroup { get; set; }
        public int IsDurable { get; set; }

        /// <summary>
        /// level
        /// </summary>		
        public int Level { get; set; }
        /// <summary>
        /// dependency_task_id
        /// </summary>		
        public string DependencyTaskId { get; set; }
        /// <summary>
        /// dependency_status
        /// </summary>		
        public int DependencyStatus { get; set; }
        /// <summary>
        /// spec
        /// </summary>		
        public string Spec { get; set; }
        /// <summary>
        /// protocol
        /// </summary>		
        public int Protocol { get; set; }
        /// <summary>
        /// command
        /// </summary>		
        public string Command { get; set; }
        /// <summary>
        /// timeout
        /// </summary>		
        public int Timeout { get; set; }
        /// <summary>
        /// multi
        /// </summary>		
        public int Multi { get; set; }

        /// <summary>
        /// notify_status
        /// </summary>		
        public int NotifyStatus { get; set; }

        /// <summary>
        /// NotifyReceiver
        /// </summary>		
        public string NotifyReceiver { get; set; }
        /// <summary>
        /// cron
        /// </summary>		
        public string Cron { get; set; }
        /// <summary>
        /// remark
        /// </summary>		
        public string Remark { get; set; }
        /// <summary>
        /// status
        /// </summary>		
        public int Status { get; set; }
        /// <summary>
        /// created
        /// </summary>		
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 删除
        /// </summary>
        public int IsDeleted { get; set; }
    }
}
