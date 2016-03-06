using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace openCaseMaster.Models
{
    /// <summary>
    /// 案例操作的基础类
    /// </summary>
    public class testCaseHelper
    {

        /// <summary>
        /// 获取step的原始参数列表(从用户组件和基础组件中查找)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XElement autoStepParam(string name, int FID)
        {
            if (isUserStep(name))
            {
                return getUserStepParam(name);

            }
            else
            {

                return getFarmeworkStep(name, FID);
            }

        }

        /// <summary>
        /// 通过stepName获得StepXml(非属性节点,而是真实的)
        /// </summary>
        /// <param name="stepName"></param>
        /// <returns></returns>
        public static XElement getUserStepRealXml(string stepName)
        {

            int stepID = Convert.ToInt32(stepName.Substring(9));
            //这里是用户控件初始化
            QCTESTEntities QC_DB = new QCTESTEntities();
            M_testCaseSteps mtcs = QC_DB.M_testCaseSteps.Where(t => t.ID == stepID).First();

            return XElement.Parse(mtcs.stepXML);
        }


        /// <summary>
        /// 获取UserStep的参数列表
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XElement getUserStepParam(string name)
        {
            //if (!isUserStep(name)) return;//是否有必要?

            int stepID = Convert.ToInt32(name.Substring(9));
            //这里是用户控件初始化
            QCTESTEntities QC_DB = new QCTESTEntities();
            M_testCaseSteps mtcs = QC_DB.M_testCaseSteps.Where(t => t.ID == stepID).First();

            //mtcs.paramXML.SetAttributeValue("name", name);
            return XElement.Parse(mtcs.paramXML);



        }


        /// <summary>
        /// 获取step的原始参数列表(从用户 基础组件 和项目中查找)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XElement autoStepParamBinding(string name, int FID, int PID)
        {
            var xe = autoStepParam(name, FID);
            if (xe == null)//从项目组件中查找
            {
                QCTESTEntities QC_DB = new QCTESTEntities();
                var fkp = QC_DB.Framework4Project.FirstOrDefault(t => t.FID == FID && t.PID == PID);
                if (fkp != null)
                {
                    var steps = XElement.Parse(fkp.controlXML);
                    var step = steps.Descendants("Step").FirstOrDefault(t => t.Attribute("name").Value == name);
                    if (step != null)
                    {
                        XElement PB = new XElement("ParamBinding");
                        PB.SetAttributeValue("name", "是否启用");
                        PB.SetAttributeValue("value", "true");
                        PB.SetAttributeValue("list", "启用:true,不启用:false");


                        step.Add(PB);
                        return step;
                    }
                }

            }
            return xe;
        }



        /// <summary>
        /// 获得基础组件xml
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XElement getFarmeworkStep(string name, int FID)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();

            var cf = QC_DB.caseFramework.First(t => t.ID == FID);
            XElement xe = XElement.Parse(cf.controlXML);


            var step = xe.Descendants("Step").FirstOrDefault(t => t.Attribute("name").Value == name);

            if (step != null)
            {
                XElement PB = new XElement("ParamBinding");
                PB.SetAttributeValue("name", "是否启用");
                PB.SetAttributeValue("value", "true");
                PB.SetAttributeValue("list", "启用:true,不启用:false");


                step.Add(PB);
                return step;
            }

            return null;

        }


        /// <summary>
        /// 是否是用户组件
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Boolean isUserStep(string name)
        {
            if (name.IndexOf("userstep_", StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                return true;
            }

            return false;
        }
    }
}