//using System;
//using System.Collections.Generic;
//using Quartz;
//using QuartzScheduler.Infrastructure.Models;
//using System.Threading.Tasks;

//namespace QuartzScheduler.Infrastructure.Interfaces
//{
//    public interface IJobScheduler
//    {
//        void Start();
//        void Shutdown(bool waitForJobsToComplete);
//        void StandBy();
//        void PauseAll();
//        void ResumeAll();
//        Task<List<Job>> GetAllJobs();
//        IScheduler GetScheduler();
//        bool UnscheduleJob(string triggerName, string triggerGroup);
//        void ScheduleJob(string jobName, string groupName, string cronExpression);
//        bool DeleteJob(string jobName, string jobGroup);
//        void PauseJob(string jobName, string groupName);
//        void ResumeJob(string jobName, string groupName);
//        void TriggerJob(string jobName, string groupName);
//        void ResumeTrigger(string triggerName, string groupName);
//        void PauseTrigger(string triggerName, string groupName);
//        bool IsJobExist(string jobName, string groupName);

//        void AddJob(Model.Task model);
//        SchedulerInformation GetMetaData();
//        Task<List<Job>> GetCurrentlyExecutingJobs();
//    }
//}
