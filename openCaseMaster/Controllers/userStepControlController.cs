using openCaseMaster.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using openCaseMaster.ViewModels;
using Newtonsoft.Json;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace openCaseMaster.Controllers
{
     [Authorize(Roles = "user")]
    public class userStepControlController : Controller
    {
        // GET: userStepControl
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string myControlInit()
        {
            return treeHelper.getUserControl();
        
        }


        public ActionResult myControlView(int id)
        {

            controlViewModel v = new controlViewModel(id);
            return PartialView("_controlView", v);

        }


        public string getFrameControl(int id)
        {
            return treeHelper.getFrameControl(id);

        }


        public string getProjectControl(int? PID, int FID)
        {
            if (PID == null)
                return "{}";
            else
                return treeHelper.getProjectControl(PID.Value, FID);

        }


        public ActionResult EditName(int id)
        {
            QCTESTEntities qx = new QCTESTEntities();
            var mtc =  qx.M_testCaseSteps.First(t => t.ID == id);

            return PartialView("_EditName", mtc);

        }

        [HttpPost]
        public bool saveEditName(int id,string name,string mark)
        {
            QCTESTEntities qx = new QCTESTEntities();
            var mtc = qx.M_testCaseSteps.First(t => t.ID == id);

            mtc.name = name;
            mtc.mark = mark;

            qx.SaveChanges();

            return true;
        }

        [HttpPost]
        public ActionResult controlSaveClick(int id, string steps)
        {
            QCTESTEntities qx = new QCTESTEntities();

            var mtc = qx.M_testCaseSteps.First(t => t.ID == id);

            XElement xe = testCaseHelper.json2StepList(steps);

            mtc.stepXML = xe.ToString(); 



            return PartialView("_controlParam", mtc);
           
           

        }


        [HttpPost]
        public Boolean controlSave(int id,string name, string steps, string Param)
        {

            QCTESTEntities qctest = new QCTESTEntities();
            var nmtc = qctest.M_testCaseSteps.First(t => t.ID == id);


            JObject ParamO = JObject.Parse(Param);
            XElement stepXML = testCaseHelper.json2StepList(steps);

           
         
            //nmtc.userID = userHelper.UserID
            nmtc.name = name;
            nmtc.stepXML = stepXML.ToString();

            XElement paramXml = new XElement("Step");
            foreach (var p in ParamO)
            {
                XElement PB = new XElement("ParamBinding");
                PB.SetAttributeValue("name", p.Key);
                PB.SetAttributeValue("value", p.Value);
                paramXml.Add(PB);
            }
            nmtc.paramXML = paramXml.ToString();

            qctest.SaveChanges();
            return true;



        }


        [HttpPost]
        public string delete(int id)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();

            if (QC_DB.M_ContainUserStepCase(id).Count() > 0)
            {
                return "组件已经被引用,无法删除";
            }
            var nmtc = QC_DB.M_testCaseSteps.First(t => t.ID == id);

            var userID = userHelper.UserID;
            if (nmtc.userID == userID)
            {
                QC_DB.M_testCaseSteps.Remove(nmtc);
                QC_DB.SaveChanges();
                return "True";
            }else
            {
                return "您无法删除其他人创建的组件";
            }
           
        }

        public string viewUse(int id)
        {


            QCTESTEntities QC_DB = new QCTESTEntities();
            var aaa = (from t in QC_DB.M_ContainUserStepCase(id)
                       select new
                       {
                           name = t.Name,
                           ID = t.ID
                       }).Take(20).ToList();
            string msg = "";

            if (aaa.Count() == 0)
            {
                return "null";
            }

            foreach (var a in aaa)
            {
                msg += a.name + "<br>";
            }

            return msg;

        }

    }
}