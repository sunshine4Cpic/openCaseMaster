using openCaseMaster.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace openCaseMaster.ViewModels
{
    public class SceneCaseDataModel
    {
        public SceneCaseDataModel(int id, int page, int rows)
        {
            QCTESTEntities QC_DB = new QCTESTEntities();
            var mtcs = from t in QC_DB.M_runTestCase
                       where t.sceneID == id
                       orderby t.ID
                       select t;
            this.total = mtcs.Count();
            var rts = mtcs.Skip(rows * (page - 1)).Take(rows);
            this.rows = new List<CaseDataModel>();
            foreach (var r in rts)
            {
                this.rows.Add(new CaseDataModel(r));
            }

        }
        public int total { get; set; }

        public List<CaseDataModel> rows { get; set; }
    }


    public class CaseDataModel
    {
        public CaseDataModel(M_runTestCase r)
        {
            this.ID = r.ID;
            this.name = r.name;
            this.mark = r.mark;
            if (r.state == null || r.state == 0)
            {
                this.state = "未执行";
            }
            else
            {
                this.state = "执行完成";
                if (r.resultXML == null) //未知错误
                {
                    this.result = "失败";
                }
                else
                {
                    var xe = XElement.Parse(r.resultXML);
                    int stepCnt = xe.Descendants("Step").Count();
                    int stepRS1Cnt = (from t in xe.Descendants("Step")
                                      where t.Attribute("ResultStatic") != null && t.Attribute("ResultStatic").Value == "1"
                                      select t).Count();
                    if (stepCnt == stepRS1Cnt)
                    {
                        this.result = "成功";
                    }
                    else
                    {
                        this.result = "失败";
                    }

                    if (xe.Attribute("flow") != null)
                        this.flow = xe.Attribute("flow").Value;

                }
                if (r.state == 2)//手工确认
                {
                    this.result = "确认";

                }


            }

            this.runDate = r.startDate.ToString() + " ~ " + r.endDate.ToString();
         
            

        }
        public int ID { get; set; }

        public string name { get; set; }

        public string state { get; set; }

        public string result { get; set; }

        public string flow { get; set; }

        public string runDate { get; set; }

        public string mark { get; set; }

    }
}