using openCaseMaster.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using openCaseMaster.ViewModels;


namespace openCaseMaster.Controllers
{
    [Authorize(Roles="admin")]
    public class applicationController : Controller
    {
        // GET: application
        public ActionResult Index()
        {
            return View();
        }

        // GET: application
        public ActionResult apps(int page, int rows)
        {

            QCTESTEntities QC_DB = new QCTESTEntities();

            var us = from t in QC_DB.M_application
                     orderby t.ID
                     select new applicationModel
                     {
                         id = t.ID,
                         name = t.name,
                         runApkName = t.runApkName,
                         package = t.package,
                         mainActiviy = t.mainActiviy,
                         package2 = t.package2,
                         isClear = t.isClear
                     };

            applicationListModel md = new applicationListModel();
            md.total = us.Count();

            md.rows = us.Skip(rows * (page - 1)).Take(rows).ToList();


            return Json(md, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult edit(int id)
        {

            QCTESTEntities QC_DB = new QCTESTEntities();
            var app = QC_DB.M_application.First(t => t.ID == id);

            var md = new applicationModel();
            md.id = app.ID;
            md.isClear = app.isClear;
            md.mainActiviy = app.mainActiviy;
            md.name = app.name;
            md.package = app.package;
            md.package2 = app.package2;
            md.runApkName = app.runApkName;


            return Json(md, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public string edit(int id,applicationModel md)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 500;
                return "非法提交";
            }
            QCTESTEntities QC_DB = new QCTESTEntities();


            var app = QC_DB.M_application.First(t => t.ID == id);
            app.isClear = md.isClear;
            app.mainActiviy = md.mainActiviy;
            app.name = md.name;
            app.package = md.package;
            app.package2 = md.package2;
            app.runApkName = md.runApkName;

            QC_DB.SaveChanges();
            return null;
        }


        [HttpPost]
        public ActionResult add(applicationModel md)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 500;
                ViewBag.err = "非法提交";
                return View("Error");
            }
            QCTESTEntities QC_DB = new QCTESTEntities();


            var app =  new M_application();
            app.isClear = md.isClear;
            app.mainActiviy = md.mainActiviy;
            app.name = md.name;
            app.package = md.package;
            app.package2 = md.package2;
            app.runApkName = md.runApkName;

            QC_DB.M_application.Add(app);

            QC_DB.SaveChanges();
            return RedirectToAction("index");
        }
    }
}