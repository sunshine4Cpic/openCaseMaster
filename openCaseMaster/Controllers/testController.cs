using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace openCaseMaster.Controllers
{
    public class testController : Controller
    {
        // GET: test
        public string Index()
        {
            Dictionary<string, string> dd = new Dictionary<string, string>();
            dd.Add("A1", "1");
            dd.Add("A1", "2");
            dd.Add("A1", "3");
            dd.Add("A2", "4");
            dd.Add("A1", "5");
            dd.Add("A2", "6");
            dd.Add("A3", "7");
            return "";
        }
    }
}