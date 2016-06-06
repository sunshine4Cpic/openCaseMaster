using Microsoft.AspNet.SignalR;
using openCaseMaster.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace openCaseMaster.Models
{
    public class hubHelper
    {
        public static void Push(string userName)
        {
            GlobalHost.ConnectionManager.GetHubContext<UserHub>().Clients.User(userName).push(1);
        }


        public static void Push(List<string> names)
        {
            GlobalHost.ConnectionManager.GetHubContext<UserHub>().Clients.User("c_zhubo").push(1);
            //GlobalHost.ConnectionManager.GetHubContext<UserHub>().Clients.Users(names).push(1);
        }
       
    }
}