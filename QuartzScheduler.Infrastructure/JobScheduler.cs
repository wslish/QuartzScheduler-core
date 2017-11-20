using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using QuartzScheduler.Infrastructure.Models;
using QuartzScheduler.Infrastructure.Utils;
using System.Threading.Tasks;

namespace QuartzScheduler.Infrastructure
{
    public class JobScheduler
    {
        private static IScheduler _scheduler;
        TaskDbContext _db = null;

        public JobScheduler(TaskDbContext db, IScheduler scheduler)
        {
            _db = db;
            _scheduler = scheduler;
        }

        static JobScheduler()
        {
            var schedulerFactory = new StdSchedulerFactory();
            _scheduler = schedulerFactory.GetScheduler().Result;
        }
        public void Start()
        {
            if (!_scheduler.IsStarted)
                _scheduler.Start();
        }

        public void Shutdown(bool waitForJobsToComplete)
        {
            if (!_scheduler.IsShutdown)
                _scheduler.Shutdown(waitForJobsToComplete);
        }

        public void StandBy()
        {
            if (!_scheduler.InStandbyMode)
                _scheduler.Standby();
        }

        /// <summary>
        /// Pause all triggers of jobs
        /// </summary>
        public void PauseAll()
        {
            _scheduler.PauseAll();
        }

        /// <summary>
        /// Resume all triggers of jobs
        /// </summary>
        public void ResumeAll()
        {
            _scheduler.ResumeAll();
        }



        /// <summary>
        ///  Remove the indicated trigger from the scheduler. If job is not durable, 
        ///  then job will be also deleted.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UnscheduleJob(string triggerName, string triggerGroup)
        {
            return await _scheduler.UnscheduleJob(new TriggerKey(triggerName, triggerGroup));
        }

        /// <summary>
        /// Delete specified job and all of its triggers. Does not stop running trigger.
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="jobGroup"></param>
        /// <returns></returns>
        public Task<bool> DeleteJob(string jobName, string jobGroup)
        {
            return _scheduler.DeleteJob(new JobKey(jobName, jobGroup));
        }

        /// <summary>
        /// Pause job's all triggers. Does not stop running trigger.
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="groupName"></param>
        public void PauseJob(string jobName, string groupName)
        {
            _scheduler.PauseJob(new JobKey(jobName, groupName));
        }

        public void ResumeJob(string jobName, string groupName)
        {
            _scheduler.ResumeJob(new JobKey(jobName, groupName));
        }

        public void TriggerJob(string jobName, string groupName)
        {
            _scheduler.TriggerJob(new JobKey(jobName, groupName));
        }
        public async Task UpdateJob(Model.Task task)
        {
            if (await IsJobExist(task.Name, task.JobGroup))
            {
                await DeleteJob(task.Name, task.JobGroup);
            }
            await AddJob(task);
        }
        public Task<bool> IsJobExist(string jobName, string groupName)
        {
            return _scheduler.CheckExists(new JobKey(jobName, groupName));
        }

        public async Task AddJob(Model.Task model)
        {
            if (await IsJobExist(model.Name, model.JobGroup))
                throw new ArgumentException(string.Format("Job name {0} Group Name {1} already exists", model.Name, model.JobGroup));

            if (!JobUtil.IsValidCronExpression(model.Cron))
                throw new ArgumentException(string.Format("Invalid Expression {0}", model.Cron));


            var jobDetail = JobBuilder.Create()
                .OfType(typeof(HttpRequestJob))
                .WithIdentity(model.Name, model.JobGroup)
                .StoreDurably(model.IsDeleted == 1)
                .RequestRecovery(model.IsDeleted == 1)
                .UsingJobData("TaskId", model.Id)
                .UsingJobData("Timeout", model.Timeout)
                .Build();

            //build trigger by cron expression
            var triggerBuilder = TriggerBuilder.Create()
                .WithIdentity(model.Name, model.JobGroup)
                .WithCronSchedule(model.Cron);


            var trigger = triggerBuilder.Build();

            await _scheduler.ScheduleJob(jobDetail, trigger);
        }

        #region 暂时没用
        public async Task ScheduleJob(string jobName, string groupName, string cronExpression)
        {
            if (!await IsJobExist(jobName, groupName))
                throw new ArgumentException(string.Format("Job name {0} Group Name {1} does not exist", jobName, groupName));

            if (!JobUtil.IsValidCronExpression(cronExpression))
                throw new ArgumentException(string.Format("Invalid Expression {0}", cronExpression));

            var jobDetail = await _scheduler.GetJobDetail(new JobKey(jobName, groupName));

            //build trigger by cron expression
            var trigger = TriggerBuilder.Create()
                .WithIdentity(jobName, groupName)
                .WithCronSchedule(cronExpression)
                .ForJob(jobDetail)
                .Build();

            await _scheduler.ScheduleJob(trigger);
        }

        public async Task<SchedulerInformation> GetMetaData()
        {
            var metaData = await _scheduler.GetMetaData();
            return new SchedulerInformation
            {
                SchedulerName = metaData.SchedulerName,
                InStandbyMode = metaData.InStandbyMode,
                Shutdown = metaData.Shutdown,
                JobStoreSupportsPersistence = metaData.JobStoreSupportsPersistence,
                NumberOfJobsExecuted = metaData.NumberOfJobsExecuted,
                RunningSince = metaData.RunningSince,
                SchedulerInstanceId = metaData.SchedulerInstanceId,
                SchedulerRemote = metaData.SchedulerRemote,
                SchedulerType = metaData.SchedulerType,
                Started = metaData.Started,
                ThreadPoolSize = metaData.ThreadPoolSize,
                ThreadPoolType = metaData.ThreadPoolType
            };
        }

        public async Task<List<Job>> GetCurrentlyExecutingJobs()
        {
            var executingJobs = await _scheduler.GetCurrentlyExecutingJobs();

            return (from job in executingJobs
                    let nextFireTime = job.Trigger.GetNextFireTimeUtc()
                    let previousFireTime = job.Trigger.GetPreviousFireTimeUtc()
                    select new Job
                    {
                        Name = job.JobDetail.Key.Name,
                        GroupName = job.JobDetail.Key.Group,
                        Triggers = new List<Trigger>
                    {
                        new Trigger
                        {
                            GroupName = job.Trigger.Key.Group,
                            Name = job.Trigger.Key.Name,
                            NextFireTime = nextFireTime.HasValue
                                    ? TimeZone.CurrentTimeZone.ToLocalTime(nextFireTime.Value.DateTime)
                                    : DateTime.MinValue,
                            PreviousFireTime = previousFireTime.HasValue
                                    ? TimeZone.CurrentTimeZone.ToLocalTime(previousFireTime.Value.DateTime)
                                    : DateTime.MinValue
                        }
                    }
                    }).ToList();
        }

        #endregion

        public async Task Init()
        {
            var taskSet = _db.Task.Where(s => s.IsDeleted == 0 && s.Status == 1).ToList();

            foreach (var item in taskSet)
            {
                await AddJob(item);

            }
            Start();
            //foreach (var item in taskSet.Where(s => s.Status != 1))
            //{
            //    this.PauseJob(item.Name, item.JobGroup);
            //}
        }

    }
}
