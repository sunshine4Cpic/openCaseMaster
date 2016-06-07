using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using openCaseMaster.Models;
using openCaseMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace openCaseMaster.Controllers
{
    [Authorize(Roles = "user")]
    public class TopicController : Controller
    {
         [AllowAnonymous]
        public ActionResult markdown()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Index(int page=1,int rows =20)
        {
         
            QCTESTEntities QC_DB = new QCTESTEntities();

            var date = QC_DB.topic.Where(t => t.state != 0);
            var lsv = from t in date
                      orderby t.power descending, t.ID descending 
                      select new topicModel_prev
                      {
                          ID = t.ID,
                          title = t.title,
                          nodeID = t.node,
                          User = new topicUserModel { ID = t.userID, userName=t.admin_user.Username, Name = t.admin_user.Name, Avatar = t.admin_user.Avatar },
                          creatDate = t.creatDate,
                          scriptCount = t.M_publicTask.M_publicTaskScript.Count,
                          replyCnt = t.topicReply.Count,
                          power = t.power.Value
                      };


            ViewBag.nodeID = 0;
            ViewBag.page = page;
            ViewBag.rows = rows;
            ViewBag.total = date.Count();



            var v = lsv.Skip(rows * (page - 1)).Take(rows).ToList();
         
        
            return View(v);
        }

        [AllowAnonymous]
        public ActionResult node(int id,int page=1,int rows=20)
        {

            QCTESTEntities QC_DB = new QCTESTEntities();

            var date = QC_DB.topic.Where(t => t.node == id && t.state != 0);

            var lsv = from t in date
                      orderby t.power descending, t.ID descending 
                      select new topicModel_prev
                      {
                          ID = t.ID,
                          title = t.title,
                          nodeID = t.node,
                          User = new topicUserModel { ID = t.userID, userName = t.admin_user.Username, Name = t.admin_user.Name, Avatar = t.admin_user.Avatar },
                          creatDate = t.creatDate,
                          scriptCount = t.M_publicTask.M_publicTaskScript.Count,
                          replyCnt = t.topicReply.Count
                      };


            ViewBag.nodeID = id;
            ViewBag.page = page;
            ViewBag.rows = rows;
            ViewBag.total = date.Count();


            var v = lsv.Skip(rows * (page - 1)).Take(rows).ToList();

            
            return View("Index",v);
        }

      

        [HttpGet]
        public ActionResult add()
        {

            ViewBag.nodes = this.NodesList();
            if (userHelper.isAdmin)
            {
                appSelectItem();
                return View("adminAdd");
            }
            else
                return View("add");
        }

        /// <summary>
        /// 编辑普通节点
        /// </summary>
        [HttpGet]
        public ActionResult edit(int id)
        {

            QCTESTEntities QC_DB = new QCTESTEntities();

            int userID = User.userID();
            var tic = QC_DB.topic.First(t =>
                t.ID == id && t.state != 0 && t.userID == userID);
            
            
            string[] aa = new string[6];
            topicModel tm = new topicModel();
            tm.node = tic.node;
            tm.title = tic.title;
            tm.body = tic.body;


            if (tic.node ==101)
            {
                var node = topicHelper.nodes.First(t => t.Key == tic.node);

                List<SelectListItem> nodes = new List<SelectListItem>();
                nodes.Add(new SelectListItem { Text = node.Value, Value = node.Key.ToString(), Selected = true });
                ViewBag.nodes = nodes;

                var tk = tic.M_publicTask;
                taskModel taskInfo = new taskModel();
                taskInfo.appName = tk.M_application.name;
                taskInfo.appID = tk.appID;
                taskInfo.taskScripts = tk.M_publicTaskScript.ToDictionary(k => k.ID, v => v.title);
                taskInfo.startDate = tk.startDate;
                taskInfo.endDate = tk.endDate;
                ViewBag.taskInfo = taskInfo;

                return View(tm);

            }
            else
            {
                var nodes = topicHelper.PublicNodes();

                foreach (var n in nodes)
                {
                    if (n.Value == tic.node.ToString())
                    {
                        n.Selected = true;
                        break;
                    }
                }
                ViewBag.nodes = nodes;

                return View(tm);
            }
        }

        /// <summary>
        /// 编辑
        /// </summary>
        [HttpPost]
        public ActionResult edit(topicModel tm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.nodes = this.NodesList();
                return View(tm);
            }

            QCTESTEntities QC_DB = new QCTESTEntities();

            int userID = User.userID();
            var tic = QC_DB.topic.First(t =>
                t.ID == tm.ID && t.state != 0 && t.userID == userID);
            tic.node = tm.node;
            tic.title = tm.title;
            tic.body = tm.body;

            QC_DB.SaveChanges();

            return RedirectToAction(tm.ID.ToString());

        }


        /// <summary>
        /// 回复
        /// </summary>
        [HttpPost]
        public ActionResult reply(int id, string body)
        {
         

            if(string.IsNullOrEmpty(body))
            {
                return RedirectToAction(id.ToString());
            }
            
            QCTESTEntities QC_DB = new QCTESTEntities();

            //是否有效主题
            var tic = QC_DB.topic.FirstOrDefault(t =>t.ID == id && t.state != 0 );
            if (tic == null) throw new HttpException(404, "page not found");

            tic.replys += 1;
           

            topicReply tr = new topicReply();
            tr.topicID = id;
            tr.body = body;
            tr.userID = User.userID();
            tr.creatDate = DateTime.Now;
            tr.floor = tic.replys;

            var names = QC_DB.topicReply.Add(tr).addNotification(QC_DB);


            QC_DB.SaveChanges();

            hubHelper.Push(names);

            return RedirectToAction(id.ToString());

        }


        [Route("reply/Delete/{id}")]
        [HttpPost]
        public bool replyDelete(int id)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();

            int userID = User.userID();
            //是否有效主题
            var tr = QC_DB.topicReply.FirstOrDefault(t => t.ID == id && t.state != 0 && t.userID == userID);
            if (tr == null) return true;
            tr.state = 0;
            QC_DB.SaveChanges();

            return true;

        }

       
        [HttpPost]
        public ActionResult add(topicModel tm)
        {


            if (!ModelState.IsValid || tm.node < 200)//普通用户不能add任务
            {
                ViewBag.nodes = this.NodesList();
                return View(tm);
            }

           

            //开始操作

            QCTESTEntities QC_DB = new QCTESTEntities();

            topic pt = new topic();

            pt.node = tm.node;
            pt.creatDate = DateTime.Now;
            pt.title = tm.title;
            pt.body = tm.body;
            pt.userID = User.userID();

            var names = QC_DB.topic.Add(pt).addNotification(QC_DB);
            
           
            QC_DB.SaveChanges();

            hubHelper.Push(names);

            TempData["event"] = "add";

            return RedirectToAction(pt.ID.ToString());
            
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult adminAdd(topicTaskModel tm)
        {
            if (tm.node <200) return add(tm);//普通节点
      

            if (!ModelState.IsValid)//验证模型
            {
                appSelectItem();
                ViewBag.nodes = this.NodesList();
                return View(tm);
            }

            //开始操作

            QCTESTEntities QC_DB = new QCTESTEntities();

            topic tp = new topic();

            tp.creatDate = DateTime.Now;
            tp.title = tm.title;
            tp.body = tm.body;
            tp.userID = User.userID();
            tp.node = tm.node;

            QC_DB.topic.Add(tp);

            //解析json,添加script
            if (tm.node == 101)
            {
                M_publicTask pt = new M_publicTask();
                pt.ID = tp.ID;
                pt.appID = tm.appID;
                //pt.creatDate = DateTime.Now;
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

            TempData["event"] = "add";

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
        [Route("topic/{id:int}")]
        public ActionResult Topic(int id)
        {
            if (TempData["event"] != null)
                ViewBag.clear = true;
            topicModel_view tv = new topicModel_view(id);

            return View(tv);
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();

            var ts = QC_DB.topic.First(t => t.ID == id);
            if (ts.userID != User.userID())
                return RedirectToAction("Index");
            ts.state = 0;
            QC_DB.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public void suggest(int id,int power)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();

            var ts = QC_DB.topic.First(t => t.ID == id);

            ts.power = power;
            QC_DB.SaveChanges();

        }

       



        /// <summary>
        /// 用户可用节点
        /// </summary>
        [NonAction]
        private List<SelectListItem> NodesList(int id = 0)
        {
            List<SelectListItem> SLI = new List<SelectListItem>();
            SLI.Add(new SelectListItem { Text = "请选择节点", Value = "" });


            SLI.AddRange(topicHelper.PublicNodes());


            if (userHelper.isAdmin)
                SLI.AddRange(topicHelper.adminNodes());

            if (id < 1) return SLI;

            foreach (var s in SLI)
            {
                if (s.Value == id.ToString())
                {
                    s.Selected = true;
                    break;
                }
            }

            return SLI;

        }


        
    }
}