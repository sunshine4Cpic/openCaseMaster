using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using openCaseMaster.Models;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace openCaseMaster.ViewModels
{
    public class controlViewModel
    {
        public controlViewModel(int id)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();

            userStep = QC_DB.M_testCaseSteps.First(t => t.ID == id);

            XElement xe = XElement.Parse(userStep.stepXML);

            var tcl =  xe.getScriptTreeList();

            var jSetting = new JsonSerializerSettings();
            jSetting.NullValueHandling = NullValueHandling.Ignore;

            this.viewJson = JsonConvert.SerializeObject(tcl, jSetting);

           

        }
        public M_testCaseSteps userStep  { get; set; }

        public string viewJson { get; set; }

    }
}