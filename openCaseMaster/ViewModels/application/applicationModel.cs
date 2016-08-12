using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace openCaseMaster.ViewModels
{
    public class applicationModel
    {
        [Required]
        public int id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 1)]
        [Display(Name = "app名")]
        public string name { get; set; }


        [Display(Name = "robotium脚本")]
        public string runApkName { get; set; }

        [Display(Name = "安卓package")]
        public string package { get; set; }

        [Display(Name = "安卓mainActiviy")]
        public string mainActiviy { get; set; }


        [Display(Name = "ios-package")]
        public string package2 { get; set; }

        [Display(Name = "清理缓存")]
        public bool? isClear { get; set; }
    
    }

    public class applicationListModel
    {
      

        public int total { get; set; }

        public List<applicationModel> rows { get; set; }

    }
}