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


        /// <summary>
        /// 将参数加入到案例文件
        /// </summary>
        public static void setParam(this XElement _xe, Dictionary<string, string> ParamList)
        {
            if (ParamList == null || ParamList.Count == 0) return;
            var BPS = from ele in _xe.Descendants("ParamBinding")
                         select ele;

         
            //获得定义的变量
            foreach (XElement BP in BPS)//遍历ParamBinding
            {
                string TempValue = BP.Attribute("value").Value;
                foreach (var P in ParamList)//遍历变量 这里可以优化?
                {
                    string name = P.Key;
                    string value = P.Value;
                    TempValue = TempValue.Replace("{" + name + "}", value);//进行替换
                }
                BP.Attribute("value").Value = TempValue;
            }
        }

        /// <summary>
        /// 将参数加入到案例文件
        /// </summary>
        public static void setParam(this XElement _xe, XElement ParamList)
        {
            if (ParamList == null) return;
            var Params = from ele in _xe.Descendants("ParamBinding")
                         select ele;

            var vParams = from ele in ParamList.Descendants("ParamBinding")
                          select ele;

            //获得定义的变量
            foreach (XElement PX in Params)//遍历ParamBinding
            {
                string TempValue = PX.Attribute("value").Value;
                foreach (var v in vParams)//遍历变量
                {
                    string name = v.Attribute("name").Value;
                    string value = v.Attribute("value").Value;
                    TempValue = TempValue.Replace("{" + name + "}", value);//进行替换
                }
                PX.Attribute("value").Value = TempValue;
            }
        }


        /// <summary>
        /// 超融合...不对组合参数表,以原始xml为基础将右侧xml中的参数融合到原始xml中.
        /// 如果右侧xml中的参数原始xml总共的没有,那么将抛弃.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static void Merger(this XElement left, XElement right)
        {
            if (left == null || right == null) return;
            var lPBs = from ele in left.Descendants("ParamBinding")
                       select ele;

            var rPBs = from ele in right.Descendants("ParamBinding")
                       select ele;

            foreach (var lPB in lPBs)
            {
                string value;
                if (lPB.Attribute("value") != null)
                    value = lPB.Attribute("value").Value;//设置默认值
                else
                    value = "";//没有默认值就为空

                foreach (var rPB in rPBs)//遍历属性
                {
                    if (rPB.Attribute("name").Value == lPB.Attribute("name").Value)
                    {
                        if (rPB.Attribute("value") != null)
                            value = rPB.Attribute("value").Value;
                        break;
                    }
                }
                lPB.SetAttributeValue("value", value);
            }
        }

        /// <summary>
        /// (step)将步骤替换掉
        /// </summary>
        /// <param name="_xe">被替换的步骤(step)</param>
        /// <param name="_xe2">要替换的步骤列表(steps)</param>
        public static void changeStep(this XElement _xe, XElement _xe2)
        {
            //获得所有用户控件
            var steps = from ele in _xe2.Descendants("Step")
                        select ele;


            foreach (var s in steps)
            {
                _xe.AddBeforeSelf(s);
            }
            _xe.Remove();


        }

    }
}