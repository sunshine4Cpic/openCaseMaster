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


                //获取Fjson  框架组件

                List<caseFramework> ss = new List<caseFramework>();

                if (FID == null)//没有指定框架就是全框架
                {
                    ss = (from t in QC_DB.caseFramework
                          where t.userID == 1
                          select t).ToList();
                }
                else
                {
                    ss = (from t in QC_DB.caseFramework
                          where t.ID == this.FID
                          select t).ToList();
                }


                List<treeViewModel> Fdata = new List<treeViewModel>();
                foreach (var s in ss)
                {
                    var treeNode = s.getControlJson4Tree();
                    Fdata.Add(treeNode);
                }

                var jSetting = new JsonSerializerSettings();
                jSetting.NullValueHandling = NullValueHandling.Ignore;

                this.FJson = JsonConvert.SerializeObject(Fdata, jSetting);


                //获取Pjson  项目组件

                var fps = from t in QC_DB.Framework4Project
                          where t.PID == this.PID
                          select t;
                if (FID != null)
                    fps = fps.Where(t => t.FID == this.FID);

                List<treeViewModel> Pdata = new List<treeViewModel>();
                foreach (var s in fps)
                {
                    var treeNode = s.getControlJson4Tree();
                    Pdata.Add(treeNode);
                }


                this.PJson = JsonConvert.SerializeObject(Pdata, jSetting);





                //获取Ujson  用户组件


                int userID = userHelper.getUserID();

                var Ups = from t in QC_DB.M_testCaseSteps
                          where t.userID == userID
                          select t;
                if (FID != null)
                    Ups = Ups.Where(t => t.FID == this.FID);

                var usts = from t in Ups
                           select new scriptStepTreeModel
                           {
                               text = t.name,
                               id = t.ID,
                               name = "userstep_" + t.ID,
                               desc = t.name,
                               iconCls = "icon-view_outline_detail"
                           };
                this.UJson = JsonConvert.SerializeObject(usts, jSetting);

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