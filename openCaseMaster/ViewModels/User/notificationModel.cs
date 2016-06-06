using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace openCaseMaster.ViewModels
{
    public class notificationModel
    {
        public int ID { get; set; }

        public int topicID { get; set; }

        public int floor { get; set; }

        public topicUserModel User;

        public int? state { get; set; }

        public string title { get; set; }

        public string body { get; set; }

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

        public DateTime creatDate { get; set; }
    }


  
}