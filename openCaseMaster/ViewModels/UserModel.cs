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
        [StringLength(50, MinimumLength = 4, ErrorMessage = "UserName length should be between 4 and 20")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "UserName length should be between 4 and 20")]
        [Display(Name = "密码")]
        public string Password { get; set; }

       
    }

    public enum UserStatus
    {
        Admin,
        User,
        invalidUser
    }

}