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
           
            string caseString = "<TestCase desc=\"" + mtc.Name + "\" ><Step name=\"R_InitStep\" desc=\"打开程序\">" +
                                "<ParamBinding name=\"waitTime\" value=\"\" /></Step></TestCase>";

            mtc.testXML = caseString;


        }

    }

}