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

    public class taskModel_add
    {





        [Required(ErrorMessage = "标题必须填写")]
        [StringLength(50,  ErrorMessage = "不能大于50个字符")]
        public string title { get; set; }

        [Required]
        [StringLength(2000, ErrorMessage = "不能超过2000个字符")]
        public string body { get; set; }




        [Required]
        [Range(201, 599)]
        public int node { get; set; }
    }


    public class taskModel_adminAdd : taskModel_add
    {
        [Required]
        [Range(100, 599)]
        new public int node { get; set; }

        public int? appID { get; set; }


        public DateTime? startDate { get; set; }

        public DateTime? endDate { get; set; }

        public string scripts { get; set; }
    }


    public class taskModel_view
    {
        public taskModel_view()
        {

        }
        public taskModel_view(int ID)
        {
            using (QCTESTEntities QC_DB = new QCTESTEntities())
            {
                var tk = QC_DB.M_publicTask.First(t => t.ID == ID);
                if(tk.node<200)
                {
                    taskInfo = new testTask();
                    taskInfo.appName = tk.M_application.name;
                    taskInfo.appID = tk.appID.Value;
                    taskInfo.taskScripts = tk.M_publicTaskScript.ToDictionary(k => k.ID, v => v.title);
                }

                
                this.userName = tk.admin_user.Username;
                this.startDate = tk.startDate;
                this.creatDate = tk.creatDate;
                this.endDate = tk.endDate;
                this.title = tk.title;
                this.body = tk.body;
                
            }
        }

        public testTask taskInfo;

        public int ID { get; set; }

        public string userName { get; set; }

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

        public int scriptCount { get; set; }

        public DateTime? startDate { get; set; }

        public DateTime? endDate { get; set; }



        public string title { get; set; }

        public string body { get; set; }


    }

    public class testTask
    {
        public int appID { get; set; }

        public string appName { get; set; }

        public Dictionary<int, string> taskScripts;
    }


    public class taskModel_prev
    {

        public string userName { get; set; }
        public int ID { get; set; }
        public int node { get; set; }
        public string nodeText { get { return userHelper.nodes[this.node]; } }

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

        public int scriptCount { get; set; }
        public string title { get; set; }


    }

}