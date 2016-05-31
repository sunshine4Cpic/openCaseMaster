using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using openCaseApi.Models;
using System.Linq.Expressions;

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
                return NotFound();
            

            return Ok(v);
        }


        [Route("Topic/{id:int}")]
        [HttpGet]
        public IHttpActionResult Body(int id)
        {
            QCTESTEntities db = new QCTESTEntities();

            var tps = db.topic.FirstOrDefault(t => t.ID == id);
            if (tps == null)
                return NotFound();

            
            return Ok(tps.body);
        }


      
        [HttpGet]
        public IHttpActionResult Scripts(int id)
        {
            QCTESTEntities db = new QCTESTEntities();
            var tps = db.topic.FirstOrDefault(t => t.ID == id);
            if (tps == null || tps.M_publicTask==null)
                return NotFound();



            var ss = from t in tps.M_publicTask.M_publicTaskScript
                     select new
                     {
                         ID = t.ID,
                         title = t.title
                     };

            return Ok(ss);


        }

      
    }
}
