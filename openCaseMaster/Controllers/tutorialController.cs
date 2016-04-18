using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace openCaseMaster.Controllers
{
    public class tutorialController : Controller
    {
        //后期帮助文档可以借助 markdownsharp 来实现.
        // GET: tutorial
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult framework()
        {
            return View();
        }

        
    }
}