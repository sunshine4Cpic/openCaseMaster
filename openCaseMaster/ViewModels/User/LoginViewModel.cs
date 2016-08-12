using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace openCaseMaster.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "用户名")]
        [StringLength(50, ErrorMessage = "{0} 必须必须在 {2} ~ {1} 个字符之间。", MinimumLength = 4)]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessage = "{0} 必须必须在 {2} ~ {1} 个字符之间。", MinimumLength = 4)]
        [Display(Name = "密码")]
        public string Password { get; set; }

       
    }


   


}