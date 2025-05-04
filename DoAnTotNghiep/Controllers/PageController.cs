using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAn_TranTheAnh_2020607026.Controllers
{
    public class PageController : Controller
    {
        Fashion db = new Fashion();

        // GET: Page
        public ActionResult Index()
        {
           
            return View();
        }
        
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string Email, string Password)
        {
            var User = db.Users.Where(s=>s.Email==Email && s.Password ==Password).FirstOrDefault();
            if (User != null   )
            {
                Session["UserID"] = User.UserID;
                if(User.RoleID == 0)
                {
                    return RedirectToAction("Index", "Page");
                }
                else if(User.RoleID == 1)
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
                
            }
            return View();
            
        }

    }
}