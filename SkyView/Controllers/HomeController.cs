using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SkyView.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        // 俯瞰桃園之美
        public ActionResult OverlookAll()
        {
            return View();
        }
        public ActionResult OverlookMap()
        {
            return View();
        }
        public ActionResult OverlookContent()
        {
            return View();
        }

        // 360環景
        public ActionResult SweepingList()
        {
            return View();
        }
        public ActionResult SweepingContent()
        {
            return View();
        }

        // 關於本站
        public ActionResult AboutUs()
        {
            return View();
        }

        // 網站導覽
        public ActionResult SiteMap()
        {
            return View();
        }
    }
}