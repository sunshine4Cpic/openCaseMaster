using openCaseMaster.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace openCaseMaster.ViewModels
{
    public class ProjectListModel
    {
        public ProjectListModel(int page, int rows)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();

            var us = from t in QC_DB.project
                     orderby t.ID
                     select new ProjectModel
                     {
                         id = t.ID,
                         Pname = t.Pname
                     };

            this.total = QC_DB.project.Count();

            this.rows = us.Skip(rows * (page - 1)).Take(rows).ToList();
        }

        public int total { get; set; }

        public List<ProjectModel> rows { get; set; }
    }

    public class ProjectModel
    {
        public int id { get; set; }

        public string Pname { get; set; }

    }
}