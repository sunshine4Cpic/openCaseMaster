﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using openCaseMaster.Models;

namespace openCaseMaster.ViewModels
{
    public class ChangePasswordModel
    {
        [Required]
        [StringLength(50, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 4)]
        [Display(Name = "旧密码")]
        public string currentPassword { get; set; }


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
    public class userInfoModel
    {
        public userInfoModel()
        {
            QCTESTEntities QC_DB = new QCTESTEntities();

            int userID = userHelper.UserID;

            this.Avatar = QC_DB.admin_user.First(t => t.ID == userID).Avatar;

            /*
            var fm = QC_DB.caseFramework.FirstOrDefault(t=>t.userID==userID);
            if (fm != null)
                this.framework = XElement.Parse( fm.controlXML).ToString();
            else
                this.framework = "";
             */
        }
        public string framework { get; set; }

        public string Avatar { get; set; }
    }

    
}