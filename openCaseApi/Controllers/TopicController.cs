using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using openCaseApi.Models;
using System.Linq.Expressions;
using openCaseApi.Extensions;

namespace openCaseApi.Controllers
{
    public class TopicController : ApiController
    {
        [HttpGet]
        [Route("Topic")]
        public IHttpActionResult Get(int node = 0, int page = 1)
        {

            QCTESTEntities db = new QCTESTEntities();

            var tps = db.topic.Where(t => t.state != 0);
            if (node != 0)
                tps = tps.Where(t => t.node == node);

            var lsv = from t in tps
                      orderby t.ID descending
                      select new topicModel_prev
                     {
                         ID = t.ID,
                         title = t.title,
                         nodeID = t.node,
                         User = new topicUserModel { ID = t.admin_user.ID, userName = t.admin_user.Username, Avatar = t.admin_user.Avatar },
                         creatDate = t.creatDate,
                         replyCnt = t.topicReply.Count
                     };


            int rows = 20;

            
            var v = lsv.Skip(rows * (page - 1)).Take(rows).ToList();
            if (v.Count == 0)
                throw new HttpResponseException(HttpStatusCode.NoContent);
            

            return Ok(v);
        }


        [Route("Topic/{id:int}")]
        [HttpGet]
        public IHttpActionResult Body(int id)
        {
            QCTESTEntities db = new QCTESTEntities();

            var tm = (from t in db.topic
                       where t.ID == id && t.state != 0
                       select new topicModel
                       {
                           ID = t.ID,
                           title = t.title,
                           nodeID = t.node,
                           User = new topicUserModel { ID = t.admin_user.ID, userName = t.admin_user.Username, Avatar = t.admin_user.Avatar },
                           Body = t.body,
                           creatDate = t.creatDate,
                           replyCnt = t.topicReply.Count
                       }).FirstOrDefault();

            if (tm == null)
                return NotFound();

            return Ok(tm);
        }

        [Authorize]
        [HttpPost]
        [Route("Topic/Add")]
        public IHttpActionResult Add([FromBody] topicAddModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)422, "格式错误"));
            }

            QCTESTEntities db = new QCTESTEntities();
            
            
            int userID = User.Identity.userID();

            topic tc = new topic();
            tc.title = model.title;
            tc.node = model.nodeID;
            tc.body = model.body;
            tc.creatDate = DateTime.Now;
            tc.userID = userID;
            db.topic.Add(tc);
            db.SaveChanges();

            return Ok(tc.ID);
        }


        [Authorize]
        [HttpPost]
        public IHttpActionResult Reply(int id, [FromBody]replyAddModel model)
        {

            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)422, "格式错误"));
            }

            QCTESTEntities QC_DB = new QCTESTEntities();

            //是否有效主题
            var tic = QC_DB.topic.FirstOrDefault(t => t.ID == id && t.state != 0);
            if (tic == null) throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)404, "找不到topic"));

          

            tic.replys += 1;


            topicReply tr = new topicReply();
            tr.topicID = id;
            tr.body = model.body;
            tr.userID = User.Identity.userID();
            tr.creatDate = DateTime.Now;
            tr.floor = tic.replys;

            QC_DB.topicReply.Add(tr);

            //推送
            //var names = QC_DB.topicReply.Add(tr).addNotification(QC_DB);


            QC_DB.SaveChanges();

            //hubHelper.Push(names);

            return Ok(tr.ID);
        }



      
        [HttpGet]
        public IHttpActionResult Scripts(int id)
        {
            QCTESTEntities db = new QCTESTEntities();
            var tps = db.topic.FirstOrDefault(t => t.ID == id && t.state != 0);
            if (tps == null)
                return NotFound();



            var ss = (from t in tps.M_publicTask.M_publicTaskScript
                     select new
                     {
                         ID = t.ID,
                         title = t.title
                     }).ToList();

            return Ok(ss);
        }



        [HttpGet]
        public IHttpActionResult Steps(int id)
        {
            QCTESTEntities db = new QCTESTEntities();
            var tps = db.topic.FirstOrDefault(t => t.ID == id && t.state != 0);
            if (tps == null)
                return NotFound();



            var ss = (from t in tps.openTestTask.openTestStep
                      select new stepModel
                      {
                          ID = t.ID,
                          taskID = t.taskID,
                          stepSort = t.stepSort,
                          demoImg = t.demoImg,
                          describe = t.describe
                      }).ToList();

            return Ok(ss);


        }
    }
}
