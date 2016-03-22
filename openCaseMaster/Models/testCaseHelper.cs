using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
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
        public static XElement autoStepParam(string name, int? FID)
        {

            switch (checkStepType(name))
            {
                case stepType.user:
                    return getUserStepParam(name);
                case stepType.project:
                    return getProjectStepParam(name);
                default:
                    if (FID == null) return null;
                    return getFarmeworkStep(name, FID.Value);

            }
        }

        /// <summary>
        /// 获取step的原始参数列表(从用户 基础组件 和项目中查找)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XElement autoStepParam(string name, int? FID, int? PID)
        {
            var xe = autoStepParam(name, FID);
            return xe;
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
            var xe = XElement.Parse(mtcs.paramXML);

            xe.SetAttributeValue("name", "userstep_" + mtcs.ID);
            xe.SetAttributeValue("desc", mtcs.name);

            //mtcs.paramXML.SetAttributeValue("name", name);
            return xe;



        }


        /// <summary>
        /// 获取projectStep的参数列表
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XElement getProjectStepParam(string name)
        {

            int stepID = Convert.ToInt32(name.Substring(8));
            //这里是用户控件初始化
            QCTESTEntities QC_DB = new QCTESTEntities();
            Framework4Project mtcs = QC_DB.Framework4Project.Where(t => t.ID == stepID).First();
            var xe = XElement.Parse(mtcs.controlXML);

            xe.SetAttributeValue("name", "prostep_" + mtcs.ID);



            XElement PB = new XElement("ParamBinding");
            PB.SetAttributeValue("name", "是否启用");
            PB.SetAttributeValue("value", "true");
            PB.SetAttributeValue("list", "启用:true,不启用:false");


            xe.Add(PB);



            //mtcs.paramXML.SetAttributeValue("name", name);
            return xe;



        }


      


        /// <summary>
        /// 返回 step 节点的 xml
        /// </summary>
        /// <param name="steps">json数据 只有steps</param>
        public static XElement json2StepList(string jsons)
        {

            XElement caseXml = new XElement("Steps");
            //caseXml.Nodes().Remove();


            JArray ja = (JArray)JsonConvert.DeserializeObject(jsons);

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

            return caseXml;
          


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
        public static stepType checkStepType(string name)
        {
            if (name.IndexOf("userstep_", StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                return stepType.user;
            }
            else if (name.IndexOf("prostep_", StringComparison.CurrentCultureIgnoreCase) == 0)
                return stepType.project;
            return stepType.frame;
        }

        

        
        /// <summary>
        /// 获得场景Excel
        /// </summary>
        /// <param name="_tnc"></param>
        /// <returns></returns>
        public static MemoryStream getSceneExcelMS(List<int> ids)
        {
           
            HSSFWorkbook hssfworkbook = ExcelHelper.InitializeWorkbook();

            ISheet config = hssfworkbook.CreateSheet("配置表");

            IRow row = config.CreateRow(0);
            ICell cell0 = row.CreateCell(0);
            cell0.SetCellValue("案例名");

            ICell cell1 = row.CreateCell(1);
            cell1.SetCellValue("案例ID");

            ICell cell2 = row.CreateCell(2);
            cell2.SetCellValue("参数数量");

            config.SetColumnWidth(0, 30 * 256);

            QCTESTEntities QC_DB = new QCTESTEntities();
            foreach (int id in ids)
            {
                
                M_testCase mtc = (from t in QC_DB.M_testCase
                                  where t.ID == id
                                  select t).FirstOrDefault();
                if (mtc == null) continue;
                var pdl = XElement.Parse(mtc.testXML).ParamDictionary();
                ExcelHelper.creatCaseSheet(hssfworkbook, pdl, mtc.ID.ToString(), mtc.Name);

                //name要做处理?
                ExcelHelper.creatConfigRow(config, mtc.Name, mtc.ID.ToString(), pdl.Count);
            }

            MemoryStream ms = new MemoryStream();
            hssfworkbook.Write(ms);
           
            return ms;
        }

      


       
    }


    public enum stepType
    {
        frame = 0,
        user = 1,
        project = 2,
    };
}