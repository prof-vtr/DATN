using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace DoAn_TranTheAnh_2020607026.Controllers
{
    public class AdminController : Controller
    {
        Fashion db = new Fashion();
        // GET: Admin
        public ActionResult Dashboard()
        {
            return View();
        }
        
    }
}