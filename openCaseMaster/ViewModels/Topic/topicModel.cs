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
        [Range(100, 599)]
        public int node { get; set; }
    }


    public class topicTaskModel : topicModel
    {

        [Required]
        public int appID { get; set; }


        public DateTime? startDate { get; set; }

        public DateTime? endDate { get; set; }

        public string scripts { get; set; }
    }

    public class topicModel_prev 
    {
        public topicUserModel User;

        public int ID { get; set; }

        public int nodeID { get; set; }

        public string nodeText { get { return topicHelper.nodes[nodeID]; } }

        
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

        public int replyCnt { get; set; }

    }

    public class topicModel_view : topicModel_prev
    {
        public topicModel_view(int ID)
        {
            using (QCTESTEntities QC_DB = new QCTESTEntities())
            {
                var tic = QC_DB.topic.First(t => t.ID == ID);
                if (tic.node == 101 && tic.M_publicTask!=null)
                {
                    var tk = tic.M_publicTask;
                    taskInfo = new taskModel();
                    taskInfo.appName = tk.M_application.name;
                    taskInfo.appID = tk.appID;
                    taskInfo.taskScripts = tk.M_publicTaskScript.ToDictionary(k => k.ID, v => v.title);
                    taskInfo.startDate = tk.startDate;
                    taskInfo.endDate = tk.endDate;
                }

                this.ID = tic.ID;

                this.nodeID = tic.node;

                this.User = new topicUserModel();

                User.Name = tic.admin_user.Name;
                User.ID = tic.userID;
                User.Avatar = tic.admin_user.Avatar;

                this.creatDate = tic.creatDate;

                this.title = tic.title;
                this.body = tic.body;

                replies = (from t in tic.topicReply
                           //where t.state != 0
                           select new replyModel
                           {
                               ID = t.ID,
                               body = t.body,
                               User = new topicUserModel { ID = t.userID, Name = t.admin_user.Name, Avatar = t.admin_user.Avatar },
                               creatDate = t.creatDate,
                               state = t.state,
                           }).ToList();
                
            }
        }



        public taskModel taskInfo;


        public string body { get; set; }


        public List<replyModel> replies;
        
    }

    public class replyModel 
    {
        public int ID { get; set; }

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

       
        public string body { get; set; }


        public int? state { get; set; }


        public topicUserModel User;
    }

    public class taskModel
    {
        public int appID { get; set; }

        public string appName { get; set; }

        public DateTime? startDate { get; set; }

        public DateTime? endDate { get; set; }


        public Dictionary<int, string> taskScripts;
    }


    public class topicUserModel
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public string Avatar { get; set; }
    }



}