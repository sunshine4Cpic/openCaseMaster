using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace openCaseMaster.ViewModels
{
    [DataContract]
    public class treeViewModel
    {
        [DataMember]
        /// <summary>
        /// 节点的 id，它对于加载远程数据很重要。
        /// </summary>
        public int? id { get; set; }
        [DataMember]
        /// <summary>
        /// 要显示的节点文本。
        /// </summary>
        public string text { get; set; }
        [DataMember]
        /// <summary>
        /// 节点状态，'open' 或 'closed'，默认是 'open'。当设置为 'closed' 时，该节点有子节点，并且将从远程站点加载它们。
        /// </summary>
        public string state { get; set; }

        [DataMember(Name = "checked")]
        /// <summary>
        /// 指示节点是否被选中。
        /// </summary>
        public Boolean? ischecked { get; set; }

        [DataMember]
        /// <summary>
        /// 节点图标。
        /// </summary>
        public string iconCls { get; set; }

        [DataMember]
        public List<treeViewModel> children { get; set; }



    }



    


}