using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using openCaseMaster.Models;
using openCaseMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;


namespace openCaseMaster.Controllers
{
    public class TestCaseController : Controller
    {
        // GET: TestCase
        public ActionResult Index()
        {
            return View();
        }


        
        public string projectListInit()
        {
            //此处应该加入权限,后期加
            //FormsIdentity id = (FormsIdentity)HttpContext.Current.User.Identity;
            //FormsAuthenticationTicket ticket = id.Ticket;//cookie

            using(QCTESTEntities QC_DB = new QCTESTEntities())
            {
                var ps = from t in QC_DB.project
                         select new
                         {
                             PID = t.ID,
                             text = t.Pname
                         };
                var tcl = from t in ps.ToList()
                          select new
                          {
                              PID = t.PID,
                              text = t.text,
                              state = "closed"
                          };

                var jSetting = new JsonSerializerSettings();
                jSetting.NullValueHandling = NullValueHandling.Ignore;

                string json = JsonConvert.SerializeObject(tcl, jSetting);

                return json;

            }
            
        }

        
        public string folderExpanded(int ID)
        {
            using (QCTESTEntities QC_DB = new QCTESTEntities())
            {


                var tcl = from t in QC_DB.M_testCase
                          where t.baseID == ID
                          orderby t.type
                          select new testCaseModel
                          {
                              id = t.ID,
                              text = t.Name,
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


                var tcl = from t in QC_DB.M_testCase
                          where t.projectID == ID
                          && t.baseID == null
                          orderby t.type
                          select new testCaseModel
                          {
                              id = t.ID,
                              text = t.Name,
                              state = t.type == 0 ? "closed" : "open",
                              type = t.type == null ? 0 : t.type.Value
                          };

                var jSetting = new JsonSerializerSettings();
                jSetting.NullValueHandling = NullValueHandling.Ignore;
                 
                string json = JsonConvert.SerializeObject(tcl, jSetting);

                return json;
            }
        }

        [HttpGet]
        public ActionResult AddNew(string id, string PID, int type)
        {
            int tempInt;
            NewCaseModel v = new NewCaseModel();
            if (int.TryParse(id, out tempInt))
                v.baseID = tempInt;
            if (int.TryParse(PID, out tempInt))
                v.projectID = tempInt;
            v.type = type;
            string defultName = type == 0 ? "新建文件夹" : "新建案例";

            v.Name = defultName;
            return PartialView("_newCase", v);
        }

        [HttpPost]
        public int AddNew(M_testCase newTC)
        {
            using (QCTESTEntities QC_DB = new QCTESTEntities())
            {
                newTC.addNewScrpit();
                QC_DB.M_testCase.Add(newTC);
                QC_DB.SaveChanges();
                return newTC.ID;
            }
        }

        [HttpGet]
        public ActionResult EditCase(int ID)
        {
            using (QCTESTEntities QC_DB = new QCTESTEntities())
            {
                M_testCase mt = QC_DB.M_testCase.First(t => t.ID == ID);
                return PartialView("_EditCase", mt);//未做错误处理
            }
        }

        [HttpPost]
        public bool EditCase(M_testCase sub)
        {
            using (QCTESTEntities QC_DB = new QCTESTEntities())
            {
                M_testCase mtc = QC_DB.M_testCase.FirstOrDefault(t => t.ID == sub.ID);
                mtc.Name = sub.Name;
                mtc.mark = sub.mark;
                QC_DB.SaveChanges();
                return true;
            }
        }

        [HttpPost]
        public bool DeleteCase(int ID)
        {
           
            using (QCTESTEntities QC_DB = new QCTESTEntities())
            {
                M_testCase mtc = QC_DB.M_testCase.First(t => t.ID == ID);
                if (QC_DB.M_testCase_delete(ID) > 0)//为什么要用存储过程我也忘了,是不是可以不用?
                    return true;
                else
                    return false;
            }
        }


        public ActionResult scriptView(int ID)
        {
            
            using (QCTESTEntities QC_DB = new QCTESTEntities())
            {
                M_testCase mtc = QC_DB.M_testCase.First(t => t.ID == ID);

                return PartialView("_scriptView", mtc);//未做错误处理
            }
        }

        [HttpPost]
        public bool editScript(int ID, string steps)
        {
            using (QCTESTEntities QC_DB = new QCTESTEntities())
            {
                M_testCase mtc = QC_DB.M_testCase.First(t => t.ID == ID);
                mtc.editScript(steps);
                QC_DB.SaveChanges();
                return true;
            }
           
        }

        public ActionResult EditStep(EditStepModel obj)
        {
            obj.initDetailed();
            
            return PartialView("_EditStep", obj);//未做错误处理

        }



        public string controlTreeList()
        {
            /*
            using (QCTESTEntities QC_DB = new QCTESTEntities())
            {
                var ss = from t in QC_DB.M_testCaseSteps
                         where t.type == type && t.PID == null
                         select t;
                if (type == 1)
                {
                    string username = HttpContext.Current.User.Identity.Name;
                    ss = ss.Where(t => t.admin_user.Username == username);
                }
            }*/
            return null;

        }
     
     
    }
}