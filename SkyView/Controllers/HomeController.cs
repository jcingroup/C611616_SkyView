﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUser.Controller;
using SkyView.Service;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Specialized;
/*Json.NET相關的命名空間*/
using Newtonsoft.Json;

namespace SkyView.Controllers
{
    public class HomeController : WebUserController
    {
        OverlookDBService OverlookDB = new OverlookDBService();
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
        public ActionResult OverlookMap(string area_id = "")
        {
            DataTable d_area;
            DataTable d_lookList;
            //抓取中心資料
            d_area = OverlookDB.AreaList(area_id);
            //抓取景觀資料
            d_lookList = OverlookDB.List("", "", area_id);
            //抓取小圖

            ViewBag.d_area = d_area;
            ViewBag.d_lookList = d_lookList;
            ViewBag.MapCurrent = area_id;

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