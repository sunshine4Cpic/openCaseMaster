using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace openCaseMaster.ViewModels
{
    public class projectTreeNode : treeViewModel
    {

        public projectTreeNode()
        {
            state = "closed";
            children = new List<treeViewModel>();
        }
        
        [DataMember]
        public int PID { get; set; }
    }
}