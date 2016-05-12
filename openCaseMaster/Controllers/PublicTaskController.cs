using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using openCaseMaster.Models;
using openCaseMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace openCaseMaster.Controllers
{
    [Authorize(Roles = "user")]
    public class PublicTaskController : Controller
    {

        [AllowAnonymous]
        public ActionResult Index(int page=1)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();

            var lsv = from t in QC_DB.M_publicTask
                      select new taskModel_view
                      {
                          ID = t.ID,
                          title=t.title,
                          appName=t.M_application.name,
                          userName = t.admin_user.Username,
                          creatDate = t.creatDate,
                          scriptCount = t.M_publicTaskScript.Count
                      };
            ViewBag.select = "Index";
            ViewBag.page = page;
            return View(lsv.ToList());
        }

        [HttpGet]
        public ActionResult add()
        {

            SelectListGroup slg = new SelectListGroup();
            slg.Name = "可选应用";

            var apps = (from t in userHelper.getApps()
                       select new SelectListItem
                       {
                           Text = t.name,
                           Value = t.ID.ToString(),
                           //Group = slg
                       }).ToList();

            
            apps.ForEach(t => t.Group = slg);
            apps.Insert(0, new SelectListItem { Text = "请选择被测应用", Value = "" });

           // var apps = userHelper.getApps().ToDictionary(k => k.ID, v => v.name);
            ViewBag.apps = apps;

            return View();
        }

        [HttpPost]
        public ActionResult add(taskModel_add tm)
        {
           

            if (!ModelState.IsValid)
            {
                var apps = userHelper.getApps().ToDictionary(k => k.ID, v => v.name);
                ViewBag.apps = apps;

                return View(tm);
            }
            QCTESTEntities QC_DB = new QCTESTEntities();

            M_publicTask pt = new M_publicTask();
            pt.appID = tm.appID;
            pt.creatDate = DateTime.Now;
            pt.title = tm.title;
            pt.body = tm.body;
            pt.userID = userHelper.getUserID();

            QC_DB.M_publicTask.Add(pt);

            //解析json

            var ja = JArray.Parse(tm.scripts);
            foreach (var j in ja.Children<JObject>())
            {

                int ID = Convert.ToInt32(j["ID"].ToString());
                var tmp = QC_DB.tmp_TaskScript.FirstOrDefault(t => t.ID == ID);
                if (tmp == null) continue;

                M_publicTaskScript ts = new M_publicTaskScript();
                ts.taskID = pt.ID;
                ts.title = tmp.title;
                ts.script = tmp.script;

                QC_DB.M_publicTaskScript.Add(ts);
                QC_DB.tmp_TaskScript.Remove(tmp);
            }
            QC_DB.SaveChanges();

            TempData["add"] = true;

            return RedirectToAction(pt.ID.ToString(), "PublicTask");
            
        }

        [Route("{control}/{id:int}")]
        public ActionResult Task(int id)
        {
            if (TempData["add"] != null)
                ViewBag.add = true;
            taskModel_view tv = new taskModel_view(id);

            return View(tv);
        }


      


    }
}