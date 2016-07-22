using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace openCaseApi.Extensions
{
    public static class UserIdentity
    {

        public static int userID(this System.Security.Principal.IIdentity _id)
        {


            var cm = (_id as ClaimsIdentity).FindFirst(ClaimTypes.NameIdentifier);


            int ID = Convert.ToInt32(cm.Value);

            return ID;
        }

        public static string userName(this System.Security.Principal.IIdentity _id)
        {
            var cm = (_id as ClaimsIdentity).FindFirst("userName");


            return cm.Value;

            
        }


    }
}