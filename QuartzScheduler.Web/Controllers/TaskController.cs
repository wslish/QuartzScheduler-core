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
using QuartzScheduler.Model;

namespace QuartzScheduler.Web.Controllers
{
    public class TaskController : Controller
    {
        TaskDbContext _db = null;
        JobScheduler scheduler;

        public TaskController(TaskDbContext db, JobScheduler iScheduler)
        {
            scheduler = iScheduler;
            _db = db;
        }

        public ActionResult Index(int? id, string name, string group, int? status, int page = 1, int pageSize = 10)
        {
            var query = _db.Task.Where(s => s.IsDeleted == 0);
            if (id.HasValue)
            {
                query = query.Where(s => s.Id == id);
            }
            if (status != -1)
            {
                query = query.Where(s => s.Status == status);
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(s => s.Name.Contains(name));
            }
            if (!string.IsNullOrWhiteSpace(group))
            {
                query = query.Where(s => s.JobGroup.Contains(group));
            }
            var total = query.Count();
            var result = query.OrderByDescending(s => s.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var PaginationInfo = new PaginationInfo()
            {
                ActualPage = page,
                ItemsPerPage = result.Count,
                TotalItems = total,
                TotalPages = int.Parse(Math.Ceiling(((decimal)total / pageSize)).ToString())
            };
            ViewBag.pager = PaginationInfo;
            return View(result);
        }

        public ActionResult Log(int? task_id, int? status, int page = 1, int pageSize = 10)
        {
            var query = _db.TaskLog.Where(s => true);
            if (task_id.HasValue)
            {
                query = query.Where(s => s.TaskId == task_id);
            }
            if (status > 0)
            {
                query = query.Where(s => s.Status == status);
            }

            var total = query.Count();
            var result = query.OrderByDescending(s => s.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var PaginationInfo = new PaginationInfo()
            {
                ActualPage = page,
                ItemsPerPage = result.Count,
                TotalItems = total,
                TotalPages = int.Parse(Math.Ceiling(((decimal)total / pageSize)).ToString())
            };
            ViewBag.pager = PaginationInfo;
            return View(result);
        }

        public ActionResult Run(int id)
        {
            var task = _db.Task.FirstOrDefault(s => s.Id == id);
            if (task != null)
            {
                scheduler.TriggerJob(task.Name, task.JobGroup);
            }
            return Json(new ApiResultModel<bool>());
        }
        [HttpPost]
        public ActionResult Pause(int id)
        {
            var task = _db.Task.FirstOrDefault(s => s.Id == id);
            if (task != null)
            {
                task.Status = 0;
                _db.SaveChanges();
                scheduler.PauseJob(task.Name, task.JobGroup);
            }
            return Json(new ApiResultModel<bool>());
        }

        [HttpPost]
        public async Task<ActionResult> Resume(int id)
        {
            var task = _db.Task.FirstOrDefault(s => s.Id == id);
            if (task != null)
            {
                task.Status = 1;
                _db.SaveChanges();
                if (await scheduler.IsJobExist(task.Name, task.JobGroup))
                {
                    scheduler.ResumeJob(task.Name, task.JobGroup);
                }
                else
                {
                    await scheduler.AddJob(task);
                }
            }
            return Json(new ApiResultModel<bool>());
        }

        [HttpPost]
        public ActionResult Remove(int id)
        {
            var task = _db.Task.FirstOrDefault(s => s.Id == id);
            if (task != null)
            {
                task.Status = 0;
                task.IsDeleted = 1;
                _db.SaveChanges();
                scheduler.DeleteJob(task.Name, task.JobGroup);
            }
            return Json(new ApiResultModel<bool>());
        }

        [HttpPost]
        public ActionResult ResumeAll()
        {
            scheduler.ResumeAll();
            return Json(new ApiResultModel<bool>());
        }

        [HttpPost]
        public ActionResult PauseAll()
        {
            scheduler.PauseAll();
            return Json(new ApiResultModel<bool>());
        }

        public ActionResult Create()
        {
            return View("task_form", new Model.Task());
        }
        public ActionResult Edit(int id)
        {
            var task = _db.Task.FirstOrDefault(s => s.Id == id && s.IsDeleted == 0);
            return View("task_form", task ?? new Model.Task());
        }
        [HttpPost]
        public async Task<ActionResult> Save(Model.Task model)
        {
            var result = new ApiResultModel<bool>();
            try
            {
                if (await scheduler.IsJobExist(model.Name, model.JobGroup) || _db.Task.Any(s => s.Name == model.Name && s.Id != model.Id))
                    throw new ArgumentException(string.Format("Job name {0} Group Name {1} already exists", model.Name, model.JobGroup));
                model.CreateTime = DateTime.Now;
                _db.Task.Add(model);
                _db.SaveChanges();
                if (model.Status == 1)
                {
                    await scheduler.UpdateJob(model);
                }
            }
            catch (Exception ex)
            {
                result.Code = 1;
                result.Message = ex.Message;
            }

            return Json(result);
        }

        #region Private methods

        #endregion
    }
}
