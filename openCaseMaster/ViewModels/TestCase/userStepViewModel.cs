using openCaseMaster.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace openCaseMaster.ViewModels
{
    public class userStepViewModel
    {
        /// <summary>
        /// 显示的treedata
        /// </summary>
        public string TreeJson { get; set; }

        /// <summary>
        /// 用户脚本id
        /// </summary>
        public int id { get; set; }


        public userStepViewModel(int id)
        {
            
            using (QCTESTEntities QC_DB = new QCTESTEntities())
            {
                int userID = HttpContext.Current.User.userID();
                var us = QC_DB.M_testCaseSteps.FirstOrDefault(t => t.ID == id && t.userID == userID);


                         
                        

            }
        }
    }
}