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
    public class SceneViewModel
    {
        public SceneViewModel(int ID)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();
            var sc = QC_DB.M_runScene.First(t => t.ID == ID);
            this.ID = sc.ID;
            this.name = sc.M_testDemand.name + "/" + sc.name;
            if(sc.deviceID!=null)
                this.DeviceName = sc.M_deviceConfig.mark;
        }
        public int ID { get; set; }

        /// <summary>
        /// 场景名
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 设备名
        /// </summary>
        public string DeviceName { get; set; }

    }


    
     
}