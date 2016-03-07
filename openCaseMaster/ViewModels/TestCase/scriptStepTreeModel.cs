using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace openCaseMaster.ViewModels
{
    
    [DataContract]
    public class scriptStepTreeModel:treeViewModel
    {
        /// <summary>
        /// step名
        /// </summary>
        [DataMember]
        public string name { get; set; }

        /// <summary>
        /// 框架ID
        /// </summary>
        [DataMember]
        public int? FID { get; set; }

        /// <summary>
        /// 项目ID
        /// </summary>
        [DataMember]
        public int? PID { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string desc { get; set; }

        [DataMember]
        public new List<scriptStepAttrModel> children { get; set; }

    }

    public class scriptStepAttrModel : treeViewModel
    {
        
        [DataMember]
        public string Key { get; set; }
        [DataMember]
        public string Value { get; set; }
    }
}