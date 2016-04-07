using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
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
    //*****************************************
    //项目权限控制不足,后期修改

    [Authorize(Roles = "user,guest")]
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
           
            var tcl = from t in userHelper.getPermissionsProject()
                      select new
                      {
                          PID = t.ID,
                          text = t.Pname,
                          state = "closed"
                      };

                string json = JsonConvert.SerializeObject(tcl);

                return json;

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
                              mark = t.mark,
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
                              mark = t.mark,
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

            if (type == 1)//adbb案例 ,添加框架
            {
                var fmks = userHelper.getBaseFrameworks();


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

                var fmks = userHelper.getBaseFrameworks();
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
        /// tree节点
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public string scriptData(int ID)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();

            var mtc = QC_DB.M_testCase.First(t => t.ID == ID);

            return scriptViewModel.getScript2Json(mtc);
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
        public ActionResult debugSave(int id, string steps, string Param)
        {

            using (QCTESTEntities QC_DB = new QCTESTEntities())
            {
                M_testCase mtc = QC_DB.M_testCase.First(t => t.ID == id);
                mtc.editScript(steps);
                QC_DB.SaveChanges();//先保存


                
                XElement xe = XElement.Parse(mtc.testXML);
                var pbs = xe.ParamDictionary(Param);

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
        public void createScene(List<int> ids)
        {
            if (ids == null) return;
            MemoryStream stm = testCaseHelper.getSceneExcelMS(ids);

            DownLoad(stm, "场景模板.xls", "vnd.ms-excel");


        }

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetID"></param>
        /// <param name="sourceID"></param>
        /// <param name="type">0移动到项目,1移动到目录</param>
        /// <returns></returns>
        [HttpPost]
        public bool sortCaseList(int targetID, int sourceID,int type)
        {

           
            QCTESTEntities QC_DB = new QCTESTEntities();

            if (type == 1)
            { 
                int rtn = QC_DB.M_testCase_sort(sourceID, targetID);//存储过程返回最终目标节点ID
                if (rtn > 0)
                {
                    return true;
                }
                else
                    return false;
            }else
            {
                M_testCase mtc = QC_DB.M_testCase.Where(t => t.ID == sourceID).First();//要移动的数据

                mtc.baseID = null;
                mtc.projectID = targetID;
                QC_DB.SaveChanges();
                return true;
            }
        }

        [HttpPost]
        public void downloadCase(List<int> ids)
        {
            if (ids == null) return;
            MemoryStream MyStream = new MemoryStream();
            ZipOutputStream zipedStream = new ZipOutputStream(MyStream);
            zipedStream.SetLevel(6);
            zipedStream.IsStreamOwner = false;

            QCTESTEntities QC_DB = new QCTESTEntities();

            var cs = from t in QC_DB.M_testCase
                     where ids.Contains(t.ID)
                     select t;


            foreach (var c in cs)
            {
                creatCaseStream(c, zipedStream);
            }
            zipedStream.Finish();
            zipedStream.Close();
            DownLoad(MyStream, "案例.zip", "x - zip - compressed");
        }


        [NonAction]
        private void creatCaseStream(M_testCase mtc, ZipOutputStream zipedStream)
        {
           
            if (mtc == null) return;

            byte[] buffer;
            if (mtc.testXML != null)
            {
                XElement xe = XElement.Parse(mtc.testXML);
                xe.SetAttributeValue("id", mtc.ID);
                xe.SetAttributeValue("FID", mtc.FID);
                buffer = System.Text.Encoding.UTF8.GetBytes(xe.ToString());
            }
            else
            {
                buffer = System.Text.Encoding.UTF8.GetBytes("");
            }

            ZipEntry entry = new ZipEntry(mtc.Name + "_" + mtc.ID + ".xml");//案例名
            entry.Size = buffer.Length;
            Crc32 crc = new Crc32();
            crc.Reset();
            crc.Update(buffer);

            entry.Crc = crc.Value;
            zipedStream.PutNextEntry(entry);
            zipedStream.Write(buffer, 0, buffer.Length);
        }

        //流方式下载
        [NonAction]
        private void DownLoad(MemoryStream stm, string FileName, string ContentType)
        {


            Response.Clear();
            Response.ContentType = "application/" + ContentType;


            //通知浏览器下载文件而不是打开
            Response.AddHeader("Content-Disposition", "attachment;  filename=" + FileName);

            byte[] ZipBuffer = new byte[stm.Length - 1];
            ZipBuffer = stm.GetBuffer();

            Response.OutputStream.Write(ZipBuffer, 0, Convert.ToInt32(ZipBuffer.Length));
            Response.End();

          


        }

        

     
    }
}