using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace openCaseMaster.Models
{
    public class topicHelper
    {
        public static Dictionary<int, string> nodes ;

        static topicHelper()
        {
            nodes = new Dictionary<int, string>();
            nodes.Add(101, "测试任务");

            nodes.Add(201, "欢乐吐槽");
            nodes.Add(202, "心得体会");
            nodes.Add(203, "BUG反馈");
            nodes.Add(204, "意见和建议");
        }
    }
}