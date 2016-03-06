using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using openCaseMaster.ViewModels;
using System.Xml.XPath;

namespace openCaseMaster.Models
{
    public static class Extend_M_testCase
    {
      



        /// <summary>
        /// 保存来自json的脚本编辑
        /// </summary>
        /// <param name="steps">json数据 只有steps</param>
        public static void editScript(this M_testCase mtc,string steps)
        {

            XElement caseXml = XElement.Parse(mtc.testXML);
            caseXml.Nodes().Remove();


            JArray ja = (JArray)JsonConvert.DeserializeObject(steps);
            
            foreach (var j in ja.Children<JObject>())
            {
                XElement step = new XElement("Step");
                step.SetAttributeValue("name", j["name"]);
                step.SetAttributeValue("desc", j["desc"]);

                var pbs = j["ParamBinding"].Children<JProperty>();
                foreach (var pb in pbs)
                {
                    XElement ParamBinding = new XElement("ParamBinding");
                    ParamBinding.SetAttributeValue("name", pb.Name);
                    ParamBinding.SetAttributeValue("value", pb.Value);
                    step.Add(ParamBinding);
                }

                caseXml.Add(step);
               
            }

            mtc.testXML = caseXml.ToString();
           

        }




        public static void addNewScrpit(this M_testCase mtc)
        {
           
            string caseString = "<TestCase desc=\"" + mtc.Name + "\" ><Step name=\"R_InitStep\" desc=\"这是一个初始步骤\">" +
                                "<ParamBinding name=\"waitTime\" value=\"\" /></Step></TestCase>";

            mtc.testXML = caseString;


        }




        public static XElement getRunScript(this M_testCase mtc, Dictionary<string, string> param)
        {

            XElement testCase = XElement.Parse(mtc.testXML);
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

            //去除不启用的节点
            var pbs = testCase.XPathSelectElements("//Step/ParamBinding[@name='是否启用' and @value='false']/..");

            foreach (var pb in pbs)
            {
                pb.Remove();
            }

            return testCase;

        }


       
       

    }

}