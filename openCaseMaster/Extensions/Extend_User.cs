using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace openCaseMaster
{
    public static class Extend_User
    {
        public static string userName(this IPrincipal User)
        {
            if (User.Identity.IsAuthenticated)
                return (User as FormUser).userName;
            return "未知";
        }

        public static int userID(this IPrincipal User)
        {
            if (User.Identity.IsAuthenticated)
                return (User as FormUser).ID;
            return -1;
        }

        /// <summary>
        /// 获得项目权限
        /// </summary>
        public static int[] Permission(this IPrincipal User)
        {


            string[] PP = (User as FormUser).Permission.TrimEnd(',').Split(',');

            return Array.ConvertAll<string, int>(PP, s => string.IsNullOrEmpty(s) ? 0 : int.Parse(s));
        }
    }
}