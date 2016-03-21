using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace openCaseMaster.Models
{
    public class userHelper
    {

        /// <summary>
        /// 获得ID
        /// </summary>
        /// <returns></returns>
        public static int getUserID()
        {

            FormsIdentity id = (FormsIdentity)HttpContext.Current.User.Identity;
            var userData = id.Ticket.UserData;//cookie
            
            JObject userJ = JObject.Parse(userData);
            return Convert.ToInt32(userJ["ID"]);
        }

        /// <summary>
        /// 获得项目权限
        /// </summary>
        public static int[] getUserPermission()
        {
            FormsIdentity id = (FormsIdentity)HttpContext.Current.User.Identity;
            var userData = id.Ticket.UserData;//cookie

            JObject userJ = JObject.Parse(userData);
            string[] PP = userJ["Permission"].ToString().TrimEnd(',').Split(',');

            return Array.ConvertAll<string, int>(PP, s => int.Parse(s));
        }

        public static bool isAdmin()
        {
            FormsIdentity id = (FormsIdentity)HttpContext.Current.User.Identity;
            var userData = id.Ticket.UserData;//cookie
            JObject userJ = JObject.Parse(userData);
            var Roles =  userJ["Roles"].ToString().Split(',');
            return Roles.Contains("admin");
        }
    }
}