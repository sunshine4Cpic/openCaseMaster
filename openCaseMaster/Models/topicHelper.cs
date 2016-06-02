using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace openCaseMaster.Models
{
    public class topicHelper
    {
        public static List<topicNode> nodesList;

        public static Dictionary<string, SelectListGroup> Groups;

        public static Dictionary<int, string> nodes;

        static topicHelper()
        {
            nodes = new Dictionary<int, string>(); 
            nodesList = new List<topicNode>();
            Groups = new Dictionary<string, SelectListGroup>();

            

            nodesList.Add(new topicNode(101, "自动化任务", "测试任务", true));
            nodesList.Add(new topicNode(102, "众测任务", "测试任务", true));



            nodesList.Add(new topicNode(201, "欢乐吐槽", "社区生活"));
            nodesList.Add(new topicNode(202, "经验心得", "社区生活"));
            nodesList.Add(new topicNode(203, "BUG反馈", "社区生活"));


            nodesList.Add(new topicNode(301, "太保F&Q", "工作太保"));
            nodesList.Add(new topicNode(302, "项目积累", "工作太保"));



        }


        /// <summary>
        /// 普通节点
        /// </summary>
        public static List<SelectListItem> PublicNodes()
        {
            var rt = from t in nodesList
                     where t.isAdminNode != true
                     select t.ListItem;

            return rt.ToList();

        }

        /// <summary>
        /// admin 节点
        /// </summary>
        public static List<SelectListItem> adminNodes()
        {
            var rt = from t in nodesList
                     where t.isAdminNode == true
                     select t.ListItem;
                    
            return rt.ToList();

        }
    }

    public class topicNode
    {
        

        public topicNode(int Key,string Value,string GroupName)
        {
            SelectListGroup Group;
            if (topicHelper.Groups.ContainsKey(GroupName))
            {
                Group = topicHelper.Groups[GroupName];
            }else
            {
                Group = new SelectListGroup { Name = GroupName };
                topicHelper.Groups.Add(GroupName, Group);
            }
            this.Group = Group;
            topicHelper.nodes.Add(Key, Value);
            this.Key = Key;
            this.Value = Value;
            ListItem = new SelectListItem { Text = Value, Value = Key.ToString(), Group = Group };
        }

        public topicNode(int Key, string Value, string GroupName, bool admin)
            : this(Key, Value, GroupName)
        {
            isAdminNode = admin;
        }
        public int Key { get; set; }
        public string Value { get; set; }
        public string desc { get; set; }

        public SelectListGroup Group;


        public bool isAdminNode { get; set; }

        public SelectListItem ListItem;


    }
}