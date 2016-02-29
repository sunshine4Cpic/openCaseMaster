using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace openCaseMaster.ViewModels
{
    public class NewCaseModel
    {
        /// <summary>
        /// 0目录 1案例
        /// </summary>
        public int type { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "最大不超过50个字符")]
        public string Name { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "最大不超过50个字符")]
        public string mark { get; set; }

        /// <summary>
        /// 项目ID
        /// </summary>
        public int? projectID { get; set; }

        /// <summary>
        /// 根目录
        /// </summary>
        public int? baseID { get; set; }
    }
}