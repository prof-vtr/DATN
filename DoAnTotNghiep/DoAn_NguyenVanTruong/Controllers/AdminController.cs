using DoAnNguyenVanTruong.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Authentication.Cookies;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.WebPages;

namespace DoAnNguyenVanTruong.Controllers
{
    public class AdminController : Controller
    {
        BaseAppDbContext db = new BaseAppDbContext();
        // GET: Admin
        public ActionResult Dashboard()
        {
            if (true)
            {
                DateTime[] data_date = new DateTime[7];
                double[] data_doanhthu = new double[7];
                string[] dates = new string[7];
                for (int x = 0; x <= 6; x++)
                {
                    int y = x - 5;
                    DateTime now = DateTime.Now.AddMonths(y);
                    data_date[x]=now;
                    dates[x]=  data_date[x].ToString("MM/yyyy");
                    
                }

                
                for (int i = 0; i <= 6; i++)
                {
                    DateTime date = data_date[i];
                    List<Order> orders = db.Orders.Where(
                        s =>  s.OrderDate.Month == date.Month && s.OrderDate.Year == date.Year && s.OrderStatus == "0"
                        ).ToList();
                    double doanh_thu_1_thang = 0;
                    if (orders != null)
                    {
                        doanh_thu_1_thang = orders.Sum(s => s.OrderTotalPrice);

                    }
                    data_doanhthu[i]=doanh_thu_1_thang;
                }
                ViewBag.lables = data_date;
                ViewBag.doanhthu = data_doanhthu;
                ViewBag.date = dates;
                return View();
               
            }
            else
            {
                return RedirectToAction("Login", "Page");
            }
        }
        public ActionResult OrderList(int? page, int? pagesize)
        {
            if (page == null)
            {
                page = 1;
            }
            if (pagesize == null)
            {

                pagesize = 8;
            }
            var orders = db.Orders.ToList();
            return View(orders.ToPagedList((int)page, (int)pagesize));
        }
        public ActionResult DetailOrder(int id)
        {
            List<Order> orders = db.Orders.Where(s => s.OrderID == id).ToList();
            ViewBag.order = orders;
            Order code = db.Orders.FirstOrDefault(s => s.OrderID == id);
            ViewBag.code = code;
            var orderdetail = db.OrderDetails.Where(s => s.OrderID == id).ToList();
            DateTime datetime = code.OrderDate.AddDays(3);
            string time = datetime.ToString("dd / MM / yy");
            ViewBag.timegiao = time;
            return View(orderdetail);
        }
        public PartialViewResult total_quantity_order()
        {
            ViewBag.total_quantity_order = db.Orders.Count();
            return PartialView("total_quantity_order");
        }
        public PartialViewResult total_price_order()
        {
            double total;
            //List<Order> orders = db.Orders.ToList();
            //foreach(var item in orders)
            //{  
            var now = DateTime.Now.Month;
            List<Order> list = db.Orders.Where(s => s.OrderDate.Month == now && s.OrderStatus=="0").ToList();
            if (list != null)
            {
                total = list.Sum(s => s.OrderTotalPrice);
            }
            else
            {
                total = 0;
            }
            //}
            ViewBag.totalprice = total;
            return PartialView("total_price_order");
        }
        public PartialViewResult total_User()
        {
            ViewBag.total_user = db.Users.Count();
            return PartialView("total_User");
        }
        public PartialViewResult Total_product()
        {
            ViewBag._total_product = db.Products.Count();
            return PartialView("Total_product");
        }
        public ActionResult Confirm_Order(int id)
        {
            Order order = db.Orders.FirstOrDefault(s => s.OrderID == id);
            order.OrderStatus = "2";
            db.Orders.Attach(order);
            db.Entry(order).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("OrderList", "Admin");
        }
        public ActionResult Ship(int id)
        {
            Order order = db.Orders.FirstOrDefault(s => s.OrderID == id);
            order.OrderStatus = "3";
            db.Orders.Attach(order);
            db.Entry(order).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("OrderList", "Admin");
        }
        [HttpPost]
        public ActionResult Select_Order(FormCollection form, int? page, int? pagesize)
        {
            string status =form["Select_order"];
            if (page == null)
            {
                page = 1;
            }
            if (pagesize == null)
            {

                pagesize = 8;
            }
            List<Order> orders = db.Orders.Where(s => s.OrderStatus == status).ToList();
            return View(orders.ToPagedList((int)page, (int)pagesize));
        }
        
        
        
    }
}