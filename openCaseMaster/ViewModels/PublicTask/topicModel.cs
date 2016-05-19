using Newtonsoft.Json.Linq;
using openCaseMaster.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;


namespace openCaseMaster.ViewModels
{

    public class topicModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "标题必须填写")]
        [StringLength(50,  ErrorMessage = "不能大于50个字符")]
        public string title { get; set; }

  
        //[StringLength(10000, ErrorMessage = "不能超过2000个字符")]
        public string body { get; set; }




        [Required]
        [Range(201, 599)]
        public int node { get; set; }
    }


    public class topicModel_taskAdd : topicModel
    {
        [Required]
        [Range(100, 500)]
        new public int node { get; set; }

        [Required]
        public int appID { get; set; }


        public DateTime? startDate { get; set; }

        public DateTime? endDate { get; set; }

        public string scripts { get; set; }
    }

    public class taskModel_prev
    {

        public string userName { get; set; }
        public int ID { get; set; }

        public int nodeID { get; set; }

        public string nodeText { get { return topicHelper.nodes[nodeID]; } }

        public string img { get; set; }

        public DateTime creatDate { get; set; }

        public string timeago
        {
            get
            {
                var ts = DateTime.Now.Subtract(creatDate);
                if (ts.TotalDays > 1)
                {
                    return ts.Days + " 天前";
                }
                else if (ts.TotalHours > 1)
                {
                    return ts.Hours + " 小时前";
                }

                return ts.Minutes + " 分钟前";

            }
        }


        public int? scriptCount { get; set; }

        public string title { get; set; }
        public int userID { get; set; }

    }

    public class taskModel_view : taskModel_prev
    {
        public taskModel_view(int ID)
        {
            using (QCTESTEntities QC_DB = new QCTESTEntities())
            {
                var tic = QC_DB.topic.First(t => t.ID == ID);
                if (tic.node < 200)
                {
                    var tk = tic.M_publicTask.First();
                    taskInfo = new testTask();
                    taskInfo.appName = tk.M_application.name;
                    taskInfo.appID = tk.appID;
                    taskInfo.taskScripts = tk.M_publicTaskScript.ToDictionary(k => k.ID, v => v.title);
                    taskInfo.startDate = tk.startDate;
                    taskInfo.endDate = tk.endDate;
                }

                this.ID = tic.ID;

                this.nodeID = tic.node;

                this.userName = tic.admin_user.Username;
                this.userID = tic.userID;

                this.creatDate = tic.creatDate;

                this.title = tic.title;
                this.body = tic.body;
                
            }
        }

        

        public testTask taskInfo;


        public string body { get; set; }



        
    }

    public class testTask
    {
        public int appID { get; set; }

        public string appName { get; set; }

        public DateTime? startDate { get; set; }

        public DateTime? endDate { get; set; }


        public Dictionary<int, string> taskScripts;
    }


    

}