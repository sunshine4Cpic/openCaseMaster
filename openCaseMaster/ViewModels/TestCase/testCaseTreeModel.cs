using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace openCaseMaster.ViewModels
{
    /// <summary>
    /// 案例tree绑定用
    /// </summary>
    [DataContract]
    public class testCaseTreeModel : treeViewModel
    {
        [DataMember]
        public int type { get; set; }
    }



}