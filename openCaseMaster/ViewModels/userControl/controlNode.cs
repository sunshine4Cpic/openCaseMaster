using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace openCaseMaster.ViewModels
{
    public class controlNode : treeViewModel
    {
        public controlNode()
        {
            iconCls = "icon-star_empty";
            state = "open";
        }
        public int FID { get; set; }
    }
}