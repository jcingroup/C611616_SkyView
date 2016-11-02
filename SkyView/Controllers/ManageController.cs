using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUser.Controller;

namespace SkyView.Controllers
{
    public class ManageController : WebUserController
    {
        // 後台首頁-導向Login
        public ActionResult Index()
        {
            return View("Login");
        }

        // 後台登入
        public ActionResult Login()
        {
            return View();
        }

        // 空拍
        public ActionResult OverlookList()
        {
            return View();
        }
        public ActionResult OverlookData()
        {
            return View();
        }

        // 環景
        public ActionResult OverviewList()
        {
            return View();
        }
        public ActionResult OverviewData()
        {
            return View();
        }

        // 變更密碼
        public ActionResult ChangePW()
        {
            return View();
        }
    }
}