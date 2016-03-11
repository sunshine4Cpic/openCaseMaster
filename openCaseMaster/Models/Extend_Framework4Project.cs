using openCaseMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace openCaseMaster.Models
{
    public static class Extend_Framework4Project
    {
        public static treeViewModel getControlJson4Tree(this Framework4Project cf)
        {
            XElement xe = XElement.Parse(cf.controlXML);    


            scriptStepTreeModel tv = new scriptStepTreeModel();

            tv.FID = cf.FID;
            tv.PID = cf.PID;
            tv.state = "open";
            tv.name = "prostep_" + cf.ID;

            tv.iconCls = "icon-view_outline_detail";

            if (xe.Attribute("desc") != null)
                tv.desc = xe.Attribute("desc").Value;


            return tv;
        }
    }
}