using openCaseMaster.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace openCaseMaster.ViewModels
{
    public class EditStepModel
    {
        public EditStepModel()
        {
            ParamBinding = new List<EditStepPB>();
        }
        

        public string name { get; set; }

        public string desc { get; set; }

        public List<EditStepPB> ParamBinding { get; set; }

        /// <summary>
        /// 初始化详情
        /// </summary>
        public void initDetailed()
        {
            var StepPb = getAutoParamBinding(name);

            foreach (var pb in StepPb)
            {
                foreach (var rPB in ParamBinding)//遍历属性
                {
                    if (rPB.name == pb.name)
                    {
                        pb.value = rPB.value;
                        ParamBinding.Remove(rPB);
                        break;
                    }
                }
            }
            ParamBinding = StepPb;//重置参数
        }

        private XElement getStepXml(string name)
        {
            if (stepType(name) == 1)
            {
                int stepID = Convert.ToInt32(name.Substring(9));
                //这里是用户控件初始化
                QCTESTEntities QC_DB = new QCTESTEntities();
                M_testCaseSteps mtcs = QC_DB.M_testCaseSteps.Where(t => t.ID == stepID).First();

                //mtcs.paramXML.SetAttributeValue("name", name);
                return XElement.Parse(mtcs.paramXML);

            }
            else
            {
                return getAtomStep(name);

            }

        }

        private List<EditStepPB> getAutoParamBinding(string name)
        {
            var StepXml = getStepXml(name);

            List<EditStepPB> pbs = new List<EditStepPB>();

            foreach (var pbx in StepXml.Descendants("ParamBinding"))
            {
                EditStepPB pb = new EditStepPB();
                pb.name = pbx.Attribute("name").Value;
                if (pbx.Attribute("value") != null)
                    pb.value = pbx.Attribute("value").Value;

                if (pbx.Attribute("desc") != null)
                    pb.desc = pbx.Attribute("desc").Value;

                if (pbx.Attribute("list") != null)
                {
                    pb.data = list2data(pbx.Attribute("list").Value);
                }
                    
                pbs.Add(pb);
            }

            return pbs;
        }

        /// <summary>
        /// 后期要删的方法
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private string list2data(string list)
        {
            if(list.Trim()=="") return null;

            var ss = list.Split(',');
            StringBuilder sb=new StringBuilder("[");
            foreach (var s in ss)
            {
                var a = s.Split(':');
                sb.Append("{label: '" + a[0]);
                sb.Append("',value: '"+a[1]);
                sb.Append("'},");
            }
            sb.Remove(sb.Length-1,1);
            sb.Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// 获得原子组件xml
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private XElement getAtomStep(string name)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();
            M_testCaseSteps step = (from t in QC_DB.M_testCaseSteps
                                    where t.type != 1 && t.name == name
                                    select t).FirstOrDefault();
            if (step != null)
            {
                XElement PB = new XElement("ParamBinding");
                PB.SetAttributeValue("name", "是否启用");
                PB.SetAttributeValue("value", "true");
                PB.SetAttributeValue("list", "启用:true,不启用:false");

                var stepXML = XElement.Parse(step.stepXML);
                stepXML.Add(PB);
                return stepXML;
            }

            return null;
            
        }



        private int stepType(string name)
        {
            if (name.IndexOf("userstep_", StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                return 1;
            }

            return 0;
        }

    }


    public class EditStepPB
    {
        public string name { get; set; }
        public string value { get; set; }

        public string desc { get; set; }

        public string title { get { return "<B>" + name + "</B> : <br />" + desc; } }

        public string data { get; set; }

    }


   

}