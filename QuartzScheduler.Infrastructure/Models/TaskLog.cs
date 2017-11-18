using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QuartzScheduler.Model
{
    [Table("task_log")]
    public class TaskLog
    {

        /// <summary>
        /// auto_increment
        /// </summary>		
        public long Id { get; set; }
        /// <summary>
        /// task_id
        /// </summary>		
        [Column("task_id")]
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
        [Column("start_time")]
        public DateTime StartTime { get; set; }
        /// <summary>
        /// end_time
        /// </summary>		
        [Column("end_time")]
        public DateTime EndTime { get; set; }
        /// <summary>
        /// status 0 失败，1 执行中，2 成功，3 取消
        /// </summary>		
        public int Status { get; set; }
        /// <summary>
        /// result
        /// </summary>		
        public string Result { get; set; }

    }
}
