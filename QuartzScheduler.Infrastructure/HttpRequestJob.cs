using Quartz;
using QuartzScheduler.Infrastructure.Models;
using QuartzScheduler.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace QuartzScheduler.Infrastructure
{
    public class HttpRequestJob : IJob
    {
        TaskDbContext _db = null;
        public HttpRequestJob(TaskDbContext db)
        {
            _db = db;
        }
        public async System.Threading.Tasks.Task Execute(IJobExecutionContext context)
        {
            TaskLog log = new TaskLog
            {
                StartTime = DateTime.Now
            };
            int taskId = context.JobDetail.JobDataMap.GetIntValue("TaskId");
            var task = _db.Task.FirstOrDefault(s => s.Id == taskId);
            if (task != null)
            {
                using (HttpClient client = new HttpClient())
                {
                    var message = await client.GetAsync(task.Command);

                    log.EndTime = DateTime.Now;
                    log.Status = (int)message.StatusCode;
                    log.Result = await message.Content.ReadAsStringAsync();
                }
            }

            _db.TaskLog.Add(log);
            _db.SaveChanges();
        }

       
    }
}
