using openCaseMaster.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace openCaseMaster.ViewModels
{
    public class AutoRunSceneModel
    {
        public AutoRunSceneModel(string device)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();


            //属于这台手机并已经开始的场景
            var Scenes = QC_DB.M_runScene.Where(t => t.M_deviceConfig.device == device && t.M_testDemand.isRun == true);

            //有安装任务 或未执行案例>0
            var rs = from t in Scenes
                     where (t.M_testDemand.apkName != null && t.installResult == null)
                     || t.M_runTestCase.Where(c => c.state == null).Count() > 0
                     select t;

            var Scene = rs.OrderBy(T => T.ID).FirstOrDefault();

            if (Scene == null) return;//没有任务退出

            this.id = Scene.ID;
            this.name = Scene.name;
            this.installApk = Scene.M_testDemand.apkName;
            this.installResult = Scene.installResult;

            var cs = from t in QC_DB.M_runTestCase
                     where t.sceneID == this.id
                     select new runCaseSimpleModel
                     {
                         id = t.ID,
                         name = t.name,
                         state = t.state
                     };
            caseList = cs.ToList();

        }
        public int id { get; set; }

        public string name { get; set; }

        /// <summary>
        /// 安装apk
        /// </summary>
        public string installApk { set; get; }

        /// <summary>
        /// 安装结果
        /// </summary>
        public string installResult { set; get; }

        public List<runCaseSimpleModel> caseList { get; set; }
    }

    public class runCaseSimpleModel
    {
        public int id { get; set; }

        public string name { get; set; }

        /// <summary>
        /// 1执行 2已处理
        /// </summary>
        public int? state { get; set; }
    }

}