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

            var root = new treeViewModel();
            root.text = cf.project.Pname;
            root.id = cf.ID;
            root.state = "open";

            var sms = xe.Descendants("Step");
            root.children = new List<treeViewModel>();
            foreach (var e in sms)
            {
                scriptStepTreeModel tv = new scriptStepTreeModel();

                tv.FID = cf.FID;
                tv.PID = cf.PID;
                tv.state = "open";
                tv.name = e.Attribute("name").Value;//name肯定有把....

                tv.iconCls = "icon-view_outline_detail";

                if (e.Attribute("desc") != null)
                    tv.desc = e.Attribute("desc").Value;


                root.children.Add(tv);

            }



            return root;
        }
    }
}