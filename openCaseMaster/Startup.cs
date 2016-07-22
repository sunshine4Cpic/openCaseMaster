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
           

            //用户权限相关
            ConfigureAuth(app);

            //推送相关 必须放在权限处理后面 不然报错
            var userIdProvider = new MyUserFactory();
            GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => userIdProvider);

            app.MapSignalR();
        }
    }
}
