using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DoAnNguyenVanTruong.Models;
using PagedList;

namespace DoAnNguyenVanTruong.Controllers
{
    public class SlidesController : Controller
    {
        private BaseAppDbContext db = new BaseAppDbContext();

        // GET: Slides
        public ActionResult Index(int? page, int? PageSize)
        {
            if (page == null)
            {
                page = 1;
            }
            if (PageSize == null)
            {

                PageSize = 8;
            }
            var slides = db.Slides.ToList();
            return View(slides.ToPagedList((int)page, (int)PageSize));
        }
        // GET: Slides/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Slide slide = db.Slides.Find(id);
            if (slide == null)
            {
                return HttpNotFound();
            }
            return View(slide);
        }

        // GET: Slides/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Slides/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SlideID,Images")] Slide slide)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var f = Request.Files["ImageFile"];
                    if (f != null && f.ContentLength > 0)
                    {
                        string filename = System.IO.Path.GetFileName(f.FileName);
                        string Upload = Server.MapPath("~/assets/images/slide/" + f.FileName);
                        f.SaveAs(Upload);
                        slide.Images = filename;
                    }
                    db.Slides.Add(slide);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch
            {

                return View(slide);
            }
        }

        // GET: Slides/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Slide slide = db.Slides.Find(id);
            if (slide == null)
            {
                return HttpNotFound();
            }
            return View(slide);
        }

        // POST: Slides/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SlideID,Images")] Slide slide)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var f = Request.Files["ImageFile"];
                    if (f != null && f.ContentLength > 0)
                    {
                        string filename = System.IO.Path.GetFileName(f.FileName);
                        string Upload = Server.MapPath("~/assets/images/slide/" + f.FileName);
                        f.SaveAs(Upload);
                        slide.Images = filename;
                    }
                    db.Entry(slide).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            catch
            {

                return View(slide);
            }
        }

        // GET: Slides/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Slide slide = db.Slides.Find(id);
            if (slide == null)
            {
                return HttpNotFound();
            }
            return View(slide);
        }

        // POST: Slides/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Slide slide = db.Slides.Find(id);
            db.Slides.Remove(slide);
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
