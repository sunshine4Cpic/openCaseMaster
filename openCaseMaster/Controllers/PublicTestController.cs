using Newtonsoft.Json;
using openCaseMaster.Models;
using openCaseMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace openCaseMaster.Controllers
{
    public class PublicTestController : Controller
    {
        // GET: PublicTest
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult add()
        {
            return View();
        }


    }
}