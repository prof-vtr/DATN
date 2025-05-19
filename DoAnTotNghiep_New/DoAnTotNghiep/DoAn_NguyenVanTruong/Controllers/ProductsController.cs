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
    public class ProductsController : Controller
    {
        private BaseAppDbContext db = new BaseAppDbContext();

        // GET: Products
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
            var products = db.Products.ToList();
            return View(products.ToPagedList((int)page, (int)PageSize));
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.BrandID = new SelectList(db.Brands, "BrandID", "BrandName");
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,ProductName,Description,Images,SaleOff,Price,CategoryID,BrandID")] Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var f = Request.Files["ImageFile"];
                    if (f != null && f.ContentLength > 0)
                    {
                        string filename = System.IO.Path.GetFileName(f.FileName);
                        string Upload = Server.MapPath("~/assets/images/product/" + f.FileName);
                        f.SaveAs(Upload);
                        product.Images = filename;
                    }
                    var productsize = new Product_Size();
                    productsize.ProductID = product.ProductID;
                    productsize.SizeID = 1;
                    productsize.Product_SizeID = 1;
                    productsize.Quantity = 1;
                    db.Products.Add(product);
                    db.Product_Size.Add(productsize);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.BrandID = new SelectList(db.Brands, "BrandID", "BrandName", product.BrandID);
                ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
                return View(product);
            }
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.BrandID = new SelectList(db.Brands, "BrandID", "BrandName", product.BrandID);
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,ProductName,Description,Images,SaleOff,Price,CategoryID,BrandID")] Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var f = Request.Files["ImageFile"];
                    if (f != null && f.ContentLength > 0)
                    {
                        string filename = System.IO.Path.GetFileName(f.FileName);
                        string Upload = Server.MapPath("~/assets/images/product/" + f.FileName);
                        f.SaveAs(Upload);
                        product.Images = filename;
                    }
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();

                }
                return RedirectToAction("Index");
            }

            catch
            {
                ViewBag.BrandID = new SelectList(db.Brands, "BrandID", "BrandName", product.BrandID);
                ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
                return View(product);
            }
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            List<Rate> rates = db.Rates.Where(s => s.ProductID == id).ToList();
            if (rates != null)
            {
                foreach (var item in rates)
                {
                    db.Rates.Remove(item);
                }
            }
            db.Products.Remove(product);
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
