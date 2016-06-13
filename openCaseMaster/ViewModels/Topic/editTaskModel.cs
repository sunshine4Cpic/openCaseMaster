using openCaseMaster.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace openCaseMaster.ViewModels
{
    public class editTaskModel
    {

        public editTaskModel()
        {

        }

        public editTaskModel(int ID)
        {
   
            QCTESTEntities QC_DB = new QCTESTEntities();

            int userID = HttpContext.Current.User.userID();

            var tic = QC_DB.topic.First(t =>
                t.ID == ID && t.state != 0 && t.userID == userID);



            this.ID = ID;
            this.node = tic.node;
            this.title = tic.title;
            this.body = tic.body;

            

           if(tic.node==101 && tic.M_publicTask!=null)
           {
               this.appName = tic.M_publicTask.M_application.name;

               this.startDate = tic.M_publicTask.startDate;
               this.endDate = tic.M_publicTask.endDate;

               this.taskScripts = tic.M_publicTask.M_publicTaskScript.ToDictionary(k => k.ID, v => v.title);
           }
           else if (this.node == 102 && tic.openTestTask != null)
           {
               this.appName = tic.openTestTask.M_application.name;

               this.startDate = tic.openTestTask.startDate;
               this.endDate = tic.openTestTask.endDate;
               this.steps = tic.openTestTask.openTestStep.ToList();
           }

        }

        [Required]
        public int ID { get; set; }

        [Required(ErrorMessage = "标题必须填写")]
        [StringLength(50, ErrorMessage = "不能大于50个字符")]
        public string title { get; set; }


        //[StringLength(10000, ErrorMessage = "不能超过2000个字符")]
        public string body { get; set; }

        public string appName { get; set; }


        public int node { get; set; }


        public DateTime? startDate { get; set; }

        public DateTime? endDate { get; set; }


        public Dictionary<int, string> taskScripts;

        public List<openTestStep> steps;
    }
}