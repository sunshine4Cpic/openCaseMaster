using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace openCaseMaster.ViewModels
{
    
    [DataContract]
    public class caseStepModel:treeViewModel
    {

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string desc { get; set; }

        [DataMember]
        public new List<caseStepAttrModel> children { get; set; }

    }

    public class caseStepAttrModel : treeViewModel
    {
        
        [DataMember]
        public string Key { get; set; }
        [DataMember]
        public string Value { get; set; }
    }
}