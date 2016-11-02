using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUser.Controller;

namespace SkyView.Controllers
{
    public class HomeController : WebUserController
    {
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.IsFirstPage = true;
            return View();
        }
        // 導向後台首頁(登入頁)
        public RedirectResult Login()
        {
            return Redirect("~/Manage");
        }
    }
}