using Newtonsoft.Json;
using openCaseMaster.Models;
using openCaseMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MarkdownSharp;

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

        [HttpPost]
        [ValidateInput(false)]
        public string markdown(string body)
        {
            Markdown md = new Markdown();
            return md.Transform(body);
        }


    }
}