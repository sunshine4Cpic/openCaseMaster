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
        public static string getControlJson4Tree(this caseFramework cf)
        {
            XElement xe = XElement.Parse(cf.controlXML);

          

            var sms = xe.Descendants("Step");

            foreach (var e in sms)
            {

                treeViewModel tv = new treeViewModel();
                tv.state = "closed";
                tv.iconCls = "icon-view_outline_detail";

                var atts = from t in e.Elements()
                           select new treeViewModel
                           {
                               state = "open",
                               iconCls = "icon-spanner_blue",
                               checkbox = false
                           };
                tv.children = atts.ToList();
                //tv.Add(tmp);
            }



            return null;

        }


       
        


        

    }
}