using Newtonsoft.Json.Linq;
using openCaseMaster.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace openCaseMaster.Models
{
    public static class Extend_caseFramework
    {
        /// <summary>
        /// 框架xml转换成前台显示的treeStep
        /// </summary>
        /// <param name="cf"></param>
        /// <returns></returns>
        public static treeViewModel getControlJson4Tree(this caseFramework cf)
        {
            

            var root = new frameTreeNode();
            root.text = cf.workName;
            root.state = "open";
            root.FID = cf.ID;

            root.children = new List<treeViewModel>();

            if (cf.controlXML != null)
            {
                XElement xe = XElement.Parse(cf.controlXML);
                var sms = xe.Descendants("Step");
                foreach (var e in sms)
                {
                    scriptStepTreeModel tv = new scriptStepTreeModel();

                    tv.FID = cf.ID;
                    //tv.PID = null;
                    tv.state = "open";
                    tv.name = e.Attribute("name").Value;//name肯定有把....
                    tv.iconCls = "icon-view_outline_detail";

                    if (e.Attribute("desc") != null)
                        tv.desc = e.Attribute("desc").Value;


                    root.children.Add(tv);

                }
            }



            return root;

        }


       
        


        

    }
}