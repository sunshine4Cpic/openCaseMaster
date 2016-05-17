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
                      where t.state != 0
                      orderby t.ID
                      select new taskModel_prev
                      {
                          ID = t.ID,
                          title = t.title,
                          nodeID = t.node,
                          userName = t.admin_user.Username,
                          creatDate = t.creatDate,
                          scriptCount = t.M_publicTaskScript.Count
                      };
            ViewBag.select = "Index";
            ViewBag.page = page;
            int rows = 20;

            var v = lsv.Skip(rows * (page - 1)).Take(rows).ToList();
         

            return View(v);
        }

        [HttpGet]
        public ActionResult add()
        {

            ViewBag.nodes = userHelper.editNodes();
            if (userHelper.isAdmin)
            {
                appSelectItem();
                return View("adminAdd");
            }
            else
                return View("add");
        }

        [HttpPost]
        public ActionResult add(taskModel_add tm)
        {
            
          
            if (!ModelState.IsValid)//验证模型
            {
                ViewBag.nodes = userHelper.editNodes();
                return View(tm);
            }

           

            //开始操作

            QCTESTEntities QC_DB = new QCTESTEntities();

            M_publicTask pt = new M_publicTask();

            pt.node = tm.node;
            pt.creatDate = DateTime.Now;
            pt.title = tm.title;
            pt.body = tm.body;
            pt.userID = userHelper.getUserID();

            QC_DB.M_publicTask.Add(pt);

            
            QC_DB.SaveChanges();

            TempData["add"] = true;

            return RedirectToAction(pt.ID.ToString(), "PublicTask");
            
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult adminAdd(taskModel_adminAdd tm)
        {
            

            if (!ModelState.IsValid)//验证模型
            {
                ViewBag.nodes = userHelper.editNodes();
                return View(tm);
            }

            

            //开始操作

            QCTESTEntities QC_DB = new QCTESTEntities();

            M_publicTask pt = new M_publicTask();
           
            pt.creatDate = DateTime.Now;
            pt.title = tm.title;
            pt.body = tm.body;
            pt.userID = userHelper.getUserID();
            pt.node = tm.node;

            QC_DB.M_publicTask.Add(pt);

            //解析json,添加script
            if (tm.node == 101)
            {
                pt.appID = tm.appID;

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
            }

            QC_DB.SaveChanges();

            TempData["add"] = true;

            return RedirectToAction(pt.ID.ToString(), "PublicTask");

        }

        /// <summary>
        /// 初始化 可选app
        /// </summary>
        [NonAction]
        private void appSelectItem()
        {
            SelectListGroup slg = new SelectListGroup();
            slg.Name = "可选应用";

            var apps = (from t in userHelper.getApps()
                        select new SelectListItem
                        {
                            Text = t.name,
                            Value = t.ID.ToString()
                        }).ToList();


            apps.ForEach(t => t.Group = slg);
            apps.Insert(0, new SelectListItem { Text = "请选择被测应用", Value = "" });
            ViewBag.apps = apps;
        }

    
        [HttpGet]
        [Route("{control}/{id:int}")]
        public ActionResult Task(int id)
        {
            if (TempData["add"] != null)
                ViewBag.add = true;
            taskModel_view tv = new taskModel_view(id);

            return View(tv);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();
            var ts = QC_DB.M_publicTask.First(t => t.ID == id);
            ts.state = 0;
            QC_DB.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}