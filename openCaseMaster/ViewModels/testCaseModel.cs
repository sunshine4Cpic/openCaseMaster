using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace openCaseMaster.ViewModels
{
    [DataContract]
    public class testCaseModel : treeViewModel
    {
        [DataMember]
        public int type { get; set; }
    }



}