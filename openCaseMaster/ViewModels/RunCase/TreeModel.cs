using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace openCaseMaster.ViewModels
{
    [DataContract]
    public class testDemandTree :treeViewModel
    {
        [DataMember]
        public int DemandID { get; set; }

        [DataMember]
        public int type { get; set; }
    }
}