using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;

namespace openCaseMaster.Models
{
    public static class XElementHelper
    {
        /// <summary>
        /// 获得参数列表
        /// </summary>
        public static Dictionary<string, string> ParamDictionary(this XElement _xe)
        {
            Dictionary<string, string> Param = new Dictionary<string, string>();
           
            var xel = from ele in _xe.Descendants("ParamBinding")
                      select ele;
            //获得定义的变量
            foreach (XElement xe in xel)
            {
                string value = xe.Attribute("value").Value;
                Regex reg = new Regex("{.*?}");
                MatchCollection matches = reg.Matches(value); // 在字符串中匹配
                foreach (Match match in matches)
                {
                    string name = match.Value.Substring(1, match.Value.Count() - 2);
                    if (!Param.ContainsKey(name))//确认没有重复的参数
                    {
                        Param.Add(name, "");
                    }
                }
            }

            return Param;
        }


    }
}