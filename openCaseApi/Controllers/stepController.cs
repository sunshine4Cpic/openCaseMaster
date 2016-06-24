using openCaseApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace openCaseApi.Controllers
{
    public class stepController : ApiController
    {
        [HttpGet]
        [Route("step")]
        public IHttpActionResult Scripts(int id)
        {
            QCTESTEntities db = new QCTESTEntities();
            var st = db.openTestStep.FirstOrDefault(t => t.ID == id);


            stepModel sm = new stepModel();
            sm.ID = st.ID;
            sm.taskID = st.taskID;
            sm.stepSort = st.stepSort;
            sm.demoImg = st.demoImg;
            sm.describe = st.describe;

            return Ok(sm);


        }
    }
}
