using M_runClient;
using Newtonsoft.Json;
using openCaseMaster.Models;
using openCaseMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace openCaseMaster.Controllers
{
    [Authorize(Roles = "user")]
    public class RunCaseController : Controller
    {
        // GET: RunCase
        public ActionResult Index()
        {
            return View();
        }


        public string folderExpanded(int ID)
        {
            using (QCTESTEntities QC_DB = new QCTESTEntities())
            {


                var tcl = from t in QC_DB.M_testDemand
                          where t.baseID == ID && t.visable!=false
                          orderby t.type
                          select new testDemandTree
                          {
                              iconCls = t.type == 0 ? null : "icon-application_windows",
                              DemandID = t.ID,
                              text = t.name,
                              //state = "close",
                              state = t.type == 0 ? "closed" : "open",
                              type = t.type == null ? 0 : t.type.Value
                          };

                var jSetting = new JsonSerializerSettings();
                jSetting.NullValueHandling = NullValueHandling.Ignore;

                string json = JsonConvert.SerializeObject(tcl, jSetting);

                return json;
            }
        }


        public string getFileByProject(int ID)
        {
            using (QCTESTEntities QC_DB = new QCTESTEntities())
            {
                var tcl = from t in QC_DB.M_testDemand
                          where t.PID == ID && t.visable != false
                          && t.baseID == null
                          orderby t.type
                          select new testDemandTree
                          {
                              DemandID = t.ID,
                              text = t.name,
                              //state = "closed",//在tree里显示场景
                              state = t.type == 0 ? "closed" : "open",
                              iconCls = t.type == 0 ? null : "icon-application_windows",
                              type = t.type == null ? 0 : t.type.Value
                          };

                var jSetting = new JsonSerializerSettings();
                jSetting.NullValueHandling = NullValueHandling.Ignore;

                string json = JsonConvert.SerializeObject(tcl, jSetting);

                return json;
            }
        }

        public string DemandScene(int ID)
        {
            using (QCTESTEntities QC_DB = new QCTESTEntities())
            {
                var tcl = from t in QC_DB.M_runScene
                          where t.DemandID==ID
                          select new testSceneTree
                          {
                              id = t.ID,
                              DemandID = t.DemandID,
                              text = t.name,
                              state = "open",
                              iconCls = "icon-movie_grey"
                          };

                var jSetting = new JsonSerializerSettings();
                jSetting.NullValueHandling = NullValueHandling.Ignore;

                string json = JsonConvert.SerializeObject(tcl, jSetting);

                return json;
            }
        }

        public ActionResult DemandView(int ID)
        {
           
            DemandViewModel dv = new DemandViewModel(ID);
            return PartialView("_DemandView", dv);
        }

       
        [HttpPost]
        public string runDemand(int ID)
        {
            
            QCTESTEntities QC_DB = new QCTESTEntities();
            int cnt = QC_DB.M_runScene.Where(t => t.DemandID == ID && t.deviceID == null).Count();
            if (cnt == 0)
            {
                M_testDemand td = QC_DB.M_testDemand.Where(t => t.ID == ID).First();
                td.isRun = true;
                QC_DB.SaveChanges();
                //开始执行咯

                var machines = from t in QC_DB.M_deviceConfig.Distinct()
                               join t1 in QC_DB.M_runScene on t.ID equals t1.deviceID
                               join t2 in QC_DB.M_testDemand on t1.DemandID equals t2.ID
                               where t2.ID == ID
                               select t.IP + ":" + t.Port;

                List<string> urls = machines.Distinct().ToList();
                string msg = null;
                foreach (var url in urls)
                {
                    if (!Call_Client.startRun(url))
                    {
                        msg += (url + ",");

                    }
                }
                if (msg != null)
                    return "执行机:" + msg + "发生错误!";
                else
                    return "开始执行";
            }
            else
            {
                return "有场景未选择执行设备";
            }
        }

       [HttpPost]
        public string stopDemand(int ID)
        {

            QCTESTEntities QC_DB = new QCTESTEntities();
            var Mtd = QC_DB.M_testDemand.Where(t => t.ID == ID).FirstOrDefault();
            if (Mtd != null)
            {
                if (Mtd.isRunOK())
                {
                    return "已经完成的任务无法停止";
                }
                Mtd.isRun = false;
            }
            QC_DB.SaveChanges();

            return "测试任务已停止";
           
        }

        [HttpPost]
        public string DemandErrorReset(int ID)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();
            int num = QC_DB.M_runTestCase_FailReset(ID);
            return "共重置" + num + "条案例.";

        }


        [HttpPost]
        public Boolean deleteScene(int id)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();
            var rs = QC_DB.M_runScene.First(t => t.ID == id);
            QC_DB.M_runScene.Remove(rs);
            QC_DB.SaveChanges();
            return true;
        }

        [HttpPost]
        public void changeDevice(int id, int? DeviceID)
        {

            QCTESTEntities QC_DB = new QCTESTEntities();

            var sc = QC_DB.M_runScene.First(t => t.ID == id);
            sc.deviceID = DeviceID;
            QC_DB.SaveChanges();
        }



        public ActionResult SceneView(int ID)
        {

            SceneViewModel dv = new SceneViewModel(ID);
            return PartialView("_SceneView", dv);
        }

        public string SceneCaseData(int id, int page,int rows)
        {


            SceneCaseDataModel scd = new SceneCaseDataModel(id, page, rows);

            string json = JsonConvert.SerializeObject(scd);

            return json;

        }

        [HttpPost]
        public string CaseErrorReset(int ID)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();
            var pj = QC_DB.M_runTestCase.FirstOrDefault(t => t.ID == ID);
            pj.state = null;
            pj.startDate = null;
            pj.endDate = null;
            pj.resultPath = null;
            pj.resultXML = null;
            QC_DB.SaveChanges();

            CaseDataModel cd = new CaseDataModel(pj);
            string json = JsonConvert.SerializeObject(cd);

            return json;
        }


        [HttpPost]
        public string userCompleted(int ID,string mark)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();

            var pj = QC_DB.M_runTestCase.FirstOrDefault(t => t.ID == ID);
            pj.state = 2;
            pj.mark = mark;
            QC_DB.SaveChanges();
            CaseDataModel cd = new CaseDataModel(pj);

            string json = JsonConvert.SerializeObject(cd);

            return json;
        }


        public ActionResult caseRecord()
        {
            return View();
        }


       
    }
}