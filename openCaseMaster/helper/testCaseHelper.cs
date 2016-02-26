using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using openCaseMaster.ViewModels;
using openCaseMaster.Models;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace openCaseMaster.helper
{
    public static class testCaseHelper
    {
        public static List<caseStepModel> getCaseTree(this M_testCase mtc)
        {
            XElement xe = XElement.Parse(mtc.testXML);
            var sms = from t in xe.Descendants("Step")
                      select t;

            List<caseStepModel> rtn = new List<caseStepModel>();

            foreach (var e in sms)
            {
                caseStepModel tmp = new caseStepModel();
                tmp.stepName = e.Attribute("name").Value;
                tmp.text = tmp.stepName + " : " + e.Attribute("desc").Value;
                tmp.state = "closed";
                tmp.iconCls = "icon-view_outline_detail";
               
                

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

        public static string getCaseTree2Json(this M_testCase mtc)
        {

            List<caseStepModel> tcl = getCaseTree(mtc);

            var jSetting = new JsonSerializerSettings();
            jSetting.NullValueHandling = NullValueHandling.Ignore;

            string json = JsonConvert.SerializeObject(tcl, jSetting);

            return json;

        }
    }
}