using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using openCaseMaster.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace openCaseMaster.ViewModels
{
    public class scriptViewModel 
    {
       

        public int PID { get; set; }

        public int? FID { get; set; }

        public string Fname { get; set; }

        /// <summary>
        /// 脚本ID
        /// </summary>
        public int ID { get; set; }

        public string scriptName { get; set; }


        public string TreeJson { get; set; }

        public string PJson { get; set; }

        public string FJson { get; set; }

        public string UJson { get; set; }


        /// <summary>
        /// 初始化脚本model
        /// </summary>
        /// <param name="ID">脚本ID</param>
        public scriptViewModel(int ID)
        {
            using (QCTESTEntities QC_DB = new QCTESTEntities())
            {
                this.ID = ID;
                var mtc = QC_DB.M_testCase.First(t => t.ID == ID);
                this.TreeJson = getScript2Json(mtc);
                this.FID = mtc.FID;
                this.scriptName = mtc.Name;
                this.Fname = mtc.caseFramework.workName;
                var PP = new ObjectParameter("Out1", DbType.Int32);
                QC_DB.M_testCase_getProject(ID, PP);
                this.PID = Convert.ToInt32(PP.Value);

                //Fjson 
                this.FJson = treeHelper.getFrameControl(FID);
                

                //获取Pjson  项目组件

                this.PJson = treeHelper.getProjectControl(PID, FID);


                //获取Ujson  用户组件
                this.UJson = treeHelper.getUserControl(FID);

                

            }

        }



        private List<scriptStepTreeModel> getScript(M_testCase mtc)
        {
            XElement xe = XElement.Parse(mtc.testXML);

            var sms = xe.Descendants("Step");

            /*
            //改数据时修改了个别Step的大小写,为了OK加了容错,正式环境后期可以去掉这个逻辑
            if (sms.Count() == 0)
            {
                sms = xe.Descendants("step");
            }*/

            List<scriptStepTreeModel> rtn = new List<scriptStepTreeModel>();

            foreach (var e in sms)
            {
                rtn.Add(e.getScriptStep());
            }

            return rtn;


        }

        private  string getScript2Json(M_testCase mtc)
        {
            List<scriptStepTreeModel> tcl = getScript(mtc);

            var jSetting = new JsonSerializerSettings();
            jSetting.NullValueHandling = NullValueHandling.Ignore;

            string json = JsonConvert.SerializeObject(tcl, jSetting);

            return json;

        }
        

        

    }
}