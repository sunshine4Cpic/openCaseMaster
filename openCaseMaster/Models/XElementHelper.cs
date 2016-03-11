using openCaseMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using System.Xml.XPath;

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
                string value = (string)xe.Attribute("value");
                if (value == null) continue;//一般没有null 的 ....这个判断比飞舞
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



        public static XElement getRunScript(this XElement testCase, Dictionary<string, string> param)
        {

            testCase.setParam(param);

            var Steps = from ele in testCase.Descendants("Step")
                        where ele.Attribute("name").Value.IndexOf("userstep_") == 0
                        select ele;

            //此处为递归 ,在用户编辑时如果限定用户组件不递归就没问题
            while (Steps.Count() > 0)
            {
                XElement Step = Steps.First();
                string stepName = Step.Attribute("name").Value;
                XElement LX = testCaseHelper.getUserStepParam(stepName);//获得用户控件的原参数表


                XElementHelper.Merger(LX, Step);//整合参数到原参数表中

                XElement StepXml = testCaseHelper.getUserStepRealXml(stepName);//获得用户控件真实步骤xml
                StepXml.setParam(LX);//替换其中参数

                /*这时候StepXml就是没有参数的组件xml了**/

                XElementHelper.changeStep(Step, StepXml);//替换组件
            }

            //替换project节点
            var pSteps = from ele in testCase.Descendants("Step")
                        where ele.Attribute("name").Value.IndexOf("prostep_") == 0
                        select ele;
            foreach (var step in pSteps)
            {
                changeProjectStep2Run(step);
            }


            //去除不启用的节点
            var pbs = testCase.XPathSelectElements("//Step/ParamBinding[@name='是否启用' and @value='false']/..");

            foreach (var pb in pbs)
            {
                pb.Remove();
            }

            return testCase;

        }


        /// <summary>
        /// 执行前转换项目组件(后期使用catch)
        /// </summary>
        /// <param name="name"></param>
        private static void changeProjectStep2Run(XElement step)
        {
            int id = Convert.ToInt32(step.Attribute("name").Value.Substring(8));


            QCTESTEntities QC_DB = new QCTESTEntities();
            Framework4Project mtcs = QC_DB.Framework4Project.FirstOrDefault(t => t.ID == id);
            if (mtcs == null) return;

            var xe = XElement.Parse(mtcs.controlXML);

            step.SetAttributeValue("name", xe.Attribute("name").Value);

        }


        /// <summary>
        /// 返回给scriptTree使用的step数据
        /// </summary>
        /// <param name="xe"></param>
        /// <returns></returns>
        public static scriptStepTreeModel getScriptStep(this XElement xe)
        {
            /*
            //改数据时修改了个别Step的大小写,为了OK加了容错,正式环境后期可以去掉这个逻辑
            if (sms.Count() == 0)
            {
                sms = xe.Descendants("step");
            }*/

            scriptStepTreeModel tmp = new scriptStepTreeModel();


            tmp.name = (string)xe.Attribute("name");
            tmp.state = "closed";
            tmp.iconCls = "icon-view_outline_detail";
            tmp.desc = (string)xe.Attribute("desc");

            var atts = from t in xe.Elements()
                       select new scriptStepAttrModel
                       {
                           Key = (string)t.Attribute("name"),
                           Value = t.Attribute("value") == null ? "" : t.Attribute("value").Value,
                           state = "open",
                           iconCls = "icon-spanner_blue",
                           checkbox = false
                       };

            tmp.children = new List<treeViewModel>();
            tmp.children.AddRange(atts.ToList());



            return tmp;


        }


        /// <summary>
        /// 获得scriptTree的全部step数据
        /// </summary>
        /// <param name="xe"></param>
        /// <returns></returns>
        public static List<scriptStepTreeModel> getScriptTreeList(this XElement xe)
        {
          

            var sms = xe.Descendants("Step");

            /*
            //改数据时修改了个别Step的大小写,为了OK加了容错,正式环境后期可以去掉这个逻辑
            if (sms.Count() == 0)
            {
                sms = xe.Descendants("step");
            }*/

            List<scriptStepTreeModel> rtn = new List<scriptStepTreeModel>();

            foreach (var e in sms)
            {
                rtn.Add(e.getScriptStep());
            }

            return rtn;


        }


    }
}