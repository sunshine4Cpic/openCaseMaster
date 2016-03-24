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

        public int FID { get; set; }

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
                this.FID = mtc.FID.Value;
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
                this.UJson = treeHelper.getUserControl(FID,PID);

                

            }

        }


        public static  string getScript2Json(M_testCase mtc)
        {
            XElement xe = XElement.Parse(mtc.testXML);

            List<scriptStepTreeModel> tcl = xe.getScriptTreeList();

            var jSetting = new JsonSerializerSettings();
            jSetting.NullValueHandling = NullValueHandling.Ignore;

            string json = JsonConvert.SerializeObject(tcl, jSetting);

            return json;

        }
        

        

    }
}