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
        public static List<caseStepModel> getScript(this M_testCase mtc)
        {
            XElement xe = XElement.Parse(mtc.testXML);
            var sms = from t in xe.Descendants("Step")
                      select t;

            List<caseStepModel> rtn = new List<caseStepModel>();

            foreach (var e in sms)
            {
                caseStepModel tmp = new caseStepModel();
                tmp.name = e.Attribute("name").Value;
                tmp.text = tmp.name + " : " + e.Attribute("desc").Value;
                tmp.state = "closed";
                tmp.iconCls = "icon-view_outline_detail";
                tmp.desc = e.Attribute("desc").Value;
               
                

                var atts = from t in e.Elements()
                           select new caseStepAttrModel
                           {
                               Key = t.Attribute("name").Value,
                               Value = t.Attribute("value").Value,
                               text = t.Attribute("name").Value + " : <span style='color:blue'>" + t.Attribute("value").Value+"</span>",
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

            List<caseStepModel> tcl = getScript(mtc);

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
                XElement step = new XElement("step");
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

    }
}