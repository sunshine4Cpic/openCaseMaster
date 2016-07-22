using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using openCaseMaster.Models;

namespace openCaseMaster.ViewModels
{
    public class ChangePasswordModel
    {
        [Required]
        [StringLength(50, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 4)]
        [Display(Name = "旧密码")]
        public string currentPassword { get; set; }


        [Required]
        [StringLength(50, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; }
    }
    public class userInfoModel
    {

        public userInfoModel(string userName)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();



            var user = QC_DB.admin_user.First(t => t.Username == userName);

            this.ID = user.ID;
            this.userName = userName;
            this.Avatar = user.Avatar;
            this.userName = user.Username;
            this.Name = user.Name;
            this.creatDate = user.GreatDate.Value;

            //自定义框架
            var fm = QC_DB.caseFramework.FirstOrDefault(t => t.userID == this.ID);
            if (fm != null)
                this.framework = XElement.Parse( fm.controlXML).ToString();
            else
                this.framework = "";
             

            topicCount = QC_DB.topic.Where(t => t.userID == ID).Count();

            replyCount = QC_DB.topicReply.Where(t => t.userID == ID).Count();

            newTopics = (from t in QC_DB.topic
                         where t.userID == ID
                         orderby t.replys descending
                         select new topic
                         {
                             ID = t.ID,
                             title = t.title,
                             replys = t.replys,
                             node = t.node
                         }).Take(7).ToList();




        }


        public int ID { get; set; }

        public string Name { get; set; }

        public string userName { get; set; }

        public string framework { get; set; }

        public string Avatar { get; set; }

        public DateTime creatDate { get; set; }

        public int replyCount { get; set; }

        public int topicCount { get; set; }

        public List<topic> newTopics;

        public class topic
        {
            public int ID { get; set; }

            public string title { get; set; }

            public int replys { get; set; }

            public int node { get; set; }
        }
    }

    

    
}