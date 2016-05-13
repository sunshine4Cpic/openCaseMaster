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
                      orderby t.ID
                      select new taskModel_view
                      {
                          ID = t.ID,
                          title=t.title,
                          appName=t.M_application.name,
                          userName = t.admin_user.Username,
                          creatDate = t.creatDate,
                          scriptCount = t.M_publicTaskScript.Count
                      };
            ViewBag.select = "Index";
            ViewBag.page = page;
            int rows = 20;
            return View(lsv.Skip(rows * (page - 1)).Take(rows).ToList());
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
            //这样实现总感觉怪怪的
            string view = "add";
            if (userHelper.isAdmin)
                view = "adminAdd";
          
            if (!ModelState.IsValid)//验证模型
            {
                if (userHelper.isAdmin) appSelectItem();
                return View(view,tm);
            }

            if(!userHelper.isAdmin && tm.node==1)//非admin 用户无法添加 测试任务
            {
                return View(view, tm);
            }

            //开始操作

            QCTESTEntities QC_DB = new QCTESTEntities();

            M_publicTask pt = new M_publicTask();
            pt.appID = tm.appID;
            pt.creatDate = DateTime.Now;
            pt.title = tm.title;
            pt.body = tm.body;
            pt.userID = userHelper.getUserID();

            QC_DB.M_publicTask.Add(pt);

            //解析json,添加script
            if (tm.node == 1)
            {
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

    

        [Route("{control}/{id:int}")]
        public ActionResult Task(int id)
        {
            if (TempData["add"] != null)
                ViewBag.add = true;
            taskModel_view tv = new taskModel_view(id);

            return View(tv);
        }


      


    }
}