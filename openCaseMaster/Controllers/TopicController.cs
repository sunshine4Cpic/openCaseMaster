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
    public class TopicController : Controller
    {

        [AllowAnonymous]
        public ActionResult Index(int page=1)
        {
         
            QCTESTEntities QC_DB = new QCTESTEntities();

            var lsv = from t in QC_DB.topic
                      where t.state != 0
                      orderby t.ID descending
                      select new taskModel_prev
                      {
                          ID = t.ID,
                          title = t.title,
                          nodeID = t.node,
                          userName = t.admin_user.Username,
                          creatDate = t.creatDate,
                          scriptCount = t.M_publicTask.Sum(tk=>tk.M_publicTaskScript.Count)
                      };

         
            ViewBag.select = "Index";
            ViewBag.page = page;
            int rows = 15;


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

            topic pt = new topic();

            pt.node = tm.node;
            pt.creatDate = DateTime.Now;
            pt.title = tm.title;
            pt.body = tm.body;
            pt.userID = userHelper.getUserID();

            QC_DB.topic.Add(pt);

            
            QC_DB.SaveChanges();

            TempData["add"] = true;

            return RedirectToAction(pt.ID.ToString());
            
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult adminAdd(taskModel_adminAdd tm)
        {
      

            if (!ModelState.IsValid)//验证模型
            {
                appSelectItem();
                ViewBag.nodes = userHelper.editNodes();
                return View(tm);
            }

            

            //开始操作

            QCTESTEntities QC_DB = new QCTESTEntities();

            topic tp = new topic();

            tp.creatDate = DateTime.Now;
            tp.title = tm.title;
            tp.body = tm.body;
            tp.userID = userHelper.getUserID();
            tp.node = tm.node;

            QC_DB.topic.Add(tp);

            //解析json,添加script
            if (tm.node == 101)
            {
                M_publicTask pt = new M_publicTask();
                pt.topicID = tp.ID;
                pt.appID = tm.appID;
                pt.creatDate = DateTime.Now;
                pt.startDate = tm.startDate;
                pt.endDate = tm.endDate;

                QC_DB.M_publicTask.Add(pt);

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

            return RedirectToAction(tp.ID.ToString());

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

        [AllowAnonymous]
        [HttpGet]
        [Route("{control}/{id:int}")]
        public ActionResult Topic(int id)
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
            
            var ts = QC_DB.topic.First(t => t.ID == id);
            if (ts.userID!=userHelper.getUserID())
                return RedirectToAction("Index");
            ts.state = 0;
            QC_DB.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}