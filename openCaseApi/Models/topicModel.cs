using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace openCaseApi.Models
{
    public class topicModel_prev
    {
        public topicUserModel User { get; set; }

        public int ID { get; set; }

        public int nodeID { get; set; }

        public DateTime creatDate { get; set; }

        public string title { get; set; }

        public int replyCnt { get; set; }

    }

    public class topicModel : topicModel_prev
    {
        public string Body { get; set; }

    }

    public class scriptListModel 
    {
        public int ID { get; set; }

        public string title { get; set; }

    }
    public class topicUserModel
    {
        public int ID { get; set; }
        public string userName { get; set; }

        //public string Name { get; set; }

        public string Avatar { get; set; }
    }

    public class node
    {
        public int ID { get; set; }
        public string name { get; set; }

    }


    public class stepModel
    {
        public int ID { get; set; }
        public int taskID { get; set; }
        public int stepSort { get; set; }
        public string demoImg { get; set; }
        public string describe { get; set; }
    }

    public class topicAddModel
    {
        [Required(ErrorMessage = "标题必须填写")]
        [StringLength(50, ErrorMessage = "不能大于50个字符")]
        public string title { get; set; }
        [Required]
        [Range(200, 500)]
        public int nodeID { get; set; }

        [Required]
        public string body { get; set; }
    }

    public class replyAddModel
    {
      

        [Required]
        public string body { get; set; }
    }

    public class replyModel
    {
        public int ID { get; set; }

        public topicUserModel User { get; set; }

        public int floor { get; set; }

        public DateTime creatDate { get; set; }
        public string body { get; set; }
    }

}