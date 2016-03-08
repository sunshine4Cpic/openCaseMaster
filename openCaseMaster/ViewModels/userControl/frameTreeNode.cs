using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace openCaseMaster.ViewModels
{
    public class frameTreeNode : treeViewModel
    {

        public frameTreeNode()
        {
            iconCls = "icon-star_boxed_full";
            state = "closed";
            children = new List<treeViewModel>();
        }
        //框架ID
        public int FID { get; set; }
    }
}