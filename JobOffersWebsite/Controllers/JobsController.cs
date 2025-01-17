﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JobOffersWebsite.Models;
using Microsoft.AspNet.Identity;

namespace JobOffersWebsite.Controllers
{
    [Authorize]
    public class JobsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var jobs = db.Jobs.Include(j => j.category);
            return View(jobs.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Job job = db.Jobs.Find(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            return View(job);
        }

        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "CategoryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Job job, HttpPostedFileBase upload)
        {
            if (upload != null)
            {
                string path = Path.Combine(Server.MapPath("~/Images"), upload.FileName);
                upload.SaveAs(path);
                job.JobImage = upload.FileName;
            }

            job.UserId = User.Identity.GetUserId();
            db.Jobs.Add(job);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");

            //ViewBag.CategoryId = new SelectList(db.Categories, "Id", "CategoryName", job.CategoryId);
            //return View(job);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Job job = db.Jobs.Find(id);

            if (job == null)
                return HttpNotFound();

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "CategoryName", job.CategoryId);
            return View(job);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Job job, HttpPostedFileBase upload)
        {
            //if (ModelState.IsValid)
            {
                string OldPath = Path.Combine(Server.MapPath("~/Images"), job.JobImage);

                if (upload != null)
                {
                    System.IO.File.Delete(OldPath);
                    string path = Path.Combine(Server.MapPath("~/Images"), upload.FileName);
                    upload.SaveAs(path);
                    job.JobImage = upload.FileName;
                }

                db.Entry(job).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "CategoryName", job.CategoryId);
            return View(job);
        }


        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Job job = db.Jobs.Find(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            return View(job);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Job job = db.Jobs.Find(id);
            db.Jobs.Remove(job);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
