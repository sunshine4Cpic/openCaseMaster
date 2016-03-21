using openCaseMaster.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace openCaseMaster.ViewModels
{
    public class caseRecordModel
    {
        public List<caseRecordStep> xrs;

        public string caseName { get; set; }

        public string resultPath { get; set; }
        public caseRecordModel(int id)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();
            var mt = QC_DB.M_runTestCase.FirstOrDefault(t => t.ID == id);
            caseName = mt.name;
            resultPath = mt.resultPath;
            XElement xe;
            if (mt.resultXML != null)
                xe = XElement.Parse(mt.resultXML);
            else
                xe = XElement.Parse(mt.testXML);

            xrs = new List<caseRecordStep>();
            foreach(var x in xe.Descendants("Step"))
            {
                var rs = new caseRecordStep(x);
                if (rs.Photo!=null)
                    rs.Photo = this.resultPath + rs.Photo;
                //rs.Photo = "~/apkInstall/9ebe59c658c9c5c5ecdd8593e4025321.jpg";
                xrs.Add(rs);
            }
        }
    }

    public class caseRecordStep
    {
        public caseRecordStep(XElement xe)
        {
            this.stepX = xe;

            name = xe.Attribute("name").Value;
            desc = xe.Attribute("desc").Value;
            if (xe.Attribute("Photo") != null)
                Photo = xe.Attribute("Photo").Value;
            //Photo = "/test/1_20151103142525.jpg",
            ResultStatic = (string)xe.Attribute("ResultStatic");
            ResultMsg = (string)xe.Attribute("ResultMsg");
        }

        public XElement stepX { get; set; }


        public string desc { get; set; }

        public string Photo { get; set; }

        public string name { get; set; }

        /// <summary>
        /// 1成功
        /// </summary>
        public string ResultStatic { get; set; }
        public string ResultMsg { get; set; }
    }

}