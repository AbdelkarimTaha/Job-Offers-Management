﻿using JobOffersWebsite.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var Categories = db.Categories.ToList();

            return View(Categories);
        }

        public ActionResult Details(int JobId)
        {
            var job = db.Jobs.Find(JobId);

            if (job == null)
                return HttpNotFound();

            Session["JobId"] = JobId;

            return View(job);
        }

        [Authorize]
        public ActionResult Apply()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Apply(string Message)
        {
            var UserId = User.Identity.GetUserId();
            var JobId = (int)Session["JobId"];

            var check = db.ApplyForJobs.Where(a => a.UserId == UserId && a.JobId == JobId).ToList();

            if (check.Count == 0)
            {
                ApplyForJob job = new ApplyForJob();
                job.Message = Message;
                job.ApplyDate = DateTime.Now;
                job.UserId = UserId;
                job.JobId = JobId;

                db.ApplyForJobs.Add(job);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult GetJobsByUser()
        {
            var UserId = User.Identity.GetUserId();
            var jobs = db.ApplyForJobs.Where(a => a.UserId == UserId).ToList();

            return View(jobs);
        }

        [Authorize]
        public ActionResult DetailsOfJob(int id)
        {
            var job = db.ApplyForJobs.Find(id);

            if (job == null)
                return HttpNotFound();

            return View(job);
        }

        public ActionResult Edit(int id)
        {
            var job = db.ApplyForJobs.Find(id);

            if (job == null)
                return HttpNotFound();

            return View(job);
        }

        [HttpPost]
        public ActionResult Edit(ApplyForJob job)
        {
            if (ModelState.IsValid)
            {
                db.Entry(job).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("GetJobsByUser");
            }

            return View(job);
        }

        public ActionResult Delete(int id)
        {
            var job = db.ApplyForJobs.Find(id);

            if (job == null)
                return HttpNotFound();

            return View(job);
        }

        [HttpPost]
        public ActionResult Delete(ApplyForJob job)
        {
            if (ModelState.IsValid)
            {
                var JobInDb = db.ApplyForJobs.Find(job.ID);

                db.ApplyForJobs.Remove(JobInDb);
                db.SaveChanges();

                return RedirectToAction("GetJobsByUser");
            }

            return View();
        }

        [Authorize]
        public ActionResult GetJobsByPublisher()
        {
            var UserId = User.Identity.GetUserId();

            var jobs = from apply in db.ApplyForJobs
                       join job in db.Jobs
                       on apply.JobId equals job.Id
                       where job.User.Id == UserId
                       select apply;

            var group = from j in jobs
                        group j by j.Job.JobTitle
                        into gr
                        select new JobsViewModel
                        {
                            JobTitle = gr.Key,
                            items = gr
                        };

            return View(group.ToList());
        }

        public ActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search(string searchName)
        {
            var SearchResult = db.Jobs.Where(j => j.JobTitle.Contains(searchName)
                                         || j.JobContent.Contains(searchName)
                                         || j.category.CategoryName.Contains(searchName)
                                         || j.category.CategoryDescription.Contains(searchName)).ToList();

            return View(SearchResult);
        }
    }
}