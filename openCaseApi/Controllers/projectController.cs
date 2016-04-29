using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using openCaseApi.Models;

namespace openCaseApi.Controllers
{
    public class projectController : ApiController
    {
        private QCTESTEntities db = new QCTESTEntities();

        // GET: api/project
        public IQueryable<project> Getproject()
        {
            return db.project;
        }

        // GET: api/project/5
        [ResponseType(typeof(project))]
        public IHttpActionResult Getproject(int id)
        {
           
            return Ok("1233123");
        }

        // PUT: api/project/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putproject(int id, project project)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != project.ID)
            {
                return BadRequest();
            }

            db.Entry(project).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!projectExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        

        // DELETE: api/project/5
        [ResponseType(typeof(project))]
        public IHttpActionResult Deleteproject(int id)
        {
            project project = db.project.Find(id);
            if (project == null)
            {
                return NotFound();
            }

            db.project.Remove(project);
            db.SaveChanges();

            return Ok(project);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool projectExists(int id)
        {
            return db.project.Count(e => e.ID == id) > 0;
        }
    }
}