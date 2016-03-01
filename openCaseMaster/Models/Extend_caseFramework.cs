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
        public static treeViewModel getControlJson4Tree(this caseFramework cf)
        {
            XElement xe = XElement.Parse(cf.controlXML);
           

            var root = new treeViewModel();
            root.text = cf.workName;
            root.id = cf.ID;

           
            var sms = xe.Descendants("Step");
            root.children = new List<treeViewModel>();
            foreach (var e in sms)
            {
                caseStepTreeModel tv = new caseStepTreeModel();
               
              
                tv.state = "open";
                tv.name = e.Attribute("name").Value;//name肯定有把....
                if (e.Attribute("desc")!=null)
                    tv.desc = e.Attribute("desc").Value;
                root.children.Add(tv);
               
            }



            return root;

        }


       
        


        

    }
}