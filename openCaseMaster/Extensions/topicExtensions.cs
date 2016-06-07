using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace openCaseMaster.Models
{
    public static class topicExtensions
    {
        /// <summary>
        /// 相关@消息推送
        /// </summary>
        /// <returns>消息名单</returns>
        public static List<string> addNotification(this topic tp, QCTESTEntities QC_DB)
        {
            //消息推送

            Regex reg = new Regex(@"@(\w{4,20})");
            MatchCollection matches = reg.Matches(tp.body); // 在字符串中匹配

            int i = 0;
            List<string> names = new List<string>();
            foreach (Match match in matches)
            {
                if (i++ > 20) break;


                string userName = match.Value.Substring(1, match.Value.Count() - 1);

                if (names.Contains(userName)) break;//重复@


                var toUser = QC_DB.admin_user.FirstOrDefault(t => t.Username == userName);

                if (toUser != null)
                {
                    names.Add(userName);
                    notification nof = new notification();
                    nof.userID = toUser.ID;
                    nof.topicID = tp.ID;
                    nof.state = 0;
                    nof.createDate = DateTime.Now;
                    QC_DB.notification.Add(nof);
                }
            }

            return names;
            //消息推送

        }



        /// <summary>
        /// 相关@消息推送
        /// </summary>
        /// <returns>消息名单</returns>
        public static List<string> addNotification(this topicReply tp, QCTESTEntities QC_DB)
        {
            //消息推送

            Regex reg = new Regex(@"@(\w{4,20})");
            MatchCollection matches = reg.Matches(tp.body); // 在字符串中匹配

            int i = 0;
            List<string> names = new List<string>();
            foreach (Match match in matches)
            {
                if (i++ > 10) break;


                string userName = match.Value.Substring(1, match.Value.Count() - 1);

                if (names.Contains(userName)) break;//重复@


                var toUser = QC_DB.admin_user.FirstOrDefault(t => t.Username == userName);

                if (toUser != null)
                {
                    names.Add(userName);
                    notification nof = new notification();
                    nof.userID = toUser.ID;
                    nof.replyID = tp.ID;
                    nof.state = 0;
                    nof.createDate = DateTime.Now;
                    QC_DB.notification.Add(nof);
                }
            }

            return names;
            //消息推送

        }
    }
}