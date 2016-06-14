using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace openCaseApi.Models
{
    public class topicModel_prev
    {
        public topicUserModel User;

        public int ID { get; set; }

        public int nodeID { get; set; }

        public DateTime creatDate { get; set; }

        public string title { get; set; }

        public int replyCnt { get; set; }

    }

    public class topicUserModel
    {
        public int ID { get; set; }
        public string userName { get; set; }

        public string Avatar { get; set; }
    }

    public class node
    {
        public int ID { get; set; }
        public string name { get; set; }

    }

}