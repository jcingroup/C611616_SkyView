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

        // 空拍-l1 桃園市地圖
        public ActionResult OverlookArea()
        {
            return View();
        }
        // 空拍-l2 區域景點列表(google地圖)
        public ActionResult OverlookMap()
        {
            return View();
        }
        // 空拍-l3 景點介紹
        public ActionResult Overlook()
        {
            return View();
        }

        // 360環景
        public ActionResult Overview()
        {
            return View();
        }

        // 關於本站
        public ActionResult AboutUs()
        {
            return View();
        }

        // 網站導覽
        public ActionResult Sitemap()
        {
            return View();
        }
    }
}