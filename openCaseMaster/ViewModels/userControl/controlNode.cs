using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace openCaseMaster.ViewModels
{
    public class controlNode : treeViewModel
    {
        public controlNode()
        {
            iconCls = "icon-view_outline_detail";
           
            state = "open";
        }
        [DataMember]
        public int? FID { get; set; }

        [DataMember]
        public int? PID { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string desc { get; set; }
    }
}