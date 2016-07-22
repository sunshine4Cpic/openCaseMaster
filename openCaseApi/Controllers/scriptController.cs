using openCaseApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml.Linq;

namespace openCaseApi.Controllers
{
    public class scriptController : ApiController
    {
        /// <summary>
        /// 自动化案例
        /// </summary>
        /// <param name="id">案例ID</param>
        [HttpGet]
        [Route("api/script/{id:int}")]
        public string Get(int id)
        {

            QCTESTEntities db = new QCTESTEntities();

            var st = db.M_publicTaskScript.First(t => t.ID == id);
            if (st == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            //XElement xe = XElement.Parse(st.script);


            return st.script;
        }


        [HttpPost]
        public IHttpActionResult submit(int id)
        {

            return Ok();
        }

    }
}
