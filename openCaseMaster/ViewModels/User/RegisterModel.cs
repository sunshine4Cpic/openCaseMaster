using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace openCaseMaster.ViewModels
{
    public class RegisterModel
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9_]*$", ErrorMessage = "只能使用英文和数字")]
        [StringLength(50, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 4)]
        [Display(Name = "帐号")]
        public string UserName { get; set; }


        [StringLength(50, ErrorMessage = "不能大于50个字符")]
        [Display(Name = "名字")]
        public string Name { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; }
    }
}