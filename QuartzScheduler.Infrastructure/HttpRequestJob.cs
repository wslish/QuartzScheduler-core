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
            try
            {

                int taskId = context.JobDetail.JobDataMap.GetIntValue("TaskId");
                int timeout = context.JobDetail.JobDataMap.GetIntValue("Timeout");

                var task = _db.Task.FirstOrDefault(s => s.Id == taskId);
                if (task != null)
                {
                    log.TaskId = task.Id;
                    log.Name = task.Name;
                    log.Command = task.Command;
                    log.EndTime = DateTime.Now;
                    using (HttpClient client = new HttpClient())
                    {
                        if (timeout > 0)
                        {
                            client.Timeout = TimeSpan.FromSeconds(timeout);
                        }
                        var message = await client.GetAsync(task.Command);

                        log.Status = 2;
                        log.Result = await message.Content.ReadAsStringAsync();
                    }
                }


            }
            catch (Exception ex)
            {
                log.Result = ex.Message;
            }
            finally
            {
                _db.TaskLog.Add(log);
                _db.SaveChanges();
            }

        }


    }
}
