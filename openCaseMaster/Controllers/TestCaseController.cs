using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using openCaseMaster.Models;
using openCaseMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml;
using System.Xml.Linq;



namespace openCaseMaster.Controllers
{
    [Authorize(Roles="user")]
    public class TestCaseController : Controller
    {

        public static List<caseFramework> cfl;


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
                var tcl = from t in QC_DB.project
                         select new
                         {
                             PID = t.ID,
                             text = t.Pname,
                             state = "closed"
                         };

             if(!User.IsInRole("admin"))
             {
                 int[] pp = userHelper.getUserPermission();
                 tcl = tcl.Where(t => pp.Contains(t.PID));
             }



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
                          select new testCaseTreeModel
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
                          select new testCaseTreeModel
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
            ViewData["AddNew"] = true;

            if (type == 1)//案例添加框架
            {
                var fmks = System.Web.HttpContext.Current.Application["Framework"] as List<caseFramework>;


                var query = fmks.Select(c => new { c.ID, c.workName });
                ViewData["SelectListItem"] = new SelectList(query.AsEnumerable(), "ID", "workName");
            }
            int tempInt;
            M_testCase v = new M_testCase();
            if (int.TryParse(id, out tempInt))
                v.baseID = tempInt;
            if (int.TryParse(PID, out tempInt))
                v.projectID = tempInt;
            v.type = type;
            string defultName = type == 0 ? "新建文件夹" : "新建案例";

            v.Name = defultName;
            return PartialView("_EditCase", v);
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

                var fmks = System.Web.HttpContext.Current.Application["Framework"] as List<caseFramework>;
                var items = fmks
                .Select(c => new {
                        Value = c.ID.ToString(),
                        Text = c.workName
                    });

                ViewData["SelectListItem"] = new SelectList(items.AsEnumerable(), "Value", "Text", mt.FID);

               
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
                mtc.FID = sub.FID;
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

            scriptViewModel tvm = new scriptViewModel(ID);

            return PartialView("_scriptView", tvm);//未做错误处理

        }

        /// <summary>
        /// 编辑脚本
        /// </summary>
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
            
            return PartialView("_EditStep", obj);

        }

        /// <summary>
        /// 获取添加到scriptView中的step步骤的 json(new)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string getStep(string name,int? FID,int? PID)
        {
         
            scriptStepTreeModel xe = testCaseHelper.autoStepParam(name, FID, PID).getScriptStep();

            xe.desc = "XXX new step XXX";

            var jSetting = new JsonSerializerSettings();
            jSetting.NullValueHandling = NullValueHandling.Ignore;

            string json = JsonConvert.SerializeObject(xe, jSetting);
            return json;
            

        }





        public string controlTreeListP(int ID)
        {

            using (QCTESTEntities QC_DB = new QCTESTEntities())
            {
                //获取所属的控件
                var ss = from t in QC_DB.caseFramework
                         where t.ID == ID
                         select t;

                List<treeViewModel> data = new List<treeViewModel>();
                foreach (var s in ss)
                {
                    var treeNode = s.getControlJson4Tree();
                    data.Add(treeNode);
                }

                var jSetting = new JsonSerializerSettings();
                jSetting.NullValueHandling = NullValueHandling.Ignore;

                string json = JsonConvert.SerializeObject(data, jSetting);

                return json;

            }


        }


        [HttpPost]
        public ActionResult debugSave(int id, string steps)
        {

            using (QCTESTEntities QC_DB = new QCTESTEntities())
            {
                M_testCase mtc = QC_DB.M_testCase.First(t => t.ID == id);
                mtc.editScript(steps);
                QC_DB.SaveChanges();//先保存

                XElement xe = XElement.Parse(mtc.testXML);
                var pbs = xe.ParamDictionary();

                if (pbs.Count>0)
                {
                    return PartialView("_DebugParam", pbs);
                    //有参数
                }else
                {
                    //没有参数
                    return null;
                }

                
            }

        }


        [HttpPost]
        public ActionResult CreateUserControl(string steps)
        {
            XElement xe = testCaseHelper.json2StepList(steps);

            int userCount = xe.Descendants("Step")
                            .Where(t => t.Attribute("name").Value.IndexOf("userstep_", StringComparison.CurrentCultureIgnoreCase) == 0)
                            .Count();
            if (userCount > 0) return PartialView("~/Views/Shared/_ModalError.cshtml", "不能选择用户组件构建组件"); 

            var pms = xe.ParamDictionary();

            return PartialView("_CreateUserControl", pms);
            
            
        }

        [HttpPost]
        public Boolean saveUserControl(string name, int PID, int FID, string steps, string Param)
        {

            JObject ParamO = JObject.Parse(Param);
            XElement stepXML = testCaseHelper.json2StepList(steps);

            M_testCaseSteps nmtc = new M_testCaseSteps();
            nmtc.PID = PID;
            nmtc.FID = FID;
            nmtc.userID = userHelper.getUserID();
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

            QCTESTEntities qctest = new QCTESTEntities();
            qctest.M_testCaseSteps.Add(nmtc);
            qctest.SaveChanges();
            return true;

        }

        


        [HttpPost]
        public XElement runScrpt(int id, Dictionary<string, string> Param)
        {
            //要传Param 要传null post时候 指定Param 参数为 其他值就可以 比如 param:"123"
            /*
            var tmpParam = Param;
            if(System.Web.HttpContext.Current.Request.Form.Count==0)
            {
                tmpParam = null;
            }*/
            using (QCTESTEntities QC_DB = new QCTESTEntities())
            {
                M_testCase mtc = QC_DB.M_testCase.First(t => t.ID == id);
                var xe = mtc.getRunScript(Param);
                return xe;
            }
            

        }


        [HttpPost]
        public ActionResult createScene(List<int> ids)
        {
            MemoryStream stm = testCaseHelper.getSceneExcelMS(ids);
            Response.Clear();
            Response.ContentType = "application/vnd.ms-excel";


            //通知浏览器下载文件而不是打开
            Response.AddHeader("Content-Disposition", "attachment;  filename=" + "场景模板.xls");

            byte[] ZipBuffer = new byte[stm.Length - 1];
            ZipBuffer = stm.GetBuffer();

            Response.OutputStream.Write(ZipBuffer, 0, Convert.ToInt32(ZipBuffer.Length));
            Response.End();
            return new EmptyResult();

        }


        
        
        

        
     
    }
}