using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuartzScheduler.Web.Models;
using QuartzScheduler.Web.Utils;
using QuartzScheduler.Infrastructure.Utils;
using QuartzScheduler.Infrastructure.Models;
using QuartzScheduler.Infrastructure;

namespace QuartzScheduler.Web.Controllers
{
    public class HomeController : Controller
    {
        TaskDbContext _db = null;
        JobScheduler scheduler;

        public HomeController(TaskDbContext db, JobScheduler iScheduler)
        {
            scheduler = iScheduler;
            _db = db;
        }

        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult JobList()
        {
            var jobs = GetAllJobs();
            return PartialView(jobs);
        }

        [HttpPost]
        public ActionResult PauseJob(string name, string group)
        {
            scheduler.PauseJob(name, group);
            return RedirectToAction("JobList");
        }

        [HttpPost]
        public ActionResult ResumeJob(string name, string group)
        {
            scheduler.ResumeJob(name, group);
            return RedirectToAction("JobList");
        }

        [HttpPost]
        public ActionResult RemoveJob(string name, string group)
        {
            scheduler.DeleteJob(name, group);
            return RedirectToAction("JobList");
        }

        [HttpPost]
        public ActionResult ResumeAll()
        {
            scheduler.ResumeAll();
            return RedirectToAction("JobList");
        }

        [HttpPost]
        public ActionResult PauseAll()
        {
            scheduler.PauseAll();
            return RedirectToAction("JobList");
        }

        public ActionResult AddJob()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> AddJob(Model.Task model)
        {
            try
            {
                if (await scheduler.IsJobExist(model.Name, model.JobGroup))
                    throw new ArgumentException(string.Format("Job name {0} Group Name {1} already exists", model.Name, model.JobGroup));
                _db.Task.Add(model);
                _db.SaveChanges();
                await scheduler.AddJob(model);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseMessage = ex.Message });
            }

            return Json(new { success = true });
        }

        #region Private methods

        private async Task<List<JobTriggerViewModel>> GetAllJobs()
        {
            var jobTriggers = new List<JobTriggerViewModel>();
            try
            {
                var jobs = await scheduler.GetAllJobs();
                foreach (var job in jobs)
                {
                    jobTriggers.AddRange(job.Triggers.Select(trigger => new JobTriggerViewModel
                    {
                        JobName = job.Name,
                        JobGroup = job.GroupName,
                        TriggerName = trigger.Name,
                        TriggerGroup = trigger.GroupName,
                        State = trigger.State,
                        NextFireTime = trigger.NextFireTime,
                        PreviousFireTime = trigger.PreviousFireTime,
                        CronExpression = trigger.CronExpression,
                        Properties =
                            trigger.DataMap != null
                                ? string.Join(";", trigger.DataMap.Select(x => x.Key + "=" + x.Value).ToArray())
                                : string.Empty
                    }));
                }
            }
            catch (Exception ex)
            {
            }
            return jobTriggers;
        }
        #endregion
    }
}
