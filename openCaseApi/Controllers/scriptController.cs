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
        [HttpGet]
        [Route("script/{id:int}")]
        public IHttpActionResult Get(int id)
        {

            QCTESTEntities db = new QCTESTEntities();

            var st = db.M_publicTaskScript.First(t => t.ID == id);
            if (st == null)
                return NotFound();

            //XElement xe = XElement.Parse(st.script);


            return Ok(st.script);
        }


        [HttpPost]
        public IHttpActionResult submit(int id)
        {

            return Ok();
        }

    }
}
