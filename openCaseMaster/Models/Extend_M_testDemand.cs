using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace openCaseMaster.Models
{
    public static class Extend_M_testDemand
    {

        public static int noRunCaseNum(this M_testDemand td)
        {


            QCTESTEntities QC_DB = new QCTESTEntities();
            return QC_DB.M_runTestCase.Where(t => t.M_runScene.M_testDemand.ID == td.ID && t.state == null).Count();

        }



        public static bool isRunOK(this M_testDemand td)
        {

            if (td.isRun == true && td.noRunCaseNum() == 0)
                return true;
            else
                return false;

        }
       
    }
}