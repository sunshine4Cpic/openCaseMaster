using Newtonsoft.Json;
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
        public topicModel()
        {

        }

        public topicModel(int ID)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();

            int userID = HttpContext.Current.User.userID();

            var tic = QC_DB.topic.First(t =>
                t.ID == ID && t.state != 0 && t.userID == userID);



            this.ID = ID;
            this.node = tic.node;
            this.title = tic.title;
            this.body = tic.body;
        }

        public int ID { get; set; }

        [Required(ErrorMessage = "标题必须填写")]
        [StringLength(50,  ErrorMessage = "不能大于50个字符")]
        public string title { get; set; }

  
        //[StringLength(10000, ErrorMessage = "不能超过2000个字符")]
        public string body { get; set; }




        [Required]
        [Range(200, 599)]
        public int node { get; set; }
    }


    public class topicTaskModel : topicModel
    {
        [Required]
        [Range(100, 200)]
        public new int node { get; set; }

        public topicTaskModel()
        {

        }

        public topicTaskModel(int ID)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();

            int userID = HttpContext.Current.User.userID();
            var tic = QC_DB.topic.First(t =>
                t.ID == ID && t.state != 0 && t.userID == userID);

            this.ID = tic.ID;
            this.title = tic.title;
            this.body = tic.body;
            this.node = tic.node;


            if (tic.node == 102 && tic.openTestTask != null)
            {
                this.appID = tic.M_publicTask.appID;
                this.startDate = tic.M_publicTask.startDate;
                this.endDate = tic.M_publicTask.endDate;
                this.steps = tic.openTestTask.openTestStep.OrderBy(t => t.stepSort).ToList();
            }
            
        }

        [Required]
        public int appID { get; set; }


        public DateTime? startDate { get; set; }

        public DateTime? endDate { get; set; }

        /// <summary>
        /// 自动化脚本列表
        /// </summary>
        public string scripts { get; set; }


        public List<openTestStep> steps { get; set; }

    }




    public class topicModel_prev 
    {
        public topicUserModel User;

        public int power { get; set; }

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
                else if (tic.node == 102 && tic.openTestTask != null)
                {
                    var tk = tic.openTestTask;
                    openTaskInfo = new openTaskModel();

                    openTaskInfo.appName = tk.M_application.name;
                    openTaskInfo.appID = tk.appID;
                    openTaskInfo.steps = tk.openTestStep.OrderBy(t => t.stepSort).ToList();
                                         
                    openTaskInfo.startDate = tk.startDate;
                    openTaskInfo.endDate = tk.endDate;
                }

                this.ID = tic.ID;

                this.nodeID = tic.node;
                this.power = tic.power;

                this.User = new topicUserModel();
                User.userName = tic.admin_user.Username;
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
                               User = new topicUserModel { ID = t.userID, userName = t.admin_user.Username, Name = t.admin_user.Name, Avatar = t.admin_user.Avatar },
                               creatDate = t.creatDate,
                               state = t.state,
                               floor = t.floor
                           }).ToList();
                
            }
        }



        public taskModel taskInfo;

        public openTaskModel openTaskInfo;


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

        public int floor { get; set; }
    }

    /// <summary>
    /// 自动化任务
    /// </summary>
    public class taskModel
    {
        public int appID { get; set; }

        public string appName { get; set; }

        public DateTime? startDate { get; set; }

        public DateTime? endDate { get; set; }


        public Dictionary<int, string> taskScripts;
    }

    /// <summary>
    /// 众测任务
    /// </summary>
    public class openTaskModel
    {
        public int appID { get; set; }

        public string appName { get; set; }

        public DateTime? startDate { get; set; }

        public DateTime? endDate { get; set; }


        public List<openTestStep> steps;
    }


    public class topicUserModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string userName { get; set; }

        public string Avatar { get; set; }

        public string AvatarUrl { get { return "/Content/userAvatar/" + Avatar; } }


        
    }



}