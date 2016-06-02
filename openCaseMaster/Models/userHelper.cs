﻿using Newtonsoft.Json.Linq;
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
        /// 获得ID
        /// </summary>
        /// <returns></returns>
        public static int UserID
        {
            get
            {
                if (HttpContext.Current.User != null)
                {
                    if (HttpContext.Current.User.Identity.IsAuthenticated)
                    {
                        FormsIdentity id = (FormsIdentity)HttpContext.Current.User.Identity;
                        var userData = id.Ticket.UserData;//cookie

                        JObject userJ = JObject.Parse(userData);
                        return Convert.ToInt32(userJ["ID"]);
                    }
                }
                return -1;
            }
        }

        /// <summary>
        /// 获得用户名
        /// </summary>
        /// <returns></returns>
        public static string UserName
        {
            get
            {
                if (HttpContext.Current.User != null)
                {
                    if (HttpContext.Current.User.Identity.IsAuthenticated)
                    {
                        return HttpContext.Current.User.Identity.Name;
                       
                    }
                }
                return "null";
            }
        }

        /// <summary>
        /// 获得项目权限
        /// </summary>
        private static int[] getUserPermission()
        {
            FormsIdentity id = (FormsIdentity)HttpContext.Current.User.Identity;
            var userData = id.Ticket.UserData;//cookie

            JObject userJ = JObject.Parse(userData);

            string[] PP = userJ["Permission"].ToString().TrimEnd(',').Split(',');

            return Array.ConvertAll<string, int>(PP, s => string.IsNullOrEmpty(s) ? 0 : int.Parse(s));
        }


        /// <summary>
        /// 获得有权限的项目
        /// </summary>
        public static IQueryable<project> getPermissionsProject()
        {
            int userID = UserID;
            QCTESTEntities QC_DB = new QCTESTEntities();

           
            var pp = getUserPermission();


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

            int userID = UserID;
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

                int[] pjs = userHelper.getUserPermission();



                apps = from t in QC_DB.project_app
                          where t.usable && pjs.Contains(t.PID)
                          select t.M_application;

               
            }

            return apps;
        }

        

        
       
    }
}