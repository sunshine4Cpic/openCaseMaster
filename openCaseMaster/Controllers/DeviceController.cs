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
    [Authorize(Roles = "admin")]
    public class DeviceController : Controller
    {
        // GET: Device
        public ActionResult DeviceManage()
        {
            return View();
        }


        public string deviceList(int page, int rows)
        {
            DeviceListModel pl = new DeviceListModel(page, rows);

            string json = JsonConvert.SerializeObject(pl);

            return json;

        }

        [HttpGet]
        public ActionResult editDevice(int id)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();
            var p = QC_DB.M_deviceConfig.First(t => t.ID == id);




            return PartialView("_editDevice", p);
        }


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
    }
}