using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;


namespace openCaseMaster.ViewModels
{

    public class taskModel
    {
      
        [Required]
        public int appID { get; set; }

        public DateTime? startDate { get; set; }

        public DateTime? endDate { get; set; }

 
        [Required]
        [StringLength(50, ErrorMessage = "不能大于50个字符")]
        public string title { get; set; }

        [Required]
        [StringLength(2000, ErrorMessage = "不能超过2000个字符")]
        public string body { get; set; }

        [Required]
        public string scripts { get; set; }
    }
}