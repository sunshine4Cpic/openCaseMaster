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
    public class userController : ApiController
    {

        [HttpGet]
        [Route("api/user/{id}")]
        public topicUserModel Info(string id)
        {
            QCTESTEntities db = new QCTESTEntities();


            var user = db.admin_user.FirstOrDefault(t => t.Username == id);

            if (user == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            topicUserModel tm = new topicUserModel();
            tm.ID = user.ID;
            //tm.Name = user.Name;
            tm.userName = user.Username;
            tm.Avatar = user.Avatar;

            return tm;

        }
    }
}