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

            XElement xx = testCaseHelper.json2StepList(steps);
            foreach(var step in xx.DescendantNodes())
            {
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



        /// <summary>
        /// 获取真实的执行案例
        /// </summary>
        /// <param name="param">参数表</param>
        /// <returns></returns>
        public static XElement getRunScript(this M_testCase mtc, Dictionary<string, string> param)
        {

            XElement testCase = XElement.Parse(mtc.testXML);
            return testCase.getRunScript(param);
            
        }


       
       

    }

}