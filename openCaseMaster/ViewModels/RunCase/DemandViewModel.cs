using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using openCaseMaster.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;

namespace openCaseMaster.ViewModels
{
    public class DemandViewModel
    {
        public DemandViewModel(int id)
        {
            this.ID = id;
            QCTESTEntities QC_DB = new QCTESTEntities();
            Scenes = new List<DemandSceneModel>();

            var dm = (from t in QC_DB.M_testDemand
                      where t.ID == ID
                      select t).First();

            name = dm.name;
            apkName = dm.apkName;

            var rc = dm.M_runScene.ToList();

            foreach (M_runScene r in rc)
            {

                DemandSceneModel ds = new DemandSceneModel();
                ds.ID = r.ID;
                ds.name = r.name;
                ds.installResult = r.installResult;
                ds.DeviceName = r.M_deviceConfig.mark;

                var TotalCase = new ObjectParameter("Out1", DbType.Int32);
                var sucess = new ObjectParameter("Out2", DbType.Int32);
                var error = new ObjectParameter("Out3", DbType.Int32);
                var noRun = new ObjectParameter("Out4", DbType.Int32);
                var Untreated = new ObjectParameter("Out5", DbType.Int32);

                QC_DB.M_runScene_result(r.ID, TotalCase, sucess, error, noRun, Untreated);

                ds.TotalCase = Convert.ToInt32(TotalCase.Value);
                ds.sucess = Convert.ToInt32(sucess.Value);
                ds.error = Convert.ToInt32(error.Value);
                ds.noRun = Convert.ToInt32(noRun.Value);
                ds.Untreated = Convert.ToInt32(Untreated.Value);
                ds.installResult = r.installResult;
                Scenes.Add(ds);
            }

        }
        public int ID { get; set; }

        public string name { get; set; }

        public string apkName { get; set; }

        public List<DemandSceneModel> Scenes { get; set; }

        public string getScenesJsonData
        {
            get
            {


                string rows = JsonConvert.SerializeObject(this.Scenes);
               


    
                DemandSceneModel ds = new DemandSceneModel();
                foreach (var s in Scenes)
                {
                    ds.TotalCase += s.TotalCase;
                    ds.sucess += s.sucess;
                    ds.error += s.error;
                    ds.noRun += s.noRun;
                    ds.Untreated += s.Untreated;
                }
                ds.name = "总计";

                var jSetting = new JsonSerializerSettings();
                jSetting.NullValueHandling = NullValueHandling.Ignore;
                string footer = JsonConvert.SerializeObject(ds, jSetting);



                return "{\"rows\":" + rows + ",\"footer\":[" + footer + "]}";
            }
        }

    }


    public class DemandSceneModel
    {
        public int ID { get; set; }

        /// <summary>
        /// 场景名
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 设备名
        /// </summary>
        public string DeviceName { get; set; }


        /// <summary>
        /// 案例总数
        /// </summary>
        public int TotalCase { get; set; }

        /// <summary>
        /// 成功
        /// </summary>
        public int sucess { get; set; }

        /// <summary>
        /// 失败
        /// </summary>
        public int error { get; set; }

        /// <summary>
        /// 未执行
        /// </summary>
        public int noRun { get; set; }

        /// <summary>
        /// 未处理
        /// </summary>
        public int Untreated { get; set; }

        /// <summary>
        /// 安装信息
        /// </summary>
        public string installResult { get; set; }


        /// <summary>
        /// 成功率
        /// </summary>
        public float SuccessRatio
        {
            get
            {
                return sucess / (float)TotalCase;

            }
        }

       
    }

    
     
}