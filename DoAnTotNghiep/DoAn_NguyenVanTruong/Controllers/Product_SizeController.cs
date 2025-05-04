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
    public class Product_SizeController : Controller
    {
        private BaseAppDbContext db = new BaseAppDbContext();

        // GET: Product_Size
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
            var product_sizes = db.Product_Size.ToList();
            return View(product_sizes.ToPagedList((int)page, (int)PageSize));
        }


        // GET: Product_Size/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product_Size product_Size = db.Product_Size.Find(id);
            if (product_Size == null)
            {
                return HttpNotFound();
            }
            return View(product_Size);
        }

        // GET: Product_Size/Create
        public ActionResult Create()
        {
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName");
            ViewBag.SizeID = new SelectList(db.Sizes, "SizeID", "NameSize");
            return View();
        }

        // POST: Product_Size/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Product_SizeID,ProductID,SizeID,Quantity")] Product_Size product_Size)
        {
            if (ModelState.IsValid)
            {
                Product_Size pro_Size = db.Product_Size.FirstOrDefault(s=>s.ProductID==product_Size.ProductID && s.SizeID==product_Size.SizeID);
                if(pro_Size == null)
                {
                    db.Product_Size.Add(product_Size);
                }
                else if(pro_Size!=null)
                {
                    pro_Size.Quantity += product_Size.Quantity;
                    db.Product_Size.Attach(pro_Size);
                    db.Entry(pro_Size).State = EntityState.Modified;
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", product_Size.ProductID);
            ViewBag.SizeID = new SelectList(db.Sizes, "SizeID", "NameSize", product_Size.SizeID);
            return View(product_Size);
        }

        // GET: Product_Size/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product_Size product_Size = db.Product_Size.Find(id);
            if (product_Size == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", product_Size.ProductID);
            ViewBag.SizeID = new SelectList(db.Sizes, "SizeID", "NameSize", product_Size.SizeID);
            return View(product_Size);
        }

        // POST: Product_Size/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Product_SizeID,ProductID,SizeID,Quantity")] Product_Size product_Size)
        {
            if (ModelState.IsValid)
            {
                var productchangeList = db.Product_Size
                    .Where(x => x.ProductID == product_Size.ProductID)
                    .ToList();

                if (productchangeList.Any())
                {
                    var productchange = productchangeList.First();
                    productchange.SizeID = 1;
                    product_Size.SizeID = product_Size.SizeID;
                    productchange.Quantity = product_Size.Quantity;

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("", "Không tìm thấy sản phẩm để cập nhật.");
            }

            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", product_Size.ProductID);
            ViewBag.SizeID = new SelectList(db.Sizes, "SizeID", "NameSize", product_Size.SizeID);
            return View(product_Size);
        }


        // GET: Product_Size/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product_Size product_Size = db.Product_Size.Find(id);
            if (product_Size == null)
            {
                return HttpNotFound();
            }
            return View(product_Size);
        }

        // POST: Product_Size/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product_Size product_Size = db.Product_Size.Find(id);
            db.Product_Size.Remove(product_Size);
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
