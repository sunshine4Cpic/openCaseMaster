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
        public static List<caseStepTreeModel> getScript(this M_testCase mtc)
        {
            XElement xe = XElement.Parse(mtc.testXML);

            var sms = xe.Descendants("Step");
            
            //改数据时修改了个别Step的大小写,为了OK加了容错,正式环境后期可以去掉这个逻辑
            if(sms.Count()==0)
            {
                sms = xe.Descendants("step");
            }

            List<caseStepTreeModel> rtn = new List<caseStepTreeModel>();

            foreach (var e in sms)
            {
                caseStepTreeModel tmp = new caseStepTreeModel();
                tmp.name = e.Attribute("name").Value;
                tmp.state = "closed";
                tmp.iconCls = "icon-view_outline_detail";
                tmp.desc = e.Attribute("desc").Value;

                var atts = from t in e.Elements()
                           select new caseStepAttrModel
                           {
                               Key = t.Attribute("name").Value,
                               Value = t.Attribute("value").Value,
                               state = "open",
                               iconCls = "icon-spanner_blue",
                               checkbox = false
                           };
                tmp.children = atts.ToList() ;
                rtn.Add(tmp);
            }

            return rtn;

            
        }

        public static string getScript2Json(this M_testCase mtc)
        {
            List<caseStepTreeModel> tcl = getScript(mtc);

            var jSetting = new JsonSerializerSettings();
            jSetting.NullValueHandling = NullValueHandling.Ignore;

            string json = JsonConvert.SerializeObject(tcl, jSetting);

            return json;

        }




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