using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using openCaseMaster.ViewModels;
using Newtonsoft.Json;
using openCaseMaster.Models;

namespace openCaseMaster.Controllers
{
    [Authorize(Roles = "admin")]
    public class ProjectController : Controller
    {
       
        // GET: Project
        public ActionResult ProjectManage()
        {
            return View();
        }


        
        public string projectList(int page, int rows)
        {
            ProjectListModel pl = new ProjectListModel(page, rows);

            string json = JsonConvert.SerializeObject(pl);

            return json;

        }

       
        [HttpGet]
        public ActionResult editProject(int id)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();
            var p = QC_DB.project.First(t => t.ID == id);


            var deviceList = from t1 in QC_DB.M_deviceConfig
                             join t2 in QC_DB.M_DevProMapping.Where(t => t.PID == id)
                            on t1.ID equals t2.DeviceID into Joined
                             from t2 in Joined.DefaultIfEmpty()
                             select new checkListModel
                             {
                                 Value = t1.ID.ToString(),
                                 Text = t1.mark,
                                 isCheck = t2 != null ? t2.usable : false
                             };

            var appList = from t1 in QC_DB.M_application
                             join t2 in QC_DB.project_app.Where(t => t.PID == id)
                            on t1.ID equals t2.appID into Joined
                             from t2 in Joined.DefaultIfEmpty()
                             select new checkListModel
                             {
                                 Value = t1.ID.ToString(),
                                 Text = t1.name,
                                 isCheck = t2 != null ? t2.usable : false
                             };


            ViewData["deviceList"] = deviceList.ToList();
            ViewData["appList"] = appList.ToList();



            return PartialView("_editProject", p);
        }

        
        [HttpPost]
        public string editProject(int ID, string Pname, string deviceList, string appList)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();
            var u = QC_DB.project.First(t => t.ID == ID);
            u.Pname = Pname;

           
            //设备
            

            var dpm = QC_DB.M_DevProMapping.Where(t => t.PID == ID);

            List<string> deviceID;
            if (string.IsNullOrEmpty(deviceList.TrimEnd(',')))
                deviceID = new List<string>();
            else
                deviceID = deviceList.TrimEnd(',').Split(',').ToList();

            foreach(var d in dpm)
            {
                if(deviceID.Contains(d.DeviceID.ToString()))
                {
                    d.usable = true;
                    deviceID.Remove(d.DeviceID.ToString());
                }else
                    d.usable = false;
            }

            foreach (var d in deviceID)
            {
                M_DevProMapping temp = new M_DevProMapping();
                temp.PID = ID;
                temp.DeviceID = Convert.ToInt32(d);
                temp.usable = true;
                QC_DB.M_DevProMapping.Add(temp);
            }

            //app
         

            var papp = QC_DB.project_app.Where(t => t.PID == ID);

            List<string> appID;
            if (string.IsNullOrEmpty(appList.TrimEnd(',')))
                appID = new List<string>();
            else
                appID = appList.TrimEnd(',').Split(',').ToList();

            foreach (var p in papp)
            {
                if (appID.Contains(p.appID.ToString()))
                {
                    p.usable = true;
                    appID.Remove(p.appID.ToString());
                }
                else
                    p.usable = false;
            }

            foreach (var p in appID)
            {
                project_app temp = new project_app();
                temp.PID = ID;
                temp.appID = Convert.ToInt32(p);
                temp.usable = true;
                QC_DB.project_app.Add(temp);
            }


            QC_DB.SaveChanges();

            return "";
        }

        [HttpGet]
        public ActionResult addProject()
        {
            QCTESTEntities QC_DB = new QCTESTEntities();

            var deviceList = from t1 in QC_DB.M_deviceConfig
                             select new checkListModel
                             {
                                 Value = t1.ID.ToString(),
                                 Text = t1.mark
                             };

            var appList = from t1 in QC_DB.M_application
                          select new checkListModel
                          {
                              Value = t1.ID.ToString(),
                              Text = t1.name
                          };


            ViewData["deviceList"] = deviceList.ToList();
            ViewData["appList"] = appList.ToList();


            return PartialView("_editProject", new project());
        }

        [HttpPost]
        public string addProject(string Pname, string deviceList, string appList)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();
            project pro = new project();
            pro.Pname = Pname;
            pro.zidonghua = true;

            QC_DB.project.Add(pro);
            //设备


           

            List<string> deviceID;
            if (string.IsNullOrEmpty(deviceList.TrimEnd(',')))
                deviceID = new List<string>();
            else
                deviceID = deviceList.TrimEnd(',').Split(',').ToList();


            foreach (var d in deviceID)
            {
                M_DevProMapping temp = new M_DevProMapping();
                temp.PID = pro.ID;
                temp.DeviceID = Convert.ToInt32(d);
                temp.usable = true;
                QC_DB.M_DevProMapping.Add(temp);
            }

            //app


          

            List<string> appID;
            if (string.IsNullOrEmpty(appList.TrimEnd(',')))
                appID = new List<string>();
            else
                appID = appList.TrimEnd(',').Split(',').ToList();

           

            foreach (var p in appID)
            {
                project_app temp = new project_app();
                temp.PID = pro.ID;
                temp.appID = Convert.ToInt32(p);
                temp.usable = true;
                QC_DB.project_app.Add(temp);
            }


            QC_DB.SaveChanges();

            return "";
        }

    }
}