using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace openCaseMaster.Models
{
    public class userHelper
    {
        


        /// <summary>
        /// 获得有权限的项目
        /// </summary>
        public static IQueryable<project> getPermissionsProject()
        {
            int userID = HttpContext.Current.User.userID();
            QCTESTEntities QC_DB = new QCTESTEntities();

  
            var pp =  HttpContext.Current.User.Permission();


            if (HttpContext.Current.User.IsInRole("admin"))
            {
                return QC_DB.project.Where(t => t.userID == null || t.userID == userID);
            }
            else
            {
                return QC_DB.project.Where(t => pp.Contains(t.ID) || t.userID == userID);
            }
        }

        public static bool isAdmin
        {
            get { return HttpContext.Current.User.IsInRole("admin"); }
        }

        /// <summary>
        /// 获取基础框架,包括自动义
        /// </summary>
        /// <returns></returns>
        public static List<caseFramework> getBaseFrameworks()
        {

            int userID = HttpContext.Current.User.userID();
            List<caseFramework> cfs = frameworkHelp.getAutoFramework();


            QCTESTEntities QC_DB = new QCTESTEntities();

            caseFramework cf = QC_DB.caseFramework.FirstOrDefault(t => t.userID == userID);
            if (cf != null) cfs.Add(cf);

            return cfs;
        }


        /// <summary>
        /// 获取可编辑的app
        /// </summary>
        /// <returns></returns>
        public static IQueryable<M_application> getApps()
        {
            QCTESTEntities QC_DB = new QCTESTEntities();

            IQueryable<M_application> apps;

            if (userHelper.isAdmin)
            {
                apps = QC_DB.M_application;
                
            }
            else
            {

                int[] pjs = HttpContext.Current.User.Permission();



                apps = from t in QC_DB.project_app
                          where t.usable && pjs.Contains(t.PID)
                          select t.M_application;

               
            }

            return apps;
        }

        

        
       
    }
}