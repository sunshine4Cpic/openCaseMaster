using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace openCaseMaster.ViewModels
{
    public class frameTreeNode : treeViewModel
    {

        public frameTreeNode()
        {
            state = "closed";
            iconCls = "icon-star_boxed_full";
            children = new List<treeViewModel>();
        }
        //框架ID
        [DataMember]
        public int FID { get; set; }

        
    }
}