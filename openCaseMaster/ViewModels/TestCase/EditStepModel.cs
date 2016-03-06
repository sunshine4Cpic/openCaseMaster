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
            FID = 1;//默认从robotium脚本查询step信息
        }
        

        public string name { get; set; }

        public string desc { get; set; }

        /// <summary>
        /// 框架ID
        /// </summary>
        public int FID { get; set; }

        /// <summary>
        /// 项目ID
        /// </summary>
        public int PID { get; set; }

        public List<EditStepPB> ParamBinding { get; set; }

        /// <summary>
        /// 初始化步骤详情
        /// </summary>
        public void initDetailed()
        {
            var StepPb = StepParamBinding();

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

       

        
      


        /// <summary>
        /// 获得当前step的可编辑属性
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>

        private List<EditStepPB> StepParamBinding()
        {
            var StepXml = testCaseHelper.autoStepParamBinding(name, this.FID, this.PID);
            

            //合并属性
            List<EditStepPB> pbs = new List<EditStepPB>();

            foreach (var pbx in StepXml.Descendants("ParamBinding"))
            {
                EditStepPB pb = new EditStepPB();
                pb.name = pbx.Attribute("name").Value;
                if (pbx.Attribute("value") != null)
                    pb.value = pbx.Attribute("value").Value;

                if (pbx.Attribute("desc") == null)
                    pb.desc = pb.name ;
                else
                    pb.desc = pb.name + " : " + pbx.Attribute("desc").Value;

                if (pbx.Attribute("list") != null)
                {
                    pb.data = list2data(pbx.Attribute("list").Value);
                }
                    
                pbs.Add(pb);
            }

            return pbs;
        }

        /// <summary>
        /// 讲step中的的list属性专程前台可读的json属性(前期设计失误产物)
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

    
  



      

    }


    public class EditStepPB
    {
        /// <summary>
        /// 参数名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string desc { get; set; }

        /// <summary>
        /// 就是list.....
        /// </summary>
        public string data { get; set; }

    }


   

}