using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;
using openCaseMaster.Models;

namespace openCaseMaster.SignalR
{
    [Authorize]
    [HubName("UserHub")]
    public class UserHub : Hub
    {
        /// <summary>
        /// 重写Hub连接事件
        /// </summary>
        /// <returns></returns>
        public override Task OnConnected()
        {
            return base.OnConnected();
        }

        /// <summary>
        /// 重写Hub连接断开的事件
        /// </summary>
        /// <returns></returns>
        public override Task OnDisconnected(bool stopCalled)
        {
            
            return base.OnDisconnected(stopCalled);
        }

        public void Push()
        {
            int userID = HttpContext.Current.User.userID();
            QCTESTEntities db = new QCTESTEntities();
            int cnt = db.notification.Where(t => t.userID == userID).Where(t => t.state == 0).Count();
            if (cnt > 0)
                Clients.User(HttpContext.Current.User.userName()).push(cnt);
            
        }

      
    }


    public class MyUserFactory : IUserIdProvider
    {

        public string GetUserId(IRequest request)
        {
            string userName = HttpContext.Current.User.userName();
            return userName;
        }
    }
}