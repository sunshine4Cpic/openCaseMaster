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
    
    public class DeviceController : Controller
    {
        [Authorize(Roles = "admin")]
        // GET: Device
        public ActionResult DeviceManage()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        public string deviceList(int page, int rows)
        {
            DeviceListModel pl = new DeviceListModel(page, rows);

            string json = JsonConvert.SerializeObject(pl);

            return json;

        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public ActionResult editDevice(int id)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();
            var p = QC_DB.M_deviceConfig.First(t => t.ID == id);




            return PartialView("_editDevice", p);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public string editDevice(int ID, string mark, string Model, string Brand)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();
            var u = QC_DB.M_deviceConfig.First(t => t.ID == ID);
            u.mark = mark;
            u.Model = Model;
            u.Brand = Brand;
            
            QC_DB.SaveChanges();

            return "";
        }


        [Authorize(Roles = "user")]
        [OutputCache(Duration = 60)]
        public ActionResult DeviceView()
        {
            QCTESTEntities QC_DB = new QCTESTEntities();

            var rtc = from t in QC_DB.M_runTestCase
                      select t;

            var ss = (from t in QC_DB.M_deviceConfig
                     select new DeviceViewModel
                     {
                         ID = t.ID,
                         mark = t.mark,
                         Model = t.Model,
                         //img = t.img == null ? "default.jpg" : t.img,
                         run = rtc.Where(r => r.M_runScene.deviceID == t.ID && r.state != null).Count(),
                         runing = rtc.Where(r => r.M_runScene.deviceID == t.ID && r.M_runScene.M_testDemand.isRun == true && r.state == null).Count()
                     }).ToList();


            string json = JsonConvert.SerializeObject(ss);


            ViewBag.data = json;
            return View();
        }
    }
}