﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace openCaseMaster.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.menu = false;
            return View();
        }


       

       
    }
}