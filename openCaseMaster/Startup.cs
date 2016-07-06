using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using openCaseMaster.SignalR;
using Microsoft.AspNet.SignalR;

[assembly: OwinStartupAttribute(typeof(openCaseMaster.Startup))]

namespace openCaseMaster
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //推送相关
            var userIdProvider = new MyUserFactory();
            GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => userIdProvider);

            app.MapSignalR();

            //用户权限相关
            ConfigureAuth(app);
        }
    }
}
